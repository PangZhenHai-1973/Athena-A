using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class ExportDictionary : Form
    {
        public ExportDictionary()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s = mainform.CDirectory + "字典";
            if (Directory.Exists(s) == false)
            {
                Directory.CreateDirectory(s);
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = s;
            sfd.OverwritePrompt = false;
            sfd.Filter = "Athena-A 字典文件(*.db)|*.db";
            FileInfo FI = new FileInfo(mainform.FilePath);
            sfd.FileName = FI.Name.Replace(FI.Extension, "") + ".db";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = sfd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请指定要保存的字典文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                label1.Enabled = false;
                label2.Enabled = false;
                textBox1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                ExportTimer.Enabled = true;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = textBox1.Text;
            using (DataTable dataTable1 = new DataTable("DTTmp"))
            {
                using (SQLiteConnection MyAccess1 = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess1.Open();
                    using (SQLiteCommand cmd1 = new SQLiteCommand(MyAccess1))
                    {
                        using (SQLiteDataAdapter ad1 = new SQLiteDataAdapter(cmd1))
                        {
                            cmd1.CommandText = "select org, tra from athenaa where tralong > 0";
                            dataTable1.Clear();
                            ad1.Fill(dataTable1);
                        }
                    }
                }
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    string s2 = "0";
                    if (File.Exists(s1) == false)
                    {
                        SQLiteConnection.CreateFile(s1);
                        s2 = "";
                    }
                    using (SQLiteConnection MyAccess2 = new SQLiteConnection("Data Source=" + s1))
                    {
                        MyAccess2.Open();
                        using (SQLiteCommand cmd2 = new SQLiteCommand(MyAccess2))
                        {
                            if (s2 == "")
                            {
                                cmd2.CommandText = "CREATE TABLE `diclanguage` ("
                                    + "`orgfontname`	TEXT DEFAULT '',"
                                    + "`orgfontsize`	NUMERIC DEFAULT 0,"
                                    + "`trafontname`	TEXT DEFAULT '',"
                                    + "`trafontsize`	NUMERIC DEFAULT 0"
                                    + ");";
                                cmd2.ExecuteNonQuery();
                                cmd2.CommandText = "CREATE TABLE `tbl` ("
                                    + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                                    + "`org`	TEXT DEFAULT '',"
                                    + "`tra`	TEXT DEFAULT ''"
                                    + ");";
                                cmd2.ExecuteNonQuery();
                            }
                            cmd2.Transaction = MyAccess2.BeginTransaction();
                            for (int i = 0; i < i1; i++)
                            {
                                s1 = dataTable1.Rows[i][0].ToString();
                                s2 = dataTable1.Rows[i][1].ToString();
                                s1 = s1.Replace("'", "''");
                                s2 = s2.Replace("'", "''");
                                cmd2.CommandText = "Insert Into tbl (org,tra) Values ('" + s1 + "','" + s2 + "')";
                                try
                                {
                                    cmd2.ExecuteNonQuery();
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            cmd2.Transaction.Commit();
                        }
                    }
                    this.Invoke(new Action(delegate
                    {
                        ExportTimer.Enabled = false;
                        progressBar1.Value = progressBar1.Maximum;
                        MessageBox.Show("导出字典完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate
                    {
                        ExportTimer.Enabled = false;
                        progressBar1.Value = 0;
                        MessageBox.Show("没有可导出的内容。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
            }
        }

        private void ExportDictionary_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar1.Value = 0;
            label1.Enabled = true;
            label2.Enabled = true;
            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void ExportDictionary_Shown(object sender, EventArgs e)
        {
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            label2.Location = new Point(label2.Location.X, progressBar1.Location.Y + (int)(progressBar1.Height / 2D - label2.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
        }

        private void ExportTimer_Tick(object sender, EventArgs e)
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
