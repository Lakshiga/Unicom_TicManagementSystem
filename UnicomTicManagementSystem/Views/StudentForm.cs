using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Repositories;
using UnicomTicManagementSystem.Services;

namespace UnicomTicManagementSystem.Views
{
    public partial class StudentForm : Form
    {
        private readonly StudentController _studentController;
        private readonly SectionController _sectionController;
        private Student _currentStudent;

        private Guid selectedStudentId = Guid.Empty;
        private Guid selectedUserId = Guid.Empty;

        public StudentForm()
        {
            InitializeComponent();
            _studentController = new StudentController();
            _sectionController = new SectionController();
            chkIsActive.Checked = true; // Default to active
            InitializeForm();
        }

        private async void InitializeForm()
        {
            await LoadSectionsAsync();
            await LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            SetLoading(true);
            try
            {
                dgvStudents.DataSource = await _studentController.GetAllStudentsAsync();

                // Hide columns you don't want visible
                if (dgvStudents.Columns.Contains("Id")) dgvStudents.Columns["Id"].Visible = false;
                if (dgvStudents.Columns.Contains("Username")) dgvStudents.Columns["Username"].Visible = false;
                if (dgvStudents.Columns.Contains("Password")) dgvStudents.Columns["Password"].Visible = false;
                if (dgvStudents.Columns.Contains("SectionName")) dgvStudents.Columns["SectionName"].Visible = false;
                if (dgvStudents.Columns.Contains("Stream")) dgvStudents.Columns["Stream"].Visible = false;
                if (dgvStudents.Columns.Contains("LastAttendanceDate")) dgvStudents.Columns["LastAttendanceDate"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load students: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
                ClearForm();
            }
        }

        private async Task LoadSectionsAsync()
        {
            try
            {
                cmbSection.DataSource = await _sectionController.GetAllSectionsAsync();
                cmbSection.DisplayMember = "Name";
                cmbSection.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            _currentStudent = null;
            name.Clear();
            address.Clear();
            username.Clear();
            password.Clear();
            cmbSection.SelectedIndex = -1;
            chkIsActive.Checked = true;
            dgvStudents.ClearSelection();
            selectedStudentId = Guid.Empty;
            selectedUserId = Guid.Empty;
        }

        private void ClearInputs()
        {
            name.Text = "";
            address.Text = "";
            username.Text = "";
            password.Text = "";
            chkIsActive.Checked = true;
            selectedStudentId = Guid.Empty;
            selectedUserId = Guid.Empty;
        }

        private async void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count > 0)
            {
                if (dgvStudents.SelectedRows[0].DataBoundItem is Student selectedStudent)
                {
                    selectedStudentId = selectedStudent.Id;
                    selectedUserId = selectedStudent.UserId;

                    _currentStudent = await _studentController.GetStudentByIdAsync(selectedStudentId);
                    if (_currentStudent != null)
                    {
                        name.Text = _currentStudent.Name;
                        address.Text = _currentStudent.Address;
                        cmbSection.SelectedValue = _currentStudent.SectionId;
                        chkIsActive.Checked = _currentStudent.IsActive;

                        var user = await UserRepository.GetUserByGuidAsync(_currentStudent.UserId);
                        username.Text = user?.Username ?? "";
                        password.Text = user?.Password ?? "";
                    }
                }
            }
            else
            {
                ClearInputs();
                _currentStudent = null;
            }
        }

        private void SetLoading(bool isLoading)
        {
            Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            add.Enabled = !isLoading;
            update.Enabled = !isLoading;
            delete.Enabled = !isLoading;
        }

        private async void add_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(address.Text)
                    || cmbSection.SelectedIndex == -1 || string.IsNullOrWhiteSpace(username.Text)
                    || string.IsNullOrWhiteSpace(password.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var student = new Student
                {
                    Name = name.Text,
                    Address = address.Text,
                    SectionId = (Guid)cmbSection.SelectedValue,
                    IsActive = chkIsActive.Checked
                };

                SetLoading(true);
                await _studentController.AddStudentAsync(student, username.Text.Trim(), password.Text.Trim());
                await LoadStudentsAsync();
                MessageBox.Show("Student added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void update_Click(object sender, EventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("Please select a student to update.", "No Student Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _currentStudent.Name = name.Text;
                _currentStudent.Address = address.Text;
                _currentStudent.SectionId = (Guid)cmbSection.SelectedValue;
                _currentStudent.IsActive = chkIsActive.Checked;

                SetLoading(true);
                await _studentController.UpdateStudentAsync(_currentStudent, username.Text.Trim(), password.Text.Trim());
                await LoadStudentsAsync();
                MessageBox.Show("Student updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async void delete_Click(object sender, EventArgs e)
        {
            if (_currentStudent == null)
            {
                MessageBox.Show("Please select a student to delete.", "No Student Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this student?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    SetLoading(true);
                    await _studentController.DeleteStudentAsync(_currentStudent.Id);
                    await LoadStudentsAsync();
                    MessageBox.Show("Student deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting student: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetLoading(false);
                }
            }
        }

        private async void StudentForm_Load(object sender, EventArgs e)
        {
            await LoadStudentsAsync();
            await LoadSectionsAsync();
        }

        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void btnTestTimeData_Click(object sender, EventArgs e)
        {
            TestTimeDataStorage();
        }

        private async void TestTimeDataStorage()
        {
            try
            {
                if (_currentStudent != null && _currentStudent.UserId != Guid.Empty)
                {
                    await UserRepository.UpdateLastLoginDateAsync(_currentStudent.UserId, DateTime.Now);
                    MessageBox.Show($"Updated last login date for student: {_currentStudent.Name}", "Time Data Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (_currentStudent != null)
                {
                    await _studentController.UpdateLastAttendanceDateAsync(_currentStudent.Id, DateTime.Now);
                    MessageBox.Show("Updated last attendance date for selected student", "Time Data Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                await LoadStudentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing time data storage: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }

        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void username_TextChanged(object sender, EventArgs e) { }

        private void password_TextChanged(object sender, EventArgs e) { }
    }
}
