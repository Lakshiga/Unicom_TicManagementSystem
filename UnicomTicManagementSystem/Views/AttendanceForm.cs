using System;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Services;

namespace UnicomTicManagementSystem.Views
{

    public partial class AttendanceForm : Form
    {
        private AttendanceService _attendanceService = new AttendanceService();
        private string selectedAttendanceId = null;  
        private string userRole;

        public AttendanceForm(string role = "lecture")
        {
            InitializeComponent();
            userRole = role;
        }

        private async void AttendanceForm_Load(object sender, EventArgs e)
        {
            await LoadSubjectsAsync();
            LoadStatusOptions();
            await LoadAttendanceGridAsync();
            ApplyRolePermissions();
            datePicker.Visible = true;
            datePicker.Value = DateTime.Today;
        }

        private async Task LoadSubjectsAsync()
        {
            var dt = await _attendanceService.GetAllSubjectsAsync();

            if (dt != null && dt.Columns.Contains("SubjectName") && dt.Columns.Contains("SubjectID"))
            {
                comboBoxSubject.DataSource = dt;
                comboBoxSubject.DisplayMember = "SubjectName";
                comboBoxSubject.ValueMember = "SubjectID";
                comboBoxSubject.SelectedIndex = -1;
            }
        }


        private void LoadStatusOptions()
        {
            comboBoxStatus.Items.Clear();
            comboBoxStatus.Items.AddRange(new[] { "Present", "Absent", "Late", "Excused" });
            comboBoxStatus.SelectedIndex = 0;
        }

        private async Task LoadAttendanceGridAsync()
        {

            var dt = await _attendanceService.GetAllAttendanceAsync();
            dataGridViewAttendance.DataSource = dt;

            if (dataGridViewAttendance.Columns["Id"] != null)
                dataGridViewAttendance.Columns["Id"].Visible = false;

            if (dataGridViewAttendance.Columns["SubjectID"] != null)
                dataGridViewAttendance.Columns["SubjectID"].Visible = false;
        }

        private void ApplyRolePermissions()
        {
            if (userRole.ToLower() == "admin" || userRole.ToLower() == "staff")
            {
                // Hide all controls except the DataGridView
                comboBoxSubject.Visible = false;
                comboBoxStatus.Visible = false;
                textBoxStudentID.Visible = false;
                textBoxStudentName.Visible = false;
                textBoxDate.Visible = false;
                textBox3.Visible = false;
                datePicker.Visible = false;
                btnMarkAttendance.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;
                btnPickDate.Visible = false;
                btnSearch.Visible = false;

                labelStudentID.Visible = false;
                labelStudentName.Visible = false;
                label3.Visible = false;
                labelStatus.Visible = false;
                labelDate.Visible = false;
                label6.Visible = false;
                dataGridViewAttendance.Dock = DockStyle.Fill;
            }
        }

