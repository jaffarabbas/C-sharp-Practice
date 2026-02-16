-- Create Database if not exists
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HRMS')
BEGIN
    CREATE DATABASE HRMS;
END
GO

USE HRMS;
GO

-- Create Leave table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Leave')
BEGIN
    CREATE TABLE Leave (
        Id INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        Balance INT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        UpdatedAt DATETIME2 DEFAULT GETDATE()
    );
    
    CREATE INDEX IX_Leave_UserId ON Leave(UserId);
END
GO

-- Insert sample data
IF NOT EXISTS (SELECT * FROM Leave WHERE UserId = 1023)
BEGIN
    INSERT INTO Leave (UserId, Balance) VALUES (1023, 15);
END
GO

-- Verify data
SELECT * FROM Leave;
GO
