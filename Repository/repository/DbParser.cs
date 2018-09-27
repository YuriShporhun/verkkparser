using DataTransferObjects;
using europarser.global.singletone;
using FastMember;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EuroRepository.repository
{
    public class DbParser
    {
        DbParser() { }
        static DbParser instance = new DbParser();
        public static DbParser Instance => instance;

        public void SavePrices(List<IdPrice> prices)
        {
            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                SqlTransaction transaction = connection.BeginTransaction();

                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepNulls, transaction))
                {
                    bulkCopy.BatchSize = 500;
                    bulkCopy.DestinationTableName = "dbo.PrevPrices";
                    bulkCopy.ColumnMappings.Add(nameof(IdPrice.ObjectID), nameof(IdPrice.ObjectID));
                    bulkCopy.ColumnMappings.Add(nameof(IdPrice.Price), nameof(IdPrice.Price));

                    try
                    {
                        using (var reader = ObjectReader.Create(prices))
                        {
                            bulkCopy.WriteToServer(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        connection.Close();
                    }
                }

                transaction.Commit();
            }
        }


        public IEnumerable<IdPrice> GetAllPricesPrev()
        { 
            string commandText = "SELECT ObjectID, Price FROM PrevPrices";

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new IdPrice
                            {
                                ObjectID = reader.GetInt32(0),
                                Price = reader.GetDecimal(1)
                            };
                        }
                    }
                }
            }
        }

        public void TruncatePrices()
        {
            string commandText = "truncate table prevprices";

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<IdPrice> GetAllPricesParser()
        {
            string commandText = "SELECT ObjectID, VerkkPrice FROM ParserItems";

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new IdPrice
                            {
                                ObjectID = reader.GetInt32(0),
                                Price = reader.GetDecimal(1)
                            };
                        }
                    }
                }
            }
        }

        public bool IsEmpty()
        {
            string commandText = "SELECT COUNT(*) from ParserItems";
            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    int result = int.Parse(command.ExecuteScalar().ToString());

                    if (result == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public List<int> GetDownloadedObjects()
        {
            string commandText = "SELECT distinct ObjectID FROM ParserItems";
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

        public List<ParserItem> SelectAll()
        {
            string commandText = "SELECT " +
                "VendorCode, VerkkPrice, AvailabilityCount1, AvailabilityCount2, AvailabilityEuroMade, AvailabilityVerkk, " +
                "Length, Width, Heigth, Discount, DiscountValue, Uri, Name, UploadToMarket, EuroMadePrice, EAN, Weight, ObjectID, " +
                "AvailabilityCount3, DiscountUntilDate, DiscountUntilTime, OriginalPrice " +
                "FROM ParserItems;";

            List<ParserItem> result = new List<ParserItem>();

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? discountDate = null;
                            TimeSpan? discountTime = null;

                            if (!reader.IsDBNull(19))
                                discountDate = reader.GetDateTime(19);

                            if (!reader.IsDBNull(20))
                                discountTime = reader.GetTimeSpan(20);

                            result.Add(new ParserItem
                            {
                                VendorCode = reader.GetString(0),
                                VerkkPrice = reader.GetDecimal(1),
                                AvailabilityCount1 = reader.GetInt32(2),
                                AvailabilityCount2 = reader.GetInt32(3),
                                AvailabilityEuroMade = reader.GetBoolean(4),
                                AvailabilityVerkk = reader.GetBoolean(5),
                                Length = reader.GetInt32(6),
                                Width = reader.GetInt32(7),
                                Height = reader.GetInt32(8),
                                Discount = reader.GetBoolean(9),
                                DiscountValue = reader.GetDecimal(10),
                                Uri = reader.GetString(11),
                                Name = reader.GetString(12),
                                UploadToMarket = reader.GetBoolean(13),
                                EuroMadePrice = reader.GetDecimal(14),
                                EAN = reader.GetString(15),
                                Weight = reader.GetDouble(16),
                                ObjectID = reader.GetInt32(17),
                                AvailabilityCount3 = reader.GetInt32(18),
                                DiscountUntilDate = discountDate,
                                DiscountUntilTime = discountTime,
                                OriginalPrice = reader.GetDecimal(21)
                            });
                        }
                    }
                }
            }

            return result;
        }

        public void Insert(ParserItem item)
        {
            string commandText = "INSERT INTO ParserItems " +
                "(VendorCode, VerkkPrice, AvailabilityCount1, AvailabilityCount2, AvailabilityCount3, AvailabilityVerkk, Length, Width, Heigth, " +
                "Discount, DiscountValue, Uri, Name, UploadToMarket, EuroMadePrice, AvailabilityEuroMade, ObjectID, Weight, EAN, DiscountUntilDate, DiscountUntilTime, OriginalPrice) " +
                "VALUES (@VendorCode, @VerkkPrice, @AvailabilityCount1, @AvailabilityCount2, @AvailabilityCount3, @AvailabilityVerkk, @Length, @Width, @Heigth, " +
                "@Discount, @DiscountValue, @Uri, @Name, @UploadToMarket, @EuroMadePrice, @AvailabilityEuroMade, @ObjectID, @Weight, @EAN, @DiscountUntilDate, @DiscountUntilTime, @OriginalPrice);";

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@AvailabilityEuroMade", item.AvailabilityEuroMade);
                    command.Parameters.AddWithValue("@Weight", item.Weight);
                    command.Parameters.AddWithValue("@EAN", item.EAN);
                    command.Parameters.AddWithValue("@ObjectID", item.ObjectID);
                    command.Parameters.AddWithValue("@EuroMadePrice", item.EuroMadePrice);
                    command.Parameters.AddWithValue("@VendorCode", item.VendorCode);
                    command.Parameters.AddWithValue("@VerkkPrice", item.VerkkPrice);
                    command.Parameters.AddWithValue("@AvailabilityCount1", item.AvailabilityCount1);
                    command.Parameters.AddWithValue("@AvailabilityCount2", item.AvailabilityCount2);
                    command.Parameters.AddWithValue("@AvailabilityCount3", item.AvailabilityCount3);
                    command.Parameters.AddWithValue("@AvailabilityVerkk", item.AvailabilityVerkk);
                    command.Parameters.AddWithValue("@Length", item.Length);
                    command.Parameters.AddWithValue("@Width", item.Width);
                    command.Parameters.AddWithValue("@Heigth", item.Height);
                    command.Parameters.AddWithValue("@Discount", item.Discount);
                    command.Parameters.AddWithValue("@DiscountValue", item.DiscountValue);
                    command.Parameters.AddWithValue("@Uri", item.Uri);
                    command.Parameters.AddWithValue("@Name", item.Name);
                    command.Parameters.AddWithValue("@UploadToMarket", item.UploadToMarket);
                    command.Parameters.AddWithValue("@OriginalPrice", item.OriginalPrice);

                    if (item.DiscountUntilDate.HasValue)
                        command.Parameters.AddWithValue("@DiscountUntilDate", item.DiscountUntilDate.Value);
                    else
                        command.Parameters.AddWithValue("@DiscountUntilDate", DBNull.Value);

                    if(item.DiscountUntilTime.HasValue)
                        command.Parameters.AddWithValue("@DiscountUntilTime", item.DiscountUntilTime.Value);
                    else
                        command.Parameters.AddWithValue("@DiscountUntilTime", DBNull.Value);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
