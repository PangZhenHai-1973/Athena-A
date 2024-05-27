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
    public partial class MoveString : Form
    {
        static ArrayList list = new ArrayList();
        static ArrayList tralt = new ArrayList();//中文长度临时列表
        static int traint = 0;//中文长度临时数据
        bool bl = false;

        public MoveString()
        {
            InitializeComponent();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0)
            {
                textBox1.Text = listBox1.Items[i].ToString();
                traint = int.Parse(tralt[i].ToString());
                if (listBox2.Items.Count > 0 || listBox3.Items.Count > 0)
                {
                    button2.Enabled = true;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = listBox1.SelectedIndex;
            if (i >= 0)
            {
                textBox3.Text = list[i].ToString();
            }
        }

        private void MoveString_Shown(object sender, EventArgs e)//显示窗口时的事件
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (mainform.MyDpi > 96F)
            {
                label3.Location = new Point(label3.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label3.Height / 2D));
                label4.Location = new Point(label4.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label4.Height / 2D));
                label5.Location = new Point(label5.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - label5.Height / 2D));
            }
            textBox1.Text = mainform.obMS[8].ToString();
            textBox2.Text = mainform.obMS[4].ToString();
            textBox3.Text = mainform.obMS[9].ToString();
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
            if (mainform.obMS[8].ToString() != "")
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
            bl = true;
        }

        private void button3_Click(object sender, EventArgs e)//清除地址
        {
            if ((int)mainform.obMS[9] > 0)
            {
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                try
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.CommandText = "update athenaa set outadd = '',outaddfree = 0 where address ='" + mainform.obMS[0].ToString() + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update athenaa set addr = '',addrlong = 0 where address ='" + mainform.obMS[8].ToString() + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from calladd where address = '" + mainform.obMS[0].ToString() + "'";
                    cmd.ExecuteNonQuery();
                    if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)
                    {
                        groupBox1.Enabled = true;
                        cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where address = '" + mainform.obMS[0].ToString() + "'";
                        cmd.ExecuteNonQuery();
                        mainform.obMS[18] = 0;
                    }
                    cmd.Transaction.Commit();
                    textBox1.Clear();
                    textBox3.Clear();
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    listBox3.Items.Clear();
                    mainform.obMS[8] = "";
                    mainform.obMS[9] = 0;
                    MessageBox.Show("迁移地址清除成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show("迁移地址清除失败，请重新打开这个窗口再试一次。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    MyAccess.Close();
                }
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

        private void button2_Click(object sender, EventArgs e)//应用
        {
            int iL2 = listBox2.Items.Count;
            int iL3 = listBox3.Items.Count;
            string s1 = mainform.obMS[0].ToString();
            string s2 = textBox1.Text;
            if (iL2 == 0 && iL3 == 0)
            {
                MessageBox.Show("没有检索到调用地址，无法实现字符串挪移功能。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                long l1 = CommonCode.HexToLong(s2) + traint;
                if (mainform.StrLenCategory == "无")
                {
                    if ((bool)mainform.obMS[15] == true)//Unicode
                    {
                        l1 = l1 + 2;
                    }
                    else
                    {
                        l1 = l1 + 1;
                    }
                }
                else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                {
                    if ((bool)mainform.obMS[15] == true)
                    {
                        l1 = l1 + 6;
                    }
                    else
                    {
                        l1 = l1 + 1;
                    }
                }
                else if (mainform.StrLenCategory == "Delphi")
                {
                    if (radioButton1.Checked == true)
                    {
                        if ((bool)mainform.obMS[15] == true && (bool)mainform.obMS[13] == true)
                        {
                            l1 = l1 + 10;
                        }
                        else if ((bool)mainform.obMS[13] == true && (bool)mainform.obMS[15] == false)
                        {
                            l1 = l1 + 9;
                        }
                        else
                        {
                            l1 = l1 + 1;
                        }
                    }
                    else
                    {
                        if ((bool)mainform.obMS[15] == true && (bool)mainform.obMS[13] == true)
                        {
                            l1 = l1 + 14;
                        }
                        else if ((bool)mainform.obMS[13] == true && (bool)mainform.obMS[15] == false)
                        {
                            l1 = l1 + 13;
                        }
                        else
                        {
                            l1 = l1 + 5;
                        }
                    }
                }
                int i9 = int.Parse(textBox3.Text);
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                try
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.CommandText = "update athenaa set Ignoretra = 0 where address = '" + s1 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update athenaa set Ignoretra = 0 where address = '" + s2 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from IgnoreTra where address = '" + s1 + "' or address = '" + s2 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from calladd where address='" + s1 + "'";
                    cmd.ExecuteNonQuery();
                    for (int i = 0; i < iL2; i++)
                    {
                        cmd.CommandText = "Insert Into calladd (address,cd,offset,bits) Values ('"
                            + s1 + "','"
                            + listBox2.Items[i].ToString() + "',"
                            + OffSetAdd32(l1, listBox2.Items[i].ToString()) + ",'32')";
                        cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i < iL3; i++)
                    {
                        cmd.CommandText = "Insert Into calladd (address,cd,offset,bits) Values ('"
                            + s1 + "','"
                            + listBox3.Items[i].ToString() + "',"
                            + OffSetAdd64(l1, listBox3.Items[i].ToString()) + ",'64')";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.CommandText = "update athenaa set outadd = '" + s2 + "',outaddfree = " + i9 + " where address = '" + s1 + "'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update athenaa set addr = '" + mainform.obMS[0].ToString() + "', addrlong = " + (int)mainform.obMS[4] + " where address = '" + s2 + "'";
                    cmd.ExecuteNonQuery();
                    //   0       1     2       3         4        5        6        7       8          9          10            11
                    //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                    //   12         13      14      15      16        17            18           19          20          21
                    //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                    //            实际是                           没有使用        0 无
                    //           长度标识                                         1 不变
                    if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)
                    {
                        if (radioButton1.Checked == true)
                        {
                            cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where address = '" + mainform.obMS[0].ToString() + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "update fileinfo set detail = '0' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = 0;
                            mainform.DelphiCodePage = "0";
                        }
                        else if (radioButton2.Checked == true)
                        {
                            cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = 1 where address = '" + mainform.obMS[0].ToString() + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "update fileinfo set detail = '1' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = 1;
                            mainform.DelphiCodePage = "1";
                        }
                        else
                        {
                            cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = " + mainform.ProTraCode + " where address = '" + mainform.obMS[0].ToString() + "'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "update fileinfo set detail = '" + mainform.ProTraCode.ToString() + "' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = mainform.ProTraCode;
                            mainform.DelphiCodePage = mainform.ProTraCode.ToString();
                        }
                    }
                    cmd.Transaction.Commit();
                    groupBox1.Enabled = false;
                    button2.Enabled = false;
                    mainform.obMS[8] = s2;
                    mainform.obMS[9] = i9;
                }
                catch (Exception MyEx)
                {
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    MyAccess.Close();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void listBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox3.SelectedItems.Count;
                if (i1 > 0)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        listBox3.Items.RemoveAt(listBox3.SelectedIndex);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//检索地址
        {
            if ((int)mainform.obMS[9] > 0)
            {
                MessageBox.Show("已经在地址“" + mainform.obMS[8].ToString() + "”处设置了挪移，请先清除此处的挪移。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (File.Exists(mainform.FilePath) == false)
                {
                    MessageBox.Show("没有找到英文版文件，请查看工程属性来了解相关信息。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    listBox1.Enabled = false;
                    listBox2.Enabled = false;
                    listBox3.Enabled = false;
                    textBox1.Enabled = false;
                    textBox2.Enabled = false;
                    textBox3.Enabled = false;
                    listBox1.Items.Clear();
                    tralt.Clear();
                    list.Clear();
                    groupBox1.Enabled = false;
                    groupBox2.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            ConcurrentDictionary<string, int> RefAddress = new ConcurrentDictionary<string, int>();
            if (radioButton4.Checked == true)
            {
                listBox2.Items.Clear();
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
                        listBox2.Items.Add(RefAddressStr[y]);
                    }
                }
                if (mainform.StrLenCategory == "无")
                {
                    i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                }
                else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                {
                    if ((bool)mainform.obMS[13] == true && (bool)mainform.obMS[15] == true)
                    {
                        i5 = (int)mainform.obMS[4] + 4;//获得当前中文字符串长度Unicode
                    }
                    else
                    {
                        i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                    }
                }
                else if (mainform.StrLenCategory == "Delphi")
                {
                    if ((bool)mainform.obMS[13] == true)
                    {
                        if (radioButton1.Checked == true)
                        {
                            i5 = (int)mainform.obMS[4] + 8;//获得当前中文字符串长度Unicode
                        }
                        else
                        {
                            i5 = (int)mainform.obMS[4] + 12;
                        }
                    }
                    else
                    {
                        i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                    }
                }
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                cmd.CommandText = "select * from athenaa where free > " + i5 + " and addr = '' and tralong > 0 and outadd = '' and moveforward = 0 and movebackward = 0 and zonebl = 0 ORDER BY address ASC";
                dataTable1.Clear();
                ad.Fill(dataTable1);
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    listBox1.BeginUpdate();
                    for (int i = 0; i < i1; i++)
                    {
                        if (dataTable1.Rows[i][0].ToString() != mainform.obMS[0].ToString())
                        {
                            listBox1.Items.Add(dataTable1.Rows[i][0].ToString());
                            list.Add(dataTable1.Rows[i][7].ToString());
                            tralt.Add(dataTable1.Rows[i][4].ToString());
                        }
                    }
                    listBox1.EndUpdate();
                }
                MyAccess.Close();
            }
            else
            {
                listBox3.Items.Clear();
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
                        listBox3.Items.Add(RefAddressStr[y]);
                    }
                }
                if (mainform.StrLenCategory == "无")
                {
                    i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                }
                else if (mainform.StrLenCategory == "标准" || mainform.StrLenCategory == "标准2")
                {
                    if ((bool)mainform.obMS[13] == true && (bool)mainform.obMS[15] == true)
                    {
                        i5 = (int)mainform.obMS[4] + 4;//获得当前中文字符串长度Unicode
                    }
                    else
                    {
                        i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                    }
                }
                else if (mainform.StrLenCategory == "Delphi")
                {
                    if ((bool)mainform.obMS[13] == true)
                    {
                        if (radioButton1.Checked == true)
                        {
                            i5 = (int)mainform.obMS[4] + 8;//获得当前中文字符串长度Unicode
                        }
                        else
                        {
                            i5 = (int)mainform.obMS[4] + 12;
                        }
                    }
                    else
                    {
                        i5 = (int)mainform.obMS[4];//获得当前中文字符串长度
                    }
                }
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                cmd.CommandText = "select * from athenaa where free > " + i5 + " and addr = '' and tralong > 0 and outadd = '' and moveforward = 0 and movebackward = 0 and zonebl = 0 ORDER BY address ASC";
                ad.SelectCommand = cmd;
                dataTable1.Clear();
                ad.Fill(dataTable1);
                x = dataTable1.Rows.Count;
                if (x > 0)
                {
                    listBox1.BeginUpdate();
                    for (int i = 0; i < x; i++)
                    {
                        if (dataTable1.Rows[i][0].ToString() != mainform.obMS[0].ToString())
                        {
                            listBox1.Items.Add(dataTable1.Rows[i][0].ToString());
                            list.Add(dataTable1.Rows[i][7].ToString());
                            tralt.Add(dataTable1.Rows[i][4].ToString());
                        }
                    }
                    listBox1.EndUpdate();
                }
                MyAccess.Close();
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (textBox1.Text != "")
            {
                button2.Enabled = true;
            }
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            label5.Enabled = true;
            label6.Enabled = true;
            listBox1.Enabled = true;
            listBox2.Enabled = true;
            listBox3.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            if (mainform.StrLenCategory == "Delphi" && (bool)mainform.obMS[13] == true)
            {
                groupBox1.Enabled = true;
            }
            groupBox2.Enabled = true;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (bl == true)
            {
                listBox1.Items.Clear();
                textBox1.Clear();
                textBox3.Clear();
                button2.Enabled = false;
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

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            int i1 = listBox3.Items.Count;
            if (i1 > 0)
            {
                i1--;
                string s = "";
                for (int i = 0; i < i1; i++)
                {
                    s = s + listBox3.Items[i].ToString() + "\r\n";
                }
                s = s + listBox3.Items[i1].ToString();
                Clipboard.SetText(s);
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItems.Count == 1)
            {
                mainform.MoveAddressHexView(listBox2.SelectedItems[0].ToString(), 4);
            }
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox3.SelectedItems.Count == 1)
            {
                mainform.MoveAddressHexView(listBox3.SelectedItems[0].ToString(), 4);
            }
        }
    }
}
