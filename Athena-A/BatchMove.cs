using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class BatchMove : Form
    {
        private bool dGV = true;

        public BatchMove()
        {
            InitializeComponent();
        }

        private void BatchMove_Shown(object sender, EventArgs e)
        {
            //dataTable2  matrixzone
            //   0         1           2           3       4       5       6
            //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
            //
            //dataTable4  ResolveAddress
            //   0      1    2          4
            //address  x32  x64  codepage  SelectedAddress
            //
            //dataTable7  athenaa
            //   0        1       2       3      4        5            6            7
            //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
            contextMenuStrip1.Font = this.Font;
            comboBox1.Text = "全部";
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        dataSet1.Clear();
                        cmd.CommandText = "select * from matrix";
                        ad.Fill(dataTable1);
                        cmd.CommandText = "select * from ResolveAddress";
                        ad.Fill(dataTable4);
                        cmd.CommandText = "select * from Resolve32";
                        ad.Fill(dataTable5);
                        cmd.CommandText = "select * from Resolve64";
                        ad.Fill(dataTable6);
                    }
                }
            }
            if (mainform.CPUType == "32")
            {
                radioButton4.Checked = true;
            }
            else
            {
                radioButton5.Checked = true;
            }
            if (mainform.StrLenCategory == "无" || mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
            {
                groupBox1.Enabled = false;
            }
            else if (mainform.StrLenCategory == "Delphi")
            {
                if (mainform.ProEncoding == "Unicode")
                {
                    radioButton3.Enabled = false;
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
            }
            int i1 = dataTable4.Rows.Count;
            if (i1 > 0)
            {
                dGV = false;
                for (int i = 0; i < i1; i++)
                {
                    dataGridView1.Rows.Add(dataTable4.Rows[i].ItemArray);
                }
                string s1 = dataTable4.Rows[0][0].ToString();
                i1 = dataTable5.Rows.Count;
                for (int i = 0; i < i1; i++)
                {
                    if (s1 == dataTable5.Rows[i][0].ToString())
                    {
                        listBox1.Items.Add(dataTable5.Rows[i][1].ToString());
                    }
                }
                i1 = dataTable6.Rows.Count;
                for (int i = 0; i < i1; i++)
                {
                    if (s1 == dataTable6.Rows[i][0].ToString())
                    {
                        listBox2.Items.Add(dataTable6.Rows[i][1].ToString());
                    }
                }
                s1 = dataTable4.Rows[0][3].ToString();
                if (s1 == "无")
                {
                    radioButton1.Checked = true;
                    label8.Text = "简体中文(936)";
                    label7.Text = "936";
                }
                else if (s1 == "不变")
                {
                    radioButton2.Checked = true;
                    label8.Text = "简体中文(936)";
                    label7.Text = "936";
                }
                else
                {
                    radioButton3.Checked = true;
                    if (s1 == "简体中文(936)")
                    {
                        label8.Text = "简体中文(936)";
                        label7.Text = "936";
                    }
                    else if (s1 == "繁体中文(950)")
                    {
                        label8.Text = "繁体中文(950)";
                        label7.Text = "950";
                    }
                    else if (s1 == "默认")
                    {
                        label8.Text = "默认";
                        label7.Text = mainform.ProTraCode.ToString();
                    }
                }
                dGV = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button4.Enabled = true;
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(mainform.FilePath) == false)
            {
                MessageBox.Show("没有找到英文版文件，请查看工程属性来了解相关信息。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (dataTable1.Rows.Count == 0)
            {
                MessageBox.Show("请首先使用编辑矩阵菜单设定矩阵区域。\r\n如果是在建立工程以后扩展了 PE 段，\r\n请先在工程属性中重读 PE 段。\r\n批量挪移只使用设定的第一个区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                dGV = false;
                int i1 = dataTable4.Rows.Count;
                if (i1 > dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows.Clear();
                    object[] ob = new object[7];
                    ob[6] = "";
                    for (int i = 0; i < i1; i++)
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
                else
                {
                    for (int i = 0; i < i1; i++)
                    {
                        dataGridView1.Rows[i].Cells[6].Value = "";
                    }
                }
                if (radioButton4.Checked)
                {
                    listBox1.Items.Clear();
                    for (int i = 0; i < i1; i++)
                    {
                        dataGridView1.Rows[i].Cells[1].Value = "";
                    }
                }
                else
                {
                    listBox2.Items.Clear();
                    for (int i = 0; i < i1; i++)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = "";
                    }
                }
                groupBox1.Enabled = false;
                groupBox2.Enabled = false;
                label1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                textBox1.Enabled = false;
                listBox1.Enabled = false;
                listBox2.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                button7.Enabled = false;
                comboBox1.Enabled = false;
                timer1.Enabled = true;
                backgroundWorker2.RunWorkerAsync();
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            int i1 = dataTable4.Rows.Count;
            ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
            if (radioButton4.Checked)
            {
                dataTable5.Rows.Clear();
                int i5 = mainform.l1.Count;//文件头信息总行数
                long l6 = 0;
                long l7 = 0;
                byte[][] VirtualAddressBytes = new byte[i1][];
                for (int i = 0; i < i1; i++)
                {
                    VirtualAddressBytes[i] = BitConverter.GetBytes(CommonCode.GetVirtualAddress(CommonCode.HexToLong(dataTable4.Rows[i][0].ToString())));
                }
                l6 = long.Parse(mainform.l4[3].ToString());
                if (long.Parse(mainform.l1[2].ToString()) == 0)//是否是有资源段
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
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            long StartLong = l6;
                            long EndLong = StartLong + LongSplit + 7;
                            fs.Seek(StartLong, SeekOrigin.Begin);
                        AA: byte xb0 = br.ReadByte();
                            byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            for (int y = 0; y < i1; y++)
                            {
                                if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                {
                                    RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb0 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb1 && VirtualAddressBytes[y][1] == xb2 && VirtualAddressBytes[y][2] == xb3 && VirtualAddressBytes[y][3] == xb0)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb1 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb2 && VirtualAddressBytes[y][1] == xb3 && VirtualAddressBytes[y][2] == xb0 && VirtualAddressBytes[y][3] == xb1)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb3 && VirtualAddressBytes[y][1] == xb0 && VirtualAddressBytes[y][2] == xb1 && VirtualAddressBytes[y][3] == xb2)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            long StartLong = l6 + LongSplit;
                            long EndLong = StartLong + LongSplit + 7;
                            fs.Seek(StartLong, SeekOrigin.Begin);
                        AA: byte xb0 = br.ReadByte();
                            byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            for (int y = 0; y < i1; y++)
                            {
                                if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                {
                                    RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb0 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb1 && VirtualAddressBytes[y][1] == xb2 && VirtualAddressBytes[y][2] == xb3 && VirtualAddressBytes[y][3] == xb0)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb1 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb2 && VirtualAddressBytes[y][1] == xb3 && VirtualAddressBytes[y][2] == xb0 && VirtualAddressBytes[y][3] == xb1)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb3 && VirtualAddressBytes[y][1] == xb0 && VirtualAddressBytes[y][2] == xb1 && VirtualAddressBytes[y][3] == xb2)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                },
                () =>
                {
                    using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            long StartLong = l6 + LongSplit * 2;
                            long EndLong = l7;
                            fs.Seek(StartLong, SeekOrigin.Begin);
                        AA: byte xb0 = br.ReadByte();
                            byte xb1 = br.ReadByte();
                            byte xb2 = br.ReadByte();
                            byte xb3 = br.ReadByte();
                            for (int y = 0; y < i1; y++)
                            {
                                if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                {
                                    RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                    goto AA;
                                }
                            }
                            while (fs.Position < EndLong)
                            {
                                xb0 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb1 && VirtualAddressBytes[y][1] == xb2 && VirtualAddressBytes[y][2] == xb3 && VirtualAddressBytes[y][3] == xb0)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb1 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb2 && VirtualAddressBytes[y][1] == xb3 && VirtualAddressBytes[y][2] == xb0 && VirtualAddressBytes[y][3] == xb1)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb2 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb3 && VirtualAddressBytes[y][1] == xb0 && VirtualAddressBytes[y][2] == xb1 && VirtualAddressBytes[y][3] == xb2)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                                xb3 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (VirtualAddressBytes[y][0] == xb0 && VirtualAddressBytes[y][1] == xb1 && VirtualAddressBytes[y][2] == xb2 && VirtualAddressBytes[y][3] == xb3)
                                    {
                                        RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                        goto AA;
                                    }
                                }
                            }
                        }
                    }
                });
                int i2 = RefAddress.Count;
                int i3 = 0;
                string[] strSplit = new string[i2];
                RefAddress.Keys.CopyTo(strSplit, 0);
                Array.Sort(strSplit);
                object[] ob = new object[2];
                for (int i = 0; i < i2; i++)
                {
                    string[] strTmp = strSplit[i].Split(' ');
                    i3 = int.Parse(strTmp[0]);
                    ob[0] = dataTable4.Rows[i3][0];
                    ob[1] = strTmp[1];
                    dataTable5.Rows.Add(ob);
                    string s1 = dataGridView1.Rows[i3].Cells[1].Value.ToString();
                    if (s1 == "")
                    {
                        dataGridView1.Rows[i3].Cells[6].Value = "1";
                        dataTable4.Rows[i3][1] = "1";
                        dataGridView1.Rows[i3].Cells[1].Value = "1";
                    }
                    else
                    {
                        s1 = (int.Parse(s1) + 1).ToString();
                        dataTable4.Rows[i3][1] = s1;
                        dataGridView1.Rows[i3].Cells[1].Value = s1;
                    }
                }
            }
            else
            {
                dataTable6.Rows.Clear();
                int x = 0;//最大搜索段
                int i5 = mainform.l1.Count;//文件头信息总行数
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
                                long V_StartLong = CommonCode.GetVirtualAddress(StartLong);
                                long[] VC_Long = new long[i1];
                                for (int i = 0; i < i1; i++)
                                {
                                    VC_Long[i] = CommonCode.GetVirtualAddress(CommonCode.HexToLong(dataTable4.Rows[i][0].ToString())) - V_StartLong - 4;
                                }
                            AA: byte xb1 = br.ReadByte();
                                byte xb2 = br.ReadByte();
                                byte xb3 = br.ReadByte();
                                byte xb4 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                    {
                                        if (VC_Long[y] != 0)
                                        {
                                            RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                            for (int z = 0; z < i1; z++)
                                            {
                                                VC_Long[z] -= 4;
                                            }
                                            goto AA;
                                        }
                                    }
                                }
                                while (fs.Position < EndLong)
                                {
                                    xb1 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb2 == (byte)VC_Long[y] && xb3 == (byte)(VC_Long[y] >> 8) && xb4 == (byte)(VC_Long[y] >> 16) && xb1 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb2 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb3 == (byte)VC_Long[y] && xb4 == (byte)(VC_Long[y] >> 8) && xb1 == (byte)(VC_Long[y] >> 16) && xb2 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb3 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb4 == (byte)VC_Long[y] && xb1 == (byte)(VC_Long[y] >> 8) && xb2 == (byte)(VC_Long[y] >> 16) && xb3 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb4 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
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
                                long V_StartLong = CommonCode.GetVirtualAddress(StartLong);
                                long[] VC_Long = new long[i1];
                                for (int i = 0; i < i1; i++)
                                {
                                    VC_Long[i] = CommonCode.GetVirtualAddress(CommonCode.HexToLong(dataTable4.Rows[i][0].ToString())) - V_StartLong - 4;
                                }
                            AA: byte xb1 = br.ReadByte();
                                byte xb2 = br.ReadByte();
                                byte xb3 = br.ReadByte();
                                byte xb4 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                    {
                                        if (VC_Long[y] != 0)
                                        {
                                            RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                            for (int z = 0; z < i1; z++)
                                            {
                                                VC_Long[z] -= 4;
                                            }
                                            goto AA;
                                        }
                                    }
                                }
                                while (fs.Position < EndLong)
                                {
                                    xb1 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb2 == (byte)VC_Long[y] && xb3 == (byte)(VC_Long[y] >> 8) && xb4 == (byte)(VC_Long[y] >> 16) && xb1 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb2 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb3 == (byte)VC_Long[y] && xb4 == (byte)(VC_Long[y] >> 8) && xb1 == (byte)(VC_Long[y] >> 16) && xb2 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb3 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb4 == (byte)VC_Long[y] && xb1 == (byte)(VC_Long[y] >> 8) && xb2 == (byte)(VC_Long[y] >> 16) && xb3 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb4 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
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
                                long V_StartLong = CommonCode.GetVirtualAddress(StartLong);
                                long[] VC_Long = new long[i1];
                                for (int i = 0; i < i1; i++)
                                {
                                    VC_Long[i] = CommonCode.GetVirtualAddress(CommonCode.HexToLong(dataTable4.Rows[i][0].ToString())) - V_StartLong - 4;
                                }
                            AA: byte xb1 = br.ReadByte();
                                byte xb2 = br.ReadByte();
                                byte xb3 = br.ReadByte();
                                byte xb4 = br.ReadByte();
                                for (int y = 0; y < i1; y++)
                                {
                                    if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                    {
                                        if (VC_Long[y] != 0)
                                        {
                                            RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                            for (int z = 0; z < i1; z++)
                                            {
                                                VC_Long[z] -= 4;
                                            }
                                            goto AA;
                                        }
                                    }
                                }
                                while (fs.Position < EndLong)
                                {
                                    xb1 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb2 == (byte)VC_Long[y] && xb3 == (byte)(VC_Long[y] >> 8) && xb4 == (byte)(VC_Long[y] >> 16) && xb1 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb2 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb3 == (byte)VC_Long[y] && xb4 == (byte)(VC_Long[y] >> 8) && xb1 == (byte)(VC_Long[y] >> 16) && xb2 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb3 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb4 == (byte)VC_Long[y] && xb1 == (byte)(VC_Long[y] >> 8) && xb2 == (byte)(VC_Long[y] >> 16) && xb3 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                    xb4 = br.ReadByte();
                                    for (int y = 0; y < i1; y++)
                                    {
                                        VC_Long[y] -= 1;
                                    }
                                    for (int y = 0; y < i1; y++)
                                    {
                                        if (xb1 == (byte)VC_Long[y] && xb2 == (byte)(VC_Long[y] >> 8) && xb3 == (byte)(VC_Long[y] >> 16) && xb4 == (byte)(VC_Long[y] >> 24))
                                        {
                                            if (VC_Long[y] != 0)
                                            {
                                                RefAddress.TryAdd(y.ToString() + " " + (fs.Position - 4).ToString("X8"), 0);
                                                for (int z = 0; z < i1; z++)
                                                {
                                                    VC_Long[z] -= 4;
                                                }
                                                goto AA;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
                int i2 = RefAddress.Count;
                int i3 = 0;
                string[] strSplit = new string[i2];
                RefAddress.Keys.CopyTo(strSplit, 0);
                Array.Sort(strSplit);
                object[] ob = new object[2];
                for (int i = 0; i < i2; i++)
                {
                    string[] strTmp = strSplit[i].Split(' ');
                    i3 = int.Parse(strTmp[0]);
                    ob[0] = dataTable4.Rows[i3][0];
                    ob[1] = strTmp[1];
                    dataTable6.Rows.Add(ob);
                    string s1 = dataGridView1.Rows[i3].Cells[2].Value.ToString();
                    if (s1 == "")
                    {
                        dataGridView1.Rows[i3].Cells[6].Value = "1";
                        dataTable4.Rows[i3][2] = "1";
                        dataGridView1.Rows[i3].Cells[2].Value = "1";
                    }
                    else
                    {
                        s1 = (int.Parse(s1) + 1).ToString();
                        dataTable4.Rows[i3][2] = s1;
                        dataGridView1.Rows[i3].Cells[2].Value = s1;
                    }
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Enabled = false;
            progressBar1.Value = 0;
            if (mainform.StrLenCategory == "Delphi")
            {
                groupBox1.Enabled = true;
                if (mainform.ProEncoding == "Unicode")
                {
                    radioButton3.Enabled = false;
                }
            }
            groupBox2.Enabled = true;
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            textBox1.Enabled = true;
            listBox1.Enabled = true;
            listBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            comboBox1.Enabled = true;
            string s = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            if (radioButton4.Checked)
            {
                int i1 = dataTable5.Rows.Count;
                if (dataGridView1.CurrentRow.Cells[1].Value.ToString() != "")
                {
                    for (int i = 0; i < i1; i++)
                    {
                        if (s == dataTable5.Rows[i][0].ToString())
                        {
                            listBox1.Items.Add(dataTable5.Rows[i][1].ToString());
                        }
                    }
                }
            }
            else
            {
                int i1 = dataTable6.Rows.Count;
                if (dataGridView1.CurrentRow.Cells[2].Value.ToString() != "")
                {
                    for (int i = 0; i < i1; i++)
                    {
                        if (s == dataTable6.Rows[i][0].ToString())
                        {
                            listBox2.Items.Add(dataTable6.Rows[i][1].ToString());
                        }
                    }
                }
            }
            dGV = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (backgroundWorker2.IsBusy == false)
            {
                if (e.ColumnIndex == 4)
                {
                    if (dataGridView1.SelectedRows[0].Cells[1].Value.ToString() != "" || dataGridView1.SelectedRows[0].Cells[2].Value.ToString() != "")
                    {
                        string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                        if (dataGridView1.SelectedRows[0].Cells[4].Value.ToString() == "0")
                        {
                            dataGridView1.SelectedRows[0].Cells[4].Value = 1;
                            for (int i = 0; i < dataTable4.Rows.Count; i++)
                            {
                                if (s1 == dataTable4.Rows[i][0].ToString())
                                {
                                    dataTable4.Rows[i][4] = 1;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            dataGridView1.SelectedRows[0].Cells[4].Value = 0;
                            for (int i = 0; i < dataTable4.Rows.Count; i++)
                            {
                                if (s1 == dataTable4.Rows[i][0].ToString())
                                {
                                    dataTable4.Rows[i][4] = 0;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int i1 = progressBar1.Value;
            if (i1 == 10)
            {
                progressBar1.Value = 0;
            }
            else
            {
                progressBar1.Value = i1 + 1;
            }
        }

        private void BatchMove_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker2.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //dataTable2  matrixzone
                //   0         1           2           3       4       5       6
                //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                //
                //dataTable4  ResolveAddress
                //   0      1    2      3             4            5
                //address  x32  x64  codepage  SelectedAddress  orglong
                //
                //dataTable7  athenaa
                //   0        1       2       3      4        5            6            7
                //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                dGV = false;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select address,orglong,delphi,ucode,zonebl,codepage,delphicodepage,tralong from athenaa where (free < 0 and outadd = '' and moveforward = 0 and movebackward = 0 and superlong = 0) or zonebl <> 0 ORDER BY address ASC";
                            dataTable7.Clear();
                            ad.Fill(dataTable7);
                            dataGridView1.Rows.Clear();
                            int i1 = dataTable7.Rows.Count;
                            if (i1 > 0)
                            {
                                dataTable4.Rows.Clear();
                                dataTable5.Rows.Clear();
                                dataTable6.Rows.Clear();
                                cmd.CommandText = "select * from ResolveAddress";
                                ad.Fill(dataTable4);
                                cmd.CommandText = "select * from Resolve32";
                                ad.Fill(dataTable5);
                                cmd.CommandText = "select * from Resolve64";
                                ad.Fill(dataTable6);
                                int i2 = dataTable4.Rows.Count;
                                bool bl7 = true;
                                for (int i = i1 - 1; i >= 0; i--)
                                {
                                    if ((int)dataTable7.Rows[i][4] == 1)
                                    {
                                        bl7 = true;
                                        for (int y = 0; y < i2; y++)
                                        {
                                            if (dataTable4.Rows[y][0].ToString() == dataTable7.Rows[i][0].ToString())
                                            {
                                                bl7 = false;
                                                break;
                                            }
                                        }
                                        if (bl7)
                                        {
                                            dataTable7.Rows.RemoveAt(i);
                                        }
                                    }
                                }
                                object[] obv = new object[7];
                                object[] ob4 = new object[6];
                                ob4[1] = ob4[2] = obv[1] = obv[2] = obv[6] = "";
                                ob4[4] = obv[4] = 0;
                                i1 = dataTable7.Rows.Count;
                                dataTable4.Rows.Clear();
                                dataTable5.Rows.Clear();
                                dataTable6.Rows.Clear();
                                //dataTable7  athenaa
                                //   0        1       2       3      4        5            6            7
                                //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                                if (mainform.StrLenCategory == "无" || mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                                {
                                    for (int i = 0; i < i1; i++)
                                    {
                                        ob4[0] = obv[0] = dataTable7.Rows[i][0];
                                        ob4[5] = obv[5] = dataTable7.Rows[i][1];
                                        ob4[3] = obv[3] = "无";
                                        dataGridView1.Rows.Add(obv);
                                        dataTable4.Rows.Add(ob4);
                                    }
                                }
                                else if (mainform.StrLenCategory == "Delphi")
                                {
                                    for (int i = 0; i < i1; i++)
                                    {
                                        ob4[0] = obv[0] = dataTable7.Rows[i][0];
                                        ob4[5] = obv[5] = dataTable7.Rows[i][1];
                                        if ((int)dataTable7.Rows[i][2] == 1)
                                        {
                                            if ((int)dataTable7.Rows[i][3] == 1)//Unicode
                                            {
                                                if (dataTable7.Rows[i][6].ToString() == "1")
                                                {
                                                    ob4[3] = obv[3] = "不变";
                                                }
                                                else
                                                {
                                                    ob4[3] = obv[3] = "无";
                                                }
                                            }
                                            else
                                            {
                                                if (dataTable7.Rows[i][6].ToString() == "0")
                                                {
                                                    if (mainform.ProTraName == "默认")
                                                    {
                                                        ob4[3] = obv[3] = "默认";
                                                    }
                                                    else
                                                    {
                                                        ob4[3] = obv[3] = "无";
                                                    }
                                                }
                                                else if (dataTable7.Rows[i][6].ToString() == "1")
                                                {
                                                    ob4[3] = obv[3] = "不变";
                                                }
                                                else if (dataTable7.Rows[i][6].ToString() == "936")
                                                {
                                                    ob4[3] = obv[3] = "简体中文(936)";
                                                }
                                                else if (dataTable7.Rows[i][6].ToString() == "950")
                                                {
                                                    ob4[3] = obv[3] = "繁体中文(950)";
                                                }
                                                else if (dataTable7.Rows[i][6].ToString() == "932")
                                                {
                                                    ob4[3] = obv[3] = "日文(932)";
                                                }
                                                else if (dataTable7.Rows[i][6].ToString() == "949")
                                                {
                                                    ob4[3] = obv[3] = "韩文(949)";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ob4[3] = obv[3] = "无";
                                        }
                                        dataGridView1.Rows.Add(obv);
                                        dataTable4.Rows.Add(ob4);
                                    }
                                }
                                button1.Enabled = true;
                                listBox1.Items.Clear();
                                listBox2.Items.Clear();
                            }
                            else
                            {
                                button1.Enabled = false;
                                MessageBox.Show("没有搜索到需要挪移的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                dGV = true;
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dGV == true && backgroundWorker2.IsBusy == false)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                int i1 = dataTable5.Rows.Count;
                string s1 = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                if (dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() != "")
                {
                    for (int i = 0; i < i1; i++)
                    {
                        if (s1 == dataTable5.Rows[i][0].ToString())
                        {
                            listBox1.Items.Add(dataTable5.Rows[i][1].ToString());
                        }
                    }
                }
                i1 = dataTable6.Rows.Count;
                if (dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString() != "")
                {
                    for (int i = 0; i < i1; i++)
                    {
                        if (s1 == dataTable6.Rows[i][0].ToString())
                        {
                            listBox2.Items.Add(dataTable6.Rows[i][1].ToString());
                        }
                    }
                }
                string s2 = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                if (mainform.StrLenCategory == "Delphi")
                {
                    if (s2 == "无")
                    {
                        radioButton1.Checked = true;
                    }
                    else if (s2 == "不变")
                    {
                        radioButton2.Checked = true;
                    }
                    else
                    {
                        radioButton3.Checked = true;
                        label8.Text = s2;
                    }
                }
                mainform.MoveAddressHexView(s1, int.Parse(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString()));
            }
        }

        private void button7_Click(object sender, EventArgs e)
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

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ContextMenuStrip = contextMenuStrip1;
                    if (backgroundWorker2.IsBusy == true)
                    {
                        contextMenuStrip1.Enabled = false;
                    }
                    else
                    {
                        contextMenuStrip1.Enabled = true;
                    }
                }
                else
                {
                    dataGridView1.ContextMenuStrip = null;
                }
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox1.SelectedItems.Count;
                int i2 = listBox1.Items.Count - i1;
                if (i1 > 0)
                {
                    string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    string s2 = "";
                    for (int i = 0; i < i1; i++)
                    {
                        s2 = listBox1.Items[listBox1.SelectedIndex].ToString();
                        for (int y = 0; y < dataTable5.Rows.Count; y++)
                        {
                            if (s1 == dataTable5.Rows[y][0].ToString() && s2 == dataTable5.Rows[y][1].ToString())
                            {
                                dataTable5.Rows.RemoveAt(y);
                                break;
                            }
                        }
                        listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                    }
                    if (i2 == 0)
                    {
                        s2 = "";
                    }
                    else
                    {
                        s2 = i2.ToString();
                    }
                    dataGridView1.SelectedRows[0].Cells[1].Value = s2;
                    for (int i = 0; i < dataTable4.Rows.Count; i++)
                    {
                        if (s1 == dataTable4.Rows[i][0].ToString())
                        {
                            dataTable4.Rows[i][1] = s2;
                            if (listBox1.Items.Count == 0 && listBox2.Items.Count == 0)
                            {
                                dataGridView1.SelectedRows[0].Cells[4].Value = 0;
                                dataTable4.Rows[i][4] = 0;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox2.SelectedItems.Count;
                int i2 = listBox2.Items.Count - i1;
                if (i1 > 0)
                {
                    string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    string s2 = "";
                    for (int i = 0; i < i1; i++)
                    {
                        s2 = listBox2.Items[listBox2.SelectedIndex].ToString();
                        for (int y = 0; y < dataTable6.Rows.Count; y++)
                        {
                            if (s1 == dataTable6.Rows[y][0].ToString() && s2 == dataTable6.Rows[y][1].ToString())
                            {
                                dataTable6.Rows.RemoveAt(y);
                                break;
                            }
                        }
                        listBox2.Items.RemoveAt(listBox2.SelectedIndex);
                    }
                    if (i2 == 0)
                    {
                        s2 = "";
                    }
                    else
                    {
                        s2 = i2.ToString();
                    }
                    dataGridView1.SelectedRows[0].Cells[2].Value = s2;
                    for (int i = 0; i < dataTable4.Rows.Count; i++)
                    {
                        if (s1 == dataTable4.Rows[i][0].ToString())
                        {
                            dataTable4.Rows[i][2] = s2;
                            if (listBox1.Items.Count == 0 && listBox2.Items.Count == 0)
                            {
                                dataGridView1.SelectedRows[0].Cells[4].Value = 0;
                                dataTable4.Rows[i][4] = 0;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string s = textBox1.Text.ToUpper();
            int i1 = dataTable4.Rows.Count;
            dataGridView1.Rows.Clear();
            object[] ob = new object[7];
            ob[6] = "";
            if (s == "")
            {
                for (int i = 0; i < i1; i++)
                {
                    ob[0] = dataTable4.Rows[i][0];
                    ob[1] = dataTable4.Rows[i][1];
                    ob[2] = dataTable4.Rows[i][2];
                    ob[3] = dataTable4.Rows[i][3];
                    ob[4] = dataTable4.Rows[i][4];
                    ob[5] = dataTable4.Rows[i][5];
                    dataGridView1.Rows.Add(ob);
                }
            }
            else
            {
                for (int i = 0; i < i1; i++)
                {
                    if (dataTable4.Rows[i][0].ToString().Contains(s))
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
            }
            if (dataGridView1.Rows.Count == 0)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i1 = dataTable4.Rows.Count;
            string com = comboBox1.Text;
            dataGridView1.Rows.Clear();
            object[] ob = new object[7];
            ob[6] = "";
            if (com == "异常")
            {
                for (int i = 0; i < i1; i++)
                {
                    if (!((dataTable4.Rows[i][1].ToString() == "1" && dataTable4.Rows[i][2].ToString() == "") || (dataTable4.Rows[i][1].ToString() == "" && dataTable4.Rows[i][2].ToString() == "1")))
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
            }
            else if (com == "空空")
            {
                for (int i = 0; i < i1; i++)
                {
                    if (dataTable4.Rows[i][1].ToString() == "" && dataTable4.Rows[i][2].ToString() == "")
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
            }
            else if (com == "已选")
            {
                for (int i = 0; i < i1; i++)
                {
                    if ((int)dataTable4.Rows[i][4] == 1)
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
            }
            else if (com == "未选")
            {
                for (int i = 0; i < i1; i++)
                {
                    if ((int)dataTable4.Rows[i][4] == 0)
                    {
                        ob[0] = dataTable4.Rows[i][0];
                        ob[1] = dataTable4.Rows[i][1];
                        ob[2] = dataTable4.Rows[i][2];
                        ob[3] = dataTable4.Rows[i][3];
                        ob[4] = dataTable4.Rows[i][4];
                        ob[5] = dataTable4.Rows[i][5];
                        dataGridView1.Rows.Add(ob);
                    }
                }
            }
            else if (com == "全部")
            {
                for (int i = 0; i < i1; i++)
                {
                    ob[0] = dataTable4.Rows[i][0];
                    ob[1] = dataTable4.Rows[i][1];
                    ob[2] = dataTable4.Rows[i][2];
                    ob[3] = dataTable4.Rows[i][3];
                    ob[4] = dataTable4.Rows[i][4];
                    ob[5] = dataTable4.Rows[i][5];
                    dataGridView1.Rows.Add(ob);
                }
            }
            if (dataGridView1.Rows.Count == 0)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
            }
            else
            {
                dataGridView1.Select();
            }
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //dataTable2  matrixzone
            //   0         1           2           3       4       5       6
            //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
            //
            //dataTable4  ResolveAddress
            //   0      1    2          4
            //address  x32  x64  codepage  SelectedAddress
            //
            //dataTable7  athenaa
            //   0        1       2       3      4        5            6            7
            //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
            dataTable4.Rows.Clear();
            dataTable5.Rows.Clear();
            dataTable6.Rows.Clear();
            dataGridView1.Rows.Clear();
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            button1.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 检索ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(mainform.FilePath) == false)
            {
                MessageBox.Show("没有找到英文版文件，请查看工程属性来了解相关信息。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (dataTable1.Rows.Count == 0)
            {
                MessageBox.Show("请首先使用编辑矩阵菜单设定矩阵区域。\r\n如果是在建立工程以后扩展了 PE 段，\r\n请先在工程属性中重读 PE 段。\r\n批量挪移只使用设定的第一个区域。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
                string s1 = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                if (radioButton4.Checked)
                {
                    listBox1.Items.Clear();
                    for (int y = dataTable5.Rows.Count - 1; y >= 0; y--)
                    {
                        if (s1 == dataTable5.Rows[y][0].ToString())
                        {
                            dataTable5.Rows.RemoveAt(y);
                        }
                    }
                    int i5 = mainform.l1.Count;//文件头信息总行数
                    long l6 = 0;
                    long l7 = 0;
                    byte[] b = BitConverter.GetBytes(CommonCode.GetVirtualAddress(CommonCode.HexToLong(s1)));
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
                    int i2 = RefAddress.Count;
                    if (i2 > 0)
                    {
                        dataGridView1.CurrentRow.Cells[1].Value = i2;
                        for (int y = 0; y < dataTable4.Rows.Count; y++)
                        {
                            if (s1 == dataTable4.Rows[y][0].ToString())
                            {
                                dataTable4.Rows[y][1] = i2;
                                break;
                            }
                        }
                        string[] strTmp = new string[i2];
                        RefAddress.Keys.CopyTo(strTmp, 0);
                        Array.Sort(strTmp);
                        object[] ob = new object[2];
                        ob[0] = s1;
                        for (int y = 0; y < i2; y++)
                        {
                            ob[1] = strTmp[y];
                            dataTable5.Rows.Add(ob);
                            listBox1.Items.Add(strTmp[y]);
                        }
                    }
                    else
                    {
                        dataGridView1.CurrentRow.Cells[1].Value = "";
                        for (int y = 0; y < dataTable4.Rows.Count; y++)
                        {
                            if (s1 == dataTable4.Rows[y][0].ToString())
                            {
                                dataTable4.Rows[y][1] = "";
                                break;
                            }
                        }
                    }
                }
                else //64 位
                {
                    listBox2.Items.Clear();
                    for (int y = dataTable6.Rows.Count - 1; y >= 0; y--)
                    {
                        if (s1 == dataTable6.Rows[y][0].ToString())
                        {
                            dataTable6.Rows.RemoveAt(y);
                        }
                    }
                    int x = 0;//最大搜索段
                    int i5 = mainform.l1.Count;//文件头信息总行数
                    long VStr_Long = CommonCode.GetVirtualAddress(CommonCode.HexToLong(s1));//获得当前字符串虚拟地址
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
                    int i2 = RefAddress.Count;
                    if (i2 > 0)
                    {
                        dataGridView1.CurrentRow.Cells[2].Value = i2;
                        for (int y = 0; y < dataTable4.Rows.Count; y++)
                        {
                            if (s1 == dataTable4.Rows[y][0].ToString())
                            {
                                dataTable4.Rows[y][2] = i2;
                                break;
                            }
                        }
                        string[] strTmp = new string[i2];
                        RefAddress.Keys.CopyTo(strTmp, 0);
                        Array.Sort(strTmp);
                        object[] ob = new object[2];
                        ob[0] = s1;
                        for (int y = 0; y < i2; y++)
                        {
                            ob[1] = strTmp[y];
                            dataTable6.Rows.Add(ob);
                            listBox2.Items.Add(strTmp[y]);
                        }
                    }
                    else
                    {
                        dataGridView1.CurrentRow.Cells[2].Value = "";
                        for (int y = 0; y < dataTable4.Rows.Count; y++)
                        {
                            if (s1 == dataTable4.Rows[y][0].ToString())
                            {
                                dataTable4.Rows[y][2] = "";
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    dataGridView1.SelectedRows[0].Cells[3].Value = "无";
                    string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    for (int i = 0; i < dataTable4.Rows.Count; i++)
                    {
                        if (s1 == dataTable4.Rows[i][0].ToString())
                        {
                            dataTable4.Rows[i][3] = "无";
                            break;
                        }
                    }
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    dataGridView1.SelectedRows[0].Cells[3].Value = "不变";
                    string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    for (int i = 0; i < dataTable4.Rows.Count; i++)
                    {
                        if (s1 == dataTable4.Rows[i][0].ToString())
                        {
                            dataTable4.Rows[i][3] = "不变";
                            break;
                        }
                    }
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    dataGridView1.SelectedRows[0].Cells[3].Value = mainform.ProTraName;
                    string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                    for (int i = 0; i < dataTable4.Rows.Count; i++)
                    {
                        if (s1 == dataTable4.Rows[i][0].ToString())
                        {
                            dataTable4.Rows[i][3] = mainform.ProTraName;
                            break;
                        }
                    }
                }
            }
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sx = "";
            for (int x = 0; x < dataGridView1.Rows.Count; x++)
            {
                if (dataGridView1.Rows[x].Cells[1].Value.ToString() != "" || dataGridView1.Rows[x].Cells[2].Value.ToString() != "")
                {
                    sx = dataGridView1.Rows[x].Cells[0].Value.ToString();
                    for (int y = 0; y < dataTable4.Rows.Count; y++)
                    {
                        if (sx == dataTable4.Rows[y][0].ToString())
                        {
                            dataGridView1.Rows[x].Cells[4].Value = 1;
                            dataTable4.Rows[y][4] = 1;
                            break;
                        }
                    }
                }
            }
        }

        private void 反选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sx = "";
            for (int x = 0; x < dataGridView1.Rows.Count; x++)
            {
                if (dataGridView1.Rows[x].Cells[1].Value.ToString() != "" || dataGridView1.Rows[x].Cells[2].Value.ToString() != "")
                {
                    sx = dataGridView1.Rows[x].Cells[0].Value.ToString();
                    for (int y = 0; y < dataTable4.Rows.Count; y++)
                    {
                        if (sx == dataTable4.Rows[y][0].ToString())
                        {
                            if ((int)dataGridView1.Rows[x].Cells[4].Value == 1)
                            {
                                dataGridView1.Rows[x].Cells[4].Value = 0;
                                dataTable4.Rows[y][4] = 0;
                            }
                            else
                            {
                                dataGridView1.Rows[x].Cells[4].Value = 1;
                                dataTable4.Rows[y][4] = 1;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            for (int i = dataTable5.Rows.Count - 1; i >= 0; i--)
            {
                if (s1 == dataTable5.Rows[i][0].ToString())
                {
                    dataTable5.Rows.RemoveAt(i);
                }
            }
            for (int i = dataTable6.Rows.Count - 1; i >= 0; i--)
            {
                if (s1 == dataTable6.Rows[i][0].ToString())
                {
                    dataTable6.Rows.RemoveAt(i);
                }
            }
            for (int i = dataTable4.Rows.Count - 1; i >= 0; i--)
            {
                if (s1 == dataTable4.Rows[i][0].ToString())
                {
                    dataTable4.Rows.RemoveAt(i);
                    break;
                }
            }
            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            if (dataGridView1.Rows.Count == 0)
            {
                button1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(0, SystemColors.Control.G, 0);
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            int iDT4 = dataTable4.Rows.Count;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.Transaction = MyAccess.BeginTransaction();
                        dataTable2.Rows.Clear();//首先清除第一区域的矩阵挪移
                        cmd.CommandText = "select * from matrixzone where mataddress = '" + dataTable1.Rows[0][0].ToString() + "'";
                        ad.Fill(dataTable2);
                        string s1 = "";
                        if (dataTable2.Rows.Count > 0)
                        {
                            for (int i = dataTable2.Rows.Count - 1; i >= 0; i--)
                            {
                                s1 = dataTable2.Rows[i][0].ToString();
                                cmd.CommandText = "update athenaa set zonebl = 0 where address ='" + s1 + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "delete from calladd where address = '" + s1 + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "delete from matrixzone where address = '" + s1 + "'";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        dataTable7.Clear();
                        for (int i = 0; i < iDT4; i++)
                        {
                            cmd.CommandText = "select address,orglong,delphi,ucode,zonebl,codepage,delphicodepage,tralong from athenaa where address = '" + dataTable4.Rows[i][0].ToString() + "'";
                            ad.Fill(dataTable7);
                        }
                        cmd.Transaction.Commit();
                        int iDT7 = dataTable7.Rows.Count;
                        bool bl = true;
                        if (iDT4 != iDT7)//排除已失效的字符串
                        {
                            for (int x = iDT4 - 1; x >= 0; x--)
                            {
                                bl = true;
                                for (int y = 0; y < iDT7; y++)
                                {
                                    if (dataTable4.Rows[x][0].ToString() == dataTable7.Rows[y][0].ToString())
                                    {
                                        bl = false;
                                        break;
                                    }
                                }
                                if (bl)
                                {
                                    s1 = dataTable4.Rows[x][0].ToString();
                                    dataTable4.Rows.RemoveAt(x);
                                    for (int m = dataTable5.Rows.Count - 1; m >= 0; m--)
                                    {
                                        if (s1 == dataTable5.Rows[m][0].ToString())
                                        {
                                            dataTable5.Rows.RemoveAt(m);
                                        }
                                    }
                                    for (int m = dataTable6.Rows.Count - 1; m >= 0; m--)
                                    {
                                        if (s1 == dataTable6.Rows[m][0].ToString())
                                        {
                                            dataTable6.Rows.RemoveAt(m);
                                        }
                                    }
                                    for (int m = dataGridView1.Rows.Count - 1; m >= 0; m--)
                                    {
                                        if (s1 == dataGridView1.Rows[m].Cells[0].Value.ToString())
                                        {
                                            dataGridView1.Rows.RemoveAt(m);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        iDT4 = dataTable4.Rows.Count;
                        dataTable7.Clear();
                        for (int i = 0; i < iDT4; i++)//筛选出需要挪移的条目
                        {
                            if ((int)dataTable4.Rows[i][4] == 1)
                            {
                                cmd.CommandText = "select address,orglong,delphi,ucode,zonebl,codepage,delphicodepage,tralong from athenaa where address = '" + dataTable4.Rows[i][0].ToString() + "'";
                                ad.Fill(dataTable7);
                            }
                        }
                        iDT7 = dataTable7.Rows.Count;
                        //保存代码页设置
                        int i1 = 0;//这时 i1 是代码页
                        int iLen = 0;//累计长度
                        string s2 = "";
                        dataTable2.Rows.Clear();
                        dataTable3.Rows.Clear();
                        if (iDT7 > 0)
                        {
                            if (mainform.StrLenCategory == "Delphi")
                            {
                                cmd.Transaction = MyAccess.BeginTransaction();
                                if (mainform.ProEncoding == "Unicode")
                                {
                                    for (int i = 0; i < iDT4; i++)
                                    {
                                        s1 = dataTable4.Rows[i][0].ToString();
                                        s2 = dataTable4.Rows[i][3].ToString();//新设定的代码页名称
                                        for (int x = 0; x < iDT7; x++)
                                        {
                                            if (s1 == dataTable7.Rows[x][0].ToString())
                                            {
                                                if ((int)dataTable7.Rows[x][2] == 1)
                                                {
                                                    if (s2 == "不变")
                                                    {
                                                        dataTable7.Rows[x][5] = 1;
                                                        dataTable7.Rows[x][6] = 1;
                                                        cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = 1 where address = '" + s1 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        dataTable7.Rows[x][5] = 0;
                                                        dataTable7.Rows[x][6] = 0;
                                                        cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where address = '" + s1 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (mainform.ProEncoding == "ANSI")
                                {
                                    for (int i = 0; i < iDT4; i++)
                                    {
                                        s1 = dataTable4.Rows[i][0].ToString();
                                        s2 = dataTable4.Rows[i][3].ToString();//新设定的代码页名称
                                        for (int x = 0; x < iDT7; x++)
                                        {
                                            if (s1 == dataTable7.Rows[x][0].ToString())
                                            {
                                                if ((int)dataTable7.Rows[x][2] == 1)
                                                {
                                                    if (s2 == "无")
                                                    {
                                                        dataTable7.Rows[x][5] = 0;
                                                        dataTable7.Rows[x][6] = 0;
                                                        cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where address = '" + s1 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else if (s2 == "不变")
                                                    {
                                                        dataTable7.Rows[x][5] = 1;
                                                        dataTable7.Rows[x][6] = 1;
                                                        cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = 1 where address = '" + s1 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    else
                                                    {
                                                        if (s2 == "默认")
                                                        {
                                                            i1 = mainform.ProTraCode;
                                                        }
                                                        else if (s2 == "简体中文(936)")
                                                        {
                                                            i1 = 936;
                                                        }
                                                        else if (s2 == "繁体中文(950)")
                                                        {
                                                            i1 = 950;
                                                        }
                                                        dataTable7.Rows[x][5] = 1;
                                                        dataTable7.Rows[x][6] = i1;
                                                        cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = " + i1 + " where address = '" + s1 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                cmd.Transaction.Commit();
                            }
                            //dataTable2  matrixzone
                            //   0         1           2           3       4       5       6
                            //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                            //
                            //dataTable3  calladd
                            //   0      1     2      3
                            //address  cd  offset  bits
                            //
                            //dataTable4  ResolveAddress
                            //   0      1    2      3             4            5
                            //address  x32  x64  codepage  SelectedAddress  orglong
                            //
                            //dataTable7  athenaa
                            //   0        1       2       3      4        5            6            7
                            //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                            //
                            //估算长度并挪移
                            string address = "";//原始字符串地址
                            string mataddress = dataTable1.Rows[0][0].ToString();//矩阵地址
                            long BaseAddress = CommonCode.HexToLong(mataddress);//矩阵基址
                            long zoneaddress = BaseAddress;//新字符串地址 long
                            object[] matrixzone = new object[7];
                            object[] calladd = new object[4];
                            if (mainform.ProEncoding == "Unicode")
                            {
                                if (mainform.StrLenCategory == "无")//Unicode 无
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
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = mataddress;//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 2;
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
                                    }
                                }
                                else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")//Unicode 标准
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
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    if ((int)dataTable7.Rows[0][2] == 1)
                                    {
                                        iLen += 4;
                                    }
                                    zoneaddress = BaseAddress + iLen;
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 2;
                                        if ((int)dataTable7.Rows[i][2] == 1)
                                        {
                                            iLen += 4;
                                        }
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
                                    }
                                }
                                else if (mainform.StrLenCategory == "Delphi")//Unicode Delphi
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
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    if ((int)dataTable7.Rows[0][2] == 1)
                                    {
                                        iLen += 8;
                                        if ((int)dataTable7.Rows[0][6] > 0)
                                        {
                                            iLen += 4;
                                        }
                                    }
                                    zoneaddress = BaseAddress + iLen;
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 2;
                                        if ((int)dataTable7.Rows[i][2] == 1)
                                        {
                                            iLen += 8;
                                            if ((int)dataTable7.Rows[i][6] > 0)
                                            {
                                                iLen += 4;
                                            }
                                        }
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
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
                                    //
                                    //dataTable7  athenaa
                                    //   0        1       2       3      4        5            6            7
                                    //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = mataddress;//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 1;
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
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
                                    //
                                    //dataTable7  athenaa
                                    //   0        1       2       3      4        5            6            7
                                    //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    if ((int)dataTable7.Rows[0][2] == 1)
                                    {
                                        iLen += 4;
                                    }
                                    zoneaddress = BaseAddress + iLen;
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 1;
                                        if ((int)dataTable7.Rows[i][2] == 1)
                                        {
                                            iLen += 4;
                                        }
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
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
                                    //
                                    //dataTable7  athenaa
                                    //   0        1       2       3      4        5            6            7
                                    //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                                    //address 原始字符串地址
                                    //mataddress 矩阵地址
                                    //zoneaddress 新字符串地址
                                    //BaseAddress 矩阵基址
                                    if ((int)dataTable7.Rows[0][2] == 1)
                                    {
                                        iLen += 8;
                                        if ((int)dataTable7.Rows[0][6] > 0)
                                        {
                                            iLen += 4;
                                        }
                                    }
                                    zoneaddress = BaseAddress + iLen;
                                    address = dataTable7.Rows[0][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[0][2];
                                    matrixzone[5] = dataTable7.Rows[0][3];
                                    matrixzone[6] = dataTable7.Rows[0][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    //
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    //
                                    iLen += (int)dataTable7.Rows[0][7];
                                    //
                                    for (int i = 1; i < dataTable7.Rows.Count; i++)
                                    {
                                        iLen += 1;
                                        if ((int)dataTable7.Rows[i][2] == 1)
                                        {
                                            iLen += 8;
                                            if ((int)dataTable7.Rows[i][6] > 0)
                                            {
                                                iLen += 4;
                                            }
                                        }
                                        address = dataTable7.Rows[i][0].ToString();
                                        matrixzone[0] = address;//原始字符串地址 string
                                        matrixzone[1] = mataddress;//矩阵地址 string
                                        zoneaddress = BaseAddress + iLen;
                                        matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                        matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                        matrixzone[4] = dataTable7.Rows[i][2];
                                        matrixzone[5] = dataTable7.Rows[i][3];
                                        matrixzone[6] = dataTable7.Rows[i][5];
                                        dataTable2.Rows.Add(matrixzone);
                                        calladd[0] = address;
                                        for (int x = 0; x < dataTable5.Rows.Count; x++)
                                        {
                                            if (address == dataTable5.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable5.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                                calladd[3] = "32";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        for (int x = 0; x < dataTable6.Rows.Count; x++)
                                        {
                                            if (address == dataTable6.Rows[x][0].ToString())
                                            {
                                                calladd[1] = dataTable6.Rows[x][1].ToString();
                                                calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                                calladd[3] = "64";
                                                dataTable3.Rows.Add(calladd);
                                            }
                                        }
                                        iLen += (int)dataTable7.Rows[i][7];
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
                                //
                                //dataTable7  athenaa
                                //   0        1       2       3      4        5            6            7
                                //address  orglong  delphi  ucode  zonebl  codepage  delphicodepage  tralong
                                //address 原始字符串地址
                                //mataddress 矩阵地址
                                //zoneaddress 新字符串地址
                                //BaseAddress 矩阵基址
                                address = dataTable7.Rows[0][0].ToString();
                                matrixzone[0] = address;//原始字符串地址 string
                                matrixzone[1] = mataddress;//矩阵地址 string
                                matrixzone[2] = mataddress;//新字符串地址 string
                                matrixzone[3] = dataTable7.Rows[0][7];//长度 int
                                matrixzone[4] = dataTable7.Rows[0][2];
                                matrixzone[5] = dataTable7.Rows[0][3];
                                matrixzone[6] = dataTable7.Rows[0][5];
                                dataTable2.Rows.Add(matrixzone);
                                //
                                calladd[0] = address;
                                for (int x = 0; x < dataTable5.Rows.Count; x++)
                                {
                                    if (address == dataTable5.Rows[x][0].ToString())
                                    {
                                        calladd[1] = dataTable5.Rows[x][1].ToString();
                                        calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                        calladd[3] = "32";
                                        dataTable3.Rows.Add(calladd);
                                    }
                                }
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (address == dataTable6.Rows[x][0].ToString())
                                    {
                                        calladd[1] = dataTable6.Rows[x][1].ToString();
                                        calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                        calladd[3] = "64";
                                        dataTable3.Rows.Add(calladd);
                                    }
                                }
                                //
                                iLen += (int)dataTable7.Rows[0][7];
                                //
                                for (int i = 1; i < dataTable7.Rows.Count; i++)
                                {
                                    iLen += 1;
                                    address = dataTable7.Rows[i][0].ToString();
                                    matrixzone[0] = address;//原始字符串地址 string
                                    matrixzone[1] = mataddress;//矩阵地址 string
                                    zoneaddress = BaseAddress + iLen;
                                    matrixzone[2] = zoneaddress.ToString("X8");//新字符串地址 string
                                    matrixzone[3] = dataTable7.Rows[i][7];//长度 int
                                    matrixzone[4] = dataTable7.Rows[i][2];
                                    matrixzone[5] = dataTable7.Rows[i][3];
                                    matrixzone[6] = dataTable7.Rows[i][5];
                                    dataTable2.Rows.Add(matrixzone);
                                    calladd[0] = address;
                                    for (int x = 0; x < dataTable5.Rows.Count; x++)
                                    {
                                        if (address == dataTable5.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable5.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd32(zoneaddress, dataTable5.Rows[x][1].ToString());
                                            calladd[3] = "32";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    for (int x = 0; x < dataTable6.Rows.Count; x++)
                                    {
                                        if (address == dataTable6.Rows[x][0].ToString())
                                        {
                                            calladd[1] = dataTable6.Rows[x][1].ToString();
                                            calladd[2] = OffSetAdd64(zoneaddress, dataTable6.Rows[x][1].ToString());
                                            calladd[3] = "64";
                                            dataTable3.Rows.Add(calladd);
                                        }
                                    }
                                    iLen += (int)dataTable7.Rows[i][7];
                                }
                            }
                        }
                        cmd.Transaction = MyAccess.BeginTransaction();
                        if (iLen > (int)dataTable1.Rows[0][1])
                        {
                            MessageBox.Show("矩阵中设置的第一个区域长度是 "
                                + dataTable1.Rows[0][1].ToString()
                                + "，而挪移需要用到的长度是 "
                                + iLen + "，可用长度不足。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            for (int i = 0; i < dataTable2.Rows.Count; i++)
                            {
                                cmd.CommandText = "Insert Into matrixzone (address,mataddress,zoneaddress,tralong,delphi,ucode,codepage) Values ('"
                                    + dataTable2.Rows[i][0].ToString() + "','"
                                    + dataTable2.Rows[i][1].ToString() + "','"
                                    + dataTable2.Rows[i][2].ToString() + "',"
                                    + dataTable2.Rows[i][3] + ","
                                    + dataTable2.Rows[i][4] + ","
                                    + dataTable2.Rows[i][5] + ","
                                    + dataTable2.Rows[i][6] + ")";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "update athenaa set zonebl = 1 where address ='" + dataTable2.Rows[i][0].ToString() + "'";
                                cmd.ExecuteNonQuery();
                            }
                            for (int i = 0; i < dataTable3.Rows.Count; i++)
                            {
                                cmd.CommandText = "Insert Into calladd (address,cd,offset,bits) Values ('"
                                    + dataTable3.Rows[i][0].ToString() + "','"
                                    + dataTable3.Rows[i][1].ToString() + "',"
                                    + dataTable3.Rows[i][2] + ",'"
                                    + dataTable3.Rows[i][3].ToString() + "')";
                                cmd.ExecuteNonQuery();
                            }
                            cmd.CommandText = "update matrix set freelong = "
                                + ((int)dataTable1.Rows[0][1] - iLen)
                                + " where mataddress = '"
                                + dataTable1.Rows[0][0].ToString() + "'";
                            cmd.ExecuteNonQuery();
                        }
                        //   0       1     2       3         4        5        6        7       8          9          10            11
                        //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                        //   12         13      14      15      16        17            18           19          20          21
                        //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                        //            实际是                           没有使用        0 无
                        //           长度标识                                         1 不变
                        //
                        //所有准备工作已就绪，接下来清空矩阵中第一个区域的所有数据
                        //
                        //dataTable2  matrixzone
                        //   0         1           2           3       4       5       6
                        //address  mataddress  zoneaddress  tralong  delphi  ucode  codepage
                        cmd.CommandText = "delete from ResolveAddress";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "delete from Resolve32";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "delete from Resolve64";
                        cmd.ExecuteNonQuery();
                        //保存现有数据
                        if (dataTable4.Rows.Count > 0)
                        {
                            for (int i = 0; i < dataTable4.Rows.Count; i++)
                            {
                                cmd.CommandText = "Insert Into ResolveAddress (address,x32,x64,codepage,SelectedAddress,orglong) Values ('"
                                    + dataTable4.Rows[i][0].ToString() + "','"
                                    + dataTable4.Rows[i][1].ToString() + "','"
                                    + dataTable4.Rows[i][2].ToString() + "','"
                                    + dataTable4.Rows[i][3].ToString() + "',"
                                    + (int)dataTable4.Rows[i][4] + ","
                                    + (int)dataTable4.Rows[i][5] + ")";
                                cmd.ExecuteNonQuery();
                            }
                            for (int i = 0; i < dataTable5.Rows.Count; i++)
                            {
                                cmd.CommandText = "Insert Into Resolve32 (address,Raddress) Values ('"
                                    + dataTable5.Rows[i][0].ToString() + "','"
                                    + dataTable5.Rows[i][1].ToString() + "')";
                                cmd.ExecuteNonQuery();
                            }
                            for (int i = 0; i < dataTable6.Rows.Count; i++)
                            {
                                cmd.CommandText = "Insert Into Resolve64 (address,Raddress) Values ('"
                                    + dataTable6.Rows[i][0].ToString() + "','"
                                    + dataTable6.Rows[i][1].ToString() + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.Transaction.Commit();
                    }
                }
            }
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button2.BackColor = SystemColors.Control;
            button2.UseVisualStyleBackColor = true;
        }
    }
}
