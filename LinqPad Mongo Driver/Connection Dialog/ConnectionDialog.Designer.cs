namespace GDSX.Externals.LinqPad.Driver
{
    partial class ConnectionDialog
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
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.dgCollectionTypes = new System.Windows.Forms.DataGridView();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cbDatabases = new System.Windows.Forms.ComboBox();
            this.lblDb = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRemoveAssembly = new System.Windows.Forms.Button();
            this.btnAddAssembly = new System.Windows.Forms.Button();
            this.lblAssemblies = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.bntImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.lbLoadedAssemblies = new System.Windows.Forms.ListBox();
            this.tvKnownTypes = new System.Windows.Forms.TreeView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbCustomSerializers = new System.Windows.Forms.ListBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblCustomSerializers = new System.Windows.Forms.Label();
            this.btnRemoveSerializers = new System.Windows.Forms.Button();
            this.btnAddSerializer = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.additionalOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.additionalOptionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgCollectionTypes)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(100, 6);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(499, 20);
            this.txtConnectionString.TabIndex = 0;
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.AutoSize = true;
            this.lblConnectionString.Location = new System.Drawing.Point(3, 9);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(91, 13);
            this.lblConnectionString.TabIndex = 1;
            this.lblConnectionString.Text = "Connection String";
            // 
            // dgCollectionTypes
            // 
            this.dgCollectionTypes.AllowDrop = true;
            this.dgCollectionTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCollectionTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgCollectionTypes.Location = new System.Drawing.Point(3, 191);
            this.dgCollectionTypes.Name = "dgCollectionTypes";
            this.tableLayoutPanel1.SetRowSpan(this.dgCollectionTypes, 2);
            this.dgCollectionTypes.Size = new System.Drawing.Size(410, 384);
            this.dgCollectionTypes.TabIndex = 6;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(605, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 22);
            this.btnConnect.TabIndex = 11;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cbDatabases
            // 
            this.cbDatabases.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDatabases.FormattingEnabled = true;
            this.cbDatabases.Location = new System.Drawing.Point(100, 32);
            this.cbDatabases.Name = "cbDatabases";
            this.cbDatabases.Size = new System.Drawing.Size(499, 21);
            this.cbDatabases.TabIndex = 13;
            this.cbDatabases.SelectedValueChanged += new System.EventHandler(this.cbDatabases_SelectedValueChanged);
            // 
            // lblDb
            // 
            this.lblDb.AutoSize = true;
            this.lblDb.Location = new System.Drawing.Point(41, 35);
            this.lblDb.Name = "lblDb";
            this.lblDb.Size = new System.Drawing.Size(53, 13);
            this.lblDb.TabIndex = 14;
            this.lblDb.Text = "Database";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.tableLayoutPanel1.Controls.Add(this.dgCollectionTypes, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lbLoadedAssemblies, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tvKnownTypes, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbCustomSerializers, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 93F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 353F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(736, 578);
            this.tableLayoutPanel1.TabIndex = 18;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRemoveAssembly);
            this.panel1.Controls.Add(this.btnAddAssembly);
            this.panel1.Controls.Add(this.lblAssemblies);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(419, 68);
            this.panel1.MaximumSize = new System.Drawing.Size(0, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(314, 24);
            this.panel1.TabIndex = 7;
            // 
            // btnRemoveAssembly
            // 
            this.btnRemoveAssembly.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRemoveAssembly.Location = new System.Drawing.Point(144, 0);
            this.btnRemoveAssembly.Name = "btnRemoveAssembly";
            this.btnRemoveAssembly.Size = new System.Drawing.Size(75, 24);
            this.btnRemoveAssembly.TabIndex = 3;
            this.btnRemoveAssembly.Text = "Remove";
            this.btnRemoveAssembly.UseVisualStyleBackColor = true;
            this.btnRemoveAssembly.Click += new System.EventHandler(this.btnRemoveAssembly_Click);
            // 
            // btnAddAssembly
            // 
            this.btnAddAssembly.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAddAssembly.Location = new System.Drawing.Point(69, 0);
            this.btnAddAssembly.Name = "btnAddAssembly";
            this.btnAddAssembly.Size = new System.Drawing.Size(75, 24);
            this.btnAddAssembly.TabIndex = 2;
            this.btnAddAssembly.Text = "Add";
            this.btnAddAssembly.UseVisualStyleBackColor = true;
            this.btnAddAssembly.Click += new System.EventHandler(this.btnAddAssembly_Click);
            // 
            // lblAssemblies
            // 
            this.lblAssemblies.AutoSize = true;
            this.lblAssemblies.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblAssemblies.Location = new System.Drawing.Point(0, 0);
            this.lblAssemblies.Name = "lblAssemblies";
            this.lblAssemblies.Padding = new System.Windows.Forms.Padding(5);
            this.lblAssemblies.Size = new System.Drawing.Size(69, 23);
            this.lblAssemblies.TabIndex = 0;
            this.lblAssemblies.Text = "Assemblies";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.bntImport);
            this.panel2.Controls.Add(this.btnExport);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(419, 544);
            this.panel2.MaximumSize = new System.Drawing.Size(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 30);
            this.panel2.TabIndex = 8;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(240, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(165, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // bntImport
            // 
            this.bntImport.Location = new System.Drawing.Point(84, 0);
            this.bntImport.Name = "bntImport";
            this.bntImport.Size = new System.Drawing.Size(75, 23);
            this.bntImport.TabIndex = 1;
            this.bntImport.Text = "Import";
            this.bntImport.UseVisualStyleBackColor = true;
            this.bntImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(3, 0);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 0;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // lbLoadedAssemblies
            // 
            this.lbLoadedAssemblies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbLoadedAssemblies.FormattingEnabled = true;
            this.lbLoadedAssemblies.Location = new System.Drawing.Point(419, 98);
            this.lbLoadedAssemblies.Name = "lbLoadedAssemblies";
            this.lbLoadedAssemblies.Size = new System.Drawing.Size(314, 87);
            this.lbLoadedAssemblies.TabIndex = 9;
            this.lbLoadedAssemblies.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbLoadedAssemblies_MouseDoubleClick);
            // 
            // tvKnownTypes
            // 
            this.tvKnownTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvKnownTypes.Location = new System.Drawing.Point(419, 191);
            this.tvKnownTypes.Name = "tvKnownTypes";
            this.tvKnownTypes.Size = new System.Drawing.Size(314, 347);
            this.tvKnownTypes.TabIndex = 10;
            this.tvKnownTypes.DoubleClick += new System.EventHandler(this.tvKnownTypes_DoubleClick);
            // 
            // panel3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel3, 2);
            this.panel3.Controls.Add(this.txtConnectionString);
            this.panel3.Controls.Add(this.cbDatabases);
            this.panel3.Controls.Add(this.lblDb);
            this.panel3.Controls.Add(this.lblConnectionString);
            this.panel3.Controls.Add(this.btnConnect);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(730, 59);
            this.panel3.TabIndex = 11;
            // 
            // lbCustomSerializers
            // 
            this.lbCustomSerializers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbCustomSerializers.FormattingEnabled = true;
            this.lbCustomSerializers.Location = new System.Drawing.Point(3, 98);
            this.lbCustomSerializers.Name = "lbCustomSerializers";
            this.lbCustomSerializers.Size = new System.Drawing.Size(410, 87);
            this.lbCustomSerializers.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblCustomSerializers);
            this.panel4.Controls.Add(this.btnRemoveSerializers);
            this.panel4.Controls.Add(this.btnAddSerializer);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 68);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(410, 24);
            this.panel4.TabIndex = 16;
            // 
            // lblCustomSerializers
            // 
            this.lblCustomSerializers.AutoSize = true;
            this.lblCustomSerializers.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblCustomSerializers.Location = new System.Drawing.Point(150, 0);
            this.lblCustomSerializers.Name = "lblCustomSerializers";
            this.lblCustomSerializers.Size = new System.Drawing.Size(92, 13);
            this.lblCustomSerializers.TabIndex = 2;
            this.lblCustomSerializers.Text = "Custom Serializers";
            this.lblCustomSerializers.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRemoveSerializers
            // 
            this.btnRemoveSerializers.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRemoveSerializers.Location = new System.Drawing.Point(75, 0);
            this.btnRemoveSerializers.Name = "btnRemoveSerializers";
            this.btnRemoveSerializers.Size = new System.Drawing.Size(75, 24);
            this.btnRemoveSerializers.TabIndex = 1;
            this.btnRemoveSerializers.Text = "Remove";
            this.btnRemoveSerializers.UseVisualStyleBackColor = true;
            this.btnRemoveSerializers.Click += new System.EventHandler(this.btnRemoveSerializers_Click);
            // 
            // btnAddSerializer
            // 
            this.btnAddSerializer.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAddSerializer.Location = new System.Drawing.Point(0, 0);
            this.btnAddSerializer.Name = "btnAddSerializer";
            this.btnAddSerializer.Size = new System.Drawing.Size(75, 24);
            this.btnAddSerializer.TabIndex = 0;
            this.btnAddSerializer.Text = "Add";
            this.btnAddSerializer.UseVisualStyleBackColor = true;
            this.btnAddSerializer.Click += new System.EventHandler(this.btnAddSerializer_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.additionalOptionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(736, 24);
            this.menuStrip1.TabIndex = 20;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.cancelToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.loadToolStripMenuItem.Text = "Import";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // cancelToolStripMenuItem
            // 
            this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
            this.cancelToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cancelToolStripMenuItem.Text = "Cancel";
            this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
            // 
            // additionalOptionsToolStripMenuItem
            // 
            this.additionalOptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.additionalOptionsToolStripMenuItem1});
            this.additionalOptionsToolStripMenuItem.Name = "additionalOptionsToolStripMenuItem";
            this.additionalOptionsToolStripMenuItem.Size = new System.Drawing.Size(119, 20);
            this.additionalOptionsToolStripMenuItem.Text = "Additional Options";
            // 
            // additionalOptionsToolStripMenuItem1
            // 
            this.additionalOptionsToolStripMenuItem1.Name = "additionalOptionsToolStripMenuItem1";
            this.additionalOptionsToolStripMenuItem1.Size = new System.Drawing.Size(174, 22);
            this.additionalOptionsToolStripMenuItem1.Text = "Additional Options";
            this.additionalOptionsToolStripMenuItem1.Click += new System.EventHandler(this.additionalOptionsToolStripMenuItem1_Click);
            // 
            // ConnectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 602);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ConnectionDialog";
            this.Text = "ConnectionDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgCollectionTypes)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label lblConnectionString;
        private System.Windows.Forms.DataGridView dgCollectionTypes;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cbDatabases;
        private System.Windows.Forms.Label lblDb;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblAssemblies;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnAddAssembly;
        private System.Windows.Forms.ListBox lbLoadedAssemblies;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button bntImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TreeView tvKnownTypes;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnRemoveAssembly;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblCustomSerializers;
        private System.Windows.Forms.Button btnRemoveSerializers;
        private System.Windows.Forms.Button btnAddSerializer;
        private System.Windows.Forms.ListBox lbCustomSerializers;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem additionalOptionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem additionalOptionsToolStripMenuItem1;
    }
}