        private async void textBoxStudentID_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxStudentID.Text))
            {
                textBoxStudentName.Text = "";
                return;
            }

            // Get the student using reference number
            var student = await _attendanceService.GetStudentByReferenceIdAsync(textBoxStudentID.Text.Trim());
            if (student != null)
            {
                textBoxStudentName.Text = student.Name;  

                // ✅ Now use actual student GUID ID to fetch subjects
                var subjects = await _attendanceService.GetSubjectsByStudentIDAsync(student.Id.ToString());

                if (subjects != null && subjects.Rows.Count > 0)
                {
                    comboBoxSubject.DataSource = subjects;
                    comboBoxSubject.DisplayMember = "SubjectName";
                    comboBoxSubject.ValueMember = "SubjectID";
                    comboBoxSubject.SelectedIndex = -1;
                }
                else
                {
                    comboBoxSubject.DataSource = null;
                }
            }
            else
            {
                textBoxStudentName.Text = "Not found";
                comboBoxSubject.DataSource = null;
            }
        }


        private async void btnMarkAttendance_Click(object sender, EventArgs e)
        {
            if (comboBoxSubject.SelectedItem == null || comboBoxStatus.SelectedItem == null || string.IsNullOrWhiteSpace(textBoxStudentID.Text))
            {
                MessageBox.Show("Please enter Student ID, select Subject and Status.");
                return;
            }

            // Lookup student by ReferenceId (from textBoxStudentID)
            var student = await _attendanceService.GetStudentByReferenceIdAsync(textBoxStudentID.Text.Trim());

            if (student == null)
            {
                MessageBox.Show("Student not found by this Reference ID.");
                return;
            }

            DateTime selectedDate = datePicker.Value;

            Attendance attendance = Attendance.CreateAttendance(
                student.Id,    // Use actual student GUID here
                Guid.Parse(comboBoxSubject.SelectedValue?.ToString()),
                selectedDate,
                comboBoxStatus.SelectedItem.ToString()
            );

            await _attendanceService.AddAttendanceAsync(attendance);
            MessageBox.Show("Attendance added successfully.");
            await LoadAttendanceGridAsync();
            ClearForm();
        }


        private async void dataGridViewAttendance_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewAttendance.Rows[e.RowIndex].Cells["Id"].Value != null)
            {
                DataGridViewRow row = dataGridViewAttendance.Rows[e.RowIndex];

                selectedAttendanceId = row.Cells["Id"].Value.ToString();

                string studentGuidStr = row.Cells["StudentID"].Value.ToString(); 

                var student = await _attendanceService.GetStudentByGuidAsync(studentGuidStr);
                if (student != null)
                {
                    textBoxStudentID.Text = student.ReferenceId.ToString();
                    textBoxStudentName.Text = student.Name;

                    var subjects = await _attendanceService.GetSubjectsByStudentIDAsync(student.Id.ToString());
                    if (subjects != null && subjects.Rows.Count > 0)
                    {
                        comboBoxSubject.DataSource = subjects;
                        comboBoxSubject.DisplayMember = "SubjectName";
                        comboBoxSubject.ValueMember = "SubjectID";
                        comboBoxSubject.SelectedValue = Guid.Parse(row.Cells["SubjectID"].Value.ToString());
                    }
                    else
                    {
                        comboBoxSubject.DataSource = null;
                    }
                }
                else
                {
                    MessageBox.Show("Student not found.");
                    return;
                }

                // Status and date
                comboBoxStatus.SelectedItem = row.Cells["Status"].Value.ToString();
                datePicker.Value = Convert.ToDateTime(row.Cells["Date"].Value);
                textBoxDate.Text = datePicker.Value.ToString("yyyy-MM-dd");
            }
        }




        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            textBoxDate.Text = datePicker.Value.ToString("yyyy-MM-dd");
            datePicker.Visible = false;
        }

        private void ClearForm()
        {
            selectedAttendanceId = null;  // ✅ clear selected ID
            textBoxStudentID.Clear();
            textBoxStudentName.Clear();
            comboBoxSubject.SelectedIndex = -1;
            comboBoxStatus.SelectedIndex = 0;
            textBoxDate.Clear();
            datePicker.Value = DateTime.Today;
        }

        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAttendanceId))
            {
                MessageBox.Show("Please select a record to delete.");
                return;
            }

            try
            {
                await _attendanceService.DeleteAttendanceAsync(selectedAttendanceId);
                MessageBox.Show("Attendance deleted successfully.");
                await LoadAttendanceGridAsync();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting attendance: {ex.Message}");
            }
        }



        private async void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedAttendanceId))
            {
                MessageBox.Show("Please select a record to update.");
                return;
            }

            var student = await _attendanceService.GetStudentByReferenceIdAsync(textBoxStudentID.Text.Trim());
            if (student == null)
            {
                MessageBox.Show("Student not found by this Reference ID.");
                return;
            }

            DateTime selectedDate = datePicker.Value;

            Attendance attendance = Attendance.CreateAttendance(
                student.Id,  // Use actual student GUID here
                Guid.Parse(comboBoxSubject.SelectedValue?.ToString()),
                selectedDate,
                comboBoxStatus.SelectedItem.ToString()
            );

            await _attendanceService.UpdateAttendanceAsync(selectedAttendanceId, attendance);
            MessageBox.Show("Attendance updated successfully.");
            await LoadAttendanceGridAsync();
            ClearForm();
        }


        private async void btnSearch_Click_1(object sender, EventArgs e)
        {
            DateTime searchDate = datePicker.Value;
            string studentReferenceId = textBoxStudentID.Text.Trim(); 
            string studentName = textBoxStudentName.Text.Trim();
            string subjectName = comboBoxSubject.Text.Trim();
            string status = comboBoxStatus.Text.Trim();

            try
            {
                var dt = await _attendanceService.SearchAttendanceAsync(searchDate, studentReferenceId, studentName, subjectName, status);
                dataGridViewAttendance.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching attendance: {ex.Message}");
            }
        }


        private void btnPickDate_Click_1(object sender, EventArgs e)
        {
            datePicker.Visible = true;
            datePicker.Focus();
        }

        private void textBoxDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxSubject_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridViewAttendance_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
