using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class MoveTool : Form
    {
        static ArrayList list1 = new ArrayList();
        static ArrayList list2 = new ArrayList();
        static ArrayList list3 = new ArrayList();
        static ArrayList list4 = new ArrayList();
        static long lg = 0;

        public MoveTool()
        {
            InitializeComponent();
        }

        private void MoveTool_Shown(object sender, EventArgs e)
        {
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            checkBox1.Location = new Point(checkBox1.Location.X, button5.Location.Y + (int)(button5.Height / 2D - checkBox1.Height / 2D));
            textBox1.Width = button1.Location.X + button1.Width - textBox1.Location.X;
            groupBox2.Width = textBox2.Location.X + textBox2.Width - groupBox2.Location.X;
            if (mainform.PEbool == true)
            {
                if (File.Exists(mainform.FilePath) == true)
                {
                    textBox1.Text = mainform.FilePath;
                    address(mainform.FilePath);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
                    listBox1.Items.Clear();
                    address(s);
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
                radioButton1.Checked = true;
            }
            else
            {
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
                radioButton2.Checked = true;
            }
            br.Close();
            fs.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            string s = Clipboard.GetText();
            if (CommonCode.Is_Hex(s) == true)
            {
                textBox2.Text = CommonCode.FormatStr(s);
            }
            else
            {
                MessageBox.Show("剪贴板中没有数据，或其内容不是有效的十六进制值。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            if (s1 == "")
            {
                MessageBox.Show("请指定用于搜索的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("指定的文件不存在，无法进行搜索。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s2 == "")
            {
                MessageBox.Show("请输入搜索的地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CommonCode.Is_Hex(s2) == false)
            {
                MessageBox.Show("输入的地址不是有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox2.Text = CommonCode.FormatStr(s2);
                long l = CommonCode.HexToLong(textBox2.Text);
                if (l > lg)
                {
                    MessageBox.Show("输入的地址已超出了文件的大小。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (l < long.Parse(list4[3].ToString()))
                {
                    MessageBox.Show("输入的地址太小，不在搜索范围之内。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ControlDisabled();
                    if (radioButton1.Checked == true)
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                    else
                    {
                        backgroundWorker2.RunWorkerAsync();
                    }
                }
            }
        }

        private void ControlDisabled()
        {
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            label4.Enabled = false;
            label5.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            listBox1.Enabled = false;
            groupBox2.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            checkBox1.Enabled = false;
        }

        private void ControlEnable()
        {
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            label5.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            listBox1.Enabled = true;
            groupBox2.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            checkBox1.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
            listBox1.Items.Clear();
            int i5 = list1.Count;//文件头信息总行数
            long l6 = 0;
            long l7 = 0;
            byte[] b = BitConverter.GetBytes(CommonCode.GetVirtualAddress(CommonCode.HexToLong(textBox2.Text)));
            byte b1 = b[0];
            byte b2 = b[1];
            byte b3 = b[2];
            byte b4 = b[3];
            l6 = long.Parse(mainform.l4[3].ToString());
            if (long.Parse(mainform.l1[2].ToString()) == 0)
            {
                l7 = long.Parse(mainform.l4[i5 - 1].ToString()) + long.Parse(mainform.l3[i5 - 1].ToString());
            }
            else
            {
                for (int i = 3; i < i5; i++)
                {
                    if (mainform.l1[2].ToString() == mainform.l2[i].ToString())
                    {
                        l7 = long.Parse(mainform.l4[i].ToString());
                        break;
                    }
                }
            }
            long LongSplit = Math.DivRem(l7 - l6, 3, out long m);
            Parallel.Invoke(() =>
            {
                long StartLong = l6;
                long EndLong = StartLong + LongSplit + 7;
                using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(StartLong, SeekOrigin.Begin);
                    AA: byte xb1 = br.ReadByte();
                        byte xb2 = br.ReadByte();
                        byte xb3 = br.ReadByte();
                        byte xb4 = br.ReadByte();
                        if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                        {
                            RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                            goto AA;
                        }
                        while (fs.Position < EndLong)
                        {
                            xb1 = br.ReadByte();
                            if (b1 == xb2 && b2 == xb3 && b3 == xb4 && b4 == xb1)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb2 = br.ReadByte();
                            if (b1 == xb3 && b2 == xb4 && b3 == xb1 && b4 == xb2)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb3 = br.ReadByte();
                            if (b1 == xb4 && b2 == xb1 && b3 == xb2 && b4 == xb3)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb4 = br.ReadByte();
                            if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                        }
                    }
                }
            },
            () =>
            {
                long StartLong = l6 + LongSplit;
                long EndLong = StartLong + LongSplit + 7;
                using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(StartLong, SeekOrigin.Begin);
                    AA: byte xb1 = br.ReadByte();
                        byte xb2 = br.ReadByte();
                        byte xb3 = br.ReadByte();
                        byte xb4 = br.ReadByte();
                        if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                        {
                            RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                            goto AA;
                        }
                        while (fs.Position < EndLong)
                        {
                            xb1 = br.ReadByte();
                            if (b1 == xb2 && b2 == xb3 && b3 == xb4 && b4 == xb1)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb2 = br.ReadByte();
                            if (b1 == xb3 && b2 == xb4 && b3 == xb1 && b4 == xb2)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb3 = br.ReadByte();
                            if (b1 == xb4 && b2 == xb1 && b3 == xb2 && b4 == xb3)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb4 = br.ReadByte();
                            if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                        }
                    }
                }
            },
            () =>
            {
                long StartLong = l6 + LongSplit * 2;
                long EndLong = l7;
                using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(StartLong, SeekOrigin.Begin);
                    AA: byte xb1 = br.ReadByte();
                        byte xb2 = br.ReadByte();
                        byte xb3 = br.ReadByte();
                        byte xb4 = br.ReadByte();
                        if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                        {
                            RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                            goto AA;
                        }
                        while (fs.Position < EndLong)
                        {
                            xb1 = br.ReadByte();
                            if (b1 == xb2 && b2 == xb3 && b3 == xb4 && b4 == xb1)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb2 = br.ReadByte();
                            if (b1 == xb3 && b2 == xb4 && b3 == xb1 && b4 == xb2)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb3 = br.ReadByte();
                            if (b1 == xb4 && b2 == xb1 && b3 == xb2 && b4 == xb3)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                            xb4 = br.ReadByte();
                            if (b1 == xb1 && b2 == xb2 && b3 == xb3 && b4 == xb4)
                            {
                                RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                goto AA;
                            }
                        }
                    }
                }
            });
            int Tmp = RefAddress.Count;
            if (Tmp > 0)
            {
                string[] RefAddressStr = new string[Tmp];
                RefAddress.Keys.CopyTo(RefAddressStr, 0);
                Array.Sort(RefAddressStr);
                for (int y = 0; y < Tmp; y++)
                {
                    listBox1.Items.Add(RefAddressStr[y]);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ControlEnable();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
            listBox2.Items.Clear();
            int i5 = list1.Count;//文件头信息总行数
            int x = 0;//最大搜索段
            long VStr_Long = CommonCode.GetVirtualAddress(CommonCode.HexToLong(textBox2.Text));//获得当前字符串虚拟地址
            long l2 = 0;
            long l6 = 0;
            long l7 = 0;
            if (long.Parse(mainform.l1[2].ToString()) == 0)//没有资源段
            {
                x = i5;
            }
            else
            {
                for (x = 3; x < i5; x++)
                {
                    if (mainform.l1[2].ToString() == mainform.l2[x].ToString())
                    {
                        break;
                    }
                }
            }
            for (int n = 3; n < x; n++)
            {
                l2 = long.Parse(mainform.l3[n].ToString());//段长度
                l6 = long.Parse(mainform.l4[n].ToString());//段起始地址
                l7 = l6 + l2;//段结尾地址
                long LongSplit = Math.DivRem(l2, 3, out long m);
                Parallel.Invoke(() =>
                {
                    long StartLong = l6;
                    long EndLong = StartLong + LongSplit + 7;
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            fs.Seek(StartLong, SeekOrigin.Begin);
                            long VC_Long = VStr_Long - CommonCode.GetVirtualAddress(StartLong) - 4;
                        AA: byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            byte xb4 = br.ReadByte();
                            if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                            {
                                if (VC_Long != 0)
                                {
                                    RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                    VC_Long -= 4;
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb1 = br.ReadByte();
                                VC_Long--;
                                if (xb2 == (byte)VC_Long && xb3 == (byte)(VC_Long >> 8) && xb4 == (byte)(VC_Long >> 16) && xb1 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                VC_Long--;
                                if (xb3 == (byte)VC_Long && xb4 == (byte)(VC_Long >> 8) && xb1 == (byte)(VC_Long >> 16) && xb2 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                VC_Long--;
                                if (xb4 == (byte)VC_Long && xb1 == (byte)(VC_Long >> 8) && xb2 == (byte)(VC_Long >> 16) && xb3 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb4 = br.ReadByte();
                                VC_Long--;
                                if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    long StartLong = l6 + LongSplit;
                    long EndLong = StartLong + LongSplit + 7;
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            fs.Seek(StartLong, SeekOrigin.Begin);
                            long VC_Long = VStr_Long - CommonCode.GetVirtualAddress(StartLong) - 4;
                        AA: byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            byte xb4 = br.ReadByte();
                            if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                            {
                                if (VC_Long != 0)
                                {
                                    RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                    VC_Long -= 4;
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb1 = br.ReadByte();
                                VC_Long--;
                                if (xb2 == (byte)VC_Long && xb3 == (byte)(VC_Long >> 8) && xb4 == (byte)(VC_Long >> 16) && xb1 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                VC_Long--;
                                if (xb3 == (byte)VC_Long && xb4 == (byte)(VC_Long >> 8) && xb1 == (byte)(VC_Long >> 16) && xb2 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                VC_Long--;
                                if (xb4 == (byte)VC_Long && xb1 == (byte)(VC_Long >> 8) && xb2 == (byte)(VC_Long >> 16) && xb3 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb4 = br.ReadByte();
                                VC_Long--;
                                if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    long StartLong = l6 + LongSplit * 2;
                    long EndLong = l7;
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            fs.Seek(StartLong, SeekOrigin.Begin);
                            long VC_Long = VStr_Long - CommonCode.GetVirtualAddress(StartLong) - 4;
                        AA: byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            byte xb4 = br.ReadByte();
                            if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                            {
                                if (VC_Long != 0)
                                {
                                    RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                    VC_Long -= 4;
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb1 = br.ReadByte();
                                VC_Long--;
                                if (xb2 == (byte)VC_Long && xb3 == (byte)(VC_Long >> 8) && xb4 == (byte)(VC_Long >> 16) && xb1 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                VC_Long--;
                                if (xb3 == (byte)VC_Long && xb4 == (byte)(VC_Long >> 8) && xb1 == (byte)(VC_Long >> 16) && xb2 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                VC_Long--;
                                if (xb4 == (byte)VC_Long && xb1 == (byte)(VC_Long >> 8) && xb2 == (byte)(VC_Long >> 16) && xb3 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                                xb4 = br.ReadByte();
                                VC_Long--;
                                if (xb1 == (byte)VC_Long && xb2 == (byte)(VC_Long >> 8) && xb3 == (byte)(VC_Long >> 16) && xb4 == (byte)(VC_Long >> 24))
                                {
                                    if (VC_Long != 0)
                                    {
                                        RefAddress.TryAdd((fs.Position - 4).ToString("X8"), 0);
                                        VC_Long -= 4;
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            int Tmp = RefAddress.Count;
            if (Tmp > 0)
            {
                string[] RefAddressStr = new string[Tmp];
                RefAddress.Keys.CopyTo(RefAddressStr, 0);
                Array.Sort(RefAddressStr);
                for (int y = 0; y < Tmp; y++)
                {
                    listBox2.Items.Add(RefAddressStr[y]);
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ControlEnable();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int i1 = listBox1.SelectedItems.Count;
            int i2 = listBox2.SelectedItems.Count;
            if (i1 == 0 && i2 == 0)
            {
                MessageBox.Show("请选择需要复制的地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Clipboard.Clear();
                string s = "";
                if (i1 > 0 && radioButton1.Checked == true)
                {
                    for (int i = 0; i < i1 - 1; i++)
                    {
                        s = s + listBox1.SelectedItems[i].ToString() + "\r\n";
                    }
                    s = s + listBox1.SelectedItems[i1 - 1].ToString();
                    Clipboard.SetDataObject(s);
                }
                else if (i2 > 0 && radioButton2.Checked == true)
                {
                    for (int i = 0; i < i2 - 1; i++)
                    {
                        s = s + listBox2.SelectedItems[i].ToString() + "\r\n";
                    }
                    s = s + listBox2.SelectedItems[i2 - 1].ToString();
                    Clipboard.SetDataObject(s);
                }
            }
        }

        private void SearchAddress_DragEnter(object sender, DragEventArgs e)
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

        private void SearchAddress_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            if (CommonCode.PE(s) == false)
            {
                MessageBox.Show("指定的文件不是一个有效的 PE 文件，无法用于计算。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox1.Text = s;
                listBox1.Items.Clear();
                address(s);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            string s = Clipboard.GetText();
            if (CommonCode.Is_Hex(s) == true)
            {
                textBox3.Text = CommonCode.FormatStr(s);
            }
            else
            {
                MessageBox.Show("剪贴板中没有数据，或其内容不是有效的十六进制值。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private int OffSet32(long l)
        {
            int i = 0;
            long l2 = long.Parse(list1[0].ToString());//获得基址
            long i3 = 0;
            long i4 = 0;
            int i5 = list1.Count;//文件头信息总行数
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(list4[i].ToString());
                i4 = i3 + long.Parse(list3[i].ToString());
                if (l > i3 && l < i4)
                {
                    break;
                }
            }
            return (int)(long.Parse(list2[i].ToString()) - i3 + l2 + l);//获得移动后的虚拟地址
        }

        private int OffSet64(long l, string s)
        {
            int i = 0;
            long l2 = long.Parse(list1[0].ToString());//获得基址
            long i3 = 0;
            long i4 = 0;
            long l5 = 0;
            long l6 = 0;
            int i5 = list1.Count;//文件头信息总行数
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(list4[i].ToString());
                i4 = i3 + long.Parse(list3[i].ToString());
                if (l > i3 && l < i4)
                {
                    break;
                }
            }
            l5 = long.Parse(list2[i].ToString()) - i3 + l2 + l;//获得移动后的虚拟地址
            l = CommonCode.HexToLong(s) + 4;
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(list4[i].ToString());
                i4 = i3 + long.Parse(list3[i].ToString());
                if (l > i3 && l < i4)
                {
                    break;
                }
            }
            l6 = long.Parse(list2[i].ToString()) - i3 + l2 + l;//获得移动后的虚拟地址
            return (int)(l5 - l6);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s3 = textBox3.Text;
            if (s1 == "")
            {
                MessageBox.Show("请指定需要进行挪移的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("指定的文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (s3 == "")
            {
                MessageBox.Show("请输入目标地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CommonCode.Is_Hex(s3) == false)
            {
                MessageBox.Show("输入的目标地址不是有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (listBox1.Items.Count == 0 && listBox2.Items.Count == 0)
            {
                MessageBox.Show("请先搜索物理地址的调用地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                long l1 = CommonCode.HexToLong(s3);
                if (l1 > long.Parse(list4[list4.Count - 1].ToString()) + long.Parse(list3[list3.Count - 1].ToString()))
                {
                    MessageBox.Show("输入的目标地址已经超出可用段范围之外。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (checkBox1.Checked == true)
                    {
                        s3 = s1 + ".org";
                        if (File.Exists(s3) == false)
                        {
                            File.Copy(s1, s3);
                        }
                    }
                    try
                    {
                        FileStream fs = new FileStream(s1, FileMode.Open);
                        BinaryWriter bw = new BinaryWriter(fs);
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            fs.Seek(CommonCode.HexToLong(listBox1.Items[i].ToString()), SeekOrigin.Begin);
                            bw.Write(OffSet32(l1));
                        }
                        for (int i = 0; i < listBox2.Items.Count; i++)
                        {
                            fs.Seek(CommonCode.HexToLong(listBox2.Items[i].ToString()), SeekOrigin.Begin);
                            bw.Write(OffSet64(l1, listBox2.Items[i].ToString()));
                        }
                        bw.Close();
                        fs.Close();
                        MessageBox.Show("修改地址成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception MyEx)
                    {
                        MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox1.SelectedItems.Count;
                if (i1 > 0)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                    }
                }
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox2.SelectedItems.Count;
                if (i1 > 0)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        listBox2.Items.RemoveAt(listBox2.SelectedIndex);
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
        }
    }
}
