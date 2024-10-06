using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using Telegram.Bot.Types.Enums;

var builder = BotApplication.CreateBuilder(args);
var bot = builder.Build();

bot.HandleCommand("/start", () => "Hello World!");

bot.HandleUpdateType(UpdateType.Message, (string messageText) => messageText);

bot.Run();
