using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class Matrix : Form
    {
        string mataddress = "";//所在区域
        int mataddressIndex = 0;//区域索引
        int addlong = 0;//区域长度
        int freelong = 0;//剩余长度

        public Matrix()
        {
            InitializeComponent();
        }

        private void Matrix_Shown(object sender, EventArgs e)
        {
            //dataTable2  matrixzone
            //   0         1           2           3       4       5       6
            //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
            //
            //dataTable3  calladd
            //   0      1     2      3
            //address  cd  offset  bits
            //
            //dataTable7  athenaa
            //   0        1       2       3      4        5            6            7
            //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
            if (mainform.MyDpi > 96F)
            {
                int i1 = textBox1.Location.Y + (int)(textBox1.Height / 2D - label3.Height / 2D);
                label3.Location = new Point(label3.Location.X, i1);
                label4.Location = new Point(label4.Location.X, i1);
                label6.Location = new Point(label6.Location.X, i1);
            }
            string s1 = mainform.obMS[0].ToString();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from matrix";
                        ad.Fill(dataTable1);
                        cmd.CommandText = "select * from matrixzone where address = '" + s1 + "'";
                        ad.Fill(dataTable2);
                        cmd.CommandText = "select * from calladd where address = '" + s1 + "'";
                        ad.Fill(dataTable3);
                    }
                }
            }
            if (dataTable3.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable3.Rows.Count; i++)
                {
                    if (dataTable3.Rows[i][3].ToString() == "32")
                    {
                        listBox1.Items.Add(dataTable3.Rows[i][1].ToString());
                    }
                    else
                    {
                        listBox2.Items.Add(dataTable3.Rows[i][1].ToString());
                    }
                }
            }
            if (dataTable2.Rows.Count > 0)
            {
                mataddress = dataTable2.Rows[0][1].ToString();
                textBox2.Text = dataTable2.Rows[0][2].ToString();
                textBox3.Text = mataddress;
            }
            if (dataTable1.Rows.Count > 0)
            {
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    if (mataddress == dataTable1.Rows[i][0].ToString())
                    {
                        mataddressIndex = i;
                        addlong = int.Parse(dataTable1.Rows[i][1].ToString());
                    }
                    dataGridView1.Rows.Add(dataTable1.Rows[i].ItemArray);
                }
            }
            //0 文件
            //1 版本
            //2 大小
            //3 目标
            //4 非PE类型
            //5 运行
            //6 字典
            //7 标记
            //8 PE类型
            //9 长度标识
            //10 代码页
            //
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)
            {
                label8.Text = mainform.ProTraName;
                label7.Text = mainform.ProTraCode.ToString();
                if ((bool)mainform.obMS[15] == true)
                {
                    radioButton3.Enabled = false;
                    if (mainform.obMS[18].ToString() == "0")
                    {
                        if (mainform.DelphiCodePage == "0")
                        {
                            radioButton1.Checked = true;
                        }
                        else
                        {
                            radioButton2.Checked = true;
                        }
                    }
                    else
                    {
                        radioButton2.Checked = true;
                    }
                }
                else
                {
                    if (mainform.obMS[18].ToString() == "0")
                    {
                        if (mainform.DelphiCodePage == "0")
                        {
                            radioButton1.Checked = true;
                        }
                        else if (mainform.DelphiCodePage == "1")
                        {
                            radioButton2.Checked = true;
                        }
                        else
                        {
                            radioButton3.Checked = true;
                        }
                    }
                    else if (mainform.obMS[18].ToString() == "1")
                    {
                        radioButton2.Checked = true;
                    }
                    else
                    {
                        radioButton3.Checked = true;
                    }
                }
            }
            else
            {
                groupBox1.Enabled = false;
            }
            if ((bool)mainform.obMS[16] == true)//启用矩阵
            {
                groupBox1.Enabled = false;
            }
            if (mainform.CPUType == "32")
            {
                radioButton4.Checked = true;
            }
            else
            {
                radioButton5.Checked = true;
            }
            textBox1.Text = mainform.obMS[4].ToString();
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

        private void button1_Click(object sender, EventArgs e)//检索
        {
            if (File.Exists(mainform.FilePath) == false)
            {
                MessageBox.Show("没有找到英文版文件，请查看工程属性来了解相关信息。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (dataTable1.Rows.Count == 0)
            {
                MessageBox.Show("请首先在编辑菜单中设定矩阵区域。\r\n如果是在建立工程以后扩展了 PE 段，\r\n请先在工程属性中重读 PE 段。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if ((bool)mainform.obMS[16] == true)
            {
                MessageBox.Show("已应用了挪移操作，请首先清除再进行检索。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                label1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                label5.Enabled = false;
                label6.Enabled = false;
                dataGridView1.Enabled = false;
                listBox1.Enabled = false;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private long OffSetAdd32(long l, string s)
        {
            int i = 0;
            long l2 = long.Parse(mainform.l1[0].ToString());//获得基址
            long i3 = 0;
            long i4 = 0;
            int i5 = mainform.l1.Count;//文件头信息总行数
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(mainform.l4[i].ToString());
                i4 = i3 + long.Parse(mainform.l3[i].ToString());
                if (l >= i3 && l < i4)
                {
                    break;
                }
            }
            return long.Parse(mainform.l2[i].ToString()) - i3 + l2 + l;//获得移动后的虚拟地址
        }

        private long OffSetAdd64(long l, string s)
        {
            int i = 0;
            long l2 = long.Parse(mainform.l1[0].ToString());//获得基址
            long i3 = 0;
            long i4 = 0;
            long l5 = 0;
            long l6 = 0;
            int i5 = mainform.l1.Count;//文件头信息总行数
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(mainform.l4[i].ToString());
                i4 = i3 + long.Parse(mainform.l3[i].ToString());
                if (l >= i3 && l < i4)
                {
                    break;
                }
            }
            l5 = long.Parse(mainform.l2[i].ToString()) - i3 + l2 + l;//获得移动后的虚拟地址
            l = CommonCode.HexToLong(s) + 4;
            for (i = 3; i < i5; i++)//所处段
            {
                i3 = long.Parse(mainform.l4[i].ToString());
                i4 = i3 + long.Parse(mainform.l3[i].ToString());
                if (l >= i3 && l < i4)
                {
                    break;
                }
            }
            l6 = long.Parse(mainform.l2[i].ToString()) - i3 + l2 + l;//获得移动后的虚拟地址
            return l5 - l6;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
            if (radioButton4.Checked == true)
            {
                listBox1.Items.Clear();
                int i5 = mainform.l1.Count;//文件头信息总行数
                long l6 = 0;
                long l7 = 0;
                byte[] b = BitConverter.GetBytes(CommonCode.GetVirtualAddress(CommonCode.HexToLong(mainform.obMS[0].ToString())));
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
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            else//64 位程序
            {
                listBox2.Items.Clear();
                int x = 0;//最大搜索段
                int i5 = mainform.l1.Count;//文件头信息总行数
                long VStr_Long = CommonCode.GetVirtualAddress(CommonCode.HexToLong(mainform.obMS[0].ToString()));//获得当前字符串虚拟地址
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
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (listBox1.Items.Count > 0 || listBox2.Items.Count > 0)
            {
                button2.Enabled = true;
            }
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            label5.Enabled = true;
            label6.Enabled = true;
            listBox1.Enabled = true;
            listBox2.Enabled = true;
            textBox1.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)
            {
                groupBox1.Enabled = true;
            }
            groupBox2.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            dataGridView1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)//应用。
        {
            if (listBox1.Items.Count == 0 && listBox2.Items.Count == 0)
            {
                MessageBox.Show("没有可用的调用地址，无法实现字符串挪移功能。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //   0       1     2       3         4        5        6        7       8          9          10            11
                //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                //   12         13      14      15      16        17            18           19          20          21
                //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                //            实际是                           没有使用        0 无
                //           长度标识                                         1 不变
                freelong = int.Parse(dataGridView1.SelectedRows[0].Cells[2].Value.ToString());//可用长度
                addlong = int.Parse(dataGridView1.SelectedRows[0].Cells[1].Value.ToString());//原始长度
                int tralong = int.Parse(mainform.obMS[4].ToString());//字符长度
                int uselong = tralong;//实际占用的长度
                if (mainform.ProEncoding == "Unicode")
                {
                    if (mainform.StrLenCategory == "无")
                    {
                        uselong += 2;
                    }
                    else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                    {
                        if ((bool)mainform.obMS[13])
                        {
                            uselong += 6;
                        }
                        else
                        {
                            uselong += 2;
                        }
                    }
                    else if (mainform.StrLenCategory == "Delphi")
                    {
                        if ((bool)mainform.obMS[13])
                        {
                            if (radioButton1.Checked)
                            {
                                uselong += 10;
                            }
                            else
                            {
                                uselong += 14;
                            }
                        }
                        else
                        {
                            uselong += 2;
                        }
                    }
                }
                else if (mainform.ProEncoding == "ANSI")
                {
                    if (mainform.StrLenCategory == "无")
                    {
                        uselong += 1;
                    }
                    else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                    {
                        if ((bool)mainform.obMS[13])
                        {
                            uselong += 5;
                        }
                        else
                        {
                            uselong += 1;
                        }
                    }
                    else if (mainform.StrLenCategory == "Delphi")
                    {
                        if ((bool)mainform.obMS[13])
                        {
                            if (radioButton1.Checked)
                            {
                                uselong += 9;
                            }
                            else
                            {
                                uselong += 13;
                            }
                        }
                        else
                        {
                            uselong += 1;
                        }
                    }
                }
                else if (mainform.ProEncoding == "UTF8")
                {
                    uselong += 1;
                }
                if (uselong > freelong)
                {
                    MessageBox.Show("可用长度不足。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    //dataTable2  matrixzone
                    //   0         1           2           3       4       5       6
                    //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                    //
                    //dataTable3  calladd
                    //   0      1     2      3
                    //address  cd  offset  bits
                    string address = mainform.obMS[0].ToString();//字符串地址
                    mataddress = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();//矩阵地址
                    mataddressIndex = dataGridView1.SelectedRows[0].Index;//注意：如果是第一个矩阵区域，那么批量挪移也会用到
                    if (CommonCode.HexToLong(address) >= CommonCode.HexToLong(mataddress) && CommonCode.HexToLong(address) <= CommonCode.HexToLong(mataddress) + addlong)
                    {
                        MessageBox.Show("需要挪移到矩阵中的地址不能处于矩阵当中。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        //mataddress = "";//所在区域 string
                        //mataddressIndex = 0;//区域索引 int
                        //addlong = 0;//区域长度 int
                        //freelong = 0;//剩余长度 int
                        int x13 = 0;
                        int x15 = 0;
                        if ((bool)mainform.obMS[13])
                        {
                            x13 = 1;
                        }
                        if ((bool)mainform.obMS[15])
                        {
                            x15 = 1;
                        }
                        SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                        MyAccess.Open();
                        SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                        SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                        cmd.Transaction = MyAccess.BeginTransaction();
                        cmd.CommandText = "update athenaa set Ignoretra = 0 where address = '" + address + "'";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "delete from IgnoreTra where address = '" + address + "'";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "Insert Into matrixzone (address,mataddress,zoneaddress,tralong,delphi,ucode) Values ('"
                            + address + "','" + mataddress + "',''," + tralong + "," + x13 + "," + x15 + ")";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update athenaa set zonebl = 1 where address = '" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            cmd.CommandText = "Insert Into calladd (address,cd,offset,bits) Values ('"
                                + address + "','"
                                + listBox1.Items[i].ToString() + "',0,'32')";
                            cmd.ExecuteNonQuery();
                        }
                        for (int i = 0; i < listBox2.Items.Count; i++)
                        {
                            cmd.CommandText = "Insert Into calladd (address,cd,offset,bits) Values ('"
                                + address + "','"
                                + listBox2.Items[i].ToString() + "',0,'64')";
                            cmd.ExecuteNonQuery();
                        }
                        if (mataddressIndex == 0)//第一个矩阵也是批量挪移矩阵
                        {
                            cmd.CommandText = "delete from Resolve32 where address = '" + address + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from Resolve64 where address = '" + address + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "delete from ResolveAddress where address = '" + address + "'";
                            cmd.ExecuteNonQuery();
                            int x32 = 0;
                            int x64 = 0;
                            for (x32 = 0; x32 < listBox1.Items.Count; x32++)
                            {
                                cmd.CommandText = "Insert Into Resolve32 (address,Raddress) Values ('" + address + "','" + listBox1.Items[x32].ToString() + "')";
                                cmd.ExecuteNonQuery();
                            }
                            for (x64 = 0; x64 < listBox2.Items.Count; x64++)
                            {
                                cmd.CommandText = "Insert Into Resolve64 (address,Raddress) Values ('" + address + "','" + listBox2.Items[x64].ToString() + "')";
                                cmd.ExecuteNonQuery();
                            }
                            string s32 = "";
                            string s64 = "";
                            if (x32 > 0)
                            {
                                s32 = x32.ToString();
                            }
                            if (x64 > 0)
                            {
                                s64 = x64.ToString();
                            }
                            string strCode = "无";
                            if (radioButton2.Checked)
                            {
                                strCode = "不变";
                            }
                            else if (radioButton3.Checked)
                            {
                                strCode = mainform.ProTraName;
                            }
                            cmd.CommandText = "Insert Into ResolveAddress (address,x32,x64,codepage,SelectedAddress,orglong) Values ('" + address + "','" + s32 + "','" + s64 + "','" + strCode + "',1," + (int)mainform.obMS[3] + ")";
                            cmd.ExecuteNonQuery();
                        }
                        //   0       1     2       3         4        5        6        7       8          9          10            11
                        //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                        //   12         13      14      15      16        17            18           19          20          21
                        //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                        //            实际是                           没有使用        0 无
                        //           长度标识                                         1 不变
                        if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)//13 Delphi,15 Unicode,17 codepage,18 delphicodepage
                        {
                            if (radioButton1.Checked == true)
                            {
                                cmd.CommandText = "update matrixzone set codepage = 0 where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update fileinfo set detail = '0' where infoname = '代码页'";
                                cmd.ExecuteNonQuery();
                                mainform.obMS[18] = 0;
                                mainform.DelphiCodePage = "0";
                            }
                            else if (radioButton2.Checked == true)
                            {
                                cmd.CommandText = "update matrixzone set codepage = 1 where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = 1 where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update fileinfo set detail = '1' where infoname = '代码页'";
                                cmd.ExecuteNonQuery();
                                mainform.obMS[18] = 1;
                                mainform.DelphiCodePage = "1";
                            }
                            else
                            {
                                cmd.CommandText = "update matrixzone set codepage = 1 where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = " + mainform.ProTraCode + " where address = '" + address + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update fileinfo set detail = '" + mainform.ProTraCode.ToString() + "' where infoname = '代码页'";
                                cmd.ExecuteNonQuery();
                                mainform.obMS[18] = mainform.ProTraCode;
                                mainform.DelphiCodePage = mainform.ProTraCode.ToString();
                            }
                        }
                        cmd.Transaction.Commit();
                        mainform.obMS[16] = true;
                        groupBox1.Enabled = false;
                        button2.Enabled = false;
                        UpdateMatrixData();
                        textBox3.Text = mataddress;
                        dataGridView1.Rows[mataddressIndex].Cells[2].Value = freelong;
                        dataTable2.Rows.Clear();
                        cmd.CommandText = "select zoneaddress from matrixzone where address = '" + address + "'";
                        textBox2.Text = cmd.ExecuteScalar().ToString();//返回首行首列的第一个值，否则结果为 null
                        MyAccess.Close();
                    }
                }
            }
        }

        private void UpdateMatrixData()
        {
            //进入这里之前，首先确定 mataddress 和 addlong，处理完后结果存储在 freelong
            SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
            MyAccess.Open();
            SQLiteCommand cmd = new SQLiteCommand(MyAccess);
            SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
            cmd.Transaction = MyAccess.BeginTransaction();
            dataTable2.Rows.Clear();
            cmd.CommandText = "select * from matrixzone where mataddress = '" + mataddress + "'";
            ad.Fill(dataTable2);
            if (dataTable2.Rows.Count > 0)
            {
                dataTable3.Rows.Clear();
                for (int i = 0; i < dataTable2.Rows.Count; i++)
                {
                    cmd.CommandText = "select * from calladd where address = '" + dataTable2.Rows[i][0].ToString() + "'";
                    ad.Fill(dataTable3);
                }
                int iLen = 0;//合计字符串长度
                string address = "";//原始字符串地址
                long BaseAddress = CommonCode.HexToLong(mataddress);//矩阵基址
                long zoneaddress = BaseAddress;//新字符串地址 long
                if (mainform.ProEncoding == "Unicode")
                {
                    if (mainform.StrLenCategory == "无")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 2;
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                    else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        if ((int)dataTable2.Rows[0][4] == 1)
                        {
                            iLen += 4;
                        }
                        zoneaddress = BaseAddress + iLen;
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 2;
                            if ((int)dataTable2.Rows[i][4] == 1)
                            {
                                iLen += 4;
                            }
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                    else if (mainform.StrLenCategory == "Delphi")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        if ((int)dataTable2.Rows[0][4] == 1)
                        {
                            iLen += 8;
                            if ((int)dataTable2.Rows[0][6] == 1)
                            {
                                iLen += 4;
                            }
                        }
                        zoneaddress = BaseAddress + iLen;
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 2;
                            if ((int)dataTable2.Rows[i][4] == 1)
                            {
                                iLen += 8;
                                if ((int)dataTable2.Rows[i][6] == 1)
                                {
                                    iLen += 4;
                                }
                            }
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                }
                else if (mainform.ProEncoding == "ANSI")
                {
                    if (mainform.StrLenCategory == "无")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 1;
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                    else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        if ((int)dataTable2.Rows[0][4] == 1)
                        {
                            iLen += 4;
                        }
                        zoneaddress = BaseAddress + iLen;
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 1;
                            if ((int)dataTable2.Rows[i][4] == 1)
                            {
                                iLen += 4;
                            }
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                    else if (mainform.StrLenCategory == "Delphi")
                    {
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        //
                        //dataTable3  calladd
                        //   0      1     2      3
                        //address  cd  offset  bits
                        if ((int)dataTable2.Rows[0][4] == 1)
                        {
                            iLen += 8;
                            if ((int)dataTable2.Rows[0][6] == 1)
                            {
                                iLen += 4;
                            }
                        }
                        zoneaddress = BaseAddress + iLen;
                        address = dataTable2.Rows[0][0].ToString();
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[0][3];
                        for (int i = 1; i < dataTable2.Rows.Count; i++)
                        {
                            iLen += 1;
                            if ((int)dataTable2.Rows[i][4] == 1)
                            {
                                iLen += 8;
                                if ((int)dataTable2.Rows[i][6] == 1)
                                {
                                    iLen += 4;
                                }
                            }
                            address = dataTable2.Rows[i][0].ToString();
                            zoneaddress = BaseAddress + iLen;
                            cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                            cmd.ExecuteNonQuery();
                            for (int x = 0; x < dataTable3.Rows.Count; x++)
                            {
                                if (address == dataTable3.Rows[x][0].ToString())
                                {
                                    if (dataTable3.Rows[x][3].ToString() == "32")
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                        cmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        cmd.CommandText = "update calladd set offset = "
                                            + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                            + " where address ='" + address
                                            + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            iLen += (int)dataTable2.Rows[i][3];
                        }
                    }
                }
                else if (mainform.ProEncoding == "UTF8")
                {
                    //dataTable2  matrixzone
                    //   0         1           2           3       4       5       6
                    //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                    //
                    //dataTable3  calladd
                    //   0      1     2      3
                    //address  cd  offset  bits
                    address = dataTable2.Rows[0][0].ToString();
                    cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                    cmd.ExecuteNonQuery();
                    for (int x = 0; x < dataTable3.Rows.Count; x++)
                    {
                        if (address == dataTable3.Rows[x][0].ToString())
                        {
                            if (dataTable3.Rows[x][3].ToString() == "32")
                            {
                                cmd.CommandText = "update calladd set offset = "
                                    + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                    + " where address ='" + address
                                    + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = "update calladd set offset = "
                                    + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                    + " where address ='" + address
                                    + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    iLen += (int)dataTable2.Rows[0][3];
                    for (int i = 1; i < dataTable2.Rows.Count; i++)
                    {
                        iLen += 1;
                        address = dataTable2.Rows[i][0].ToString();
                        zoneaddress = BaseAddress + iLen;
                        cmd.CommandText = "update matrixzone set zoneaddress = '" + zoneaddress.ToString("X8") + "' where address ='" + address + "'";
                        cmd.ExecuteNonQuery();
                        for (int x = 0; x < dataTable3.Rows.Count; x++)
                        {
                            if (address == dataTable3.Rows[x][0].ToString())
                            {
                                if (dataTable3.Rows[x][3].ToString() == "32")
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd32(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '32'";
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = "update calladd set offset = "
                                        + OffSetAdd64(zoneaddress, dataTable3.Rows[x][1].ToString())
                                        + " where address ='" + address
                                        + "' and cd = '" + dataTable3.Rows[x][1].ToString() + "' and bits = '64'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        iLen += (int)dataTable2.Rows[i][3];
                    }
                }
                freelong = addlong - iLen;
            }
            else
            {
                freelong = addlong;
            }
            cmd.CommandText = "update matrix set freelong = " + freelong + " where mataddress ='" + mataddress + "'";
            cmd.ExecuteNonQuery();
            cmd.Transaction.Commit();
            MyAccess.Close();
        }

        private void button3_Click(object sender, EventArgs e)//清除地址
        {
            string str1 = mainform.obMS[0].ToString();
            if ((bool)mainform.obMS[16] == true)//13 Delphi,15 Unicode,17 codepage,18 delphicodepage,16 zonebl
            {
                //注意：如果是第一个矩阵区域，那么批量挪移也会用到
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                cmd.Transaction = MyAccess.BeginTransaction();
                cmd.CommandText = "update athenaa set zonebl = 0 where address ='" + str1 + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "delete from calladd where address = '" + str1 + "'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "delete from matrixzone where address = '" + str1 + "'";
                cmd.ExecuteNonQuery();
                if (dataGridView1.Rows[0].Cells[0].Value.ToString() == textBox3.Text)//所在区域 mataddress mataddressIndex
                {
                    cmd.CommandText = "delete from Resolve32 where address = '" + str1 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from Resolve64 where address = '" + str1 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from ResolveAddress where address = '" + str1 + "'";
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
                MyAccess.Close();
                UpdateMatrixData();
                dataGridView1.Rows[mataddressIndex].Cells[2].Value = freelong;
                //
                if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)//13 Delphi,15 Unicode,17 codepage,18 delphicodepage
                {
                    groupBox1.Enabled = true;
                }
                textBox2.Clear();
                textBox3.Clear();
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                mainform.obMS[16] = false;
                MessageBox.Show("矩阵地址清除成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            int i1 = listBox1.Items.Count;
            if (i1 > 0)
            {
                i1--;
                string s = "";
                for (int i = 0; i < i1; i++)
                {
                    s = s + listBox1.Items[i].ToString() + "\r\n";
                }
                s = s + listBox1.Items[i1].ToString();
                Clipboard.SetText(s);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            int i1 = listBox2.Items.Count;
            if (i1 > 0)
            {
                i1--;
                string s = "";
                for (int i = 0; i < i1; i++)
                {
                    s = s + listBox2.Items[i].ToString() + "\r\n";
                }
                s = s + listBox2.Items[i1].ToString();
                Clipboard.SetText(s);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 1)
            {
                mainform.MoveAddressHexView(listBox1.SelectedItems[0].ToString(), 4);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count == 1)
            {
                mainform.MoveAddressHexView(listBox2.SelectedItems[0].ToString(), 4);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(textBox2.Text);
            }
            catch
            { }
        }

        private void label6_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(textBox3.Text);
            }
            catch
            { }
        }
    }
}
