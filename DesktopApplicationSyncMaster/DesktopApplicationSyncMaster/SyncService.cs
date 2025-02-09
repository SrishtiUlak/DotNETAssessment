using DesktopApplicationSyncMaster;
using DotNET_Assessment;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class SyncService
{
    private readonly string mssqlConnectionString = "Server=ULAK\\SQLEXPRESS;Database=CustomerDB;Integrated Security=True;";
    private readonly string sqliteConnectionString;
    private Timer syncTimer;

    public SyncService()
    {
        string relativeDbPath = "\\SqlLiteDatabase\\CustomerDB.sqlite";
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string projectBasePath = baseDirectory; // Default in case the loop fails

        DirectoryInfo directory = new DirectoryInfo(baseDirectory);

        while (directory != null && !directory.Name.Equals("DotNETAssessment", StringComparison.OrdinalIgnoreCase))
        {
            directory = directory.Parent;
        }

        if (directory != null)
        {
            projectBasePath = directory.FullName;
            Console.WriteLine("Project Base Path: " + projectBasePath);
        }
        else
        {
            Console.WriteLine("Warning: 'DotNETAssessment' folder not found. Using Base Directory.");
        }

        // Ensure correct path construction
        string sqliteDatabasePath = projectBasePath+relativeDbPath;

        // Assign corrected connection string
        sqliteConnectionString = $"Data Source={sqliteDatabasePath};Version=3;";

        Console.WriteLine("SQLite Database Path: " + sqliteDatabasePath);



        DatabaseInitializer dbInit = new DatabaseInitializer(mssqlConnectionString, sqliteConnectionString);
        dbInit.CreateMSSQLDatabase();
        dbInit.CreateSQLiteDatabase();
       
    }


    public void StartSync(int syncIntervalInSeconds)
    {
        syncTimer?.Dispose(); // Prevent multiple timers
        syncTimer = new Timer(async _ => await SyncData(), null, TimeSpan.Zero, TimeSpan.FromSeconds(syncIntervalInSeconds));
    }

    public async Task SyncData()
    {
        try
        {
            var customers = await FetchCustomersFromMSSQL();
            await SyncToSQLite(customers);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sync failed: {ex.Message}");
        }
    }

    public async Task<List<Customer>> FetchCustomersFromMSSQL()
    {
        var customers = new List<Customer>();
        using (var connection = new SqlConnection(mssqlConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("SELECT * FROM Customer", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    customers.Add(new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3)
                    });
                }
            }
        }
        return customers;
    }

    public async Task<List<Location>> FetchLocationsFromMSSQL()
    {
        var locations = new List<Location>();
        using (var connection = new SqlConnection(mssqlConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand("SELECT * FROM Location", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    locations.Add(new Location
                    {
                        LocationID = reader.GetInt32(0),
                        CustomerID = reader.GetInt32(1),
                        Address = reader.GetString(2)
                    });
                }
            }
        }
        return locations;
    }

    public async Task SyncToSQLite(List<Customer> customers)
    {
        using (var connection = new SQLiteConnection(sqliteConnectionString))
        {
            await connection.OpenAsync();
            using (var transaction = connection.BeginTransaction())
            {
                foreach (var customer in customers)
                {
                    var existingCustomer = await GetExistingCustomer(connection, customer.CustomerID);
                    string changes = existingCustomer != null ? CompareCustomers(existingCustomer, customer) : null;

                    using (var cmd = new SQLiteCommand(connection))
                    {
                        if (existingCustomer != null)
                        {
                            if (!string.IsNullOrEmpty(changes))
                            {
                                await LogChange(connection, customer.CustomerID, changes, transaction);
                            }
                            await UpdateCustomer(cmd, customer);
                        }
                        else
                        {
                            await InsertCustomer(cmd, customer);
                        }
                    }
                }
                transaction.Commit();
            }
        }
    }

    private async Task<Customer> GetExistingCustomer(SQLiteConnection connection, int customerId)
    {
        using (var cmd = new SQLiteCommand("SELECT * FROM Customer WHERE CustomerID = @CustomerID", connection))
        {
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Customer
                    {
                        CustomerID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3)
                    };
                }
            }
        }
        return null;
    }

    private string CompareCustomers(Customer oldCustomer, Customer newCustomer)
    {
        List<string> changes = new List<string>();

        if (oldCustomer.Name != newCustomer.Name)
            changes.Add($"Name changed from '{oldCustomer.Name}' to '{newCustomer.Name}'");

        if (oldCustomer.Email != newCustomer.Email)
            changes.Add($"Email changed from '{oldCustomer.Email}' to '{newCustomer.Email}'");

        if (oldCustomer.Phone != newCustomer.Phone)
            changes.Add($"Phone changed from '{oldCustomer.Phone}' to '{newCustomer.Phone}'");

        return changes.Count > 0 ? string.Join(", ", changes) : null;
    }

    private async Task LogChange(SQLiteConnection connection, int customerId, string changeDescription, SQLiteTransaction transaction)
    {
        using (var cmd = new SQLiteCommand("INSERT INTO SyncLog (CustomerID, ChangeDescription, Timestamp) VALUES (@CustomerID, @ChangeDescription, @Timestamp)", connection, transaction))
        {
            cmd.Parameters.AddWithValue("@CustomerID", customerId);
            cmd.Parameters.AddWithValue("@ChangeDescription", changeDescription);
            cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
            await cmd.ExecuteNonQueryAsync();
        }
    }

    private async Task InsertCustomer(SQLiteCommand cmd, Customer customer)
    {
        cmd.CommandText = "INSERT INTO Customer (CustomerID, Name, Email, Phone) VALUES (@CustomerID, @Name, @Email, @Phone)";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
        cmd.Parameters.AddWithValue("@Name", customer.Name);
        cmd.Parameters.AddWithValue("@Email", customer.Email);
        cmd.Parameters.AddWithValue("@Phone", customer.Phone);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task UpdateCustomer(SQLiteCommand cmd, Customer customer)
    {
        cmd.CommandText = "UPDATE Customer SET Name = @Name, Email = @Email, Phone = @Phone WHERE CustomerID = @CustomerID";
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
        cmd.Parameters.AddWithValue("@Name", customer.Name);
        cmd.Parameters.AddWithValue("@Email", customer.Email);
        cmd.Parameters.AddWithValue("@Phone", customer.Phone);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task ManualSyncData()
    {
        var customers = await FetchCustomersFromMSSQL();
        await SyncToSQLite(customers);
    }
  
}

public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class Location
{
    public int LocationID { get; set; }
    public int CustomerID { get; set; }
    public string Address { get; set; }
}
