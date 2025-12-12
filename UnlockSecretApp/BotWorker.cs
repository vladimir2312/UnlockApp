using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types.ReplyMarkups;

public class BotWorker : BackgroundService
{
    private readonly TelegramBotClient _botClient;
    private readonly string _webAppUrl = "https://unlockapp-11212.onrender.com/";

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
            // Создаем клавиатуру с кнопками (reply_markup)
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                // Первый ряд - ОСНОВНАЯ кнопка с открытием в браузере
                new[]
                {
                    InlineKeyboardButton.WithUrl(
                        "🎁 Открыть поздравление",
                        _webAppUrl
                    )
                },
                
                // Третий ряд - дополнительные кнопки
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
                      "Я подготовил для тебя особенное поздравление...\n" 
                      
                      ,
                parseMode: ParseMode.Html,
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

                // Создаем клавиатуру для сообщения с помощью
                var helpKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl(
                            "🎁 Открыть в браузере ",
                            _webAppUrl
                        )
                    },
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
                            "↩️ Назад к выбору",
                            "back_to_start"
                        )
                    }
                });

                // Отправляем сообщение с помощью
                await bot.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "❓ <b>Как лучше открыть :</b>\n\n" +
                          "🚀 <b>Рекомендуемый способ:</b>\n" +
                          "1. Нажми <b>\"🎁 Открыть в браузере\"</b>\n" +
                          "2. Приложение откроется в Chrome/Safari\n" +
                        

                        

                          "💡 <b>Советы для браузера:</b>\n" +
                          "• Поверни телефон горизонтально 📲\n" +
                        

                          "🔧 <b>Если что-то не работает:</b>\n" +
                          "• Перезапусти бота командой /start\n" +
                          "• Напиши автору: @dinoZaViK",
                    parseMode: ParseMode.Html,
                    replyMarkup: helpKeyboard,
                    cancellationToken: token
                );
            }
            else if (callbackQuery.Data == "back_to_start")
            {
                await bot.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: token);

                // Возвращаемся к начальному сообщению
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithUrl(
                            "🎁 Открыть поздравление",
                            _webAppUrl
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
                      "Я подготовил для тебя особенное поздравление...\n",
                    parseMode: ParseMode.Html,
                    replyMarkup: inlineKeyboard,
                    cancellationToken: token
                );
            }
        }

        // Обработка команды /help в текстовом виде
        if (update.Type == UpdateType.Message && update.Message?.Text == "/help")
        {
            var helpKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithUrl(
                        "🎁 Открыть в браузере",
                        _webAppUrl
                    )
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "ℹ️ Подробная помощь",
                        "help"
                    )
                }
            });

         
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