using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace DotNET_Assessment
{
    public class DatabaseInitializer
    {
        private readonly string _mssqlConnectionString;
        private readonly string _sqliteConnectionString;

        public DatabaseInitializer(string mssqlConnection, string sqliteConnection)
        {
            _mssqlConnectionString = mssqlConnection;
            _sqliteConnectionString = sqliteConnection;
        }


        public void CreateMSSQLDatabase()
        {
            using (SqlConnection conn = new SqlConnection(_mssqlConnectionString))
            {
                conn.Open();
                string createDbQuery = "IF DB_ID('CustomerDB') IS NULL CREATE DATABASE CustomerDB";
                using (SqlCommand cmd = new SqlCommand(createDbQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                string createTablesQuery = @"
                    USE CustomerDB;
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Customer' AND xtype='U')
                    CREATE TABLE Customer (
                        CustomerID INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(100),
                        Email NVARCHAR(100),
                        Phone NVARCHAR(15)
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Location' AND xtype='U')
                    CREATE TABLE Location (
                        LocationID INT IDENTITY(1,1) PRIMARY KEY,
                        CustomerID INT FOREIGN KEY REFERENCES Customer(CustomerID),
                        Address NVARCHAR(255)
                    );";

                using (SqlCommand cmd = new SqlCommand(createTablesQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("MSSQL Database and Tables Created!");

                string checkQuery = "SELECT COUNT(*) FROM Customer";
                using (var command = new SqlCommand(checkQuery, conn))
                {
                    int count = (int)command.ExecuteScalar();
                    if (count == 0) // Insert only if the table is empty
                    {
                        DatabaseInserter dbinserter = new DatabaseInserter(_mssqlConnectionString);
                        dbinserter.InsertCustomerDataMSSQL();
                        dbinserter.InsertLocationDataMSSQL();
                    }
                }
            }
        }

        public void CreateSQLiteDatabase()
        {
            if (!File.Exists(_sqliteConnectionString))
            {
                string databaseFilePath = _sqliteConnectionString.Replace("Data Source=", "");
                string directoryPath = Path.GetDirectoryName(databaseFilePath);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    SQLiteConnection.CreateFile(databaseFilePath);

                    using (var connection = new SQLiteConnection(_sqliteConnectionString))
                    {
                        connection.Open();
                        string createTablesQuery = @"
                            CREATE TABLE IF NOT EXISTS Customer (
                                CustomerID INTEGER PRIMARY KEY AUTOINCREMENT,
                                Name TEXT,
                                Email TEXT,
                                Phone TEXT
                            );

                            CREATE TABLE IF NOT EXISTS Location (
                                LocationID INTEGER PRIMARY KEY AUTOINCREMENT,
                                CustomerID INTEGER,
                                Address TEXT,
                                FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
                            );

                            CREATE TABLE IF NOT EXISTS SyncLog (
                                LogID INTEGER PRIMARY KEY AUTOINCREMENT,
                                CustomerID INTEGER,
                                Field TEXT,
                                OldValue TEXT,
                                NewValue TEXT,
                                Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                                FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
                            );";

                        using (var command = new SQLiteCommand(createTablesQuery, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                        Console.WriteLine("SQLite Database and Tables Created!");
                    }
                }
                else
                {
                    throw new NotSupportedException("The given path's format is not supported.");
                }
            }
        }
    }
}