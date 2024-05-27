using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class Statistics : Form
    {
        public Statistics()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Statistics_Shown(object sender, EventArgs e)
        {
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label2.Height / 2D));
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select count(address) from athenaa";
                        object ob = cmd.ExecuteScalar();
                        textBox1.Text = ob.ToString();
                        cmd.CommandText = "select count(address) from athenaa where tralong > 0";
                        ob = cmd.ExecuteScalar();
                        textBox2.Text = ob.ToString();
                    }
                }
            }
        }
    }
}
