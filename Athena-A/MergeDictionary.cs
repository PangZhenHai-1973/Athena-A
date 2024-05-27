using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace Athena_A
{
    public partial class MergeDictionary : Form
    {
        int FT = 0;//0 db 1 mdb 2 txt

        public MergeDictionary()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.OverwritePrompt = false;
            sf.InitialDirectory = mainform.CDirectory + "字典";
            sf.Filter = "Athena-A 字典文件(*.db)|*.db";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                string s = sf.FileName;
                if (File.Exists(s) == true)
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
                    {
                        MyAccess.Open();
                        using (DataTable dataTable2 = MyAccess.GetSchema("Tables"))
                        {
                            ArrayList AL = new ArrayList();
                            int iTmp = dataTable2.Rows.Count;
                            for (int i = 0; i < iTmp; i++)
                            {
                                AL.Add(dataTable2.Rows[i][2].ToString());
                            }
                            if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                            {
                                MessageBox.Show("文件不是由 Athena-A 创建的字典文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                textBox1.Text = s;
                            }
                        }
                    }
                }
                else
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.CommandText = "CREATE TABLE `diclanguage` ("
                                + "`orgfontname`	TEXT DEFAULT '',"
                                + "`orgfontsize`	NUMERIC DEFAULT 0,"
                                + "`trafontname`	TEXT DEFAULT '',"
                                + "`trafontsize`	NUMERIC DEFAULT 0"
                                + ");";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "CREATE TABLE `tbl` ("
                                + "`num`	INTEGER PRIMARY KEY AUTOINCREMENT,"
                                + "`org`	TEXT DEFAULT '',"
                                + "`tra`	TEXT DEFAULT ''"
                                + ");";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    textBox1.Text = s;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //0 db 1 mdb 2 txt
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Athena-A 字典文件(*.db)|*.db|文本文件(*.txt)|*.txt";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string s = open.FileName;
                if (Path.GetExtension(s).ToLower() == ".db")
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
                    {
                        MyAccess.Open();
                        using (DataTable dataTable2 = MyAccess.GetSchema("Tables"))
                        {
                            ArrayList AL = new ArrayList();
                            int iTmp = dataTable2.Rows.Count;
                            for (int i = 0; i < iTmp; i++)
                            {
                                AL.Add(dataTable2.Rows[i][2].ToString());
                            }
                            if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                            {
                                MessageBox.Show("文件不是由 Athena-A 创建的字典文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                textBox2.Text = s;
                                FT = 0;
                            }
                        }
                    }
                }
                else if (Path.GetExtension(s).ToLower() == ".txt")
                {
                    textBox2.Text = s;
                    FT = 2;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool CheckSplit(string s)
        {
            string pt = "[\t]";
            Regex r = new Regex(pt);
            if (r.Matches(s).Count == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DisableControl()
        {
            label1.Enabled = false;
            label2.Enabled = false;
            label3.Enabled = false;
            label4.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void EnableControl()
        {
            label1.Enabled = true;
            label2.Enabled = true;
            label3.Enabled = true;
            label4.Enabled = true;
            textBox1.Enabled = true;
            textBox2.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            if (s1 == "")
            {
                MessageBox.Show("请指定要保存的字典文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (s2 == "")
            {
                MessageBox.Show("请指定需要导入的字典文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (s1 == s2)
            {
                MessageBox.Show("你指定的是同一个字典文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(s1) == false)
            {
                MessageBox.Show("需要保存的字典文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(s2) == false)
            {
                MessageBox.Show("需要导入的字典文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MergeTimer.Enabled = true;
                DisableControl();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            using (DataTable dataTable1 = new DataTable("DTTmp"))
            {
                if (FT == 0)
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s2))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                            {
                                cmd.CommandText = "select org, tra from tbl";
                                ad.Fill(dataTable1);
                            }
                        }
                    }
                    int i1 = dataTable1.Rows.Count;
                    if (i1 > 0)
                    {
                        using (SQLiteConnection MyAccess1 = new SQLiteConnection("Data Source=" + s1))
                        {
                            MyAccess1.Open();
                            using (SQLiteCommand cmd1 = new SQLiteCommand(MyAccess1))
                            {
                                cmd1.Transaction = MyAccess1.BeginTransaction();
                                for (int i = 0; i < i1; i++)
                                {
                                    s1 = dataTable1.Rows[i][0].ToString();
                                    s2 = dataTable1.Rows[i][1].ToString();
                                    s1 = s1.Replace("'", "''");
                                    s2 = s2.Replace("'", "''");
                                    cmd1.CommandText = "Insert Into tbl (org, tra) Values ('" + s1 + "','" + s2 + "')";
                                    try
                                    {
                                        cmd1.ExecuteNonQuery();
                                    }
                                    catch
                                    {
                                        continue;
                                    }
                                }
                                cmd1.Transaction.Commit();
                            }
                        }
                    }
                }
                else if (FT == 2)
                {
                    Encoding ed = mainform.AA_Default_Encoding;
                    ArrayList al = new ArrayList();
                    using (FileStream fs = new FileStream(s2, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader sr = new StreamReader(fs, ed))
                        {
                            string s = "";
                            while ((s = sr.ReadLine()) != null)//判定是否是最后一行
                            {
                                if (CheckSplit(s) == true)
                                {
                                    al.Add(s);
                                }
                            }
                        }
                    }
                    int i1 = al.Count;
                    if (i1 > 0)
                    {
                        using (SQLiteConnection MyAccess1 = new SQLiteConnection("Data Source=" + s1))
                        {
                            MyAccess1.Open();
                            using (SQLiteCommand cmd1 = new SQLiteCommand(MyAccess1))
                            {
                                cmd1.Transaction = MyAccess1.BeginTransaction();
                                for (int i = 0; i < i1; i++)
                                {
                                    string[] str = al[i].ToString().Split('\t');
                                    if (str.Length == 2)
                                    {
                                        s1 = str[0].ToString().Replace("'", "''").Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                        s2 = str[1].ToString().Replace("'", "''").Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                        cmd1.CommandText = "Insert Into tbl (org, tra) Values ('" + s1 + "','" + s2 + "')";
                                        try
                                        {
                                            cmd1.ExecuteNonQuery();
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                                cmd1.Transaction.Commit();
                            }
                        }
                    }
                }
            }
            this.Invoke(new Action(delegate
            {
                MergeTimer.Enabled = false;
                progressBar1.Value = progressBar1.Maximum;
                MessageBox.Show("字典合并完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                progressBar1.Value = 0;
                EnableControl();
            }));
        }

        private void MergeDictionary_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void MergeDictionary_Shown(object sender, EventArgs e)
        {
            label2.Location = new Point(label2.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label2.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            label3.Location = new Point(label3.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label3.Height / 2D));
            button2.Location = new Point(button2.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - button2.Height / 2D));
            label4.Location = new Point(label4.Location.X, progressBar1.Location.Y + (int)(progressBar1.Height / 2D - label4.Height / 2D));
        }

        private void MergeTimer_Tick(object sender, EventArgs e)
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
