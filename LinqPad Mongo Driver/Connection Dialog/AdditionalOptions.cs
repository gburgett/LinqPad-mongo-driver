using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GDSX.Externals.LinqPad.Driver
{
    public partial class AdditionalOptions : Form
    {
        private ConnectionProperties mProps;
        private ICollection<Type> mLoadedTypes;
        private bool mCloseAlreadyValidated = false;

        public ConnectionAdditionalOptions SelectedOptions { get; set; }

        public AdditionalOptions(ConnectionProperties props, ICollection<Type> loadedTypes)
        {
            this.mProps = props;
            this.mLoadedTypes = loadedTypes;

            InitializeComponent();
            this.addToolStripMenuItem.Click += AddToolStripMenuItemOnClick;
            this.removeToolStripMenuItem.Click += RemoveToolStripMenuItemOnClick;

            InitToolTips();

            LoadFromProps();
        }


        /// <summary>
        /// Initializes the ToolTip text
        /// </summary>
        private void InitToolTips()
        {
            toolTip.SetToolTip(this.cbBlanketIgnoreExtraElements,
@"Check this box to have the LinqPad driver automatically ignore all extra elements 
when deserializing any type from BSON.");

            toolTip.SetToolTip(this.cbAllowSave, 
@"If unchecked, the driver will throw an exception whenever
an attempt is made to call MongoCollection.Save<T>(T obj),
unless T is an explicitly allowed type.  You can still use
save on any collections retrieved using db.GetCollection<>().");

            toolTip.SetToolTip(this.lbSaveAllowedTypes,
@"This is the list of explicitly allowed types which will not cause
exceptions to be thrown when a call is made to MongoCollection.Save<T>(T obj).
Doubleclick to add types, right-click to remove selected types.");
        }

        /// <summary>
        /// Sets the current values of the visible controls to the values in the properties.
        /// </summary>
        private void LoadFromProps()
        {
            this.cbAllowSave.Checked = this.mProps.AdditionalOptions.AllowSaveForAllTypes;
            this.cbBlanketIgnoreExtraElements.Checked = this.mProps.AdditionalOptions.BlanketIgnoreExtraElements;
            foreach (string type in this.mProps.AdditionalOptions.ExplicitSaveAllowedTypes)
            {
                this.lbSaveAllowedTypes.Items.Add(type);
            }
        }

        /// <summary>
        /// Populates the Connection Options with the current values in the form controls
        /// </summary>
        /// <param name="options"></param>
        private void Populate(ConnectionAdditionalOptions options)
        {
            options.AllowSaveForAllTypes = this.cbAllowSave.Checked;
            options.BlanketIgnoreExtraElements = this.cbBlanketIgnoreExtraElements.Checked;

            options.ExplicitSaveAllowedTypes.AddRange(this.lbSaveAllowedTypes.Items.Cast<string>());
        }


        /// <summary>
        /// Checks to see if the data changed from save, and if so, prompts the user.
        /// </summary>
        /// <returns>True if the form should close, False if the form should stay open.</returns>
        private bool DoCancel()
        {
            var working = new ConnectionAdditionalOptions();
            this.Populate(working);

            if (!working.Equals(this.mProps.AdditionalOptions))
            {
                var result = MessageBox.Show("Changes have been made.  Do you want to save first?", "Cancel", MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return true;
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    return false;
                }
                else if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    return this.DoSave();
                }

                return false;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            return true;
        }

        /// <summary>
        /// validates and saves the data to the connection
        /// </summary>
        /// <returns>true if the data successfully saved, false if the form should stay open</returns>
        private bool DoSave()
        {
            var options = new ConnectionAdditionalOptions();
            this.Populate(options);
            this.SelectedOptions = options;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            return true;
        }


        private void ShowAddAllowTypeDialog()
        {
            IEnumerable<Type> typesInTree;
            if (string.IsNullOrEmpty(mProps.SelectedDatabase))
            {
                typesInTree = this.mLoadedTypes;
            }
            else
            {
                HashSet<String> visibleTypes = new HashSet<string>(mProps.CollectionTypeMappings[mProps.SelectedDatabase].Select(x => x.CollectionType).Where(x => x != null));
                typesInTree = mLoadedTypes.Where(x => visibleTypes.Contains(x.ToString()) ||
                                    //backwards compatibility
                                (x.AssemblyQualifiedName != null && visibleTypes.Contains(x.AssemblyQualifiedName)));
            }
            TreeNode[] nodes = ConnectionDialog.MakeTree(typesInTree);

            using (var selector = new SingleTypeSelector(nodes))
            {
                selector.Name = "Select Type to allow Save";

                DialogResult result = selector.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.lbSaveAllowedTypes.Items.Add(selector.SelectedType.ToString());
                }
            }
        }

        #region Event Handlers
        private void AdditionalOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.mCloseAlreadyValidated)
            {
                e.Cancel = !this.DoCancel();
                this.mCloseAlreadyValidated = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.DoSave())
            {
                this.mCloseAlreadyValidated = true;
                this.Close();
            }

            this.mCloseAlreadyValidated = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.DoCancel())
            {
                this.mCloseAlreadyValidated = true;
                this.Close();
            }

            this.mCloseAlreadyValidated = false;
        }


        private void lbSaveAllowedTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowAddAllowTypeDialog();
        }

        private void RemoveToolStripMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            ListBox.SelectedObjectCollection selected = this.lbSaveAllowedTypes.SelectedItems;
            if (selected.Count == 0)
                return;

            foreach (var item in selected.Cast<object>().ToArray())
            {
                this.lbSaveAllowedTypes.Items.Remove(item);
            }
        }

        private void AddToolStripMenuItemOnClick(object sender, EventArgs eventArgs)
        {
            this.ShowAddAllowTypeDialog();
        }
        #endregion

        
    }
}
