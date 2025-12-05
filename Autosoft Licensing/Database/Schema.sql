-- Autosoft Licensing - SQL Server schema
-- Safe re-creation (dev only). Remove DROP in production migrations.

IF OBJECT_ID('dbo.LicenseRequestModules', 'U') IS NOT NULL DROP TABLE dbo.LicenseRequestModules;
IF OBJECT_ID('dbo.LicenseModules', 'U') IS NOT NULL DROP TABLE dbo.LicenseModules;
IF OBJECT_ID('dbo.Modules', 'U') IS NOT NULL DROP TABLE dbo.Modules;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Dealers', 'U') IS NOT NULL DROP TABLE dbo.Dealers;
IF OBJECT_ID('dbo.LicenseRequests', 'U') IS NOT NULL DROP TABLE dbo.LicenseRequests;
IF OBJECT_ID('dbo.Licenses', 'U') IS NOT NULL DROP TABLE dbo.Licenses;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

-- Users
CREATE TABLE Users (
  Id INT IDENTITY PRIMARY KEY,
  Username NVARCHAR(100) NOT NULL,
  DisplayName NVARCHAR(200) NOT NULL,
  Role NVARCHAR(20) NOT NULL,      -- Admin, Support
  Email NVARCHAR(200) NULL,
  PasswordHash NVARCHAR(512) NOT NULL,
  CreatedUtc DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedUtc DEFAULT (SYSUTCDATETIME())
);
CREATE UNIQUE INDEX UX_Users_Username ON Users(Username);

-- Products and Modules (Product has zero or more Modules)
CREATE TABLE Products (
  Id INT IDENTITY PRIMARY KEY,
  ProductID NVARCHAR(100) NOT NULL,    -- business key shown in UI
  Name NVARCHAR(200) NULL,
  Description NVARCHAR(MAX) NULL,
  ReleaseNotes NVARCHAR(MAX) NULL,
  CreatedBy NVARCHAR(100) NULL,
  CreatedUtc DATETIME2 NOT NULL CONSTRAINT DF_Products_CreatedUtc DEFAULT (SYSUTCDATETIME()),
  LastModifiedUtc DATETIME2 NOT NULL CONSTRAINT DF_Products_LastModifiedUtc DEFAULT (SYSUTCDATETIME()),
  IsDeleted BIT NOT NULL CONSTRAINT DF_Products_IsDeleted DEFAULT (0)
);
CREATE UNIQUE INDEX UX_Products_ProductID ON Products(ProductID);
-- Enforce unique product names (excluding NULLs)
CREATE UNIQUE INDEX UX_Products_Name ON Products(Name) WHERE Name IS NOT NULL;

