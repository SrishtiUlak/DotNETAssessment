# Desktop Application SyncMaster

A .NET desktop app to sync customer and location data between MSSQL and SQLite databases and interact with a REST API for customer data.

## Prerequisites
- **.NET 5.0+**
- **Microsoft SQL Server (MSSQL)**
- **SQLite**
- **Visual Studio**

## Setup

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/your-username/DesktopApplicationSyncMaster.git
    ```

2. **Install NuGet Packages**:
    ```bash
    dotnet restore
    ```

3. **Create `CustomerDB` Database in MSSQL**:
    ```sql
    CREATE DATABASE CustomerDB;
    ```


5. **Check API URL**:
    Make sure the API endpoint is correct: 
    ```text
    https://localhost:7263/api/Customers
    ```

## Usage

- **Set Sync Interval**: Enter seconds, click **Start Sync**.
- **Manual Fetch**: Click **Manual Fetch** to load data.
- **Fetch API Data**: Click **Fetch API** to display API response.

## API
- **GET** `/api/Customers`: Fetch customer data in JSON.

## License
MIT License.
