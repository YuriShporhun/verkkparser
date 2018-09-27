using MySql.Data.MySqlClient;
using System;
using System.Globalization;

namespace EuroRepository.models
{
    public enum OLD_UMIField
    {
        В_АРХИВЕ,
        НАЛИЧИЕ_У_ПОСТАВЩИКА,
        ВЫГРУЖАТЬ_В_МАРКЕТ_СПБ,
        ВЫГРУЖАТЬ_В_МАРКЕТ_МСК,
        ЦЕНА,
        АРТИКУЛ,
        ДОБАВИТЬ_ЦЕНУ
    }

    public static class OLD_UMIFieldEx
    {
        public static int GetFieldID(this OLD_UMIField field)
        {
            switch (field)
            {
                /*case OLD_UMIField.В_АРХИВЕ:
                    return 3349;
                case OLD_UMIField.НАЛИЧИЕ_У_ПОСТАВЩИКА:
                    return 3445;
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_СПБ:
                    return 857;
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_МСК:
                    return 3071;
                case OLD_UMIField.АРТИКУЛ:
                    return 388;*/
                case OLD_UMIField.ЦЕНА:
                    return 306;
                case OLD_UMIField.ДОБАВИТЬ_ЦЕНУ:
                    return 306;
                default:
                    throw new NotSupportedException("Такой тип заливки не поддерживается");
            }
        }

        public static string GetColumn(this OLD_UMIField field)
        {
            switch (field)
            {
                case OLD_UMIField.В_АРХИВЕ:
                    return "int_val";
                case OLD_UMIField.НАЛИЧИЕ_У_ПОСТАВЩИКА:
                    return "int_val";
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_СПБ:
                    return "int_val";
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_МСК:
                    return "int_val";
                case OLD_UMIField.ЦЕНА:
                    return "float_val";
                case OLD_UMIField.ДОБАВИТЬ_ЦЕНУ:
                    return "float_val";
                case OLD_UMIField.АРТИКУЛ:
                    return "varchar_val";
                default:
                    throw new NotSupportedException("Такой тип данных не поддерживается");
            }
        }

        public static MySqlDbType GetMySqlDbType(this OLD_UMIField field)
        {
            switch (field)
            {
                case OLD_UMIField.В_АРХИВЕ:
                    return MySqlDbType.Int32;
                case OLD_UMIField.НАЛИЧИЕ_У_ПОСТАВЩИКА:
                    return MySqlDbType.Int32;
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_СПБ:
                    return MySqlDbType.Int32;
                case OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_МСК:
                    return MySqlDbType.Int32;
                case OLD_UMIField.ЦЕНА:
                    return MySqlDbType.Double;
                case OLD_UMIField.ДОБАВИТЬ_ЦЕНУ:
                    return MySqlDbType.Double;
                case OLD_UMIField.АРТИКУЛ:
                    return MySqlDbType.VarChar;
                default:
                    throw new NotSupportedException("Такой тип данных не поддерживается");
            }
        }

        public static OLD_UMIField ToOldUMIFrield(this string action)
        {
            switch (action)
            {
                case "marketSPB":
                    return OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_СПБ;
                case "marketMSK":
                    return OLD_UMIField.ВЫГРУЖАТЬ_В_МАРКЕТ_МСК;
                case "availability":
                    return OLD_UMIField.НАЛИЧИЕ_У_ПОСТАВЩИКА;
                case "archive":
                    return OLD_UMIField.В_АРХИВЕ;
                case "price":
                    return OLD_UMIField.ЦЕНА;
                case "addPrice":
                    return OLD_UMIField.ДОБАВИТЬ_ЦЕНУ;
                default:
                    throw new NotSupportedException("Такой тип действия не поддерживается");
            }
        }

        private static int YesNoToInt(string value)
        {
            if (value.ToLower() == "да" || value == "1")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static object ToMySQLValue(this string action, string value)
        {
            switch (action)
            {
                case "archive":
                    return YesNoToInt(value);
                case "marketSPB":
                    return YesNoToInt(value);
                case "marketMSK":
                    return YesNoToInt(value);
                case "availability":
                    return YesNoToInt(value);
                case "price":
                    return double.Parse(value, CultureInfo.InvariantCulture);
                case "addPrice":
                    return double.Parse(value, CultureInfo.InvariantCulture);
                default:
                    throw new NotSupportedException("Такой тип действия не поддерживается");
            }
        }
    }
}
