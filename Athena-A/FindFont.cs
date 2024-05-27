using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class FindFont : Form
    {
        public FindFont()
        {
            InitializeComponent();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                button1.Enabled = true;
                listBox1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                listBox1.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Text.PrivateFontCollection pfc = new System.Drawing.Text.PrivateFontCollection();
                string s1 = "";
                foreach (string s in openFileDialog1.FileNames)
                {
                    pfc.AddFontFile(s);
                    listBox1.Items.Add(s);
                    s1 = Path.GetFileName(s);
                    mainform.PriFont.Add(s1);
                    mainform.PriFont.Add(s1.Replace(".ttf", "").Replace(".ttc", ""));
                }
                foreach (FontFamily ff in pfc.Families)
                {
                    mainform.PriFont.Add(ff.Name);
                }
                int i1 = mainform.PriFont.Count - 1;
                for (int i = i1; i >= 0; i--)
                {
                    for (int y = i - 1; y >= 0; y--)
                    {
                        if (mainform.PriFont[i].ToString() == mainform.PriFont[y].ToString())
                        {
                            mainform.PriFont.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void LoadSystemFont()
        {
            System.Drawing.Text.InstalledFontCollection ifc = new System.Drawing.Text.InstalledFontCollection();
            foreach (FontFamily ff in ifc.Families)
            {
                if (ff.Name != "")
                {
                    mainform.SysFont.Add(ff.Name);
                }
            }
            string[] sf = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "*.tt*");
            int i1 = sf.Length;
            string s1 = "";
            for (int i = 0; i < i1; i++)
            {
                s1 = Path.GetFileName(sf[i]);
                mainform.SysFont.Add(s1);
                mainform.SysFont.Add(s1.Replace(".ttf", "").Replace(".ttc", ""));
            }
            i1 = mainform.SysFont.Count - 1;
            for (int i = i1; i >= 0; i--)
            {
                for (int y = i - 1; y >= 0; y--)
                {
                    if (mainform.SysFont[i] == mainform.SysFont[y])
                    {
                        mainform.SysFont.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void FindFont_Shown(object sender, EventArgs e)
        {
            LoadSystemFont();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            mainform.SysFont.Clear();
            if (checkBox1.Checked == true)
            {
                LoadSystemFont();
            }
        }

        private void FindFont_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox2.Checked == false)
            {
                mainform.PriFont.Clear();
            }
        }
    }
}
