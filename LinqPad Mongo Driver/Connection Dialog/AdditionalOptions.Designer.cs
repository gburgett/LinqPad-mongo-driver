namespace GDSX.Externals.LinqPad.Driver
{
    partial class AdditionalOptions
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
            this.components = new System.ComponentModel.Container();
            this.cbBlanketIgnoreExtraElements = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbAllowSave = new System.Windows.Forms.CheckBox();
            this.lbSaveAllowedTypes = new System.Windows.Forms.ListBox();
            this.lblSaveAllowedTypes = new System.Windows.Forms.Label();
            this.cmSaveAllowedTypes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmSaveAllowedTypes.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbBlanketIgnoreExtraElements
            // 
            this.cbBlanketIgnoreExtraElements.AutoSize = true;
            this.cbBlanketIgnoreExtraElements.Location = new System.Drawing.Point(25, 28);
            this.cbBlanketIgnoreExtraElements.Name = "cbBlanketIgnoreExtraElements";
            this.cbBlanketIgnoreExtraElements.Size = new System.Drawing.Size(168, 17);
            this.cbBlanketIgnoreExtraElements.TabIndex = 0;
            this.cbBlanketIgnoreExtraElements.Text = "Blanket Ignore Extra Elements";
            this.cbBlanketIgnoreExtraElements.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(464, 227);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(389, 227);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbAllowSave
            // 
            this.cbAllowSave.AutoSize = true;
            this.cbAllowSave.Location = new System.Drawing.Point(25, 50);
            this.cbAllowSave.Name = "cbAllowSave";
            this.cbAllowSave.Size = new System.Drawing.Size(221, 17);
            this.cbAllowSave.TabIndex = 6;
            this.cbAllowSave.Text = "Allow MongoCollection \'Save\' for all types";
            this.cbAllowSave.UseVisualStyleBackColor = true;
            // 
            // lbSaveAllowedTypes
            // 
            this.lbSaveAllowedTypes.ContextMenuStrip = this.cmSaveAllowedTypes;
            this.lbSaveAllowedTypes.FormattingEnabled = true;
            this.lbSaveAllowedTypes.Location = new System.Drawing.Point(25, 103);
            this.lbSaveAllowedTypes.Name = "lbSaveAllowedTypes";
            this.lbSaveAllowedTypes.Size = new System.Drawing.Size(247, 56);
            this.lbSaveAllowedTypes.TabIndex = 7;
            this.lbSaveAllowedTypes.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSaveAllowedTypes_MouseDoubleClick);
            // 
            // lblSaveAllowedTypes
            // 
            this.lblSaveAllowedTypes.AutoSize = true;
            this.lblSaveAllowedTypes.Location = new System.Drawing.Point(22, 78);
            this.lblSaveAllowedTypes.Name = "lblSaveAllowedTypes";
            this.lblSaveAllowedTypes.Size = new System.Drawing.Size(119, 13);
            this.lblSaveAllowedTypes.TabIndex = 8;
            this.lblSaveAllowedTypes.Text = "Types allowed to \'Save\'";
            // 
            // cmSaveAllowedTypes
            // 
            this.cmSaveAllowedTypes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.cmSaveAllowedTypes.Name = "cmSaveAllowedTypes";
            this.cmSaveAllowedTypes.Size = new System.Drawing.Size(118, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // AdditionalOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 262);
            this.Controls.Add(this.lblSaveAllowedTypes);
            this.Controls.Add(this.lbSaveAllowedTypes);
            this.Controls.Add(this.cbAllowSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbBlanketIgnoreExtraElements);
            this.Name = "AdditionalOptions";
            this.Text = "AdditionalOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AdditionalOptions_FormClosing);
            this.cmSaveAllowedTypes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbBlanketIgnoreExtraElements;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.CheckBox cbAllowSave;
        private System.Windows.Forms.ListBox lbSaveAllowedTypes;
        private System.Windows.Forms.Label lblSaveAllowedTypes;
        private System.Windows.Forms.ContextMenuStrip cmSaveAllowedTypes;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
    }
}