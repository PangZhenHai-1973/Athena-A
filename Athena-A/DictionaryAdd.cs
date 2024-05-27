using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class DictionaryAdd : Form
    {
        public DictionaryAdd()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            if (s1 != "" && s2 == "")
            {
                MessageBox.Show("请输入译文。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (s2 == "")
            {
                DictionaryEdit.RStr1 = "";
                DictionaryEdit.RStr2 = "";
            }
            else
            {
                DictionaryEdit.RStr1 = s1;
                DictionaryEdit.RStr2 = s2;
            }
            this.Close();
        }

        private void DictionaryAdd_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                label2.Location = new Point(label2.Location.X, button1.Location.Y + (int)(button1.Height / 2D - label2.Height / 2D));
                button1.Location = new Point(textBox1.Width + textBox1.Location.X - button1.Width, button1.Location.Y);
            }
        }
    }
}
