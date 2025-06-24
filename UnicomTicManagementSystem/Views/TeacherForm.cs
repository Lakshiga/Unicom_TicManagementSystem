using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace UnicomTicManagementSystem.Views
{
    public partial class TeacherForm : Form
    {
        private TeacherController _controller = new TeacherController();
        private SectionController _sectionController = new SectionController();

        private Guid selectedTeacherId = Guid.Empty;
        private Guid selectedUserId = Guid.Empty;

        public TeacherForm()
        {
            InitializeComponent();
            LoadTeachersAsync();
            LoadSectionsAsync();
        }

        private async Task LoadTeachersAsync()
        {
            dgvTeachers.DataSource = await _controller.GetAllTeachersAsync();
            dgvTeachers.ClearSelection();
        }

        private async void LoadSectionsAsync()
        {
            comboSection.DataSource = await _controller.GetAllSectionsAsync();
            comboSection.DisplayMember = "Name";
            comboSection.ValueMember = "Id";
        }

        private void ClearInputs()
        {
            txtName.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            comboSection.SelectedIndex = -1;
            selectedTeacherId = Guid.Empty;
            selectedUserId = Guid.Empty;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtPhone.Text == "" || txtAddress.Text == ""
                || txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (txtPassword.Text.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (await UserRepository.UserExistsAsync(txtUsername.Text))
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            // Create new Teacher with generated UserId
            Teacher teacher = Teacher.CreateTeacher(
                txtName.Text,
                txtPhone.Text,
                txtAddress.Text,
                Guid.NewGuid() // UserId to be used after user creation
            );

            await _controller.AddUserAndTeacherAsync(txtUsername.Text.Trim(), txtPassword.Text.Trim(), "Lecture", teacher);

            if (comboSection.SelectedValue != null)
            {
                Guid sectionId = (Guid)comboSection.SelectedValue;
                await _controller.AssignSectionToTeacherAsync(teacher.Id, sectionId);
            }

            await LoadTeachersAsync();
            ClearInputs();
            MessageBox.Show("Teacher added successfully.");
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedTeacherId == Guid.Empty)
                {
                    MessageBox.Show("Select a teacher to update.");
                    return;
                }

                if (txtPassword.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters long.", "Invalid Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Teacher teacher = new Teacher
                {
                    Id = selectedTeacherId,
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    UserId = selectedUserId
                };

                await _controller.UpdateTeacherAsync(teacher);

                if (selectedUserId != Guid.Empty &&
                    !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                    !string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    var updatedUser = new UnicomTicManagementSystem.Models.User
                    {
                        Id = selectedUserId,
                        Username = txtUsername.Text.Trim(),
                        Password = txtPassword.Text.Trim()
                    };

                    await UserRepository.UpdateUserAsync(updatedUser);
                }

                if (comboSection.SelectedValue != null &&
                    Guid.TryParse(comboSection.SelectedValue.ToString(), out Guid sectionId))
                {
                    await _controller.AssignSectionToTeacherAsync(selectedTeacherId, sectionId);
                }

                await LoadTeachersAsync();
                ClearInputs();
                MessageBox.Show("Teacher updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating teacher: {ex.Message}");
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTeacherId == Guid.Empty)
            {
                MessageBox.Show("Select a teacher to delete.");
                return;
            }

            await _controller.DeleteTeacherAsync(selectedTeacherId);
            await LoadTeachersAsync();
            ClearInputs();
            MessageBox.Show("Teacher deleted successfully.");
        }

        private async void dgvTeachers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count > 0)
            {
                var row = dgvTeachers.SelectedRows[0];
                if (row.DataBoundItem is Teacher teacher)
                {
                    selectedTeacherId = teacher.Id;
                    selectedUserId = teacher.UserId;

                    txtName.Text = teacher.Name;
                    txtPhone.Text = teacher.Phone;
                    txtAddress.Text = teacher.Address;

                    var user = await UserRepository.GetUserByGuidAsync(selectedUserId);
                    txtUsername.Text = user?.Username ?? "";
                    txtPassword.Text = user?.Password ?? "";
                }
            }
        }

        private async void btnSearch_Click_1(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            dgvTeachers.DataSource = await _controller.SearchTeachersAsync(keyword);
        }
    }
}
