using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class AddSection : Form
    {
        public AddSection()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s == "")
            {
                this.Close();
            }
            else
            {
                try
                {
                    int i = int.Parse(textBox1.Text);
                    if (i <= 0 || i > 1024000)
                    {
                        MessageBox.Show("请输入一个介于 1 到 1024000 之间的一个整数。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        EditSection.AddBytes = i;
                        if (radioButton1.Checked)
                        {
                            EditSection.SectionCharacteristics = "只读";
                        }
                        else
                        {
                            EditSection.SectionCharacteristics = "可执行";
                        }
                        this.Close();
                    }
                }
                catch
                {
                    MessageBox.Show("请输入一个介于 1 到 1024000 之间的一个整数。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddSection_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
                textBox1.Width = button1.Width;
            }
        }
    }
}
