using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;

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
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            if (update.Message.Text.ToLower() == "/start")
            {
                await bot.SendMessage(
                    update.Message.Chat.Id,
                    "Привет! Нажми кнопку ниже, чтобы открыть подарок 🎁",
                    cancellationToken: token
                );
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}
