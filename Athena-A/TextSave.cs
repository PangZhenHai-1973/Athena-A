using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;

namespace Athena_A
{
    public partial class TextSave : Form
    {
        ArrayList al1 = new ArrayList();
        Encoding ed;

        public TextSave()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "TXT 文件(*.TXT)|*.TXT|XML 文件(*.XML)|*.XML";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    string s = open.FileName;
                    textBox1.Text = s;
                    ANSI_Encoding();
                    Task.Factory.StartNew(() => SaveTask(s));
                }
            }
        }

        private void SaveTask(string s)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string cb3 = comboBox3.Text;
            textBox2.Clear();
            al1.Clear();
            textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            textBox2.Enabled = false;
            panel1.Enabled = false;
            using (FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length > 2)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte b = br.ReadByte();
                        if (b == 255)
                        {
                            if (br.ReadByte() == 254)
                            {
                                ed = Encoding.Unicode;
                                comboBox3.Text = "UTF-16 LE";
                            }
                        }
                        else if (b == 254)
                        {
                            if (br.ReadByte() == 255)
                            {
                                ed = Encoding.BigEndianUnicode;
                                comboBox3.Text = "UTF-16 BE";
                            }
                        }
                        else if (b == 239)
                        {
                            if (br.ReadByte() == 187)
                            {
                                if (br.ReadByte() == 191)
                                {
                                    ed = Encoding.UTF8;
                                    comboBox3.Text = "带有 BOM 的 UTF-8";
                                }
                            }
                        }
                        else if (cb3 == "UTF-8" || cb3 == "带有 BOM 的 UTF-8")
                        {
                            ed = Encoding.UTF8;
                        }
                        fs.Seek(0, SeekOrigin.Begin);
                        using (StreamReader sr = new StreamReader(fs, ed))
                        {
                            StringBuilder sb = new StringBuilder();
                            while ((s = sr.ReadLine()) != null)//判定是否是最后一行
                            {
                                al1.Add(s);
                            }
                            int i1 = al1.Count - 1;
                            if (i1 >= 0)
                            {
                                for (int i = 0; i < i1; i++)
                                {
                                    sb.Append(al1[i].ToString() + "\r\n");
                                }
                                sb.Append(al1[i1].ToString());
                                textBox2.Text = sb.ToString();
                            }
                            sr.Close();
                        }
                        br.Close();
                    }
                }
                fs.Close();
            }
            panel1.Enabled = true;
            textBox2.Enabled = true;
            textBox1.BackColor = System.Drawing.Color.White;
        }

        private void ANSI_Encoding()
        {
            if (comboBox1.Text == "简体中文(936)")
            {
                ed = Encoding.GetEncoding(936);
            }
            else if (comboBox1.Text == "繁体中文(950)")
            {
                ed = Encoding.GetEncoding(950);
            }
            else if (comboBox1.Text == "日文(932)")
            {
                ed = Encoding.GetEncoding(932);
            }
            else if (comboBox1.Text == "韩文(949)")
            {
                ed = Encoding.GetEncoding(949);
            }
            else if (comboBox1.Text == "默认")
            {
                ed = mainform.AA_Default_Encoding;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (al1.Count > 0)
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("请指定要处理的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (textBox3.Text == "" && textBox4.Text == "")
                {
                    MessageBox.Show("请指定保留内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (radioButton2.Checked && textBox3.Text == "")
                {
                    MessageBox.Show("请指定保留内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    textBox2.Clear();
                    textBox2.Enabled = false;
                    textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                    panel1.Enabled = false;
                    BackgroundWorker moveStrBackgroundWorker = new BackgroundWorker();
                    moveStrBackgroundWorker.DoWork += MoveStrBackgroundWorker_DoWork;
                    moveStrBackgroundWorker.RunWorkerCompleted += MoveStrBackgroundWorker_RunWorkerCompleted;
                    moveStrBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void MoveStrBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = textBox1.Text;
            string s3 = textBox3.Text;
            string s4 = textBox4.Text;
            string stem = "";
            if (radioButton2.Checked)
            {
                for (int i = al1.Count - 1; i >= 0; i--)
                {
                    stem = al1[i].ToString();
                    if (stem != s3)
                    {
                        al1.RemoveAt(i);
                    }
                }
            }
            else
            {
                if (s3 == "")
                {
                    for (int i = al1.Count - 1; i >= 0; i--)
                    {
                        stem = al1[i].ToString();
                        if (stem.Contains(s4) == false)
                        {
                            al1.RemoveAt(i);
                        }
                    }
                }
                else if (s4 == "")
                {
                    for (int i = al1.Count - 1; i >= 0; i--)
                    {
                        stem = al1[i].ToString();
                        if (stem.Contains(s3) == false)
                        {
                            al1.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    if (comboBox2.Text == "OR")
                    {
                        for (int i = al1.Count - 1; i >= 0; i--)
                        {
                            stem = al1[i].ToString();
                            if (stem.Contains(s3) == false && stem.Contains(s4) == false)
                            {
                                al1.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        for (int i = al1.Count - 1; i >= 0; i--)
                        {
                            stem = al1[i].ToString();
                            if (!(stem.Contains(s3) && stem.Contains(s4)))
                            {
                                al1.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = new FileStream(s1, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    string cb3 = comboBox3.Text;
                    if (cb3 == "ANSI")
                    {
                        ANSI_Encoding();
                    }
                    else if (cb3 == "UTF-16 LE")
                    {
                        ed = Encoding.Unicode;
                        bw.Write((byte)255);
                        bw.Write((byte)254);
                    }
                    else if (cb3 == "UTF-16 BE")
                    {
                        ed = Encoding.BigEndianUnicode;
                        bw.Write((byte)254);
                        bw.Write((byte)255);
                    }
                    else if (cb3 == "UTF-8")
                    {
                        ed = Encoding.UTF8;
                    }
                    else if (cb3 == "带有 BOM 的 UTF-8")
                    {
                        ed = Encoding.UTF8;
                        bw.Write((byte)239);
                        bw.Write((byte)187);
                        bw.Write((byte)191);
                    }
                    int i1 = al1.Count - 1;
                    if (i1 >= 0)
                    {
                        for (int i = 0; i < i1; i++)
                        {
                            stem = al1[i].ToString();
                            sb.Append(stem + "\r\n");
                            bw.Write(ed.GetBytes(stem + "\r\n"));
                        }
                        stem = al1[i1].ToString();
                        sb.Append(stem);
                        bw.Write(ed.GetBytes(stem));
                    }
                }
            }
            textBox2.Text = sb.ToString();
        }

        private void MoveStrBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox2.Enabled = true;
            panel1.Enabled = true;
            textBox1.BackColor = System.Drawing.Color.White;
            MessageBox.Show("处理完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            textBox2.DeselectAll();
        }

        private void TextSave_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            textBox1.Text = s;
            ANSI_Encoding();
            Task.Factory.StartNew(() => SaveTask(s));
        }

        private void TextSave_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    if (Path.GetExtension(s1).ToLower() == ".txt" || Path.GetExtension(s1).ToLower() == ".xml")
                    {
                        Clipboard.SetDataObject(filenames[0]);
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void TextSave_Shown(object sender, EventArgs e)
        {
            comboBox3.Text = "ANSI";
            comboBox2.Text = "OR";
            comboBox1.Text = "简体中文(936)";
            textBox1.Location = new Point(textBox1.Location.X, button1.Location.Y + (int)(button1.Height / 2D - textBox1.Height / 2D));
            label1.Location = new Point(label1.Location.X, button1.Location.Y + (int)(button1.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, comboBox3.Location.Y + (int)(comboBox3.Height / 2D - label2.Height / 2D));
            label3.Location = new Point(label3.Location.X, comboBox3.Location.Y + (int)(comboBox3.Height / 2D - label3.Height / 2D));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (File.Exists(s))
            {
                if (comboBox3.Text == "ANSI")
                {
                    ANSI_Encoding();
                }
                else if (comboBox3.Text == "UTF-16 LE")
                {
                    ed = Encoding.Unicode;
                }
                else if (comboBox3.Text == "UTF-16 BE")
                {
                    ed = Encoding.BigEndianUnicode;
                }
                else if (comboBox3.Text == "UTF-8" || comboBox3.Text == "带有 BOM 的 UTF-8")
                {
                    ed = Encoding.UTF8;
                }
                textBox2.Clear();
                al1.Clear();
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox2.Enabled = false;
                panel1.Enabled = false;
                Task.Factory.StartNew(() => UpdateTask(s));
            }
        }

        private void UpdateTask(string s)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs, ed))
                {
                    StringBuilder sb = new StringBuilder();
                    while ((s = sr.ReadLine()) != null)//判定是否是最后一行
                    {
                        al1.Add(s);
                    }
                    int i1 = al1.Count - 1;
                    if (i1 >= 0)
                    {
                        for (int i = 0; i < i1; i++)
                        {
                            sb.Append(al1[i].ToString() + "\r\n");
                        }
                        sb.Append(al1[i1].ToString());
                        textBox2.Text = sb.ToString();
                    }
                }
            }
            panel1.Enabled = true;
            textBox2.Enabled = true;
            textBox1.BackColor = System.Drawing.Color.White;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "ANSI")
            {
                comboBox1.Enabled = true;
            }
            else
            {
                comboBox1.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                comboBox2.Enabled = false;
                textBox4.Enabled = false;
            }
            else
            {
                comboBox2.Enabled = true;
                textBox4.Enabled = true;
            }
        }
    }
}
