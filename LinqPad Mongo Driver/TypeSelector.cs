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
    public partial class TypeSelector : Form
    {
        public Type SelectedType { get; private set; }

        public Type SelectedSerializer { get; private set; }

        public TypeSelector(TreeNode[] allTypes, TreeNode[] allSerializers)
        {
            InitializeComponent();

            this.tvLeft.Nodes.AddRange(allTypes);
            this.tvRight.Nodes.AddRange(allSerializers);
        }
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            if(this.tvLeft.SelectedNode == null || this.tvRight.SelectedNode == null)
            {
                MessageBox.Show("You must select a type and a serializer for that type");
                return;
            }
                

            var type = this.tvLeft.SelectedNode.Tag as Type;
            var serializer = this.tvRight.SelectedNode.Tag as Type;

            if(type== null || serializer == null)
            {
                MessageBox.Show("You must select a type and a serializer for that type");
                return;
            }

            this.SelectedType = type;
            this.SelectedSerializer = serializer;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void tvRight_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvRight.SelectedNode == null ||
                (this.tvRight.SelectedNode.Tag as Type) == null)
            {
                this.lblSerializers.Text = "Serializer";
                this.SelectedSerializer = null;
                return;
            }

            this.SelectedSerializer = this.tvRight.SelectedNode.Tag as Type;
            this.lblSerializers.Text = this.SelectedSerializer.ToString();
        }

        private void tvLeft_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.tvLeft.SelectedNode == null ||
                (this.tvLeft.SelectedNode.Tag as Type) == null)
            {
                this.lblTypes.Text = "Type";
                this.SelectedType = null;
                return;
            }

            this.SelectedType = this.tvLeft.SelectedNode.Tag as Type;
            this.lblTypes.Text = this.SelectedType.ToString();
        }
        
    }
}
