using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicomTicManagementSystem.Controllers;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    /// <summary>
    /// Example form demonstrating proper async/await implementation for database operations
    /// This form shows best practices for keeping the UI responsive during database operations
    /// </summary>
    public partial class AsyncExampleForm : Form
    {
        private readonly TeacherController _teacherController;
        private readonly SectionController _sectionController;
        private bool _isLoading = false;

        public AsyncExampleForm()
        {
            InitializeComponent();
            _teacherController = new TeacherController();
            _sectionController = new SectionController();
            
            // Load data asynchronously when form loads
            LoadFormDataAsync();
        }

        /// <summary>
        /// Loads all form data asynchronously
        /// </summary>
        private async Task LoadFormDataAsync()
        {
            try
            {
                SetLoadingState(true);
                
                // Load teachers and sections in parallel for better performance
                var teachersTask = _teacherController.GetAllTeachersAsync();
                var sectionsTask = _sectionController.GetAllSectionsAsync();
                
                await Task.WhenAll(teachersTask, sectionsTask);
                
                // Update UI with loaded data
                dgvTeachers.DataSource = teachersTask.Result;
                cmbSections.DataSource = sectionsTask.Result;
                cmbSections.DisplayMember = "Name";
                cmbSections.ValueMember = "Id";
                
                // Clear selections
                dgvTeachers.ClearSelection();
                cmbSections.SelectedIndex = -1;
                
                UpdateStatusLabel($"Loaded {teachersTask.Result.Count} teachers and {sectionsTask.Result.Count} sections");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Error loading data");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Example of async button click handler for adding a teacher
        /// </summary>
        private async void btnAddTeacher_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                SetLoadingState(true);
                
                // Create teacher object
                var teacher = new Teacher
                {
                    Name = txtTeacherName.Text.Trim(),
                    Phone = txtTeacherPhone.Text.Trim(),
                    Address = txtTeacherAddress.Text.Trim()
                };

                // Add teacher asynchronously
                await _teacherController.AddTeacherAsync(teacher);
                
                // Reload data to show the new teacher
                await LoadFormDataAsync();
                
                // Clear form
                ClearTeacherForm();
                
                MessageBox.Show("Teacher added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel("Teacher added successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding teacher: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Error adding teacher");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Example of async button click handler for updating a teacher
        /// </summary>
        private async void btnUpdateTeacher_Click(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a teacher to update.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInput())
                return;

            try
            {
                SetLoadingState(true);
                
                var selectedTeacher = dgvTeachers.SelectedRows[0].DataBoundItem as Teacher;
                if (selectedTeacher == null)
                {
                    MessageBox.Show("Invalid teacher selection.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update teacher properties
                selectedTeacher.Name = txtTeacherName.Text.Trim();
                selectedTeacher.Phone = txtTeacherPhone.Text.Trim();
                selectedTeacher.Address = txtTeacherAddress.Text.Trim();

                // Update teacher asynchronously
                await _teacherController.UpdateTeacherAsync(selectedTeacher);
                
                // Reload data to show the updated teacher
                await LoadFormDataAsync();
                
                // Clear form
                ClearTeacherForm();
                
                MessageBox.Show("Teacher updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel("Teacher updated successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating teacher: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Error updating teacher");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Example of async button click handler for deleting a teacher
        /// </summary>
        private async void btnDeleteTeacher_Click(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a teacher to delete.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedTeacher = dgvTeachers.SelectedRows[0].DataBoundItem as Teacher;
            if (selectedTeacher == null)
            {
                MessageBox.Show("Invalid teacher selection.", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirm deletion
            var result = MessageBox.Show(
                $"Are you sure you want to delete teacher '{selectedTeacher.Name}'?", 
                "Confirm Deletion", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                SetLoadingState(true);
                
                // Delete teacher asynchronously
                await _teacherController.DeleteTeacherAsync(selectedTeacher.Id);
                
                // Reload data to reflect the deletion
                await LoadFormDataAsync();
                
                // Clear form
                ClearTeacherForm();
                
                MessageBox.Show("Teacher deleted successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusLabel("Teacher deleted successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting teacher: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Error deleting teacher");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Example of async search functionality
        /// </summary>
        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var searchTerm = txtSearch.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If search is empty, reload all data
                await LoadFormDataAsync();
                return;
            }

            try
            {
                SetLoadingState(true);
                
                // Search teachers asynchronously
                var searchResults = await _teacherController.SearchTeachersAsync(searchTerm);
                
                // Update UI with search results
                dgvTeachers.DataSource = searchResults;
                dgvTeachers.ClearSelection();
                
                UpdateStatusLabel($"Found {searchResults.Count} teachers matching '{searchTerm}'");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching teachers: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatusLabel("Error searching teachers");
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        /// <summary>
        /// Example of async data refresh
        /// </summary>
        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadFormDataAsync();
        }

        /// <summary>
        /// Handles teacher selection in the grid
        /// </summary>
        private void dgvTeachers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTeachers.SelectedRows.Count > 0 && !_isLoading)
            {
                var selectedTeacher = dgvTeachers.SelectedRows[0].DataBoundItem as Teacher;
                if (selectedTeacher != null)
                {
                    // Populate form with selected teacher data
                    txtTeacherName.Text = selectedTeacher.Name;
                    txtTeacherPhone.Text = selectedTeacher.Phone;
                    txtTeacherAddress.Text = selectedTeacher.Address;
                }
            }
        }

        /// <summary>
        /// Validates form input
        /// </summary>
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTeacherName.Text))
            {
                MessageBox.Show("Teacher name is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTeacherName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTeacherPhone.Text))
            {
                MessageBox.Show("Teacher phone is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTeacherPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTeacherAddress.Text))
            {
                MessageBox.Show("Teacher address is required.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTeacherAddress.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears the teacher form
        /// </summary>
        private void ClearTeacherForm()
        {
            txtTeacherName.Clear();
            txtTeacherPhone.Clear();
            txtTeacherAddress.Clear();
            dgvTeachers.ClearSelection();
        }

        /// <summary>
        /// Sets the loading state for the form
        /// </summary>
        private void SetLoadingState(bool isLoading)
        {
            _isLoading = isLoading;
            
            // Update cursor
            Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            
            // Enable/disable controls
            btnAddTeacher.Enabled = !isLoading;
            btnUpdateTeacher.Enabled = !isLoading;
            btnDeleteTeacher.Enabled = !isLoading;
            btnSearch.Enabled = !isLoading;
            btnRefresh.Enabled = !isLoading;
            
            // Show/hide loading indicator
            if (isLoading)
            {
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "Loading...";
            }
            else
            {
                progressBar.Visible = false;
            }
        }

        /// <summary>
        /// Updates the status label
        /// </summary>
        private void UpdateStatusLabel(string message)
        {
            lblStatus.Text = message;
        }

        /// <summary>
        /// Handles form closing
        /// </summary>
        private void AsyncExampleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Cancel any ongoing operations if needed
            if (_isLoading)
            {
                var result = MessageBox.Show(
                    "An operation is in progress. Are you sure you want to close?", 
                    "Confirm Close", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
} 