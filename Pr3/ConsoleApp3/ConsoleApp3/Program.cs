using System;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Linq;
using Telegram.Bot.Types;
using System.Threading;
using System.Globalization;


namespace Bot
{
    class Telegram_Bot
    {
        private static string token = "6921097576:AAF579S92omOyn-8KbgEC8uLuXEQmCbeNJc";
        private static TelegramBotClient client;
        private static long chatId;
        static void Main(string[] args)
        {
            client = new TelegramBotClient(token);
            client.StartReceiving(Update, Error);
            Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода.");
            Console.ReadLine();
        }

        private static async Task Error(ITelegramBotClient arg1, Exception ex, CancellationToken arg3)
        {
            string errorMessage = "Произошла ошибка: " + ex.Message;

            try
            {
                if (chatId != 0)
                {
                    await client.SendTextMessageAsync(chatId, errorMessage);
                }
                else
                {
                    Console.WriteLine("Не удалось найти ID чата для отправки сообщения об ошибке.");
                }
            }
            catch (Exception sendEx)
            {
                Console.WriteLine("Ошибка при отправке сообщения в телеграм: " + sendEx.Message);
            }

            await Task.Delay(5000);
            client.StartReceiving(Update, Error);
        }


        private static async Task Update(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            try
            {
                if (arg2.Message.Text != null)
                {
                    chatId = arg2.Message.Chat.Id;
                    string[] messageParts = arg2.Message.Text.Split(' ');
                    string command = messageParts[0];
                    string[] parameters = messageParts.Skip(1).ToArray();

                    switch (command)
                    {

                        case "/start":

                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Привет! Я бот для получения и работы с курсами валют. Чтобы увидеть доступные команды, введите /help");
                            break;


                        case "/help":
                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Список команд\n/helpcode Выдаст множнство кодов валют.\n\n" +
                                "/today Код валюты1 Код валюты2 -поддерживаются любые коды заглавными буквами к примеру: RUB, USD, EUR.\n" +
                                "При вводе первым RUB- данные с ЦБ России, при вводе USD-с openexchangerates.org, в остальных случаях с Exchange Rates.\n\n" +
                                "/date Код валюты1 Код валюты2 дата(день.месяц.год)\n Код валюты1 только RUB и USD, остальнные в разработке.\n\n" +
                                "/week Код валюты1 Код валюты2. Выдаст курс за последние 7 дней.\n Код валюты1 только RUB и USD, остальнные в разработке.\n\n" +
                                "/mounth Код валюты1 Код валюты2. Выдаст курс за последние 30 дней.\n Код валюты1 только RUB и USD, остальнные в разработке.\n\n" +
                                "/year Код валюты1 Код валюты2. Выдаст курс с 1 января текущего года по сегодняшний день.\n Код валюты1 только RUB и USD, остальнные в разработке.\n\n" +
                                "/duration Код валюты1 Код валюты2 дата1(день.месяц.год) дата2(день.месяц.год). Выдаст курс с даты1 по дату2.\n Код валюты1 только RUB и USD, остальнные в разработке.\n\n" +
                                "Примеры запросов:\n/today MGA JOD\n/date RUB USD 01.11.2020\n/week RUB KZT\n/mounth USD FJD\n/year USD SVC\n/duration USD JOD 01.11.2020 01.10.2023");

                            break;

                        case "/helpcode":
                            string response = await ExchangeRateApiHelper.GetAllCurrencies();


                            int maxMessageLength = 4096;
                            int index = 0;
                            while (index < response.Length)
                            {
                                int length = Math.Min(maxMessageLength, response.Length - index);
                                string part = response.Substring(index, length);
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, part);

                                index += maxMessageLength;
                            }
                            break;




                        case "/today":
                            if (parameters.Length >= 2)
                            {
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                                    string date2 = DateTime.Parse(date).ToString("dd/MM/yyyy");
                                    var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, date);
                                    string exchangeRateToday = result.ExchangeRateString;
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRateToday);
                                }
                                else
                                                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    string date = DateTime.Now.ToString("dd/MM/yyyy");
                                    var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, date);
                                    string exchangeRateToday = result.ExchangeRateString;
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRateToday);
                                }
                                else
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    string exchangeRateToday = await ExchangeRateApiHelper.GetExchangeRateToday(currency1, currency2);
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRateToday);
                                }
                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют");
                            }
                            break;

                        case "/date":
                            if (parameters.Length >= 3)
                            {
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    string date = DateTime.Parse(parameters[2]).ToString("yyyy-MM-dd");
                                    var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, date);
                                    string exchangeRate = result.ExchangeRateString;
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    string date = DateTime.Parse(parameters[2]).ToString("dd/MM/yyyy");
                                    var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, date);
                                    string exchangeRate = result.ExchangeRateString;
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                {
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Первая валюта, только USD или RUB, остальные в разработке");
                                }

                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют или дату");
                            }
                            break;

                        case "/week":
                            if (parameters.Length >= 2)
                            {
                                decimal averageExchange = 0;
                                decimal maxExchange = decimal.MinValue;
                                decimal minExchange = decimal.MaxValue;
                                string dates;
                                string exchangeRate = "";
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = DateTime.Now;
                                    date = date.AddDays(-6);
                                    TimeSpan difference = DateTime.Now.AddDays(1) - date;
                                    int differenceInDays = difference.Days;
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("yyyy-MM-dd");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        date = date.AddDays(1);
                                    }
                                    exchangeRate += $"\nСредний курс за неделю {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за неделю {maxExchange}.\nМинимальный курс за неделю {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = DateTime.Now;
                                    date = date.AddDays(-6);
                                    TimeSpan difference = DateTime.Now.AddDays(1) - date;
                                    int differenceInDays = difference.Days;
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("dd/MM/yyyy");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        date = date.AddDays(1);
                                    }

                                    exchangeRate += $"\nСредний курс за неделю {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за неделю {maxExchange}.\nМинимальный курс за неделю {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                {
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Первая валюта, только USD или RUB, остальные в разработке");
                                }
                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют");
                            }
                            break;

                        case "/mounth":
                            if (parameters.Length >= 2)
                            {
                                decimal averageExchange = 0;
                                decimal maxExchange = decimal.MinValue;
                                decimal minExchange = decimal.MaxValue;
                                string dates;
                                string exchangeRate = "";
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = DateTime.Now;
                                    date = date.AddDays(-29);
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("yyyy-MM-dd");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        date = date.AddDays(1);
                                    }
                                    exchangeRate += $"\nСредний курс за месяц {Math.Round(averageExchange / 30, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за месяц {maxExchange}.\nМинимальный курс за месяц {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = DateTime.Now;
                                    date = date.AddDays(-29);
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("dd/MM/yyyy");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        date = date.AddDays(1);
                                    }
                                    exchangeRate += $"\nСредний курс за месяц {Math.Round(averageExchange / 30, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за месяц {maxExchange}.\nМинимальный курс за месяц {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                {
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Первая валюта, только USD или RUB, остальные в разработке");
                                }
                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют");
                            }
                            break;

                        case "/year":
                            if (parameters.Length >= 2)
                            {
                                decimal averageExchange = 0;
                                decimal maxExchange = decimal.MinValue;
                                decimal minExchange = decimal.MaxValue;
                                string dates;
                                string exchangeRate = "";
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
                                    TimeSpan difference = DateTime.Now - date;
                                    int differenceInDays = difference.Days;
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("yyyy-MM-dd");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        if (date.AddDays(1).Month > date.Month)
                                        {
                                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                            exchangeRate = "";
                                        }
                                        date = date.AddDays(1);
                                    }
                                    exchangeRate += $"\nСредний курс за год {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за год {maxExchange}.\nМинимальный курс за год {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    DateTime date = new DateTime(DateTime.Now.Year, 1, 1);
                                    TimeSpan difference = DateTime.Now - date;
                                    int differenceInDays = difference.Days;
                                    while (date < DateTime.Now)
                                    {
                                        dates = (date).ToString("dd/MM/yyyy");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        if (date.AddDays(1).Month > date.Month)
                                        {
                                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                            exchangeRate = "";
                                        }
                                        date = date.AddDays(1);
                                    }

                                    exchangeRate += $"\nСредний курс за год {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за год {maxExchange}.\nМинимальный курс за год {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                {
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Первая валюта, только USD или RUB, остальные в разработке");
                                }
                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют");
                            }
                            break;

                        case "/duration":
                            if (parameters.Length >= 3)
                            {
                                decimal averageExchange = 0;
                                decimal maxExchange = decimal.MinValue;
                                decimal minExchange = decimal.MaxValue;
                                string dates = DateTime.Parse(parameters[2]).ToString("dd/MM/yyyy");
                                DateTime date = DateTime.ParseExact(dates, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                string dateends = DateTime.Parse(parameters[3]).ToString("dd/MM/yyyy");
                                DateTime dateend = DateTime.ParseExact(dateends, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                                TimeSpan difference = dateend - date;
                                int differenceInDays = difference.Days;
                                string exchangeRate = "";
                                if (parameters[0] == "USD")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    while (date < dateend)
                                    {
                                        dates = (date).ToString("yyyy-MM-dd");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateOpen(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        if (date.AddDays(1).Month > date.Month)
                                        {
                                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                            exchangeRate = "";
                                        }
                                        date = date.AddDays(1);
                                    }
                                    exchangeRate += $"\nСредний курс за неделю {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за неделю {maxExchange}.\nМинимальный курс за неделю {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                if (parameters[0] == "RUB")
                                {
                                    string currency1 = parameters[0];
                                    string currency2 = parameters[1];
                                    while (date < dateend)
                                    {
                                        dates = (date).ToString("dd/MM/yyyy");
                                        var result = await ExchangeRateApiHelper.GetExchangeRateCenBank(currency1, currency2, dates);
                                        exchangeRate += result.ExchangeRateString;
                                        exchangeRate += "\n";
                                        averageExchange += result.ExchangeRate;
                                        if (maxExchange < result.ExchangeRate)
                                            maxExchange = result.ExchangeRate;
                                        if (minExchange > result.ExchangeRate)
                                            minExchange = result.ExchangeRate;
                                        if (date.AddDays(1).Month > date.Month)
                                        {
                                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                            exchangeRate = "";
                                        }
                                        date = date.AddDays(1);
                                    }

                                    exchangeRate += $"\nСредний курс за указанный период {Math.Round(averageExchange / differenceInDays, 6, MidpointRounding.AwayFromZero)}.\nМаксимальный курс за указанный период {maxExchange}.\nМинимальный курс за указанный период {minExchange}.";
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, exchangeRate);
                                }
                                else
                                {
                                    await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Первая валюта, только USD или RUB, остальные в разработке");
                                }
                            }
                            else
                            {
                                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Вы забыли указать коды валют или дату");
                            }
                            break;

                        default:
                            await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Такой команды не существует. Ожидайте может скоро появится.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при обновлении: " + ex.Message);
                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Произошла ошибка при обновлении: " + ex.Message);
            }
        }

    }
}
