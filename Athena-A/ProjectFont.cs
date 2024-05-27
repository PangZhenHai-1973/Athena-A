using System;
using System.Drawing;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class ProjectFont : Form
    {
        public static string ProjectOrgName = "";
        public static string ProjectOrgSize = "";
        public static string ProjectTraName = "";
        public static string ProjectTraSize = "";

        public ProjectFont()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = ProjectOrgName = this.Font.Name;
            textBox2.Text = ProjectOrgSize = this.Font.Size.ToString();
            textBox3.Text = ProjectTraName = this.Font.Name;
            textBox4.Text = ProjectTraSize = this.Font.Size.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = new Font(textBox1.Text, float.Parse(textBox2.Text));
            fontDialog1.ShowDialog();
            textBox1.Text = ProjectOrgName = fontDialog1.Font.Name;
            textBox2.Text = ProjectOrgSize = fontDialog1.Font.Size.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fontDialog2.Font = new Font(textBox3.Text, float.Parse(textBox4.Text));
            fontDialog2.ShowDialog();
            textBox3.Text = ProjectTraName = fontDialog2.Font.Name;
            textBox4.Text = ProjectTraSize = fontDialog2.Font.Size.ToString();
        }

        private void ProjectFont_Shown(object sender, EventArgs e)
        {
            textBox1.Text = ProjectOrgName;
            textBox2.Text = ProjectOrgSize;
            textBox3.Text = ProjectTraName;
            textBox4.Text = ProjectTraSize;
            button3.Location = new Point(button3.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button3.Height / 2D));
            button4.Location = new Point(button4.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - button4.Height / 2D));
            int MoveY = label1.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label1.Height / 2D);
            label1.Location = new Point(label1.Location.X, label1.Location.Y - MoveY);
            label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
            label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
            label4.Location = new Point(label4.Location.X, label4.Location.Y - MoveY);
        }
    }
}
