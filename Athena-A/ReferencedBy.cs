using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
//using System.Diagnostics;

namespace Athena_A
{
    public partial class ReferencedBy : Form
    {
        public ReferencedBy()
        {
            InitializeComponent();
        }

        private void ReferencedBy_Shown(object sender, EventArgs e)
        {
            if (mainform.CPUType == "32")
            {
                radioButton4.Checked = true;
            }
            else
            {
                radioButton5.Checked = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.FilePath) == false)
            {
                MessageBox.Show("没有找到英文版文件，请查看工程属性来了解相关信息。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                label2.Enabled = false;
                label4.Enabled = false;
                listBox1.Enabled = false;
                listBox2.Enabled = false;
                button1.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                groupBox2.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
            }
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
            label2.Enabled = true;
            label4.Enabled = true;
            listBox1.Enabled = true;
            listBox2.Enabled = true;
            button1.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            groupBox2.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
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
    }
}
