using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class CharTool : Form
    {
        bool bl = false;
        Encoding ed = CodePagesEncodingProvider.Instance.GetEncoding(0);

        public CharTool()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox2.WordWrap = true;
            }
            else
            {
                textBox2.WordWrap = false;
            }
        }

        private void CharTool_Shown(object sender, EventArgs e)
        {
            label3.Text = "";
            comboBox1.Text = "拼音";
            comboBox2.Text = "简体中文(936)";
            if (mainform.MyDpi > 96F)
            {
                label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
                int Y1 = comboBox1.Location.Y + (int)(comboBox1.Height / 2D - label2.Height / 2D);
                label2.Location = new Point(label2.Location.X, Y1);
                label3.Location = new Point(label3.Location.X, Y1);
                label4.Location = new Point(label4.Location.X, Y1);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请指定要处理的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(textBox1.Text) == false)
            {
                MessageBox.Show("指定的文件不存在。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                panel1.Enabled = false;
                textBox2.Clear();
                if (comboBox2.Text == "简体中文(936)")
                {
                    ed = Encoding.GetEncoding(936);
                }
                else if (comboBox2.Text == "繁体中文(950)")
                {
                    ed = Encoding.GetEncoding(950);
                }
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "TXT 文件(*.TXT)|*.TXT";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = open.FileName;
                textBox2.Clear();
                label3.Text = "";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (bl == true)
            {
                MessageBox.Show("处理完成。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Value = 0;
            panel1.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s = textBox1.Text;
            string s1 = "";
            string s2 = "";
            bl = false;
            ArrayList al = new ArrayList();
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            if (fs.Length > 2)
            {
                BinaryReader br = new BinaryReader(fs);
                byte b = br.ReadByte();
                if (b == 255)
                {
                    if (br.ReadByte() == 254)
                    {
                        ed = Encoding.Unicode;
                    }
                }
                else if (b == 254)
                {
                    if (br.ReadByte() == 255)
                    {
                        ed = Encoding.BigEndianUnicode;
                    }
                }
                else if (b == 239)
                {
                    if (br.ReadByte() == 187)
                    {
                        if (br.ReadByte() == 191)
                        {
                            ed = Encoding.UTF8;
                        }
                    }
                }
                fs.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(fs, ed);
                progressBar1.Maximum = (int)fs.Length;
                while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                {
                    progressBar1.Value = progressBar1.Value + ed.GetBytes(s1).Length;
                    for (int i = 0; i < s1.Length; i++)
                    {
                        s2 = s1.Substring(i, 1);
                        if (al.Contains(s2) == false)
                        {
                            al.Add(s2);
                        }
                    }
                }
                sr.Close();
                br.Close();
            }
            fs.Close();
            if (al.Count > 0)
            {
                label3.Text = al.Count.ToString();
                StringBuilder sb = new StringBuilder();
                if (comboBox1.Text == "拼音")
                {
                    al.Sort();
                    for (int i = 0; i < al.Count; i++)
                    {
                        sb.Append(al[i].ToString());
                    }
                }
                else
                {
                    if (checkBox2.Checked == true)
                    {
                        if (comboBox2.Text == "简体中文(936)")
                        {
                            object[] ob = new object[al.Count];
                            byte[] bt1 = new byte[2];
                            for (int i = 0; i < al.Count; i++)
                            {
                                bt1 = Encoding.GetEncoding(936).GetBytes(al[i].ToString());
                                if (bt1.Length == 1)
                                {
                                    ob[i] = System.Int64.Parse(bt1[0].ToString("X2"), System.Globalization.NumberStyles.AllowHexSpecifier);
                                }
                                else
                                {
                                    ob[i] = System.Int64.Parse(bt1[1].ToString("X2") + bt1[0].ToString("X2"), System.Globalization.NumberStyles.AllowHexSpecifier);
                                }
                            }
                            Array.Sort(ob);
                            byte[] bt2 = new byte[2];
                            for (int i = 0; i < ob.Length; i++)
                            {
                                s = int.Parse(ob[i].ToString()).ToString("X4");
                                s1 = s.Substring(0, 2);
                                s2 = s.Substring(2, 2);
                                bt2[0] = (byte)System.Int64.Parse(s2, System.Globalization.NumberStyles.AllowHexSpecifier);
                                bt2[1] = (byte)System.Int64.Parse(s1, System.Globalization.NumberStyles.AllowHexSpecifier);
                                if (s1 == "00")
                                {
                                    sb.Append(Encoding.GetEncoding(936).GetString(bt2, 0, 1));
                                }
                                else
                                {
                                    sb.Append(Encoding.GetEncoding(936).GetString(bt2, 0, 2));
                                }
                            }
                        }
                        else
                        {
                            object[] ob = new object[al.Count];
                            byte[] bt1 = new byte[2];
                            for (int i = 0; i < al.Count; i++)
                            {
                                bt1 = Encoding.GetEncoding(950).GetBytes(al[i].ToString());
                                if (bt1.Length == 1)
                                {
                                    ob[i] = System.Int64.Parse(bt1[0].ToString("X2"), System.Globalization.NumberStyles.AllowHexSpecifier);
                                }
                                else
                                {
                                    ob[i] = System.Int64.Parse(bt1[1].ToString("X2") + bt1[0].ToString("X2"), System.Globalization.NumberStyles.AllowHexSpecifier);
                                }
                            }
                            Array.Sort(ob);
                            byte[] bt2 = new byte[2];
                            for (int i = 0; i < ob.Length; i++)
                            {
                                s = int.Parse(ob[i].ToString()).ToString("X4");
                                s1 = s.Substring(0, 2);
                                s2 = s.Substring(2, 2);
                                bt2[0] = (byte)System.Int64.Parse(s2, System.Globalization.NumberStyles.AllowHexSpecifier);
                                bt2[1] = (byte)System.Int64.Parse(s1, System.Globalization.NumberStyles.AllowHexSpecifier);
                                if (s1 == "00")
                                {
                                    sb.Append(Encoding.GetEncoding(950).GetString(bt2, 0, 1));
                                }
                                else
                                {
                                    sb.Append(Encoding.GetEncoding(950).GetString(bt2, 0, 2));
                                }
                            }
                        }
                    }
                    else
                    {
                        object[] ob = new object[al.Count];
                        byte[] bt = new byte[2];
                        for (int i = 0; i < al.Count; i++)
                        {
                            bt = Encoding.Unicode.GetBytes(al[i].ToString());
                            ob[i] = System.Int64.Parse(bt[1].ToString("X2") + bt[0].ToString("X2"), System.Globalization.NumberStyles.AllowHexSpecifier);
                        }
                        Array.Sort(ob);
                        for (int i = 0; i < ob.Length; i++)
                        {
                            s = int.Parse(ob[i].ToString()).ToString("X4");
                            s1 = s.Substring(0, 2);
                            s2 = s.Substring(2, 2);
                            bt[0] = (byte)System.Int64.Parse(s2, System.Globalization.NumberStyles.AllowHexSpecifier);
                            bt[1] = (byte)System.Int64.Parse(s1, System.Globalization.NumberStyles.AllowHexSpecifier);
                            sb.Append(Encoding.Unicode.GetString(bt));
                        }
                    }
                }
                al.Clear();
                textBox2.Text = sb.ToString();
            }
            else
            {
                label3.Text = "0";
            }
            progressBar1.Value = progressBar1.Maximum;
            bl = true;
        }

        private void CharTool_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            textBox1.Text = s;
            textBox2.Clear();
            label3.Text = "";
        }

        private void CharTool_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    if (Path.GetExtension(s1).ToLower() == ".txt")
                    {
                        Clipboard.SetDataObject(filenames[0]);
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                comboBox2.Enabled = true;
            }
            else
            {
                comboBox2.Enabled = false;
            }
        }
    }
}