CREATE TABLE Modules (
  Id INT IDENTITY PRIMARY KEY,
  ProductId INT NOT NULL,              -- FK to Products.Id (surrogate key)
  ModuleCode NVARCHAR(100) NOT NULL,
  Name NVARCHAR(200) NULL,
  Description NVARCHAR(4000) NULL,
  IsActive BIT NOT NULL CONSTRAINT DF_Modules_IsActive DEFAULT (1),

  CONSTRAINT FK_Modules_Product FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_Modules_Product_ModuleCode ON Modules(ProductId, ModuleCode);
CREATE INDEX IX_Modules_ProductId ON Modules(ProductId);

-- Dealers (optional reference table). You currently store DealerCode as text in other tables.
CREATE TABLE Dealers (
  Id INT IDENTITY PRIMARY KEY,
  DealerCode NVARCHAR(50) NOT NULL,
  Name NVARCHAR(200) NULL,
  CreatedUtc DATETIME2 NOT NULL CONSTRAINT DF_Dealers_CreatedUtc DEFAULT (SYSUTCDATETIME())
);
CREATE UNIQUE INDEX UX_Dealers_DealerCode ON Dealers(DealerCode);

-- License Requests (.ARL history)
CREATE TABLE LicenseRequests (
  Id INT IDENTITY PRIMARY KEY,
  CompanyName NVARCHAR(255) NOT NULL,
  ProductID NVARCHAR(100) NOT NULL,  -- references Products.ProductID logically
  DealerCode NVARCHAR(50) NOT NULL,
  LicenseType NVARCHAR(20) NOT NULL, -- Demo|Paid (ARL uses "Paid", but we map to enum internally)
  RequestedPeriodMonths INT NOT NULL,
  LicenseKey NVARCHAR(200) NOT NULL,
  CurrencyCode NVARCHAR(10) NULL,
  RequestDateUtc DATETIME2 NOT NULL CONSTRAINT DF_LicenseRequests_RequestDateUtc DEFAULT (SYSUTCDATETIME()),
  RequestFileBase64 NVARCHAR(MAX) NULL,
  CreatedByUserId INT NOT NULL,

  CONSTRAINT CK_LicenseRequests_LicenseType CHECK (LicenseType IN ('Demo','Paid')),
  CONSTRAINT FK_LicenseRequests_User FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);
CREATE INDEX IX_LicenseRequests_Product ON LicenseRequests(ProductID);
CREATE INDEX IX_LicenseRequests_Dealer ON LicenseRequests(DealerCode);

-- Requested modules per request
CREATE TABLE LicenseRequestModules (
  LicenseRequestId INT NOT NULL,
  ModuleId INT NOT NULL,
  CONSTRAINT PK_LicenseRequestModules PRIMARY KEY (LicenseRequestId, ModuleId),
  CONSTRAINT FK_LicenseRequestModules_Request FOREIGN KEY (LicenseRequestId) REFERENCES LicenseRequests(Id) ON DELETE CASCADE,
  CONSTRAINT FK_LicenseRequestModules_Module FOREIGN KEY (ModuleId) REFERENCES Modules(Id)
);

-- Licenses (.ASL activations)
CREATE TABLE Licenses (
  Id INT IDENTITY PRIMARY KEY,
  CompanyName NVARCHAR(255) NOT NULL,
  ProductID NVARCHAR(100) NOT NULL,
  DealerCode NVARCHAR(50) NOT NULL,
  LicenseKey NVARCHAR(200) NOT NULL,
  LicenseType NVARCHAR(20) NOT NULL,  -- Demo|Subscription|Permanent (maps to enum)
  ValidFromUtc DATETIME2 NOT NULL,
  ValidToUtc DATETIME2 NOT NULL,
  CurrencyCode NVARCHAR(10) NULL,
  Status NVARCHAR(20) NOT NULL,       -- Valid|Expired|Invalid|Deleted
  ImportedOnUtc DATETIME2 NOT NULL CONSTRAINT DF_Licenses_ImportedOnUtc DEFAULT (SYSUTCDATETIME()),
  ImportedByUserId INT NULL,
  RawAslBase64 NVARCHAR(MAX) NULL,
  Remarks NVARCHAR(MAX) NULL,

  CONSTRAINT CK_Licenses_LicenseType CHECK (LicenseType IN ('Demo','Subscription','Permanent')),
  CONSTRAINT CK_Licenses_Status CHECK (Status IN ('Valid','Expired','Invalid','Deleted')),
  CONSTRAINT FK_Licenses_User FOREIGN KEY (ImportedByUserId) REFERENCES Users(Id)
);

-- Licensed modules per license
CREATE TABLE LicenseModules (
  LicenseId INT NOT NULL,
  ModuleId INT NOT NULL,
  CONSTRAINT PK_LicenseModules PRIMARY KEY (LicenseId, ModuleId),
  CONSTRAINT FK_LicenseModules_License FOREIGN KEY (LicenseId) REFERENCES Licenses(Id) ON DELETE CASCADE,
  CONSTRAINT FK_LicenseModules_Module FOREIGN KEY (ModuleId) REFERENCES Modules(Id)
);

-- Helpful indexes
CREATE INDEX IX_Licenses_Product ON Licenses(ProductID, CompanyName);
CREATE INDEX IX_Licenses_Status ON Licenses(Status);
CREATE INDEX IX_Licenses_LicenseKey ON Licenses(LicenseKey);

-- Add IsDeleted column if it does not exist
IF COL_LENGTH('dbo.Products', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD IsDeleted bit NOT NULL CONSTRAINT DF_Products_IsDeleted DEFAULT(0);
END

-- Users table granular permissions upgrade
-- Adds IsActive and fine-grained permission flags.
-- Existing rows default to enabled (IsActive=1) and no permissions (except admin to be set in Seed.sql).

IF COL_LENGTH('dbo.Users', 'IsActive') IS NULL
ALTER TABLE dbo.Users ADD [IsActive] BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT(1) WITH VALUES;

IF COL_LENGTH('dbo.Users', 'CanGenerateLicense') IS NULL
ALTER TABLE dbo.Users ADD [CanGenerateLicense] BIT NOT NULL CONSTRAINT DF_Users_CanGenerateLicense DEFAULT(0) WITH VALUES;

IF COL_LENGTH('dbo.Users', 'CanViewRecords') IS NULL
ALTER TABLE dbo.Users ADD [CanViewRecords] BIT NOT NULL CONSTRAINT DF_Users_CanViewRecords DEFAULT(0) WITH VALUES;

IF COL_LENGTH('dbo.Users', 'CanManageProduct') IS NULL
ALTER TABLE dbo.Users ADD [CanManageProduct] BIT NOT NULL CONSTRAINT DF_Users_CanManageProduct DEFAULT(0) WITH VALUES;

IF COL_LENGTH('dbo.Users', 'CanManageUsers') IS NULL
ALTER TABLE dbo.Users ADD [CanManageUsers] BIT NOT NULL CONSTRAINT DF_Users_CanManageUsers DEFAULT(0) WITH VALUES;