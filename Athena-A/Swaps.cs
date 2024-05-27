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
    public partial class Swaps : Form
    {
        public Swaps()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
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
                MessageBox.Show("请指定需要处理的字典。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("需要处理的字典不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ProgressBarTimer.Enabled = true;
                DisableControl();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void DisableControl()
        {
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
        }

        private void EnableControl()
        {
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = textBox1.Text;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
            {

                try
                {
                    MyAccess.Open();
                }
                catch
                {
                    MyAccess.Close();
                    this.Invoke(new Action(delegate
                    {
                        ProgressBarTimer.Enabled = false;
                        progressBar1.Value = 0;
                        MessageBox.Show("打开字典出错，这个文件不是有效的字典文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                    return;
                }
                using (DataTable dataTmp = MyAccess.GetSchema("Tables"))
                {
                    ArrayList AL = new ArrayList();
                    int iTmp = dataTmp.Rows.Count;
                    for (int i = 0; i < iTmp; i++)
                    {
                        AL.Add(dataTmp.Rows[i][2].ToString());
                    }
                    if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                    {
                        MyAccess.Close();
                        this.Invoke(new Action(delegate
                        {
                            ProgressBarTimer.Enabled = false;
                            progressBar1.Value = 0;
                            MessageBox.Show("指定的字典不是由该程序创建的字典文件，因此无法操作。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    else
                    {
                        using (DataTable zdDataTmp = new DataTable("zdTmp"))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                cmd.CommandText = "select org, tra from tbl";
                                using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                                {
                                    ad.Fill(zdDataTmp);
                                    int i1 = zdDataTmp.Rows.Count;
                                    if (i1 == 0)
                                    {
                                        MyAccess.Close();
                                        this.Invoke(new Action(delegate
                                        {
                                            ProgressBarTimer.Enabled = false;
                                            progressBar1.Value = 0;
                                            MessageBox.Show("指定的字典中没有任何内容，交换操作被终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }));
                                    }
                                    else
                                    {
                                        cmd.Transaction = MyAccess.BeginTransaction();
                                        cmd.CommandText = "delete from tbl";
                                        cmd.ExecuteNonQuery();
                                        cmd.CommandText = "delete from sqlite_sequence";
                                        cmd.ExecuteNonQuery();
                                        string str1 = "";
                                        string str2 = "";
                                        for (int i = 0; i < i1; i++)
                                        {
                                            str1 = zdDataTmp.Rows[i][0].ToString().Replace("'", "''");
                                            str2 = zdDataTmp.Rows[i][1].ToString().Replace("'", "''");
                                            cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + str2 + "','" + str1 + "')";
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
                                        cmd.CommandText = "VACUUM";
                                        cmd.ExecuteNonQuery();
                                        MyAccess.Close();
                                        this.Invoke(new Action(delegate
                                        {
                                            ProgressBarTimer.Enabled = false;
                                            progressBar1.Value = progressBar1.Maximum;
                                            MessageBox.Show("交换成功。", "确定");
                                        }));
                                        progressBar1.Value = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Swaps_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void Swaps_Shown(object sender, EventArgs e)
        {
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            if (mainform.MyDpi > 96F)
            {
                int MoveY = label2.Location.Y - textBox1.Location.Y - (int)(textBox1.Height / 2D - label2.Height / 2D);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
                label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
            }
        }

        private void ProgressBarTimer_Tick(object sender, EventArgs e)
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

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EnableControl();
        }
    }
}
