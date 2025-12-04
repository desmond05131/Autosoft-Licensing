-- Safe idempotent seed for development

-- Admin user (Username: admin, Password: admin) - SHA256 hex of "admin"
IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin')
BEGIN
    INSERT INTO dbo.Users (Username, DisplayName, Role, Email, PasswordHash)
    VALUES ('admin', 'System Administrator', 'Admin', 'admin@example.com',
            '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918'); -- SHA256("admin")
END

-- Sample dealer
IF NOT EXISTS (SELECT 1 FROM dbo.Dealers WHERE DealerCode = 'DEALER-001')
BEGIN
    INSERT INTO dbo.Dealers (DealerCode, Name)
    VALUES ('DEALER-001', 'Default Dealer');
END

-- Sample product (business ProductID = PROD-001)
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductID = 'PROD-001')
BEGIN
    INSERT INTO dbo.Products (ProductID, Name)
    VALUES ('PROD-001', 'Sample Product');
END

-- Additional sample product for testing
IF NOT EXISTS (SELECT 1 FROM dbo.Products WHERE ProductID = 'SAMPLE-PRODUCT')
BEGIN
    INSERT INTO dbo.Products (ProductID, Name)
    VALUES ('SAMPLE-PRODUCT', 'Sample Product');
END

DECLARE @ProductId INT = (SELECT TOP(1) Id FROM dbo.Products WHERE ProductID = 'PROD-001');
DECLARE @SampleProductId INT = (SELECT TOP(1) Id FROM dbo.Products WHERE ProductID = 'SAMPLE-PRODUCT');

-- Sample modules for PROD-001
IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @ProductId AND ModuleCode = 'MODULE-001')
BEGIN
    INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
    VALUES (@ProductId, 'MODULE-001', 'Module 1', 'Sample Module 1');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @ProductId AND ModuleCode = 'MODULE-002')
BEGIN
    INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
    VALUES (@ProductId, 'MODULE-002', 'Module 2', 'Sample Module 2');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @ProductId AND ModuleCode = 'MODULE-003')
BEGIN
    INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
    VALUES (@ProductId, 'MODULE-003', 'Module 3', 'Sample Module 3');
END

-- Sample modules for SAMPLE-PRODUCT (used by smoke tests and E2E tests)
IF @SampleProductId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @SampleProductId AND ModuleCode = 'MODULE-001')
    BEGIN
        INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
        VALUES (@SampleProductId, 'MODULE-001', 'Module 1', 'Sample Module 1 for Sample Product');
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @SampleProductId AND ModuleCode = 'MODULE-002')
    BEGIN
        INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
        VALUES (@SampleProductId, 'MODULE-002', 'Module 2', 'Sample Module 2 for Sample Product');
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE ProductId = @SampleProductId AND ModuleCode = 'MODULE-003')
    BEGIN
        INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description)
        VALUES (@SampleProductId, 'MODULE-003', 'Module 3', 'Sample Module 3 for Sample Product');
    END
END