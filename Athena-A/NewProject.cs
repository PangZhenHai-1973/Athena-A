using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class NewProject : Form
    {
        private string Projectname = "";//工程名称
        private string OrgPEFileName = "";//需要进行翻译的 PE 文件
        private string OrgUnPEFileName = "";//需要进行翻译的非 PE 文件
        private ArrayList list1;
        private ArrayList list2;
        private ArrayList list3;//Delphi 标识
        private ArrayList list4;//字符串长度信息
        private ArrayList list11;
        private ArrayList list22;
        private ArrayList list33;//Delphi 标识
        private ArrayList list44;//字符串长度信息
        private ArrayList list111;
        private ArrayList list222;
        private ArrayList list333;//Delphi 标识
        private ArrayList list444;//字符串长度信息
        private ArrayList al1;//起始地址
        private ArrayList al2;//结束地址
        private ArrayList SmartFilterAL = new ArrayList();
        private bool NewSmartBL = true;//智能过滤
        private long PFL = 0;
        private string CPU = "";

        public NewProject()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//确定文件
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                string s1 = mainform.CDirectory + "工程\\文件\\";
                string s2 = s1 + Path.GetFileName(s);
                if (s.Contains(s1) == true)
                {
                    textBox1.Text = s;
                }
                else
                {
                    DialogResult dr = MessageBox.Show("是否把文件复制到工程目录中？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        if (Directory.Exists(s1) == false)
                        {
                            Directory.CreateDirectory(s1);
                        }
                        if (File.Exists(s2) == true)
                        {
                            DialogResult dlrt = MessageBox.Show("工程目录中已存在这个文件，是否覆盖？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (dlrt == DialogResult.OK)
                            {
                                File.Copy(s, s2, true);
                                textBox1.Text = s2;
                            }
                            else
                            {
                                textBox1.Text = s;
                            }
                        }
                        else
                        {
                            File.Copy(s, s2);
                            textBox1.Text = s2;
                        }
                    }
                    else
                    {
                        textBox1.Text = s;
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                label2.Enabled = false;
                label3.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                dataGridView1.Rows.Clear();
                dataGridView1.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                label2.Enabled = true;
                label3.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                dataGridView1.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
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
                    if (i == 2)
                    {
                        textBox2.Text = str1;
                    }
                    else if (i == 3)
                    {
                        textBox3.Text = str1;
                    }
                }
            }
            else
            {
                MessageBox.Show("粘贴板中没有数据，或是其中的数据不是文本内容。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            PasteText(2);
        }

        private void label3_Click(object sender, EventArgs e)
        {
            PasteText(3);
        }

        private int Compare(string s1, string s2)//比较输入的内容和存在的内容之间的关系
        {
            long l1 = CommonCode.HexToLong(s1);
            long l2 = CommonCode.HexToLong(s2);
            string str1 = "";
            string str2 = "";
            long l3 = 0;
            long l4 = 0;
            int i1 = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                str1 = dataGridView1.Rows[i].Cells[0].Value.ToString();
                str2 = dataGridView1.Rows[i].Cells[1].Value.ToString();
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

        private void button4_Click(object sender, EventArgs e)//添加地址
        {
            string str1 = textBox2.Text;
            string str2 = textBox3.Text;
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
                textBox2.Text = "";
            }
            else if (CommonCode.Is_Hex(str2) == false)
            {
                MessageBox.Show("结束地址不是有效的十六进制值，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Text = "";
            }
            else if (CommonCode.HexToLong(str2) == 0)
            {
                MessageBox.Show("结束地址不能为零，请重新输入。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Text = "";
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
                    textBox2.Text = "";
                    textBox3.Text = "";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)//清空所有地址
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

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveANPESetup(string s)//保存配置文件
        {
            ArrayList al = new ArrayList();
            al.Add("File=" + textBox1.Text);
            al.Add("Source=" + comboBox1.Text);
            al.Add("Target=" + comboBox2.Text);
            al.Add("Auto=" + radioButton1.Checked.ToString());
            al.Add("Manual=" + radioButton2.Checked.ToString());
            al.Add("ANSI=" + radioButton3.Checked.ToString());
            al.Add("UTF8=" + radioButton4.Checked.ToString());
            al.Add("Unicode=" + radioButton5.Checked.ToString());
            al.Add("SmartKeep=" + checkBox2.Checked.ToString());
            al.Add("Category=" + comboBox5.Text);
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

        private void button3_Click(object sender, EventArgs e)//保存配置文件
        {
            CommonCode.SetupFolder();//创建配置文件夹
            string s1 = mainform.CDirectory + "配置\\ANPESetup.ini";
            if (checkBox1.Checked == false)
            {
                if (File.Exists(s1) == false)
                {
                    StreamWriter sw = File.CreateText(s1);
                    sw.Close();
                }
                SaveANPESetup(s1);
            }
            else
            {
                string s2 = "";
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = mainform.CDirectory + "配置";
                save.OverwritePrompt = false;
                save.Filter = "配置文件(*.ini)|*.ini";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    s2 = save.FileName;
                    if (s1 == s2)
                    {
                        MessageBox.Show("手动保存的配置文件不能与默认的配置文件相同，请另外指定不同的文件名。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        SaveANPESetup(s2);
                    }
                }
            }
        }

        private void LoadANPESetup(string s)//加载配置文件
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
                textBox1.Text = al[0].ToString().Remove(0, 5);
                comboBox1.Text = al[1].ToString().Remove(0, 7);
                comboBox2.Text = al[2].ToString().Remove(0, 7);
                if (al[3].ToString().Remove(0, 5) == "True")
                {
                    radioButton1.Checked = true;
                }
                else if (al[4].ToString().Remove(0, 7) == "True")
                {
                    radioButton2.Checked = true;
                }
                if (al[5].ToString().Remove(0, 5) == "True")
                {
                    radioButton3.Checked = true;
                }
                else if (al[6].ToString().Remove(0, 5) == "True")
                {
                    radioButton4.Checked = true;
                }
                else if (al[7].ToString().Remove(0, 8) == "True")
                {
                    radioButton5.Checked = true;
                }
                if (al[8].ToString().Remove(0, 10) == "True")
                {
                    checkBox2.Checked = true;
                }
                else
                {
                    checkBox2.Checked = false;
                }
                comboBox5.Text = al[9].ToString().Remove(0, 9);
                int i1 = al.Count;
                dataGridView1.Rows.Clear();
                for (int i = 10; i < i1; i++)
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

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = mainform.CDirectory + "配置";
            string s2 = s1 + "\\ANPESetup.ini";
            if (checkBox1.Checked == false)
            {
                if (Directory.Exists(s1) == true)
                {
                    if (File.Exists(s2) == true)
                    {
                        LoadANPESetup(s2);
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
                    LoadANPESetup(s2);
                }
            }
        }

        private bool File_Length()//比较输入的地址是否超出文件的大小
        {
            FileStream f = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            long l = f.Length - 1;
            long l2 = 0;
            long l3 = 0;
            f.Close();
            bool bl = true;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string str1 = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string str2 = dataGridView1.Rows[i].Cells[1].Value.ToString();
                l2 = CommonCode.HexToLong(str1);
                l3 = CommonCode.HexToLong(str2);
                if (l2 > l)
                {
                    MessageBox.Show("地址范围“" + str1 + "”已超过了文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bl = false;
                    break;
                }
                else if (l3 > l)
                {
                    MessageBox.Show("地址范围“" + str2 + "”已超过了文件的大小，请修正。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bl = false;
                    break;
                }
            }
            return bl;
        }

        private void Create_Project_DB()//创建工程文件
        {
            SQLiteConnection.CreateFile(Projectname);
            using (SQLiteConnection SC = new SQLiteConnection("Data Source=" + Projectname))
            {
                SC.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(SC))
                {
                    SCM.Transaction = SC.BeginTransaction();
                    SCM.CommandText = "CREATE TABLE `athenaa` ("
                        + "`address`	TEXT NOT NULL,"
                        + "`org`	TEXT DEFAULT '',"
                        + "`tra`	TEXT DEFAULT '',"
                        + "`orglong`	INTEGER DEFAULT 0,"
                        + "`tralong`	INTEGER DEFAULT 0,"
                        + "`addr`	TEXT DEFAULT '',"
                        + "`addrlong`	INTEGER DEFAULT 0,"
                        + "`free`	INTEGER DEFAULT 0,"
                        + "`outadd`	TEXT DEFAULT '',"
                        + "`outaddfree`	INTEGER DEFAULT 0,"
                        + "`moveforward`	INTEGER DEFAULT 0,"
                        + "`movebackward`	INTEGER DEFAULT 0,"
                        + "`superlong`	INTEGER DEFAULT 0,"
                        + "`delphi`	INTEGER DEFAULT 0,"
                        + "`utf8`	INTEGER DEFAULT 0,"
                        + "`ucode`	INTEGER DEFAULT 0,"
                        + "`zonebl`	INTEGER DEFAULT 0,"
                        + "`codepage`	INTEGER DEFAULT 0,"
                        + "`delphicodepage`	INTEGER DEFAULT 0,"
                        + "`Ignoretra`	INTEGER DEFAULT 0,"
                        + "`Undefined1`	INTEGER DEFAULT 0,"
                        + "`Undefined2`	INTEGER DEFAULT 0,"
                        + "PRIMARY KEY(address)"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `calladd` ("
                        + "`address`	TEXT DEFAULT '',"
                        + "`cd`	TEXT DEFAULT '',"
                        + "`offset`	INTEGER DEFAULT 0,"
                        + "`bits`	TEXT DEFAULT ''"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `fileinfo` ("
                        + "`infoname`	TEXT DEFAULT '',"
                        + "`detail`	TEXT DEFAULT ''"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('文件','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('版本','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('大小','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('目标','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('非PE类型','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('运行','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('字典','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('标记','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('PE类型','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('长度标识','')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "Insert Into fileinfo (infoname, detail) Values ('代码页','0')";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `matrix` ("
                        + "`mataddress`	TEXT DEFAULT '',"
                        + "`addlong`	INTEGER DEFAULT 0,"
                        + "`freelong`	INTEGER DEFAULT 0"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `matrixzone` ("
                        + "`address`	TEXT DEFAULT '',"
                        + "`mataddress`	TEXT DEFAULT '',"
                        + "`zoneaddress`	TEXT DEFAULT '',"
                        + "`tralong`	INTEGER DEFAULT 0,"
                        + "`delphi`	INTEGER DEFAULT 0,"
                        + "`ucode`	INTEGER DEFAULT 0,"
                        + "`codepage`	INTEGER DEFAULT 0"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `pesec` ("
                        + "`vsize`	TEXT DEFAULT '',"
                        + "`voffset`	TEXT DEFAULT '',"
                        + "`rsize`	TEXT DEFAULT '',"
                        + "`roffset`	TEXT DEFAULT ''"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `prolanguage` ("
                        + "`encoding`	TEXT DEFAULT '',"
                        + "`orgname`	TEXT DEFAULT '',"
                        + "`orgcode`	INTEGER DEFAULT 0,"
                        + "`orgfontname`	TEXT DEFAULT '',"
                        + "`orgfontsize`	INTEGER DEFAULT 0,"
                        + "`traname`	TEXT DEFAULT '',"
                        + "`tracode`	INTEGER DEFAULT 0,"
                        + "`trafontname`	TEXT DEFAULT '',"
                        + "`trafontsize`	INTEGER DEFAULT 0"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `ResolveAddress` ("
                        + "`address`	TEXT NOT NULL,"
                        + "`x32`	TEXT DEFAULT '',"
                        + "`x64`	TEXT DEFAULT '',"
                        + "`codepage`	TEXT DEFAULT '',"
                        + "`SelectedAddress`	INTEGER DEFAULT 0,"
                        + "`orglong`	INTEGER DEFAULT 0"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `Resolve32` ("
                        + "`address`	TEXT NOT NULL,"
                        + "`Raddress`	TEXT DEFAULT ''"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `Resolve64` ("
                        + "`address`	TEXT NOT NULL,"
                        + "`Raddress`	TEXT DEFAULT ''"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.CommandText = "CREATE TABLE `IgnoreTra` ("
                        + "`address`	TEXT NOT NULL,"
                        + "`ignorebl`	INTEGER DEFAULT 0"
                        + ");";
                    SCM.ExecuteNonQuery();
                    SCM.Transaction.Commit();
                }
            }
        }

        private bool JapaneseStr(string s)
        {
            byte[] bt = Encoding.GetEncoding(932).GetBytes(s);
            if (bt.Length == 1)
            {
                if (bt[0] == 0x9 || bt[0] == 0xA || bt[0] == 0xD || bt[0] == 0xE)
                {
                    return true;
                }
                if (bt[0] >= 0x20 && bt[0] <= 0x7E)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (bt[0] == 0x81)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0xAC)
                    {
                        return true;
                    }
                    if (bt[1] == 0xB8 || bt[1] == 0xBE || bt[1] == 0xBF)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xC8 && bt[1] <= 0xCA)
                    {
                        return true;
                    }
                    if (bt[1] > 0xDA && bt[1] < 0xDC)
                    {
                        return true;
                    }
                    if (bt[1] == 0xDF && bt[1] == 0xE0)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xE3 && bt[1] <= 0xE7)
                    {
                        return true;
                    }
                    if (bt[1] == 0xF1)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xF5 && bt[1] <= 0xF7)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0x82)
                {
                    if (bt[1] >= 0x4F && bt[1] <= 0x58)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x60 && bt[1] <= 0x79)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x81 && bt[1] <= 0x9A)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x9F && bt[1] <= 0xF1)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0x83)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0x96)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x9F && bt[1] <= 0xB6)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xBF && bt[1] <= 0xD6)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0x84)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0x60)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x70 && bt[1] <= 0x7E)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x80 && bt[1] <= 0x91)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x9F && bt[1] <= 0xBE)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0x87)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0x49)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x54 && bt[1] <= 0x5D)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x6F && bt[1] <= 0x75)
                    {
                        return true;
                    }
                    if (bt[1] == 0x80 || bt[1] == 0x82 || bt[1] == 0x84 || bt[1] == 0x8A)
                    {
                        return true;
                    }
                    if (bt[1] == 0x93 || bt[1] == 0x94 || bt[1] == 0x98 || bt[1] == 0x99)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0x88)
                {
                    if (bt[1] >= 0x9F && bt[1] <= 0xFC)
                    {
                        return true;
                    }
                }
                if (bt[0] >= 0x89 && bt[0] <= 0x9F)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0xFC)
                    {
                        return true;
                    }
                }
                if (bt[0] >= 0xE0 && bt[0] <= 0xE9)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0xFC)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xEA)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0xA4)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xFA)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0x49)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x55 && bt[1] <= 0x57)
                    {
                        return true;
                    }
                    if (bt[1] >= 0x5C && bt[1] <= 0xFC)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xFB)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0xFC)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xFC)
                {
                    if (bt[1] >= 0x40 && bt[1] <= 0x4B)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private bool KoreanStr(string s)//韩文
        {
            byte[] bt = Encoding.GetEncoding(949).GetBytes(s);
            if (bt.Length == 1)
            {
                if (bt[0] == 0x9 || bt[0] == 0xA || bt[0] == 0xD || bt[0] == 0xE)
                {
                    return true;
                }
                if (bt[0] >= 0x20 && bt[0] <= 0x7E)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (bt[0] == 0xA1)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xA8)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAA && bt[1] <= 0xAC)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAE && bt[1] <= 0xC9)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xCB && bt[1] <= 0xD2)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xD5 && bt[1] <= 0xE9)
                    {
                        return true;
                    }
                    if (bt[1] == 0xEB)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xEE && bt[1] <= 0xF2)
                    {
                        return true;
                    }
                    if (bt[1] == 0xF4)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xFA && bt[1] <= 0xFE)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA2)
                {
                    if (bt[1] >= 0xA5 && bt[1] <= 0xA7)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAB && bt[1] <= 0xAC)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAE && bt[1] <= 0xAF)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xB1 && bt[1] <= 0xB6)
                    {
                        return true;
                    }
                    if (bt[1] == 0xC1)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xD2 && bt[1] <= 0xD4)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xD6 && bt[1] <= 0xD9)
                    {
                        return true;
                    }
                    if (bt[1] == 0xE0)
                    {
                        return true;
                    }
                    if (bt[1] == 0xE2)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xE5 && bt[1] <= 0xE7)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA3)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xDB)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xDD && bt[1] <= 0xFE)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA5)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xAA)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xB0 && bt[1] <= 0xB9)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xC1 && bt[1] <= 0xD8)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xE1 && bt[1] <= 0xF8)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA6)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xE4)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA7)
                {
                    if (bt[1] == 0xA6)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAE && bt[1] <= 0xB0)
                    {
                        return true;
                    }
                    if (bt[1] == 0xB3 || bt[1] == 0xB7 || bt[1] == 0xB8)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA8)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xA3)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAA && bt[1] <= 0xAD)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xE7 && bt[1] <= 0xF0)
                    {
                        return true;
                    }
                    if (bt[1] == 0xF6 || bt[1] == 0xF9 || bt[1] == 0xFA)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xA9)
                {
                    if (bt[1] == 0xA1 || bt[1] == 0xA3)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xAA && bt[1] <= 0xAD)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xE7 && bt[1] <= 0xF8)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xAA)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xF3)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xAB)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xF6)
                    {
                        return true;
                    }
                }
                if (bt[0] == 0xAC)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xC1)
                    {
                        return true;
                    }
                    if (bt[1] >= 0xD1 && bt[1] <= 0xF1)
                    {
                        return true;
                    }
                }
                if (bt[0] >= 0xCA && bt[0] <= 0xFD)
                {
                    if (bt[1] >= 0xA1 && bt[1] <= 0xFE)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void LoadFilter()
        {
            string s1 = mainform.CDirectory + "保留\\Filter.ini";
            if (File.Exists(s1) == true)
            {
                using (StreamReader sr = File.OpenText(s1))
                {
                    s1 = sr.ReadLine();
                }
                if (File.Exists(s1) == true)
                {
                    if (SmartFilterAL.Count > 0)
                    {
                        NewSmartBL = true;
                    }
                    else
                    {
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                        {
                            try
                            {
                                MyAccess.Open();
                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                {
                                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                                    {
                                        cmd.CommandText = "select filterstr from filter";
                                        using (DataTable DTmp = new DataTable("filter"))
                                        {
                                            using (DataColumn DC = new DataColumn("filterstr"))
                                            {
                                                DTmp.Columns.Add(DC);
                                                ad.Fill(DTmp);
                                                int i1 = DTmp.Rows.Count;
                                                if (i1 > 0)
                                                {
                                                    for (int i = 0; i < i1; i++)
                                                    {
                                                        SmartFilterAL.Add(DTmp.Rows[i][0].ToString());
                                                    }
                                                    NewSmartBL = true;
                                                }
                                                else
                                                {
                                                    NewSmartBL = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                NewSmartBL = false;
                            }
                        }
                    }
                }
                else
                {
                    NewSmartBL = false;
                }
            }
            else
            {
                NewSmartBL = false;
            }
        }

        private void Analyze_PE()//提取并保存 PE 头文件信息
        {
            ArrayList lt1 = new ArrayList();
            ArrayList lt2 = new ArrayList();
            ArrayList lt3 = new ArrayList();
            ArrayList lt4 = new ArrayList();
            int i1 = 0;
            using (FileStream fs = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
                    uint u1 = br.ReadUInt32();//PE标识位置
                    fs.Seek(u1 + 4, SeekOrigin.Begin);//CPU 类型
                    i1 = br.ReadUInt16();//读出 CPU 类型
                    if (i1 == 332)
                    {
                        CPU = "32";
                        fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                        i1 = br.ReadUInt16();//文件段数
                        fs.Seek(u1 + 52, SeekOrigin.Begin);//基址
                        lt1.Add(br.ReadUInt32().ToString());//基址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 128, SeekOrigin.Begin);//输入表
                        lt1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 136, SeekOrigin.Begin);//资源段
                        lt1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 248, SeekOrigin.Begin);//各个段
                        for (int i = 0; i < i1; i++)
                        {
                            fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                            lt1.Add(br.ReadUInt32().ToString());
                            lt2.Add(br.ReadUInt32().ToString());
                            lt3.Add(br.ReadUInt32().ToString());
                            lt4.Add(br.ReadUInt32().ToString());
                            fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                        }
                    }
                    else
                    {
                        CPU = "64";
                        fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                        i1 = br.ReadUInt16();//文件段数
                        fs.Seek(u1 + 48, SeekOrigin.Begin);//基址
                        lt1.Add(br.ReadUInt64().ToString());//基址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 144, SeekOrigin.Begin);//输入表
                        lt1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 152, SeekOrigin.Begin);//资源段
                        lt1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                        lt2.Add("0");
                        lt3.Add("0");
                        lt4.Add("0");
                        fs.Seek(u1 + 264, SeekOrigin.Begin);//各个段
                        for (int i = 0; i < i1; i++)
                        {
                            fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                            lt1.Add(br.ReadUInt32().ToString());
                            lt2.Add(br.ReadUInt32().ToString());
                            lt3.Add(br.ReadUInt32().ToString());
                            lt4.Add(br.ReadUInt32().ToString());
                            fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                        }
                    }
                }
            }
            i1 = lt1.Count;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        cmd.CommandText = "Insert Into pesec (vsize,voffset,rsize,roffset) Values (" + lt1[i].ToString() + "," + lt2[i].ToString() + "," + lt3[i].ToString() + "," + lt4[i].ToString() + ")";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
            }
            lt1 = null;
            lt2 = null;
            lt3 = null;
            lt4 = null;
        }

        private void button6_Click(object sender, EventArgs e)//PE 新建工程
        {
            if (checkBox2.Checked == true && checkBox2.Enabled == true)
            {
                LoadFilter();
            }
            else
            {
                NewSmartBL = false;
            }
            OrgPEFileName = textBox1.Text;//文件位置
            CommonCode.DictionaryFolder();//创建工程文件夹和字典文件夹
            list1 = new ArrayList();//字符串地址
            list2 = new ArrayList();//字符串
            list3 = new ArrayList();//Delphi 标识
            list4 = new ArrayList();//字符串长度信息
            list11 = new ArrayList();
            list22 = new ArrayList();
            list33 = new ArrayList();
            list44 = new ArrayList();
            list111 = new ArrayList();
            list222 = new ArrayList();
            list333 = new ArrayList();
            list444 = new ArrayList();
            al1 = new ArrayList();//提取文件开始范围
            al2 = new ArrayList();//提取文件结束范围
            if (OrgPEFileName == "")
            {
                MessageBox.Show("请指定需要提取的文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(OrgPEFileName) == false)
            {
                MessageBox.Show("指定的文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (CommonCode.PE(OrgPEFileName) == false)
            {
                MessageBox.Show("指定的文件不是 PE 文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Projectname = mainform.CDirectory + "工程\\" + Path.GetFileNameWithoutExtension(OrgPEFileName);
                if (radioButton3.Checked == true)
                {
                    Projectname = Projectname + " ANSI " + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
                }
                else if (radioButton4.Checked == true)
                {
                    Projectname = Projectname + " UTF8 " + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
                }
                else if (radioButton5.Checked == true)
                {
                    Projectname = Projectname + " Unicode " + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
                }
                if (File.Exists(Projectname) == true)
                {
                    MessageBox.Show("名称为“" + Path.GetFileName(Projectname) + "”的工程文件已存在，请稍候再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    progressBar1.Maximum = 20;
                    if (radioButton2.Checked == true)//手动提取
                    {
                        if (dataGridView1.Rows.Count == 0)
                        {
                            MessageBox.Show("手动方式请指定地址范围。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (File_Length() == true)
                        {
                            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                            {
                                al1.Add(dataGridView1.Rows[i].Cells[0].Value);//开始
                                al2.Add(dataGridView1.Rows[i].Cells[1].Value);//结束
                            }
                            tabControl1.Enabled = false;
                            Create_Project_DB();
                            Analyze_PE();
                            bool bl = false;
                            string codestr1 = comboBox1.Text;//原始语言
                            if (comboBox5.Text == "无")
                            {
                                if (radioButton3.Checked == true)//ANSI 工程
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_None_English.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_None_French.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_None_German.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_None_Korean.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_None_Japanese.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                }
                                else if (radioButton4.Checked == true)//UTF-8 工程
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        UTF8_None_English.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                }
                                else if (radioButton5.Checked == true)//Unicode 工程
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        Unicode_None_English.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                }
                            }
                            else if (comboBox5.Text == "标准")
                            {
                                if (radioButton3.Checked == true)//ANSI 工程
                                {
                                    MessageBox.Show("不可用");
                                    tabControl1.Enabled = true;
                                    bl = true;
                                }
                                else if (radioButton4.Checked == true)//UTF-8 工程
                                {
                                    MessageBox.Show("不可用");
                                    tabControl1.Enabled = true;
                                    bl = true;
                                }
                                else if (radioButton5.Checked == true)//Unicode 工程
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        Unicode_Standard1_English.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                }
                            }
                            else if (comboBox5.Text == "标准2")
                            {
                                if (radioButton3.Checked == true)//ANSI 工程
                                {
                                    MessageBox.Show("不可用");
                                    tabControl1.Enabled = true;
                                    bl = true;
                                }
                                else if (radioButton4.Checked == true)//UTF-8 工程
                                {
                                    MessageBox.Show("不可用");
                                    tabControl1.Enabled = true;
                                    bl = true;
                                }
                                else if (radioButton5.Checked == true)//Unicode 工程
                                {
                                    if (codestr1 == "英语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        Unicode_Standard2_English.RunWorkerAsync();
                                    }
                                    else if (codestr1 == "法语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "德语(1252)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "俄语(1251)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "韩文(949)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "日语(932)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "简体中文(936)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                    else if (codestr1 == "繁体中文(950)")
                                    {
                                        MessageBox.Show("不可用");
                                        tabControl1.Enabled = true;
                                        bl = true;
                                    }
                                }
                            }
                            else if (comboBox5.Text == "Delphi")
                            {
                                if (radioButton3.Checked == true)//ANSI 工程
                                {
                                    if (comboBox1.Text == "英语(1252)" || comboBox1.Text == "法语(1252)" || comboBox1.Text == "德语(1252)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_Delphi_English.RunWorkerAsync();
                                    }
                                    else if (comboBox1.Text == "日语(932)" || comboBox1.Text == "韩文(949)")
                                    {
                                        PENewProjectTimer.Enabled = true;
                                        ANSI_Delphi_Japanese_Korean.RunWorkerAsync();
                                    }
                                }
                                else if (radioButton5.Checked == true)//Unicode 工程
                                {
                                    PENewProjectTimer.Enabled = true;
                                    Unicode_Delphi.RunWorkerAsync();
                                }
                            }
                            if (bl)
                            {
                                try
                                {
                                    File.Delete(Projectname);
                                    Projectname = "";
                                }
                                catch
                                { }
                            }
                        }
                    }
                    else//自动提取
                    {
                        AutoPE();
                    }
                }
            }
        }

        private void AutoPE()
        {
            tabControl1.Enabled = false;
            ArrayList t1 = new ArrayList();
            ArrayList t2 = new ArrayList();
            ArrayList t3 = new ArrayList();
            ArrayList t4 = new ArrayList();
            string s1 = "";
            string s2 = "";
            FileStream fs = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            long L = fs.Length - 1;//文件大小
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
            uint u1 = br.ReadUInt32();//PE标识位置
            fs.Seek(u1 + 4, SeekOrigin.Begin);//CPU 类型
            int i1 = br.ReadUInt16();//读出 CPU 类型
            if (i1 == 332)
            {
                CPU = "32";
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
                CPU = "64";
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
            Create_Project_DB();
            bool bl = false;
            using (SQLiteConnection SC = new SQLiteConnection("Data Source=" + Projectname))
            {
                SC.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(SC))
                {
                    SCM.Transaction = SC.BeginTransaction();
                    for (int i = 0; i < t1.Count; i++)
                    {
                        SCM.CommandText = "Insert Into pesec (vsize,voffset,rsize,roffset) Values (" + t1[i].ToString() + "," + t2[i].ToString() + "," + t3[i].ToString() + "," + t4[i].ToString() + ")";
                        SCM.ExecuteNonQuery();
                    }
                    SCM.Transaction.Commit();
                    SC.Close();
                    t1.Clear();
                    t2.Clear();
                    t3.Clear();
                    t4.Clear();
                    string codestr1 = comboBox1.Text;
                    if (comboBox5.Text == "无")
                    {
                        if (radioButton3.Checked == true)//ANSI 工程
                        {
                            if (codestr1 == "英语(1252)")//完成
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_None_English.RunWorkerAsync();
                            }
                            else if (codestr1 == "法语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_None_French.RunWorkerAsync();
                            }
                            else if (codestr1 == "德语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_None_German.RunWorkerAsync();
                            }
                            else if (codestr1 == "俄语(1251)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "韩文(949)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_None_Korean.RunWorkerAsync();
                            }
                            else if (codestr1 == "日语(932)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_None_Japanese.RunWorkerAsync();
                            }
                            else if (codestr1 == "简体中文(936)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "繁体中文(950)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                        }
                        else if (radioButton4.Checked == true)//UTF-8 工程
                        {
                            if (codestr1 == "英语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                UTF8_None_English.RunWorkerAsync();
                            }
                            else if (codestr1 == "法语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "德语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "俄语(1251)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "韩文(949)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "日语(932)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "简体中文(936)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "繁体中文(950)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                        }
                        else if (radioButton5.Checked == true)//Unicode 工程
                        {
                            if (codestr1 == "英语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                Unicode_None_English.RunWorkerAsync();
                            }
                            else if (codestr1 == "法语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "德语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "俄语(1251)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "韩文(949)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "日语(932)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "简体中文(936)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "繁体中文(950)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                        }
                    }
                    else if (comboBox5.Text == "标准")
                    {
                        if (radioButton3.Checked == true)
                        {
                            MessageBox.Show("不可用");
                            tabControl1.Enabled = true;
                            bl = true;
                        }
                        else if (radioButton4.Checked == true)
                        {
                            MessageBox.Show("不可用");
                            tabControl1.Enabled = true;
                            bl = true;
                        }
                        else if (radioButton5.Checked == true)
                        {
                            if (codestr1 == "英语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                Unicode_Standard1_English.RunWorkerAsync();
                            }
                            else if (codestr1 == "法语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "德语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "俄语(1251)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "韩文(949)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "日语(932)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "简体中文(936)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "繁体中文(950)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                        }
                    }
                    else if (comboBox5.Text == "标准2")
                    {
                        if (radioButton3.Checked == true)
                        {
                            MessageBox.Show("不可用");
                            tabControl1.Enabled = true;
                            bl = true;
                        }
                        else if (radioButton4.Checked == true)
                        {
                            MessageBox.Show("不可用");
                            tabControl1.Enabled = true;
                            bl = true;
                        }
                        else if (radioButton5.Checked == true)
                        {
                            if (codestr1 == "英语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                Unicode_Standard2_English.RunWorkerAsync();
                            }
                            else if (codestr1 == "法语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "德语(1252)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "俄语(1251)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "韩文(949)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "日语(932)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "简体中文(936)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                            else if (codestr1 == "繁体中文(950)")
                            {
                                MessageBox.Show("不可用");
                                tabControl1.Enabled = true;
                                bl = true;
                            }
                        }
                    }
                    else if (comboBox5.Text == "Delphi")
                    {
                        if (radioButton3.Checked == true)
                        {
                            if (comboBox1.Text == "英语(1252)" || comboBox1.Text == "法语(1252)" || comboBox1.Text == "德语(1252)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_Delphi_English.RunWorkerAsync();
                            }
                            else if (comboBox1.Text == "日语(932)" || comboBox1.Text == "韩文(949)")
                            {
                                PENewProjectTimer.Enabled = true;
                                ANSI_Delphi_Japanese_Korean.RunWorkerAsync();
                            }
                        }
                        else if (radioButton5.Checked == true)
                        {
                            PENewProjectTimer.Enabled = true;
                            Unicode_Delphi.RunWorkerAsync();
                        }
                    }
                }
            }
            if (bl)
            {
                try
                {
                    File.Delete(Projectname);
                }
                catch
                { }
            }
        }

        private void ANSI_None_English_DoWork(object sender, DoWorkEventArgs e)//自动提取无长度标识
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                        if (i2 > 1 && b1 == 0)
                                        {
                                            bl = false;
                                            bList.RemoveLast();
                                            if (i2 == 2 && i3 == 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                            {
                                                bl = true;
                                            }
                                            if (bl)
                                            {
                                                if (i2 < 4)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                    {
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(s1);
                                                        list4.Add(i2);
                                                    }
                                                }
                                                else
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(s1);
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(s1);
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                        if (i2 > 1 && b1 == 0)
                                        {
                                            bl = false;
                                            bList.RemoveLast();
                                            if (i2 == 2 && i3 == 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                            {
                                                bl = true;
                                            }
                                            if (bl)
                                            {
                                                byte[] bt = new byte[i2];
                                                bList.CopyTo(bt, 0);
                                                list1.Add(l3.ToString("X8"));
                                                list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                list4.Add(i2);
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list11.Add(l3.ToString("X8"));
                                                        list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                        list44.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list111.Add(l3.ToString("X8"));
                                                        list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                        list444.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_None_PeSave();
            }
        }

        private void ANSI_None_PeSave()//ANSI无字符串长度标识保存
        {
            int orgcode = 0;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)" || orgname == "德语(1252)" || orgname == "法语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "俄语(1251)")
            {
                orgcode = 1251;
            }
            else if (orgname == "韩文(949)")
            {
                orgcode = 949;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            else if (orgname == "简体中文(936)")
            {
                orgcode = 936;
            }
            else if (orgname == "繁体中文(950)")
            {
                orgcode = 950;
            }
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
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('ANSI','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    for (int i = 0; i < i11; i++)
                    {
                        s1 = list11[i].ToString();
                        s2 = list22[i].ToString();
                        i2 = (int)list44[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i111 = list111.Count;
                    for (int i = 0; i < i111; i++)
                    {
                        s1 = list111[i].ToString();
                        s2 = list222[i].ToString();
                        i2 = (int)list444[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void ANSI_None_French_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                        if (b1 == 0 && i2 > 1)
                                        {
                                            bList.RemoveLast();
                                            if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                            {
                                                if ((i3 / i2) < 0.7)
                                                {
                                                    if (i2 < 4)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                        {
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                        list4.Add(i2);
                                                    }
                                                }
                                            }
                                            else if (i2 > 5)
                                            {
                                                if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            if (i2 < 4)
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            if (i2 < 4)
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                        if (b1 == 0 && i2 > 1)
                                        {
                                            bList.RemoveLast();
                                            if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                            {
                                                if ((i3 / i2) < 0.7)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                            else if (i2 > 5)
                                            {
                                                if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0x8B) || (b1 >= 0x8D && b1 <= 0x9B) || (b1 >= 0x9D && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 192 || b1 == 194 || (b1 >= 198 && b1 <= 203) || b1 == 206 || b1 == 207 || b1 == 212 || b1 == 214 || b1 == 217 || b1 == 219 || b1 == 220 || b1 == 224 || b1 == 226 || (b1 >= 230 && b1 <= 235) || b1 == 238 || b1 == 239 || b1 == 244 || b1 == 246 || b1 == 249 || b1 == 251 || b1 == 252);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_None_PeSave();
            }
        }

        private void ANSI_None_German_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                        if (b1 == 0 && i2 > 1)
                                        {
                                            bList.RemoveLast();
                                            if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                            {
                                                if ((i3 / i2) < 0.7)
                                                {
                                                    if (i2 < 4)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                        {
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                        list4.Add(i2);
                                                    }
                                                }
                                            }
                                            else if (i2 > 5)
                                            {
                                                if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            if (i2 < 4)
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            if (i2 < 4)
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                byte[] bt = new byte[i2];
                                                                bList.CopyTo(bt, 0);
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                        if (b1 == 0 && i2 > 1)
                                        {
                                            bList.RemoveLast();
                                            if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                            {
                                                if ((i3 / i2) < 0.7)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                            else if (i2 > 5)
                                            {
                                                if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                int i4 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 > 31 && b1 < 253) || b1 == 10 || b1 == 13 || b1 == 9)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x81 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7 || b1 == 0xC4 || b1 == 0xD6 || b1 == 0xDC || b1 == 0xDF || b1 == 0xE4 || b1 == 0xF6 || b1 == 0xFC);
                                                if (b1 == 0 && i2 > 1)
                                                {
                                                    bList.RemoveLast();
                                                    if (i2 >= 2 && i2 <= 5 && i4 == 0)
                                                    {
                                                        if ((i3 / i2) < 0.7)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                    else if (i2 > 5)
                                                    {
                                                        if ((i3 / i2) < 0.3 && (i4 / i2) < 0.2)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.GetEncoding(1252).GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_None_PeSave();
            }
        }

        private void ANSI_None_Japanese_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if (b1 > 0)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        bList.Clear();
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            bList.AddLast(b1);
                                        } while (b1 > 0);
                                        if (i2 > 2)
                                        {
                                            bList.RemoveLast();
                                            byte[] bt = new byte[i2];
                                            bList.CopyTo(bt, 0);
                                            string s1 = Encoding.GetEncoding(932).GetString(bt);
                                            if (s1.Contains("ﾌﾌﾌ") == false)
                                            {
                                                i3 = s1.Length - 1;
                                                for (int x = i3; x >= 0; x--)
                                                {
                                                    if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                    {
                                                        s1 = s1.Substring(x + 1);
                                                        if (s1.Length > 1)
                                                        {
                                                            if (s1.Length < 4)
                                                            {
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list1.Add(l3.ToString("X8"));
                                                                    list2.Add(s1);
                                                                    list4.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                l3 = l3 + i2;
                                                                i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                l3 = l3 - i2;
                                                                list1.Add(l3.ToString("X8"));
                                                                list2.Add(s1);
                                                                list4.Add(i2);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else if (x == 0)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list1.Add(l3.ToString("X8"));
                                                                list2.Add(s1);
                                                                list4.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (i2 > 2)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.GetEncoding(932).GetString(bt);
                                                    if (s1.Contains("ﾌﾌﾌ") == false)
                                                    {
                                                        i3 = s1.Length - 1;
                                                        for (int x = i3; x >= 0; x--)
                                                        {
                                                            if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    if (s1.Length < 4)
                                                                    {
                                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                                        {
                                                                            l3 = l3 + i2;
                                                                            i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                            l3 = l3 - i2;
                                                                            list11.Add(l3.ToString("X8"));
                                                                            list22.Add(s1);
                                                                            list44.Add(i2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        l3 = l3 + i2;
                                                                        i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                        l3 = l3 - i2;
                                                                        list11.Add(l3.ToString("X8"));
                                                                        list22.Add(s1);
                                                                        list44.Add(i2);
                                                                    }
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                if (i2 < 4)
                                                                {
                                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                                    {
                                                                        list11.Add(l3.ToString("X8"));
                                                                        list22.Add(s1);
                                                                        list44.Add(i2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (i2 > 2)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.GetEncoding(932).GetString(bt);
                                                    if (s1.Contains("ﾌﾌﾌ") == false)
                                                    {
                                                        i3 = s1.Length - 1;
                                                        for (int x = i3; x >= 0; x--)
                                                        {
                                                            if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    if (s1.Length < 4)
                                                                    {
                                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                                        {
                                                                            l3 = l3 + i2;
                                                                            i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                            l3 = l3 - i2;
                                                                            list111.Add(l3.ToString("X8"));
                                                                            list222.Add(s1);
                                                                            list444.Add(i2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        l3 = l3 + i2;
                                                                        i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                        l3 = l3 - i2;
                                                                        list111.Add(l3.ToString("X8"));
                                                                        list222.Add(s1);
                                                                        list444.Add(i2);
                                                                    }
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                if (i2 < 4)
                                                                {
                                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                                    {
                                                                        list111.Add(l3.ToString("X8"));
                                                                        list222.Add(s1);
                                                                        list444.Add(i2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if (b1 > 0)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        bList.Clear();
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            bList.AddLast(b1);
                                        } while (b1 > 0);
                                        if (i2 > 2)
                                        {
                                            bList.RemoveLast();
                                            byte[] bt = new byte[i2];
                                            bList.CopyTo(bt, 0);
                                            string s1 = Encoding.GetEncoding(932).GetString(bt);
                                            if (s1.Contains("ﾌﾌﾌ") == false)
                                            {
                                                i3 = s1.Length - 1;
                                                for (int x = i3; x >= 0; x--)
                                                {
                                                    if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                    {
                                                        s1 = s1.Substring(x + 1);
                                                        if (s1.Length > 1)
                                                        {
                                                            l3 = l3 + i2;
                                                            i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                            l3 = l3 - i2;
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else if (x == 0)
                                                    {
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(s1);
                                                        list4.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (i2 > 2)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.GetEncoding(932).GetString(bt);
                                                    if (s1.Contains("ﾌﾌﾌ") == false)
                                                    {
                                                        i3 = s1.Length - 1;
                                                        for (int x = i3; x >= 0; x--)
                                                        {
                                                            if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(s1);
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                int i3 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (i2 > 2)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.GetEncoding(932).GetString(bt);
                                                    if (s1.Contains("ﾌﾌﾌ") == false)
                                                    {
                                                        i3 = s1.Length - 1;
                                                        for (int x = i3; x >= 0; x--)
                                                        {
                                                            if (JapaneseStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(s1);
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_None_PeSave();
            }
        }

        private void ANSI_None_Korean_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if (b1 > 0)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        bList.Clear();
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            bList.AddLast(b1);
                                        } while (b1 > 0);
                                        if (b1 == 0)
                                        {
                                            if (i2 > 2)
                                            {
                                                bList.RemoveLast();
                                                byte[] bt = new byte[i2];
                                                bList.CopyTo(bt, 0);
                                                string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                for (int x = s1.Length - 1; x >= 0; x--)
                                                {
                                                    if (KoreanStr(s1.Substring(x, 1)) == false)
                                                    {
                                                        s1 = s1.Substring(x + 1);
                                                        if (s1.Length > 1)
                                                        {
                                                            if (s1.Length < 4)
                                                            {
                                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list1.Add(l3.ToString("X8"));
                                                                    list2.Add(s1);
                                                                    list4.Add(i2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                l3 = l3 + i2;
                                                                i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                l3 = l3 - i2;
                                                                list1.Add(l3.ToString("X8"));
                                                                list2.Add(s1);
                                                                list4.Add(i2);
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else if (x == 0)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list1.Add(l3.ToString("X8"));
                                                                list2.Add(s1);
                                                                list4.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (b1 == 0)
                                                {
                                                    if (i2 > 2)
                                                    {
                                                        bList.RemoveLast();
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                        for (int x = s1.Length - 1; x >= 0; x--)
                                                        {
                                                            if (KoreanStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    if (s1.Length < 4)
                                                                    {
                                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                                        {
                                                                            l3 = l3 + i2;
                                                                            i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                            l3 = l3 - i2;
                                                                            list11.Add(l3.ToString("X8"));
                                                                            list22.Add(s1);
                                                                            list44.Add(i2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        l3 = l3 + i2;
                                                                        i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                        l3 = l3 - i2;
                                                                        list11.Add(l3.ToString("X8"));
                                                                        list22.Add(s1);
                                                                        list44.Add(i2);
                                                                    }
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                if (i2 < 4)
                                                                {
                                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                                    {
                                                                        list11.Add(l3.ToString("X8"));
                                                                        list22.Add(s1);
                                                                        list44.Add(i2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (b1 == 0)
                                                {
                                                    if (i2 > 2)
                                                    {
                                                        bList.RemoveLast();
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                        for (int x = s1.Length - 1; x >= 0; x--)
                                                        {
                                                            if (KoreanStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    if (s1.Length < 4)
                                                                    {
                                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                                        {
                                                                            l3 = l3 + i2;
                                                                            i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                            l3 = l3 - i2;
                                                                            list111.Add(l3.ToString("X8"));
                                                                            list222.Add(s1);
                                                                            list444.Add(i2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        l3 = l3 + i2;
                                                                        i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                        l3 = l3 - i2;
                                                                        list111.Add(l3.ToString("X8"));
                                                                        list222.Add(s1);
                                                                        list444.Add(i2);
                                                                    }
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                if (i2 < 4)
                                                                {
                                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                                    {
                                                                        list111.Add(l3.ToString("X8"));
                                                                        list222.Add(s1);
                                                                        list444.Add(i2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if (b1 > 0)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        bList.Clear();
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            bList.AddLast(b1);
                                        } while (b1 > 0);
                                        if (b1 == 0)
                                        {
                                            if (i2 > 2)
                                            {
                                                bList.RemoveLast();
                                                byte[] bt = new byte[i2];
                                                bList.CopyTo(bt, 0);
                                                string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                for (int x = s1.Length - 1; x >= 0; x--)
                                                {
                                                    if (KoreanStr(s1.Substring(x, 1)) == false)
                                                    {
                                                        s1 = s1.Substring(x + 1);
                                                        if (s1.Length > 1)
                                                        {
                                                            l3 = l3 + i2;
                                                            i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                            l3 = l3 - i2;
                                                            list1.Add(l3.ToString("X8"));
                                                            list2.Add(s1);
                                                            list4.Add(i2);
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else if (x == 0)
                                                    {
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(s1);
                                                        list4.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (b1 == 0)
                                                {
                                                    if (i2 > 2)
                                                    {
                                                        bList.RemoveLast();
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                        for (int x = s1.Length - 1; x >= 0; x--)
                                                        {
                                                            if (KoreanStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list11.Add(l3.ToString("X8"));
                                                                    list22.Add(s1);
                                                                    list44.Add(i2);
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(s1);
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i2 = 0;
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if (b1 > 0)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                bList.Clear();
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    bList.AddLast(b1);
                                                } while (b1 > 0);
                                                if (b1 == 0)
                                                {
                                                    if (i2 > 2)
                                                    {
                                                        bList.RemoveLast();
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        string s1 = Encoding.GetEncoding(949).GetString(bt);
                                                        for (int x = s1.Length - 1; x >= 0; x--)
                                                        {
                                                            if (KoreanStr(s1.Substring(x, 1)) == false)
                                                            {
                                                                s1 = s1.Substring(x + 1);
                                                                if (s1.Length > 1)
                                                                {
                                                                    l3 = l3 + i2;
                                                                    i2 = Encoding.GetEncoding(949).GetByteCount(s1);
                                                                    l3 = l3 - i2;
                                                                    list111.Add(l3.ToString("X8"));
                                                                    list222.Add(s1);
                                                                    list444.Add(i2);
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else if (x == 0)
                                                            {
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(s1);
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_None_PeSave();
            }
        }

        private void UTF8_None_English_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                        if (i2 > 1 && b1 == 0)
                                        {
                                            bl = false;
                                            bList.RemoveLast();
                                            if (i2 == 2 && i3 == 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                            {
                                                bl = true;
                                            }
                                            if (bl)
                                            {
                                                if (i2 < 4)
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    string s1 = Encoding.UTF8.GetString(bt);
                                                    if (SmartFilterAL.Contains(s1.ToLower()))
                                                    {
                                                        list1.Add(l3.ToString("X8"));
                                                        list2.Add(s1);
                                                        list4.Add(i2);
                                                    }
                                                }
                                                else
                                                {
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(Encoding.UTF8.GetString(bt));
                                                    list4.Add(i2);
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            string s1 = Encoding.UTF8.GetString(bt);
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list11.Add(l3.ToString("X8"));
                                                                list22.Add(s1);
                                                                list44.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(Encoding.UTF8.GetString(bt));
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        if (i2 < 4)
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            string s1 = Encoding.UTF8.GetString(bt);
                                                            if (SmartFilterAL.Contains(s1.ToLower()))
                                                            {
                                                                list111.Add(l3.ToString("X8"));
                                                                list222.Add(s1);
                                                                list444.Add(i2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            byte[] bt = new byte[i2];
                                                            bList.CopyTo(bt, 0);
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(Encoding.UTF8.GetString(bt));
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    b1 = br1.ReadByte();
                                    if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                    {
                                        l3 = fs1.Position - 1;
                                        i2 = 0;
                                        i3 = 0;
                                        i4 = 0;
                                        bList.Clear();
                                        if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                        {
                                            i3++;
                                        }
                                        else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                        {
                                            i4++;
                                        }
                                        bList.AddLast(b1);
                                        do
                                        {
                                            i2++;
                                            b1 = br1.ReadByte();
                                            if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                            {
                                                i3++;
                                            }
                                            else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                            {
                                                i4++;
                                            }
                                            bList.AddLast(b1);
                                        } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                        if (i2 > 1 && b1 == 0)
                                        {
                                            bl = false;
                                            bList.RemoveLast();
                                            if (i2 == 2 && i3 == 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                            {
                                                bl = true;
                                            }
                                            else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                            {
                                                bl = true;
                                            }
                                            if (bl)
                                            {
                                                byte[] bt = new byte[i2];
                                                bList.CopyTo(bt, 0);
                                                list1.Add(l3.ToString("X8"));
                                                list2.Add(Encoding.UTF8.GetString(bt));
                                                list4.Add(i2);
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            b1 = br2.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs2.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br2.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list11.Add(l3.ToString("X8"));
                                                        list22.Add(Encoding.UTF8.GetString(bt));
                                                        list44.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                bool bl = false;
                                int i2 = 0;//总计数
                                int i3 = 0;//常规符号计数
                                int i4 = 0;//特殊符号计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                byte b1 = 0;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            b1 = br3.ReadByte();
                                            if ((b1 >= 0x20 && b1 <= 0x7E) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                            {
                                                l3 = fs3.Position - 1;
                                                i2 = 0;
                                                i3 = 0;
                                                i4 = 0;
                                                bList.Clear();
                                                if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                {
                                                    i3++;
                                                }
                                                else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                {
                                                    i4++;
                                                }
                                                bList.AddLast(b1);
                                                do
                                                {
                                                    i2++;
                                                    b1 = br3.ReadByte();
                                                    if ((b1 >= 0x21 && b1 <= 0x40) || (b1 >= 0x5B && b1 <= 0x60) || (b1 >= 0x7B && b1 <= 0x7E))
                                                    {
                                                        i3++;
                                                    }
                                                    else if ((b1 >= 0x80 && b1 <= 0xBF) || b1 == 0xD7 || b1 == 0xF7)
                                                    {
                                                        i4++;
                                                    }
                                                    bList.AddLast(b1);
                                                } while ((b1 >= 0x20 && b1 <= 0xBF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD || b1 == 0xD7 || b1 == 0xF7);
                                                if (i2 > 1 && b1 == 0)
                                                {
                                                    bl = false;
                                                    bList.RemoveLast();
                                                    if (i2 == 2 && i3 == 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 3 && i3 <= 2 && i4 == 0)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 4 && i3 <= 2 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 == 5 && i3 <= 3 && i4 <= 1)
                                                    {
                                                        bl = true;
                                                    }
                                                    else if (i2 > 5 && (i3 / i2) < 0.4 && i4 <= 2)
                                                    {
                                                        bl = true;
                                                    }
                                                    if (bl)
                                                    {
                                                        byte[] bt = new byte[i2];
                                                        bList.CopyTo(bt, 0);
                                                        list111.Add(l3.ToString("X8"));
                                                        list222.Add(Encoding.UTF8.GetString(bt));
                                                        list444.Add(i2);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                UTF8_None_PeSave();
            }
        }

        private void UTF8_None_PeSave()//UTF8保存
        {
            int orgcode = 65001;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)")
            {
                orgname = "英语";
            }
            else if (orgname == "德语(1252)")
            {
                orgname = "德语";
            }
            else if (orgname == "法语(1252)")
            {
                orgname = "法语";
            }
            else if (orgname == "俄语(1251)")
            {
                orgname = "俄语";
            }
            else if (orgname == "韩文(949)")
            {
                orgname = "韩文";
            }
            else if (orgname == "日语(932)")
            {
                orgname = "日语";
            }
            else if (orgname == "简体中文(936)")
            {
                orgname = "简体中文";
            }
            else if (orgname == "繁体中文(950)")
            {
                orgname = "繁体中文";
            }
            int tracode = 65001;
            string traname = "";
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('UTF8','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, utf8) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    if (i11 > 0)
                    {
                        for (int i = 0; i < i11; i++)
                        {
                            s1 = list11[i].ToString();
                            s2 = list22[i].ToString();
                            i2 = (int)list44[i];
                            s2 = s2.Replace("'", "''");
                            try
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, utf8) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    int i111 = list111.Count;
                    if (i111 > 0)
                    {
                        for (int i = 0; i < i111; i++)
                        {
                            s1 = list111[i].ToString();
                            s2 = list222[i].ToString();
                            i2 = (int)list444[i];
                            s2 = s2.Replace("'", "''");
                            try
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, utf8) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void Unicode_None_English_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            if (NewSmartBL)
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    try
                                    {
                                        do
                                        {
                                            i2 = 0;
                                            if (br1.ReadByte() == 0)
                                            {
                                                i2++;
                                                if (br1.ReadByte() == 0)
                                                {
                                                    i2++;
                                                    if (br1.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        } while (i2 != 3);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    i2 = 0;
                                    do
                                    {
                                        fs1.Seek(-4, SeekOrigin.Current);
                                        i1 = br1.ReadUInt16();
                                        i2++;
                                        if (CommonCode.CheckEnglish(i1) == false)
                                        {
                                            i1 = 128;
                                            break;
                                        }
                                    } while (i1 > 0 && i1 < 128);
                                    try
                                    {
                                        if (i2 > 1)
                                        {
                                            l3 = fs1.Position;
                                            i2 = i2 * 2;
                                            bList.Clear();
                                            for (int x = 0; x < i2; x++)
                                            {
                                                bList.AddLast(br1.ReadByte());
                                            }
                                            if (i2 == 4)
                                            {
                                                fs1.Seek(-6, SeekOrigin.Current);
                                                i1 = br1.ReadUInt16();
                                                fs1.Seek(4, SeekOrigin.Current);
                                                if (i1 != 0)
                                                {
                                                    continue;
                                                }
                                            }
                                            bList.RemoveLast();
                                            bList.RemoveLast();
                                            i2 = i2 - 2;
                                            byte[] bt = new byte[i2];
                                            bList.CopyTo(bt, 0);
                                            if (i2 < 7)
                                            {
                                                string s1 = Encoding.Unicode.GetString(bt);
                                                if (SmartFilterAL.Contains(s1.ToLower()))
                                                {
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(s1);
                                                    list4.Add(i2);
                                                }
                                            }
                                            else
                                            {
                                                list1.Add(l3.ToString("X8"));
                                                list2.Add(Encoding.Unicode.GetString(bt));
                                                list4.Add(i2);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            },
                            () =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            try
                                            {
                                                do
                                                {
                                                    i2 = 0;
                                                    if (br2.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                        if (br2.ReadByte() == 0)
                                                        {
                                                            i2++;
                                                            if (br2.ReadByte() == 0)
                                                            {
                                                                i2++;
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                } while (i2 != 3);
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            i2 = 0;
                                            do
                                            {
                                                fs2.Seek(-4, SeekOrigin.Current);
                                                i1 = br2.ReadUInt16();
                                                i2++;
                                                if (CommonCode.CheckEnglish(i1) == false)
                                                {
                                                    i1 = 128;
                                                    break;
                                                }
                                            } while (i1 > 0 && i1 < 128);
                                            try
                                            {
                                                if (i2 > 1)
                                                {
                                                    l3 = fs2.Position;
                                                    i2 = i2 * 2;
                                                    bList.Clear();
                                                    for (int x = 0; x < i2; x++)
                                                    {
                                                        bList.AddLast(br2.ReadByte());
                                                    }
                                                    if (i2 == 4)
                                                    {
                                                        fs2.Seek(-6, SeekOrigin.Current);
                                                        i1 = br2.ReadUInt16();
                                                        fs2.Seek(4, SeekOrigin.Current);
                                                        if (i1 != 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    bList.RemoveLast();
                                                    bList.RemoveLast();
                                                    i2 = i2 - 2;
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    if (i2 < 7)
                                                    {
                                                        string s1 = Encoding.Unicode.GetString(bt);
                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                        {
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(s1);
                                                            list44.Add(i2);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        list11.Add(l3.ToString("X8"));
                                                        list22.Add(Encoding.Unicode.GetString(bt));
                                                        list44.Add(i2);
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            try
                                            {
                                                do
                                                {
                                                    i2 = 0;
                                                    if (br3.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                        if (br3.ReadByte() == 0)
                                                        {
                                                            i2++;
                                                            if (br3.ReadByte() == 0)
                                                            {
                                                                i2++;
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                } while (i2 != 3);
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            i2 = 0;
                                            do
                                            {
                                                fs3.Seek(-4, SeekOrigin.Current);
                                                i1 = br3.ReadUInt16();
                                                i2++;
                                                if (CommonCode.CheckEnglish(i1) == false)
                                                {
                                                    i1 = 128;
                                                    break;
                                                }
                                            } while (i1 > 0 && i1 < 128);
                                            try
                                            {
                                                if (i2 > 1)
                                                {
                                                    l3 = fs3.Position;
                                                    i2 = i2 * 2;
                                                    bList.Clear();
                                                    for (int x = 0; x < i2; x++)
                                                    {
                                                        bList.AddLast(br3.ReadByte());
                                                    }
                                                    if (i2 == 4)
                                                    {
                                                        fs3.Seek(-6, SeekOrigin.Current);
                                                        i1 = br3.ReadUInt16();
                                                        fs3.Seek(4, SeekOrigin.Current);
                                                        if (i1 != 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    bList.RemoveLast();
                                                    bList.RemoveLast();
                                                    i2 = i2 - 2;
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    if (i2 < 7)
                                                    {
                                                        string s1 = Encoding.Unicode.GetString(bt);
                                                        if (SmartFilterAL.Contains(s1.ToLower()))
                                                        {
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(s1);
                                                            list444.Add(i2);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        list111.Add(l3.ToString("X8"));
                                                        list222.Add(Encoding.Unicode.GetString(bt));
                                                        list444.Add(i2);
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (BinaryReader br1 = new BinaryReader(fs1))
                    {
                        PFL = fs1.Length - 1;
                        int dgv = al1.Count;
                        for (int i = 0; i < dgv; i++)
                        {
                            long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                            long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                            long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                            Parallel.Invoke(() =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1;
                                long EndPo = l1 + LSplit;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                fs1.Seek(StartPo, SeekOrigin.Begin);
                                while (fs1.Position < EndPo)
                                {
                                    try
                                    {
                                        do
                                        {
                                            i2 = 0;
                                            if (br1.ReadByte() == 0)
                                            {
                                                i2++;
                                                if (br1.ReadByte() == 0)
                                                {
                                                    i2++;
                                                    if (br1.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                        } while (i2 != 3);
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    i2 = 0;
                                    do
                                    {
                                        fs1.Seek(-4, SeekOrigin.Current);
                                        i1 = br1.ReadUInt16();
                                        i2++;
                                        if (CommonCode.CheckEnglish(i1) == false)
                                        {
                                            i1 = 128;
                                            break;
                                        }
                                    } while (i1 > 0 && i1 < 128);
                                    try
                                    {
                                        if (i2 > 1)
                                        {
                                            l3 = fs1.Position;
                                            i2 = i2 * 2;
                                            bList.Clear();
                                            for (int x = 0; x < i2; x++)
                                            {
                                                bList.AddLast(br1.ReadByte());
                                            }
                                            if (i2 == 4)
                                            {
                                                fs1.Seek(-6, SeekOrigin.Current);
                                                i1 = br1.ReadUInt16();
                                                fs1.Seek(4, SeekOrigin.Current);
                                                if (i1 != 0)
                                                {
                                                    continue;
                                                }
                                            }
                                            bList.RemoveLast();
                                            bList.RemoveLast();
                                            byte[] bt = new byte[i2 - 2];
                                            bList.CopyTo(bt, 0);
                                            list1.Add(l3.ToString("X8"));
                                            list2.Add(Encoding.Unicode.GetString(bt));
                                            list4.Add(i2 - 2);
                                        }
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                            },
                            () =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit;
                                long EndPo = l1 + LSplit * 2;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br2 = new BinaryReader(fs2))
                                    {
                                        fs2.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs2.Position < EndPo)
                                        {
                                            try
                                            {
                                                do
                                                {
                                                    i2 = 0;
                                                    if (br2.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                        if (br2.ReadByte() == 0)
                                                        {
                                                            i2++;
                                                            if (br2.ReadByte() == 0)
                                                            {
                                                                i2++;
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                } while (i2 != 3);
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            i2 = 0;
                                            do
                                            {
                                                fs2.Seek(-4, SeekOrigin.Current);
                                                i1 = br2.ReadUInt16();
                                                i2++;
                                                if (CommonCode.CheckEnglish(i1) == false)
                                                {
                                                    i1 = 128;
                                                    break;
                                                }
                                            } while (i1 > 0 && i1 < 128);
                                            try
                                            {
                                                if (i2 > 1)
                                                {
                                                    l3 = fs2.Position;
                                                    i2 = i2 * 2;
                                                    bList.Clear();
                                                    for (int x = 0; x < i2; x++)
                                                    {
                                                        bList.AddLast(br2.ReadByte());
                                                    }
                                                    if (i2 == 4)
                                                    {
                                                        fs2.Seek(-6, SeekOrigin.Current);
                                                        i1 = br2.ReadUInt16();
                                                        fs2.Seek(4, SeekOrigin.Current);
                                                        if (i1 != 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    bList.RemoveLast();
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2 - 2];
                                                    bList.CopyTo(bt, 0);
                                                    list11.Add(l3.ToString("X8"));
                                                    list22.Add(Encoding.Unicode.GetString(bt));
                                                    list44.Add(i2 - 2);
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            },
                            () =>
                            {
                                int i1 = 0;
                                int i2 = 0;//总计数
                                long l3 = 0;
                                long StartPo = l1 + LSplit * 2;
                                long EndPo = l2;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    using (BinaryReader br3 = new BinaryReader(fs3))
                                    {
                                        fs3.Seek(StartPo, SeekOrigin.Begin);
                                        while (fs3.Position < EndPo)
                                        {
                                            try
                                            {
                                                do
                                                {
                                                    i2 = 0;
                                                    if (br3.ReadByte() == 0)
                                                    {
                                                        i2++;
                                                        if (br3.ReadByte() == 0)
                                                        {
                                                            i2++;
                                                            if (br3.ReadByte() == 0)
                                                            {
                                                                i2++;
                                                            }
                                                            else
                                                            {
                                                                continue;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                } while (i2 != 3);
                                            }
                                            catch
                                            {
                                                break;
                                            }
                                            i2 = 0;
                                            do
                                            {
                                                fs3.Seek(-4, SeekOrigin.Current);
                                                i1 = br3.ReadUInt16();
                                                i2++;
                                                if (CommonCode.CheckEnglish(i1) == false)
                                                {
                                                    i1 = 128;
                                                    break;
                                                }
                                            } while (i1 > 0 && i1 < 128);
                                            try
                                            {
                                                if (i2 > 1)
                                                {
                                                    l3 = fs3.Position;
                                                    i2 = i2 * 2;
                                                    bList.Clear();
                                                    for (int x = 0; x < i2; x++)
                                                    {
                                                        bList.AddLast(br3.ReadByte());
                                                    }
                                                    if (i2 == 4)
                                                    {
                                                        fs3.Seek(-6, SeekOrigin.Current);
                                                        i1 = br3.ReadUInt16();
                                                        fs3.Seek(4, SeekOrigin.Current);
                                                        if (i1 != 0)
                                                        {
                                                            continue;
                                                        }
                                                    }
                                                    bList.RemoveLast();
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2 - 2];
                                                    bList.CopyTo(bt, 0);
                                                    list111.Add(l3.ToString("X8"));
                                                    list222.Add(Encoding.Unicode.GetString(bt));
                                                    list444.Add(i2 - 2);
                                                }
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            });
                            int iCount1 = list1.Count;
                            int iCount11 = list11.Count;
                            if (iCount1 > 0 && iCount11 > 0)
                            {
                                iCount1--;
                                long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                                long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                                if (LTmp1 > LTmp11)
                                {
                                    list11.RemoveAt(0);
                                    list22.RemoveAt(0);
                                    list44.RemoveAt(0);
                                }
                            }
                            iCount11 = list11.Count;
                            int iCount111 = list111.Count;
                            if (iCount11 > 0 && iCount111 > 0)
                            {
                                iCount11--;
                                long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                                long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                                if (LTmp11 > LTmp111)
                                {
                                    list111.RemoveAt(0);
                                    list222.RemoveAt(0);
                                    list444.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Unicode_None_PeSave();
            }
        }

        private void Unicode_None_PeSave()//Unicode保存
        {
            int orgcode = 1200;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)")
            {
                orgname = "英语";
            }
            else if (orgname == "德语(1252)")
            {
                orgname = "德语";
            }
            else if (orgname == "法语(1252)")
            {
                orgname = "法语";
            }
            else if (orgname == "俄语(1251)")
            {
                orgname = "俄语";
            }
            else if (orgname == "韩文(949)")
            {
                orgname = "韩文";
            }
            else if (orgname == "日语(932)")
            {
                orgname = "日语";
            }
            else if (orgname == "简体中文(936)")
            {
                orgname = "简体中文";
            }
            else if (orgname == "繁体中文(950)")
            {
                orgname = "繁体中文";
            }
            int tracode = 1200;
            string traname = "";
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('Unicode','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    if (i11 > 0)
                    {
                        for (int i = 0; i < i11; i++)
                        {
                            s1 = list11[i].ToString();
                            s2 = list22[i].ToString();
                            i2 = (int)list44[i];
                            s2 = s2.Replace("'", "''");
                            try
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    int i111 = list111.Count;
                    if (i111 > 0)
                    {
                        for (int i = 0; i < i111; i++)
                        {
                            s1 = list111[i].ToString();
                            s2 = list222[i].ToString();
                            i2 = (int)list444[i];
                            s2 = s2.Replace("'", "''");
                            try
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void Unicode_Standard1_English_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ArrayList tem1 = new ArrayList();
                    ArrayList tem2 = new ArrayList();
                    PFL = fs.Length - 1;
                    int dgv = al1.Count;
                    int i1 = 0;
                    int i2 = 0;
                    long l1 = 0;
                    long l2 = 0;
                    long l3 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    for (int i = 0; i < dgv; i++)
                    {
                        l1 = CommonCode.HexToLong(al1[i].ToString());
                        l2 = CommonCode.HexToLong(al2[i].ToString());
                        fs.Seek(l1, SeekOrigin.Begin);
                        while (fs.Position < l2)
                        {
                            try
                            {
                                do
                                {
                                    i2 = 0;
                                    if (br.ReadByte() == 0)
                                    {
                                        i2++;
                                        if (br.ReadByte() == 0)
                                        {
                                            i2++;
                                            if (br.ReadByte() == 0)
                                            {
                                                i2++;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                } while (i2 != 3);
                            }
                            catch
                            {
                                break;
                            }
                            i2 = 0;
                            do
                            {
                                fs.Seek(-4, SeekOrigin.Current);
                                i1 = br.ReadUInt16();
                                i2++;
                                if (CommonCode.CheckEnglish(i1) == false)
                                {
                                    i1 = 128;
                                    break;
                                }
                            } while (i1 > 0 && i1 < 128);
                            try
                            {
                                if (i2 > 1)
                                {
                                    l3 = fs.Position;
                                    i2 = i2 * 2;
                                    bList.Clear();
                                    for (int x = 0; x < i2; x++)
                                    {
                                        bList.AddLast(br.ReadByte());
                                    }
                                    if (i2 == 4)
                                    {
                                        fs.Seek(-6, SeekOrigin.Current);
                                        i1 = br.ReadUInt16();
                                        fs.Seek(4, SeekOrigin.Current);
                                        if (i1 != 0)
                                        {
                                            continue;
                                        }
                                    }
                                    bList.RemoveLast();
                                    bList.RemoveLast();
                                    byte[] bt = new byte[i2 - 2];
                                    bList.CopyTo(bt, 0);
                                    tem1.Add(l3.ToString("X8"));
                                    tem2.Add(Encoding.Unicode.GetString(bt));
                                    list4.Add(i2 - 2);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    uint u1 = 0;
                    i1 = tem1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        fs.Seek(CommonCode.HexToLong(tem1[i].ToString()) - 4, SeekOrigin.Begin);
                        u1 = br.ReadUInt32();
                        list1.Add(tem1[i].ToString());
                        list2.Add(tem2[i].ToString());
                        if (u1 == int.Parse(list4[i].ToString()) / 2)
                        {
                            list3.Add("True");
                        }
                        else
                        {
                            list3.Add("False");
                        }
                    }
                    if (list1.Count > 0 && NewSmartBL == true)
                    {
                        i1 = list2.Count;
                        for (int x = i1 - 1; x >= 0; x--)
                        {
                            string s1 = list2[x].ToString().ToLower();
                            if (s1.Length < 4)
                            {
                                if (SmartFilterAL.Contains(s1) == false)
                                {
                                    list1.RemoveAt(x);
                                    list2.RemoveAt(x);
                                    list3.RemoveAt(x);
                                    list4.RemoveAt(x);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Unicode_Standard_PeSave();
            }
        }

        private void Unicode_Standard2_English_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ArrayList tem1 = new ArrayList();
                    ArrayList tem2 = new ArrayList();
                    PFL = fs.Length - 1;
                    int dgv = al1.Count;
                    int i1 = 0;
                    int i2 = 0;
                    long l1 = 0;
                    long l2 = 0;
                    long l3 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    for (int i = 0; i < dgv; i++)
                    {
                        l1 = CommonCode.HexToLong(al1[i].ToString());
                        l2 = CommonCode.HexToLong(al2[i].ToString());
                        fs.Seek(l1, SeekOrigin.Begin);
                        while (fs.Position < l2)
                        {
                            try
                            {
                                do
                                {
                                    i2 = 0;
                                    if (br.ReadByte() == 0)
                                    {
                                        i2++;
                                        if (br.ReadByte() == 0)
                                        {
                                            i2++;
                                            if (br.ReadByte() == 0)
                                            {
                                                i2++;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                } while (i2 != 3);
                            }
                            catch
                            {
                                break;
                            }
                            i2 = 0;
                            do
                            {
                                fs.Seek(-4, SeekOrigin.Current);
                                i1 = br.ReadUInt16();
                                i2++;
                                if (CommonCode.CheckEnglish(i1) == false)
                                {
                                    i1 = 128;
                                    break;
                                }
                            } while (i1 > 0 && i1 < 128);
                            try
                            {
                                if (i2 > 1)
                                {
                                    l3 = fs.Position;
                                    i2 = i2 * 2;
                                    bList.Clear();
                                    for (int x = 0; x < i2; x++)
                                    {
                                        bList.AddLast(br.ReadByte());
                                    }
                                    if (i2 == 4)
                                    {
                                        fs.Seek(-6, SeekOrigin.Current);
                                        i1 = br.ReadUInt16();
                                        fs.Seek(4, SeekOrigin.Current);
                                        if (i1 != 0)
                                        {
                                            continue;
                                        }
                                    }
                                    bList.RemoveLast();
                                    bList.RemoveLast();
                                    byte[] bt = new byte[i2 - 2];
                                    bList.CopyTo(bt, 0);
                                    tem1.Add(l3.ToString("X8"));
                                    tem2.Add(Encoding.Unicode.GetString(bt));
                                    list4.Add(i2 - 2);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    uint u1 = 0;
                    i1 = tem1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        fs.Seek(CommonCode.HexToLong(tem1[i].ToString()) - 4, SeekOrigin.Begin);
                        u1 = br.ReadUInt32();
                        list1.Add(tem1[i].ToString());
                        list2.Add(tem2[i].ToString());
                        if (u1 == int.Parse(list4[i].ToString()))
                        {
                            list3.Add("True");
                        }
                        else
                        {
                            list3.Add("False");
                        }
                    }
                    if (list1.Count > 0 && NewSmartBL == true)
                    {
                        i1 = list2.Count;
                        for (int x = i1 - 1; x >= 0; x--)
                        {
                            string s1 = list2[x].ToString().ToLower();
                            if (s1.Length < 4)
                            {
                                if (SmartFilterAL.Contains(s1) == false)
                                {
                                    list1.RemoveAt(x);
                                    list2.RemoveAt(x);
                                    list3.RemoveAt(x);
                                    list4.RemoveAt(x);
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Unicode_Standard_PeSave();
            }
        }

        private void Unicode_Standard_PeSave()//Unicode保存
        {
            int orgcode = 1200;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)")
            {
                orgname = "英语";
            }
            else if (orgname == "德语(1252)")
            {
                orgname = "德语";
            }
            else if (orgname == "法语(1252)")
            {
                orgname = "法语";
            }
            else if (orgname == "俄语(1251)")
            {
                orgname = "俄语";
            }
            else if (orgname == "韩文(949)")
            {
                orgname = "韩文";
            }
            else if (orgname == "日语(932)")
            {
                orgname = "日语";
            }
            else if (orgname == "简体中文(936)")
            {
                orgname = "简体中文";
            }
            else if (orgname == "繁体中文(950)")
            {
                orgname = "繁体中文";
            }
            int tracode = 1200;
            string traname = "";
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('Unicode','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int bl = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        if (list3[i].ToString() == "True")
                        {
                            bl = 1;
                        }
                        else
                        {
                            bl = 0;
                        }
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + "," + bl + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    for (int i = 0; i < i11; i++)
                    {
                        if (list33[i].ToString() == "True")
                        {
                            bl = 1;
                        }
                        else
                        {
                            bl = 0;
                        }
                        s1 = list11[i].ToString();
                        s2 = list22[i].ToString();
                        i2 = (int)list44[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + "," + bl + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i111 = list111.Count;
                    for (int i = 0; i < i111; i++)
                    {
                        if (list333[i].ToString() == "True")
                        {
                            bl = 1;
                        }
                        else
                        {
                            bl = 0;
                        }
                        s1 = list111[i].ToString();
                        s2 = list222[i].ToString();
                        i2 = (int)list444[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + "," + bl + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void ANSI_Delphi_English_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br1 = new BinaryReader(fs1))
                {
                    PFL = fs1.Length - 1;
                    int dgv = al1.Count;
                    for (int i = 0; i < dgv; i++)
                    {
                        long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                        long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                        long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                        Parallel.Invoke(() =>
                        {
                            int i1 = 0;
                            int xx = 0;
                            int xd = 0;
                            long l3 = 0;
                            long l4 = 0;
                            uint u1 = 0;
                            uint u2 = 0;
                            long StartPo = l1;
                            long EndPo = l1 + LSplit;
                            byte b1 = 0;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            fs1.Seek(StartPo, SeekOrigin.Begin);
                            while (fs1.Position < EndPo)
                            {
                                l3 = fs1.Position;
                                b1 = br1.ReadByte();
                                bList.AddLast(b1);
                                i1 = 0;
                                xx = 0;
                                xd = 0;
                                if ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                {
                                    do
                                    {
                                        b1 = br1.ReadByte();
                                        i1++;
                                        bList.AddLast(b1);
                                        if (b1 >= 0x7F)
                                        {
                                            xd++;//多
                                        }
                                        else
                                        {
                                            xx++;//少
                                        }
                                    } while ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD);
                                    if (b1 == 0)
                                    {
                                        bList.RemoveLast();
                                        l4 = fs1.Position;
                                        fs1.Seek(l3 - 8, SeekOrigin.Begin);
                                        u1 = br1.ReadUInt32();
                                        u2 = br1.ReadUInt32();
                                        fs1.Seek(l4, SeekOrigin.Begin);
                                        byte[] bt = new byte[i1];
                                        bList.CopyTo(bt, 0);
                                        string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                        if (u1 == 4294967295 && u2 == i1)
                                        {
                                            list1.Add(l3.ToString("X8"));
                                            list2.Add(s1);
                                            list3.Add(1);
                                            list4.Add(i1);
                                        }
                                        else if (xx > 0)
                                        {
                                            if (i1 > 3)
                                            {
                                                if (xd / (double)i1 < 0.26)
                                                {
                                                    list1.Add(l3.ToString("X8"));
                                                    list2.Add(s1);
                                                    list3.Add(0);
                                                    list4.Add(i1);
                                                }
                                            }
                                        }
                                    }
                                }
                                bList.Clear();
                            }
                        },
                        () =>
                        {
                            int i1 = 0;
                            int xx = 0;
                            int xd = 0;
                            long l3 = 0;
                            long l4 = 0;
                            uint u1 = 0;
                            uint u2 = 0;
                            long StartPo = l1 + LSplit;
                            long EndPo = l1 + LSplit * 2;
                            byte b1 = 0;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                using (BinaryReader br2 = new BinaryReader(fs2))
                                {
                                    fs2.Seek(StartPo, SeekOrigin.Begin);
                                    while (fs2.Position < EndPo)
                                    {
                                        l3 = fs2.Position;
                                        b1 = br2.ReadByte();
                                        bList.AddLast(b1);
                                        i1 = 0;
                                        xx = 0;
                                        xd = 0;
                                        if ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                        {
                                            do
                                            {
                                                b1 = br2.ReadByte();
                                                i1++;
                                                bList.AddLast(b1);
                                                if (b1 >= 0x7F)
                                                {
                                                    xd++;//多
                                                }
                                                else
                                                {
                                                    xx++;//少
                                                }
                                            } while ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD);
                                            if (b1 == 0)
                                            {
                                                bList.RemoveLast();
                                                l4 = fs2.Position;
                                                fs2.Seek(l3 - 8, SeekOrigin.Begin);
                                                u1 = br2.ReadUInt32();
                                                u2 = br2.ReadUInt32();
                                                fs2.Seek(l4, SeekOrigin.Begin);
                                                byte[] bt = new byte[i1];
                                                bList.CopyTo(bt, 0);
                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                if (u1 == 4294967295 && u2 == i1)
                                                {
                                                    list11.Add(l3.ToString("X8"));
                                                    list22.Add(s1);
                                                    list33.Add(1);
                                                    list44.Add(i1);
                                                }
                                                else if (xx > 0)
                                                {
                                                    if (i1 > 3)
                                                    {
                                                        if (xd / (double)i1 < 0.26)
                                                        {
                                                            list11.Add(l3.ToString("X8"));
                                                            list22.Add(s1);
                                                            list33.Add(0);
                                                            list44.Add(i1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        bList.Clear();
                                    }
                                }
                            }
                        },
                        () =>
                        {
                            int i1 = 0;
                            int xx = 0;
                            int xd = 0;
                            long l3 = 0;
                            long l4 = 0;
                            uint u1 = 0;
                            uint u2 = 0;
                            long StartPo = l1 + LSplit * 2;
                            long EndPo = l2;
                            byte b1 = 0;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                using (BinaryReader br3 = new BinaryReader(fs3))
                                {
                                    fs3.Seek(StartPo, SeekOrigin.Begin);
                                    while (fs3.Position < EndPo)
                                    {
                                        l3 = fs3.Position;
                                        b1 = br3.ReadByte();
                                        bList.AddLast(b1);
                                        i1 = 0;
                                        xx = 0;
                                        xd = 0;
                                        if ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD)
                                        {
                                            do
                                            {
                                                b1 = br3.ReadByte();
                                                i1++;
                                                bList.AddLast(b1);
                                                if (b1 >= 0x7F)
                                                {
                                                    xd++;//多
                                                }
                                                else
                                                {
                                                    xx++;//少
                                                }
                                            } while ((b1 >= 0x20 && b1 <= 0xFF) || b1 == 0x9 || b1 == 0xA || b1 == 0xD);
                                            if (b1 == 0)
                                            {
                                                bList.RemoveLast();
                                                l4 = fs3.Position;
                                                fs3.Seek(l3 - 8, SeekOrigin.Begin);
                                                u1 = br3.ReadUInt32();
                                                u2 = br3.ReadUInt32();
                                                fs3.Seek(l4, SeekOrigin.Begin);
                                                byte[] bt = new byte[i1];
                                                bList.CopyTo(bt, 0);
                                                string s1 = Encoding.GetEncoding(1252).GetString(bt);
                                                if (u1 == 4294967295 && u2 == i1)
                                                {
                                                    list111.Add(l3.ToString("X8"));
                                                    list222.Add(s1);
                                                    list333.Add(1);
                                                    list444.Add(i1);
                                                }
                                                else if (xx > 0)
                                                {
                                                    if (i1 > 3)
                                                    {
                                                        if (xd / (double)i1 < 0.26)
                                                        {
                                                            list111.Add(l3.ToString("X8"));
                                                            list222.Add(s1);
                                                            list333.Add(0);
                                                            list444.Add(i1);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        bList.Clear();
                                    }
                                }
                            }
                        });
                        int iCount1 = list1.Count;
                        int iCount11 = list11.Count;
                        if (iCount1 > 0 && iCount11 > 0)
                        {
                            iCount1--;
                            long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                            long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                            if (LTmp1 > LTmp11)
                            {
                                list11.RemoveAt(0);
                                list22.RemoveAt(0);
                                list44.RemoveAt(0);
                            }
                        }
                        iCount11 = list11.Count;
                        int iCount111 = list111.Count;
                        if (iCount11 > 0 && iCount111 > 0)
                        {
                            iCount11--;
                            long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                            long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                            if (LTmp11 > LTmp111)
                            {
                                list111.RemoveAt(0);
                                list222.RemoveAt(0);
                                list444.RemoveAt(0);
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_Delphi_English_PeSave();
            }
        }

        private void ANSI_Delphi_English_PeSave()//ANSI保存
        {
            int orgcode = 0;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)" || orgname == "德语(1252)" || orgname == "法语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "俄语(1251)")
            {
                orgcode = 1251;
            }
            else if (orgname == "韩文(949)")
            {
                orgcode = 949;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            else if (orgname == "简体中文(936)")
            {
                orgcode = 936;
            }
            else if (orgname == "繁体中文(950)")
            {
                orgcode = 950;
            }
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
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('ANSI','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            if ((int)list3[i] == 1)
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            }
                            else
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",0)";
                            }
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    for (int i = 0; i < i11; i++)
                    {
                        s1 = list11[i].ToString();
                        s2 = list22[i].ToString();
                        i2 = (int)list44[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            if ((int)list33[i] == 1)
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            }
                            else
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",0)";
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i111 = list111.Count;
                    for (int i = 0; i < i111; i++)
                    {
                        s1 = list111[i].ToString();
                        s2 = list222[i].ToString();
                        i2 = (int)list444[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            if ((int)list333[i] == 1)
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            }
                            else
                            {
                                cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",0)";
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void ANSI_Delphi_Japanese_Korean_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            int orgcode = 1252;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)" || orgname == "德语(1252)" || orgname == "法语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "俄语(1251)")
            {
                orgcode = 1251;
            }
            else if (orgname == "韩文(949)")
            {
                orgcode = 949;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            else if (orgname == "简体中文(936)")
            {
                orgcode = 936;
            }
            else if (orgname == "繁体中文(950)")
            {
                orgcode = 950;
            }
            using (FileStream fs1 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br1 = new BinaryReader(fs1))
                {
                    PFL = fs1.Length - 1;
                    int dgv = al1.Count;
                    for (int i = 0; i < dgv; i++)
                    {
                        long l1 = CommonCode.HexToLong(al1[i].ToString());//起始位置
                        long l2 = CommonCode.HexToLong(al2[i].ToString());//结尾位置
                        long LSplit = Math.DivRem(l2 - l1, 3, out long LTmp);
                        Parallel.Invoke(() =>
                        {
                            int i2 = 0;//总计数
                            long l3 = 0;
                            uint u1 = 0;
                            byte b1 = 0;
                            long StartPo = l1;
                            long EndPo = l1 + LSplit;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            fs1.Seek(StartPo, SeekOrigin.Begin);
                            while (fs1.Position < EndPo)
                            {
                                do
                                {
                                    i2 = 0;
                                    if (br1.ReadByte() == 255)
                                    {
                                        i2++;
                                        if (br1.ReadByte() == 255)
                                        {
                                            i2++;
                                            if (br1.ReadByte() == 255)
                                            {
                                                i2++;
                                                if (br1.ReadByte() == 255)
                                                {
                                                    i2++;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                } while (i2 != 4);
                                if (i2 == 4)
                                {
                                    u1 = br1.ReadUInt32();
                                    if (u1 > 0)
                                    {
                                        l3 = fs1.Position;
                                        i2 = 0;
                                        bList.Clear();
                                        do
                                        {
                                            b1 = br1.ReadByte();
                                            bList.AddLast(b1);
                                            i2++;
                                        } while (b1 > 0 && b1 != 255);
                                        i2--;
                                        if (i2 == u1)
                                        {
                                            bList.RemoveLast();
                                            byte[] bt = new byte[i2];
                                            bList.CopyTo(bt, 0);
                                            list1.Add(l3.ToString("X8"));
                                            list2.Add(Encoding.GetEncoding(orgcode).GetString(bt));
                                            list4.Add(i2);
                                        }
                                        else
                                        {
                                            fs1.Seek(l3 - 4, SeekOrigin.Begin);
                                        }
                                    }
                                }
                            }
                        },
                        () =>
                        {
                            int i2 = 0;//总计数
                            long l3 = 0;
                            uint u1 = 0;
                            byte b1 = 0;
                            long StartPo = l1 + LSplit;
                            long EndPo = l1 + LSplit * 2;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            using (FileStream fs2 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                using (BinaryReader br2 = new BinaryReader(fs2))
                                {
                                    fs2.Seek(StartPo, SeekOrigin.Begin);
                                    while (fs2.Position < EndPo)
                                    {
                                        do
                                        {
                                            i2 = 0;
                                            if (br2.ReadByte() == 255)
                                            {
                                                i2++;
                                                if (br2.ReadByte() == 255)
                                                {
                                                    i2++;
                                                    if (br2.ReadByte() == 255)
                                                    {
                                                        i2++;
                                                        if (br2.ReadByte() == 255)
                                                        {
                                                            i2++;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        } while (i2 != 4);
                                        if (i2 == 4)
                                        {
                                            u1 = br2.ReadUInt32();
                                            if (u1 > 0)
                                            {
                                                l3 = fs2.Position;
                                                i2 = 0;
                                                bList.Clear();
                                                do
                                                {
                                                    b1 = br2.ReadByte();
                                                    bList.AddLast(b1);
                                                    i2++;
                                                } while (b1 > 0 && b1 != 255);
                                                i2--;
                                                if (i2 == u1)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list11.Add(l3.ToString("X8"));
                                                    list22.Add(Encoding.GetEncoding(orgcode).GetString(bt));
                                                    list44.Add(i2);
                                                }
                                                else
                                                {
                                                    fs2.Seek(l3 - 4, SeekOrigin.Begin);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        () =>
                        {
                            int i2 = 0;//总计数
                            long l3 = 0;
                            uint u1 = 0;
                            byte b1 = 0;
                            long StartPo = l1 + LSplit * 2;
                            long EndPo = l2;
                            LinkedList<byte> bList = new LinkedList<byte>();
                            using (FileStream fs3 = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                using (BinaryReader br3 = new BinaryReader(fs3))
                                {
                                    fs3.Seek(StartPo, SeekOrigin.Begin);
                                    while (fs3.Position < EndPo)
                                    {
                                        do
                                        {
                                            i2 = 0;
                                            if (br3.ReadByte() == 255)
                                            {
                                                i2++;
                                                if (br3.ReadByte() == 255)
                                                {
                                                    i2++;
                                                    if (br3.ReadByte() == 255)
                                                    {
                                                        i2++;
                                                        if (br3.ReadByte() == 255)
                                                        {
                                                            i2++;
                                                        }
                                                        else
                                                        {
                                                            break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        } while (i2 != 4);
                                        if (i2 == 4)
                                        {
                                            u1 = br3.ReadUInt32();
                                            if (u1 > 0)
                                            {
                                                l3 = fs3.Position;
                                                i2 = 0;
                                                bList.Clear();
                                                do
                                                {
                                                    b1 = br3.ReadByte();
                                                    bList.AddLast(b1);
                                                    i2++;
                                                } while (b1 > 0 && b1 != 255);
                                                i2--;
                                                if (i2 == u1)
                                                {
                                                    bList.RemoveLast();
                                                    byte[] bt = new byte[i2];
                                                    bList.CopyTo(bt, 0);
                                                    list111.Add(l3.ToString("X8"));
                                                    list222.Add(Encoding.GetEncoding(orgcode).GetString(bt));
                                                    list444.Add(i2);
                                                }
                                                else
                                                {
                                                    fs3.Seek(l3 - 4, SeekOrigin.Begin);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                        int iCount1 = list1.Count;
                        int iCount11 = list11.Count;
                        if (iCount1 > 0 && iCount11 > 0)
                        {
                            iCount1--;
                            long LTmp1 = CommonCode.HexToLong(list1[iCount1].ToString()) + long.Parse(list4[iCount1].ToString());
                            long LTmp11 = CommonCode.HexToLong(list11[0].ToString());
                            if (LTmp1 > LTmp11)
                            {
                                list11.RemoveAt(0);
                                list22.RemoveAt(0);
                                list44.RemoveAt(0);
                            }
                        }
                        iCount11 = list11.Count;
                        int iCount111 = list111.Count;
                        if (iCount11 > 0 && iCount111 > 0)
                        {
                            iCount11--;
                            long LTmp11 = CommonCode.HexToLong(list11[iCount11].ToString()) + long.Parse(list44[iCount11].ToString());
                            long LTmp111 = CommonCode.HexToLong(list111[0].ToString());
                            if (LTmp11 > LTmp111)
                            {
                                list111.RemoveAt(0);
                                list222.RemoveAt(0);
                                list444.RemoveAt(0);
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0 && list11.Count == 0 && list111.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                ANSI_Delphi_Japanese_Korean_PeSave();
            }
        }

        private void ANSI_Delphi_Japanese_Korean_PeSave()
        {
            int orgcode = 0;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)" || orgname == "德语(1252)" || orgname == "法语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "俄语(1251)")
            {
                orgcode = 1251;
            }
            else if (orgname == "韩文(949)")
            {
                orgcode = 949;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            else if (orgname == "简体中文(936)")
            {
                orgcode = 936;
            }
            else if (orgname == "繁体中文(950)")
            {
                orgcode = 950;
            }
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
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('ANSI','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i2 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i11 = list11.Count;
                    for (int i = 0; i < i11; i++)
                    {
                        s1 = list11[i].ToString();
                        s2 = list22[i].ToString();
                        i2 = (int)list44[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    int i111 = list111.Count;
                    for (int i = 0; i < i111; i++)
                    {
                        s1 = list111[i].ToString();
                        s2 = list222[i].ToString();
                        i2 = (int)list444[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void Unicode_Delphi_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs = new FileStream(OrgPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    PFL = fs.Length - 1;
                    int dgv = al1.Count;
                    byte b1 = 0;
                    byte b2 = 0;
                    int i3 = 0;
                    int i4 = 0;
                    long l1 = 0;
                    long l2 = 0;
                    long l3 = 0;
                    uint u1 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    for (int i = 0; i < dgv; i++)
                    {
                        l1 = CommonCode.HexToLong(al1[i].ToString());
                        l2 = CommonCode.HexToLong(al2[i].ToString()) - 2;
                        fs.Seek(l1, SeekOrigin.Begin);
                        while (fs.Position < l2)
                        {
                            i3 = 0;
                            if (br.ReadByte() == 255)
                            {
                                i3++;
                                if (br.ReadByte() == 255)
                                {
                                    i3++;
                                    if (br.ReadByte() == 255)
                                    {
                                        i3++;
                                        if (br.ReadByte() == 255)
                                        {
                                            i3++;
                                        }
                                    }
                                }
                            }
                            if (i3 == 4)
                            {
                                u1 = br.ReadUInt32() * 2;
                                if (u1 > 0)
                                {
                                    l3 = fs.Position;
                                    i3 = 0;
                                    i4 = 0;
                                    bList.Clear();
                                    do
                                    {
                                        b1 = br.ReadByte();
                                        bList.AddLast(b1);
                                        i3++;
                                        b2 = br.ReadByte();
                                        bList.AddLast(b2);
                                        i3++;
                                        if (b2 > 0)
                                        {
                                            i4++;
                                        }
                                    } while (b1 != 0 || b2 != 0);
                                    i3 = i3 - 2;
                                    if (i3 == u1 && i4 < 3)
                                    {
                                        bList.RemoveLast();
                                        bList.RemoveLast();
                                        byte[] bt = new byte[i3];
                                        bList.CopyTo(bt, 0);
                                        list1.Add(l3.ToString("X8"));
                                        list2.Add(Encoding.Unicode.GetString(bt));
                                        list3.Add(1);
                                        list4.Add(i3);
                                    }
                                    else
                                    {
                                        fs.Seek(l3 - 4, SeekOrigin.Begin);
                                    }
                                }
                            }
                        }
                    }
                    int i5 = list1.Count - 1;
                    if (i5 >= 1)
                    {
                        int i1 = 0;
                        int i2 = 0;
                        for (int i = 0; i < i5; i++)
                        {
                            l1 = CommonCode.HexToLong(list1[i].ToString()) + int.Parse(list4[i].ToString());
                            l2 = CommonCode.HexToLong(list1[i + 1].ToString()) - 8;
                            l3 = l2 - l1;
                            if (l3 > 10)
                            {
                                fs.Seek(l1 + 2, SeekOrigin.Begin);
                                while (fs.Position < l2)
                                {
                                    while (br.ReadByte() == 0)
                                    {
                                        //
                                    }
                                    fs.Seek(-1, SeekOrigin.Current);
                                    do
                                    {
                                        i3 = 0;
                                        if (br.ReadByte() == 0)
                                        {
                                            i3++;
                                            if (br.ReadByte() == 0)
                                            {
                                                i3++;
                                                if (br.ReadByte() == 0)
                                                {
                                                    i3++;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                    } while (i3 != 3 && fs.Position < l2);
                                    if (i3 == 3)
                                    {
                                        l3 = l2 - fs.Position;
                                        if (l3 >= 0)
                                        {
                                            i2 = 0;
                                            do
                                            {
                                                fs.Seek(-4, SeekOrigin.Current);
                                                i1 = br.ReadUInt16();
                                                i2++;
                                            } while (i1 > 0 && CommonCode.Check_Unicode_English(i1) == true);
                                            if (i2 > 3)
                                            {
                                                l3 = fs.Position;
                                                i2 = i2 * 2;
                                                bList.Clear();
                                                for (int x = 0; x < i2; x++)
                                                {
                                                    b1 = br.ReadByte();
                                                    bList.AddLast(b1);
                                                }
                                                bList.RemoveLast();
                                                bList.RemoveLast();
                                                i2 = i2 - 2;
                                                byte[] bt = new byte[i2];
                                                bList.CopyTo(bt, 0);
                                                list1.Add(l3.ToString("X8"));
                                                list2.Add(Encoding.Unicode.GetString(bt));
                                                list3.Add(0);
                                                list4.Add(i2);
                                            }
                                            else
                                            {
                                                fs.Seek((i2 + 1) * 2, SeekOrigin.Current);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (list1.Count == 0)
            {
                PENewProjectTimer.Enabled = false;
                progressBar1.Value = 0;
                tabControl1.Enabled = true;
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                Unicode_Delphi_PeSave();
            }
        }

        private void Unicode_Delphi_PeSave()//Unicode保存
        {
            int orgcode = 1200;
            string orgname = comboBox1.Text;
            if (orgname == "英语(1252)")
            {
                orgname = "英语";
            }
            else if (orgname == "德语(1252)")
            {
                orgname = "德语";
            }
            else if (orgname == "法语(1252)")
            {
                orgname = "法语";
            }
            else if (orgname == "俄语(1251)")
            {
                orgname = "俄语";
            }
            else if (orgname == "韩文(949)")
            {
                orgname = "韩文";
            }
            else if (orgname == "日语(932)")
            {
                orgname = "日语";
            }
            else if (orgname == "简体中文(936)")
            {
                orgname = "简体中文";
            }
            else if (orgname == "繁体中文(950)")
            {
                orgname = "繁体中文";
            }
            int tracode = 1200;
            string traname = comboBox2.Text;
            if (traname == "简体中文(936)")
            {
                traname = "简体中文";
            }
            else if (traname == "繁体中文(950)")
            {
                traname = "繁体中文";
            }
            else if (traname == "默认")
            {
                traname = "默认";
            }
            string sltype = comboBox5.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + CPU + "' where infoname = 'PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + sltype + "' where infoname = '长度标识'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + FileVersionInfo.GetVersionInfo(OrgPEFileName).FileVersion + "' where infoname = '版本'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('Unicode','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    string s1 = "";
                    string s2 = "";
                    int i3 = 0;
                    int i4 = 0;
                    int i1 = list1.Count;
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i3 = (int)list3[i];
                        i4 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi, ucode) Values ('" + s1 + "','" + s2 + "'," + i4 + "," + i4 + "," + i3 + ",1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            list33 = null;
            list44 = null;
            list111 = null;
            list222 = null;
            list333 = null;
            list444 = null;
            al1 = null;
            al2 = null;
            PENewProjectTimer.Enabled = false;
            progressBar1.Value = 0;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
            tabControl1.Enabled = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            OrgUnPEFileName = textBox4.Text;
            Projectname = mainform.CDirectory + "工程\\" + Path.GetFileName(OrgUnPEFileName) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
            if (OrgUnPEFileName == "")
            {
                MessageBox.Show("请指定需要的文件。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(OrgUnPEFileName) == false)
            {
                MessageBox.Show("指定的文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(Projectname) == true)
            {
                MessageBox.Show("名称为“" + Path.GetFileName(Projectname) + "”的工程文件已存在，请稍候再试。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (CommonCode.PE(OrgUnPEFileName) == true)
            {
                MessageBox.Show("指定的文件是一个 PE 文件，请使用 PE 处理方法。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (checkBox4.Checked == true && checkBox4.Enabled == true)
                {
                    LoadFilter();
                }
                else
                {
                    NewSmartBL = false;
                }
                UnPEBackgroundWorker.RunWorkerAsync();
            }
        }

        private void UnPEBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            tabControl1.Enabled = false;
            string codestr1 = comboBox4.Text;
            CommonCode.DictionaryFolder();
            list1 = new ArrayList();
            list2 = new ArrayList();
            list3 = new ArrayList();
            list4 = new ArrayList();
            list11 = new ArrayList();
            list22 = new ArrayList();
            if (radioButton6.Checked == true)
            {
                if (codestr1 == "英语(1252)" || codestr1 == "法语(1252)" || codestr1 == "德语(1252)")
                {
                    UnPeEnglish1();
                }
            }
            else if (radioButton7.Checked == true)
            {
                if (codestr1 == "英语(1252)" || codestr1 == "法语(1252)" || codestr1 == "德语(1252)")
                {
                    UnPeEnglish2();
                }
            }
            else if (radioButton8.Checked == true)
            {
                if (codestr1 == "英语(1252)" || codestr1 == "法语(1252)" || codestr1 == "德语(1252)")
                {
                    UnPeEnglish3();
                }
            }
            else if (radioButton9.Checked == true)
            {
                if (codestr1 == "英语(1252)" || codestr1 == "法语(1252)" || codestr1 == "德语(1252)")
                {
                    UnPeEnglish1();
                }
                else if (codestr1 == "日语(932)")
                {
                    UnPeJapanese();
                }
            }
            else if (radioButton10.Checked == true)
            {
                UnPeSkyrim();
            }
            else if (radioButton11.Checked == true)
            {
                UnPeQTpo();
            }
        }

        private void UnPEBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar2.Value = 0;
            list1 = null;
            list2 = null;
            list3 = null;
            list4 = null;
            list11 = null;
            list22 = null;
            tabControl1.Enabled = true;
        }

        private void UnPeSave()//非PE保存
        {
            string orgname = comboBox4.Text;
            int orgcode = 0;
            int tracode = 0;
            if (orgname == "英语(1252)" || orgname == "法语(1252)" || orgname == "德语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            string traname = comboBox2.Text;
            if (traname == "简体中文(936)")
            {
                tracode = 936;
            }
            else if (traname == "繁体中文(950)")
            {
                tracode = 950;
            }
            else
            {
                tracode = mainform.AA_Default_Encoding.CodePage;
            }
            string UnPEType = "";
            if (radioButton6.Checked == true)
            {
                UnPEType = "1";
            }
            else if (radioButton7.Checked == true)
            {
                UnPEType = "2";
            }
            else if (radioButton8.Checked == true)
            {
                UnPEType = "3";
            }
            else if (radioButton9.Checked == true)
            {
                UnPEType = "4";
            }
            Create_Project_DB();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    int i1 = list1.Count;
                    cmd.CommandText = "update fileinfo set detail = '" + OrgUnPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + UnPEType + "' where infoname = '非PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('ANSI','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    int i2 = 0;
                    string s1 = "";
                    string s2 = "";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            UnPENewProjectTimer.Enabled = false;
            progressBar2.Value = 20;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnicodeUnPeSave()//非PE保存
        {
            int orgcode = 0;
            string orgname = comboBox4.Text;
            if (orgname == "英语(1252)" || orgname == "德语(1252)" || orgname == "法语(1252)")
            {
                orgcode = 1252;
            }
            else if (orgname == "俄语(1251)")
            {
                orgcode = 1251;
            }
            else if (orgname == "韩文(949)")
            {
                orgcode = 949;
            }
            else if (orgname == "日语(932)")
            {
                orgcode = 932;
            }
            else if (orgname == "简体中文(936)")
            {
                orgcode = 936;
            }
            else if (orgname == "繁体中文(950)")
            {
                orgcode = 950;
            }
            int tracode = 0;
            string traname = "";
            string UnPEType = "";
            if (radioButton6.Checked == true)
            {
                UnPEType = "1";
            }
            else if (radioButton7.Checked == true)
            {
                UnPEType = "2";
            }
            else if (radioButton8.Checked == true)
            {
                UnPEType = "3";
            }
            else if (radioButton9.Checked == true)
            {
                UnPEType = "4";
            }
            Create_Project_DB();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    int i1 = list1.Count;
                    cmd.CommandText = "update fileinfo set detail = '" + OrgUnPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + (PFL + 1).ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + UnPEType + "' where infoname = '非PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('Unicode','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    int i2 = 0;
                    string s1 = "";
                    string s2 = "";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString();
                        i2 = (int)list4[i];
                        s2 = s2.Replace("'", "''");
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, ucode) Values ('" + s1 + "','" + s2 + "'," + i2 + "," + i2 + ", 1)";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            UnPENewProjectTimer.Enabled = false;
            progressBar2.Value = 20;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnPeEnglish1()//非PE提取格式 1
        {
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    PFL = fs.Length - 1;
                    int i2 = 0;
                    long l1 = 0;
                    string s1 = "";
                    byte b1 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    while (fs.Position < PFL)
                    {
                        b1 = br.ReadByte();
                        if (b1 > 0)
                        {
                            fs.Seek(-1, SeekOrigin.Current);
                            l1 = fs.Position;
                            i2 = 0;
                            bList.Clear();
                            do
                            {
                                if (fs.Position == PFL)
                                {
                                    b1 = br.ReadByte();
                                    bList.AddLast(b1);
                                    i2++;
                                    break;
                                }
                                else
                                {
                                    b1 = br.ReadByte();
                                }
                                try
                                {
                                    bList.AddLast(b1);
                                }
                                catch
                                {
                                    break;
                                }
                                i2++;
                            } while (b1 > 0);
                            i2--;
                            bList.RemoveLast();
                            byte[] bt = new byte[i2];
                            bList.CopyTo(bt, 0);
                            s1 = Encoding.GetEncoding(1252).GetString(bt);
                            if (CommonCode.Check(s1) == true)
                            {
                                list1.Add(l1.ToString("X8"));
                                list2.Add(s1);
                                list4.Add(i2);
                            }
                        }
                    }
                    fs.Close();
                    br.Close();
                    if (list1.Count == 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    else
                    {
                        if (NewSmartBL == true)
                        {
                            int i1 = list2.Count;
                            for (int x = i1 - 1; x >= 0; x--)
                            {
                                s1 = list2[x].ToString().ToLower();
                                if (s1.Length < 4)
                                {
                                    if (SmartFilterAL.Contains(s1) == false)
                                    {
                                        list1.RemoveAt(x);
                                        list2.RemoveAt(x);
                                        list4.RemoveAt(x);
                                    }
                                }
                            }
                        }
                        UnPeSave();
                    }
                }
            }
        }

        private void UnPeEnglish2()//非PE提取格式 ANSI 2
        {
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    PFL = fs.Length - 1;
                    int i2 = 0;
                    long l1 = 0;
                    string s1 = "";
                    byte b1 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    while (fs.Position < PFL)
                    {
                        b1 = br.ReadByte();
                        if (b1 > 0)
                        {
                            fs.Seek(-1, SeekOrigin.Current);
                            l1 = fs.Position;
                            i2 = 0;
                            bList.Clear();
                            do
                            {
                                if (fs.Position == PFL)
                                {
                                    b1 = br.ReadByte();
                                    bList.AddLast(b1);
                                    i2++;
                                    break;
                                }
                                else
                                {
                                    b1 = br.ReadByte();
                                }
                                try
                                {
                                    bList.AddLast(b1);
                                }
                                catch
                                {
                                    break;
                                }
                                i2++;
                            } while (b1 > 0);
                            bList.RemoveLast();
                            byte[] bt = new byte[i2 - 1];
                            bList.CopyTo(bt, 0);
                            s1 = Encoding.GetEncoding(1252).GetString(bt);
                            if ((fs.Position - i2 - 4) > 0)
                            {
                                fs.Seek((fs.Position - i2 - 4), SeekOrigin.Begin);
                                if (br.ReadUInt32() == i2)
                                {
                                    if (CommonCode.Check(s1) == true)
                                    {
                                        list1.Add(l1.ToString("X8"));
                                        list2.Add(s1);
                                        list4.Add(i2 - 1);
                                    }
                                }
                                fs.Seek(fs.Position + i2, SeekOrigin.Begin);
                            }
                            else if (CommonCode.Check(s1) == true)
                            {
                                list1.Add(l1.ToString("X8"));
                                list2.Add(s1);
                                list4.Add(i2 - 1);
                            }
                        }
                    }
                    fs.Close();
                    br.Close();
                    if (list1.Count == 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    else
                    {
                        if (NewSmartBL == true)
                        {
                            int i1 = list2.Count;
                            for (int x = i1 - 1; x >= 0; x--)
                            {
                                s1 = list2[x].ToString().ToLower();
                                if (s1.Length < 4)
                                {
                                    if (SmartFilterAL.Contains(s1) == false)
                                    {
                                        list1.RemoveAt(x);
                                        list2.RemoveAt(x);
                                        list4.RemoveAt(x);
                                    }
                                }
                            }
                        }
                        UnPeSave();
                    }
                }
            }
        }

        private void UnPeEnglish3()//非PE提取格式 Unicode
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    ArrayList tem1 = new ArrayList();
                    ArrayList tem2 = new ArrayList();
                    PFL = fs.Length - 1;
                    int i1 = 0;
                    int i2 = 0;
                    long l3 = 0;
                    string s1 = "";
                    byte b1 = 0;
                    fs.Seek(8, SeekOrigin.Begin);
                    while (fs.Position < PFL)
                    {
                        try
                        {
                            if (br.ReadByte() == 0)
                            {
                                if (br.ReadByte() == 0)
                                {
                                    i2 = br.ReadByte();
                                    if (i2 == 0)
                                    {
                                        fs.Seek(-2, SeekOrigin.Current);
                                    }
                                    else
                                    {
                                        fs.Seek(-1, SeekOrigin.Current);
                                        i2 = br.ReadInt16();
                                        if (i2 >= 255)
                                        {
                                            i2 = 0;
                                        }
                                        else
                                        {
                                            fs.Seek(-2, SeekOrigin.Current);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            break;
                        }
                        if (i2 > 0)
                        {
                            i2 = 0;
                            l3 = fs.Position;
                            do
                            {
                                i1 = br.ReadInt16();
                                if (i1 > 0)
                                {
                                    i2++;
                                }
                            } while (i1 > 0);
                        }
                        try
                        {
                            if (i2 > 1)
                            {
                                fs.Seek(l3, SeekOrigin.Begin);
                                i2 = i2 * 2;
                                LinkedList<byte> bList = new LinkedList<byte>();
                                for (int x = 0; x < i2; x++)
                                {
                                    b1 = br.ReadByte();
                                    bList.AddLast(b1);
                                }
                                byte[] bt = new byte[i2];
                                bList.CopyTo(bt, 0);
                                s1 = Encoding.Unicode.GetString(bt);
                                list1.Add(l3.ToString("X8"));
                                list2.Add(s1);
                                list3.Add("False");
                                list4.Add(i2);
                                bList.Clear();
                            }
                            i2 = 0;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    br.Close();
                    fs.Close();
                    if (list1.Count == 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    else
                    {
                        if (NewSmartBL == true)
                        {
                            i1 = list2.Count;
                            for (int x = i1 - 1; x >= 0; x--)
                            {
                                s1 = list2[x].ToString().ToLower();
                                if (s1.Length < 4)
                                {
                                    if (SmartFilterAL.Contains(s1) == false)
                                    {
                                        list1.RemoveAt(x);
                                        list2.RemoveAt(x);
                                        list3.RemoveAt(x);
                                        list4.RemoveAt(x);
                                    }
                                }
                            }
                        }
                        UnicodeUnPeSave();
                    }
                }
            }
        }

        private void UnPeJapanese()//非PE提取格式 1
        {
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    PFL = fs.Length - 1;
                    int i2 = 0;
                    long l1 = 0;
                    string s1 = "";
                    byte b1 = 0;
                    LinkedList<byte> bList = new LinkedList<byte>();
                    while (fs.Position < PFL)
                    {
                        b1 = br.ReadByte();
                        if (b1 > 0)
                        {
                            fs.Seek(-1, SeekOrigin.Current);
                            l1 = fs.Position;
                            i2 = 0;
                            bList.Clear();
                            do
                            {
                                if (fs.Position == PFL)
                                {
                                    b1 = br.ReadByte();
                                    bList.AddLast(b1);
                                    i2++;
                                    break;
                                }
                                else
                                {
                                    b1 = br.ReadByte();
                                }
                                try
                                {
                                    bList.AddLast(b1);
                                }
                                catch
                                {
                                    break;
                                }
                                i2++;
                            } while (b1 > 0);
                            i2--;
                            if (i2 == 1)
                            {
                                continue;
                            }
                            bList.RemoveLast();
                            byte[] bt = new byte[i2];
                            bList.CopyTo(bt, 0);
                            s1 = Encoding.GetEncoding(932).GetString(bt);
                            for (int x = s1.Length - 1; x >= 0; x--)
                            {
                                if (JapaneseStr(s1.Substring(x, 1)) == false)
                                {
                                    s1 = s1.Substring(x + 1);
                                    if (s1 != "")
                                    {
                                        l1 = l1 + i2;
                                        i2 = Encoding.GetEncoding(932).GetByteCount(s1);
                                        l1 = l1 - i2;
                                        list1.Add(l1.ToString("X8"));
                                        list2.Add(s1);
                                        list4.Add(i2);
                                        break;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else if (x == 0)
                                {
                                    list1.Add(l1.ToString("X8"));
                                    list2.Add(s1);
                                    list4.Add(i2);
                                }
                            }
                        }
                    }
                    fs.Close();
                    br.Close();
                    if (list1.Count == 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                    else
                    {
                        if (NewSmartBL == true)
                        {
                            int i1 = list2.Count;
                            for (int x = i1 - 1; x >= 0; x--)
                            {
                                s1 = list2[x].ToString().ToLower();
                                if (s1.Length < 4)
                                {
                                    if (SmartFilterAL.Contains(s1) == false)
                                    {
                                        list1.RemoveAt(x);
                                        list2.RemoveAt(x);
                                        list4.RemoveAt(x);
                                    }
                                }
                            }
                        }
                        UnPeSave();
                    }
                }
            }
        }

        private void UnPeSkyrim()
        {
            int orgcode = Encoding.UTF8.CodePage;
            if (comboBox4.Text == "英语(1252)")
            {
                orgcode = 1252;
            }
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    PFL = fs.Length;
                    int iCount = br.ReadInt32();
                    int iL = br.ReadInt32();
                    LinkedList<byte> bList = new LinkedList<byte>();
                    int iStart = (int)fs.Length - iL;
                    int iP = 0;
                    byte b = 0;
                    int x = 0;
                    long L1 = 0;
                    for (int i = 0; i < iCount; i++)
                    {
                        fs.Seek(i * 8 + 12, SeekOrigin.Begin);
                        iP = br.ReadInt32();
                        fs.Seek(iStart + iP, SeekOrigin.Begin);
                        if (br.ReadUInt32() < 20000)
                        {
                            list3.Add(1);
                        }
                        else
                        {
                            fs.Seek(-4, SeekOrigin.Current);
                            list3.Add(0);
                        }
                        L1 = fs.Position;
                        x = 0;
                        bList.Clear();
                        do
                        {
                            b = br.ReadByte();
                            bList.AddLast(b);
                            x++;
                        } while (b > 0);
                        x--;
                        bList.RemoveLast();
                        byte[] bt = new byte[x];
                        bList.CopyTo(bt, 0);
                        list1.Add(L1.ToString("X8"));
                        list2.Add(Encoding.GetEncoding(orgcode).GetString(bt));
                        list4.Add(x);
                        list11.Add(L1.ToString("X8"));
                        list22.Add(i);
                    }
                }
            }
            if (list1.Count == 0)
            {
                this.Invoke(new Action(delegate
                {
                    MessageBox.Show("没有搜索到任何数据，无法创建工程。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
            else
            {
                SkyrimSave();
            }
        }

        private void SkyrimSave()
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            int orgcode = Encoding.UTF8.CodePage;
            string orgname = comboBox4.Text;
            if (orgname == "英语(1252)")
            {
                orgcode = 1252;
            }
            int tracode = Encoding.UTF8.CodePage;
            string traname = comboBox3.Text;
            string UnPEType = "5";
            Create_Project_DB();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '" + OrgUnPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + PFL.ToString() + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + UnPEType + "' where infoname = '非PE类型'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('UTF8','" + orgname + "'," + orgcode + ",'" + traname + "'," + tracode + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    int i2 = 0;
                    string s1 = "";
                    string s2 = "";
                    int i1 = list1.Count;
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        s1 = list1[i].ToString();
                        s2 = list2[i].ToString().Replace("'", "''");
                        i2 = (int)list4[i];
                        try
                        {
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, utf8, delphi) Values ('"
                                + s1 + "','" + s2 + "'," + i2 + "," + i2 + ",1," + (int)list3[i] + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                    i1 = list11.Count;
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        try
                        {
                            cmd.CommandText = "Insert Into calladd (address, offset) Values ('"
                                + list11[i].ToString() + "'," + (int)list22[i] + ")";
                            cmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            UnPENewProjectTimer.Enabled = false;
            progressBar2.Value = 20;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void UnPeQTpo()
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            Create_Project_DB();
            using (FileStream fs = new FileStream(OrgUnPEFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                PFL = fs.Length;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + Projectname))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        cmd.CommandText = "update fileinfo set detail = '" + OrgUnPEFileName.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                        cmd.Transaction = MyAccess.BeginTransaction();
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update fileinfo set detail = '" + PFL.ToString() + "' where infoname = '大小'";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update fileinfo set detail = '6' where infoname = '非PE类型'";
                        cmd.ExecuteNonQuery();
                        string EdName = "UTF8";
                        Encoding edorg = Encoding.UTF8;
                        Encoding edtra = Encoding.UTF8;
                        StreamReader sr = new StreamReader(fs, edorg);
                        cmd.CommandText = "Insert Into prolanguage (encoding, orgname, orgcode, traname, tracode) Values ('" + EdName + "','" + edorg.CodePage.ToString() + "'," + int.Parse(edorg.CodePage.ToString()) + ",'" + edtra.CodePage.ToString() + "'," + int.Parse(edtra.CodePage.ToString()) + ")";
                        cmd.ExecuteNonQuery();
                        string s = "";
                        string stmp = "";
                        int i = 0;
                        int x = 0;
                        int m = 0;
                        while ((s = sr.ReadLine()) != "")
                        {
                            i++;
                            stmp = s.Replace("'", "''");
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong) Values ('" + string.Format("{0:00000000}", i) + "','" + stmp + "'," + Encoding.UTF8.GetByteCount(s) + ")";
                            cmd.ExecuteNonQuery();
                        }
                        i++;
                        cmd.CommandText = "Insert Into athenaa (address) Values ('" + string.Format("{0:00000000}", i) + "')";
                        cmd.ExecuteNonQuery();
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
                                        if (s.Contains("msgid") == false)
                                        {
                                            i++;
                                            stmp = s.Replace("'", "''");
                                            cmd.CommandText = "Insert Into athenaa (address, org, orglong) Values ('" + string.Format("{0:00000000}", i) + "','" + stmp + "'," + Encoding.UTF8.GetByteCount(s) + ")";
                                            cmd.ExecuteNonQuery();
                                        }
                                        else
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
                                        if (list3[x].ToString() == "msgstr[1] \"\"")
                                        {
                                            for (int y = 0; y < list3.Count; y++)
                                            {
                                                stmp = list3[y].ToString();
                                                if (stmp.Contains("msgid_plural ") == true && stmp.Length > 15)
                                                {
                                                    list3[x] = stmp.Replace("msgid_plural ", "msgstr[1] ");
                                                    break;
                                                }
                                            }
                                        }
                                        if (list3[x].ToString() == "msgstr[1] \"\"")
                                        {
                                            for (int y = 0; y < list3.Count; y++)
                                            {
                                                stmp = list3[y].ToString();
                                                if (stmp.Contains("msgid ") == true && stmp.Length > 8)
                                                {
                                                    list3[x] = stmp.Replace("msgid ", "msgstr[1] ");
                                                    break;
                                                }
                                            }
                                        }
                                        if (list3[x].ToString() == "msgstr[0] \"\"")
                                        {
                                            for (int y = 0; y < list3.Count; y++)
                                            {
                                                stmp = list3[y].ToString();
                                                if (stmp.Contains("msgid ") == true && stmp.Length > 8)
                                                {
                                                    list3[x] = stmp.Replace("msgid ", "msgstr[0] ");
                                                    break;
                                                }
                                            }
                                        }
                                        if (list3[x].ToString() == "msgstr \"\"")
                                        {
                                            for (int y = 0; y < list3.Count; y++)
                                            {
                                                stmp = list3[y].ToString();
                                                if (stmp.Contains("msgid ") == true && stmp.Length > 8)
                                                {
                                                    list3[x] = stmp.Replace("msgid ", "msgstr ");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    for (x = 0; x < list3.Count; x++)
                                    {
                                        i++;
                                        s = list3[x].ToString();
                                        m = Encoding.UTF8.GetByteCount(s);
                                        stmp = s.Replace("'", "''");
                                        if (s.Contains("msgstr") == true)
                                        {
                                            //tra   orglong   tralong
                                            cmd.CommandText = "Insert Into athenaa (address, org, tra, orglong, tralong) Values ('" + string.Format("{0:00000000}", i) + "','" + stmp + "','" + stmp + "'," + m + "," + m + ")";
                                        }
                                        else
                                        {
                                            cmd.CommandText = "Insert Into athenaa (address, org, orglong) Values ('" + string.Format("{0:00000000}", i) + "','" + stmp + "'," + m + ")";
                                        }
                                        cmd.ExecuteNonQuery();
                                    }
                                    list3.Clear();
                                    list1.Clear();
                                }
                                i++;
                                cmd.CommandText = "Insert Into athenaa (address) Values ('" + string.Format("{0:00000000}", i) + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        cmd.Transaction.Commit();
                        sr.Close();
                    }
                }
            }
            mainform.ProjectFileName = Projectname;
            mainform.NewPro = true;
            UnPENewProjectTimer.Enabled = false;
            progressBar2.Value = 20;
            this.Invoke(new Action(delegate
            {
                MessageBox.Show("新建工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }));
        }

        private void NewProject_Shown(object sender, EventArgs e)//显示窗口
        {
            int LY = label4.Location.Y - comboBox1.Location.Y - (int)(comboBox1.Height / 2D - label4.Height / 2D);
            label2.Location = new Point(label2.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label2.Height / 2D));
            label3.Location = new Point(label3.Location.X, textBox3.Location.Y + (int)(textBox3.Height / 2D - label3.Height / 2D));
            label4.Location = new Point(label4.Location.X, label4.Location.Y - LY);
            label5.Location = new Point(label5.Location.X, label5.Location.Y - LY);
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            button2.Location = new Point(button2.Location.X, comboBox1.Location.Y + (int)(comboBox1.Height / 2D - button2.Height / 2D));
            button3.Location = new Point(button3.Location.X, comboBox2.Location.Y + (int)(comboBox2.Height / 2D - button3.Height / 2D));
            label7.Location = new Point(label7.Location.X, label7.Location.Y - LY);
            label8.Location = new Point(label8.Location.X, label8.Location.Y - LY);
            button8.Location = new Point(button8.Location.X, textBox4.Location.Y + (int)(textBox4.Height / 2D - button8.Height / 2D));
            button9.Location = new Point(button9.Location.X, comboBox4.Location.Y + (int)(comboBox4.Height / 2D - button9.Height / 2D));
            button10.Location = new Point(button10.Location.X, comboBox3.Location.Y + (int)(comboBox3.Height / 2D - button10.Height / 2D));
            comboBox1.Text = "英语(1252)";
            comboBox4.Text = "英语(1252)";
            comboBox5.Text = "无";
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
            string s1 = mainform.CDirectory + "保留\\Filter.ini";
            string s2 = mainform.CDirectory + "保留\\默认.db";
            if (File.Exists(s1) == true)
            {
                StreamReader sr = File.OpenText(s1);
                string s3 = sr.ReadLine();
                sr.Close();
                if (File.Exists(s3) == true)
                {
                    checkBox2.Enabled = true;
                    checkBox4.Enabled = true;
                }
                else
                {
                    if (File.Exists(s2) == true)
                    {
                        checkBox2.Enabled = true;
                        checkBox4.Enabled = true;
                        StreamWriter sw = new StreamWriter(s1, false);
                        sw.WriteLine(s2);
                        sw.Close();
                    }
                    else
                    {
                        checkBox2.Enabled = false;
                        checkBox4.Enabled = false;
                    }
                }
            }
            else if (File.Exists(s2) == true)
            {
                checkBox2.Enabled = true;
                checkBox4.Enabled = true;
                StreamWriter sw = new StreamWriter(s1, false);
                sw.WriteLine(s2);
                sw.Close();
            }
            else
            {
                checkBox2.Enabled = false;
                checkBox4.Enabled = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                comboBox2.Enabled = true;
                toolTip1.SetToolTip(comboBox5, "");
                if (comboBox5.Text == "Delphi")
                {
                    toolTip1.SetToolTip(comboBox5, "FF FF FF FF 04 00 00 00 46 69 6C 65 00\r\n例如 File 这个英文单词");
                }
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                comboBox2.Enabled = false;
                toolTip1.SetToolTip(comboBox5, "");
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)
            {
                comboBox2.Enabled = false;
                toolTip1.SetToolTip(comboBox5, "");
                string s = comboBox5.Text;
                if (s == "Delphi")
                {
                    toolTip1.SetToolTip(comboBox5, "FF FF FF FF 04 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                }
                else if (s == "标准")
                {
                    toolTip1.SetToolTip(comboBox5, "04 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                }
                else if (s == "标准2")
                {
                    toolTip1.SetToolTip(comboBox5, "08 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                }
            }
        }

        private void SaveANUnPESetup(string s)//保存配置文件
        {
            ArrayList al = new ArrayList();
            al.Add("File=" + textBox4.Text);
            al.Add("Source=" + comboBox4.Text);
            al.Add("Target=" + comboBox3.Text);
            al.Add("SmartKeep=" + checkBox4.Checked.ToString());
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
            else if (radioButton10.Checked == true)
            {
                al.Add("Format=5");
            }
            else if (radioButton11.Checked == true)
            {
                al.Add("Format=6");
            }
            StreamWriter sw = new StreamWriter(s, false);
            int i1 = al.Count;
            for (int i = 0; i < i1; i++)
            {
                sw.WriteLine(al[i].ToString());//写入配置信息
            }
            al.Clear();
            sw.Close();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            CommonCode.SetupFolder();
            string s1 = mainform.CDirectory + "配置\\ANUnPESetup.ini";
            if (checkBox3.Checked == false)
            {
                if (File.Exists(s1) == false)
                {
                    StreamWriter sw = File.CreateText(s1);
                    sw.Close();
                }
                SaveANUnPESetup(s1);
            }
            else
            {
                string s2 = "";
                SaveFileDialog save = new SaveFileDialog();
                save.InitialDirectory = mainform.CDirectory + "配置";
                save.OverwritePrompt = false;
                save.Filter = "配置文件(*.ini)|*.ini";
                if (save.ShowDialog() == DialogResult.OK)
                {
                    s2 = save.FileName;
                    if (s1 == s2)
                    {
                        MessageBox.Show("手动保存的配置文件不能与默认的配置文件相同，请另外指定不同的文件名。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        SaveANUnPESetup(s2);
                    }
                }
            }
        }

        private void LoadANUnPESetup(string s)//加载配置文件
        {
            try
            {
                ArrayList al = new ArrayList();
                StreamReader sr = File.OpenText(s);//打开配置文件
                while ((s = sr.ReadLine()) != null)//判定是否是最后一行
                {
                    al.Add(s);
                }
                sr.Close();//关闭流
                textBox4.Text = al[0].ToString().Remove(0, 5);
                comboBox4.Text = al[1].ToString().Remove(0, 7);
                comboBox3.Text = al[2].ToString().Remove(0, 7);
                if (al[3].ToString().Remove(0, 10) == "True")
                {
                    checkBox4.Checked = true;
                }
                else
                {
                    checkBox4.Checked = false;
                }
                s = al[4].ToString().Remove(0, 7);
                if (s == "1")
                {
                    radioButton6.Checked = true;
                }
                else if (s == "2")
                {
                    radioButton7.Checked = true;
                }
                else if (s == "3")
                {
                    radioButton8.Checked = true;
                }
                else if (s == "4")
                {
                    radioButton9.Checked = true;
                }
                else if (s == "5")
                {
                    radioButton10.Checked = true;
                }
                else if (s == "6")
                {
                    radioButton11.Checked = true;
                }
            }
            catch
            {
                MessageBox.Show("加载配置文件时出现错误，配置文件不正确或已被非法修改。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string s1 = mainform.CDirectory + "配置";
            string s2 = s1 + "\\ANUnPESetup.ini";
            if (checkBox3.Checked == false)
            {
                if (Directory.Exists(s1) == true)
                {
                    if (File.Exists(s2) == true)
                    {
                        LoadANUnPESetup(s2);
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
                    LoadANUnPESetup(s2);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "所有文件(*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                string s1 = mainform.CDirectory + "工程\\文件\\";
                string s2 = s1 + Path.GetFileName(s);
                if (s.Contains(s1) == true)
                {
                    textBox4.Text = s;
                }
                else
                {
                    DialogResult dr = MessageBox.Show("是否把文件复制到工程目录中？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        if (Directory.Exists(s1) == false)
                        {
                            Directory.CreateDirectory(s1);
                        }
                        if (File.Exists(s2) == true)
                        {
                            DialogResult dlrt = MessageBox.Show("工程目录中已存在这个文件，是否覆盖？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (dlrt == DialogResult.OK)
                            {
                                File.Copy(s, s2, true);
                                textBox4.Text = s2;
                            }
                            else
                            {
                                textBox4.Text = s;
                            }
                        }
                        else
                        {
                            File.Copy(s, s2);
                            textBox4.Text = s2;
                        }
                    }
                    else
                    {
                        textBox4.Text = s;
                    }
                }
            }
        }

        private void tabPage1_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            string s1 = mainform.CDirectory + "工程\\文件\\";
            string s2 = s1 + Path.GetFileName(s);
            if (s.Contains(s1) == true)
            {
                textBox1.Text = s;
            }
            else
            {
                DialogResult dr = MessageBox.Show("是否把文件复制到工程目录中？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dr == DialogResult.OK)
                {
                    if (Directory.Exists(s1) == false)
                    {
                        Directory.CreateDirectory(s1);
                    }
                    if (File.Exists(s2) == true)
                    {
                        DialogResult dlrt = MessageBox.Show("工程目录中已存在这个文件，是否覆盖？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dlrt == DialogResult.OK)
                        {
                            File.Copy(s, s2, true);
                            textBox1.Text = s2;
                        }
                        else
                        {
                            textBox1.Text = s;
                        }
                    }
                    else
                    {
                        File.Copy(s, s2);
                        textBox1.Text = s2;
                    }
                }
                else
                {
                    textBox1.Text = s;
                }
            }
        }

        private void tabPage2_DragDrop(object sender, DragEventArgs e)
        {
            string s = (string)Clipboard.GetData(DataFormats.Text);
            string s1 = mainform.CDirectory + "工程\\文件\\";
            string s2 = s1 + Path.GetFileName(s);
            if (s.Contains(s1) == true)
            {
                textBox4.Text = s;
            }
            else
            {
                DialogResult dr = MessageBox.Show("是否把文件复制到工程目录中？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dr == DialogResult.OK)
                {
                    if (Directory.Exists(s1) == false)
                    {
                        Directory.CreateDirectory(s1);
                    }
                    if (File.Exists(s2) == true)
                    {
                        DialogResult dlrt = MessageBox.Show("工程目录中已存在这个文件，是否覆盖？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dlrt == DialogResult.OK)
                        {
                            File.Copy(s, s2, true);
                            textBox4.Text = s2;
                        }
                        else
                        {
                            textBox4.Text = s;
                        }
                    }
                    else
                    {
                        File.Copy(s, s2);
                        textBox4.Text = s2;
                    }
                }
                else
                {
                    textBox4.Text = s;
                }
            }
        }

        private void tabPage1_DragEnter(object sender, DragEventArgs e)
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

        private void tabPage2_DragEnter(object sender, DragEventArgs e)
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

        private void button12_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked == true)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked == true)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked == true)
            {
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(comboBox5, "");
            string s = comboBox5.Text;
            if (s == "Delphi")
            {
                if (radioButton4.Checked == true)
                {
                    radioButton3.Checked = true;
                }
                radioButton4.Enabled = false;
                if (radioButton3.Checked)
                {
                    toolTip1.SetToolTip(comboBox5, "FF FF FF FF 04 00 00 00 46 69 6C 65 00\r\n例如 File 这个英文单词");
                }
                else if (radioButton5.Checked)
                {
                    toolTip1.SetToolTip(comboBox5, "FF FF FF FF 04 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                }
            }
            else
            {
                radioButton4.Enabled = true;
                checkBox2.Enabled = true;
                if (radioButton5.Checked)
                {
                    if (s == "标准")
                    {
                        toolTip1.SetToolTip(comboBox5, "04 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                    }
                    else if (s == "标准2")
                    {
                        toolTip1.SetToolTip(comboBox5, "08 00 00 00 46 00 69 00 6C 00 65 00 00 00\r\n例如 File 这个英文单词");
                    }
                }
            }
        }

        private BackgroundWorker UnPEBackgroundWorker = new BackgroundWorker();
        private BackgroundWorker ANSI_None_English = new BackgroundWorker();
        private BackgroundWorker ANSI_None_French = new BackgroundWorker();
        private BackgroundWorker ANSI_None_German = new BackgroundWorker();
        private BackgroundWorker ANSI_Delphi_Japanese_Korean = new BackgroundWorker();
        private BackgroundWorker ANSI_Delphi_English = new BackgroundWorker();
        private BackgroundWorker ANSI_None_Japanese = new BackgroundWorker();
        private BackgroundWorker ANSI_None_Korean = new BackgroundWorker();
        private BackgroundWorker UTF8_None_English = new BackgroundWorker();
        private BackgroundWorker Unicode_None_English = new BackgroundWorker();
        private BackgroundWorker Unicode_Delphi = new BackgroundWorker();
        private BackgroundWorker Unicode_Standard1_English = new BackgroundWorker();
        private BackgroundWorker Unicode_Standard2_English = new BackgroundWorker();

        private void NewProject_Load(object sender, EventArgs e)
        {
            UnPEBackgroundWorker.DoWork += UnPEBackgroundWorker_DoWork;
            UnPEBackgroundWorker.RunWorkerCompleted += UnPEBackgroundWorker_RunWorkerCompleted;
            ANSI_None_English.DoWork += ANSI_None_English_DoWork;
            ANSI_None_French.DoWork += ANSI_None_French_DoWork;
            ANSI_None_German.DoWork += ANSI_None_German_DoWork;
            ANSI_Delphi_Japanese_Korean.DoWork += ANSI_Delphi_Japanese_Korean_DoWork;
            ANSI_Delphi_English.DoWork += ANSI_Delphi_English_DoWork;
            ANSI_None_Japanese.DoWork += ANSI_None_Japanese_DoWork;
            ANSI_None_Korean.DoWork += ANSI_None_Korean_DoWork;
            UTF8_None_English.DoWork += UTF8_None_English_DoWork;
            Unicode_None_English.DoWork += Unicode_None_English_DoWork;
            Unicode_Delphi.DoWork += Unicode_Delphi_DoWork;
            Unicode_Standard1_English.DoWork += Unicode_Standard1_English_DoWork;
            Unicode_Standard2_English.DoWork += Unicode_Standard2_English_DoWork;
        }

        private void PENewProjectTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value == 20)
            {
                progressBar1.Value = 0;
            }
            else
            {
                progressBar1.Value++;
            }
        }

        private void UnPENewProjectTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar2.Value == 20)
            {
                progressBar2.Value = 0;
            }
            else
            {
                progressBar2.Value++;
            }
        }
    }
}
