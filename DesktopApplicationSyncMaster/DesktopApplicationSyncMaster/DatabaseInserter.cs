using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNET_Assessment
{
    public class DatabaseInserter
    {
        private readonly string _mssqlConnectionString;

        public DatabaseInserter(string mssqlConnection)
        {
            _mssqlConnectionString = mssqlConnection;
        }

        public void InsertCustomerDataMSSQL()
        {
            using (SqlConnection conn = new SqlConnection(_mssqlConnectionString))
            {
                conn.Open();
                string insertCustomerQuery = @"
                   INSERT INTO Customer (Name, Email, Phone) VALUES
                    ('John Doe', 'john.doe@example.com', '123-456-7890'),
                    ('Jane Smith', 'jane.smith@example.com', '234-567-8901'),
                    ('Alice Johnson', 'alice.johnson@example.com', '345-678-9012'),
                    ('Bob Brown', 'bob.brown@example.com', '456-789-0123'),
                    ('Charlie Davis', 'charlie.davis@example.com', '567-890-1234'),
                    ('Eva Wilson', 'eva.wilson@example.com', '678-901-2345'),
                    ('Frank Miller', 'frank.miller@example.com', '789-012-3456'),
                    ('Grace Lee', 'grace.lee@example.com', '890-123-4567'),
                    ('Henry Garcia', 'henry.garcia@example.com', '901-234-5678'),
                    ('Irene Martinez', 'irene.martinez@example.com', '012-345-6789'),
                    ('Kevin Taylor', 'kevin.taylor@example.com', '123-456-7891'),
                    ('Laura Clark', 'laura.clark@example.com', '234-567-8902'),
                    ('Michael White', 'michael.white@example.com', '345-678-9013'),
                    ('Nancy Hall', 'nancy.hall@example.com', '456-789-0124'),
                    ('Oliver Allen', 'oliver.allen@example.com', '567-890-1235'),
                    ('Patricia Young', 'patricia.young@example.com', '678-901-2346'),
                    ('Quinn King', 'quinn.king@example.com', '789-012-3457'),
                    ('Rachel Wright', 'rachel.wright@example.com', '890-123-4568'),
                    ('Samuel Scott', 'samuel.scott@example.com', '901-234-5679'),
                    ('Tina Green', 'tina.green@example.com', '012-345-6790');
                ";

                using (SqlCommand cmd = new SqlCommand(insertCustomerQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Customer Data Inserted into MSSQL!");
            }
        }

        public void InsertLocationDataMSSQL()
        {
            using (SqlConnection conn = new SqlConnection(_mssqlConnectionString))
            {
                conn.Open();
                string insertLocationQuery = @"
                    INSERT INTO Location (CustomerID, Address) VALUES
                        (1, '123 Main St, Springfield, IL'),
                        (1, '456 Elm St, Chicago, IL'),
                        (2, '789 Oak St, New York, NY'),
                        (2, '321 Pine St, Los Angeles, CA'),
                        (3, '654 Maple St, Houston, TX'),
                        (3, '987 Birch St, Phoenix, AZ'),
                        (4, '111 Cedar St, Philadelphia, PA'),
                        (4, '222 Walnut St, San Antonio, TX'),
                        (5, '333 Spruce St, San Diego, CA'),
                        (5, '444 Cherry St, Dallas, TX'),
                        (6, '555 Willow St, San Jose, CA'),
                        (6, '666 Poplar St, Austin, TX'),
                        (7, '777 Ash St, Jacksonville, FL'),
                        (7, '888 Fir St, Fort Worth, TX'),
                        (8, '999 Pineapple St, Columbus, OH'),
                        (8, '1010 Orange St, Charlotte, NC'),
                        (9, '1212 Banana St, San Francisco, CA'),
                        (9, '1313 Grape St, Indianapolis, IN'),
                        (10, '1414 Mango St, Seattle, WA'),
                        (10, '1515 Peach St, Denver, CO'),
                        (11, '1616 Apple St, Boston, MA'),
                        (11, '1717 Berry St, Atlanta, GA'),
                        (12, '1818 Cherry St, Miami, FL'),
                        (12, '1919 Date St, Nashville, TN'),
                        (13, '2020 Fig St, Portland, OR'),
                        (13, '2121 Grapefruit St, Las Vegas, NV'),
                        (14, '2222 Honeydew St, Orlando, FL'),
                        (14, '2323 Kiwi St, Minneapolis, MN'),
                        (15, '2424 Lemon St, Tampa, FL'),
                        (15, '2525 Lime St, Pittsburgh, PA');
                ";

                using (SqlCommand cmd = new SqlCommand(insertLocationQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Location Data Inserted into MSSQL!");
            }
        }



    }
}
