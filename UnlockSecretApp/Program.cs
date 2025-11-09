using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);

// Токен бота
string token = "8431829253:AAGNLz7LW9Yy7fQ8Qi6ctP2LoDsz9L9oyA0";
var botClient = new TelegramBotClient(token);

var app = builder.Build();

// Для проверки, что приложение запущено
app.MapGet("/", () => "Bot is running!");

// Запуск бота в фоне
using var cts = new CancellationTokenSource();
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    cancellationToken: cts.Token
);

Console.WriteLine("Bot started");

// --- Методы для обработки сообщений ---
async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
{
    if (update.Type == UpdateType.Message && update.Message!.Text != null)
    {
        if (update.Message.Text.ToLower() == "/start")
        {
            // В новой версии SendMessage вместо SendTextMessageAsync
            await bot.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "Привет! Нажми кнопку ниже, чтобы открыть подарок 🎁",
                cancellationToken: token
            );
        }
    }
}

Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
{
    Console.WriteLine(exception.Message);
    return Task.CompletedTask;
}

// Настройка маршрутов и HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
