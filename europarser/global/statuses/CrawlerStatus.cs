namespace europarser.global.statuses
{
    public enum CrawlerStatus
    {
        Ready,
        Run,
        Done
    }

    public static class CrowlerStatusEx
    {
        public static string ToAppStateName(this CrawlerStatus status)
        {
            switch (status)
            {
                case CrawlerStatus.Ready:
                    return "Не запущен";
                case CrawlerStatus.Run:
                    return "Запущен";
                case CrawlerStatus.Done:
                    return "Работа выполнена";
                default:
                    return "Статус не определен";
            }
        }
    }
}
