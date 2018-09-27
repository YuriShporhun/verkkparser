using System;

namespace DataTransferObjects
{
    public class ParserItem
    {
        /// <summary>
        /// Идентификатор объекта в UMI
        /// </summary>
        public int ObjectID { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Цена на Verkk
        /// </summary>
        public decimal VerkkPrice { get; set; }

        /// <summary>
        /// Вес
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Наличие 1
        /// </summary>
        public int AvailabilityCount1 { get; set; }

        /// <summary>
        /// Наличние 2
        /// </summary>
        public int AvailabilityCount2 { get; set; }

        /// <summary>
        /// Наличние 3
        /// </summary>
        public int AvailabilityCount3 { get; set; }

        /// <summary>
        /// Наличие на Verkk
        /// </summary>
        public bool AvailabilityVerkk { get; set; }

        /// <summary>
        /// Наличие EuroMade
        /// </summary>
        public bool AvailabilityEuroMade { get; set; }

        /// <summary>
        /// Длина
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Штрихкод
        /// </summary>
        public string EAN { get; set; }

        /// <summary>
        /// Акция
        /// </summary>
        public bool Discount { get; set; }

        /// <summary>
        /// Размер скидки
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// Адрес товара на verkk
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Наименование товара на Euromade
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цена на EuroMade
        /// </summary>
        public decimal EuroMadePrice { get; set; }

        /// <summary>
        /// Выгружать на маркет
        /// </summary>
        public bool UploadToMarket { get; set; }

        /// <summary>
        /// Дата окончания скидки 
        /// </summary>
        public DateTime? DiscountUntilDate { get; set; }

        /// <summary>
        /// Время окончания скидки
        /// </summary>
        public TimeSpan? DiscountUntilTime { get; set; }

        /// <summary>
        /// Закупочная цена
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
