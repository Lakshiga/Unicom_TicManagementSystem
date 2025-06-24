using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    public partial class ExamForm : Form
    {
        private ExamController examController = new ExamController();
        private Guid selectedExamId = Guid.Empty;
        
        public ExamForm()
        {
            InitializeComponent();
            LoadSubjectsAsync();
            LoadExamsAsync();
        }

        private async void LoadSubjectsAsync()
        {
            var subjects = await examController.GetAllSubjectsAsync();
            cmbSubject.DataSource = subjects;
            cmbSubject.DisplayMember = "SubjectName";
            cmbSubject.ValueMember = "Id";
            cmbSubject.SelectedIndex = -1;
        }

        private async Task LoadExamsAsync()
        {
            dataGridView.DataSource = await examController.GetExamsAsync();
            dataGridView.ClearSelection();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                string examName = txtname.Text.Trim();
                Guid subjectId = (Guid)cmbSubject.SelectedValue;

                var exam = Exam.CreateExam(examName, subjectId);

                await examController.AddExamAsync(exam);
                MessageBox.Show("Exam added successfully.");

                ClearInputs();
                await LoadExamsAsync();
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedExamId == Guid.Empty)
            {
                MessageBox.Show("Please select an exam to update.");
                return;
            }

            if (ValidateInputs())
            {
                string examName = txtname.Text.Trim();
                Guid subjectId = (Guid)cmbSubject.SelectedValue;

                // Create a new exam instance and update its properties
                var exam = Exam.CreateExam(examName, subjectId);
                exam.UpdateId(selectedExamId); // Use the UpdateId method to set the ID

                await examController.UpdateExamAsync(exam);
                MessageBox.Show("Exam updated successfully.");

                ClearInputs();
                await LoadExamsAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedExamId == Guid.Empty)
            {
                MessageBox.Show("Please select an exam to delete.");
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this exam?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                await examController.DeleteExamAsync(selectedExamId);
                MessageBox.Show("Exam deleted successfully.");

                ClearInputs();
                await LoadExamsAsync();
            }
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView.SelectedRows[0];
                var exam = row.DataBoundItem as Exam;
                
                if (exam != null)
                {
                    selectedExamId = exam.Id;
                    txtname.Text = exam.ExamName;
                    
                    // Find the subject in the combo box
                    for (int i = 0; i < cmbSubject.Items.Count; i++)
                    {
                        var subject = cmbSubject.Items[i] as Subject;
                        if (subject != null && subject.Id == exam.SubjectId)
                        {
                            cmbSubject.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private bool ValidateInputs()
        {
            if (cmbSubject.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a subject.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtname.Text))
            {
                MessageBox.Show("Please enter an exam name.");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            txtname.Clear();
            cmbSubject.SelectedIndex = -1;
            selectedExamId = Guid.Empty;
            dataGridView.ClearSelection();
        }
    }
}
