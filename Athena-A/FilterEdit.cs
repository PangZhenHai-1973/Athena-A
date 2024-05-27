using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Text;

namespace Athena_A
{
    public partial class FilterEdit : Form
    {
        static ArrayList DelAL = new ArrayList();

        public FilterEdit()
        {
            InitializeComponent();
        }

        private void OpenFilter(string s)//打开过滤文件
        {
            dataGridView1.DataSource = null;
            ContrlFA();
            DeleteFilterTimer.Enabled = true;
            OpenFilterBackgroundWorker.RunWorkerAsync(s);
        }

        private void FilterEdit_Shown(object sender, EventArgs e)
        {
            contextMenuStrip1.Font = this.Font;
            string s1 = mainform.CDirectory + "过滤";
            if (Directory.Exists(s1) == false)
            {
                Directory.CreateDirectory(s1);
            }
            s1 = mainform.CDirectory + "过滤\\Filter.ini";
            if (File.Exists(s1) == false)
            {
                using (StreamWriter sw = new StreamWriter(s1, false))
                {
                    sw.Write(mainform.CDirectory + "过滤\\默认.db");
                }
            }
            using (StreamReader sr = File.OpenText(s1))
            {
                s1 = sr.ReadLine();
            }
            if (File.Exists(s1) == false)
            {
                s1 = mainform.CDirectory + "过滤\\默认.db";
                if (File.Exists(s1) == false)
                {
                    SQLiteConnection.CreateFile(s1);
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.CommandText = "CREATE TABLE `filter` ("
                                + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "`filterstr`	TEXT DEFAULT ''"
                                + ");";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "CREATE TABLE `showfont` ("
                                + "`fontname`	TEXT DEFAULT '',"
                                + "`fontsize`	NUMERIC DEFAULT 0"
                                + ");";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(mainform.CDirectory + "过滤\\Filter.ini", false))
                    {
                        sw.Write(s1);
                    }
                }
            }
            textBox1.Text = s1;
            OpenFilter(s1);
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            button6.Location = new Point(button6.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - button6.Height / 2D));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)//恢复默认
        {
            string s1 = mainform.CDirectory + "过滤";
            if (Directory.Exists(s1) == false)
            {
                Directory.CreateDirectory(s1);
            }
            s1 = mainform.CDirectory + "过滤\\Filter.ini";
            string s2 = mainform.CDirectory + "过滤\\默认.db";
            using (StreamWriter sw = new StreamWriter(s1, false))
            {
                sw.Write(s2);
            }
            if (File.Exists(s2) == false)
            {
                SQLiteConnection.CreateFile(s2);
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s2))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        cmd.CommandText = "CREATE TABLE `filter` ("
                            + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                            + "`filterstr`	TEXT DEFAULT ''"
                            + ");";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "CREATE TABLE `showfont` ("
                            + "`fontname`	TEXT DEFAULT '',"
                            + "`fontsize`	NUMERIC DEFAULT 0"
                            + ");";
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            textBox1.Text = s2;
            OpenFilter(s2);
        }

        private void button5_Click(object sender, EventArgs e)//另存文件
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "过滤文件(*.db)|*.db";
            sfd.InitialDirectory = mainform.CDirectory + "过滤";
            sfd.OverwritePrompt = false;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string s = sfd.FileName;
                string tem = textBox1.Text;
                if (tem == s)
                {
                    MessageBox.Show("保存的文件不能与当前使用的文件相同。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (File.Exists(tem) == false)
                {
                    MessageBox.Show("源文件不存在，无法保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    File.Copy(textBox1.Text, s, true);
                    MessageBox.Show("保存成功。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)//选择过滤文件
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = mainform.CDirectory + "过滤";
            open.Filter = "过滤文件(*.db)|*.db";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
                {
                    try
                    {
                        MyAccess.Open();
                        MyAccess.Close();
                        OpenFilter(s);
                        textBox1.Text = s;
                        string s1 = mainform.CDirectory + "过滤\\Filter.ini";
                        using (StreamWriter sw = new StreamWriter(s1, false))
                        {
                            sw.Write(s);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("打开过滤文件出错，请确认是否是正确的文件。", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            string s = "条目总行数是 " + dataGridView1.Rows.Count.ToString();
            toolTip1.SetToolTip(textBox1, s);
        }

        private void FilterEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker2.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void ContrlTR()//启用控件
        {
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            dataGridView1.Enabled = true;
        }

        private void ContrlFA()//禁用控件
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            button7.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            dataGridView1.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            if (File.Exists(str1) == false)
            {
                MessageBox.Show("过滤文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                str2 = str2.Replace("/", "//");
                str2 = str2.Replace("'", "''");
                str2 = str2.Replace("%", "/%");
                str2 = str2.Replace("_", "/_");
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + str1))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "PRAGMA case_sensitive_like = 1";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "select * from filter where filterstr Like '%" + str2 + "%' ESCAPE '/'";
                            dataTable1.Clear();
                            dataGridView1.DataSource = null;
                            ad.Fill(dataTable1);
                            dataGridView1.DataSource = dataSet1;
                            dataGridView1.DataMember = dataTable1.TableName;
                        }
                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font != null)
            {
                fontDialog1.Font = filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font;
            }
            else
            {
                fontDialog1.Font = this.Font;
            }
            fontDialog1.ShowDialog();
            textBox2.Font = fontDialog1.Font;
            filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font = fontDialog1.Font;
            string s1 = textBox1.Text;
            if (File.Exists(s1) == true)
            {
                try
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            if (dataTable2.Rows.Count == 1)
                            {
                                cmd.CommandText = "update showfont set fontname ='" + fontDialog1.Font.Name + "', fontsize =" + fontDialog1.Font.Size;
                            }
                            else
                            {
                                cmd.CommandText = "Insert Into showfont(fontname, fontsize) Values ('" + fontDialog1.Font.Name + "'," + fontDialog1.Font.Size + ")";
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                { }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (File.Exists(s1) == false)
            {
                MessageBox.Show("需要整理的过滤文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dataTable1.Clear();
                dataGridView1.DataSource = null;
                ContrlFA();
                DeleteFilterTimer.Enabled = true;
                backgroundWorker2.RunWorkerAsync(s1);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
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
                        cmd.CommandText = "ALTER TABLE filter RENAME TO filterTmp";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "CREATE TABLE `filter` ("
                            + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                            + "`filterstr`	TEXT DEFAULT ''"
                            + ");";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "Insert Into filter(filterstr) select distinct filterstr from filterTmp";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DROP TABLE filterTmp";
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                        dataTable1.Rows.Clear();
                        cmd.CommandText = "select * from filter ORDER BY filterstr ASC";
                        ad.Fill(dataTable1);
                        //cmd.CommandText = "select seq from sqlite_sequence where name = 'filter'";
                        //MessageBox.Show(cmd.ExecuteScalar().ToString());
                    }
                }
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DeleteFilterTimer.Enabled = false;
            ContrlTR();
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            if (dataTable1.Rows.Count > 0)
            {
                progressBar1.Value = progressBar1.Maximum;
                MessageBox.Show("整理完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Value = 0;
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) > 0)
            {
                contextMenuStrip1.Enabled = true;
            }
            else
            {
                contextMenuStrip1.Enabled = false;
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataTable1.Rows.Count > 0)
            {
                int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                DeleteFilterArrayList = new ArrayList();
                for (; intLastRow >= intFirstRow; intLastRow--)
                {
                    DeleteFilterArrayList.Add(intLastRow);
                }
                dataGridView1.DataSource = null;
                progressBar1.Maximum = 20;
                using (BackgroundWorker DeleteFilterBackgroundWorker = new BackgroundWorker())
                {
                    DeleteFilterTimer.Enabled = true;
                    DeleteFilterBackgroundWorker.DoWork += DeleteFilterBackgroundWorker_DoWork;
                    DeleteFilterBackgroundWorker.RunWorkerCompleted += DeleteFilterBackgroundWorker_RunWorkerCompleted;
                    DeleteFilterBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private ArrayList DeleteFilterArrayList;

        private void DeleteFilterBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            StringBuilder SB = new StringBuilder();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + textBox1.Text))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    int i1 = DeleteFilterArrayList.Count;
                    int x = 0, y = 0;
                    cmd.Transaction = MyAccess.BeginTransaction();
                    for (int i = 0; i < i1; i++)
                    {
                        y = (int)DeleteFilterArrayList[i];
                        if (x == 100)
                        {
                            cmd.CommandText = "delete from filter where num = " + (Int64)dataTable1.Rows[y][0] + SB.ToString();
                            cmd.ExecuteNonQuery();
                            SB.Clear();
                            x = 0;
                        }
                        else
                        {
                            SB.Append(" or num = " + (Int64)dataTable1.Rows[y][0]);
                            x++;
                        }
                        dataTable1.Rows.RemoveAt(y);
                    }
                    string s1 = SB.ToString();
                    if (s1 != "")
                    {
                        cmd.CommandText = "delete from filter where" + s1.Remove(0, 3);
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
            }
        }

        private void DeleteFilterBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DeleteFilterTimer.Enabled = false;
            progressBar1.Value = 0;
            DeleteFilterArrayList = null;
            ContrlTR();
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
        }

        private void DeleteFilterTimer_Tick(object sender, EventArgs e)
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

        private void OpenFilterBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + e.Argument.ToString()))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from filter ORDER BY filterstr ASC";
                        dataTable1.Clear();
                        dataGridView1.DataSource = null;
                        ad.Fill(dataTable1);
                        cmd.CommandText = "select * from showfont";
                        dataTable2.Clear();
                        ad.Fill(dataTable2);
                        if (dataTable2.Rows.Count == 1)
                        {
                            Font FEFont = new Font(dataTable2.Rows[0][0].ToString(), float.Parse(dataTable2.Rows[0][1].ToString()));
                            textBox2.Font = FEFont;
                            filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font = FEFont;
                        }
                    }
                }
            }
        }

        private void OpenFilterBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DeleteFilterTimer.Enabled = false;
            progressBar1.Value = 0;
            ContrlTR();
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
        }
    }
}
