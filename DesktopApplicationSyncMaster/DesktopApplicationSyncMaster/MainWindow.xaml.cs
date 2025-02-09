using System;
using System.Collections.Generic;
using System.Windows;
using System.Net.Http;
using System.Threading.Tasks;
using DesktopApplicationSyncMaster;
using DotNET_Assessment;

namespace DotNETAssessment
{
    public partial class MainWindow : Window
    {
        private SyncService syncService;

        public MainWindow()
        {
            InitializeComponent();
            syncService = new SyncService();
            LoadData(); // Load data on startup

        }

        private async void LoadData()
        {
            try
            {
                var customers = await syncService.FetchCustomersFromMSSQL();
                CustomerDataGrid.ItemsSource = customers;

                var locations = await syncService.FetchLocationsFromMSSQL();
                LocationDataGrid.ItemsSource = locations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartSync_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SyncIntervalTextBox.Text, out int interval) && interval > 0)
            {
                syncService.StartSync(interval);
                SyncLogListBox.Items.Add($"Sync started every {interval} seconds...");
            }
            else
            {
                MessageBox.Show("Please enter a valid sync interval in seconds.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void ManualFetch_Click(object sender, RoutedEventArgs e)
        {
            SyncLogListBox.Items.Add("Fetching data manually...");
            await syncService.ManualSyncData(); // Use the correct method
            LoadData();
            SyncLogListBox.Items.Add("Data fetched successfully.");
        }

        private async void FetchAPI_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make sure the URL is correct, and the API is running
                    var response = await client.GetStringAsync("https://localhost:7263/api/Customers");

                    // Make sure this control exists in your XAML and is named ApiResponseTextBox
                    ApiResponseTextBox.Text = response;
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"API Request Failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Catch other unexpected exceptions to help with debugging
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
