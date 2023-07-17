using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Url_Request_Helper
{
    public class DatabaseWriter<T>
    {
        private static string ConnectionString;
        private static SqlConnection Connection;

        public DatabaseWriter()
        {
        }

        public void SetConnectionString(string serverName, string modelName, string userName, string password)
        {
            var connectionString = "";
            if (!string.IsNullOrEmpty(serverName) && !string.IsNullOrEmpty(modelName) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                if (!serverName.ToLowerInvariant().Contains("solvoyo"))
                {
                    serverName += ".solvoyo.com";
                }
                connectionString = $"DATA SOURCE={serverName};USER ID={userName};PASSWORD={password};CONNECTION TIMEOUT=600;INITIAL CATALOG={modelName}; MultipleActiveResultSets=true";
            }

            ConnectionString = connectionString;
        }

        public static void OpenConnection()
        {
            if (Connection == null)
                Connection = new SqlConnection(ConnectionString);

            if (Connection.State != ConnectionState.Open)
                Connection.Open();
        }

        public static SqlConnection GetConnection()
        {
            return Connection;
        }

        public static SqlDataReader GetSqlDataReader(string query)
        {
            SqlDataReader result = null;

            try
            {
                OpenConnection();
                var cmdsql = new SqlCommand(query, Connection);
                result = cmdsql.ExecuteReader();
            }
            catch (Exception e)
            {
                //GlobalLogger.LogError(e, "Can't load data (AvailableMethod): " + e.Message);
            }

            return result;
        }

        public static void ExecuteQuery(string query)
        {
            var sqlCmd = new SqlCommand(query, Connection);
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //GlobalLogger.LogError(e, $"Take an error while execute query: {query}");
            }
        }

        public void WriteToDatabase(List<Store> data, string destinationTableName)
        {
            try
            {
                CreateDestinationTableIfNotExists(destinationTableName);
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Create a DataTable
                    DataTable dataTable = new DataTable();

                    // Define the columns of the DataTable
                    dataTable.Columns.Add("ID", typeof(int));
                    dataTable.Columns.Add("ULKE", typeof(string));
                    dataTable.Columns.Add("IL", typeof(string));
                    dataTable.Columns.Add("ILID", typeof(string));
                    dataTable.Columns.Add("PLAKA", typeof(string));
                    dataTable.Columns.Add("KOD", typeof(string));
                    dataTable.Columns.Add("MAGAZATITLE", typeof(string));
                    dataTable.Columns.Add("MAGAZAKONUM", typeof(object));
                    dataTable.Columns.Add("MAGAZATEL", typeof(string));
                    dataTable.Columns.Add("ACIKLAMA", typeof(object));
                    dataTable.Columns.Add("H_ICI", typeof(object));
                    dataTable.Columns.Add("ILCE", typeof(string));
                    dataTable.Columns.Add("ILCEID", typeof(string));
                    dataTable.Columns.Add("SEMT", typeof(object));
                    dataTable.Columns.Add("SEMTID", typeof(object));
                    dataTable.Columns.Add("ADRES", typeof(string));
                    dataTable.Columns.Add("CMRTS", typeof(object));
                    dataTable.Columns.Add("PZR", typeof(object));
                    dataTable.Columns.Add("Lat", typeof(string));
                    dataTable.Columns.Add("Long", typeof(string));
                    dataTable.Columns.Add("FTGRF", typeof(object));

                    // Add the data rows to the DataTable
                    foreach (var item in data)
                    {
                        DataRow row = dataTable.NewRow();
                        row["ID"] = item.ID;
                        row["ULKE"] = item.ULKE;
                        row["IL"] = item.IL;
                        row["ILID"] = item.ILID;
                        row["PLAKA"] = item.PLAKA;
                        row["KOD"] = item.KOD;
                        row["MAGAZATITLE"] = item.MAGAZATITLE;
                        row["MAGAZAKONUM"] = item.MAGAZAKONUM;
                        row["MAGAZATEL"] = item.MAGAZATEL;
                        row["ACIKLAMA"] = item.ACIKLAMA;
                        row["H_ICI"] = item.H_ICI;
                        row["ILCE"] = item.ILCE;
                        row["ILCEID"] = item.ILCEID;
                        row["SEMT"] = item.SEMT;
                        row["SEMTID"] = item.SEMTID;
                        row["ADRES"] = item.ADRES;
                        row["CMRTS"] = item.CMRTS;
                        row["PZR"] = item.PZR;
                        row["Lat"] = item.Lat;
                        row["Long"] = item.Long;
                        row["FTGRF"] = item.FTGRF;

                        dataTable.Rows.Add(row);
                    }

                    // Bulk insert the DataTable into the database
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = destinationTableName;
                        bulkCopy.WriteToServer(dataTable);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void CreateDestinationTableIfNotExists(string destinationTableName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Check if the table already exists
                    string checkTableExistsQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName";
                    using (SqlCommand command = new SqlCommand(checkTableExistsQuery, connection))
                    {
                        command.Parameters.AddWithValue("@TableName", destinationTableName);
                        int tableCount = (int)command.ExecuteScalar();

                        if (tableCount > 0)
                        {
                            Console.WriteLine($"Table '{destinationTableName}' already exists.");
                            return;
                        }
                    }

                    // Create the table if it does not exist
                    string createTableQuery = "CREATE TABLE " + destinationTableName + " ( " +
                                              "ID INT, " +
                                              "ULKE NVARCHAR(255), " +
                                              "IL NVARCHAR(255), " +
                                              "ILID NVARCHAR(255), " +
                                              "PLAKA NVARCHAR(255), " +
                                              "KOD NVARCHAR(255), " +
                                              "MAGAZATITLE NVARCHAR(255), " +
                                              "MAGAZAKONUM NVARCHAR(MAX), " +
                                              "MAGAZATEL NVARCHAR(255), " +
                                              "ACIKLAMA NVARCHAR(MAX), " +
                                              "H_ICI NVARCHAR(MAX), " +
                                              "ILCE NVARCHAR(255), " +
                                              "ILCEID NVARCHAR(255), " +
                                              "SEMT NVARCHAR(MAX), " +
                                              "SEMTID NVARCHAR(MAX), " +
                                              "ADRES NVARCHAR(MAX), " +
                                              "CMRTS NVARCHAR(MAX), " +
                                              "PZR NVARCHAR(MAX), " +
                                              "Lat NVARCHAR(255), " +
                                              "Long NVARCHAR(255), " +
                                              "FTGRF NVARCHAR(MAX) " +
                                              ")";

                    using (SqlCommand command = new SqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Table '{destinationTableName}' created successfully.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        private DataTable CreateDataTableFromType(Type type)
        {
            DataTable dataTable = new DataTable();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            return dataTable;
        }

        private void SetDataRowValues(DataRow row, T item)
        {
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                object value = property.GetValue(item);
                row[property.Name] = value ?? DBNull.Value;
            }
        }
    }

}