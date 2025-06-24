using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Controllers.Repositories;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Views
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
        }

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!await UserRepository.HasAnyUsersAsync())
                {
                    lblMessage.Text = "CREATE ADMIN ACCOUNT :";
                    btnLogin.Text = "Register Admin";
                }
                else
                {
                    lblMessage.Text = "USERNAME :";
                    btnLogin.Text = "Login";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing login form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!await UserRepository.HasAnyUsersAsync())
                {
                    // First-time admin registration
                    var newAdmin = new User
                    {
                        Username = username.Trim(),

                        Password = password.Trim(),
                        Role = "Admin"
                    };

                    await UserRepository.AddUserAsync(newAdmin);
                    MessageBox.Show("Admin account created successfully. You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RefreshFormAsync();
                    return;
                }

                // Regular login process
                var user = await UserRepository.AuthenticateAsync(username, password);

                if (user != null)
                {
                    this.Hide();

                    switch (user.Role.ToLower())
                    {
                        case "admin":
                        case "staff":
                        case "lecture":
                            var mainForm = new MainForm(user.Role, username);
                            this.Hide();
                            mainForm.ShowDialog();
                            this.Show();
                            break;

                        case "student":
                            var studentRepository = new StudentRepository();
                            var student = await studentRepository.GetStudentByUsernameAsync(user.Username);
                            if (student != null)
                            {
                                var userLogin = UserLogin.CreateUserLogin(
                                    student.Username,
                                    student.Password,
                                    user.Role,
                                    student.Name,
                                    student.Address,
                                    student.Stream
                                );

                                StudentDashboard dashboard = new StudentDashboard(username);
                                this.Hide();
                                dashboard.ShowDialog();
                                this.Show();
                            }
                            else
                            {
                                MessageBox.Show("Student profile not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Show();
                            }
                            break;

                        default:
                            MessageBox.Show("Unknown role. Access denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Show();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshFormAsync()
        {
            try
            {
                if (!await UserRepository.HasAnyUsersAsync())
                {
                    lblMessage.Text = "CREATE ADMIN ACCOUNT :";
                    btnLogin.Text = "Register Admin";
                }
                else
                {
                    lblMessage.Text = "USERNAME :";
                    btnLogin.Text = "Login";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
