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
        Console.WriteLine("🤖 Бот запущен и готов дарить подарки!");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        // Обработка команды /start
        if (update.Type == UpdateType.Message && update.Message?.Text == "/start")
        {
            var webAppUrl = "https://unlockapp-11212.onrender.com/"; // Ваш WebApp

            // Создаем клавиатуру с кнопкой WebApp
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
                    InlineKeyboardButton.WithCallbackData("💌 Что внутри?", "about"),
                    InlineKeyboardButton.WithCallbackData("❓ Помощь", "help")
                }
            });

            // Отправляем красивое сообщение
            await bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "🎂 <b>С днём рождения, моя любимая!</b> 💖\n\n" +
                      "Я подготовил для тебя особенный сюрприз...\n" +
                      "Это не просто подарок, а целое путешествие\n" +
                      "по нашим воспоминаниям и мечтам ✨\n\n" +
                      "<i>Нажми на кнопку ниже, чтобы начать...</i>",
                parseMode: ParseMode.Html,
                replyMarkup: inlineKeyboard
            );
        }

        // Обработка нажатий на callback кнопки
        if (update.Type == UpdateType.CallbackQuery)
        {
            var callback = update.CallbackQuery;

            if (callback.Data == "about")
            {
                await bot.AnswerCallbackQueryAsync(callback.Id);
                await bot.SendTextMessageAsync(
                    chatId: callback.Message.Chat.Id,
                    text: "✨ <b>Внутри подарка:</b>\n\n" +
                          "• 🔐 Секретный код с нашей историей\n" +
                          "• 💫 Твои самые прекрасные качества\n" +
                          "• 💌 Письма в наше будущее\n" +
                          "• 📸 Галерея наших моментов\n" +
                          "• 🎀 И много маленьких сюрпризов!\n\n" +
                          "<i>Открой и узнаешь всё остальное...</i> 💖",
                    parseMode: ParseMode.Html
                );
            }
            else if (callback.Data == "help")
            {
                await bot.AnswerCallbackQueryAsync(callback.Id);
                await bot.SendTextMessageAsync(
                    chatId: callback.Message.Chat.Id,
                    text: "❓ <b>Помощь:</b>\n\n" +
                          "Просто нажми кнопку \"🎁 Открыть подарок\"!\n" +
                          "Приложение откроется прямо в Telegram.\n\n" +
                          "Если что-то не работает, попробуй:\n" +
                          "1. Обновить Telegram\n" +
                          "2. Перезагрузить бота\n" +
                          "3. Написать разработчику 💌",
                    parseMode: ParseMode.Html
                );
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"❌ Ошибка бота: {exception.Message}");
        return Task.CompletedTask;
    }
}