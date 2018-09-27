using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.global.statuses
{
    public enum ParserStatus
    {
        Ready,
        Parsing,
        Done
    }

    public static class ParserStatusEx
    {
        public static string ToAppStateName(this ParserStatus status)
        {
            switch (status)
            {
                case ParserStatus.Ready:
                    return "Не запущен";
                case ParserStatus.Parsing:
                    return "Парсинг";
                case ParserStatus.Done:
                    return "Завершил работу";
                default:
                    return "Не определен";
            }
        }
    }
}
