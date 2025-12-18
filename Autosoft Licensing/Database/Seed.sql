-- Safe idempotent seed for development
-- Ensures admin has the new permission columns set and "self-heals" existing admin records

-- 1. Ensure Admin Exists (with explicit permission columns)
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (
        Username,
        DisplayName,
        Role,
        Email,
        PasswordHash,
        CreatedUtc,
        IsActive,
        CanGenerateLicense,
        CanViewRecords,
        CanManageProduct,
        CanManageUsers
    )
    VALUES (
        'admin',
        'System Administrator',
        'Admin',
        'admin@example.com',
        '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', -- SHA256("admin")
        SYSUTCDATETIME(),
        1, -- IsActive
        1, -- CanGenerateLicense
        1, -- CanViewRecords
        1, -- CanManageProduct
        1  -- CanManageUsers
    );
END

-- 2. SELF-HEAL: Update Admin permissions if they were inserted with defaults (0)
-- Use COALESCE/CASE to avoid overwriting intentional existing values (safe for dev seeding)
UPDATE dbo.Users
SET
    Email = COALESCE(Email, 'admin@example.com'),
    DisplayName = COALESCE(DisplayName, 'System Administrator'),
    PasswordHash = CASE WHEN PasswordHash IS NULL OR PasswordHash = '' THEN '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918' ELSE PasswordHash END,
    CreatedUtc = COALESCE(CreatedUtc, SYSUTCDATETIME()),
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';


