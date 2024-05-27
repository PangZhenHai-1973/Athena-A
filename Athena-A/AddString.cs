using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class AddString : Form
    {

        int UTF8bool = 0;
        int Unicodebool = 0;
        int Delphibool = 0;
        string address = "";
        string org = "";
        int orglong = 0;

        public AddString()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("没有找到工程文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                org = org.Replace("'", "''");
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        try
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            cmd.CommandText = "Insert Into athenaa (address, org, orglong, free, delphi, utf8, ucode) Values ('" + address + "','" + org + "'," + orglong + "," + orglong + "," + Delphibool + "," + UTF8bool + "," + Unicodebool + ")";
                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                            button2.Enabled = false;
                            mainform.AddStringbl = true;
                            mainform.AddStr = address;
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (s1 != "")
            {
                if (CommonCode.Is_Hex(s1) == false)
                {
                    MessageBox.Show("输入的地址不是十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Clear();
                }
            }
        }

        void AutoTest(int i1)//地址和长度
        {
            using (FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    address = i1.ToString("X8");
                    fs.Seek(i1, SeekOrigin.Begin);
                    LinkedList<byte> bList = new LinkedList<byte>();
                    int item = br.ReadByte();
                    int i2 = 0;
                    if (textBox3.Text != "")
                    {
                        i2 = int.Parse(textBox3.Text);
                    }
                    bool errorbl = false;
                    if (item == 0)
                    {
                        MessageBox.Show("输入的地址处为“00”，无法找到字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        orglong = 0;
                        fs.Seek(i1, SeekOrigin.Begin);
                        if (Unicodebool == 1)
                        {
                            if (i2 == 0)
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadUInt16();
                                        orglong = orglong + 2;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0);
                            }
                            else
                            {
                                if (Math.IEEERemainder(i2, 2) != 0)
                                {
                                    i2++;
                                }
                                do
                                {
                                    try
                                    {
                                        item = br.ReadUInt16();
                                        orglong = orglong + 2;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0 && orglong < i2);
                            }
                            if (errorbl == false)
                            {
                                orglong = orglong - 2;
                            }
                            if (orglong > 0)
                            {
                                fs.Seek(i1, SeekOrigin.Begin);
                                for (int i = 0; i < orglong; i++)
                                {
                                    bList.AddLast(br.ReadByte());
                                }
                                byte[] bt = new byte[orglong];
                                bList.CopyTo(bt, 0);
                                org = Encoding.Unicode.GetString(bt);
                                textBox2.Text = org;
                                fs.Seek(i1 - 4, SeekOrigin.Begin);
                                if (orglong == br.ReadUInt32())
                                {
                                    Delphibool = 1;
                                }
                                else
                                {
                                    Delphibool = 0;
                                }
                                button2.Enabled = true;
                            }
                        }
                        else if (UTF8bool == 1)
                        {
                            if (i2 == 0)
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0);
                            }
                            else
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0 && orglong < i2);
                            }
                            if (errorbl == false)
                            {
                                orglong = orglong - 1;
                            }
                            if (orglong > 0)
                            {
                                fs.Seek(i1, SeekOrigin.Begin);
                                for (int i = 0; i < orglong; i++)
                                {
                                    bList.AddLast(br.ReadByte());
                                }
                                byte[] bt = new byte[orglong];
                                bList.CopyTo(bt, 0);
                                org = Encoding.UTF8.GetString(bt);
                                textBox2.Text = org;
                                button2.Enabled = true;
                            }
                        }
                        else if (Delphibool == 1)
                        {
                            if (i2 == 0)
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0);
                            }
                            else
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0 && orglong < i2);
                            }
                            if (errorbl == false)
                            {
                                orglong = orglong - 1;
                            }
                            if (orglong > 0)
                            {
                                fs.Seek(i1, SeekOrigin.Begin);
                                for (int i = 0; i < orglong; i++)
                                {
                                    bList.AddLast(br.ReadByte());
                                }
                                byte[] bt = new byte[orglong];
                                bList.CopyTo(bt, 0);
                                org = Encoding.GetEncoding(mainform.ProOrgCode).GetString(bt);
                                textBox2.Text = org;
                                fs.Seek(i1 - 8, SeekOrigin.Begin);
                                if (br.ReadUInt32() == 4294967295)
                                {
                                    Delphibool = 1;
                                }
                                else
                                {
                                    Delphibool = 0;
                                }
                                button2.Enabled = true;
                            }
                        }
                        else
                        {
                            if (i2 == 0)
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0);
                            }
                            else
                            {
                                do
                                {
                                    try
                                    {
                                        item = br.ReadByte();
                                        orglong++;
                                    }
                                    catch
                                    {
                                        errorbl = true;
                                        break;
                                    }
                                } while (item != 0 && orglong < i2);
                            }
                            if (errorbl == false)
                            {
                                orglong = orglong - 1;
                            }
                            if (orglong > 0)
                            {
                                fs.Seek(i1, SeekOrigin.Begin);
                                for (int i = 0; i < orglong; i++)
                                {
                                    bList.AddLast(br.ReadByte());
                                }
                                byte[] bt = new byte[orglong];
                                bList.CopyTo(bt, 0);
                                org = Encoding.GetEncoding(mainform.ProOrgCode).GetString(bt);
                                textBox2.Text = org;
                                button2.Enabled = true;
                            }
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;//地址
            if (s1 == "")
            {
                MessageBox.Show("请输入检测的地址。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                textBox2.Clear();
                int i1 = (int)CommonCode.HexToLong(s1);//地址
                if (File.Exists(mainform.ProjectFileName) == false)
                {
                    MessageBox.Show("没有找到工程文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (File.Exists(mainform.FilePath) == false)
                {
                    MessageBox.Show("没有找到创建这个工程时用到的文件，无法检测。查看工程属性了解详细信息。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (i1 > int.Parse(mainform.FileSize))
                {
                    MessageBox.Show("输入的地址不能超过文件的大小。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    textBox1.Text = CommonCode.FormatStrHex(s1);
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                            {
                                cmd.CommandText = "select address,orglong from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                                using (DataTable TmpDT = new DataTable("athenaa"))
                                {
                                    ad.Fill(TmpDT);
                                    int i4 = 0;
                                    int i5 = 0;
                                    bool bl = true;
                                    if (TmpDT.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < TmpDT.Rows.Count; i++)
                                        {
                                            i4 = (int)CommonCode.HexToLong(TmpDT.Rows[i][0].ToString());//地址
                                            i5 = int.Parse(TmpDT.Rows[i][1].ToString());//英文字符串长度
                                            if (i1 >= i4 && i1 <= i4 + i5)
                                            {
                                                MessageBox.Show("输入的检测地址已包含在 " + TmpDT.Rows[i][0].ToString() + " 之中，请检查输入的地址是否正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                bl = false;
                                                break;
                                            }
                                        }
                                    }
                                    if (bl)
                                    {
                                        if (mainform.ProEncoding == "UTF8")
                                        {
                                            UTF8bool = 1;
                                        }
                                        if (mainform.ProEncoding == "Unicode")
                                        {
                                            Unicodebool = 1;
                                        }
                                        if (mainform.StrLenCategory == "Delphi")
                                        {
                                            Delphibool = 1;
                                        }
                                        AutoTest(i1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
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
                    textBox1.Text = str1;
                }
            }
            else
            {
                MessageBox.Show("粘贴板中没有数据，或是其中的数据不是文本内容。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            string s3 = textBox3.Text;
            if (s3 != "")
            {
                try
                {
                    uint u = uint.Parse(s3);
                }
                catch
                {
                    MessageBox.Show("输入的长度不是有效的整数值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddString_Shown(object sender, EventArgs e)
        {
            int i1 = textBox1.Location.Y + (int)(textBox1.Height / 2D - label2.Height / 2D);
            label2.Location = new Point(label2.Location.X, i1);
            label3.Location = new Point(label3.Location.X, i1);
            int i2 = textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D);
            button1.Location = new Point(button1.Location.X, i2);
            button2.Location = new Point(button2.Location.X, i2);
            button3.Location = new Point(button3.Location.X, i2);
            if (mainform.MyDpi < 144F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning32;
            }
            else if (mainform.MyDpi >= 144F && mainform.MyDpi < 192F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning48;
            }
            else if (mainform.MyDpi >= 192F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning64;
            }
        }
    }
}
