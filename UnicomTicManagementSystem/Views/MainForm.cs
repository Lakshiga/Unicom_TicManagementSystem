using System;
using System.Data.SQLite;

using System.Windows.Forms;
using UnicomTicManagementSystem.Data;

namespace UnicomTicManagementSystem.Views
{
    public partial class MainForm : Form
    {
        private string userRole;
        private string currentLoggedInUsername;
        private Form currentLoadedForm;

        public MainForm()
        {
            InitializeComponent();
            LoadDashboardCounts();
        }

        public MainForm(string role, string username) : this()
        {
            userRole = role;
            currentLoggedInUsername = username;
            lblWelcome.Text = $"Welcome to {userRole} Dashboard";
            ApplyRoleAccess();
        }

        private void ApplyRoleAccess()
        {
            flowSidebar.Controls.Clear();

            flowSidebar.Controls.Add(lblWelcome);

            if (userRole == "Admin")
            {
                flowSidebar.Controls.Add(button1); // Student
                flowSidebar.Controls.Add(button2); // Lectures
                flowSidebar.Controls.Add(button3); // Section
                flowSidebar.Controls.Add(button4); // Subject
                flowSidebar.Controls.Add(button5); // Staff
                flowSidebar.Controls.Add(button6); // Timetable
                flowSidebar.Controls.Add(button8); // Exam
                flowSidebar.Controls.Add(button7); // Marks
                flowSidebar.Controls.Add(button9); // Room
                flowSidebar.Controls.Add(btnResetPassword); // Reset
                flowSidebar.Controls.Add(button10); // Attendance
            }
            else if (userRole.ToLower() == "staff")
            {
                flowSidebar.Controls.Add(button6); // Timetable
                flowSidebar.Controls.Add(button7); // Marks
                flowSidebar.Controls.Add(button8); // Exam
                flowSidebar.Controls.Add(btnResetPassword); // Reset
                flowSidebar.Controls.Add(button10); // Attendance
            }
            else if (userRole.ToLower() == "lecture")
            {
                flowSidebar.Controls.Add(button6); // Timetable
                flowSidebar.Controls.Add(button7); // Marks
                flowSidebar.Controls.Add(btnResetPassword); // Reset
                flowSidebar.Controls.Add(button10); // Attendance
            }

            flowSidebar.Controls.Add(btnlogout);
        }

        private void LoadFormInPanel(Form form)
        {
            try
            {
                if (currentLoadedForm != null && !currentLoadedForm.IsDisposed)
                {
                    currentLoadedForm.Close();
                    currentLoadedForm.Dispose();
                }

                panel2.Controls.Clear();
                form.TopLevel = false;
                form.Dock = DockStyle.Fill;
                form.FormBorderStyle = FormBorderStyle.None;
                panel2.Controls.Add(form);
                form.Show();
                currentLoadedForm = form;

                LoadDashboardCounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new StudentForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Student Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new TeacherForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Teacher Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new SectionForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Section Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new SubjectForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Subject Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new StaffForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Staff Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new TimeTableForm(userRole));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading TimeTable Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new MarkForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Mark Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new ExamForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Exam Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new RoomForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Room Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new AttendanceForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Attendance Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            try
            {
                LoadFormInPanel(new PasswordResetForm(currentLoggedInUsername));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Password Reset Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnlogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (currentLoadedForm != null && !currentLoadedForm.IsDisposed)
                {
                    currentLoadedForm.Close();
                    currentLoadedForm.Dispose();
                }

                this.Hide();
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
            }
        }

        private void LoadDashboardCounts()
        {
            try
            {
                using (var conn = DbCon.GetConnection())
                {
                    var cmd1 = new SQLiteCommand("SELECT COUNT(*) FROM Students", conn);
                    lblTotalStudents.Text = "TOTAL \nSTUDENTS:\n⚫" + cmd1.ExecuteScalar().ToString();

                    var cmd2 = new SQLiteCommand("SELECT COUNT(*) FROM Teachers", conn);
                    lblTotalLectures.Text = "TOTAL \nLECTURES:\n⚫" + cmd2.ExecuteScalar().ToString();

                    var cmd3 = new SQLiteCommand("SELECT COUNT(*) FROM Sections", conn);
                    lblTotalCourses.Text = "TOTAL \nCOURSES:\n⚫" + cmd3.ExecuteScalar().ToString();

                    var cmd4 = new SQLiteCommand("SELECT COUNT(*) FROM Subjects", conn);
                    lblTotalSubjects.Text = "TOTAL \nSUBJECTS:\n⚫" + cmd4.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard counts: {ex.Message}");
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (currentLoadedForm != null && !currentLoadedForm.IsDisposed)
            {
                currentLoadedForm.Close();
                currentLoadedForm.Dispose();
            }

            base.OnFormClosing(e);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTotalStudents_Click(object sender, EventArgs e)
        {

        }

        private void lblTotalLectures_Click(object sender, EventArgs e)
        {

        }
    }
}