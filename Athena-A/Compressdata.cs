using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class Compressdata : Form
    {
        public Compressdata()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = CommonCode.Open_Dictionary_File(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (s1 == "")
            {
                MessageBox.Show("请指定需要整理的字典。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("指定的字典不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                {
                    try
                    {
                        MyAccess.Open();
                    }
                    catch
                    {
                        MessageBox.Show("打开字典出错，这个文件不是有效的字典文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    using (DataTable dataTable2 = MyAccess.GetSchema("Tables"))
                    {
                        ArrayList AL = new ArrayList();
                        int i1 = dataTable2.Rows.Count;
                        for (int i = 0; i < i1; i++)
                        {
                            AL.Add(dataTable2.Rows[i][2].ToString());
                        }
                        if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                        {
                            MessageBox.Show("指定的字典文件不是由该程序创建的，无法进行整理。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            int iCount = 0;
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                cmd.CommandText = "select count(org) from tbl";
                                iCount = int.Parse(cmd.ExecuteScalar().ToString());
                            }
                            if (iCount == 0)
                            {
                                MessageBox.Show("在字典中没有找到需要整理的数据。", "确定");
                            }
                            else
                            {
                                label1.Enabled = false;
                                label2.Enabled = false;
                                label3.Enabled = false;
                                textBox1.Enabled = false;
                                button1.Enabled = false;
                                button2.Enabled = false;
                                button3.Enabled = false;
                                ProgressTimer.Enabled = true;
                                LoadingDictionary.RunWorkerAsync(s1);
                            }
                        }
                    }
                }
            }
        }

        private void LoadingDictionary_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + e.Argument.ToString()))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.Transaction = MyAccess.BeginTransaction();
                        cmd.CommandText = "ALTER TABLE tbl RENAME TO tblTmp";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "CREATE TABLE `tbl` ("
                            + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                            + "`org`	TEXT DEFAULT '',"
                            + "`tra`	TEXT DEFAULT ''"
                            + ");";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "Insert Into tbl(org, tra) select distinct org, tra from tblTmp where org!=tra";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DROP TABLE tblTmp";
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void LoadingDictionary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressTimer.Enabled = false;
            MessageBox.Show("字典整理完成。", "确定");
            progressBar1.Value = 0;
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Compressdata_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (LoadingDictionary.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void Compressdata_Shown(object sender, EventArgs e)
        {
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            if (mainform.MyDpi > 96F)
            {
                int MoveY = label2.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label2.Height / 2D);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
                label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value == 50)
            {
                progressBar1.Value = 0;
            }
            else
            {
                progressBar1.Value++;
            }
        }
    }
}
