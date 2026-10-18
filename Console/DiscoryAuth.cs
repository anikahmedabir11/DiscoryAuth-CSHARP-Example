using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DiscoryAuthSDK
{
    public class UserData
    {
        public string username { get; set; } = "";
        public string expires_at { get; set; } = "";
        public string hwid { get; set; } = "";
        public string license_key { get; set; } = "";
        public string session_token { get; set; } = "";
        public bool is_lifetime { get; set; } = false;
    }

    public class AppData
    {
        public string name { get; set; } = "";
        public string version { get; set; } = "";
        public bool hwid_lock { get; set; } = true;
    }

    public class DiscoryAuth
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string Secret { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string Hwid { get; private set; }

        public bool Initialized { get; private set; }
        public string SessionToken { get; private set; }

        public UserData UserData { get; private set; } = new UserData();
        public AppData AppData { get; private set; } = new AppData();

        private static readonly HttpClient client = new HttpClient();

        public DiscoryAuth(string name, string appId, string secret, string version = "1.0", string url = "https://discoryauth.cc/api/v1")
        {
            Name = name;
            AppId = appId;
            Secret = secret;
            Version = version;
            Url = url.TrimEnd('/');
            Hwid = GetHWID();
        }

        private string GetHWID()
        {
            try
            {
                var psi = new ProcessStartInfo("wmic", "csproduct get uuid")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p != null)
                {
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();
                    var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length > 1 && !string.IsNullOrWhiteSpace(lines[1]))
                    {
                        return lines[1].Trim();
                    }
                }
            }
            catch { }

            // Fallback unique machine identifier
            return Environment.MachineName + "_" + Environment.UserName;
        }

        public bool Init()
        {
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    hwid = Hwid,
                    version = Version
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/init", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var succ) && succ.GetBoolean())
                {
                    Initialized = true;
                    if (root.TryGetProperty("app", out var appElem))
                    {
                        AppData.name = appElem.TryGetProperty("name", out var n) ? n.GetString() ?? Name : Name;
                        AppData.version = appElem.TryGetProperty("version", out var v) ? v.GetString() ?? Version : Version;
                    }
                    return true;
                }
                else
                {
                    string msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Initialization failed.";
                    Console.WriteLine($"[-] Init Error: {msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Network Error: {ex.Message}");
                return false;
            }
        }

        public bool License(string licenseKey)
        {
            if (!Initialized) throw new InvalidOperationException("Call api.Init() first!");
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    license_key = licenseKey.Trim(),
                    hwid = Hwid
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/license", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var succ) && succ.GetBoolean())
                {
                    if (root.TryGetProperty("session_token", out var tok))
                        SessionToken = tok.GetString();

                    if (root.TryGetProperty("user", out var u))
                    {
                        UserData.username = u.TryGetProperty("username", out var un) ? un.GetString() : "DiscoryUser";
                        UserData.expires_at = u.TryGetProperty("expires_at", out var exp) ? exp.GetString() : "Never";
                        UserData.hwid = u.TryGetProperty("hwid", out var hw) ? hw.GetString() : Hwid;
                        UserData.license_key = licenseKey;
                        UserData.session_token = SessionToken;
                    }
                    return true;
                }
                else
                {
                    string msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Invalid license key.";
                    Console.WriteLine($"[-] License Error: {msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Network Error: {ex.Message}");
                return false;
            }
        }

        public bool Login(string username, string password)
        {
            if (!Initialized) throw new InvalidOperationException("Call api.Init() first!");
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    username = username.Trim(),
                    password = password.Trim(),
                    hwid = Hwid
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/login", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var succ) && succ.GetBoolean())
                {
                    if (root.TryGetProperty("session_token", out var tok))
                        SessionToken = tok.GetString();

                    if (root.TryGetProperty("user", out var u))
                    {
                        UserData.username = username;
                        UserData.expires_at = u.TryGetProperty("expires_at", out var exp) ? exp.GetString() : "Never";
                        UserData.hwid = u.TryGetProperty("hwid", out var hw) ? hw.GetString() : Hwid;
                        UserData.session_token = SessionToken;
                    }
                    return true;
                }
                else
                {
                    string msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Login failed.";
                    Console.WriteLine($"[-] Login Error: {msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Network Error: {ex.Message}");
                return false;
            }
        }

        public bool Register(string username, string password, string licenseKey)
        {
            if (!Initialized) throw new InvalidOperationException("Call api.Init() first!");
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    username = username.Trim(),
                    password = password.Trim(),
                    license_key = licenseKey.Trim(),
                    hwid = Hwid
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/register", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var succ) && succ.GetBoolean())
                {
                    return true;
                }
                else
                {
                    string msg = root.TryGetProperty("message", out var m) ? m.GetString() : "Registration failed.";
                    Console.WriteLine($"[-] Registration Error: {msg}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Network Error: {ex.Message}");
                return false;
            }
        }

        public string Var(string varName)
        {
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    var_id = varName,
                    session_token = SessionToken
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/var", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                var root = doc.RootElement;

                if (root.TryGetProperty("success", out var succ) && succ.GetBoolean())
                {
                    return root.TryGetProperty("response", out var r) ? r.GetString() ?? "" : "";
                }
            }
            catch { }
            return "";
        }

        public bool Check()
        {
            if (string.IsNullOrEmpty(SessionToken)) return false;
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    session_token = SessionToken
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = client.PostAsync($"{Url}/check", content).GetAwaiter().GetResult();
                var jsonStr = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                using var doc = JsonDocument.Parse(jsonStr);
                return doc.RootElement.TryGetProperty("success", out var succ) && succ.GetBoolean();
            }
            catch
            {
                return false;
            }
        }

        public void Log(string message)
        {
            try
            {
                var payload = new
                {
                    app_id = AppId,
                    app_secret = Secret,
                    message = message,
                    pcuser = Environment.UserName
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                client.PostAsync($"{Url}/log", content);
            }
            catch { }
        }
    }
}
