using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class ProjectProperties : Form
    {
        public ProjectProperties()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                if (mainform.PEbool != CommonCode.PE(s))
                {
                    MessageBox.Show("文件类型不一致，无法重新设置文件的位置。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (FileVersionInfo.GetVersionInfo(s).FileVersion != textBox2.Text && textBox2.Text != "")
                {
                    MessageBox.Show("版本不一致，无法重新设置文件的位置。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
                    if (fs.Length.ToString() == mainform.FileSize)
                    {
                        textBox1.Text = s;
                        button3.Enabled = true;
                    }
                    else
                    {
                        DialogResult dr = MessageBox.Show("文件的大小不一致，确定仍要重新设置文件的位置吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (dr == DialogResult.OK)
                        {
                            textBox1.Text = s;
                            button3.Enabled = true;
                            textBox3.Text = int.Parse(fs.Length.ToString()).ToString("#,#") + " 字节";
                        }
                    }
                    fs.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string s = comboBox1.Text;
                if (s == "简体中文(936)")
                {
                    mainform.ProTraCode = 936;
                    mainform.ProTraName = s;
                }
                else if (s == "繁体中文(950)")
                {
                    mainform.ProTraCode = 950;
                    mainform.ProTraName = s;
                }
                else
                {
                    mainform.ProTraCode = mainform.AA_Default_Encoding.CodePage;
                    mainform.ProTraName = "默认";
                }
                FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read);
                mainform.FileSize = fs.Length.ToString();
                fs.Close();
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                try
                {
                    MyAccess.Open();
                    SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.CommandText = "update fileinfo set detail = '" + textBox1.Text.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '文件'";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update prolanguage set traname = '" + s + "', tracode =" + mainform.ProTraCode;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "update fileinfo set detail = '" + mainform.FileSize + "' where infoname = '大小'";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    mainform.FilePath = textBox1.Text;
                    textBox7.Text = s;
                    if (File.Exists(mainform.FilePath) == true)
                    {
                        mainform.FPBool = true;
                    }
                }
                catch (Exception MyEx)
                {
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    MyAccess.Close();
                    button3.Enabled = false;
                    textBox4.Select(0, 0);
                }
            }
        }

        private void ProjectProperties_Shown(object sender, EventArgs e)
        {
            int LY = label1.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label1.Height / 2D);
            label1.Location = new Point(label1.Location.X, label1.Location.Y - LY);
            label2.Location = new Point(label2.Location.X, label2.Location.Y - LY);
            label3.Location = new Point(label3.Location.X, label3.Location.Y - LY);
            label4.Location = new Point(label4.Location.X, label4.Location.Y - LY);
            label5.Location = new Point(label5.Location.X, label5.Location.Y - LY);
            label6.Location = new Point(label6.Location.X, label6.Location.Y - LY);
            label7.Location = new Point(label7.Location.X, label7.Location.Y - LY);
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                textBox4.Text = mainform.DictionaryStr;
                textBox1.Text = mainform.FilePath;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select encoding,orgname,traname from prolanguage";
                            using (DataTable TmpDT = new DataTable("prolanguage"))
                            {
                                ad.Fill(TmpDT);
                                textBox5.Text = TmpDT.Rows[0][0].ToString();
                                textBox6.Text = TmpDT.Rows[0][1].ToString();
                                if (TmpDT.Rows[0][0].ToString() != "ANSI")
                                {
                                    textBox7.Enabled = false;
                                    comboBox1.Enabled = false;
                                    textBox7.BackColor = System.Drawing.SystemColors.Control;
                                }
                                else
                                {
                                    textBox7.Text = TmpDT.Rows[0][2].ToString();
                                    comboBox1.Text = TmpDT.Rows[0][2].ToString();
                                    textBox7.BackColor = System.Drawing.Color.White;
                                }
                                if (mainform.PEbool == true)
                                {
                                    textBox8.Text = mainform.CPUType + " 位";
                                }
                                else
                                {
                                    textBox8.Enabled = false;
                                    textBox8.BackColor = System.Drawing.SystemColors.Control;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (textBox7.Text != comboBox1.Text)
            {
                button3.Enabled = true;
            }
        }

        private void label7_DoubleClick(object sender, EventArgs e)
        {
            if (mainform.PEbool == true)
            {
                DialogResult dr = MessageBox.Show("一般情况下只有扩展了 PE 后才需要重读 PE 结构信息。\r\n确实重读 PE 结构信息吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    if (File.Exists(mainform.ProjectFileName) == false)
                    {
                        MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (File.Exists(mainform.FilePath) == false)
                    {
                        MessageBox.Show("没有找到创建此工程时的文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        mainform.l1.Clear();
                        mainform.l2.Clear();
                        mainform.l3.Clear();
                        mainform.l4.Clear();
                        FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read);
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
                            mainform.l1.Add(br.ReadUInt32().ToString());//基址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 128, SeekOrigin.Begin);//输入表
                            mainform.l1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 136, SeekOrigin.Begin);//资源段
                            mainform.l1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 248, SeekOrigin.Begin);//各个段
                            for (int i = 0; i < i1; i++)
                            {
                                fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                                mainform.l1.Add(br.ReadUInt32().ToString());
                                mainform.l2.Add(br.ReadUInt32().ToString());
                                mainform.l3.Add(br.ReadUInt32().ToString());
                                mainform.l4.Add(br.ReadUInt32().ToString());
                                fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                            }
                        }
                        else
                        {
                            fs.Seek(u1 + 6, SeekOrigin.Begin);//读出文件段数
                            i1 = br.ReadUInt16();//文件段数
                            fs.Seek(u1 + 48, SeekOrigin.Begin);//基址
                            mainform.l1.Add(br.ReadUInt64().ToString());//基址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 144, SeekOrigin.Begin);//输入表
                            mainform.l1.Add(br.ReadUInt32().ToString());//输入表虚拟地址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 152, SeekOrigin.Begin);//资源段
                            mainform.l1.Add(br.ReadUInt32().ToString());//资源段虚拟地址
                            mainform.l2.Add("0");
                            mainform.l3.Add("0");
                            mainform.l4.Add("0");
                            fs.Seek(u1 + 264, SeekOrigin.Begin);//各个段
                            for (int i = 0; i < i1; i++)
                            {
                                fs.Seek(fs.Position + 8, SeekOrigin.Begin);
                                mainform.l1.Add(br.ReadUInt32().ToString());
                                mainform.l2.Add(br.ReadUInt32().ToString());
                                mainform.l3.Add(br.ReadUInt32().ToString());
                                mainform.l4.Add(br.ReadUInt32().ToString());
                                fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                            }
                        }
                        mainform.FileSize = fs.Length.ToString();
                        textBox3.Text = int.Parse(mainform.FileSize).ToString("#,#") + " 字节";
                        br.Close();
                        fs.Close();
                        bool bl = true;
                        SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                        MyAccess.Open();
                        SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                        i1 = mainform.l1.Count;
                        cmd.Transaction = MyAccess.BeginTransaction();
                        cmd.CommandText = "delete from pesec";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "update fileinfo set detail = '" + mainform.FileSize + "' where infoname = '大小'";
                        cmd.ExecuteNonQuery();
                        for (int i = 0; i < i1; i++)
                        {
                            cmd.CommandText = "Insert Into pesec (vsize,voffset,rsize,roffset) Values (" + mainform.l1[i].ToString() + "," + mainform.l2[i].ToString() + "," + mainform.l3[i].ToString() + "," + mainform.l4[i].ToString() + ")";
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception x)
                            {
                                MyAccess.Close();
                                bl = false;
                                MessageBox.Show(x.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        cmd.Transaction.Commit();
                        MyAccess.Close();
                        if (bl == true)
                        {
                            MessageBox.Show("重读 PE 结构成功，建议重新打开此工程。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }
    }
}
