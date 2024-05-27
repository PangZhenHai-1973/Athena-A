using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class DelphiCodePage : Form
    {
        public DelphiCodePage()
        {
            InitializeComponent();
        }

        private void DelphiCodePage_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                comboBox1.Font = mainform.MyNewFont;
            }
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, comboBox1.Location.Y + (int)(comboBox1.Height / 2D - label2.Height / 2D));
            int i1 = mainform.AA_Default_Encoding.CodePage;
            if (i1 == 936)
            {
                comboBox1.Text = "简体中文(936)";
            }
            else if (i1 == 950)
            {
                comboBox1.Text = "繁体中文(950)";
            }
            else
            {
                comboBox1.Text = "默认";
            }
            textBox1.Text = i1.ToString();
            if (mainform.StrLenCategory == "Delphi")
            {
                if (mainform.ProEncoding == "Unicode")
                {
                    radioButton1.Checked = true;
                    radioButton2.Enabled = false;
                    label1.Enabled = false;
                    label2.Enabled = false;
                    textBox1.Enabled = false;
                    comboBox1.Enabled = false;
                }
                else
                {
                    radioButton1.Enabled = false;
                    radioButton2.Checked = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(mainform.FilePath) == false)
            {
                MessageBox.Show("没有找到创建此工程时的原始文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("没有找到工程文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                if (checkBox1.Checked == true)
                {
                    cmd.CommandText = "select address from athenaa where delphi = 1 and outaddfree = 0 and moveforward = 0 and movebackward = 0 and superlong = 0 and zonebl = 0";
                }
                else
                {
                    cmd.CommandText = "select address from athenaa where tralong > 0 and delphi = 1 and outaddfree =0 and moveforward = 0 and movebackward = 0 and superlong = 0 and zonebl = 0";
                }
                dataTable1.Rows.Clear();
                ad.Fill(dataTable1);
                MyAccess.Close();
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    int i2 = 0;
                    bool bl = false;
                    dataTable2.Rows.Clear();
                    dataGridView1.Rows.Clear();
                    object[] ob = new object[3];
                    ob[1] = 1;
                    ob[2] = false;
                    FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    for (int i = 0; i < i1; i++)
                    {
                        fs.Seek(CommonCode.HexToLong(dataTable1.Rows[i][0].ToString()) - 12, SeekOrigin.Begin);
                        i2 = br.ReadInt16();
                        dataTable1.Rows[i][1] = i2;
                        bl = false;
                        for (int y = 0; y < dataTable2.Rows.Count; y++)
                        {
                            if (i2 == (int)dataTable2.Rows[y][0])
                            {
                                bl = true;
                                dataTable2.Rows[y][1] = (int)dataTable2.Rows[y][1] + 1;
                                break;
                            }
                        }
                        if (bl == false)
                        {
                            ob[0] = i2;
                            dataTable2.Rows.Add(ob);
                        }
                    }
                    br.Close();
                    fs.Close();
                    for (int i = 0; i < dataTable2.Rows.Count; i++)
                    {
                        dataGridView1.Rows.Add(dataTable2.Rows[i].ItemArray);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("没有找到工程文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string s1 = comboBox1.Text;
                int cp = 0;
                if (radioButton1.Checked)
                {
                    cp = 1;
                }
                else
                {
                    if (s1 == "简体中文(936)")
                    {
                        cp = 936;
                    }
                    else if (s1 == "繁体中文(950)")
                    {
                        cp = 950;
                    }
                    else if (s1 == "英语(1252)" || s1 == "法语(1252)" || s1 == "德语(1252)")
                    {
                        cp = 1252;
                    }
                    else if (s1 == "日语(932)")
                    {
                        cp = 932;
                    }
                    else if (s1 == "韩文(949)")
                    {
                        cp = 949;
                    }
                    else if (s1 == "默认")
                    {
                        cp = mainform.AA_Default_Encoding.CodePage;
                    }
                    else if (s1 == "不变")
                    {
                        cp = 1;
                    }
                }
                for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
                {
                    if ((bool)dataGridView1.Rows[i].Cells[2].Value == false)
                    {
                        dataTable2.Rows.RemoveAt(i);
                    }
                }
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                int i1 = 0;
                cmd.Transaction = MyAccess.BeginTransaction();
                cmd.CommandText = "update athenaa set codepage = 0,delphicodepage = 0 where delphi = 1 and outaddfree =0 and moveforward = 0 and movebackward = 0 and superlong = 0 and zonebl = 0";
                cmd.ExecuteNonQuery();
                for (int i = 0; i < dataTable2.Rows.Count; i++)
                {
                    i1 = (int)dataTable2.Rows[i][0];
                    for (int x = 0; x < dataTable1.Rows.Count; x++)
                    {
                        if (i1 == (int)dataTable1.Rows[x][1])
                        {
                            cmd.CommandText = "update athenaa set codepage = 1,delphicodepage = " + cp + " where address = '" + dataTable1.Rows[x][0].ToString() + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                cmd.Transaction.Commit();
                MyAccess.Close();
                button2.Enabled = false;
            }
        }
    }
}
