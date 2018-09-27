using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.global.statuses
{
    public enum UploaderStatus
    {
        Ready,
        Started,
        Excel,
        Download,
        Update,
        Done
    }

    public static class UploaderStatusEx
    {
        public static string ToAppStateName(this UploaderStatus status)
        {
            switch (status)
            {
                case UploaderStatus.Ready:
                    return "Готов к запуску";
                case UploaderStatus.Started:
                    return "Инициализация";
                case UploaderStatus.Excel:
                    return "Разбор файла";
                case UploaderStatus.Download:
                    return "Загрузка кодов";
                case UploaderStatus.Update:
                    return "Обновление базы";
                case UploaderStatus.Done:
                    return "Работа завершена";
                default:
                    return "Статус не определен";
            }
        }
    }
}
