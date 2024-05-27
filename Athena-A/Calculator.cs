using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class Calculator : Form
    {

        private ArrayList list1 = new ArrayList();
        private ArrayList list2 = new ArrayList();
        private ArrayList list3 = new ArrayList();
        private ArrayList list4 = new ArrayList();
        private long lg = 0;
        private int cpu = 0;

        public Calculator()
        {
            InitializeComponent();
        }

        private void ComputeV()//计算偏移地址的虚拟地址
        {
            string s = textBox2.Text;
            long l = CommonCode.HexToLong(s);
            if (l > lg)
            {
                MessageBox.Show("指定的偏移地址已超过文件的大小，无法进行计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int i = 0;
                long l1 = long.Parse(list1[0].ToString());//获得基址
                long l2 = long.Parse(list4[3].ToString());
                if (l < l2)
                {
                    l2 = l1 + l;
                    textBox3.Text = CommonCode.FormatStr(l2.ToString("X16"));
                }
                else
                {
                    long l3 = 0;
                    long l4 = 0;
                    int i5 = list1.Count;//文件头信息总行数
                    for (i = 3; i < i5; i++)//段内调用
                    {
                        l3 = long.Parse(list4[i].ToString());
                        l4 = long.Parse(list4[i].ToString()) + long.Parse(list3[i].ToString());
                        if (l >= l3 && l < l4)
                        {
                            l2 = long.Parse(list2[i].ToString()) - l3 + l1 + l;
                            textBox3.Text = CommonCode.FormatStr(l2.ToString("X16"));
                            break;
                        }
                    }
                    if (i == i5)
                    {
                        MessageBox.Show("输入的地址不在正常范围之内。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ComputeO()//计算虚拟地址的偏移地址
        {
            string s = textBox3.Text;
            long l = CommonCode.HexToLong(s);
            long l1 = long.Parse(list1[0].ToString());//获得基址
            int i5 = list1.Count;//文件头信息总行数
            long offset = long.Parse(list2[i5 - 1].ToString()) - long.Parse(list4[i5 - 1].ToString()) + l1 + lg;
            int i = 0;
            if (l < l1 || l > offset)
            {
                MessageBox.Show("输入的虚拟地址大小已超出正常范围，无法进行计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (l < long.Parse(list4[3].ToString()) + l1)
            {
                textBox2.Text = CommonCode.FormatStr((l - l1).ToString("X8"));
            }
            else
            {
                for (i = 3; i < i5; i++)
                {
                    offset = long.Parse(list2[i].ToString()) - long.Parse(list4[i].ToString()) + l1;//获得偏移量
                    if (l >= long.Parse(list4[i].ToString()) + offset && l < long.Parse(list4[i].ToString()) + long.Parse(list3[i].ToString()) + offset)
                    {
                        textBox2.Text = CommonCode.FormatStr((l - offset).ToString("X8"));
                        break;
                    }
                }
                if (i == i5)
                {
                    MessageBox.Show("输入的虚拟地址不在正常范围之内。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void address(string s)//加载 PE 文件头信息
        {
            list1.Clear();
            list2.Clear();
            list3.Clear();
            list4.Clear();
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            lg = fs.Length - 1;
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u1 = br.ReadUInt32();//PE标识位置
            fs.Seek(u1 + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadInt16();//读取 CPU 类型
            if (i1 == 332)
            {
                cpu = 32;
                fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                i1 = br.ReadUInt16();//文件段数
                fs.Seek(u1 + 52, SeekOrigin.Begin);//基址
                list1.Add(br.ReadUInt32().ToString());//基址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 128, SeekOrigin.Begin);//输入表
                list1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 136, SeekOrigin.Begin);//资源段
                list1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 248, SeekOrigin.Begin);//各个段
                for (int i = 0; i < i1; i++)
                {
                    fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                    list1.Add(br.ReadUInt32().ToString());
                    list2.Add(br.ReadUInt32().ToString());
                    list3.Add(br.ReadUInt32().ToString());
                    list4.Add(br.ReadUInt32().ToString());
                    fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                }
            }
            else
            {
                cpu = 64;
                fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                i1 = br.ReadUInt16();//文件段数
                fs.Seek(u1 + 48, SeekOrigin.Begin);//基址
                list1.Add(br.ReadInt64().ToString());//基址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 144, SeekOrigin.Begin);//输入表
                list1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 152, SeekOrigin.Begin);//资源段
                list1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                list2.Add("0");
                list3.Add("0");
                list4.Add("0");
                fs.Seek(u1 + 264, SeekOrigin.Begin);//各个段
                for (int i = 0; i < i1; i++)
                {
                    fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                    list1.Add(br.ReadUInt32().ToString());
                    list2.Add(br.ReadUInt32().ToString());
                    list3.Add(br.ReadUInt32().ToString());
                    list4.Add(br.ReadUInt32().ToString());
                    fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                }
            }
            br.Close();
            fs.Close();
        }

        private void Calculator_Shown(object sender, EventArgs e)
        {
            if (mainform.PEbool == true)
            {
                if (File.Exists(mainform.FilePath) == true)
                {
                    textBox1.Text = mainform.FilePath;
                    address(mainform.FilePath);
                }
            }
            if (mainform.MyDpi > 96F)
            {
                textBox1.Width = button2.Location.X + button2.Width - textBox1.Location.X;
                label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
                radioButton1.Location = new Point(radioButton1.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - radioButton1.Height / 2D));
                radioButton2.Location = new Point(radioButton2.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - radioButton1.Height / 2D));
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                textBox2.ReadOnly = true;
                textBox3.ReadOnly = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s == "")
            {
                MessageBox.Show("请指定用于计算的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(s) == false)
            {
                MessageBox.Show("指定的文件不存在，无法进行计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string x = "";
                if (radioButton2.Checked == true)
                {
                    x = textBox3.Text;
                    if (x == "")
                    {
                        MessageBox.Show("请输入用于计算偏移地址的虚拟地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (CommonCode.Is_Hex(x) == false)
                        {
                            MessageBox.Show("虚拟地址不是有效的十六进制值，无法计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            textBox3.Text = CommonCode.FormatStr(x);
                            textBox2.Clear();
                            ComputeO();
                        }
                    }
                }
                else
                {
                    x = textBox2.Text;
                    if (x == "")
                    {
                        MessageBox.Show("请输入用于计算虚拟地址的偏移地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        if (CommonCode.Is_Hex(x) == false)
                        {
                            MessageBox.Show("偏移地址不是有效的十六进制值，无法计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            textBox2.Text = CommonCode.FormatStr(x);
                            textBox3.Clear();
                            ComputeV();
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = CommonCode.Open_Exe_File(textBox1.Text);
            if (s != "")
            {
                if (CommonCode.PE(s) == false)
                {
                    MessageBox.Show("指定的文件不是一个有效的 PE 文件，无法用于计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    textBox1.Text = s;
                    textBox2.Clear();
                    textBox3.Clear();
                    address(s);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = textBox2.Text;
            if (s != "")
            {
                if (CommonCode.Is_Hex(s) == false)
                {
                    MessageBox.Show("偏移地址不是有效的十六进制值，无法转换。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    textBox2.Text = CommonCode.InvertHex(CommonCode.FormatStr(s));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = textBox3.Text;
            if (s != "")
            {
                if (cpu == 32)
                {
                    if (CommonCode.Is_Hex(s) == false)
                    {
                        MessageBox.Show("虚拟地址不是有效的十六进制值，无法转换。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        textBox3.Text = CommonCode.InvertHex(CommonCode.FormatStr(s));
                    }
                }
                else
                {
                    if (CommonCode.Is_Hex(s) == false)
                    {
                        MessageBox.Show("虚拟地址不是有效的十六进制值，无法转换。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        textBox3.Text = CommonCode.InvertHex(CommonCode.FormatStr(s));
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = "";
            if (radioButton1.Checked == true)
            {
                s = textBox3.Text;
                if (s != "")
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(s);
                }
            }
            else
            {
                s = textBox2.Text;
                if (s != "")
                {
                    Clipboard.Clear();
                    Clipboard.SetDataObject(s);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string s = Clipboard.GetText();
            if (CommonCode.Is_Hex(s) == true)
            {
                if (radioButton1.Checked == true)
                {
                    textBox2.Text = CommonCode.FormatStr(s);
                }
                else
                {
                    textBox3.Text = CommonCode.FormatStr(s);
                }
            }
            else
            {
                MessageBox.Show("剪贴板中没有数据，或其内容不是有效的十六进制值。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Calculator_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            if (CommonCode.PE(s) == false)
            {
                MessageBox.Show("指定的文件不是一个有效的 PE 文件，无法用于计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox1.Text = s;
                textBox2.Clear();
                textBox3.Clear();
                address(s);
            }
        }

        private void Calculator_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }
    }
}
