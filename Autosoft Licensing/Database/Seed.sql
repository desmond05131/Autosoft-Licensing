-- Database/Seed.sql

-- 1. Calculate the Hash for 'admin' (SHA256 -> Base64)
-- This ensures it matches the C# logic: Convert.ToBase64String(SHA256(UTF8("admin")))
DECLARE @adminPwd VARBINARY(MAX) = HASHBYTES('SHA2_256', 'admin');
DECLARE @adminHash NVARCHAR(MAX) = CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@adminPwd"))', 'VARCHAR(MAX)');

-- 2. Create Admin if missing
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (
        Username, DisplayName, Role, Email, PasswordHash, CreatedUtc, 
        IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
    )
    VALUES (
        'admin', 'System Administrator', 'Admin', 'admin@example.com',
        @adminHash, -- Uses the calculated Base64 hash
        SYSUTCDATETIME(), 1, 1, 1, 1, 1
    );
END

-- 3. FORCE UPDATE: Update the existing admin user with the correct Base64 hash
-- This fixes the issue where the old "Hex" hash is currently stored
UPDATE dbo.Users
SET
    PasswordHash = @adminHash,
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';