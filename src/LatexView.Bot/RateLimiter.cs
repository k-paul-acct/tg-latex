using System.Collections.Concurrent;
using System.Diagnostics;
using MinimalTelegramBot.Handling.Filters;
using Results = MinimalTelegramBot.Results.Results;

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
