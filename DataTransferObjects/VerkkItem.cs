using System;

namespace DataTransferObjects
{
    public class VerkkItem
    {
        /// <summary>
        /// Артикул
        /// </summary>
        public int VendorCode { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public Uri ULR { get; set; }

        /// <summary>
        /// Цена в евро
        /// </summary>
        public decimal PriceEuro { get; set; }

        /// <summary>
        /// Акция
        /// </summary>
        public bool Discount { get; set; }

        /// <summary>
        /// Категория
        /// </summary>
        public string Cathegory { get; set; }

        /// <summary>
        /// Загружен на сайт
        /// </summary>
        public bool Uploaded { get; set; }
    }
}
