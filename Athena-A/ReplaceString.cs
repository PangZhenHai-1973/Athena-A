using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class ReplaceString : Form
    {
        public ReplaceString()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainform.RStr1 = textBox1.Text;
            string s2 = textBox2.Text;
            if (checkBox1.Checked)
            {
                if (s2 == "\\t")
                {
                    s2 = "\t";
                }
                else if (s2 == "\\r")
                {
                    s2 = "\r";
                }
                else if (s2 == "\\n")
                {
                    s2 = "\n";
                }
                else if (s2 == "\\r\\n")
                {
                    s2 = "\r\n";
                }
            }
            mainform.RStr2 = s2;
            this.Close();
        }

        private void ReplaceString_Shown(object sender, EventArgs e)
        {
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label2.Height / 2D));
        }
    }
}
