/*
PAGE: PreviewLicense.cs
ROLE: Dealer Admin (Dealer EXE)
PURPOSE:
  Small reusable UserControl / modal used by GenerateLicensePage to preview the plain JSON license payload
  before encryption and download. Displays canonical JSON, computed checksum, and list of enabled modules.

KEY UI ELEMENTS:
  - Read-only code-view textbox (syntax-highlight optional) showing canonical JSON payload without checksum or with checksum as a separate line
  - Label: ChecksumSHA256 (hex)
  - Buttons: Close, Export JSON (optional)
  - Optional: "Validate" button to re-run checksum calculation on displayed payload (for debugging)

BACKEND SERVICE CALLS (use your implemented services):
  - Use ServiceRegistry.Encryption.BuildJsonWithChecksum(licenseData) to obtain canonical JSON (checksum injected)
  - Use ServiceRegistry.Encryption.ComputeSha256Hex(bytes) to show checksum value (for confirmation)

VALIDATION & RULES:
  - When opened, show only payload that will be encrypted (the same canonical serialization used to compute the checksum).
  - Do not allow editing in normal mode (Preview only). If an admin must change something, return to GenerateLicensePage.

UX NOTES:
  - Modal overlay; Esc or Close returns to GenerateLicensePage.
  - If Export JSON is used, it writes the canonical JSON (not encrypted) only for internal debugging; gate this behind Admin role.

ACCEPTANCE CRITERIA:
  - Preview shows canonical JSON exactly as will be hashed and encrypted.
  - Checksum shown matches ServiceRegistry.Encryption computed value.

COPILOT PROMPTS:
  - "// Implement PreviewLicenseControl that accepts LicenseData (or LicensePayload) and shows canonical JSON using ServiceRegistry.Encryption.BuildJsonWithChecksum(licenseData) and the SHA-256 checksum via ServiceRegistry.Encryption.ComputeSha256Hex(Encoding.UTF8.GetBytes(json))."
  - "// Use a read-only TextEdit or MemoEdit for JSON content; format/indent for readability but ensure canonical bytes equal the serializer output used by BuildJsonWithChecksum."
*/
