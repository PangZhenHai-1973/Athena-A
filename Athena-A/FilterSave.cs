using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class FilterSave : Form
    {
        public FilterSave()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool OpenFilter(string s)//打开过滤文件
        {
            bool bl = true;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
            {
                try
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select * from filter ORDER BY filterstr ASC";
                            dataTable1.Clear();
                            ad.Fill(dataTable1);
                            cmd.CommandText = "select * from showfont";
                            dataTable2.Clear();
                            ad.Fill(dataTable2);
                            if (dataTable2.Rows.Count == 1)
                            {
                                Font temfont = new Font(dataTable2.Rows[0][0].ToString(), float.Parse(dataTable2.Rows[0][1].ToString()));
                                textBox2.Font = temfont;
                                filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("打开过滤保留文件出错，请确认是否是正确的文件。", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    bl = false;
                }
            }
            return bl;
        }

        private void FilterSave_Shown(object sender, EventArgs e)
        {
            button3.Location = new Point(button3.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button3.Height / 2D));
            string s1 = mainform.CDirectory + "保留";
            if (Directory.Exists(s1) == false)
            {
                Directory.CreateDirectory(s1);
            }
            s1 = mainform.CDirectory + "保留\\Filter.ini";
            if (File.Exists(s1) == false)
            {
                using (StreamWriter sw = new StreamWriter(s1, false))
                {
                    sw.Write(mainform.CDirectory + "保留\\默认.db");
                }
            }
            using (StreamReader sr = File.OpenText(s1))
            {
                s1 = sr.ReadLine();
            }
            if (File.Exists(s1) == false)
            {
                s1 = mainform.CDirectory + "保留\\默认.db";
                if (File.Exists(s1) == false)
                {
                    SQLiteConnection.CreateFile(s1);
                    using (SQLiteConnection SC = new SQLiteConnection("Data Source=" + s1))
                    {
                        SC.Open();
                        using (SQLiteCommand SCM = new SQLiteCommand(SC))
                        {
                            SCM.Transaction = SC.BeginTransaction();
                            SCM.CommandText = "CREATE TABLE `filter` ("
                                + "`filterstr`	TEXT DEFAULT '',"
                                + "PRIMARY KEY(filterstr)"
                                + ");";
                            SCM.ExecuteNonQuery();
                            SCM.CommandText = "CREATE TABLE `showfont` ("
                                + "`fontname`	TEXT DEFAULT '',"
                                + "`fontsize`	NUMERIC DEFAULT 0"
                                + ");";
                            SCM.ExecuteNonQuery();
                            SCM.Transaction.Commit();
                        }
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(mainform.CDirectory + "保留\\Filter.ini", false))
                    {
                        sw.Write(s1);
                    }
                }
            }
            textBox1.Text = s1;
            using (BackgroundWorker OpenFilterBackgroundWorker = new BackgroundWorker())
            {
                dataGridView1.DataSource = null;
                OpenFilterBackgroundWorker.RunWorkerCompleted += OpenFilterBackgroundWorker_RunWorkerCompleted;
                OpenFilterBackgroundWorker.DoWork += OpenFilterBackgroundWorker_DoWork;
                OpenFilterBackgroundWorker.RunWorkerAsync(s1);
            }
        }

        private void OpenFilterBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + e.Argument.ToString()))
            {
                try
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select * from filter ORDER BY filterstr ASC";
                            dataTable1.Clear();
                            ad.Fill(dataTable1);
                            cmd.CommandText = "select * from showfont";
                            dataTable2.Clear();
                            ad.Fill(dataTable2);
                            if (dataTable2.Rows.Count == 1)
                            {
                                Font temfont = new Font(dataTable2.Rows[0][0].ToString(), float.Parse(dataTable2.Rows[0][1].ToString()));
                                textBox2.Font = temfont;
                                filterstrDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont;
                            }
                        }
                    }
                }
                catch
                {
                    this.Invoke(new Action(delegate
                    {
                        MessageBox.Show("打开过滤保留文件出错，请确认是否是正确的文件。", "错误", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    }));
                }
            }
        }

        private void OpenFilterBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                string s2 = textBox2.Text.ToLower();
                for (int i = 0; i < i1; i++)
                {
                    if (dataTable1.Rows[i][0].ToString() == s2)
                    {
                        dataGridView1.CurrentCell = dataGridView1[0, i];
                        dataGridView1.CurrentCell.Selected = true;
                        break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            if (File.Exists(s1) == false)
            {
                MessageBox.Show("过滤保留文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string s2 = textBox2.Text.ToLower();
                if (s2 == "")
                {
                    MessageBox.Show("请输入需要添加的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    int i1 = dataTable1.Rows.Count;
                    bool bl = true;
                    for (int i = 0; i < i1; i++)
                    {
                        if (dataTable1.Rows[i][0].ToString() == s2)
                        {
                            bl = false;
                            MessageBox.Show("添加的字符串已存在。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                    }
                    if (bl)
                    {
                        string s3 = s2.Replace("'", "''");
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                cmd.CommandText = "Insert Into filter (filterstr) Values ('" + s3 + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                        dataTable1.Rows.Add(s2);
                        dataGridView1.CurrentCell = dataGridView1[0, i1];
                        dataGridView1.CurrentCell.Selected = true;
                    }
                }
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                if (dataTable1.Rows.Count > 0)
                {
                    int i1 = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    string s = dataTable1.Rows[i1][0].ToString().Replace("'", "''");
                    string s1 = textBox1.Text;
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.CommandText = "delete from filter where filterstr = '" + s + "'";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    dataTable1.Rows.RemoveAt(i1);
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
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
                {
                    try
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            cmd.CommandText = "delete from showfont";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "Insert Into showfont(fontname, fontsize) Values ('" + fontDialog1.Font.Name + "'," + fontDialog1.Font.Size + ")";
                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                        }
                    }
                    catch
                    { }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string s1 = mainform.CDirectory + "保留";
            if (Directory.Exists(s1) == false)
            {
                Directory.CreateDirectory(s1);
            }
            s1 = mainform.CDirectory + "保留\\Filter.ini";
            string s2 = mainform.CDirectory + "保留\\默认.db";
            using (StreamWriter sw = new StreamWriter(s1, false))
            {
                sw.Write(s2);
            }
            if (File.Exists(s2) == false)
            {
                SQLiteConnection.CreateFile(s2);
                using (SQLiteConnection SC = new SQLiteConnection("Data Source=" + s2))
                {
                    SC.Open();
                    using (SQLiteCommand SCM = new SQLiteCommand(SC))
                    {
                        SCM.Transaction = SC.BeginTransaction();
                        SCM.CommandText = "CREATE TABLE `filter` ("
                            + "`filterstr`	TEXT DEFAULT '',"
                            + "PRIMARY KEY(filterstr)"
                            + ");";
                        SCM.ExecuteNonQuery();
                        SCM.CommandText = "CREATE TABLE `showfont` ("
                            + "`fontname`	TEXT DEFAULT '',"
                            + "`fontsize`	NUMERIC DEFAULT 0"
                            + ");";
                        SCM.ExecuteNonQuery();
                        SCM.Transaction.Commit();
                    }
                }
            }
            textBox1.Text = s2;
            OpenFilter(s2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "过滤保留文件(*.db)|*.db";
            sfd.InitialDirectory = mainform.CDirectory + "保留";
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

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = mainform.CDirectory + "保留";
            open.Filter = "过滤保留文件(*.db)|*.db";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                if (OpenFilter(s))
                {
                    textBox1.Text = s;
                    string s1 = mainform.CDirectory + "保留\\Filter.ini";
                    using (StreamWriter sw = new StreamWriter(s1, false))
                    {
                        sw.Write(s);
                    }
                }
            }
        }
    }
}
