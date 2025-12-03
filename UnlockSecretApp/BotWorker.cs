using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.ReplyMarkups;

public class BotWorker : BackgroundService
{
    private readonly TelegramBotClient _botClient;
    private readonly ILogger<BotWorker> _logger;
    private readonly HashSet<long> _processedMessages = new();
    private readonly object _lock = new();

    public BotWorker(ILogger<BotWorker> logger)
    {
        string token = "8431829253:AAGNLz7LW9Yy7fQ8Qi6ctP2LoDsz9L9oyA0";
        _botClient = new TelegramBotClient(token);
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Удаляем вебхук чтобы использовать polling
            await _botClient.DeleteWebhookAsync(cancellationToken: stoppingToken);

            // Настраиваем получение через polling с правильными параметрами
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation("🤖 Бот успешно запущен!");

            // Бесконечная задача чтобы сервис не завершался
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Ошибка при запуске бота");
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        // Проверка на дублирование сообщений
        var messageId = update.Message?.MessageId ?? update.CallbackQuery?.Message?.MessageId ?? 0;
        var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id ?? 0;

        var uniqueKey = (chatId << 32) | (uint)messageId;

        lock (_lock)
        {
            if (_processedMessages.Contains(uniqueKey))
            {
                _logger.LogDebug("Пропущено дублирующее сообщение: {MessageId}", messageId);
                return;
            }
            _processedMessages.Add(uniqueKey);

            // Очищаем старые записи каждые 1000 сообщений
            if (_processedMessages.Count > 1000)
            {
                _processedMessages.Clear();
            }
        }

        try
        {
            // Обработка команды /start
            if (update.Type == UpdateType.Message && update.Message?.Text == "/start")
            {
                _logger.LogInformation("Получен /start от пользователя: {UserId} ({Username})",
                    update.Message.From?.Id, update.Message.From?.Username);

                var webAppUrl = "https://unlockapp-11212.onrender.com/";

                // Создаем клавиатуру
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithWebApp(
                            "🎁 Открыть подарок",
                            new WebAppInfo(webAppUrl)
                        )
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithUrl("💬 Автор", "https://t.me/dinoZaViK")
                    }
                });

                // Отправляем сообщение
                await bot.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "🎂 <b>С днём рождения, любимая!</b> 💖\n\n" +
                          "Я подготовил для тебя особенный подарок...\n" +
                          "Это цифровой сюрприз с нашими воспоминаниями ✨\n\n" +
                          "<i>Нажми на кнопку ниже чтобы открыть...</i>",
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token
                );

                _logger.LogInformation("✅ Отправлен подарок для {UserId}", update.Message.From?.Id);
            }

            // Обработка команды /help
            else if (update.Type == UpdateType.Message && update.Message?.Text == "/help")
            {
                await bot.SendTextMessageAsync(
                    chatId: update.Message.Chat.Id,
                    text: "❓ <b>Помощь:</b>\n\n" +
                          "1. Напиши <b>/start</b> чтобы открыть подарок 🎁\n" +
                          "2. Нажми кнопку \"🎁 Открыть подарок\"\n" +
                          "3. Следуй инструкциям в приложении\n\n" +
                          "По вопросам пиши автору: @dinoZaViK",
                    parseMode: ParseMode.Html,
                    cancellationToken: token
                );
            }

            // Обработка любых других текстовых сообщений
            else if (update.Type == UpdateType.Message && !string.IsNullOrEmpty(update.Message?.Text))
            {
                var message = update.Message.Text.ToLower();

                if (message.Contains("спасибо") || message.Contains("благодар"))
                {
                    await bot.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "💖 Рад, что тебе нравится!\nАвтор будет рад услышать отзыв: @dinoZaViK",
                        cancellationToken: token
                    );
                }
                else if (!message.StartsWith("/"))
                {
                    await bot.SendTextMessageAsync(
                        chatId: update.Message.Chat.Id,
                        text: "✨ Напиши <b>/start</b> чтобы открыть подарок!\n" +
                              "Или свяжись с автором: @dinoZaViK",
                        parseMode: ParseMode.Html,
                        cancellationToken: token
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке сообщения от {UserId}",
                update.Message?.From?.Id ?? update.CallbackQuery?.From?.Id);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "❌ Ошибка в Telegram боте");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛑 Остановка бота...");
        await base.StopAsync(cancellationToken);
    }
}