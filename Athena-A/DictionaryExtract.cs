using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class DictionaryExtract : Form
    {

        ArrayList list1 = new ArrayList();
        ArrayList list2 = new ArrayList();
        ArrayList list3 = new ArrayList();
        ArrayList list4 = new ArrayList();
        ArrayList list5 = new ArrayList();
        ArrayList list6 = new ArrayList();
        ArrayList al1 = new ArrayList();//地址
        ArrayList al2 = new ArrayList();//地址
        public static string Dictionaryname;//字典名称
        Correct CT = new Correct();
        bool cShowBool = false;
        bool DoneBool = false;

        public DictionaryExtract()
        {
            InitializeComponent();
        }

        private bool File_Length(string s1, string s2)//比较输入的地址是否超出文件的大小
        {
            FileStream f = new FileStream(s1, FileMode.Open, FileAccess.Read);
            long l = f.Length - 1;
            long l2 = 0;
            long l3 = 0;
            f.Close();
            int i1 = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string str1 = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string str2 = dataGridView1.Rows[i].Cells[1].Value.ToString();
                l2 = CommonCode.HexToLong(str1);
                l3 = CommonCode.HexToLong(str2);
                if (l2 > l)
                {
                    if (s2 == "org")
                    {
                        MessageBox.Show("地址范围“" + str1 + "”已超过了原文文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        i1 = 1;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("地址范围“" + str1 + "”已超过了译文文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        i1 = 1;
                        break;
                    }
                }
                else if (l3 > l)
                {
                    if (s2 == "tra")
                    {
                        MessageBox.Show("地址范围“" + str2 + "”已超过了原文文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        i1 = 1;
                        break;
                    }
                    else
                    {
                        MessageBox.Show("地址范围“" + str2 + "”已超过了译文文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        i1 = 1;
                        break;
                    }
                }
            }
            if (i1 == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void LoadAEPESetup(string s)//加载配置文件
        {
            try
            {
                string s1 = "";
                char c = ' ';
                string[] splits1 = new string[2];
                ArrayList al = new ArrayList();
                StreamReader sr = File.OpenText(s);//打开配置文件
                while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                {
                    al.Add(s1);
                }
                sr.Close();//关闭流
                textBox1.Text = al[0].ToString().Remove(0, 13);
                textBox2.Text = al[1].ToString().Remove(0, 14);
                comboBox1.Text = al[2].ToString().Remove(0, 7);
                comboBox2.Text = al[3].ToString().Remove(0, 7);
                if (al[4].ToString().Remove(0, 5) == "True")
                {
                    radioButton1.Checked = true;
                }
                else if (al[5].ToString().Remove(0, 7) == "True")
                {
                    radioButton2.Checked = true;
                }
                if (al[6].ToString().Remove(0, 5) == "True")
                {
                    radioButton3.Checked = true;
                }
                else if (al[7].ToString().Remove(0, 5) == "True")
                {
                    radioButton4.Checked = true;
                }
                else if (al[8].ToString().Remove(0, 8) == "True")
                {
                    radioButton5.Checked = true;
                }
                int i1 = al.Count;
                dataGridView1.Rows.Clear();
                for (int i = 9; i < i1; i++)
                {
                    splits1 = al[i].ToString().Split(c);
                    string[] subItem = { splits1[0], splits1[1] };
                    dataGridView1.Rows.Add(subItem);
                }
            }
            catch
            {
                MessageBox.Show("加载配置文件时出现错误，配置文件不正确或已被非法修改。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveAEPESetup(string s)//保存配置文件
        {
            ArrayList al = new ArrayList();
            al.Add("OriginalFile=" + textBox1.Text);
            al.Add("TranslateFile=" + textBox2.Text);
            al.Add("Source=" + comboBox1.Text);
            al.Add("Target=" + comboBox2.Text);
            al.Add("Auto=" + radioButton1.Checked.ToString());
            al.Add("Manual=" + radioButton2.Checked.ToString());
            al.Add("ANSI=" + radioButton3.Checked.ToString());
            al.Add("UTF8=" + radioButton4.Checked.ToString());
            al.Add("Unicode=" + radioButton5.Checked.ToString());
            int i1 = dataGridView1.Rows.Count;
            if (i1 > 0)
            {
                for (int i = 0; i < i1; i++)
                {
                    al.Add(dataGridView1.Rows[i].Cells[0].Value.ToString() + " " + dataGridView1.Rows[i].Cells[1].Value.ToString());
                }
            }
            StreamWriter sw = new StreamWriter(s, false);
            i1 = al.Count;
            for (int i = 0; i < i1; i++)
            {
                sw.WriteLine(al[i].ToString());//写入配置信息
            }
            al.Clear();
            sw.Close();
        }

        private void PasteText(int i)//粘贴剪贴板中的十六进制值
        {
            IDataObject cb = Clipboard.GetDataObject();//剪贴板操作
            if (cb.GetDataPresent(DataFormats.Text))//判断剪贴板中的内容是否是文本内容
            {
                string str1 = (string)cb.GetData(DataFormats.Text);
                if (CommonCode.Is_Hex(str1) == false)
                {
                    MessageBox.Show("粘贴板中的数据不是有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (i == 3)
                    {
                        textBox3.Text = str1;
                    }
                    if (i == 4)
                    {
                        textBox4.Text = str1;
                    }
                }
            }
            else
            {
                MessageBox.Show("粘贴板中没有数据，或是其中的数据不是文本内容。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int Compare(string s1, string s2)//比较输入的内容和存在的内容之间的关系
        {
            long l1 = CommonCode.HexToLong(s1);
            long l2 = CommonCode.HexToLong(s2);
            long l3 = 0;
            long l4 = 0;
            int i1 = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string str1 = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string str2 = dataGridView1.Rows[i].Cells[1].Value.ToString();
                l3 = CommonCode.HexToLong(str1);
                l4 = CommonCode.HexToLong(str2);
                if (l1 >= l3 && l2 <= l4)
                {
                    MessageBox.Show("输入的范围相同或已被包含在内。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    i1 = 1;
                    break;
                }
                else if ((l1 < l3 && l2 > l3) || (l1 < l4 && l2 > l4))
                {
                    MessageBox.Show("输入的范围与地址 " + str1 + " 到地址 " + str2 + " 存在重叠交叉。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    i1 = 1;
                    break;
                }
            }
            if (i1 == 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = CommonCode.Open_Exe_File(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = CommonCode.Open_Exe_File(textBox2.Text);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            PasteText(3);
        }

        private void label4_Click(object sender, EventArgs e)
        {
            PasteText(4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string str1 = textBox3.Text;
            string str2 = textBox4.Text;
            if (str1 == "")
            {
                MessageBox.Show("请输入起始地址。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (str2 == "")
            {
                MessageBox.Show("请输入结束地址。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CommonCode.Is_Hex(str1) == false)
            {
                MessageBox.Show("起始地址不是有效的十六进制值，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Text = "";
            }
            else if (CommonCode.Is_Hex(str2) == false)
            {
                MessageBox.Show("结束地址不是有效的十六进制值，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Text = "";
            }
            else if (CommonCode.HexToLong(str2) == 0)
            {
                MessageBox.Show("结束地址不能为零，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox4.Text = "";
            }
            else if (str1 == str2)
            {
                MessageBox.Show("起始地址和结束地址不能相同，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CommonCode.HexToLong(str1) > CommonCode.HexToLong(str2))
            {
                MessageBox.Show("起始地址不能大于结束地址，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                str1 = CommonCode.FormatStrHex(str1);//格式化十六进制值字符串
                str2 = CommonCode.FormatStrHex(str2);
                if (Compare(str1, str2) == 0)
                {
                    string[] subItem = { str1, str2 };
                    dataGridView1.Rows.Add(subItem);
                    textBox3.Text = "";
                    textBox4.Text = "";
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                DialogResult d = MessageBox.Show("确实要清除所有地址吗？", "清除地址", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (d == DialogResult.OK)
                {
                    dataGridView1.Rows.Clear();
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s1 = mainform.CDirectory + "配置";
            string s2 = s1 + "\\AEPESetup.ini";
            if (checkBox2.Checked == false)
            {
                if (Directory.Exists(s1) == true)
                {
                    if (File.Exists(s2) == true)
                    {
                        LoadAEPESetup(s2);
                    }
                }
            }
            else
            {
                OpenFileDialog open = new OpenFileDialog();
                open.InitialDirectory = s1;
                open.Filter = "配置文件(*.ini)|*.ini";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    s2 = open.FileName;
                    LoadAEPESetup(s2);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CommonCode.SetupFolder();
            string s1 = mainform.CDirectory + "配置\\AEPESetup.ini";
            if (checkBox2.Checked == false)
            {
                if (File.Exists(s1) == false)
                {
                    StreamWriter sw = File.CreateText(s1);
                    sw.Close();
                }
                SaveAEPESetup(s1);
            }
            else
            {
                string s2 = "";
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = mainform.CDirectory + "配置";
                save.Filter = "配置文件(*.ini)|*.ini";
                save.OverwritePrompt = false;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    s2 = save.FileName;
                    if (s1 == s2)
                    {
                        MessageBox.Show("手动保存的配置文件不能与默认的配置文件相同，请另外指定不同的文件名。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        SaveAEPESetup(s2);
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = false;
            label4.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            dataGridView1.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            dataGridView1.Rows.Clear();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = true;
            label4.Enabled = true;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            dataGridView1.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void Create_Dictionary()
        {
            SQLiteConnection.CreateFile(Dictionaryname);
            SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Dictionaryname);
            MyAccess.Open();
            SQLiteCommand cmd = new SQLiteCommand(MyAccess);
            cmd.CommandText = "CREATE TABLE `diclanguage` ("
                + "`orgfontname`	TEXT DEFAULT '',"
                + "`orgfontsize`	NUMERIC DEFAULT 0,"
                + "`trafontname`	TEXT DEFAULT '',"
                + "`trafontsize`	NUMERIC DEFAULT 0"
                + ");";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE `tbl` ("
                + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                + "`org`	TEXT DEFAULT '',"
                + "`tra`	TEXT DEFAULT ''"
                + ");";
            cmd.ExecuteNonQuery();
            MyAccess.Close();
        }

        private void Save_Data()//把提取出的内容写入字典
        {
            Create_Dictionary();
            SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Dictionaryname);
            MyAccess.Open();
            SQLiteCommand cmd = new SQLiteCommand(MyAccess);
            int i1 = list1.Count;
            int abc = 0;
            progressBar1.Maximum = i1;
            progressBar2.Maximum = i1;
            if (i1 > 0)
            {
                cmd.Transaction = MyAccess.BeginTransaction();
                for (int i = 0; i < i1; i++)
                {
                    abc++;
                    if (abc == 100)
                    {
                        progressBar1.Value = i;
                        progressBar2.Value = i;
                        abc = 0;
                    }
                    string str1 = list1[i].ToString();
                    str1 = str1.Replace("'", "''");
                    string str2 = list2[i].ToString();
                    str2 = str2.Replace("'", "''");
                    cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + str1 + "','" + str2 + "')";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        continue;
                    }
                }
                cmd.Transaction.Commit();
            }
            MyAccess.Close();
            i1 = list3.Count;
            int i2 = list4.Count;
            if (i1 == 0 && i2 == 0)
            {
                progressBar1.Value = progressBar1.Maximum;
                progressBar2.Value = progressBar2.Maximum;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("字典保存成功，建议使用字典编辑器对提取的字典进一步进行编辑，以便使提取的数据更准确。", "确定");
                }));
                progressBar1.Value = 0;
                progressBar2.Value = 0;
                DoneBool = true;
            }
            else
            {
                progressBar1.Maximum = i1;
                progressBar2.Maximum = i1;
                for (int i = 0; i < list3.Count - 1; i++)
                {
                    progressBar1.Value = i;
                    progressBar2.Value = i;
                    for (int x = list3.Count - 1; x > i; x--)
                    {
                        if (list3[x].ToString() == list3[i].ToString())
                        {
                            list3.RemoveAt(x);
                        }
                    }
                }
                progressBar1.Maximum = i2;
                progressBar2.Maximum = i2;
                for (int i = 0; i < list4.Count - 1; i++)
                {
                    progressBar1.Value = i;
                    progressBar2.Value = i;
                    for (int x = list4.Count - 1; x > i; x--)
                    {
                        if (list4[x].ToString() == list4[i].ToString())
                        {
                            list4.RemoveAt(x);
                        }
                    }
                }
                i1 = list3.Count;
                i2 = list4.Count;
                progressBar1.Maximum = i1;
                progressBar2.Maximum = i1;
                for (int i = 0; i < i1; i++)
                {
                    progressBar1.Value = i;
                    progressBar2.Value = i;
                    CT.listBox1.Items.Add(list3[i].ToString());
                }
                progressBar1.Maximum = i2;
                progressBar2.Maximum = i2;
                for (int i = 0; i < i2; i++)
                {
                    progressBar1.Value = i;
                    progressBar2.Value = i;
                    CT.listBox2.Items.Add(list4[i].ToString());
                }
                progressBar1.Value = 0;
                progressBar2.Value = 0;
                cShowBool = true;
            }
        }

        private void ANSIPEEnglish(string s1, string s2)//ANSI字符串提取
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s3 = "";
            string s4 = "";
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            bool bool1 = true;
            bool bool2 = true;
            int x = 0;
            int y = 0;
            int z = 0;
            byte b1 = 0;
            byte b2 = 0;
            int i3 = 0;
            int i4 = 0;
            int abc = 0;
            long l1 = 0;
            long l2 = 0;
            long l3 = 0;
            long l4 = 0;
            long l5 = 0;
            long l6 = 0;
            uint u1 = 0;
            uint u2 = 0;
            int dgv = al2.Count;
            LinkedList<byte> bList1 = new LinkedList<byte>();
            LinkedList<byte> bList2 = new LinkedList<byte>();
            int tracode = 0;
            string traname = comboBox2.Text;
            if (traname == "简体中文(936)")
            {
                tracode = 936;
            }
            else if (traname == "繁体中文(950)")
            {
                tracode = 950;
            }
            else if (traname == "默认")
            {
                tracode = mainform.AA_Default_Encoding.CodePage;
            }
            progressBar1.Maximum = (int)CommonCode.HexToLong(al2[dgv - 1].ToString());
            for (int i = 0; i < dgv; i++)
            {
                l3 = CommonCode.HexToLong(al1[i].ToString());
                l4 = CommonCode.HexToLong(al2[i].ToString());
                fs1.Seek(l3, SeekOrigin.Begin);
                fs2.Seek(l3, SeekOrigin.Begin);
                while (fs1.Position < l4)
                {
                    b1 = br1.ReadByte();
                    b2 = br2.ReadByte();
                    abc++;
                    if (abc == 100)
                    {
                        progressBar1.Value = (int)fs1.Position;
                        abc = 0;
                    }
                    if (b1 != b2)
                    {
                        x = y = z = 0;
                        if (b1 != 0 && b2 != 0)
                        {
                            do
                            {
                                fs1.Seek(-2, SeekOrigin.Current);
                                fs2.Seek(-2, SeekOrigin.Current);
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                            } while (b1 > 0 && CommonCode.CheckEnglish(b1) == true);
                            l1 = fs1.Position;
                            l2 = fs2.Position;
                            bool1 = true;
                            bool2 = true;
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                                if (b1 == 0)
                                {
                                    bool1 = false;
                                }
                                if (b1 > 0 && bool1 == false)
                                {
                                    x++;
                                    bool1 = true;
                                }
                                if (b2 == 0)
                                {
                                    bool2 = false;
                                }
                                if (b2 > 0 && bool2 == false)
                                {
                                    y++;
                                    bool2 = true;
                                }
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            if (z > 2)
                            {
                                if (x == 0 && y == 0)//字符串没有经过任何挪移
                                {
                                    do
                                    {
                                        try
                                        {
                                            b1 = br1.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList1.AddLast(b1);
                                    } while (b1 != 0);
                                    do
                                    {
                                        try
                                        {
                                            b2 = br2.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList2.AddLast(b2);
                                    } while (b2 != 0);
                                    fs1.Seek(l1 - 4, SeekOrigin.Begin);
                                    u1 = br1.ReadUInt32();
                                    if (u1 != 4294967295)
                                    {
                                        bList1.RemoveLast();
                                        bList2.RemoveLast();
                                        byte[] bt1 = new byte[bList1.Count];
                                        bList1.CopyTo(bt1, 0);
                                        bList1.Clear();
                                        s3 = Encoding.GetEncoding(1252).GetString(bt1);
                                        byte[] bt2 = new byte[bList2.Count];
                                        bList2.CopyTo(bt2, 0);
                                        bList2.Clear();
                                        s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                        list1.Add(s3);
                                        list2.Add(s4);
                                    }
                                }
                                else//字符串被借用了位置
                                {
                                    for (int ix = 0; ix <= x; ix++)
                                    {
                                        i3 = 0;
                                        do
                                        {
                                            try
                                            {
                                                b1 = br1.ReadByte();
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            bList1.AddLast(b1);
                                            i3++;
                                        } while (b1 != 0);
                                        l5 = fs1.Position;
                                        fs1.Seek(-i3 - 4, SeekOrigin.Current);
                                        u1 = br1.ReadUInt32();
                                        if (u1 != 4294967295)
                                        {
                                            bList1.RemoveLast();
                                            byte[] bt1 = new byte[bList1.Count];
                                            bList1.CopyTo(bt1, 0);
                                            bList1.Clear();
                                            s3 = Encoding.GetEncoding(1252).GetString(bt1);
                                            list3.Add(s3);
                                            list5.Add((l5 - i3).ToString());
                                        }
                                        fs1.Seek(l5, SeekOrigin.Begin);
                                        if (ix < x)
                                        {
                                            do
                                            {
                                                b1 = br1.ReadByte();
                                            } while (b1 == 0);
                                            fs1.Seek(-1, SeekOrigin.Current);
                                            u1 = br1.ReadUInt32();
                                            if (u1 != 4294967295)
                                            {
                                                fs1.Seek(-4, SeekOrigin.Current);
                                            }
                                        }
                                    }
                                    for (int iy = 0; iy <= y; iy++)
                                    {
                                        i4 = 0;
                                        do
                                        {
                                            try
                                            {
                                                b2 = br2.ReadByte();
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            bList2.AddLast(b2);
                                            i4++;
                                        } while (b2 != 0);
                                        l6 = fs2.Position;
                                        fs2.Seek(-i4 - 4, SeekOrigin.Current);
                                        u2 = br2.ReadUInt32();
                                        if (u2 != 4294967295)
                                        {
                                            bList2.RemoveLast();
                                            byte[] bt2 = new byte[bList2.Count];
                                            bList2.CopyTo(bt2, 0);
                                            bList2.Clear();
                                            s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                            list4.Add(s4);
                                            list6.Add((l6 - i4).ToString());
                                        }
                                        fs2.Seek(l6, SeekOrigin.Begin);
                                        if (iy < y)
                                        {
                                            do
                                            {
                                                b2 = br2.ReadByte();
                                            } while (b2 == 0);
                                            fs2.Seek(-1, SeekOrigin.Current);
                                            u2 = br2.ReadUInt32();
                                            if (u2 != 4294967295)
                                            {
                                                fs2.Seek(-4, SeekOrigin.Current);
                                            }
                                        }
                                    }
                                }
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                        else//字符串被移动了位置
                        {
                            l1 = fs1.Position - 1;
                            l2 = fs2.Position - 1;
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            while (fs1.Position < l1 + z)
                            {
                                i3 = 0;
                                do
                                {
                                    b1 = br1.ReadByte();
                                } while (b1 == 0);
                                fs1.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b1 = br1.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList1.AddLast(b1);
                                    i3++;
                                } while (b1 != 0);
                                l5 = fs1.Position;
                                fs1.Seek(-i3 - 4, SeekOrigin.Current);
                                u1 = br1.ReadUInt32();
                                if (u1 != 4294967295)
                                {
                                    bList1.RemoveLast();
                                    byte[] bt1 = new byte[bList1.Count];
                                    bList1.CopyTo(bt1, 0);
                                    bList1.Clear();
                                    s3 = Encoding.GetEncoding(1252).GetString(bt1);
                                    list3.Add(s3);
                                    list5.Add((l5 - i3).ToString());
                                }
                                fs1.Seek(l5, SeekOrigin.Begin);
                                do
                                {
                                    b1 = br1.ReadByte();
                                } while (b1 == 0);
                                fs1.Seek(-1, SeekOrigin.Current);
                                if (fs1.Position < l1 + z)
                                {
                                    u1 = br1.ReadUInt32();
                                    if (u1 != 4294967295)
                                    {
                                        fs1.Seek(-4, SeekOrigin.Current);
                                    }
                                }
                            }
                            while (fs2.Position < l2 + z)
                            {
                                i4 = 0;
                                do
                                {
                                    b2 = br2.ReadByte();
                                } while (b2 == 0);
                                fs2.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b2 = br2.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList2.AddLast(b2);
                                    i4++;
                                } while (b2 != 0);
                                l6 = fs2.Position;
                                fs2.Seek(-i4 - 4, SeekOrigin.Current);
                                u2 = br2.ReadUInt32();
                                if (u2 != 4294967295)
                                {
                                    bList2.RemoveLast();
                                    byte[] bt2 = new byte[bList2.Count];
                                    bList2.CopyTo(bt2, 0);
                                    bList2.Clear();
                                    s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                    list4.Add(s4);
                                    list6.Add((l6 - i4).ToString());
                                }
                                fs2.Seek(l6, SeekOrigin.Begin);
                                do
                                {
                                    b2 = br2.ReadByte();
                                } while (b2 == 0);
                                fs2.Seek(-1, SeekOrigin.Current);
                                if (fs2.Position < l2 + z)
                                {
                                    u2 = br2.ReadUInt32();
                                    if (u2 != 4294967295)
                                    {
                                        fs2.Seek(-4, SeekOrigin.Current);
                                    }
                                }
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                    }
                }
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();//提取完成
            if (list1.Count == 0 && list2.Count == 0 && list3.Count == 0 && list4.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，请检查指定的文件是否是两\r\n个完全相同的文件，或是指定的范围是否有效。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Save_Data();
            }
        }

        private void ANSIPEFrench(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void ANSIPEGerman(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void ANSIPEJapanese(string s1, string s2)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s3 = "";
            string s4 = "";
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            bool bool1 = true;
            bool bool2 = true;
            int x = 0;
            int y = 0;
            int z = 0;
            byte b1 = 0;
            byte b2 = 0;
            int i3 = 0;
            int i4 = 0;
            int abc = 0;
            long l1 = 0;
            long l2 = 0;
            long l3 = 0;
            long l4 = 0;
            long l5 = 0;
            long l6 = 0;
            uint u1 = 0;
            uint u2 = 0;
            int dgv = al2.Count;
            LinkedList<byte> bList1 = new LinkedList<byte>();
            LinkedList<byte> bList2 = new LinkedList<byte>();
            int tracode = 0;
            string traname = comboBox2.Text;
            if (traname == "简体中文(936)")
            {
                tracode = 936;
            }
            else if (traname == "繁体中文(950)")
            {
                tracode = 950;
            }
            else if (traname == "默认")
            {
                tracode = mainform.AA_Default_Encoding.CodePage;
            }
            progressBar1.Maximum = (int)CommonCode.HexToLong(al2[dgv - 1].ToString());
            for (int i = 0; i < dgv; i++)
            {
                l3 = CommonCode.HexToLong(al1[i].ToString());
                l4 = CommonCode.HexToLong(al2[i].ToString());
                fs1.Seek(l3, SeekOrigin.Begin);
                fs2.Seek(l3, SeekOrigin.Begin);
                while (fs1.Position < l4)
                {
                    b1 = br1.ReadByte();
                    b2 = br2.ReadByte();
                    abc++;
                    if (abc == 100)
                    {
                        progressBar1.Value = (int)fs1.Position;
                        abc = 0;
                    }
                    if (b1 != b2)
                    {
                        x = y = z = 0;
                        if (b1 != 0 && b2 != 0)
                        {
                            do
                            {
                                fs1.Seek(-2, SeekOrigin.Current);
                                fs2.Seek(-2, SeekOrigin.Current);
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                            } while (b1 > 0);
                            l1 = fs1.Position;
                            l2 = fs2.Position;
                            bool1 = true;
                            bool2 = true;
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                                if (b1 == 0)
                                {
                                    bool1 = false;
                                }
                                if (b1 > 0 && bool1 == false)
                                {
                                    x++;
                                    bool1 = true;
                                }
                                if (b2 == 0)
                                {
                                    bool2 = false;
                                }
                                if (b2 > 0 && bool2 == false)
                                {
                                    y++;
                                    bool2 = true;
                                }
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            if (z > 2)
                            {
                                if (x == 0 && y == 0)//字符串没有经过任何挪移
                                {
                                    do
                                    {
                                        try
                                        {
                                            b1 = br1.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList1.AddLast(b1);
                                    } while (b1 != 0);
                                    do
                                    {
                                        try
                                        {
                                            b2 = br2.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList2.AddLast(b2);
                                    } while (b2 != 0);
                                    fs1.Seek(l1 - 4, SeekOrigin.Begin);
                                    u1 = br1.ReadUInt32();
                                    if (u1 != 4294967295)
                                    {
                                        bList1.RemoveLast();
                                        bList2.RemoveLast();
                                        byte[] bt1 = new byte[bList1.Count];
                                        bList1.CopyTo(bt1, 0);
                                        bList1.Clear();
                                        s3 = Encoding.GetEncoding(932).GetString(bt1);
                                        byte[] bt2 = new byte[bList2.Count];
                                        bList2.CopyTo(bt2, 0);
                                        bList2.Clear();
                                        s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                        list1.Add(s3);
                                        list2.Add(s4);
                                    }
                                }
                                else//字符串被借用了位置
                                {
                                    for (int ix = 0; ix <= x; ix++)
                                    {
                                        i3 = 0;
                                        do
                                        {
                                            try
                                            {
                                                b1 = br1.ReadByte();
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            bList1.AddLast(b1);
                                            i3++;
                                        } while (b1 != 0);
                                        l5 = fs1.Position;
                                        fs1.Seek(-i3 - 4, SeekOrigin.Current);
                                        u1 = br1.ReadUInt32();
                                        if (u1 != 4294967295)
                                        {
                                            bList1.RemoveLast();
                                            byte[] bt1 = new byte[bList1.Count];
                                            bList1.CopyTo(bt1, 0);
                                            bList1.Clear();
                                            s3 = Encoding.GetEncoding(932).GetString(bt1);
                                            list3.Add(s3);
                                            list5.Add((l5 - i3).ToString());
                                        }
                                        fs1.Seek(l5, SeekOrigin.Begin);
                                        if (ix < x)
                                        {
                                            do
                                            {
                                                b1 = br1.ReadByte();
                                            } while (b1 == 0);
                                            fs1.Seek(-1, SeekOrigin.Current);
                                            u1 = br1.ReadUInt32();
                                            if (u1 != 4294967295)
                                            {
                                                fs1.Seek(-4, SeekOrigin.Current);
                                            }
                                        }
                                    }
                                    for (int iy = 0; iy <= y; iy++)
                                    {
                                        i4 = 0;
                                        do
                                        {
                                            try
                                            {
                                                b2 = br2.ReadByte();
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            bList2.AddLast(b2);
                                            i4++;
                                        } while (b2 != 0);
                                        l6 = fs2.Position;
                                        fs2.Seek(-i4 - 4, SeekOrigin.Current);
                                        u2 = br2.ReadUInt32();
                                        if (u2 != 4294967295)
                                        {
                                            bList2.RemoveLast();
                                            byte[] bt2 = new byte[bList2.Count];
                                            bList2.CopyTo(bt2, 0);
                                            bList2.Clear();
                                            s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                            list4.Add(s4);
                                            list6.Add((l6 - i4).ToString());
                                        }
                                        fs2.Seek(l6, SeekOrigin.Begin);
                                        if (iy < y)
                                        {
                                            do
                                            {
                                                b2 = br2.ReadByte();
                                            } while (b2 == 0);
                                            fs2.Seek(-1, SeekOrigin.Current);
                                            u2 = br2.ReadUInt32();
                                            if (u2 != 4294967295)
                                            {
                                                fs2.Seek(-4, SeekOrigin.Current);
                                            }
                                        }
                                    }
                                }
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                        else//字符串被移动了位置
                        {
                            l1 = fs1.Position - 1;
                            l2 = fs2.Position - 1;
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            while (fs1.Position < l1 + z)
                            {
                                i3 = 0;
                                do
                                {
                                    b1 = br1.ReadByte();
                                } while (b1 == 0);
                                fs1.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b1 = br1.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList1.AddLast(b1);
                                    i3++;
                                } while (b1 != 0);
                                l5 = fs1.Position;
                                fs1.Seek(-i3 - 4, SeekOrigin.Current);
                                u1 = br1.ReadUInt32();
                                if (u1 != 4294967295)
                                {
                                    bList1.RemoveLast();
                                    byte[] bt1 = new byte[bList1.Count];
                                    bList1.CopyTo(bt1, 0);
                                    bList1.Clear();
                                    s3 = Encoding.GetEncoding(932).GetString(bt1);
                                    list3.Add(s3);
                                    list5.Add((l5 - i3).ToString());
                                }
                                fs1.Seek(l5, SeekOrigin.Begin);
                                do
                                {
                                    b1 = br1.ReadByte();
                                } while (b1 == 0);
                                fs1.Seek(-1, SeekOrigin.Current);
                                if (fs1.Position < l1 + z)
                                {
                                    u1 = br1.ReadUInt32();
                                    if (u1 != 4294967295)
                                    {
                                        fs1.Seek(-4, SeekOrigin.Current);
                                    }
                                }
                            }
                            while (fs2.Position < l2 + z)
                            {
                                i4 = 0;
                                do
                                {
                                    b2 = br2.ReadByte();
                                } while (b2 == 0);
                                fs2.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b2 = br2.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList2.AddLast(b2);
                                    i4++;
                                } while (b2 != 0);
                                l6 = fs2.Position;
                                fs2.Seek(-i4 - 4, SeekOrigin.Current);
                                u2 = br2.ReadUInt32();
                                if (u2 != 4294967295)
                                {
                                    bList2.RemoveLast();
                                    byte[] bt2 = new byte[bList2.Count];
                                    bList2.CopyTo(bt2, 0);
                                    bList2.Clear();
                                    s4 = Encoding.GetEncoding(tracode).GetString(bt2);
                                    list4.Add(s4);
                                    list6.Add((l6 - i4).ToString());
                                }
                                fs2.Seek(l6, SeekOrigin.Begin);
                                do
                                {
                                    b2 = br2.ReadByte();
                                } while (b2 == 0);
                                fs2.Seek(-1, SeekOrigin.Current);
                                if (fs2.Position < l2 + z)
                                {
                                    u2 = br2.ReadUInt32();
                                    if (u2 != 4294967295)
                                    {
                                        fs2.Seek(-4, SeekOrigin.Current);
                                    }
                                }
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                    }
                }
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();//提取完成
            if (list1.Count == 0 && list2.Count == 0 && list3.Count == 0 && list4.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，请检查指定的文件是否是两\r\n个完全相同的文件，或是指定的范围是否有效。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Save_Data();
            }
        }

        private void UTF8PEEnglish(string s1, string s2)//UTF8字符串提取
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s3 = "";
            string s4 = "";
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            int dgv = al2.Count;
            bool bool1 = true;
            bool bool2 = true;
            int x = 0;
            int y = 0;
            int z = 0;
            byte b1 = 0;
            byte b2 = 0;
            int i3 = 0;
            int i4 = 0;
            int abc = 0;
            long l1 = 0;
            long l2 = 0;
            long l3 = 0;
            long l4 = 0;
            long l5 = 0;
            long l6 = 0;
            LinkedList<byte> bList1 = new LinkedList<byte>();
            LinkedList<byte> bList2 = new LinkedList<byte>();
            progressBar1.Maximum = (int)CommonCode.HexToLong(al2[dgv - 1].ToString());
            for (int i = 0; i < dgv; i++)
            {
                l3 = CommonCode.HexToLong(al1[i].ToString());
                l4 = CommonCode.HexToLong(al2[i].ToString());
                fs1.Seek(l3, SeekOrigin.Begin);
                fs2.Seek(l3, SeekOrigin.Begin);
                while (fs1.Position < l4)
                {
                    b1 = br1.ReadByte();
                    b2 = br2.ReadByte();
                    abc++;
                    if (abc == 100)
                    {
                        progressBar1.Value = (int)fs1.Position;
                        abc = 0;
                    }
                    if (b1 != b2)
                    {
                        x = y = z = 0;
                        if (b1 != 0 && b2 != 0)
                        {
                            do
                            {
                                fs1.Seek(-2, SeekOrigin.Current);
                                fs2.Seek(-2, SeekOrigin.Current);
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                            } while (b1 > 0 && CommonCode.CheckEnglish(b1) == true);
                            l1 = fs1.Position;
                            l2 = fs2.Position;
                            bool1 = true;
                            bool2 = true;
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                                if (b1 == 0)
                                {
                                    bool1 = false;
                                }
                                if (b1 > 0 && bool1 == false)
                                {
                                    x++;
                                    bool1 = true;
                                }
                                if (b2 == 0)
                                {
                                    bool2 = false;
                                }
                                if (b2 > 0 && bool2 == false)
                                {
                                    y++;
                                    bool2 = true;
                                }
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            if (x == 0 && y == 0)//字符串没有经过任何挪移
                            {
                                do
                                {
                                    try
                                    {
                                        b1 = br1.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList1.AddLast(b1);
                                } while (b1 != 0);
                                do
                                {
                                    try
                                    {
                                        b2 = br2.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList2.AddLast(b2);
                                } while (b2 != 0);
                                bList1.RemoveLast();
                                bList2.RemoveLast();
                                byte[] bt1 = new byte[bList1.Count];
                                bList1.CopyTo(bt1, 0);
                                bList1.Clear();
                                byte[] bt2 = new byte[bList2.Count];
                                bList2.CopyTo(bt2, 0);
                                bList2.Clear();
                                s3 = Encoding.UTF8.GetString(bt1);
                                s4 = Encoding.UTF8.GetString(bt2);
                                list1.Add(s3);
                                list2.Add(s4);
                            }
                            else//字符串被借用了位置
                            {
                                for (int ix = 0; ix <= x; ix++)
                                {
                                    i3 = 0;
                                    do
                                    {
                                        try
                                        {
                                            b1 = br1.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList1.AddLast(b1);
                                        i3++;
                                    } while (b1 != 0);
                                    l5 = fs1.Position;
                                    bList1.RemoveLast();
                                    byte[] bt1 = new byte[bList1.Count];
                                    bList1.CopyTo(bt1, 0);
                                    bList1.Clear();
                                    s3 = Encoding.UTF8.GetString(bt1);
                                    list3.Add(s3);
                                    list5.Add((l5 - i3).ToString());
                                    fs1.Seek(l5, SeekOrigin.Begin);
                                    if (ix < x)
                                    {
                                        do
                                        {
                                            b1 = br1.ReadByte();
                                        } while (b1 == 0);
                                        fs1.Seek(-1, SeekOrigin.Current);
                                    }
                                }
                                for (int iy = 0; iy <= y; iy++)
                                {
                                    i4 = 0;
                                    do
                                    {
                                        try
                                        {
                                            b2 = br2.ReadByte();
                                        }
                                        catch
                                        {
                                            break;
                                        }
                                        bList2.AddLast(b2);
                                        i4++;
                                    } while (b2 != 0);
                                    l6 = fs2.Position;
                                    bList2.RemoveLast();
                                    byte[] bt2 = new byte[bList2.Count];
                                    bList2.CopyTo(bt2, 0);
                                    bList2.Clear();
                                    s4 = Encoding.UTF8.GetString(bt2);
                                    list4.Add(s4);
                                    list6.Add((l6 - i4).ToString());
                                    fs2.Seek(l6, SeekOrigin.Begin);
                                    if (iy < y)
                                    {
                                        do
                                        {
                                            b2 = br2.ReadByte();
                                        } while (b2 == 0);
                                        fs2.Seek(-1, SeekOrigin.Current);
                                    }
                                }
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                        else//字符串被移动了位置
                        {
                            l1 = fs1.Position - 1;
                            l2 = fs2.Position - 1;
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            do
                            {
                                b1 = br1.ReadByte();
                                b2 = br2.ReadByte();
                                z++;
                            } while (b1 != 0 || b2 != 0);
                            fs1.Seek(l1, SeekOrigin.Begin);
                            fs2.Seek(l2, SeekOrigin.Begin);
                            while (fs1.Position < l1 + z)
                            {
                                i3 = 0;
                                do
                                {
                                    b1 = br1.ReadByte();
                                } while (b1 == 0);
                                fs1.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b1 = br1.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList1.AddLast(b1);
                                    i3++;
                                } while (b1 != 0);
                                l5 = fs1.Position;
                                bList1.RemoveLast();
                                byte[] bt1 = new byte[bList1.Count];
                                bList1.CopyTo(bt1, 0);
                                bList1.Clear();
                                s3 = Encoding.UTF8.GetString(bt1);
                                list3.Add(s3);
                                list5.Add((l5 - i3).ToString());
                                fs1.Seek(l5, SeekOrigin.Begin);
                            }
                            while (fs2.Position < l2 + z)
                            {
                                i4 = 0;
                                do
                                {
                                    b2 = br2.ReadByte();
                                } while (b2 == 0);
                                fs2.Seek(-1, SeekOrigin.Current);
                                do
                                {
                                    try
                                    {
                                        b2 = br2.ReadByte();
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    bList2.AddLast(b2);
                                    i4++;
                                } while (b2 != 0);
                                l6 = fs2.Position;
                                bList2.RemoveLast();
                                byte[] bt2 = new byte[bList2.Count];
                                bList2.CopyTo(bt2, 0);
                                bList2.Clear();
                                s4 = Encoding.UTF8.GetString(bt2);
                                list4.Add(s4);
                                list6.Add((l6 - i4).ToString());
                                fs2.Seek(l6, SeekOrigin.Begin);
                            }
                            fs1.Seek(l1 + z, SeekOrigin.Begin);
                            fs2.Seek(l2 + z, SeekOrigin.Begin);
                        }
                    }
                }
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();//提取完成
            if (list1.Count == 0 && list2.Count == 0 && list3.Count == 0 && list4.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，请检查指定的文件是否是两\r\n个完全相同的文件，或是指定的范围是否有效。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Save_Data();
            }
        }

        private void UTF8PEFrench(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UTF8PEGerman(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnicodePEEnglish(string s1, string s2)//Unicode字符串提取
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            int dgv = al2.Count;
            int z = 0;
            int i1 = 0;
            int i2 = 0;
            int i3 = 0;
            int i4 = 0;
            int abc = 0;
            long l1 = 0;
            long l2 = 0;
            long l3 = 0;
            long l4 = 0;
            LinkedList<byte> bList1 = new LinkedList<byte>();
            LinkedList<byte> bList2 = new LinkedList<byte>();
            progressBar1.Maximum = (int)CommonCode.HexToLong(al2[dgv - 1].ToString());
            for (int i = 0; i < dgv; i++)
            {
                l3 = CommonCode.HexToLong(al1[i].ToString());
                l4 = CommonCode.HexToLong(al2[i].ToString());
                fs1.Seek(l3, SeekOrigin.Begin);
                fs2.Seek(l3, SeekOrigin.Begin);
                while (fs1.Position < l4)
                {
                    i1 = br1.ReadByte();
                    i2 = br2.ReadByte();
                    abc++;
                    if (abc == 100)
                    {
                        progressBar1.Value = (int)fs1.Position;
                        abc = 0;
                    }
                    if (i1 != i2)
                    {
                        z = 0;
                        fs1.Seek(1, SeekOrigin.Current);
                        fs2.Seek(1, SeekOrigin.Current);
                        do
                        {
                            fs1.Seek(-4, SeekOrigin.Current);
                            fs2.Seek(-4, SeekOrigin.Current);
                            i1 = br1.ReadUInt16();
                            i2 = br2.ReadUInt16();
                        } while (i1 > 0 && CommonCode.CheckEnglish(i1) == true);
                        l1 = fs1.Position;
                        l2 = fs2.Position;
                        do
                        {
                            i1 = br1.ReadUInt16();
                            i2 = br2.ReadUInt16();
                            z += 2;
                        } while (i1 != 0 || i2 != 0);
                        fs1.Seek(l1, SeekOrigin.Begin);
                        fs2.Seek(l2, SeekOrigin.Begin);
                        i3 = i4 = 0;
                        do
                        {
                            i1 = br1.ReadUInt16();
                            i3 += 2;
                        } while (i1 != 0);
                        do
                        {
                            i2 = br2.ReadUInt16();
                            i4 += 2;
                        } while (i2 != 0);
                        fs1.Seek(l1, SeekOrigin.Begin);
                        fs2.Seek(l2, SeekOrigin.Begin);
                        i3 = i3 - 2;
                        i4 = i4 - 2;
                        for (int ix = 0; ix < i3; ix++)
                        {
                            bList1.AddLast(br1.ReadByte());
                        }
                        for (int iy = 0; iy < i4; iy++)
                        {
                            bList2.AddLast(br2.ReadByte());
                        }
                        byte[] bt1 = new byte[i3];
                        bList1.CopyTo(bt1, 0);
                        bList1.Clear();
                        byte[] bt2 = new byte[i4];
                        bList2.CopyTo(bt2, 0);
                        bList2.Clear();
                        s1 = Encoding.Unicode.GetString(bt1);
                        s2 = Encoding.Unicode.GetString(bt2);
                        if (s1.Length > 1 || s2.Length > 1)
                        {
                            list1.Add(s1);
                            list2.Add(s2);
                        }
                        fs1.Seek(l1 + z, SeekOrigin.Begin);
                        fs2.Seek(l2 + z, SeekOrigin.Begin);
                    }
                }
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();//提取完成
            i1 = list1.Count;
            i2 = list2.Count;
            if (i1 == 0 && i2 == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，请检查指定的文件是否是两\r\n个完全相同的文件，或是指定的范围是否有效。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Save_Data();
            }
        }

        private void UnicodePEFrench(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnicodePEGerman(string s1, string s2)
        {
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("此功能没有实现。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnPE1(string s1, string s2)//非PE文件提取 1
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            int tracode = 0;
            int orgcode = 0;
            string orgname = comboBox4.Text;
            string traname = comboBox3.Text;
            if (traname == "简体中文(936)")
            {
                tracode = 936;
            }
            else if (traname == "繁体中文(950)")
            {
                tracode = 950;
            }
            else if (traname == "默认")
            {
                tracode = mainform.AA_Default_Encoding.CodePage;
            }
            if (orgname == "英语(1252)" || orgname == "法语(1252)" || orgname == "德语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            int abc = 0;
            byte b1 = 0;
            long l1 = fs1.Length;
            long l2 = fs2.Length;
            LinkedList<byte> bList1 = new LinkedList<byte>();
            ArrayList tem1 = new ArrayList();
            ArrayList tem2 = new ArrayList();
            ArrayList tem3 = new ArrayList();
            progressBar2.Maximum = (int)l1;
            while (fs1.Position < l1 - 1)
            {
                abc++;
                if (abc == 100)
                {
                    progressBar2.Value = (int)fs1.Position;
                    abc = 0;
                }
                if (br1.ReadByte() != 0)
                {
                    fs1.Seek(-1, SeekOrigin.Current);
                    try
                    {
                        do
                        {
                            b1 = br1.ReadByte();
                            bList1.AddLast(b1);
                        } while (b1 != 0);
                        bList1.RemoveLast();
                        byte[] bt = new byte[bList1.Count];
                        bList1.CopyTo(bt, 0);
                        bList1.Clear();
                        tem1.Add(Encoding.GetEncoding(orgcode).GetString(bt));
                        tem3.Add(Encoding.GetEncoding(tracode).GetString(bt));
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            progressBar2.Maximum = (int)l2;
            while (fs2.Position < l2 - 1)
            {
                abc++;
                if (abc == 100)
                {
                    progressBar2.Value = (int)fs2.Position;
                    abc = 0;
                }
                if (br2.ReadByte() != 0)
                {
                    fs2.Seek(-1, SeekOrigin.Current);
                    try
                    {
                        do
                        {
                            b1 = br2.ReadByte();
                            bList1.AddLast(b1);
                        } while (b1 != 0);
                        bList1.RemoveLast();
                        byte[] bt = new byte[bList1.Count];
                        bList1.CopyTo(bt, 0);
                        bList1.Clear();
                        tem2.Add(Encoding.GetEncoding(tracode).GetString(bt));
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            int int1 = tem1.Count;
            int int2 = tem2.Count;
            if (int1 == int2)
            {
                for (int i = 0; i < int1; i++)
                {
                    s1 = tem1[i].ToString();
                    s2 = tem2[i].ToString();
                    if (s2 != tem3[i].ToString() && s1.Length > 1)
                    {
                        list1.Add(s1);
                        list2.Add(s2);
                    }
                }
            }
            else
            {
                for (int i = 0; i < int1; i++)
                {
                    s1 = tem1[i].ToString();
                    if (s1.Length > 1)
                    {
                        list3.Add(s1);
                    }
                }
                for (int i = 0; i < int2; i++)
                {
                    s2 = tem2[i].ToString();
                    if (s2.Length > 1)
                    {
                        list4.Add(s2);
                    }
                }
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();
            Save_Data();
        }

        private void Skyrim(string s1, string s2)//上古卷轴V
        {
            FileStream fs1 = new FileStream(s1, FileMode.Open, FileAccess.Read);
            FileStream fs2 = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            BinaryReader br2 = new BinaryReader(fs2);
            int i1 = br1.ReadInt32();
            int i2 = br2.ReadInt32();
            if (i1 == i2)
            {
                int orgcode = Encoding.UTF8.CodePage;
                if (comboBox4.Text == "英语(1252)")
                {
                    orgcode = 1252;
                }
                Create_Dictionary();
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Dictionaryname);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                string str1 = "";
                string str2 = "";
                LinkedList<byte> bList1 = new LinkedList<byte>();
                byte b1 = 0;
                int x = 0;
                int iStart = i1 * 8 + 8;
                progressBar2.Maximum = i1;
                cmd.Transaction = MyAccess.BeginTransaction();
                for (int i = 0; i < i1; i++)
                {
                    progressBar2.Value = i;
                    x = 0;
                    fs1.Seek(i * 8 + 12, SeekOrigin.Begin);
                    fs1.Seek(iStart + br1.ReadInt32(), SeekOrigin.Begin);
                    if (br1.ReadUInt32() > 20000)
                    {
                        fs1.Seek(-4, SeekOrigin.Current);
                    }
                    do
                    {
                        b1 = br1.ReadByte();
                        bList1.AddLast(b1);
                        x++;
                    } while (b1 > 0);
                    x--;
                    if (x > 0)
                    {
                        bList1.RemoveLast();
                        byte[] bt1 = new byte[x];
                        bList1.CopyTo(bt1, 0);
                        bList1.Clear();
                        str1 = Encoding.GetEncoding(orgcode).GetString(bt1);
                        x = 0;
                        fs2.Seek(i * 8 + 12, SeekOrigin.Begin);
                        fs2.Seek(iStart + br2.ReadInt32(), SeekOrigin.Begin);
                        if (br2.ReadUInt32() > 20000)
                        {
                            fs2.Seek(-4, SeekOrigin.Current);
                        }
                        do
                        {
                            b1 = br2.ReadByte();
                            bList1.AddLast(b1);
                            x++;
                        } while (b1 > 0);
                        x--;
                        if (x > 0)
                        {
                            bList1.RemoveLast();
                            byte[] bt2 = new byte[x];
                            bList1.CopyTo(bt2, 0);
                            bList1.Clear();
                            str2 = Encoding.UTF8.GetString(bt2);
                            if (str1 != str2)
                            {
                                str1 = str1.Replace("'", "''");
                                str2 = str2.Replace("'", "''");
                                cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + str1 + "','" + str2 + "')";
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                cmd.Transaction.Commit();
                MyAccess.Close();
                progressBar2.Value = 0;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("字典保存成功，建议使用字典编辑器对提取的字典进一步进行编辑，以便使提取的数据更准确。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("两个文件的结构不同，无法提取。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            br1.Close();
            br2.Close();
            fs1.Close();
            fs2.Close();
        }

        private void AutoPE(string str1, string str2)//PE 文件自动提取
        {
            ArrayList t1 = new ArrayList();
            ArrayList t2 = new ArrayList();
            ArrayList t3 = new ArrayList();
            ArrayList t4 = new ArrayList();
            string s1 = "";
            string s2 = "";
            FileStream fs = new FileStream(str1, FileMode.Open, FileAccess.Read);
            long L = fs.Length - 1;//文件大小
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u1 = br.ReadUInt32();//PE标识位置
            fs.Seek(u1 + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadUInt16();//读出 CPU 类型
            if (i1 == 332)
            {
                fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                i1 = br.ReadUInt16();//文件段数
                fs.Seek(u1 + 52, SeekOrigin.Begin);//基址
                t1.Add(br.ReadUInt32().ToString());//基址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 128, SeekOrigin.Begin);//输入表
                t1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 136, SeekOrigin.Begin);//资源段
                t1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 248, SeekOrigin.Begin);//各个段
                for (int i = 0; i < i1; i++)
                {
                    fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                    t1.Add(br.ReadUInt32().ToString());
                    t2.Add(br.ReadUInt32().ToString());
                    t3.Add(br.ReadUInt32().ToString());
                    t4.Add(br.ReadUInt32().ToString());
                    fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                }
                br.Close();
                fs.Close();
                i1 = int.Parse(t1[1].ToString());
                int i2 = int.Parse(t1[2].ToString());//资源段
                s1 = int.Parse(t4[3].ToString()).ToString("X8");
                if (i2 == 0)
                {
                    s2 = ((int)L).ToString("X8");//没有资源段则搜索范围为整个文件
                }
                else if (i2 > i1)//资源段大于输入表段
                {
                    int x = t1.Count;
                    int a = 0;
                    for (int i = 3; i < x; i++)
                    {
                        if (t2[i].ToString() == t1[1].ToString())
                        {
                            s2 = int.Parse(t4[i].ToString()).ToString("X8");//范围是输入表段
                            a = 1;
                            break;
                        }
                    }
                    if (a == 0)
                    {
                        for (int i = 3; i < x; i++)
                        {
                            if (t2[i].ToString() == t1[2].ToString())
                            {
                                s2 = int.Parse(t4[i].ToString()).ToString("X8");//如果输入表段数据与实际不符则以资源段为标准
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int x = t1.Count;
                    for (int i = 3; i < x; i++)
                    {
                        if (t2[i].ToString() == t1[2].ToString())
                        {
                            s2 = int.Parse(t4[i].ToString()).ToString("X8");
                            break;
                        }
                    }
                }
            }
            else
            {
                fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                i1 = br.ReadUInt16();//文件段数
                fs.Seek(u1 + 48, SeekOrigin.Begin);//基址
                t1.Add(br.ReadUInt64().ToString());//基址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 144, SeekOrigin.Begin);//输入表
                t1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 152, SeekOrigin.Begin);//资源段
                t1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                t2.Add("0");
                t3.Add("0");
                t4.Add("0");
                fs.Seek(u1 + 264, SeekOrigin.Begin);//各个段
                for (int i = 0; i < i1; i++)
                {
                    fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                    t1.Add(br.ReadUInt32().ToString());
                    t2.Add(br.ReadUInt32().ToString());
                    t3.Add(br.ReadUInt32().ToString());
                    t4.Add(br.ReadUInt32().ToString());
                    fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                }
                br.Close();
                fs.Close();
                i1 = int.Parse(t1[1].ToString());
                int i2 = int.Parse(t1[2].ToString());//资源段
                s1 = int.Parse(t4[3].ToString()).ToString("X8");
                if (i2 == 0)
                {
                    s2 = ((int)L).ToString("X8");//没有资源段则搜索范围为整个文件
                }
                else if (i2 > i1)//资源段大于输入表段
                {
                    int x = t1.Count;
                    int a = 0;
                    for (int i = 3; i < x; i++)
                    {
                        if (t2[i].ToString() == t1[1].ToString())
                        {
                            s2 = int.Parse(t4[i].ToString()).ToString("X8");//范围是输入表段
                            a = 1;
                            break;
                        }
                    }
                    if (a == 0)
                    {
                        for (int i = 3; i < x; i++)
                        {
                            if (t2[i].ToString() == t1[2].ToString())
                            {
                                s2 = int.Parse(t4[i].ToString()).ToString("X8");//如果输入表段数据与实际不符则以资源段为标准
                                break;
                            }
                        }
                    }
                }
                else
                {
                    int x = t1.Count;
                    for (int i = 3; i < x; i++)
                    {
                        if (t2[i].ToString() == t1[2].ToString())
                        {
                            s2 = int.Parse(t4[i].ToString()).ToString("X8");
                            break;
                        }
                    }
                }
            }
            al1.Add(s1);//起始地址
            al2.Add(s2);//结束地址
            t1.Clear();
            t2.Clear();
            t3.Clear();
            t4.Clear();
            string codestr1 = comboBox1.Text;
            if (radioButton3.Checked == true)//ANSI 字典提取
            {
                if (codestr1 == "英语(1252)")
                {
                    ANSIPEEnglish(str1, str2);
                }
                else if (codestr1 == "法语(1252)")
                {
                    ANSIPEFrench(str1, str2);
                }
                else if (codestr1 == "德语(1252)")
                {
                    ANSIPEGerman(str1, str2);
                }
                else if (codestr1 == "俄语(1251)")
                {
                    //
                }
                else if (codestr1 == "韩文(949)")
                {
                    //
                }
                else if (codestr1 == "日语(932)")
                {
                    ANSIPEJapanese(str1, str2);
                }
                else if (codestr1 == "简体中文(936)")
                {
                    //
                }
                else if (codestr1 == "繁体中文(950)")
                {
                    //
                }
            }
            else if (radioButton4.Checked == true)//UTF-8 字典提取
            {
                if (codestr1 == "英语(1252)")
                {
                    UTF8PEEnglish(str1, str2);
                }
                else if (codestr1 == "法语(1252)")
                {
                    UTF8PEFrench(str1, str2);
                }
                else if (codestr1 == "德语(1252)")
                {
                    UTF8PEGerman(str1, str2);
                }
                else if (codestr1 == "俄语(1251)")
                {
                    //
                }
                else if (codestr1 == "韩文(949)")
                {
                    //
                }
                else if (codestr1 == "日语(932)")
                {
                    //
                }
                else if (codestr1 == "简体中文(936)")
                {
                    //
                }
                else if (codestr1 == "繁体中文(950)")
                {
                    //
                }
            }
            else if (radioButton5.Checked == true)//Unicode 字典提取
            {
                if (codestr1 == "英语(1252)")
                {
                    UnicodePEEnglish(str1, str2);
                }
                else if (codestr1 == "法语(1252)")
                {
                    UnicodePEFrench(str1, str2);
                }
                else if (codestr1 == "德语(1252)")
                {
                    UnicodePEGerman(str1, str2);
                }
                else if (codestr1 == "俄语(1251)")
                {
                    //
                }
                else if (codestr1 == "韩文(949)")
                {
                    //
                }
                else if (codestr1 == "日语(932)")
                {
                    //
                }
                else if (codestr1 == "简体中文(936)")
                {
                    //
                }
                else if (codestr1 == "繁体中文(950)")
                {
                    //
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)//开始字典的提取
        {
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            CommonCode.DictionaryFolder();
            Dictionaryname = mainform.CDirectory + "字典\\" + Path.GetFileNameWithoutExtension(str1) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".db";
            if (str1 == "")
            {
                MessageBox.Show("必须指定未翻译的原版文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (str2 == "")
            {
                MessageBox.Show("必须指定已翻译的译版文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (str1 == str2)
            {
                MessageBox.Show("你指定的原版文件和已翻译的译版文件是同一个文件，请修正。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(str1) == false)
            {
                MessageBox.Show("你指定的原版文件不存在。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(str2) == false)
            {
                MessageBox.Show("你指定的译版文件不存在。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (CommonCode.PE(str1) == false)
            {
                MessageBox.Show("原版文件不是 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (CommonCode.PE(str2) == false)
            {
                MessageBox.Show("译版文件不是 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(Dictionaryname) == true)
            {
                MessageBox.Show("名称为“" + Path.GetFileName(Dictionaryname) + "”的字典文件已存在，请稍候再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                backgroundWorker1.RunWorkerAsync();
                timer1.Enabled = true;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)//提取字典线程
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            string codestr1 = comboBox1.Text;
            al1.Clear();
            al2.Clear();
            list1.Clear();
            list2.Clear();
            list3.Clear();
            list4.Clear();
            list5.Clear();
            list6.Clear();
            if (radioButton2.Checked == true)//手动方式提取
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("手动提取请指定地址范围。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                else
                {
                    if (File_Length(str1, "org") == true)
                    {
                        if (File_Length(str2, "tra") == true)
                        {
                            if (CommonCode.File_Version_Info(str1, str2) == false)
                            {
                                this.Invoke(new Action(delegate
                                {
                                    MessageBox.Show("原文版的版本与译文版的版本不相同，无法进行字典的提取。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }));
                            }
                            else
                            {
                                tabControl1.Enabled = false;
                                int dgv = dataGridView1.Rows.Count;
                                for (int i = 0; i < dgv; i++)
                                {
                                    al1.Add(dataGridView1.Rows[i].Cells[0].Value);
                                    al2.Add(dataGridView1.Rows[i].Cells[1].Value);
                                }
                                if (radioButton3.Checked == true)//ANSI 字典提取
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        ANSIPEEnglish(str1, str2);
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        ANSIPEFrench(str1, str2);
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        ANSIPEGerman(str1, str2);
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        ANSIPEJapanese(str1, str2);
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        //
                                    }
                                }
                                else if (radioButton4.Checked == true)//UTF-8 字典提取
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        UTF8PEEnglish(str1, str2);
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        UTF8PEFrench(str1, str2);
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        UTF8PEGerman(str1, str2);
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        //UTF8Japanese(str1, str2);
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        //
                                    }
                                }
                                else if (radioButton5.Checked == true)//Unicode 字典提取
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        UnicodePEEnglish(str1, str2);
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        UnicodePEFrench(str1, str2);
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        UnicodePEGerman(str1, str2);
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        //UnicodeJapanese(str1, str2);
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        //
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        //
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else//自动方式提取
            {
                if (CommonCode.PE(str1) == true)//PE 文件自动
                {
                    if (CommonCode.File_Version_Info(str1, str2) == false)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("原版文件的版本与译版文件的版本不相同，无法进行字典的提取。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    else
                    {
                        tabControl1.Enabled = false;
                        AutoPE(str1, str2);
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            tabControl1.Enabled = true;
        }

        private void DictionaryExtract_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cShowBool == true)
            {
                cShowBool = false;
                timer1.Enabled = false;
                if (mainform.MyDpi > 96F)
                {
                    CT.Font = mainform.MyNewFont;
                }
                CT.ShowDialog();
            }
            else
            {
                if (DoneBool == false)
                {
                    timer1.Enabled = true;
                }
                else
                {
                    DoneBool = false;
                    timer1.Enabled = false;
                }
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                comboBox2.Enabled = true;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked == true)
            {
                comboBox2.Enabled = false;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)
            {
                comboBox2.Enabled = false;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = (string)Clipboard.GetData(DataFormats.Text);
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            textBox2.Text = (string)Clipboard.GetData(DataFormats.Text);
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void DictionaryExtract_Shown(object sender, EventArgs e)
        {
            comboBox1.Text = "英语(1252)";
            comboBox4.Text = "英语(1252)";
            if (mainform.AA_Default_Encoding.CodePage == 936)
            {
                comboBox2.Text = "简体中文(936)";
                comboBox3.Text = "简体中文(936)";
            }
            else if (mainform.AA_Default_Encoding.CodePage == 950)
            {
                comboBox2.Text = "繁体中文(950)";
                comboBox3.Text = "繁体中文(950)";
            }
            else
            {
                comboBox2.Text = "默认";
                comboBox3.Text = "默认";
            }
            int TBH = (int)(textBox1.Height / 2D - button1.Height / 2D);
            int CBH = (int)(comboBox1.Height / 2D - button1.Height / 2D);
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + TBH);
            button2.Location = new Point(button2.Location.X, textBox2.Location.Y + TBH);
            button3.Location = new Point(button3.Location.X, comboBox1.Location.Y + CBH);
            button4.Location = new Point(button4.Location.X, comboBox2.Location.Y + CBH);
            //
            button10.Location = new Point(button10.Location.X, textBox6.Location.Y + TBH);
            button9.Location = new Point(button9.Location.X, textBox5.Location.Y + TBH);
            button11.Location = new Point(button11.Location.X, comboBox4.Location.Y + CBH);
            button12.Location = new Point(button12.Location.X, comboBox3.Location.Y + CBH);
            if (mainform.MyDpi > 96F)
            {
                int MoveY = label1.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label1.Height / 2D);
                label1.Location = new Point(label1.Location.X, label1.Location.Y - MoveY);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
                label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
                label4.Location = new Point(label4.Location.X, label4.Location.Y - MoveY);
                label5.Location = new Point(label5.Location.X, label5.Location.Y - MoveY);
                label6.Location = new Point(label6.Location.X, label6.Location.Y - MoveY);
                label7.Location = new Point(label7.Location.X, label7.Location.Y - MoveY);
                label8.Location = new Point(label8.Location.X, label8.Location.Y - MoveY);
                label9.Location = new Point(label9.Location.X, label9.Location.Y - MoveY);
                label10.Location = new Point(label10.Location.X, label10.Location.Y - MoveY);
                int i1 = comboBox1.Location.X + comboBox1.Width;
                groupBox3.Width = i1 - groupBox3.Location.X;
                groupBox4.Width = i1 - groupBox4.Location.X;
                dataGridView1.Width = i1 - dataGridView1.Location.X;
            }
        }

        private void SaveAEUnPESetup(string s)//保存配置文件
        {
            ArrayList al = new ArrayList();
            al.Add("OriginalFile=" + textBox6.Text);
            al.Add("TranslateFile=" + textBox5.Text);
            al.Add("Source=" + comboBox4.Text);
            al.Add("Target=" + comboBox3.Text);
            if (radioButton6.Checked == true)
            {
                al.Add("Format=1");
            }
            else if (radioButton7.Checked == true)
            {
                al.Add("Format=2");
            }
            else if (radioButton8.Checked == true)
            {
                al.Add("Format=3");
            }
            else if (radioButton9.Checked == true)
            {
                al.Add("Format=4");
            }
            StreamWriter sw = new StreamWriter(s, false);
            for (int i = 0; i < al.Count; i++)
            {
                sw.WriteLine(al[i].ToString());//写入配置信息
            }
            al.Clear();
            sw.Close();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            CommonCode.SetupFolder();
            string s1 = mainform.CDirectory + "配置\\AEUnPESetup.ini";
            if (checkBox3.Checked == false)
            {
                if (File.Exists(s1) == false)
                {
                    StreamWriter sw = File.CreateText(s1);
                    sw.Close();
                }
                SaveAEUnPESetup(s1);
            }
            else
            {
                string s2 = "";
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = mainform.CDirectory + "配置";
                save.Filter = "配置文件(*.ini)|*.ini";
                save.OverwritePrompt = false;
                if (save.ShowDialog() == DialogResult.OK)
                {
                    s2 = save.FileName;
                    if (s1 == s2)
                    {
                        MessageBox.Show("手动保存的配置文件不能与默认的配置文件相同，请另外指定不同的文件名。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        SaveAEUnPESetup(s2);
                    }
                }
            }
        }

        private void LoadAEUnPESetup(string s)//加载配置文件
        {
            try
            {
                string s1 = "";
                string[] splits1 = new string[2];
                ArrayList al = new ArrayList();
                StreamReader sr = File.OpenText(s);//打开配置文件
                while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                {
                    al.Add(s1);
                }
                sr.Close();//关闭流
                textBox6.Text = al[0].ToString().Remove(0, 13);
                textBox5.Text = al[1].ToString().Remove(0, 14);
                comboBox4.Text = al[2].ToString().Remove(0, 7);
                comboBox3.Text = al[3].ToString().Remove(0, 7);
                s1 = al[4].ToString().Remove(0, 7);
                if (s1 == "1")
                {
                    radioButton6.Checked = true;
                }
                else if (s1 == "2")
                {
                    radioButton7.Checked = true;
                }
                else if (s1 == "3")
                {
                    radioButton8.Checked = true;
                }
                else if (s1 == "4")
                {
                    radioButton9.Checked = true;
                    textBox5.Text = "";
                    textBox5.Enabled = false;
                    button9.Enabled = false;
                }
            }
            catch
            {
                MessageBox.Show("加载配置文件时出现错误，配置文件不正确或已被非法修改。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string s1 = mainform.CDirectory + "配置";
            string s2 = s1 + "\\AEUnPESetup.ini";
            if (checkBox3.Checked == false)
            {
                if (Directory.Exists(s1) == true)
                {
                    if (File.Exists(s2) == true)
                    {
                        LoadAEUnPESetup(s2);
                    }
                }
            }
            else
            {
                OpenFileDialog open = new OpenFileDialog();
                open.InitialDirectory = s1;
                open.Filter = "配置文件(*.ini)|*.ini";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    s2 = open.FileName;
                    LoadAEUnPESetup(s2);
                }
            }
        }

        private void textBox6_DragDrop(object sender, DragEventArgs e)
        {
            textBox6.Text = (string)Clipboard.GetData(DataFormats.Text);
        }

        private void textBox6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void textBox5_DragDrop(object sender, DragEventArgs e)
        {
            textBox5.Text = (string)Clipboard.GetData(DataFormats.Text);
        }

        private void textBox5_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    Clipboard.SetDataObject(filenames[0]);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = false;
            if (radioButton9.Checked == false)
            {
                string str1 = textBox6.Text;
                string str2 = textBox5.Text;
                CommonCode.DictionaryFolder();
                Dictionaryname = mainform.CDirectory + "字典\\" + Path.GetFileName(str1) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".db";
                if (str1 == "")
                {
                    MessageBox.Show("必须指定未翻译的原版文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (str2 == "")
                {
                    MessageBox.Show("必须指定已翻译的译版文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (str1 == str2)
                {
                    MessageBox.Show("你指定的原版文件和已翻译的译版文件是同一个文件，请修正。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (File.Exists(str1) == false)
                {
                    MessageBox.Show("你指定的原版文件不存在。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (File.Exists(str2) == false)
                {
                    MessageBox.Show("你指定的译版文件不存在。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (CommonCode.PE(str1) == true)
                {
                    MessageBox.Show("原版文件是一个 PE 文件，请使用 PE 文件提取方法。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (CommonCode.PE(str2) == true)
                {
                    MessageBox.Show("译版文件是一个 PE 文件，请使用 PE 文件提取方法。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (File.Exists(Dictionaryname) == true)
                {
                    MessageBox.Show("名称为“" + Path.GetFileName(Dictionaryname) + "”的字典文件已存在，请稍候再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            else
            {
                string str1 = textBox6.Text;
                if (str1 == "")
                {
                    MessageBox.Show("必须指定一个 .po 文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (CommonCode.PE(str1) == true)
                {
                    MessageBox.Show("指定的文件是一个 PE 文件，请使用 PE 文件提取方法。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Dictionaryname = mainform.CDirectory + "字典\\" + Path.GetFileName(str1) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".db";
                    if (File.Exists(Dictionaryname) == true)
                    {
                        MessageBox.Show("名称为“" + Path.GetFileName(Dictionaryname) + "”的字典文件已存在，请稍候再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        CommonCode.DictionaryFolder();
                        Create_Dictionary();
                        backgroundWorker3.RunWorkerAsync();
                    }
                }
            }
            timer1.Enabled = true;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string str1 = textBox6.Text;
            string str2 = textBox5.Text;
            al1.Clear();
            al2.Clear();
            list1.Clear();
            list2.Clear();
            list3.Clear();
            list4.Clear();
            list5.Clear();
            list6.Clear();
            if (radioButton6.Checked == true || radioButton7.Checked == true)
            {
                UnPE1(str1, str2);
            }
            else if (radioButton8.Checked == true)
            {
                Skyrim(str1, str2);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar2.Value = 0;
            tabControl1.Enabled = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (radioButton9.Checked == true)
            {
                open.Filter = "Qt Linguist .po 文件(*.po)|*.po";
            }
            else
            {
                open.Filter = "所有文件(*.*)|*.*";
            }
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = open.FileName;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "所有文件(*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                textBox5.Text = open.FileName;
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            FileStream fs = new FileStream(textBox6.Text, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.UTF8);
            SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Dictionaryname);
            MyAccess.Open();
            SQLiteCommand cmd = new SQLiteCommand(MyAccess);
            cmd.Transaction = MyAccess.BeginTransaction();
            string s = "";
            string stmp = "";
            int x = 0;
            bool bl = false;
            progressBar2.Value = 50;
            while ((s = sr.ReadLine()) != null)//判定是否是最后一行
            {
                if (s != "")
                {
                    list1.Add(s);
                }
                else
                {
                    if (list1.Count > 0)
                    {
                        for (x = 0; x < list1.Count; x++)
                        {
                            s = list1[x].ToString();
                            if (s.Contains("msgid") == true)
                            {
                                for (; x < list1.Count; x++)
                                {
                                    s = list1[x].ToString();
                                    list2.Add(s);
                                    if (x < list1.Count - 1)
                                    {
                                        x++;
                                        for (; x < list1.Count; x++)
                                        {
                                            s = list1[x].ToString();
                                            if (s.Contains("msgid") == false && s.Contains("msgstr") == false)
                                            {
                                                list2.Add(s);
                                            }
                                            else
                                            {
                                                x--;
                                                break;
                                            }
                                        }
                                    }
                                    if (list2.Count == 1)
                                    {
                                        s = list2[0].ToString();
                                    }
                                    else
                                    {
                                        s = list2[0].ToString();
                                        for (int y = 1; y < list2.Count; y++)
                                        {
                                            stmp = list2[y].ToString();
                                            s = s.Substring(0, s.Length - 1) + stmp.Substring(1, stmp.Length - 1);
                                        }
                                    }
                                    list3.Add(s);
                                    list2.Clear();
                                }
                            }
                        }
                        for (x = list3.Count - 1; x >= 0; x--)
                        {
                            bl = false;
                            s = list3[x].ToString();
                            if (s.Contains("msgstr[1] ") == true && s.Contains("msgstr[1] \"\"") == false)
                            {
                                for (int y = 0; y < list3.Count; y++)
                                {
                                    stmp = list3[y].ToString();
                                    if (stmp.Contains("msgid_plural ") == true && stmp.Contains("msgid_plural \"\"") == false)
                                    {
                                        stmp = stmp.Replace("msgid_plural ", "msgstr[1] ");
                                        stmp = stmp.Replace("'", "''");
                                        s = s.Replace("'", "''");
                                        cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + stmp + "','" + s + "')";
                                        cmd.ExecuteNonQuery();
                                        bl = true;
                                        break;
                                    }
                                }
                            }
                            if (bl == false)
                            {
                                if (s.Contains("msgstr[1] ") == true && s.Contains("msgstr[1] \"\"") == false)
                                {
                                    for (int y = 0; y < list3.Count; y++)
                                    {
                                        stmp = list3[y].ToString();
                                        if (stmp.Contains("msgid ") == true && stmp.Contains("msgid \"\"") == false)
                                        {
                                            stmp = stmp.Replace("msgid ", "msgstr[1] ");
                                            stmp = stmp.Replace("'", "''");
                                            s = s.Replace("'", "''");
                                            cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + stmp + "','" + s + "')";
                                            cmd.ExecuteNonQuery();
                                            break;
                                        }
                                    }
                                }
                            }
                            if (s.Contains("msgstr[0] ") == true && s.Contains("msgstr[0] \"\"") == false)
                            {
                                for (int y = 0; y < list3.Count; y++)
                                {
                                    stmp = list3[y].ToString();
                                    if (stmp.Contains("msgid ") == true && stmp.Contains("msgid \"\"") == false)
                                    {
                                        stmp = stmp.Replace("msgid ", "msgstr[0] ");
                                        stmp = stmp.Replace("'", "''");
                                        s = s.Replace("'", "''");
                                        cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + stmp + "','" + s + "')";
                                        cmd.ExecuteNonQuery();
                                        break;
                                    }
                                }
                            }
                            if (s.Contains("msgstr ") == true && s.Contains("msgstr \"\"") == false)
                            {
                                for (int y = 0; y < list3.Count; y++)
                                {
                                    stmp = list3[y].ToString();
                                    if (stmp.Contains("msgid ") == true && stmp.Contains("msgid \"\"") == false)
                                    {
                                        stmp = stmp.Replace("msgid ", "msgstr ");
                                        stmp = stmp.Replace("'", "''");
                                        s = s.Replace("'", "''");
                                        cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + stmp + "','" + s + "')";
                                        cmd.ExecuteNonQuery();
                                        break;
                                    }
                                }
                            }
                        }
                        list3.Clear();
                        list1.Clear();
                    }
                }
            }
            progressBar2.Value = progressBar2.Maximum;
            cmd.Transaction.Commit();
            sr.Close();
            fs.Close();
            MyAccess.Close();
        }

        private void backgroundWorker3_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tabControl1.Enabled = true;
            progressBar2.Value = 0;
            MessageBox.Show("字典保存成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked == true)
            {
                textBox5.Text = "";
                textBox5.Enabled = false;
                button9.Enabled = false;
            }
            else
            {
                textBox5.Enabled = true;
                button9.Enabled = true;
            }
        }
    }
}
