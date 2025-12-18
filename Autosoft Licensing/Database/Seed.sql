-- Database/Seed.sql

-- 1. Ensure Admin Exists
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (
        Username, DisplayName, Role, Email, PasswordHash, CreatedUtc, 
        IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
    )
    VALUES (
        'admin', 'System Administrator', 'Admin', 'admin@example.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- LOWERCASE HEX for "admin"
        SYSUTCDATETIME(), 1, 1, 1, 1, 1
    );
END

-- 2. SELF-HEALING: Force Update Existing Admin
-- This overwrites the old Base64 or Uppercase hash with the correct Lowercase Hex
UPDATE dbo.Users
SET
    PasswordHash = '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';