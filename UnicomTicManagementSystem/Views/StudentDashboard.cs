using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Models;
using UnicomTicManagementSystem.Repositories;
using UnicomTicManagementSystem.Controllers.Repositories;

namespace UnicomTicManagementSystem.Views
{
    public partial class StudentDashboard : Form
    {
        private string username;
        private Student student;
        private StudentRepository _studentRepository;
        private string sectionName;

        public StudentDashboard(string username)
        {
            InitializeComponent();
            this.username = username;
            _studentRepository = new StudentRepository();
            LoadStudentDataAsync();
        }

        private async void LoadStudentDataAsync()
        {
            // ✅ Correctly assign to class-level student variable
            student = await _studentRepository.GetStudentByUsernameAsync(username);

            if (student != null)
            {
                lblName.Text = student.Name;
                lblUsername.Text = username;
                lblPassword.Text = student.Password;
                lblAddress.Text = student.Address;
            }
            else
            {
                MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnTimetable_Click(object sender, EventArgs e)
        {
            if (student != null)
            {
                dataGridView1.DataSource = await _studentRepository.GetTimetableBySectionAsync(sectionName);
            }
        }

        private async void btnExamMarks_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(username))
            {
                dataGridView1.DataSource = await _studentRepository.GetExamMarksByUsernameAsync(username);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }

        private async void btnViewAttendance_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(username))
            {
                dataGridView1.DataSource = await _studentRepository.GetAttendanceByUsernameAsync(username);
            }
        }
    }
}
