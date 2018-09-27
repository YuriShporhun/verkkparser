using DataTransferObjects;
using europarser.global.singletone;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using EuroRepository;
using System.Data.SqlClient;
using System.Linq;
using FastMember;
using EuroRepository.models;
using System.Data;

namespace europarser.repository
{

    public class DbEuroMade
    {
        DbEuroMade() { }
        static DbEuroMade instance = new DbEuroMade();
        public static DbEuroMade Instance => instance;

        public List<UmiItem> Download(Action<string> updateState)
        {
            updateState("Инициализация базы");

            string commandText = SQL.ResourceManager.GetString("DownloadFromEuromade");
            List<UmiItem> result = new List<UmiItem>();

            using (MySqlConnection connection = ConnectionBuilder.EuroMadeSelect as MySqlConnection)
            {

                connection.Open();

                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    command.CommandTimeout = 600;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        int downloadCount = 0;
                        updateState("Инициализация скачивания");

                        while (reader.Read())
                        {
                            int ObjectID = reader.GetInt32(0);

                            bool? UploadToMarket = null;
                            string UrlOrName = null;
                            decimal? PriceEuro = null;

                            if (!reader.IsDBNull(1))
                                UploadToMarket = reader.GetBoolean(1);

                            if (!reader.IsDBNull(2))
                                UrlOrName = reader.GetString(2);

                            if (!reader.IsDBNull(3))
                                PriceEuro = reader.GetDecimal(3);

                            int FieldID = reader.GetInt32(4);

                            result.Add(new UmiItem(ObjectID, FieldID, UrlOrName, PriceEuro, UploadToMarket));
                            downloadCount++;
                            updateState($"Скачано {downloadCount} объектов");
                        }
                    }
                }
            }

            updateState("Все объекты скачаны");

