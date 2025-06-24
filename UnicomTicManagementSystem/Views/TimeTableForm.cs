using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    public partial class TimeTableForm : Form
    {
        private TimetableController controller = new TimetableController();
        private Guid selectedTimetableId = Guid.Empty;
        private string userRole;

        public TimeTableForm(string role = "Admin")
        {
            InitializeComponent();
            userRole = role;
        }

        private async void TimeTableForm_Load(object sender, EventArgs e)
        {
            await LoadTimetableDataAsync();
            await LoadSubjectsAsync();
            await LoadRoomsAsync();
            ApplyRolePermissions();
        }

        private async Task LoadSubjectsAsync()
        {
            comboSubject.Items.Clear();
            DataTable dt = await controller.GetSubjectsAsync();
            foreach (DataRow row in dt.Rows)
            {
                comboSubject.Items.Add(row["SubjectName"].ToString());
            }
        }

        private async Task LoadRoomsAsync()
        {
            comboRoom.Items.Clear();
            DataTable dt = await controller.GetRoomsAsync();
            foreach (DataRow row in dt.Rows)
            {
                comboRoom.Items.Add(row["RoomName"].ToString());
            }
        }

        private async Task LoadTimetableDataAsync()
        {
            dataGridView1.DataSource = await controller.GetAllTimetablesAsync();
            dataGridView1.ClearSelection();
        }

        private void ClearForm()
        {
            comboSubject.SelectedIndex = -1;
            txtTimeSlot.Clear();
            textBox1.Clear();
            comboRoom.SelectedIndex = -1;
            selectedTimetableId = Guid.Empty;
            datePicker.Value = DateTime.Today;
        }

        private void ApplyRolePermissions()
        {
            var role = userRole.ToLower();
            if (role == "lecture" || role == "staff")
            {
                foreach (Control control in Controls)
                {
                    if (control is Button || control is ComboBox || control is TextBox || control is DateTimePicker)
                        control.Visible = false;
                }

                foreach (Label label in Controls.OfType<Label>())
                    label.Visible = false;

                dataGridView1.Dock = DockStyle.Fill;
                dataGridView1.ReadOnly = true;
            }
        }


        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (comboSubject.SelectedItem == null || string.IsNullOrWhiteSpace(txtTimeSlot.Text) || comboRoom.SelectedItem == null)
            {
                MessageBox.Show("Please enter all fields: Subject, TimeSlot, Room, and Date.");
                return;
            }

            string subject = comboSubject.SelectedItem.ToString();
            string timeSlot = txtTimeSlot.Text.Trim();
            string room = comboRoom.SelectedItem.ToString();
            DateTime date = datePicker.Value;

            await controller.AddTimetableAsync(subject, timeSlot, room, date);
            MessageBox.Show("Timetable entry added.");
            await LoadTimetableDataAsync();
            ClearForm();
        }


        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTimetableId == Guid.Empty)
            {
                MessageBox.Show("Select a record to update.");
                return;
            }

            string subject = comboSubject.SelectedItem.ToString();
            string timeSlot = txtTimeSlot.Text.Trim();
            string room = comboRoom.SelectedItem.ToString();
            DateTime date = datePicker.Value;

            await controller.UpdateTimetableAsync(selectedTimetableId, subject, timeSlot, room, date);
            MessageBox.Show("Timetable updated.");
            await LoadTimetableDataAsync();
            ClearForm();
        }


        private async void btnDelete_Click_1(object sender, EventArgs e)
        {
            if (selectedTimetableId == Guid.Empty)
            {
                MessageBox.Show("Select a record to delete.");
                return;
            }

            await controller.DeleteTimetableAsync(selectedTimetableId);
            MessageBox.Show("Timetable deleted.");
            await LoadTimetableDataAsync();
            ClearForm();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (userRole.ToLower() == "lecture" || userRole.ToLower() == "staff")
                return;

            if (e.RowIndex >= 0 && dataGridView1.Rows[e.RowIndex].Cells["Id"].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Convert to Guid
                selectedTimetableId = Guid.Parse(row.Cells["Id"].Value.ToString());

                comboSubject.SelectedItem = row.Cells["Subject"].Value.ToString();
                txtTimeSlot.Text = row.Cells["TimeSlot"].Value.ToString();
                comboRoom.Text = row.Cells["Room"].Value.ToString();
                textBox1.Text = Convert.ToDateTime(row.Cells["Date"].Value).ToString("yyyy-MM-dd");
                datePicker.Value = Convert.ToDateTime(row.Cells["Date"].Value);
            }
        }

        private void btnPickDate_Click(object sender, EventArgs e)
        {
            datePicker.Visible = true;
            datePicker.BringToFront();
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            textBox1.Text = datePicker.Value.ToString("yyyy-MM-dd");
            datePicker.Visible = false;
        }
    }
}
