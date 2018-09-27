namespace DataTransferObjects
{
    public class EuroItem
    {
        /// <summary>
        /// Идентификатор объекта в UMI
        /// </summary>
        public int ObjectID { get; set; }

        /// <summary>
        /// Адрес товара на verkk
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Наименование товара на Euromade
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Артикул на EuroMade
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// Цена на EuroMade
        /// </summary>
        public decimal EuroMadePrice { get; set; }

        /// <summary>
        /// Выгружать на маркет
        /// </summary>
        public bool UploadToMarket { get; set; }

        /// <summary>
        /// Наличие у поставщика
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// Закупочная цена
        /// </summary>
        public decimal OriginalPrice { get; set; }
    }
}
