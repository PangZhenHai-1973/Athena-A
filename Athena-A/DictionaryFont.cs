using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class DictionaryFont : Form
    {
        public static string DictionaryOrgName = "";
        public static string DictionaryOrgSize = "";
        public static string DictionaryTraName = "";
        public static string DictionaryTraSize = "";

        public DictionaryFont()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = new Font(textBox1.Text, float.Parse(textBox2.Text));
            fontDialog1.ShowDialog();
            textBox1.Text = DictionaryOrgName = fontDialog1.Font.Name;
            textBox2.Text = DictionaryOrgSize = fontDialog1.Font.Size.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fontDialog2.Font = new Font(textBox3.Text, float.Parse(textBox4.Text));
            fontDialog2.ShowDialog();
            textBox3.Text = DictionaryTraName = fontDialog2.Font.Name;
            textBox4.Text = DictionaryTraSize = fontDialog2.Font.Size.ToString();
        }

        private void DictionaryFont_Shown(object sender, EventArgs e)
        {
            textBox1.Text = DictionaryOrgName;
            textBox2.Text = DictionaryOrgSize;
            textBox3.Text = DictionaryTraName;
            textBox4.Text = DictionaryTraSize;
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            button2.Location = new Point(button2.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - button2.Height / 2D));
            int MoveY = label1.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label1.Height / 2D);
            label1.Location = new Point(label1.Location.X, label1.Location.Y - MoveY);
            label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
            label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
            label4.Location = new Point(label4.Location.X, label4.Location.Y - MoveY);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = DictionaryOrgName = this.Font.Name;
            textBox2.Text = DictionaryOrgSize = this.Font.Size.ToString();
            textBox3.Text = DictionaryTraName = this.Font.Name;
            textBox4.Text = DictionaryTraSize = this.Font.Size.ToString();
        }
    }
}
