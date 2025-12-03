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
        Console.WriteLine("🤖 Бот запущен");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        // Обработка команды /start - показываем кнопки
        if (update.Type == UpdateType.Message && update.Message?.Text == "/start")
        {
            var webAppUrl = "https://unlockapp-11212.onrender.com/";

            // Создаем клавиатуру с кнопками (reply_markup)
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                // Первый ряд - основная кнопка WebApp
                new[]
                {
                    InlineKeyboardButton.WithWebApp(
                        "🎁 Открыть подарок",
                        new WebAppInfo(webAppUrl)
                    )
                },
                // Второй ряд - дополнительные кнопки
                new[]
                {
                    InlineKeyboardButton.WithUrl(
                        "💬 Связь с автором",
                        "https://t.me/dinoZaViK"
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        "❓ Помощь",
                        "help"
                    )
                }
            });

            // Отправляем сообщение с HTML-разметкой и кнопками
            await bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "🎂 <b>С днём рождения, любимая!</b> 💖\n\n" +
                      "Я подготовил для тебя особенный подарок...\n" +
                      "Это цифровой сюрприз с нашими воспоминаниями ✨\n\n" +
                      "<i>Нажми на кнопку ниже, чтобы начать...</i>",
                parseMode: ParseMode.Html, // Включаем HTML для красивого текста
                replyMarkup: inlineKeyboard,
                cancellationToken: token
            );
        }

        // Обработка нажатий на кнопку "Помощь" (callback_data)
        if (update.Type == UpdateType.CallbackQuery)
        {
            var callbackQuery = update.CallbackQuery;

            if (callbackQuery.Data == "help")
            {
                // Отвечаем на callback чтобы убрать "часики" на кнопке
                await bot.AnswerCallbackQueryAsync(
                    callbackQueryId: callbackQuery.Id,
                    text: "Открываю помощь...",
                    cancellationToken: token
                );

                // Отправляем сообщение с помощью
                await bot.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "❓ <b>Как открыть подарок:</b>\n\n" +
                          "1. Нажми кнопку <b>\"🎁 Открыть подарок\"</b>\n" +
                          "2. Приложение откроется прямо в Telegram\n" +
                          "3. Следуй инструкциям внутри\n\n" +
                          "Если что-то не работает:\n" +
                          "• Обнови Telegram\n" +
                          "• Перезапусти бота командой /start\n" +
                          "• Напиши автору: @dinoZaViK",
                    parseMode: ParseMode.Html,
                    replyMarkup: new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithUrl(
                                "💬 Написать автору",
                                "https://t.me/dinoZaViK"
                            )
                        },
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData(
                                "↩️ Назад",
                                "back_to_start"
                            )
                        }
                    }),
                    cancellationToken: token
                );
            }
            else if (callbackQuery.Data == "back_to_start")
            {
                await bot.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: token);

                // Возвращаемся к начальному сообщению
                var webAppUrl = "https://unlockapp-11212.onrender.com/";
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
                        InlineKeyboardButton.WithUrl(
                            "💬 Связь с автором",
                            "https://t.me/dinoZaViK"
                        ),
                        InlineKeyboardButton.WithCallbackData(
                            "❓ Помощь",
                            "help"
                        )
                    }
                });

                // Редактируем текущее сообщение вместо отправки нового
                await bot.EditMessageTextAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    messageId: callbackQuery.Message.MessageId,
                    text: "🎂 <b>С днём рождения, любимая!</b> 💖\n\n" +
                          "Я подготовил для тебя особенный подарок...\n" +
                          "Это цифровой сюрприз с нашими воспоминаниями ✨\n\n" +
                          "<i>Нажми на кнопку ниже, чтобы начать...</i>",
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token
                );
            }
        }

        // Обработка команды /help в текстовом виде
        if (update.Type == UpdateType.Message && update.Message?.Text == "/help")
        {
            await bot.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Напиши /start чтобы открыть подарок с кнопками! 🎁",
                cancellationToken: token
            );
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        // Игнорируем ошибку 409 (конфликт нескольких экземпляров бота)
        if (exception.Message.Contains("409"))
        {
            Console.WriteLine("⚠️ Telegram API: Другой экземпляр бота уже запущен");
            return Task.CompletedTask;
        }

        Console.WriteLine($"❌ Ошибка бота: {exception.Message}");
        return Task.CompletedTask;
    }
}
