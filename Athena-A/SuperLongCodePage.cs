using System;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class SuperLongCodePage : Form
    {
        bool cbl = true;

        public SuperLongCodePage()
        {
            InitializeComponent();
        }

        private void SuperLongCodePage_Shown(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            label1.Text = mainform.ProTraName;
            label2.Text = mainform.ProTraCode.ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    try
                    {
                        cmd.Transaction = MyAccess.BeginTransaction();
                        if (radioButton1.Checked == true)
                        {
                            cmd.CommandText = "update fileinfo set detail = '0' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = 0;
                            mainform.DelphiCodePage = "0";
                        }
                        else if (radioButton2.Checked == true)
                        {
                            cmd.CommandText = "update fileinfo set detail = '1' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = 1;
                            mainform.DelphiCodePage = "1";
                        }
                        else
                        {
                            cmd.CommandText = "update fileinfo set detail = '" + mainform.ProTraCode.ToString() + "' where infoname = '代码页'";
                            cmd.ExecuteNonQuery();
                            mainform.obMS[18] = mainform.ProTraCode;
                            mainform.DelphiCodePage = mainform.ProTraCode.ToString();
                        }
                        cmd.Transaction.Commit();
                        cbl = false;
                        this.Close();
                    }
                    catch (Exception MyEx)
                    {
                        MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void SuperLongCodePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cbl)
            {
                e.Cancel = true;
            }
        }
    }
}
