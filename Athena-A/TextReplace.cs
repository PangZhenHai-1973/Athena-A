using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

namespace Athena_A
{
    public partial class TextReplace : Form
    {
        ArrayList al1 = new ArrayList();
        Encoding ed;

        public TextReplace()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("以下面这个循环为例：");
            sb.Append("\r\n\r\n" + "<String Key=\"Artist\">" + "\r\n" + "      <de>Interpret</de>" + "\r\n" + "      <it>Artista</it>");
            sb.Append("\r\n" + "</String>" + "\r\n\r\n");
            sb.Append("循环前标识：<String" + "\r\n" + "循环后标识：</String>" + "\r\n");
            sb.Append("源前标识：<String Key=\"" + "\r\n" + "源后标识：\">" + "\r\n");
            sb.Append("目标前标识：<it>" + "\r\n" + "目标后标识：</it>" + "\r\n\r\n");
            sb.Append("替换后的结果：" + "\r\n\r\n");
            sb.Append("<String Key=\"Artist\">" + "\r\n" + "      <de>Interpret</de>" + "\r\n" + "      <it>Artist</it>");
            sb.Append("\r\n" + "</String>");
            sb.Append("\r\n\r\n" + "注意的是文本只能由上至下进行替换。");
            MessageBox.Show(sb.ToString(), "说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = textBox3.Text;
            string s2 = textBox4.Text;
            string s3 = textBox5.Text;
            string s4 = textBox6.Text;
            string s5 = textBox7.Text;
            string s6 = textBox8.Text;
            string s7 = textBox1.Text;
            if (s7 == "")
            {
                MessageBox.Show("请指定需要替换的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (s1 == "")
            {
                MessageBox.Show("请指定循环前标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s2 == "")
            {
                MessageBox.Show("请指定循环后标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s3 == "")
            {
                MessageBox.Show("请指定源前标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s4 == "")
            {
                MessageBox.Show("请指定源后标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s5 == "")
            {
                MessageBox.Show("请指定目标前标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s6 == "")
            {
                MessageBox.Show("请指定目标后标识。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (al1.Count > 0)
                {
                    textBox2.Clear();
                    textBox2.Enabled = false;
                    textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                    panel1.Enabled = false;
                    int i1 = 0;
                    int i2 = 0;
                    int i3 = 0;
                    string org = "";
                    string tar = "";
                    string stem = "";
                    i1 = al1.Count;
                    ArrayList al2 = new ArrayList();
                    al2.Add("以下内容没有进行替换：");
                    for (int i = 0; i < i1; i++)
                    {
                        if (al1[i].ToString().Contains(s1) == true)
                        {
                            while (al1[i].ToString().Contains(s3) == false && al1[i].ToString().Contains(s4) == false)
                            {
                                if (i == i1 || al1[i].ToString().Contains(s2) == true)
                                {
                                    break;
                                }
                                else
                                {
                                    i++;
                                }
                            }
                            if (i < i1 && al1[i].ToString().Contains(s3) == true && al1[i].ToString().Contains(s4) == true)
                            {
                                org = al1[i].ToString();
                                i2 = org.IndexOf(s3) + s3.Length;
                                org = org.Substring(i2, org.LastIndexOf(s4) - i2);
                                i3 = i + 1;
                                while (al1[i].ToString().Contains(s5) == false && al1[i].ToString().Contains(s6) == false)
                                {
                                    if (i == i1 || al1[i].ToString().Contains(s2) == true)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        i++;
                                    }
                                }
                                if (i < i1 && al1[i].ToString().Contains(s5) == true && al1[i].ToString().Contains(s6) == true)
                                {
                                    tar = al1[i].ToString();
                                    i2 = tar.IndexOf(s5) + s5.Length;
                                    tar = tar.Substring(i2, tar.LastIndexOf(s6) - i2);
                                    al1[i] = al1[i].ToString().Replace(tar, org);
                                }
                                else
                                {
                                    al2.Add("第 " + i3.ToString() + " 行 - " + org);
                                }
                            }
                        }
                    }
                    StringBuilder sb = new StringBuilder();
                    StreamWriter sw = new StreamWriter(s7, false, ed);
                    i1 = i1 - 1;
                    for (int i = 0; i < i1; i++)
                    {
                        stem = al1[i].ToString();
                        sb.Append(stem + "\r\n");
                        sw.WriteLine(stem);
                    }
                    stem = al1[i1].ToString();
                    sb.Append(stem);
                    sw.WriteLine(stem);
                    sw.Close();
                    textBox2.Text = sb.ToString();
                    textBox2.Enabled = true;
                    panel1.Enabled = true;
                    textBox1.BackColor = System.Drawing.Color.White;
                    MessageBox.Show("替换完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    i1 = al2.Count;
                    if (i1 > 1)
                    {
                        StreamWriter swlog = new StreamWriter(s7 + ".log");
                        for (int x = 0; x < i1; x++)
                        {
                            swlog.WriteLine(al2[x].ToString());
                        }
                        swlog.Close();
                        MessageBox.Show("替换过程中有些内容没有进行替换，请查看同目录下的日志文件。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("没有内容可进行替换。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "TXT 文件(*.TXT)|*.TXT|XML 文件(*.XML)|*.XML";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox2.Clear();
                string s = open.FileName;
                textBox1.Text = s;
                al1.Clear();
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox2.Enabled = false;
                panel1.Enabled = false;
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
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
                Task.Factory.StartNew(() => ReplaceTask(s));
            }
        }

        private void ReplaceTask(string s)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
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
                br.Close();
            }
            fs.Close();
            panel1.Enabled = true;
            textBox2.Enabled = true;
            textBox1.BackColor = System.Drawing.Color.White;
        }

        private void TextReplace_DragEnter(object sender, DragEventArgs e)
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

        private void TextReplace_DragDrop(object sender, DragEventArgs e)
        {
            textBox2.Clear();
            string s = (string)Clipboard.GetData(DataFormats.Text);
            textBox1.Text = s;
            al1.Clear();
            textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            textBox2.Enabled = false;
            panel1.Enabled = false;
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
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
            Task.Factory.StartNew(() => ReplaceTask(s));
        }

        private void TextReplace_Shown(object sender, EventArgs e)
        {
            comboBox1.Text = "简体中文(936)";
            if (mainform.MyDpi > 96F)
            {
                int LY = label1.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label1.Height / 2D);
                label1.Location = new Point(label1.Location.X, label1.Location.Y - LY);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - LY);
                label3.Location = new Point(label3.Location.X, label3.Location.Y - LY);
                label4.Location = new Point(label4.Location.X, label4.Location.Y - LY);
                label5.Location = new Point(label5.Location.X, label5.Location.Y - LY);
                label6.Location = new Point(label6.Location.X, label6.Location.Y - LY);
                label7.Location = new Point(label7.Location.X, label7.Location.Y - LY);
                label8.Location = new Point(label8.Location.X, label8.Location.Y - LY);
            }
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
        }
    }
}
