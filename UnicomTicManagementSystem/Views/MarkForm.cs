using System;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers;

namespace UnicomTicManagementSystem.Views
{
    public partial class MarkForm : Form
    {
        private MarkController controller = new MarkController();
        private Guid selectedMarkId = Guid.Empty;
        private Guid studentGuidId = Guid.Empty;
        private string userRole;

        public MarkForm(string role = "Lecture")
        {
            InitializeComponent();
            userRole = role.ToLower(); // normalize role for comparison
        }

        private async void MarkForm_Load(object sender, EventArgs e)
        {
            ApplyRolePermissions();
            await LoadExamsAsync();
            await LoadMarksAsync();
        }

        private async Task LoadExamsAsync()
        {
            comboExam.Items.Clear();
            var dt = await controller.GetExamsAsync();

            MessageBox.Show($"Exams found: {dt.Rows.Count}");

            foreach (DataRow row in dt.Rows)
            {
                comboExam.Items.Add(row["ExamName"].ToString());
            }
        }


        private void ApplyRolePermissions()
        {
            if (userRole == "admin")
            {
                txtScore.Visible = false;
                comboExam.Visible = false;
                comboSubject.Visible = false;
                txtStudentID.Visible = false;
                txtStudentName.Visible = false;
                btnAdd.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;

                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label6.Visible = false;

                dataGridView1.Dock = DockStyle.Fill;
                dataGridView1.ReadOnly = true;
                dataGridView1.ClearSelection();
                dataGridView1.DefaultCellStyle.SelectionBackColor = dataGridView1.DefaultCellStyle.BackColor;
                dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor;
            }
        }

        // New overload for loading subjects by Guid
        private async Task LoadSubjectsAsync(Guid studentGuid)
        {
            comboSubject.Items.Clear();
            var dt = await controller.GetSubjectsByStudentAsync(studentGuid);
            foreach (DataRow row in dt.Rows)
            {
                comboSubject.Items.Add(row["SubjectName"].ToString());
            }
        }

        private async Task LoadMarksAsync()
        {
            dataGridView1.DataSource = await controller.GetAllMarksAsync();
        }

        private async void txtStudentID_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtStudentID.Text, out int referenceId))
            {
                var (guidId, name) = await controller.GetStudentByReferenceIdAsync(referenceId);
                if (guidId != Guid.Empty)
                {
                    studentGuidId = guidId;
                    txtStudentName.Text = name;
                    await LoadSubjectsAsync(guidId);
                }
                else
                {
                    txtStudentName.Text = "Not Found";
                    comboSubject.Items.Clear();
                }
            }
        }

        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (userRole == "admin") return;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // For debugging: show what is inside the StudentID cell
                var studentIdCellValue = row.Cells["StudentID"].Value?.ToString();
                if (!int.TryParse(studentIdCellValue, out int referenceId))
                {
                    MessageBox.Show($"Invalid Reference ID in selected row: '{studentIdCellValue}'");
                    return;
                }

                // Continue as normal if valid int referenceId
                selectedMarkId = Guid.TryParse(row.Cells["Id"].Value?.ToString(), out Guid markGuid) ? markGuid : Guid.Empty;

                var (studentGuid, studentName) = await controller.GetStudentByReferenceIdAsync(referenceId);

                studentGuidId = studentGuid;
                txtStudentID.Text = referenceId.ToString();
                txtStudentName.Text = studentName ?? "Unknown";

                comboSubject.SelectedItem = row.Cells["Subject"].Value?.ToString();
                comboExam.SelectedItem = row.Cells["Exam"].Value?.ToString();
                txtScore.Text = row.Cells["Score"].Value?.ToString();
            }
        }




        private void ClearForm()
        {
            txtStudentID.Clear();
            txtStudentName.Clear();
            comboSubject.SelectedIndex = -1;
            comboExam.SelectedIndex = -1;
            txtScore.Clear();
            selectedMarkId = Guid.Empty;
            studentGuidId = Guid.Empty;
        }

        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (selectedMarkId == Guid.Empty)
            {
                MessageBox.Show("Select a row to delete.");
                return;
            }

            await controller.DeleteMarkAsync(selectedMarkId);
            MessageBox.Show("Deleted Successfully !");
            await LoadMarksAsync();
            ClearForm();
        }

        private async void btnUpdate_Click_1(object sender, EventArgs e)
        {
            if (selectedMarkId == Guid.Empty || studentGuidId == Guid.Empty)
            {
                MessageBox.Show("Select a row and enter a valid student.");
                return;
            }

            if (!int.TryParse(txtScore.Text, out int score))
            {
                MessageBox.Show("Invalid Score.");
                return;
            }

            await controller.UpdateMarkAsync(selectedMarkId, studentGuidId, comboSubject.SelectedItem.ToString(), comboExam.SelectedItem.ToString(), score);
            MessageBox.Show("Updated Successfully !");
            await LoadMarksAsync();
            ClearForm();
        }

        private async void btnAdd_Click_1(object sender, EventArgs e)
        {
            if (studentGuidId == Guid.Empty || comboSubject.SelectedItem == null || comboExam.SelectedItem == null || !int.TryParse(txtScore.Text, out int score))
            {
                MessageBox.Show("Please complete all fields.");
                return;
            }

            if (score < 0 || score > 100)
            {
                MessageBox.Show("Score must be between 0 and 100.");
                return;
            }

            await controller.AddMarkAsync(studentGuidId, comboSubject.SelectedItem.ToString(), comboExam.SelectedItem.ToString(), score);
            MessageBox.Show("Mark added Successfully !");
            await LoadMarksAsync();
            ClearForm();
        }
    }
}
