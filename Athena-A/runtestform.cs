using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Athena_A
{
    public partial class runtestform : Form
    {
        public runtestform()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s == "")
            {
                MessageBox.Show("请指定需要测试的可执行文件。", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(s) == false)
            {
                MessageBox.Show("指定的可执行文件不存在，无法进行测试。", "敬告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        cmd.CommandText = "update fileinfo set detail = '" + mainform.runtest + "' where infoname = '运行'";
                        cmd.ExecuteNonQuery();
                    }
                }
                System.Diagnostics.Process.Start(mainform.runtest);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "EXE 文件(*.EXE)|*.EXE";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                if (CommonCode.PE(s) == true)
                {
                    textBox1.Text = s;
                    mainform.runtest = s;
                }
                else
                {
                    MessageBox.Show("指定的文件不是有效的 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void runtestform_Shown(object sender, EventArgs e)
        {
            string s = mainform.runtest;
            if (File.Exists(s) == true)
            {
                textBox1.Text = s;
            }
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
