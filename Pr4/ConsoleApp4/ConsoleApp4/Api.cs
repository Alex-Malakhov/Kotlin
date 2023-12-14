using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;


namespace Bot
{
    class ExchangeRateApiHelper
    {
        private static HttpClient client = new HttpClient();
        public class ExchangeData
        {
            public string ExchangeRateString { get; set; }
            public decimal ExchangeRate { get; set; }
        }

        public static async Task<ExchangeData> GetExchangeRateOpen(string baseCurrency, string targetCurrency, string date)
        {
            string apiKey = "9fdf7922117b43539d109b3b1aefbbf2";
            string apiUrl = $"https://openexchangerates.org/api/historical/{date}.json?app_id={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    JObject exchangeRateData = JObject.Parse(responseData);
                    decimal exchangeRate = Convert.ToDecimal(exchangeRateData["rates"][targetCurrency]);

                    string exchangeRateString = $"{baseCurrency} / {targetCurrency} курс на {DateTime.Parse(date).ToString("dd/MM/yyyy")}: 1 / {exchangeRate}";
                    return new ExchangeData { ExchangeRateString = exchangeRateString, ExchangeRate = exchangeRate };
                }
                else
                {
                    return new ExchangeData { ExchangeRateString = "Ошибка при получении данных о курсе валют", ExchangeRate = 0 };
                }
            }
        }
        public static async Task<string> GetAllCurrencies()
        {
            string apiKey = "9fdf7922117b43539d109b3b1aefbbf2";
            string apiUrl = $"https://openexchangerates.org/api/currencies.json?app_id={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    return "Ошибка при получении данных о кодах валют";
                }
            }
        }


        public static async Task<string> GetExchangeRateToday(string baseCurrency, string targetCurrency)
        {
            string apiUrl = "https://api.exchangerate-api.com/v4/latest/" + baseCurrency;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    decimal exchangeRate = ParseExchangeRate(responseData, targetCurrency);
                    return $"{baseCurrency} / {targetCurrency} курс на {DateTime.Now.ToString("dd/MM/yyyy")}: 1 / {exchangeRate}";
                }
                else
                {
                    return "Произошла ошибка при получении данных о курсе валют";
                }
            }
        }
        private static decimal ParseExchangeRate(string responseData, string targetCurrency)
        {
            dynamic responseJson = JObject.Parse(responseData);
            if (responseJson["rates"] != null)
            {
                var rates = responseJson["rates"];
                var rate = rates[targetCurrency];

                if (rate != null)
                {
                    return rate;
                }
            }
            throw new Exception("Курс не найден");
        }


        public static async Task<ExchangeData> GetExchangeRateCenBank(string baseCurrency, string targetCurrency, string date)
        {
            string apiUrl = "https://www.cbr.ru/scripts/XML_daily.asp";
            string fullUrl = $"{apiUrl}?date_req={date}&VAL_NM_RQ={targetCurrency}";

            HttpResponseMessage response = await client.GetAsync(fullUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var result = ParseExchangeRateCenBan(responseData, date, baseCurrency, targetCurrency);
                return result;
            }
            else
            {
                return new ExchangeData { ExchangeRateString = "Произошла ошибка при получении данных с ЦБР", ExchangeRate = 0 };
            }
        }
        private static ExchangeData ParseExchangeRateCenBan(string Data, string date, string baseCurrency, string targetCurrency)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Data);
            var currencyNodes = xmlDoc.SelectNodes($"//Valute[CharCode='{targetCurrency}']");
            if (currencyNodes.Count == 1)
            {
                var currency1Node = currencyNodes[0];

                string currency1Name = currency1Node.SelectSingleNode("Name")?.InnerText;
                string exchangeRate = currency1Node.SelectSingleNode("Value")?.InnerText;
                string exchangeNominal = currency1Node.SelectSingleNode("Nominal")?.InnerText;
                decimal ExchangeRate1 = decimal.Parse(exchangeRate);
                decimal ExchangeNominal = decimal.Parse(exchangeNominal);
                decimal ExchangeRate2 = ExchangeNominal / ExchangeRate1;
                if (ExchangeRate2 < 1)
                    ExchangeRate2 = Math.Round(ExchangeRate2, 6, MidpointRounding.AwayFromZero);
                else
                    ExchangeRate2 = Math.Round(ExchangeRate2, 2, MidpointRounding.AwayFromZero);
                string exchangeRate2 = ExchangeRate2.ToString();
                string exchangeRateString = $"{baseCurrency} / {targetCurrency} курс на {date}: 1 / {exchangeRate2}";
                return new ExchangeData { ExchangeRateString = exchangeRateString, ExchangeRate = decimal.Parse(exchangeRate2) };
            }
            return new ExchangeData { ExchangeRateString = "Курс не найден", ExchangeRate = 0 };
        }

    }

}
