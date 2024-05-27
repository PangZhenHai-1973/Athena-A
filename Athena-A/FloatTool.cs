using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class FloatTool : Form
    {
        public FloatTool()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (s1 != "")
            {
                string s2 = "";
                try
                {
                    float f = float.Parse(s1);
                    byte[] by = new byte[4];
                    by = BitConverter.GetBytes(f);
                    for (int i = 0; i < 4; i++)
                    {
                        s2 = s2 + by[i].ToString("X2");
                    }
                    textBox2.Text = s2;
                }
                catch
                {
                    textBox2.Text = "";
                }
                s2 = "";
                try
                {
                    double d = double.Parse(s1);
                    byte[] by = new byte[8];
                    by = BitConverter.GetBytes(d);
                    for (int i = 0; i < 8; i++)
                    {
                        s2 = s2 + by[i].ToString("X2");
                    }
                    textBox3.Text = s2;
                }
                catch
                {
                    textBox3.Text = "";
                }
            }
            else
            {
                textBox2.Text = "";
                textBox3.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = textBox2.Text;
            if (s != "")
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(s);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = textBox3.Text;
            if (s != "")
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(s);
            }
        }

        private void FloatTool_Shown(object sender, EventArgs e)
        {
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - button1.Height / 2D));
            button2.Location = new Point(button2.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - button2.Height / 2D));
        }
    }
}
