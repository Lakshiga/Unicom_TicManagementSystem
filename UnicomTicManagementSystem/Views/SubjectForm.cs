using System;
using System.Data;
using System.Windows.Forms;
using System.Threading.Tasks;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    public partial class SubjectForm : Form
    {
        SubjectController controller = new SubjectController();
        Guid selectedSubjectId = Guid.Empty;

        public SubjectForm()
        {
            InitializeComponent();
            LoadSubjectsAsync();
            LoadSectionsAsync();
        }

        private async void LoadSectionsAsync()
        {
            var sections = await controller.GetAllSectionsAsync();

            // For Add/Update
            comboBoxSelectSection.DataSource = sections;
            comboBoxSelectSection.DisplayMember = "Name";
            comboBoxSelectSection.ValueMember = "Id";
            comboBoxSelectSection.SelectedIndex = -1;

            // For Search
            comboBoxSearchSection.DataSource = sections;
            comboBoxSearchSection.DisplayMember = "Name";
            comboBoxSearchSection.ValueMember = "Id";
            comboBoxSearchSection.SelectedIndex = -1;
        }

        private async Task LoadSubjectsAsync()
        {
            var subjects = await controller.GetSubjectsAsync();

            // Bind to DataGridView
            dataGridView1.DataSource = subjects;

            // Hide SectionId column (if present)
            if (dataGridView1.Columns.Contains("SectionId"))
            {
                dataGridView1.Columns["SectionId"].Visible = false;
            }
        }

        private void SubjectForm_Load(object sender, EventArgs e)
        {
            // Optional: logic to run on form load
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtSubjectName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || comboBoxSelectSection.SelectedValue == null)
            {
                MessageBox.Show("Please enter subject name and select a section.");
                return;
            }

            Guid sectionId = (Guid)comboBoxSelectSection.SelectedValue;

            var subject = Subject.CreateSubject(name, sectionId);

            await controller.AddSubjectAsync(subject);
            MessageBox.Show("Subject added successfully.");

            await LoadSubjectsAsync();
            ClearForm();
        }

        private async void btn_update_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId == Guid.Empty)
            {
                MessageBox.Show("Please select a subject to update.");
                return;
            }

            string name = txtSubjectName.Text.Trim();

            if (comboBoxSelectSection.SelectedValue == null || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a subject name and select a section.");
                return;
            }
            Guid sectionId = (Guid)comboBoxSelectSection.SelectedValue;

            var subject = Subject.CreateSubject(name, sectionId);
            subject.Id = selectedSubjectId;

            await controller.UpdateSubjectAsync(subject);
            MessageBox.Show("Subject updated successfully.");

            await LoadSubjectsAsync();
            ClearForm();
        }

        private async void btn_delete_Click(object sender, EventArgs e)
        {
            if (selectedSubjectId == Guid.Empty)
            {
                MessageBox.Show("Please select a subject to delete.");
                return;
            }

            await controller.DeleteSubjectAsync(selectedSubjectId);
            MessageBox.Show("Subject deleted successfully.");

            await LoadSubjectsAsync();
            ClearForm();
        }

        private async void btn_search_Click(object sender, EventArgs e)
        {
            if (comboBoxSearchSection.SelectedValue == null)
            {
                MessageBox.Show("Please select a section to search.");
                return;
            }

            Guid sectionId = (Guid)comboBoxSearchSection.SelectedValue;
            var subjects = await controller.GetSubjectsBySectionAsync(sectionId);
            dataGridView1.DataSource = subjects;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                var subject = row.DataBoundItem as Subject;
                
                if (subject != null)
                {
                    selectedSubjectId = subject.Id;
                    txtSubjectName.Text = subject.SubjectName;

                    // Find the section in the combo box
                    for (int i = 0; i < comboBoxSelectSection.Items.Count; i++)
                    {
                        var section = comboBoxSelectSection.Items[i] as Section;
                        if (section != null && section.Id == subject.SectionId)
                        {
                            comboBoxSelectSection.SelectedIndex = i;
                            comboBoxSearchSection.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            selectedSubjectId = Guid.Empty;
            txtSubjectName.Clear();
            comboBoxSearchSection.SelectedIndex = -1;
            comboBoxSelectSection.SelectedIndex = -1;
        }

        private void comboBoxSearchSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: handle selection changes
            // Example: Debug.WriteLine(comboBoxSearchSection.SelectedValue?.ToString());
        }

        private void comboBoxSelectSection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