            return result;
        }

        public IEnumerable<string> GetAllVendorCodes()
        {
            string commandText = $"SELECT UriOrName FROM EuroItems WHERE FieldId=388;";
            using (SqlConnection connection = ConnectionBuilder.ParserDB)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                                yield return reader.GetString(0);
                        }
                    }
                }
            }
        }

        public IEnumerable<UMI_ObjectProperty> GetAllCodesWithObjId()
        {
            string commandText = SQL.GetAllVendorCodes;

            using (MySqlConnection connection = ConnectionBuilder.EuroMadeSelectUpdate)
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new UMI_ObjectProperty
                            {
                                ObjectID = reader.GetInt32(0),
                                Value = reader.GetString(1)
                            };
                        }
                    }
                }
            }
        }

        public IEnumerable<int> GetByVendorCode(string vendorCode)
        {
            string commandText = $"SELECT obj_id FROM cms3_object_content WHERE varchar_val='{vendorCode}' and field_id=@field_id;";

            using (MySqlConnection connection = ConnectionBuilder.EuroMadeSelect)
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@field_id", OLD_UMIField.АРТИКУЛ.GetFieldID());

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return reader.GetInt32(0);
                        }
                    }
                }
            }
        }

        public bool UpdateFields(List<UMI_ObjectProperty> properties, string action)
        {
            int fieldID = -1;
            string column = string.Empty;
            MySqlDbType? type = null;

            try
            {
                fieldID = action.ToOldUMIFrield().GetFieldID();
                column = action.ToOldUMIFrield().GetColumn();
                type = action.ToOldUMIFrield().GetMySqlDbType();
            }
            catch (NotSupportedException)
            {
                return false;
            }

            if (action != "addPrice")
            {

                using (MySqlConnection connection = ConnectionBuilder.EuroMadeSelectUpdate)
                {
                    string commandText = "UPDATE cms3_object_content SET " +
        $"{column}=@NewValue WHERE field_id=@fieldID and obj_id=@objectID;";

                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();
                    using (MySqlCommand command = new MySqlCommand(commandText, connection, transaction))
                    {
                        command.Parameters.Add(new MySqlParameter("@NewValue", type.Value));
                        command.Parameters.Add(new MySqlParameter("@fieldID", MySqlDbType.Int32));
                        command.Parameters.Add(new MySqlParameter("@objectID", MySqlDbType.Int32));

                        try
                        {

                            foreach (var item in properties)
                            {
                                command.Parameters["@NewValue"].Value = action.ToMySQLValue(item.Value);
                                command.Parameters["@objectID"].Value = item.ObjectID;
                                command.Parameters["@fieldID"].Value = fieldID;
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            connection.Close();
                            return false;
                        }

                        transaction.Commit();
                    }
                }
            }
            else
            {     
                string commandText = @"INSERT INTO cms3_object_content (obj_id, field_id, int_val, varchar_val, text_val, rel_val, float_val)
                                       VALUES(@objectID, @fieldID, NULL, NULL, NULL, NULL, @NewValue);";

                using (MySqlConnection connection = ConnectionBuilder.EuroMadeSelectUpdate)
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(commandText, connection))
                    {
                        command.Parameters.Add(new MySqlParameter("@NewValue", type.Value));
                        command.Parameters.Add(new MySqlParameter("@fieldID", MySqlDbType.Int32));
                        command.Parameters.Add(new MySqlParameter("@objectID", MySqlDbType.Int32));

                        try
                        {

                            foreach (var item in properties)
                            {
                                command.Parameters["@NewValue"].Value = action.ToMySQLValue(item.Value);
                                command.Parameters["@objectID"].Value = item.ObjectID;
                                command.Parameters["@fieldID"].Value = fieldID;
                                command.ExecuteNonQuery();
                            }
                        }
                        catch (Exception e)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public void Upload(List<UmiItem> euroMadeData, Action<string> updateState)
        {
            updateState("Инициализация загрузчика.");

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();
                updateState("Подготовка транзакции.");

                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepNulls, transaction))
                {
                    bulkCopy.BatchSize = 500;
                    bulkCopy.DestinationTableName = "dbo.EuroItems";
                    bulkCopy.ColumnMappings.Add(nameof(UmiItem.ObjectID), nameof(UmiItem.ObjectID));
                    bulkCopy.ColumnMappings.Add(nameof(UmiItem.FieldID), nameof(UmiItem.FieldID));
                    bulkCopy.ColumnMappings.Add(nameof(UmiItem.EuroMadePrice), nameof(UmiItem.EuroMadePrice));
                    bulkCopy.ColumnMappings.Add(nameof(UmiItem.UploadToMarket), nameof(UmiItem.UploadToMarket));
                    bulkCopy.ColumnMappings.Add(nameof(UmiItem.UriOrName), nameof(UmiItem.UriOrName));

                    try
                    {
                        using (var reader = ObjectReader.Create(euroMadeData))
                        {
                            updateState("Запись объектов транзакции.");
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        updateState("Отмена транзакции.");
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                transaction.Commit();
            }

            updateState("Все объекты загружены");
        }

        public List<int> GetAvailableObjects()
        {
            string commandText = "SELECT distinct ObjectID FROM EuroItems";
            List<int> result = new List<int>();

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ObjectID = reader.GetInt32(0);
                            result.Add(ObjectID);
                        }
                    }
                }
            }

            return result;
        }

        public EuroItem GetEuroItem(int objectId)
        {
            string commandText = "SELECT ObjectID, UploadToMarket, UriOrName, EuroMadePrice, FieldID FROM EuroItems WHERE ObjectId=@objectId";
            List<UmiItem> umiItems = new List<UmiItem>();

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@ObjectId", objectId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int ObjectID = reader.GetInt32(0);

                            bool? UploadToMarket = null;
                            string UrlOrName = null;
                            decimal? PriceEuro = null;

                            if (!reader.IsDBNull(1))
                                UploadToMarket = reader.GetBoolean(1);

                            if (!reader.IsDBNull(2))
                                UrlOrName = reader.GetString(2);

                            if (!reader.IsDBNull(3))
                                PriceEuro = reader.GetDecimal(3);

                            int FieldID = reader.GetInt32(4);

                            umiItems.Add(new UmiItem(ObjectID, FieldID, UrlOrName, PriceEuro, UploadToMarket));
                        }
                    }
                }
            }

            decimal? price = null;

            if (umiItems.Where(i => i.FieldID == 248).Any())
            {
                price = umiItems
                    .Where(i => i.FieldID == 248)
                    .First()
                    .EuroMadePrice;
            }

            if(!price.HasValue)
            {
                price = 0;
            }


            bool? market = null;

            if (umiItems.Where(i => i.FieldID == 857).Any())
            {
                market = umiItems
                    .Where(i => i.FieldID == 857)
                    .First()
                    .UploadToMarket;
            }

            if(!market.HasValue)
            {
                market = false;
            }

            bool? available = null;

            if (umiItems.Where(i => i.FieldID == 3445).Any())
            {
                available = umiItems
                    .Where(i => i.FieldID == 3445)
                    .First()
                    .UploadToMarket;
            }

            if(!available.HasValue)
            {
                available = false;
            }

            string vendorCode = null;

            if (umiItems.Where(i => i.FieldID == 388).Any())
            {
                vendorCode = umiItems
                    .Where(i => i.FieldID == 388)
                    .First()
                    .UriOrName;
            }

            if(string.IsNullOrWhiteSpace(vendorCode))
            {
                vendorCode = "НЕ_ЗАДАНО";
            }

            int _objectId = umiItems[0].ObjectID;

            string name = null;

            if (umiItems.Where(i => i.FieldID == 117).Any())
            {
                name = umiItems
                    .Where(i => i.FieldID == 117)
                    .First()
                    .UriOrName ?? "НЕ_ЗАДАНО";
            }

            decimal? originalPrice = null;

            if (umiItems.Where(i => i.FieldID == 64).Any())
            {
                originalPrice = umiItems
                    .Where(i => i.FieldID == 64)
                    .First()
                    .EuroMadePrice;
            }

            if (!originalPrice.HasValue)
            {
                originalPrice = 0;
            }

            return new EuroItem
            {
                Uri = umiItems.Where(i => i.FieldID == 547).First().UriOrName,
                EuroMadePrice = price.Value,
                Name = name,
                UploadToMarket = market.Value,
                Available = available.Value,
                ObjectID = _objectId,
                VendorCode = vendorCode,
                OriginalPrice = originalPrice.Value
            };
        }
    }
}
