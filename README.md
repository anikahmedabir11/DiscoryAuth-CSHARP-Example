# DiscoryAuth-CSHARP-Example : Please star ⭐

DiscoryAuth C# / .NET example SDK for [https://discoryauth.cc](https://discoryauth.cc) license key & API authentication.

---

### Included Projects
1. **`Console/`** - A complete console application demonstrating `api.Init()`, `api.Login()`, `api.Register()`, and `api.License()` key-only mode.
2. **`Form/`** - A modern Windows Forms GUI application styled with dark mode and DiscoryAuth electric blue accent.

---

### Getting Started

1. **Configure Application Credentials** in `Console/Program.cs` or `Form/LoginForm.cs`:
   ```csharp
   const string AppName    = "My C# Protected App";
   const string AppId      = "app_xxxxxxxx";             // From https://discoryauth.cc/app/
   const string AppSecret  = "sec_xxxxxxxxxxxxxxxxxxxx"; // From https://discoryauth.cc/app/
   const string AppVersion = "1.0";
   const string ApiUrl     = "https://discoryauth.cc/api/v1";
   ```

2. **Run Console App:**
   ```bash
   cd Console
   dotnet run
   ```

3. **Run Form App:**
   ```bash
   cd Form
   dotnet run
   ```

---

### SDK Methods
* `auth.Init()` - Handshakes with server and verifies region status.
* `auth.License(key)` - Fast key-only activation.
* `auth.Login(user, pass)` - User account authentication.
* `auth.Register(user, pass, key)` - Register user with license key.
* `auth.Var(name)` - Fetches encrypted remote string variable.
* `auth.Check()` - Heartbeat session verification.
* `auth.Log(message)` - Dashboard audit logging.

---

### License
MIT License.
