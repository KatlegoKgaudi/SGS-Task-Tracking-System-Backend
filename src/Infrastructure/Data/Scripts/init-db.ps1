# Wait for SQL Server to be ready
Write-Host "Waiting for SQL Server to start..."
for ($i = 1; $i -le 50; $i++) {
    try {
        Invoke-Sqlcmd -ServerInstance "localhost" -Username "sa" -Password "YourStrong!Password123" -Query "SELECT 1" -ErrorAction Stop
        Write-Host "SQL Server is ready."
        break
    }
    catch {
        Write-Host "Not ready yet... waiting ($i/50)"
        Start-Sleep -Seconds 2
    }
}

Write-Host "Running database creation and seeding script..."
Invoke-Sqlcmd -ServerInstance "localhost" -Username "sa" -Password "YourStrong!Password123" -InputFile "C:\scripts\CreateAndSeedDatabase.sql"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database setup completed successfully!"
    Write-Host "Connection string for API:"
    Write-Host "Server=sgs-tasktracker-db,1433;Database=TaskTrackerDB;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=true;"
} else {
    Write-Host "Database setup failed!"
    exit 1
}