using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.ReplyMarkups;

public class BotWorker : BackgroundService
{
    private readonly TelegramBotClient _botClient;

    public BotWorker()
    {
        string token = "8431829253:AAGNLz7LW9Yy7fQ8Qi6ctP2LoDsz9L9oyA0";

        _botClient = new TelegramBotClient(token);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            cancellationToken: stoppingToken
        );
        Console.WriteLine("Bot started");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.Type == UpdateType.Message && update.Message.Text == "/start")
        {
            var webAppUrl = "https://unlockapp-11212.onrender.com/"; // сюда вставь ссылку на твоё WebApp

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
        new[]
        {
            InlineKeyboardButton.WithWebApp("Открыть подарок 🎁", new WebAppInfo(webAppUrl))
        }
    });

            await bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Привет! Нажми кнопку ниже, чтобы открыть подарок 🎁",
                replyMarkup: inlineKeyboard
            );
        }

    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}
