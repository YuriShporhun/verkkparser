using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.global.statuses
{
    public enum LoaderStatus
    {
        Ready,
        Download,
        Upload,
        Done,
        Disconnected
    }

    public static class LoaderStatusEx
    {
        public static string ToAppStateName(this LoaderStatus status)
        {
            switch (status)
            {
                case LoaderStatus.Ready:
                    return "Не запущен";
                case LoaderStatus.Download:
                    return "Загружает с Euromade.ru";
                case LoaderStatus.Upload:
                    return "Загружает в базу данных";
                case LoaderStatus.Done:
                    return "Работа выполнена";
                case LoaderStatus.Disconnected:
                    return "Связь с EuroMade оборвана";
                default:
                    return "Статус не определен";
            }
        }
    }
}
