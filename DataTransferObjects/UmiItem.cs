namespace DataTransferObjects
{
    public class UmiItem
    {
        public UmiItem(int objectID, int fieldID, string uriOrName, decimal? euroMadePrice, bool? uploadToMarket)
        {
            ObjectID = objectID;
            FieldID = fieldID;
            UriOrName = uriOrName;
            EuroMadePrice = euroMadePrice;
            UploadToMarket = uploadToMarket;
        }

        /// <summary>
        /// Идентификатор объекта в иерархии UMI CMS
        /// </summary>
        public int ObjectID { get; private set; }

        /// <summary>
        /// Идентификатор поля в иерархии UMI CMS
        /// </summary>
        public int FieldID { get; private set; }

        /// <summary>
        /// Адрес товара на verkk или его наименование на Euromade
        /// </summary>
        public string UriOrName { get; private set; }

        /// <summary>
        /// Цена на EuroMade
        /// </summary>
        public decimal? EuroMadePrice { get; private set; }

        /// <summary>
        /// Выгружать на маркет
        /// </summary>
        public bool? UploadToMarket { get; private set; }
    }
}
