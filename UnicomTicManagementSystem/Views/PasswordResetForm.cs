using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Views
{
    public partial class PasswordResetForm : Form
    {
        private string username;

        public PasswordResetForm(string loggedInUsername)
        {
            InitializeComponent();
            username = loggedInUsername;
        }

        private async void btnReset_Click_1(object sender, EventArgs e)
        {
            string current = txtCurrent.Text;
            string newPass = txtNew.Text;
            string confirm = txtConfirm.Text;

            if (string.IsNullOrWhiteSpace(current) || string.IsNullOrWhiteSpace(newPass) || string.IsNullOrWhiteSpace(confirm))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            // Check current password
            if (!await UserRepository.ValidateUserAsync(username, current))
            {
                MessageBox.Show("Current password is incorrect.");
                return;
            }

            if (newPass != confirm)
            {
                MessageBox.Show("New password and confirm password do not match.");
                return;
            }

            // Update password
            if (await UserRepository.ResetUserPasswordAsync(username, newPass))
            {
                MessageBox.Show("Password updated successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to update password.");
            }
        }
    }
}
