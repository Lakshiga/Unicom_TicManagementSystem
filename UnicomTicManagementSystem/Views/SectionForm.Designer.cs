namespace UnicomTicManagementSystem.Views
{
    partial class SectionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvSections = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.secName = new System.Windows.Forms.TextBox();
            this.secSearch = new System.Windows.Forms.TextBox();
            this.add = new System.Windows.Forms.Button();
            this.update = new System.Windows.Forms.Button();
            this.search = new System.Windows.Forms.Button();
            this.delete = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSections)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSections
            // 
            this.dgvSections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSections.Location = new System.Drawing.Point(75, 289);
            this.dgvSections.Margin = new System.Windows.Forms.Padding(4);
            this.dgvSections.Name = "dgvSections";
            this.dgvSections.RowHeadersWidth = 51;
            this.dgvSections.Size = new System.Drawing.Size(544, 197);
            this.dgvSections.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 22;
            this.label1.Text = "SECTION NAME :";
            // 
            // secName
            // 
            this.secName.Location = new System.Drawing.Point(263, 82);
            this.secName.Margin = new System.Windows.Forms.Padding(4);
            this.secName.Name = "secName";
            this.secName.Size = new System.Drawing.Size(339, 22);
            this.secName.TabIndex = 17;
            // 
            // secSearch
            // 
            this.secSearch.Location = new System.Drawing.Point(263, 230);
            this.secSearch.Margin = new System.Windows.Forms.Padding(4);
            this.secSearch.Name = "secSearch";
            this.secSearch.Size = new System.Drawing.Size(339, 22);
            this.secSearch.TabIndex = 21;
            // 
            // add
            // 
            this.add.Location = new System.Drawing.Point(489, 140);
            this.add.Margin = new System.Windows.Forms.Padding(4);
            this.add.Name = "add";
            this.add.Size = new System.Drawing.Size(113, 41);
            this.add.TabIndex = 15;
            this.add.Text = "ADD";
            this.add.UseVisualStyleBackColor = true;
            // 
            // update
            // 
            this.update.Location = new System.Drawing.Point(272, 140);
            this.update.Margin = new System.Windows.Forms.Padding(4);
            this.update.Name = "update";
            this.update.Size = new System.Drawing.Size(111, 39);
            this.update.TabIndex = 13;
            this.update.Text = "UPDATE";
            this.update.UseVisualStyleBackColor = true;
            this.update.Click += new System.EventHandler(this.update_Click);
            // 
            // search
            // 
            this.search.Location = new System.Drawing.Point(101, 213);
            this.search.Margin = new System.Windows.Forms.Padding(4);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(112, 39);
            this.search.TabIndex = 19;
            this.search.Text = "SEARCH";
            this.search.UseVisualStyleBackColor = true;
            // 
            // delete
            // 
            this.delete.Location = new System.Drawing.Point(101, 140);
            this.delete.Margin = new System.Windows.Forms.Padding(4);
            this.delete.Name = "delete";
            this.delete.Size = new System.Drawing.Size(113, 39);
            this.delete.TabIndex = 11;
            this.delete.Text = "DELETE";
            this.delete.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(267, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 25);
            this.label2.TabIndex = 25;
            this.label2.Text = "MANAGE SECTIONS";
            // 
            // SectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 554);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvSections);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.secSearch);
            this.Controls.Add(this.search);
            this.Controls.Add(this.secName);
            this.Controls.Add(this.add);
            this.Controls.Add(this.update);
            this.Controls.Add(this.delete);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SectionForm";
            this.Text = "SectionForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSections)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSections;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox secName;
        private System.Windows.Forms.TextBox secSearch;
        private System.Windows.Forms.Button add;
        private System.Windows.Forms.Button update;
        private System.Windows.Forms.Button search;
        private System.Windows.Forms.Button delete;
        private System.Windows.Forms.Label label2;
        
    }
}