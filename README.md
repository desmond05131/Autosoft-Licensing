# Autosoft Licensing System
### Client-Server Model with Dynamic Configuration

**Autosoft Licensing** is a robust Windows Forms (WinForms) application designed to manage the full lifecycle of software licensing. It handles license generation, validation, and management using a secure client-server architecture. 

A key feature of this branch is its **Dynamic Configuration Model**, which allows the application to connect to any SQL Server instance without recompiling, handling initial setup via a user-friendly GUI.

---

## 🚀 Key Features

* **Dynamic Database Connection**: No hardcoded connection strings. The app detects missing configurations and prompts the user to enter Server/Database credentials on the first run.
* **Secure License Generation**: Generates encrypted `.asl` (Autosoft License) files using **AES-256** encryption.
* **Integrity Verification**: Implements SHA-256 checksums to prevent license tampering.
* **Request Processing**: Imports and processes `.arl` (Autosoft Request License) files from clients.
* **User & Product Management**: Admin tools to manage internal users, dealers, and software products.

---

## 🛠️ Technology Stack

* **Framework**: .NET Framework 4.8
* **Language**: C#
* **UI**: Windows Forms (WinForms)
* **Database**: Microsoft SQL Server
* **Security**: `System.Security.Cryptography` (AES-256, SHA-256)

---

## 📂 Project Structure

The solution is organized into logical layers to separate concerns:

| Directory | Description |
| :--- | :--- |
| **`Data/`** | Handles raw database connectivity logic. Contains `SqlConnectionFactory` which builds connection strings dynamically based on user settings. |
| **`Database/`** | SQL scripts for setting up the environment (`Schema.sql` for tables, `Seed.sql` for initial data). |
| **`Services/`** | Business logic layer. Uses `ServiceRegistry` for Dependency Injection to manage services like Encryption, Validation, and Database access. |
| **`UI/`** | Contains all Forms and User Controls. `MainForm.cs` serves as the shell, while `Pages/` contains specific views like `GenerateLicensePage`. |
| **`Utils/`** | Helper classes and constants. `CryptoConstants.cs` manages access to encryption keys stored in `App.config`. |

---

## ⚙️ Getting Started

### 1. Prerequisites
* Visual Studio 2019 or 2022.
* SQL Server Express or LocalDB.
* .NET Framework 4.8 Developer Pack.

### 2. Database Setup
1.  Open SQL Server Management Studio (SSMS).
2.  Create a new database named `AutosoftLicensing`.
3.  Execute the script **`Autosoft Licensing/Database/Schema.sql`** to create the tables.
4.  Execute the script **`Autosoft Licensing/Database/Seed.sql`** to insert the default Admin user.

### 3. Application Configuration
The encryption keys are stored in `App.config`. For development, default keys are provided.
* **AesKey**: 32-byte Base64 string.
* **AesIV**: 16-byte Base64 string.

> **Security Note**: In a production environment, ensure you generate new random keys and update the `App.config` file.

### 4. Running the Application
1.  Build the solution in Visual Studio.
2.  Run the application.
3.  **First Launch**: The app will detect that no database connection is configured.
4.  The **Connection Settings** form will appear automatically.
5.  Enter your SQL Server details:
    * **Server**: e.g., `.\SQLEXPRESS` or `(localdb)\mssqllocaldb`
    * **Database**: `AutosoftLicensing`
    * **User/Password**: Leave blank for Integrated Security (Windows Auth), or provide SQL credentials.
6.  Click **Save & Test**. If successful, the Login screen will appear.

---

## 🔐 Security Details

* **License File (.asl)**: Contains JSON data encrypted with AES-256 (CBC mode, PKCS7 padding).
* **Tamper Protection**: A SHA-256 hash of the "canonicalized" JSON data is embedded in the license. If a user modifies the decrypted JSON and re-encrypts it without the correct secret key, the checksum validation will fail.

---

## 🧪 Testing

The solution includes a test project `Autosoft_Licensing.Tests` containing:
* **Integration Tests**: Verify database interactions and full license generation flows.
* **Unit Tests**: Validate logic in isolation (e.g., Checksum calculation).
* **Test Assets**: Sample `.arl` and `.asl` files are located in the `TestAssets` folder for manual testing.
