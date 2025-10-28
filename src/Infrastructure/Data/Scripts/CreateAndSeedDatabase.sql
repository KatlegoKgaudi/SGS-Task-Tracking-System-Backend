IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'TaskTrackerDB')
BEGIN
    CREATE DATABASE TaskTrackerDB;
END
GO

USE TaskTrackerDB;
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'task_tracker')
BEGIN
    EXEC('CREATE SCHEMA task_tracker');
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE task_tracker.Users (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(50) NOT NULL UNIQUE,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        Role INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaskItems' AND xtype='U')
BEGIN
    CREATE TABLE task_tracker.TaskItems (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000),
        Status INT NOT NULL DEFAULT 0,
        DueDate DATETIME2 NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AssignedUserId INT NULL,
        FOREIGN KEY (AssignedUserId) REFERENCES task_tracker.Users(Id)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RefreshTokens' AND xtype='U')
BEGIN
    CREATE TABLE task_tracker.RefreshTokens (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Token NVARCHAR(500) NOT NULL,
        Expires DATETIME2 NOT NULL,
        Created DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsRevoked BIT NOT NULL DEFAULT 0,
        UserId INT NOT NULL,
        FOREIGN KEY (UserId) REFERENCES task_tracker.Users(Id) ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskItems_Status')
BEGIN
    CREATE INDEX IX_TaskItems_Status ON task_tracker.TaskItems (Status);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskItems_DueDate')
BEGIN
    CREATE INDEX IX_TaskItems_DueDate ON task_tracker.TaskItems (DueDate);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TaskItems_AssignedUserId')
BEGIN
    CREATE INDEX IX_TaskItems_AssignedUserId ON task_tracker.TaskItems (AssignedUserId);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_Token')
BEGIN
    CREATE INDEX IX_RefreshTokens_Token ON task_tracker.RefreshTokens (Token);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_UserId_IsRevoked')
BEGIN
    CREATE INDEX IX_RefreshTokens_UserId_IsRevoked ON task_tracker.RefreshTokens (UserId, IsRevoked);
END
GO

IF NOT EXISTS (SELECT 1 FROM task_tracker.Users)
BEGIN
    -- Insert Super Admins (Role = 2)
    INSERT INTO task_tracker.Users (Username, Email, PasswordHash, Role) VALUES
    ('katlego', 'katlego@sgs.com', 'qL7C1Q9JvJ7V9z7V5Q2q3A==.aN1p5k8V2qL7C1Q9JvJ7V9z7V5Q2q3A==aN1p5k8V2qL7C1Q9JvJ7V9z7V5Q2q3A==', 2),
    ('superadmin', 'admin@sgs.com', 'qL7C1Q9JvJ7V9z7V5Q2q3A==.aN1p5k8V2qL7C1Q9JvJ7V9z7V5Q2q3A==aN1p5k8V2qL7C1Q9JvJ7V9z7V5Q2q3A==', 2);

    -- Insert Regular Users (Role = 0)
    INSERT INTO task_tracker.Users (Username, Email, PasswordHash, Role) VALUES
    ('john_doe', 'john.doe@sgs.com', 'rL4oAvkMlWOxbbGFPNMvUe==.WEODBfqFfYK3VeZ6eB6s8QqKq6KWEODBfqFfYK3VeZ6eB6s8QqKq6K', 0),
    ('jane_smith', 'jane.smith@sgs.com', 'rL4oAvkMlWOxbbGFPNMvUe==.WEODBfqFfYK3VeZ6eB6s8QqKq6KWEODBfqFfYK3VeZ6eB6s8QqKq6K', 0),
    ('mike_wilson', 'mike.wilson@sgs.com', 'rL4oAvkMlWOxbbGFPNMvUe==.WEODBfqFfYK3VeZ6eB6s8QqKq6KWEODBfqFfYK3VeZ6eB6s8QqKq6K', 0),
    ('sarah_jones', 'sarah.jones@sgs.com', 'rL4oAvkMlWOxbbGFPNMvUe==.WEODBfqFfYK3VeZ6eB6s8QqKq6KWEODBfqFfYK3VeZ6eB6s8QqKq6K', 0);

    INSERT INTO task_tracker.TaskItems (Title, Description, Status, DueDate, AssignedUserId) VALUES
    ('Implement User Authentication', 'Create login and registration system with JWT tokens', 1, DATEADD(day, 7, GETUTCDATE()), 3),
    ('Design Database Schema', 'Design and implement the task tracking database', 2, DATEADD(day, 3, GETUTCDATE()), 4),
    ('Create API Endpoints', 'Develop REST API endpoints for task management', 0, DATEADD(day, 10, GETUTCDATE()), 3),
    ('Write Unit Tests', 'Create comprehensive unit tests for all services', 0, DATEADD(day, 14, GETUTCDATE()), 5),
    ('Frontend Development', 'Build Angular frontend application', 1, DATEADD(day, 21, GETUTCDATE()), 6),
    ('Deploy to Production', 'Deploy application to production environment', 0, DATEADD(day, 30, GETUTCDATE()), 4),
    ('Performance Optimization', 'Optimize database queries and API performance', 0, DATEADD(day, 25, GETUTCDATE()), 5),
    ('Security Audit', 'Conduct security review and implement fixes', 0, DATEADD(day, 18, GETUTCDATE()), 6);

    PRINT 'Database seeded successfully with test data.';
    PRINT 'Super Admin credentials:';
    PRINT '  Username: katlego, Password: SuperAdmin123!';
    PRINT '  Username: superadmin, Password: SuperAdmin123!';
    PRINT 'Regular User credentials:';
    PRINT '  Username: john_doe, Password: User123!';
    PRINT '  Username: jane_smith, Password: User123!';
END
ELSE
BEGIN
    PRINT 'Database already contains data. Skipping seeding.';
END
GO