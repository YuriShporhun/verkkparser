using europarser.global.statuses;
using europarser.repository;
using static europarser.global.Variables;

namespace europarser.models
{
    public class AppState
    {
        AppState() { }

        public string crawlerState { get; private set; }
        public string isCrawlerHtmlReady { get; private set; }
        public string isCrawlerExcelReady { get; private set; }
        public string isParserExcelReady { get; private set; }
        public string loaderState { get; private set; }
        public string parserProgress { get; private set; }
        public string loaderProgress { get; private set; }
        public string parserState { get; private set; }

        public static AppState GetState()
        {
            string IsCrawlerHtmlReady = "Не готов";
            string IsCrawlerExcelReady = "Не готов";

            if (!Repository.VerkkItems.IsEmpty() || Crawler.Status == CrawlerStatus.Done)
            {
                IsCrawlerHtmlReady = "<button id='show-crawler-data' class='btn btn-outline-primary btn-sm'>Просмотреть</button>";
            }

            if (!Repository.VerkkItems.IsEmpty() || Crawler.Status == CrawlerStatus.Done)
            {
                IsCrawlerExcelReady = "<button id='show-crawler-excel' class='btn btn-outline-primary btn-sm'>Скачать</button>";
            }

            string ParserProgress = $"<b>{Parser.CurrentObject} / {Parser.TotalObjectCount}</b>";

            string IsParserExcelReady = "Не готов";

            if(!Repository.Parser.IsEmpty() || Parser.Status == ParserStatus.Done)
            {
                IsParserExcelReady = "<button id='show-parser-excel' class='btn btn-outline-primary btn-sm'>Скачать</button>";
            }

            return new AppState
            {
                crawlerState = Crawler.Status.ToAppStateName(),
                parserState = Parser.Status.ToAppStateName(),
                isCrawlerHtmlReady = IsCrawlerHtmlReady,
                loaderState = Loader.Status.ToAppStateName(),
                parserProgress = ParserProgress,
                loaderProgress = Loader.Progress,
                isParserExcelReady = IsParserExcelReady,
                isCrawlerExcelReady = IsCrawlerExcelReady
            };
        }
    }
}
