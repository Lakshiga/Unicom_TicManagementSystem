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
using UnicomTicManagementSystem.Data;
using UnicomTicManagementSystem.Models;

namespace UnicomTicManagementSystem.Views
{
    public partial class SectionForm : Form
    {
        private readonly SectionController _sectionController = new SectionController();
        private Section _currentSection = null;
        private bool _isLoading = false;

        public SectionForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            InitializeForm();
            LoadSectionsAsync();
        }

        private void InitializeCustomComponents()
        {
            // Database initialization
            try
            {
                DatabaseInitializer.CreateTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database initialization failed: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit if DB fails
            }

            // Set up event handlers
            add.Click += btnAdd_Click;
            update.Click += btnUpdate_Click;
            delete.Click += btnDelete_Click;
            search.Click += btnSearch_Click;
            dgvSections.SelectionChanged += dgvSections_SelectionChanged;

            // Attach key press event handlers
            secName.KeyPress += txtSectionName_KeyPress;
            secSearch.KeyPress += txtSearch_KeyPress;

            // Form settings
            this.TopLevel = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;
        }
        
        private void InitializeForm()
        {
            // Configure DataGridView
            dgvSections.AutoGenerateColumns = false;
            dgvSections.AllowUserToAddRows = false;
            dgvSections.AllowUserToDeleteRows = false;
            dgvSections.ReadOnly = true;
            dgvSections.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSections.MultiSelect = false;

            // Add columns
            dgvSections.Columns.Clear();
            dgvSections.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });
            dgvSections.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                HeaderText = "Section Name",
                DataPropertyName = "Name",
                Width = 200
            });
            dgvSections.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CreatedDate",
                HeaderText = "Created Date",
                DataPropertyName = "CreatedDate",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });
            dgvSections.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ModifiedDate",
                HeaderText = "Modified Date",
                DataPropertyName = "ModifiedDate",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" }
            });
            
            // Clear form
            ClearForm();
        }

        private async Task LoadSectionsAsync()
        {
            try
            {
                _isLoading = true;
                SetLoadingState(true);

                var sections = await _sectionController.GetAllSectionsAsync();

                if (sections != null && sections.Any())
                {
                    dgvSections.DataSource = null;
                    dgvSections.DataSource = sections;
                    dgvSections.ClearSelection();
                }
                else
                {
                    dgvSections.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvSections.DataSource = null;
            }
            finally
            {
                _isLoading = false;
                SetLoadingState(false);
            }
        }  

        private void dgvSections_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading || dgvSections.SelectedRows.Count == 0)
                return;

            var selectedRow = dgvSections.SelectedRows[0];
            if (selectedRow.DataBoundItem is Section section)
            {
                _currentSection = section;
                secName.Text = section.Name;
                DisableEditMode();
            }
        }
        
        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var sectionName = secName.Text.Trim();
                
                if (await _sectionController.SectionNameExistsAsync(sectionName))
                {
                    MessageBox.Show($"Section '{sectionName}' already exists.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetLoadingState(true);
                
                var newSection = await _sectionController.AddSectionAsync(sectionName);
                
                if (newSection != null)
                {
                    await LoadSectionsAsync();
                    ClearForm();
                    
                    MessageBox.Show($"Section '{newSection.Name}' added successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add section. Please try again.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding section: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentSection == null)
                {
                    MessageBox.Show("Please select a section to update.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidateInput())
                    return;

                var newName = secName.Text.Trim();
                
                if (_currentSection.Name.Equals(newName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("No changes detected.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (await _sectionController.SectionNameExistsAsync(newName))
                {
                    MessageBox.Show($"Section '{newName}' already exists.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetLoadingState(true);
                var updatedSection = await _sectionController.UpdateSectionAsync(_currentSection.Id, newName);
                
                await LoadSectionsAsync();
                ClearForm();
                
                MessageBox.Show($"Section updated successfully to '{updatedSection.Name}'!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating section: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentSection == null)
                {
                    MessageBox.Show("Please select a section to delete.", "Validation Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete section '{_currentSection.Name}'?", 
                    "Confirm Delete", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    SetLoadingState(true);
                    await _sectionController.DeleteSectionAsync(_currentSection.Id);
                    
                    await LoadSectionsAsync();
                    ClearForm();
                    
                    MessageBox.Show($"Section '{_currentSection.Name}' deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting section: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                var searchTerm = secSearch.Text.Trim();

                SetLoadingState(true);
                var sections = await _sectionController.GetAllSectionsAsync(searchTerm);

                if (sections != null && sections.Any())
                {
                    dgvSections.DataSource = null;
                    dgvSections.DataSource = sections;
                    dgvSections.ClearSelection();
                }
                else
                {
                    dgvSections.DataSource = null;
                    MessageBox.Show($"No sections found matching '{searchTerm}'.", "Search Results", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnSearch_Click(sender, e);
            }
        }

        private void txtSectionName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                if (_currentSection == null)
                    btnAdd_Click(sender, e);
                else
                    btnUpdate_Click(sender, e);
            }
        }

        #region Helper Methods

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(secName.Text.Trim()))
            {
                MessageBox.Show("Please enter a section name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                secName.Focus();
                return false;
            }

            try
            {
                _sectionController.ValidateSectionName(secName.Text.Trim());
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                secName.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            _currentSection = null;
            secName.Clear();
            dgvSections.ClearSelection();
            DisableEditMode();
        }

        private void EnableEditMode()
        {
            add.Enabled = false;
            update.Enabled = false;
            delete.Enabled = false;
            secName.Focus();
        }

        private void DisableEditMode()
        {
            add.Enabled = true;
            update.Enabled = true;
            delete.Enabled = true;
        }

        private void SetLoadingState(bool isLoading)
        {
            add.Enabled = !isLoading;
            update.Enabled = !isLoading && _currentSection != null;
            delete.Enabled = !isLoading && _currentSection != null;
            search.Enabled = !isLoading;
            dgvSections.Enabled = !isLoading;
            secName.Enabled = !isLoading;
            secSearch.Enabled = !isLoading;

            if (isLoading)
            {
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        #endregion

        private void update_Click(object sender, EventArgs e)
        {

        }
    }
}
