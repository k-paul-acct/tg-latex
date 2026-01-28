using LatexView.Bot.Data;
using Microsoft.EntityFrameworkCore;
using MinimalTelegramBot;
using MinimalTelegramBot.Builder;
using MinimalTelegramBot.Handling;
using MinimalTelegramBot.Handling.Filters;
using Refit;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var builder = BotApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddRefitClient<ILatexCompilerApi>()
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["LatexCompilerApi:BaseAddress"]!);
        client.Timeout = TimeSpan.FromMinutes(5);
    });

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CompilerService>();

var bot = builder.Build();

using (var scope = bot.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

bot.HandleCommand("/start", async (Update update, UserService userService) =>
{
    var user = update.Message!.From!;
    await userService.EnsureCreated(user.Id, user.Username, user.FirstName, user.LastName);
    return
        """
        Это бот для компиляции LaTeX-кода и проектов.

        Отправьте мне:
        1. Код LaTeX. Это может быть формула (заключённая в `$`), просто текст (для вставки в `\begin{document}`) или контент всего `.tex`-файла.
        2. Файл с расширением `.tex`.
        3. Ссылку на git-репозиторий с проектом LaTeX.

        И я скомпилирую это в PDF или проведу конвертацию в изображение (для формул).

        Также я умею оптимизировать PDF. Для этого просто отправьте мне файл для оптимизации.

        Все данные в безопасности: контент удаляется сразу после завершения работы с ним.
        """;
});

var compileRateLimiter = new RateLimiter(6, TimeSpan.FromSeconds(60));

bot.HandleUpdateType(UpdateType.Message, async (BotRequestContext context) =>
{
    var messageId = context.Update.Message!.Id;
    var (text, keybard) = GetCompileSelectOptionsMenu();
    await context.Client.SendMessage(context.ChatId, text, ParseMode.None, messageId, keybard);
}).Filter(context => context.BotRequestContext.MessageText is not null);

bot.HandleCallbackData("compile_confirm", async (BotRequestContext context) =>
{
    var messageId = context.Update.CallbackQuery!.Message!.Id;
    var message = "Выбранные параметры компиляции:\n- по умолчанию";
    var keybard = new InlineKeyboardMarkup(
        [
            [InlineKeyboardButton.WithCallbackData("✅ Скомпилировать", "compile")],
            [InlineKeyboardButton.WithCallbackData("⬅️ Назад", "compile_select_options")],
        ]);
    await context.Client.EditMessageText(context.ChatId, messageId, message, replyMarkup: keybard);
});

bot.HandleCallbackData("compile_select_options", async (BotRequestContext context) =>
{
    var messageId = context.Update.CallbackQuery!.Message!.Id;
    var (text, keybard) = GetCompileSelectOptionsMenu();
    await context.Client.EditMessageText(context.ChatId, messageId, text, replyMarkup: keybard);
});

bot.HandleCallbackDataPrefix("compile", async (BotRequestContext context, CompilerService compilerService) =>
{
    var text = context.Update.CallbackQuery!.Message!.ReplyToMessage!.Text!;
    var textMessageId = context.Update.CallbackQuery!.Message!.ReplyToMessage!.Id;
    var messageId = context.Update.CallbackQuery!.Message!.Id;
    using var result = await compilerService.Compile(context.ChatId, textMessageId, text);
    var inputFile = InputFile.FromStream(result.DocumentStream, result.DocumentName);
    await context.Client.EditMessageReplyMarkup(context.ChatId, messageId, replyMarkup: null);
    await context.Client.SendDocument(context.ChatId, inputFile, "✅ Успешно скомпилировано", ParseMode.None, messageId);
}).Filter(compileRateLimiter.InvokeAsync);

bot.HandleUpdateType(UpdateType.EditedMessage, async (BotRequestContext context, CompilerService compilerService) =>
{
    // TODO:
}).Filter(context => context.BotRequestContext.MessageText is not null).Filter(compileRateLimiter.InvokeAsync);

bot.Run();

static (string, InlineKeyboardMarkup) GetCompileSelectOptionsMenu()
{
    var text = "Выберите параметры для компиляции:";
    var keybard = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("По умолчанию", "compile_confirm"));
    return (text, keybard);
}
