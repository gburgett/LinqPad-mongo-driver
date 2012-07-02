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
    public partial class SingleTypeSelector : Form
    {
        /// <summary>
        /// Gets the <see cref="Type"/> that was selected by doubleclicking a node in the <see cref="TreeView"/>.
        /// </summary>
        public Type SelectedType { get; private set; }

        private TreeNode[] mVisibleTypes;

        /// <summary>
        /// Displays the given TreeNodes in a <see cref="TreeView"/>.  Expects the TreeNodes
        /// to be tagged with <see cref="Type"/> objects so they can be selected.
        /// </summary>
        /// <param name="visibleTypes">The <see cref="TreeNode"/> array to be displayed</param>
        public SingleTypeSelector(TreeNode[] visibleTypes)
        {
            this.mVisibleTypes = visibleTypes;

            InitializeComponent();

            this.treeView1.Nodes.AddRange(this.mVisibleTypes);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;

            Type item = treeView1.SelectedNode.Tag as Type;
            if (item == null)
                return;

            SelectedType = item;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
