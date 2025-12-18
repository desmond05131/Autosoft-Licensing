-- Database/Seed.sql

-- 1. Ensure Admin Exists if strictly missing
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (
        Username, DisplayName, Role, Email, PasswordHash, CreatedUtc, 
        IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
    )
    VALUES (
        'admin', 'System Administrator', 'Admin', 'admin@example.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- SHA256("admin")
        SYSUTCDATETIME(), 1, 1, 1, 1, 1
    );
END

-- 2. FORCE UPDATE: Overwrite permissions AND Password to ensure access is restored
UPDATE dbo.Users
SET
    -- This hash is for the password: "admin"
    PasswordHash = '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918',
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';