using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class BatchText : Form
    {
        ArrayList AL = new ArrayList();

        public BatchText()
        {
            InitializeComponent();
        }

        private void BatchText_Shown(object sender, EventArgs e)
        {
            comboBox1.Text = "文本文件(*.txt)";
            comboBox2.Text = "ANSI";
            comboBox3.Text = "简体中文(936)";
            comboBox4.Text = "简体中文(936)";
            if (mainform.MyDpi > 96F)
            {
                int Y1 = comboBox1.Location.Y + (int)(comboBox1.Height / 2D - label1.Height / 2D);
                label1.Location = new Point(label1.Location.X, Y1);
                label4.Location = new Point(label4.Location.X, Y1);
                int Y2 = comboBox2.Location.Y + (int)(comboBox2.Height / 2D - label2.Height / 2D);
                label2.Location = new Point(label2.Location.X, Y2);
                label3.Location = new Point(label3.Location.X, Y2);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "ANSI")
            {
                comboBox3.Enabled = true;
            }
            else
            {
                comboBox3.Enabled = false;
            }
        }

        private void SearchFileTextSub(string s)
        {
            DirectoryInfo di = new DirectoryInfo(s);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach (FileSystemInfo i in fsi)
            {
                if (i is DirectoryInfo)
                {
                    try
                    {
                        SearchFileTextSub(i.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    if (i.Extension.ToLower() == ".txt")
                    {
                        AL.Add(i.FullName);
                    }
                }
            }
        }

        private void SearchFileText(string s)
        {
            string[] f = Directory.GetFiles(s);
            for (int i = 0; i < f.Length; i++)
            {
                if (Path.GetExtension(f[i]).ToLower() == ".txt")
                {
                    AL.Add(f[i]);
                }
            }
        }

        private void SearchFileHtmlSub(string s)
        {
            DirectoryInfo di = new DirectoryInfo(s);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach (FileSystemInfo i in fsi)
            {
                if (i is DirectoryInfo)
                {
                    try
                    {
                        SearchFileHtmlSub(i.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    if (i.Extension.ToLower() == ".html" || i.Extension.ToLower() == ".htm")
                    {
                        AL.Add(i.FullName);
                    }
                }
            }
        }

        private void SearchFileHtml(string s)
        {
            string[] f = Directory.GetFiles(s);
            for (int i = 0; i < f.Length; i++)
            {
                s = Path.GetExtension(f[i]).ToLower();
                if (s == ".html" || s == ".htm")
                {
                    AL.Add(f[i]);
                }
            }
        }

        private void SearchFileXmlSub(string s)
        {
            DirectoryInfo di = new DirectoryInfo(s);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach (FileSystemInfo i in fsi)
            {
                if (i is DirectoryInfo)
                {
                    try
                    {
                        SearchFileXmlSub(i.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    if (i.Extension.ToLower() == ".xml")
                    {
                        AL.Add(i.FullName);
                    }
                }
            }
        }

        private void SearchFileXml(string s)
        {
            string[] f = Directory.GetFiles(s);
            for (int i = 0; i < f.Length; i++)
            {
                if (Path.GetExtension(f[i]).ToLower() == ".xml")
                {
                    AL.Add(f[i]);
                }
            }
        }

        private void DisabledControl()
        {
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            checkBox1.Enabled = false;
        }

        private void EnabledControl()
        {
            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            if (comboBox2.Text == "ANSI")
            {
                comboBox3.Enabled = true;
            }
            comboBox4.Enabled = true;
            checkBox1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (Directory.Exists(s1) == false)
            {
                MessageBox.Show("请指定需要转换的搜索目录或指定的目录不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                AL.Clear();
                DisabledControl();
                string s2 = comboBox1.Text;
                if (s2 == "文本文件(*.txt)")
                {
                    if (checkBox1.Checked == true)
                    {
                        SearchFileTextSub(s1);
                    }
                    else
                    {
                        SearchFileText(s1);
                    }
                }
                else if (s2 == "网页文件(*.htm,*.html)")
                {
                    if (checkBox1.Checked == true)
                    {
                        SearchFileHtmlSub(s1);
                    }
                    else
                    {
                        SearchFileHtml(s1);
                    }
                }
                else if (s2 == "XML 文件(*.xml)")
                {
                    if (checkBox1.Checked == true)
                    {
                        SearchFileXmlSub(s1);
                    }
                    else
                    {
                        SearchFileXml(s1);
                    }
                }
                if (AL.Count == 0)
                {
                    MessageBox.Show("没有搜索到任何需要转码的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EnabledControl();
                }
                else
                {
                    progressBar1.Maximum = AL.Count - 1;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        void ConvertCode(string s, Encoding OriginalED, Encoding TargetED)
        {
            FileInfo f = new FileInfo(s);
            bool bl = true;
            if (f.Attributes.ToString().ToLower().Contains("readonly") == true)
            {
                try
                {
                    f.Attributes = FileAttributes.Normal;
                }
                catch
                {
                    bl = false;
                }
            }
            if (bl == true)
            {
                try
                {
                    FileStream fsr = new FileStream(s, FileMode.Open, FileAccess.ReadWrite);
                    if (fsr.Length > 2)
                    {
                        BinaryReader br = new BinaryReader(fsr);
                        byte b = br.ReadByte();
                        if (b == 255)
                        {
                            if (br.ReadByte() == 254)
                            {
                                OriginalED = Encoding.Unicode;
                            }
                        }
                        else if (b == 254)
                        {
                            if (br.ReadByte() == 255)
                            {
                                OriginalED = Encoding.BigEndianUnicode;
                            }
                        }
                        else if (b == 239)
                        {
                            if (br.ReadByte() == 187)
                            {
                                if (br.ReadByte() == 191)
                                {
                                    OriginalED = Encoding.UTF8;
                                }
                            }
                        }
                        if (OriginalED.CodePage != TargetED.CodePage)
                        {
                            fsr.Seek(0, SeekOrigin.Begin);
                            StreamReader sr = new StreamReader(fsr, OriginalED);
                            string s1 = "";
                            ArrayList altmp = new ArrayList();
                            while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                            {
                                altmp.Add(s1);
                            }
                            fsr.SetLength(0);
                            StreamWriter sw = new StreamWriter(fsr, TargetED);
                            int i1 = altmp.Count;
                            for (int i = 0; i < i1; i++)
                            {
                                sw.WriteLine(altmp[i].ToString());
                            }
                            altmp.Clear();
                            sw.Close();
                            sr.Close();
                        }
                        br.Close();
                        fsr.Close();
                    }
                    fsr.Close();
                }
                catch
                { }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string s1 = comboBox2.Text;
            Encoding TargetED = Encoding.Unicode;
            if (s1 == "ANSI")
            {
                s1 = comboBox3.Text;
                if (s1 == "简体中文(936)")
                {
                    TargetED = Encoding.GetEncoding(936);
                }
                else if (s1 == "繁体中文(950)")
                {
                    TargetED = Encoding.GetEncoding(950);
                }
                else if (s1 == "日文(932)")
                {
                    TargetED = Encoding.GetEncoding(932);
                }
                else if (s1 == "韩文(949)")
                {
                    TargetED = Encoding.GetEncoding(949);
                }
            }
            else if (s1 == "Unicode big endian")
            {
                TargetED = Encoding.BigEndianUnicode;
            }
            else if (s1 == "UTF-8")
            {
                TargetED = Encoding.UTF8;
            }
            s1 = comboBox4.Text;
            Encoding OriginalED = Encoding.GetEncoding(936);
            if (s1 == "繁体中文(950)")
            {
                OriginalED = Encoding.GetEncoding(950);
            }
            else if (s1 == "日文(932)")
            {
                OriginalED = Encoding.GetEncoding(932);
            }
            else if (s1 == "韩文(949)")
            {
                OriginalED = Encoding.GetEncoding(949);
            }
            for (int i = 0; i < AL.Count; i++)
            {
                progressBar1.Value = i;
                ConvertCode(AL[i].ToString(), OriginalED, TargetED);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("转码完成。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            progressBar1.Value = 0;
            EnabledControl();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BatchText_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }
    }
}
