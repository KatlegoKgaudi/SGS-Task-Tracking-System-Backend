#!/bin/bash
# Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
for i in {1..50}; do
    if /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "YourStrong!Password123" -Q "SELECT 1;" > /dev/null 2>&1; then
        echo "SQL Server is ready."
        break
    else
        echo "Not ready yet... waiting ($i/50)"
        sleep 2
    fi
done

echo "Running database creation and seeding script..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "YourStrong!Password123" -d master -i /scripts/CreateAndSeedDatabase.sql

if [ $? -eq 0 ]; then
    echo "Database setup completed successfully!"
    echo "Connection string for API:"
    echo "Server=sgs-tasktracker-db,1433;Database=TaskTrackerDB;User Id=sa;Password=YourStrong!Password123;TrustServerCertificate=true;"
else
    echo "Database setup failed!"
    exit 1
fi