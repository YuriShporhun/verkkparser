using DataTransferObjects;
using europarser.global.singletone;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace europarser.repository
{
    public class DbVerkkItems
    {
        DbVerkkItems() { }
        static DbVerkkItems instance = new DbVerkkItems();
        public static DbVerkkItems Instance => instance;

        public bool IsEmpty()
        {
            string commandText = "SELECT COUNT(*) from VerkkItems";
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

        public List<VerkkItem> Get()
        {
            string commandText = "SELECT VendorCode, Name, URL, PriceEuro, Discount, Category, Uploaded From VerkkItems";
            List<VerkkItem> result = new List<VerkkItem>();

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new VerkkItem {
                                VendorCode = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                ULR = new Uri(reader.GetString(2)),
                                PriceEuro = reader.GetDecimal(3),
                                Discount = reader.GetBoolean(4),
                                Cathegory = reader.GetString(5),
                                Uploaded = reader.GetBoolean(6)
                            });
                        }
                    }
                }
            }

            return result;
        }

        public void Insert(IEnumerable<VerkkItem> siteItems)
        {
            string commandText = "INSERT INTO VerkkItems (VendorCode, Name, URL, PriceEuro, Discount, Category, Uploaded) " +
                "VALUES (@VendorCode, @Name, @URL, @PriceEuro, @Discount, @Category, @Uploaded);";

            using (SqlConnection connection = ConnectionBuilder.ParserDB as SqlConnection)
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {

                    command.Parameters.Add("@VendorCode", SqlDbType.Int);
                    command.Parameters.Add("@Name", SqlDbType.NVarChar);
                    command.Parameters.Add("@URL", SqlDbType.NVarChar);
                    command.Parameters.Add("@PriceEuro", SqlDbType.SmallMoney);
                    command.Parameters.Add("@Discount", SqlDbType.Bit);
                    command.Parameters.Add("@Category", SqlDbType.NVarChar);
                    command.Parameters.Add("@Uploaded", SqlDbType.Bit);

                    foreach (var item in siteItems)
                    {
                        command.Parameters["@VendorCode"].Value = item.VendorCode;
                        command.Parameters["@Name"].Value = item.Name;
                        command.Parameters["@URL"].Value = item.ULR.AbsoluteUri;
                        command.Parameters["@PriceEuro"].Value = item.PriceEuro;
                        command.Parameters["@Discount"].Value = item.Discount;
                        command.Parameters["@Category"].Value = item.Cathegory;
                        command.Parameters["@Uploaded"].Value = item.Uploaded;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
