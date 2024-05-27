using System;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class SearchDictionary : Form
    {
        public SearchDictionary()
        {
            InitializeComponent();
        }

        private void SearchDictionary_Shown(object sender, EventArgs e)
        {
            contextMenuStrip1.Font = this.Font;
            contextMenuStrip2.Font = this.Font;
        }

        private void SearchDictionary_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void 应用ToolStripMenuItemOrg_Click(object sender, EventArgs e)
        {
            Application.OpenForms[0].Controls[1].Controls[0].Controls[1].Controls[0].Text = SearchDictionaryDataTable1.Rows[dataGridView2.CurrentCell.RowIndex][1].ToString();
            Application.OpenForms[0].Activate();
        }

        private void 应用ToolStripMenuItemTra_Click(object sender, EventArgs e)
        {
            Application.OpenForms[0].Controls[1].Controls[0].Controls[1].Controls[0].Text = SearchDictionaryDataTable1.Rows[dataGridView2.CurrentCell.RowIndex][1].ToString();
            Application.OpenForms[0].Activate();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SearchDictionaryDataTable1.Rows.Count > 0)
                {
                    dataGridView1.ContextMenuStrip = contextMenuStrip1;
                    contextMenuStrip1.Show(MousePosition);
                }
            }
            else
            {
                dataGridView1.ContextMenuStrip = null;
            }
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (SearchDictionaryDataTable1.Rows.Count > 0)
                {
                    dataGridView2.ContextMenuStrip = contextMenuStrip2;
                    contextMenuStrip2.Show(MousePosition);
                }
            }
            else
            {
                dataGridView2.ContextMenuStrip = null;
            }
        }
    }
}
