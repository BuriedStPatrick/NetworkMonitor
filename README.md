# Network Monitor
## Run
In repository root:
```
dotnet restore
cd .\NetworkMonitor.App
dotnet run
```

## Installing as a service
Will be filled in at a later date.

## Rolling File Log
By default, creates a daily log file inside `C:\temp\nm`

## Database Log
### Pre-requisites
* [MSSQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Setup
1. In MSSQL Server, create a database `NM_Log`
2. Ensure whatever the identity of the service you're using has access to `db_owner` for `NM_Log`

## Override configs
Settings can be overriden by adding an `appsettings.Development.json` - it will take precedent over all other configs.
