-- Safe idempotent seed for development

-- Admin user (Username: admin, Password: admin) - SHA256 hex of "admin"
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, DisplayName, Role, Email, PasswordHash)
    VALUES ('admin', 'System Administrator', 'Admin', 'admin@example.com',
            '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918'); -- SHA256("admin")
END

-- 2. SELF-HEAL: Update Admin permissions if they were inserted with defaults (0)
UPDATE Users
SET 
    IsActive = 1,
    CanGenerateLicense = 1,
    CanViewRecords = 1,
    CanManageProduct = 1,
    CanManageUsers = 1
WHERE Username = 'admin';

-- Sample dealer
IF NOT EXISTS (SELECT 1 FROM dbo.Dealers WHERE DealerCode = 'DEALER-001')
BEGIN
    INSERT INTO dbo.Dealers (DealerCode, Name)
    VALUES ('DEALER-001', 'Default Dealer');
END

