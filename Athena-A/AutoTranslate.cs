using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class AutoTranslate : Form
    {
        public AutoTranslate()
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
            label1.Enabled = false;
            label2.Enabled = false;
            checkBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            textBox1.Enabled = false;
            string s1 = textBox1.Text;
            if (s1 == "")
            {
                MessageBox.Show("请指定用于翻译的字典。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("指定的字典不存在。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (checkBox1.Checked == false)
                {
                    MessageBox.Show("在这之前最好先对字典进行整理。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("二次翻译将清除所有字符串挪移设置。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                ProgressBarTimer.Enabled = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            bool bl;
            if (checkBox1.Checked == false)
            {
                bl = false;
            }
            else
            {
                bl = true;
            }
            using (SQLiteConnection MyAccess1 = new SQLiteConnection("Data Source=" + textBox1.Text))
            {
                MyAccess1.Open();
                using (DataTable dataTable3 = MyAccess1.GetSchema("Tables"))
                {
                    ArrayList AL = new ArrayList();
                    int i3 = dataTable3.Rows.Count;
                    for (int i = 0; i < i3; i++)
                    {
                        AL.Add(dataTable3.Rows[i][2].ToString());
                    }
                    if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                    {
                        this.Invoke(new Action(delegate
                        {
                            ProgressBarTimer.Enabled = false;
                            progressBar1.Value = progressBar1.Maximum;
                            MessageBox.Show("指定的字典文件不是由该程序创建的，无法进行翻译。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }));
                    }
                    else
                    {
                        using (SQLiteCommand cmd1 = new SQLiteCommand(MyAccess1))
                        {
                            using (SQLiteDataAdapter ad1 = new SQLiteDataAdapter(cmd1))
                            {
                                cmd1.CommandText = "select distinct org,tra from tbl ORDER BY org ASC";
                                ad1.Fill(dataTable1);
                                if (dataTable1.Rows.Count == 0)
                                {
                                    this.Invoke(new Action(delegate
                                    {
                                        ProgressBarTimer.Enabled = false;
                                        progressBar1.Value = progressBar1.Maximum;
                                        MessageBox.Show("字典中没有任何内容，无法进行翻译。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }));
                                }
                                else
                                {
                                    using (SQLiteConnection MyAccess2 = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                                    {
                                        MyAccess2.Open();
                                        using (SQLiteCommand cmd2 = new SQLiteCommand(MyAccess2))
                                        {
                                            using (SQLiteDataAdapter ad2 = new SQLiteDataAdapter(cmd2))
                                            {
                                                dataTable2.Clear();
                                                if (bl == false)
                                                {
                                                    if (mainform.UnPEType == "6")
                                                    {
                                                        cmd2.CommandText = "select * from athenaa where tralong > 0";
                                                    }
                                                    else
                                                    {
                                                        cmd2.CommandText = "select * from athenaa where tralong = 0";
                                                    }
                                                }
                                                else
                                                {
                                                    cmd2.CommandText = "select * from athenaa where tralong > 0";
                                                }
                                                ad2.Fill(dataTable2);
                                                //AutoInc 22 自动增长列
                                                int i2 = dataTable2.Rows.Count;
                                                if (i2 == 0)
                                                {
                                                    this.Invoke(new Action(delegate
                                                    {
                                                        ProgressBarTimer.Enabled = false;
                                                        progressBar1.Value = progressBar1.Maximum;
                                                        MessageBox.Show("没有找到符合条件的可翻译项。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    }));
                                                }
                                                else
                                                {
                                                    string tra2 = "";
                                                    int tralong4 = 0;
                                                    int free7 = 0;
                                                    try
                                                    {
                                                        //   0       1     2       3         4        5        6        7       8          9          10            11
                                                        //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                                                        //   12         13      14      15      16        17            18           19          20          21
                                                        //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                                                        //            实际是                           没有使用        0 无
                                                        //           长度标识                                         1 不变
                                                        cmd2.Transaction = MyAccess2.BeginTransaction();
                                                        if (bl == false)
                                                        {
                                                            if (mainform.UnPEType == "6")
                                                            {
                                                                var dtResult = from dt2 in dataTable2.AsEnumerable()
                                                                               join dt1 in dataTable1.AsEnumerable()
                                                                               on dt2.Field<string>("org") equals dt1.Field<string>("org")
                                                                               select new
                                                                               {
                                                                                   v1 = dt2.Field<Int32>("AutoInc"),
                                                                                   v2 = dt2.Field<string>("address"),
                                                                                   v3 = dt1.Field<string>("tra"),
                                                                               };
                                                                ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                                                                foreach (var y in dtResult)
                                                                {
                                                                    if (cdTmp.TryAdd(y.v1, 0))
                                                                    {
                                                                        tra2 = y.v3;
                                                                        tralong4 = Encoding.UTF8.GetByteCount(tra2);
                                                                        tra2 = tra2.Replace("'", "''");
                                                                        cmd2.CommandText = "update athenaa set tra = '" + tra2 + "'," + "tralong = " + tralong4 + " where address = '" + y.v2 + "'";
                                                                        cmd2.ExecuteNonQuery();
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var dtResult = from dt2 in dataTable2.AsEnumerable()
                                                                               join dt1 in dataTable1.AsEnumerable()
                                                                               on dt2.Field<string>("org") equals dt1.Field<string>("org")
                                                                               select new
                                                                               {
                                                                                   v1 = dt2.Field<Int32>("AutoInc"),
                                                                                   v2 = dt2.Field<string>("address"),
                                                                                   v3 = dt2.Field<string>("org"),
                                                                                   v4 = dt1.Field<string>("tra"),
                                                                                   v5 = dt2.Field<int>("orglong"),
                                                                                   v14 = dt2.Field<bool>("utf8"),
                                                                                   v15 = dt2.Field<bool>("ucode")
                                                                               };
                                                                ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                                                                foreach (var y in dtResult)
                                                                {
                                                                    if (cdTmp.TryAdd(y.v1, 0))
                                                                    {
                                                                        tra2 = y.v4;
                                                                        if (y.v14)
                                                                        {
                                                                            tralong4 = Encoding.UTF8.GetByteCount(tra2);
                                                                        }
                                                                        else if (y.v15)
                                                                        {
                                                                            tralong4 = Encoding.Unicode.GetByteCount(tra2);
                                                                        }
                                                                        else
                                                                        {
                                                                            tralong4 = Encoding.GetEncoding(mainform.ProTraCode).GetByteCount(tra2);
                                                                        }
                                                                        free7 = y.v5 - tralong4;
                                                                        tra2 = tra2.Replace("'", "''");
                                                                        cmd2.CommandText = "update athenaa set tra = '" + tra2 + "'," + "tralong = " + tralong4 + ", free = " + free7 + " where address = '" + y.v2 + "'";
                                                                        cmd2.ExecuteNonQuery();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (mainform.UnPEType != "5")
                                                            {
                                                                cmd2.CommandText = "delete from calladd";
                                                                cmd2.ExecuteNonQuery();
                                                            }
                                                            cmd2.CommandText = "update athenaa set addr = '',addrlong = 0,outadd = '',outaddfree = 0,moveforward = 0,movebackward = 0, superlong = 0, zonebl = 0 where tralong > 0";
                                                            cmd2.ExecuteNonQuery();
                                                            cmd2.CommandText = "delete from matrixzone";
                                                            cmd2.ExecuteNonQuery();
                                                            cmd2.CommandText = "delete from matrix";
                                                            cmd2.ExecuteNonQuery();
                                                            var dtResult = from dt2 in dataTable2.AsEnumerable()
                                                                           join dt1 in dataTable1.AsEnumerable()
                                                                           on dt2.Field<string>("tra") equals dt1.Field<string>("org")
                                                                           select new
                                                                           {
                                                                               v1 = dt2.Field<Int32>("AutoInc"),
                                                                               v2 = dt2.Field<string>("address"),
                                                                               v3 = dt2.Field<string>("org"),
                                                                               v4 = dt1.Field<string>("tra"),
                                                                               v5 = dt2.Field<int>("orglong"),
                                                                               v14 = dt2.Field<bool>("utf8"),
                                                                               v15 = dt2.Field<bool>("ucode")
                                                                           };
                                                            ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                                                            foreach (var y in dtResult)
                                                            {
                                                                if (cdTmp.TryAdd(y.v1, 0))
                                                                {
                                                                    tra2 = y.v4;
                                                                    if (y.v14)
                                                                    {
                                                                        tralong4 = Encoding.UTF8.GetByteCount(tra2);
                                                                    }
                                                                    else if (y.v15)
                                                                    {
                                                                        tralong4 = Encoding.Unicode.GetByteCount(tra2);
                                                                    }
                                                                    else
                                                                    {
                                                                        tralong4 = Encoding.GetEncoding(mainform.ProTraCode).GetByteCount(tra2);
                                                                    }
                                                                    free7 = y.v5 - tralong4;
                                                                    tra2 = tra2.Replace("'", "''");
                                                                    cmd2.CommandText = "update athenaa set tra = '" + tra2 + "'," + "tralong = " + tralong4 + ", free = " + free7 + " where address = '" + y.v2 + "'";
                                                                    cmd2.ExecuteNonQuery();
                                                                }
                                                            }
                                                        }
                                                        cmd2.Transaction.Commit();
                                                        this.Invoke(new Action(delegate
                                                        {
                                                            ProgressBarTimer.Enabled = false;
                                                            progressBar1.Value = progressBar1.Maximum;
                                                            MessageBox.Show("自动翻译完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        }));
                                                        progressBar1.Value = 0;
                                                        mainform.AutoTrabl = true;
                                                    }
                                                    catch (Exception MyEx)
                                                    {
                                                        this.Invoke(new Action(delegate
                                                        {
                                                            ProgressBarTimer.Enabled = false;
                                                            progressBar1.Value = progressBar1.Maximum;
                                                            MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        }));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AutoTranslate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Enabled = true;
            label2.Enabled = true;
            checkBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            textBox1.Enabled = true;
        }

        private void AutoTranslate_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
                label2.Location = new Point(label2.Location.X, progressBar1.Location.Y + (int)(progressBar1.Height / 2D - label2.Height / 2D));
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
    }
}
