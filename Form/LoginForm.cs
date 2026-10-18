using System;
using System.Drawing;
using System.Windows.Forms;
using DiscoryAuthSDK;

namespace DiscoryAuthGUI
{
    public class LoginForm : Form
    {
        private DiscoryAuth auth;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtKey;
        private Label lblStatus;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnLicenseOnly;

        public LoginForm()
        {
            InitializeComponent();
            auth = new DiscoryAuth(
                "My C# GUI App",
                "app_xxxxxxxx",
                "sec_xxxxxxxxxxxxxxxxxxxx",
                "1.0",
                "https://discoryauth.cc/api/v1"
            );

            this.Load += (s, e) =>
            {
                lblStatus.Text = "Connecting to DiscoryAuth...";
                if (auth.Init())
                {
                    lblStatus.Text = $"Ready - {auth.AppData.name} (v{auth.AppData.version})";
                    lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
                }
                else
                {
                    lblStatus.Text = "Failed to connect to authentication server!";
                    lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
                }
            };
        }

        private void InitializeComponent()
        {
            this.Text = "DiscoryAuth - Login & License";
            this.Size = new Size(380, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 20);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            var title = new Label
            {
                Text = "DiscoryAuth Authentication",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(29, 99, 255)
            };
            this.Controls.Add(title);

            var lblU = new Label { Text = "Username", Location = new Point(24, 70), AutoSize = true, ForeColor = Color.Gray };
            txtUsername = new TextBox { Location = new Point(24, 92), Width = 315, BackColor = Color.FromArgb(28, 28, 32), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            var lblP = new Label { Text = "Password", Location = new Point(24, 130), AutoSize = true, ForeColor = Color.Gray };
            txtPassword = new TextBox { Location = new Point(24, 152), Width = 315, PasswordChar = '•', BackColor = Color.FromArgb(28, 28, 32), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            var lblK = new Label { Text = "License Key (For Register or Key-Only)", Location = new Point(24, 190), AutoSize = true, ForeColor = Color.Gray };
            txtKey = new TextBox { Location = new Point(24, 212), Width = 315, BackColor = Color.FromArgb(28, 28, 32), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            btnLogin = new Button
            {
                Text = "Log In",
                Location = new Point(24, 260),
                Width = 150,
                Height = 36,
                BackColor = Color.FromArgb(29, 99, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += OnLoginClick;

            btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(189, 260),
                Width = 150,
                Height = 36,
                BackColor = Color.FromArgb(38, 38, 44),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += OnRegisterClick;

            btnLicenseOnly = new Button
            {
                Text = "Activate with License Key Only",
                Location = new Point(24, 308),
                Width = 315,
                Height = 36,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLicenseOnly.FlatAppearance.BorderSize = 0;
            btnLicenseOnly.Click += OnLicenseOnlyClick;

            lblStatus = new Label
            {
                Text = "Status: Idle",
                Location = new Point(24, 355),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            this.Controls.Add(lblU);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblP);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblK);
            this.Controls.Add(txtKey);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(btnLicenseOnly);
            this.Controls.Add(lblStatus);
        }

        private void OnLoginClick(object sender, EventArgs e)
        {
            lblStatus.Text = "Verifying...";
            if (auth.Login(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show($"Welcome, {auth.UserData.username}!\nExpiry: {auth.UserData.expires_at}", "Authentication Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Login failed! Check credentials.";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        private void OnRegisterClick(object sender, EventArgs e)
        {
            lblStatus.Text = "Registering...";
            if (auth.Register(txtUsername.Text, txtPassword.Text, txtKey.Text))
            {
                MessageBox.Show("Registration successful! You may now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Registered successfully!";
                lblStatus.ForeColor = Color.FromArgb(16, 185, 129);
            }
            else
            {
                lblStatus.Text = "Registration failed!";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        private void OnLicenseOnlyClick(object sender, EventArgs e)
        {
            lblStatus.Text = "Validating Key...";
            if (auth.License(txtKey.Text))
            {
                MessageBox.Show($"License Accepted!\nExpiry: {auth.UserData.expires_at}", "Authentication Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Invalid License Key!";
                lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoginForm());
        }
    }
}
