# Autosoft Licensing System

## 📖 Overview
**Autosoft Licensing** is a Windows Forms (WinForms) desktop application designed to manage the lifecycle of software licenses for AutoCount plugins. It serves as an administrative tool that allows internal staff to:
* Manage Users (Admins/Staff).
* Manage Products (Plugins).
* Process License Requests (`.arl` files).
* Generate Signed License Files (`.asl` files).
* Track License History.

The system implements a secure cryptographic pipeline to ensure that licenses cannot be tampered with or forged.

---

## 🏗 System Architecture
The application follows a **Service-Oriented Architecture** with strict separation of concerns:

* **UI Layer (`/UI`)**: Contains Forms and User Controls (Pages). It handles user interaction and delegates logic to services.
* **Service Layer (`/Services`)**: Contains the business logic (e.g., `LicenseRequestService`, `EncryptionService`).
* **Data Access Layer (`/Data`, `/Database`)**: Handles SQL Server interactions via `SqlConnectionFactory` and ADO.NET.
* **Models (`/Models`)**: POCO classes representing database entities and JSON DTOs.

### 🔑 Key Technologies
* **.NET Framework**: Core runtime.
* **Windows Forms**: UI Framework.
* **MSSQL Server**: Relational database for storage.
* **AES-256 & SHA-256**: Security standards for license generation.
* **Newtonsoft.Json**: JSON serialization for license files.

---

## ⚙️ Logic & Workflows

### 1. The Licensing Logic (Core)
The heart of the application is the **License Generation Pipeline**. This process converts a raw request into a secured, usable license.

#### **Step 1: License Request (`.arl`)**
Clients generate a **License Request File** (`.arl`). This is a standard JSON file containing:
* `CompanyName`: The client's registered name.
* `ProductID`: The ID of the plugin they want to use.
* `DealerCode`: The dealer managing the client.
* `RequestedPeriodMonths`: Duration of the license.

#### **Step 2: Validation & Processing**
When the Admin uploads an `.arl` file in the **Generate License** module:
1.  **Parsing**: The `ArlReaderService` reads the file and deserializes the JSON.
2.  **Business Validation**: The `LicenseRequestService` checks:
    * Are all mandatory fields present?
    * Is the Product ID valid?
    * Is the requested duration within allowed limits (e.g., max 10 years)?

#### **Step 3: Security & Encryption (The "Black Box")**
Once validated, the `AslGeneratorService` and `EncryptionService` take over:
1.  **Key Generation**: A unique `LicenseKey` is generated using `LicenseKeyGenerator`.
2.  **Canonicalization**: The license data object is converted to a "Canonical JSON" string. This ensures that field order and spacing are identical every time, which is critical for hashing.
3.  **Checksum**: A **SHA-256** hash is calculated from the canonical JSON. This checksum is embedded into the license data.
4.  **Encryption**: The entire JSON payload (including the checksum) is encrypted using **AES-256-CBC** (Cipher Block Chaining).
    * **Key**: Defined in `App.config`.
    * **IV**: Defined in `App.config`.
5.  **Signing**: The final output is encoded in Base64.

#### **Step 4: Output (`.asl`)**
The result is an **AutoCount Signed License** (`.asl`) file. This file acts as a "digital key" for the client. The client software will decrypt it, recalculate the hash, and verify integrity before unlocking features.

---

## 📦 Module Breakdown

### 1. Authentication Module (`LoginPage`)
* **Function**: Gatekeeper for the application.
* **Logic**: Accepts Username/Password. Uses `UserService` to query the `Users` table.
* **Security**: Prevents unauthorized access to the license generator.

### 2. User Management (`ManageUserPage`)
* **Function**: CRUD (Create, Read, Update, Delete) for system operators.
* **Features**:
    * Add new admins or support staff.
    * Update passwords.
    * Assign active/inactive status to control access.

### 3. Product Management (`ManageProductPage`)
* **Function**: Registry of software products that can be licensed.
* **Logic**:
    * Stores `ProductID`, `ProductName`, and `Description`.
    * Only products defined here can be referenced in an `.arl` file.

### 4. License Generation UI (`GenerateLicensePage`)
* **Function**: The main workspace for admins.
* **Features**:
    * **Upload**: Drag-and-drop or select `.arl` files.
    * **Editor**: Allows the admin to override the requested duration (e.g., changing a 1-month request to a 1-year license).
    * **Preview**: Shows the decrypted data before finalizing.
    * **Generate**: Triggers the encryption pipeline and saves the `.asl` file.

### 5. License Records (`LicenseRecordsPage`)
* **Function**: Audit trail.
* **Logic**: Every generated license is saved to the `LicenseRecords` database table.
* **Features**:
    * Search by Company Name or License Key.
    * View historical expiration dates.
    * Check which admin generated a specific license.

---

## 🛠 Configuration & Setup

### Database Setup
1.  Open **SQL Server Management Studio (SSMS)**.
2.  Execute the script located in `Database/Schema.sql` to create tables.
3.  Execute `Database/Seed.sql` to insert the default Admin user.

### Application Configuration (`App.config`)
The application relies on specific keys in the `App.config` file for database connection and encryption security.

> ⚠️ **CRITICAL SECURITY WARNING**: The Encryption Key and IV match the logic in the client DLL. If these keys are changed here, previously generated licenses will become invalid.

```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=YOUR_SERVER_NAME;Database=AutosoftLicensing;Trusted_Connection=True;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>

  <appSettings>
    <add key="EncryptionKey" value="[YOUR_BASE64_KEY_HERE]" />

    <add key="EncryptionIV" value="[YOUR_BASE64_IV_HERE]" />
  </appSettings>
</configuration>
