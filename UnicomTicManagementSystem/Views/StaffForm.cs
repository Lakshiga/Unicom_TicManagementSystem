using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;

namespace UnicomTicManagementSystem.Views
{
    public partial class StaffForm : Form
    {
        private readonly StaffController _staffController;
        private Guid selectedStaffId = Guid.Empty;
        private Guid selectedUserId = Guid.Empty;


        public StaffForm()
        {
            InitializeComponent();
            _staffController = new StaffController();
            LoadStaffAsync();
        }

        private async Task LoadStaffAsync()
        {
            dgvStaff.DataSource = null;
            dgvStaff.DataSource = await _staffController.GetAllStaffAsync();

            dgvStaff.ClearSelection();
        }

        private void ClearForm()
        {
            txtName.Text = "";
            txtAddress.Text = "";
            txtEmail.Text = "";
            txtUsername.Text = "";
            textBox5.Text = "";
            selectedStaffId = Guid.Empty;
            selectedUserId = Guid.Empty;
        }


        private async void dgvStaff_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count > 0)
            {
                var row = dgvStaff.SelectedRows[0].DataBoundItem as Staff;

                if (row != null)
                {
                    selectedStaffId = row.Id;
                    selectedUserId = row.UserId;

                    txtName.Text = row.Name;
                    txtAddress.Text = row.Address;
                    txtEmail.Text = row.Email;

                    // Use GetUserByGuidAsync here:
                    var user = await UserRepository.GetUserByGuidAsync(selectedUserId);
                    txtUsername.Text = user?.Username ?? "";
                    textBox5.Text = user?.Password ?? "";
                }
            }
        }


        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (selectedStaffId == Guid.Empty)
            {
                MessageBox.Show("Select a staff to delete.");
                return;
            }

            Guid staffId = new Guid(selectedStaffId.ToString()); // Ensure selectedStaffId is a valid Guid

            await _staffController.DeleteStaffAsync(staffId);
            await LoadStaffAsync();
            ClearForm();
            MessageBox.Show("Staff deleted successfully.");
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == Guid.Empty)
            {
                MessageBox.Show("Select a staff to update.");
                return;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                if (addr.Address != txtEmail.Text)
                {
                    MessageBox.Show("Please enter a valid email address.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            var staff = new Staff
            {
                Id = selectedStaffId,
                Name = txtName.Text,
                Address = txtAddress.Text,
                Email = txtEmail.Text,
                UserId = selectedUserId
            };

            await _staffController.UpdateStaffAsync(staff, txtUsername.Text.Trim(), textBox5.Text.Trim());
            await LoadStaffAsync();
            ClearForm();
            MessageBox.Show("Staff updated successfully.");
        }


        private async void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtAddress.Text == "" || txtEmail.Text == "" ||
                txtUsername.Text == "" || textBox5.Text == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                if (addr.Address != txtEmail.Text)
                {
                    MessageBox.Show("Please enter a valid email address.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Please enter a valid email address.");
                return;
            }

            if (await UserRepository.UserExistsAsync(txtUsername.Text))
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            var staff = Staff.CreateStaff(
                txtName.Text,
                txtAddress.Text,
                txtEmail.Text,
                Guid.NewGuid()
            );

            await _staffController.AddStaffAsync(staff, txtUsername.Text.Trim(), textBox5.Text.Trim());
            await LoadStaffAsync();
            ClearForm();
            MessageBox.Show("Staff added successfully.");
        }

        private void dgvStaff_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
