-- Database/Seed.sql

-- 1. Ensure Admin Exists
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, DisplayName, Role, Email, PasswordHash, CreatedUtc, IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers)
    VALUES (
        'admin', 
        'System Administrator', 
        'Admin', 
        'admin@example.com', 
        'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', -- Correct Base64 Hash for "admin"
        SYSUTCDATETIME(), 
        1, 1, 1, 1, 1
    );
END

-- 2. FORCE UPDATE (Self-Healing)
-- This line is CRITICAL. It overwrites the incorrect "Hex" hash currently in your DB with the correct "Base64" hash.
UPDATE Users
SET 
    PasswordHash = 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';