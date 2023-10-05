using System;
using System.Diagnostics;
using System.Net;
using Newtonsoft.Json.Linq;

namespace WikipediaSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            VariableStorage variables = new VariableStorage();
            SearchHandler searchHandler = new SearchHandler(variables);
            ApiUrlCreator apiUrlCreator = new ApiUrlCreator(variables);
            ResponseParser responseParser = new ResponseParser(variables);
            BrowserLauncher browserLauncher = new BrowserLauncher(variables);

            variables.SearchQuery = searchHandler.GetSearch();
            string apiUrl = apiUrlCreator.CreateApiUrl();
            string response = variables.WebClient.DownloadString(apiUrl);
            string articleUrl = responseParser.ParseUrl(response);
            browserLauncher.Browser(articleUrl);

            Console.ReadKey();
        }
    }

    class VariableStorage
    {
        public WebClient WebClient { get; private set; }
        public string SearchQuery { get; set; }

        public VariableStorage()
        {
            WebClient = new WebClient();
        }
    }

    class SearchHandler
    {
        private VariableStorage variables;

        public SearchHandler(VariableStorage variables)
        {
            this.variables = variables;
        }

        public string GetSearch()
        {
            Console.WriteLine("Введите поисковый запрос:");
            return Console.ReadLine();
        }
    }

    class ApiUrlCreator
    {
        private VariableStorage variables;

        public ApiUrlCreator(VariableStorage variables)
        {
            this.variables = variables;
        }

        public string CreateApiUrl()
        {
            string encodedQuery = Uri.EscapeDataString(variables.SearchQuery);
            return $"https://ru.wikipedia.org/w/api.php?action=query&list=search&utf8=&format=json&srsearch={encodedQuery}";
        }
    }

    class ResponseParser
    {
        private VariableStorage variables;

        public ResponseParser(VariableStorage variables)
        {
            this.variables = variables;
        }

        public string ParseUrl(string json)
        {
            JToken responseToken = JToken.Parse(json);
            JToken firstResult = responseToken["query"]?["search"]?.First;

            if (firstResult != null)
            {
                string pageId = firstResult["pageid"].ToString();
                return $"https://ru.wikipedia.org/w/index.php?curid={pageId}";
            }

            return null;
        }
    }

    class BrowserLauncher
    {
        private VariableStorage variables;

        public BrowserLauncher(VariableStorage variables)
        {
            this.variables = variables;
        }

        public void Browser(string url)
        {
            try
            {
                Process.Start(url);
                Console.WriteLine("Открытие прошло успешно.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Не удалось открыть статью в браузере.");
                Console.WriteLine(ex.Message);
            }
        }
    }
}