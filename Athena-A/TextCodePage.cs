using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;

namespace Athena_A
{
    public partial class TextCodePage : Form
    {
        ArrayList al1 = new ArrayList();
        Encoding ed;

        public TextCodePage()
        {
            InitializeComponent();
        }

        private void TextCodePage_Shown(object sender, EventArgs e)
        {
            comboBox1.Text = "简体中文(936)";
            label1.Location = new Point(label1.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, comboBox1.Location.Y + (int)(comboBox1.Height / 2D - label2.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - button1.Height / 2D));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "TXT 文件(*.TXT)|*.TXT";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox1.Clear();
                string s = open.FileName;
                textBox2.Text = s;
                al1.Clear();
                textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                textBox1.Enabled = false;
                panel1.Enabled = false;
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
                Task.Factory.StartNew(() => CodePageTask(s));
            }
        }

        private void CodePageTask(string s)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
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
                                textBox1.Text = sb.ToString();
                            }
                        }
                    }
                }
            }
            panel1.Enabled = true;
            textBox1.Enabled = true;
            textBox1.BackColor = System.Drawing.Color.White;
        }

        private void TextCodePage_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Clear();
            string s = (string)Clipboard.GetData(DataFormats.Text);
            textBox2.Text = s;
            al1.Clear();
            textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            textBox1.Enabled = false;
            panel1.Enabled = false;
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
            Task.Factory.StartNew(() => CodePageTask(s));
        }

        private void TextCodePage_DragEnter(object sender, DragEventArgs e)
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = textBox2.Text;
            if (s != "")
            {
                if (File.Exists(s) == false)
                {
                    MessageBox.Show("文件不存在。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    al1.Clear();
                    textBox1.BackColor = System.Drawing.Color.WhiteSmoke;
                    textBox1.Enabled = false;
                    panel1.Enabled = false;
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
                    Task.Factory.StartNew(() => CodePageTask(s));
                }
            }
        }
    }
}
