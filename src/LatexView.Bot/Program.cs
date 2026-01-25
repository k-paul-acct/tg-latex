using System.Collections.Concurrent;
using System.Diagnostics;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Handling.Filters;
using Telegram.Bot.Types.Enums;
using Results = MinimalTelegramBot.Results.Results;

var builder = BotApplication.CreateBuilder(args);
var bot = builder.Build();

bot.HandleCommand("/start", () => "Hello World!");

bot.HandleUpdateType(UpdateType.Message, (string messageText) => messageText)
   .Filter(new RateLimiter(6, TimeSpan.FromSeconds(60)).InvokeAsync);

bot.Run();

internal sealed class RateLimiter : IHandlerFilter
{
    private readonly long _limit;
    private readonly long _windowTicks;
    private readonly ConcurrentDictionary<long, Counter> _cache;

    public RateLimiter(int limit, TimeSpan window)
    {
        _limit = limit;
        _windowTicks = (long)(window.TotalSeconds * Stopwatch.Frequency);
        _cache = [];
    }

    public ValueTask<MinimalTelegramBot.Results.IResult> InvokeAsync(
        BotRequestFilterContext context,
        BotRequestFilterDelegate next)
    {
        var timestamp = Stopwatch.GetTimestamp();
        var updatedCounter = _cache.AddOrUpdate(
            context.BotRequestContext.ChatId,
            static (_, arg) => new Counter(arg.timestamp, 1),
            static (_, counter, arg) =>
                arg.timestamp - counter.StartTimestamp >= arg._windowTicks
                    ? new Counter(arg.timestamp, 1)
                    : new Counter(counter.StartTimestamp, counter.Count + 1),
            (timestamp, _windowTicks));

        return updatedCounter.Count > _limit
            ? ValueTask.FromResult(Results.Message("Too Many Requests"))
            : next(context);
    }

    private readonly record struct Counter(long StartTimestamp, long Count);
}
