using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class DictionaryEdit : Form
    {
        string fp = "";
        public static string RStr1 = "";
        public static string RStr2 = "";
        public static int TraBL = 1;

        public DictionaryEdit()
        {
            InitializeComponent();
        }

        private bool OpenDic(string s)//打开字典文件
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s))
            {
                try
                {
                    MyAccess.Open();
                }
                catch
                {
                    MyAccess.Close();
                    MessageBox.Show("打开字典出错。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                DataTable dataTable3 = MyAccess.GetSchema("Tables");
                ArrayList AL = new ArrayList();
                int i3 = dataTable3.Rows.Count;
                for (int i = 0; i < i3; i++)
                {
                    AL.Add(dataTable3.Rows[i][2].ToString());
                }
                if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                {
                    MyAccess.Close();
                    MessageBox.Show("指定的文件不是该程序创建的字典文件，无法打开。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select num, org, tra from tbl";
                            dataTable1.Clear();
                            dataGridView1.DataSource = null;
                            ad.Fill(dataTable1);
                            dataGridView1.DataSource = dataSet1;
                            dataGridView1.DataMember = dataTable1.TableName;
                            cmd.CommandText = "select orgfontname, orgfontsize, trafontname, trafontsize from diclanguage";
                            using (DataTable dataTable4 = new DataTable("DTTmp"))
                            {
                                ad.Fill(dataTable4);
                                if (dataTable4.Rows.Count == 1)
                                {
                                    DictionaryFont.DictionaryOrgName = dataTable4.Rows[0][0].ToString();
                                    DictionaryFont.DictionaryOrgSize = dataTable4.Rows[0][1].ToString();
                                    DictionaryFont.DictionaryTraName = dataTable4.Rows[0][2].ToString();
                                    DictionaryFont.DictionaryTraSize = dataTable4.Rows[0][3].ToString();
                                    if (DictionaryFont.DictionaryOrgName != "")
                                    {
                                        Font temfont1 = new Font(DictionaryFont.DictionaryOrgName, float.Parse(DictionaryFont.DictionaryOrgSize));
                                        orgDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont1;
                                        textBox1.Font = temfont1;
                                        if (toolStripComboBox2.Text == "原文")
                                        {
                                            toolStripTextBox2.Font = temfont1;
                                        }
                                    }
                                    else
                                    {
                                        DictionaryFont.DictionaryOrgSize = "";
                                    }
                                    if (DictionaryFont.DictionaryTraName != "")
                                    {
                                        Font temfont2 = new Font(DictionaryFont.DictionaryTraName, float.Parse(DictionaryFont.DictionaryTraSize));
                                        traDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont2;
                                        textBox2.Font = temfont2;
                                        if (toolStripComboBox2.Text == "译文")
                                        {
                                            toolStripTextBox2.Font = temfont2;
                                        }
                                    }
                                    else
                                    {
                                        DictionaryFont.DictionaryTraSize = "";
                                    }
                                }
                            }
                        }
                    }
                }
                if (dataTable1.Rows.Count == 0)
                {
                    MessageBox.Show("字典中没有任何内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void 水平滚动RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (水平滚动RToolStripMenuItem.Checked == false)
            {
                水平滚动RToolStripMenuItem.Checked = true;
            }
            else
            {
                水平滚动RToolStripMenuItem.Checked = false;
            }
            if (水平滚动RToolStripMenuItem.Checked == true)
            {
                if (textBox1.ScrollBars == ScrollBars.None)
                {
                    textBox1.ScrollBars = ScrollBars.Horizontal;
                    textBox2.ScrollBars = ScrollBars.Horizontal;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.Both;
                    textBox2.ScrollBars = ScrollBars.Both;
                }
            }
            else
            {
                if (textBox1.ScrollBars == ScrollBars.Both)
                {
                    textBox1.ScrollBars = ScrollBars.Vertical;
                    textBox2.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.None;
                    textBox2.ScrollBars = ScrollBars.None;
                }
            }
        }

        private void 垂直滚动RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (垂直滚动RToolStripMenuItem.Checked == false)
            {
                垂直滚动RToolStripMenuItem.Checked = true;
            }
            else
            {
                垂直滚动RToolStripMenuItem.Checked = false;
            }
            if (垂直滚动RToolStripMenuItem.Checked == true)
            {
                if (textBox1.ScrollBars == ScrollBars.None)
                {
                    textBox1.ScrollBars = ScrollBars.Vertical;
                    textBox2.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.Both;
                    textBox2.ScrollBars = ScrollBars.Both;
                }
            }
            else
            {
                if (textBox1.ScrollBars == ScrollBars.Both)
                {
                    textBox1.ScrollBars = ScrollBars.Horizontal;
                    textBox2.ScrollBars = ScrollBars.Horizontal;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.None;
                    textBox2.ScrollBars = ScrollBars.None;
                }
            }
        }

        private void DisabledPanelContrl()
        {
            panel1.Enabled = false;
            panel2.Enabled = false;
        }

        private void EnablePanelContrl()
        {
            panel1.Enabled = true;
            panel2.Enabled = true;
        }

        private void TextClear()
        {
            textBox1.Clear();
            textBox2.Clear();
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s1 = CommonCode.Open_Dictionary_File(fp);
            if (s1 != "")
            {
                fp = s1;
                TextClear();
                if (OpenDic(fp) == true)
                {
                    EnablePanelContrl();
                    RowEnterDis(0);
                    dataGridView1.Enabled = true;
                    this.Text = "编辑字典 - " + fp;
                }
                else
                {
                    DisabledPanelContrl();
                    dataGridView1.Enabled = false;
                    dataTable1.Clear();
                    this.Text = "编辑字典";
                    fp = "";
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Clear();
            toolStripComboBox1.Text = "小于";
            toolStripTextBox2.Clear();
            toolStripComboBox2.Text = "原文";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (File.Exists(fp) == false)
            {
                MessageBox.Show(fp + "\r\n不存在，无法查找。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string str1 = toolStripTextBox1.Text;
                string str2 = toolStripTextBox2.Text;
                uint u1 = 0;
                if (str1 != "")
                {
                    try
                    {
                        u1 = uint.Parse(str1);
                    }
                    catch
                    {
                        MessageBox.Show("输入的字符长度不是有效的整数值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                string com1 = toolStripComboBox1.Text;
                string com2 = toolStripComboBox2.Text;
                if (str2 != "")
                {
                    if (toolStripComboBox3.Items.Contains(str2) == false)
                    {
                        toolStripComboBox3.Items.Add(str2);
                    }
                }
                if (str2 == "换行符 \\n 转义")
                {
                    str2 = "\n";
                }
                else if (str2 == "回车符 \\r 转义")
                {
                    str2 = "\r";
                }
                else if (str2 == "制表符 \\t 转义")
                {
                    str2 = "\t";
                }
                else
                {
                    str2 = str2.Replace("/", "//");
                    str2 = str2.Replace("'", "''");
                    str2 = str2.Replace("%", "/%");
                    str2 = str2.Replace("_", "/_");
                }
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        if (str2 == "")
                        {
                            if (str1 == "")
                            {
                                if (com2 == "相同")
                                {
                                    cmd.CommandText = "select num, org, tra from tbl where org = tra";
                                }
                                else
                                {
                                    cmd.CommandText = "select num, org, tra from tbl";
                                }
                            }
                            else
                            {
                                if (com1 == "小于")
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) < " + u1;
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) < " + u1;
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) < " + u1;
                                    }
                                }
                                else if (com1 == "等于")
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) = " + u1;
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) = " + u1;
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) = " + u1;
                                    }
                                }
                                else
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) > " + u1;
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) > " + u1;
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) > " + u1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (str1 == "")
                            {
                                if (com2 == "原文")
                                {
                                    cmd.CommandText = "select num, org, tra from tbl where org Like '%" + str2 + "%' ESCAPE '/'";
                                }
                                else if (com2 == "译文")
                                {
                                    cmd.CommandText = "select num, org, tra from tbl where tra Like '%" + str2 + "%' ESCAPE '/'";
                                }
                                else
                                {
                                    cmd.CommandText = "select num, org, tra from tbl where org = tra and org Like '%" + str2 + "%' ESCAPE '/'";
                                }
                            }
                            else
                            {
                                if (com1 == "小于")
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) < " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) < " + u1 + " and tra Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) < " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                }
                                else if (com1 == "等于")
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) = " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) = " + u1 + " and tra Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) = " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                }
                                else
                                {
                                    if (com2 == "原文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(org) > " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else if (com2 == "译文")
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where length(tra) > " + u1 + " and tra Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                    else
                                    {
                                        cmd.CommandText = "select num, org, tra from tbl where org = tra and length(org) > " + u1 + " and org Like '%" + str2 + "%' ESCAPE '/'";
                                    }
                                }
                            }
                        }
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            ad.SelectCommand = cmd;
                            dataTable1.Clear();
                            ad.Fill(dataTable1);
                        }
                        if (dataTable1.Rows.Count == 0)
                        {
                            TextClear();
                        }
                        else
                        {
                            RowEnterDis(0);
                        }
                    }
                }
            }
        }

        private void RowEnterDis(int i1)
        {
            string s1 = dataTable1.Rows[i1][1].ToString();
            string s2 = dataTable1.Rows[i1][2].ToString();
            if (s1.Contains("\r") == true && s1.Contains("\n") == false)
            {
                s1 = s1.Replace("\r", "\r\n");
                s2 = s2.Replace("\r", "\r\n");
            }
            else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
            {
                s1 = s1.Replace("\n", "\r\n");
                s2 = s2.Replace("\n", "\r\n");
            }
            textBox1.Text = s1;
            textBox2.Text = s2;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            int MyKeyValue = e.KeyValue;
            if (MyKeyValue == 46)
            {
                if (dataTable1.Rows.Count > 0)
                {
                    DialogResult dr = MessageBox.Show("确实要删除选定的内容吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        DeleteConcurrentDictionary = new ConcurrentDictionary<int, int>();
                        int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                        int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                DeleteConcurrentDictionary.TryAdd(intFirstRow, (int)dataTable1.Rows[intFirstRow][0]);
                            }
                        }
                        dataGridView1.DataSource = null;
                        DisabledPanelContrl();
                        menuStrip1.Enabled = false;
                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
            else if (MyKeyValue == 38)//向上键
            {
                if (dataTable1.Rows.Count > 0)
                {
                    int i1 = dataGridView1.CurrentRow.Index;
                    if (i1 > 0)
                    {
                        RowEnterDis(i1 - 1);
                    }
                }
            }
            else if (MyKeyValue == 40)//向下键
            {
                if (dataTable1.Rows.Count > 0)
                {
                    int i1 = dataGridView1.CurrentRow.Index;
                    if (i1 < dataTable1.Rows.Count - 1)
                    {
                        RowEnterDis(i1 + 1);
                    }
                }
            }
        }

        ConcurrentDictionary<int, int> DeleteConcurrentDictionary;

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int i1 = DeleteConcurrentDictionary.Count;
            Parallel.Invoke(() =>
            {
                int[] ik = new int[i1];
                DeleteConcurrentDictionary.Keys.CopyTo(ik, 0);
                Array.Sort(ik);
                this.Invoke(new Action(delegate
                {
                    for (int i = i1 - 1; i >= 0; i--)
                    {
                        dataTable1.Rows.RemoveAt(ik[i]);
                    }
                }));
            },
            () =>
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        int m = 0;
                        StringBuilder sb = new StringBuilder();
                        int[] iy = new int[i1];
                        DeleteConcurrentDictionary.Values.CopyTo(iy, 0);
                        cmd.Transaction = MyAccess.BeginTransaction();
                        for (int i = 0; i < i1; i++)
                        {
                            if (m == 0)
                            {
                                sb.Clear();
                                sb.Append("delete from tbl where num = " + iy[i].ToString());
                            }
                            else
                            {
                                sb.Append(" or num = " + iy[i].ToString());
                            }
                            m++;
                            if (m == 100)
                            {
                                cmd.CommandText = sb.ToString();
                                cmd.ExecuteNonQuery();
                                m = 0;
                            }
                        }
                        if (m > 0)
                        {
                            cmd.CommandText = sb.ToString();
                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                    }
                }
            });
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            DeleteConcurrentDictionary = null;
            if (dataTable1.Rows.Count > 0)
            {
                RowEnterDis(dataGridView1.CurrentRow.Index);
            }
            menuStrip1.Enabled = true;
            EnablePanelContrl();
        }

        private void 替换RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataTable1.Rows.Count > 0)
            {
                if (File.Exists(fp) == false)
                {
                    MessageBox.Show("找不到字典文件，请检查字典文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    RStr1 = "";
                    RStr2 = "";
                    DictionaryReplace DR = new DictionaryReplace();
                    if (mainform.MyDpi > 96F)
                    {
                        DR.Font = mainform.MyNewFont;
                    }
                    DR.ShowDialog();
                    if (RStr1 != "" && RStr1 != RStr2)
                    {
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                                string s1 = "";
                                string s2 = "";
                                if (TraBL == 1)
                                {
                                    textBox2.Text = textBox2.Text.Replace(RStr1, RStr2);
                                    for (; intFirstRow <= intLastRow; intFirstRow++)
                                    {
                                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                        {
                                            s2 = dataTable1.Rows[intFirstRow][2].ToString().Replace(RStr1, RStr2);
                                            dataTable1.Rows[intFirstRow][2] = s2;
                                            s2 = s2.Replace("'", "''");
                                            cmd.CommandText = "update tbl set tra = '" + s2 + "' where num = " + (int)dataTable1.Rows[intFirstRow][0];
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                else if (TraBL == 2)
                                {
                                    textBox1.Text = textBox1.Text.Replace(RStr1, RStr2);
                                    for (; intFirstRow <= intLastRow; intFirstRow++)
                                    {
                                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                        {
                                            s1 = dataTable1.Rows[intFirstRow][1].ToString().Replace(RStr1, RStr2);
                                            dataTable1.Rows[intFirstRow][1] = s1;
                                            s1 = s1.Replace("'", "''");
                                            cmd.CommandText = "update tbl set org = '" + s1 + "' where num = " + (int)dataTable1.Rows[intFirstRow][0];
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                else
                                {
                                    textBox1.Text = textBox1.Text.Replace(RStr1, RStr2);
                                    textBox2.Text = textBox2.Text.Replace(RStr1, RStr2);
                                    for (; intFirstRow <= intLastRow; intFirstRow++)
                                    {
                                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                        {
                                            s1 = dataTable1.Rows[intFirstRow][1].ToString().Replace(RStr1, RStr2);
                                            s2 = dataTable1.Rows[intFirstRow][2].ToString().Replace(RStr1, RStr2);
                                            dataTable1.Rows[intFirstRow][1] = s1;
                                            dataTable1.Rows[intFirstRow][2] = s2;
                                            s1 = s1.Replace("'", "''");
                                            s2 = s2.Replace("'", "''");
                                            cmd.CommandText = "update tbl set org = '" + s1 + "',tra ='" + s2 + "' where num = " + (int)dataTable1.Rows[intFirstRow][0];
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void 添加AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(fp) == false)
            {
                MessageBox.Show("找不到字典文件，请检查字典文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                RStr1 = "";
                RStr2 = "";
                DictionaryAdd DA = new DictionaryAdd();
                if (mainform.MyDpi > 96F)
                {
                    DA.Font = mainform.MyNewFont;
                }
                DA.ShowDialog();
                if (RStr1 != "" && RStr2 != "")
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            string s1 = RStr1;
                            string s2 = RStr2;
                            RStr1 = RStr1.Replace("'", "''");
                            RStr2 = RStr2.Replace("'", "''");
                            cmd.CommandText = "Insert Into tbl(org, tra) Values ('" + RStr1 + "','" + RStr2 + "')";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "select seq from sqlite_sequence where name = 'tbl'";
                            long L1 = long.Parse(cmd.ExecuteScalar().ToString());
                            object[] ob = new object[3];
                            ob[0] = L1;
                            ob[1] = s1;
                            ob[2] = s2;
                            dataTable1.Rows.Add(ob);
                        }
                    }
                }
            }
        }

        private void SaveEdit()
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                string s1 = textBox1.Text;
                string s2 = textBox2.Text;
                if (s2 != "")
                {
                    int ix = dataGridView1.CurrentRow.Index;
                    int i1 = (int)dataTable1.Rows[ix][0];
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            if (s1.Contains("\r") == true && s1.Contains("\n") == false)
                            {
                                s2 = s2.Replace("\r\n", "\r");
                            }
                            else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
                            {
                                s2 = s2.Replace("\r\n", "\n");
                            }
                            dataTable1.Rows[ix][2] = s2;
                            s2 = s2.Replace("'", "''");
                            cmd.CommandText = "update tbl set tra = '" + s2 + "' where num = " + i1;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SaveEdit();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (dataTable1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1[1, 0];
                dataGridView1.CurrentCell.Selected = true;
                textBox1.Text = dataTable1.Rows[0][1].ToString();
                textBox2.Text = dataTable1.Rows[0][2].ToString();
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            int i = dataTable1.Rows.Count - 1;
            if (i >= 0)
            {
                dataGridView1.CurrentCell = dataGridView1[1, i];
                dataGridView1.CurrentCell.Selected = true;
                textBox1.Text = dataTable1.Rows[i][1].ToString();
                textBox2.Text = dataTable1.Rows[i][2].ToString();
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (File.Exists(fp) == false)
            {
                MessageBox.Show("找不到字典文件，请检查字典文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    int i1 = dataGridView1.CurrentRow.Index;
                    int i2 = (int)dataTable1.Rows[i1][0];
                    string org = dataTable1.Rows[i1][1].ToString();
                    string tra = dataTable1.Rows[i1][2].ToString();
                    if (org != "" && tra != "")
                    {
                        if (org.Substring(0, 1) == tra.Substring(0, 1) && org.Length > 1 && tra.Length > 1)
                        {
                            org = org.Substring(1, org.Length - 1).Replace("'", "''");
                            tra = tra.Substring(1, tra.Length - 1).Replace("'", "''");
                            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                            {
                                MyAccess.Open();
                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                {
                                    cmd.CommandText = "update tbl set org ='" + org + "', tra ='" + tra + "' where num =" + i2;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            org = org.Replace("''", "'");
                            tra = tra.Replace("''", "'");
                            dataTable1.Rows[i1][1] = org;
                            dataTable1.Rows[i1][2] = tra;
                            RowEnterDis(i1);
                        }
                    }
                }
            }
        }

        private void DictionaryEdit_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    if (Path.GetExtension(s1).ToLower() == ".db")
                    {
                        Clipboard.SetDataObject(filenames[0]);
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void DictionaryEdit_DragDrop(object sender, DragEventArgs e)
        {
            string s1 = (string)Clipboard.GetData(DataFormats.Text);
            if (s1 != "")
            {
                fp = s1;
                TextClear();
                if (OpenDic(fp) == true)
                {
                    EnablePanelContrl();
                    RowEnterDis(0);
                    dataGridView1.Enabled = true;
                    this.Text = "编辑字典 - " + fp;
                }
                else
                {
                    DisabledPanelContrl();
                    dataGridView1.Enabled = false;
                    dataTable1.Clear();
                    this.Text = "编辑字典";
                    fp = "";
                }
            }
        }

        private void 关闭XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 字体FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (orgDataGridViewTextBoxColumn.DefaultCellStyle.Font != null)
            {
                DictionaryFont.DictionaryOrgName = orgDataGridViewTextBoxColumn.DefaultCellStyle.Font.Name;
                DictionaryFont.DictionaryOrgSize = orgDataGridViewTextBoxColumn.DefaultCellStyle.Font.Size.ToString();
            }
            else
            {
                DictionaryFont.DictionaryOrgName = dataGridView1.DefaultCellStyle.Font.Name;
                DictionaryFont.DictionaryOrgSize = dataGridView1.DefaultCellStyle.Font.Size.ToString();
            }
            if (traDataGridViewTextBoxColumn.DefaultCellStyle.Font != null)
            {
                DictionaryFont.DictionaryTraName = traDataGridViewTextBoxColumn.DefaultCellStyle.Font.Name;
                DictionaryFont.DictionaryTraSize = traDataGridViewTextBoxColumn.DefaultCellStyle.Font.Size.ToString();
            }
            else
            {
                DictionaryFont.DictionaryTraName = dataGridView1.DefaultCellStyle.Font.Name;
                DictionaryFont.DictionaryTraSize = dataGridView1.DefaultCellStyle.Font.Size.ToString();
            }
            DictionaryFont DF = new DictionaryFont();
            if (mainform.MyDpi > 96F)
            {
                DF.Font = mainform.MyNewFont;
            }
            DF.ShowDialog();
            Font OrgFont = new Font(DictionaryFont.DictionaryOrgName, float.Parse(DictionaryFont.DictionaryOrgSize));
            orgDataGridViewTextBoxColumn.DefaultCellStyle.Font = OrgFont;
            textBox1.Font = OrgFont;
            Font TraFont = new Font(DictionaryFont.DictionaryTraName, float.Parse(DictionaryFont.DictionaryTraSize));
            traDataGridViewTextBoxColumn.DefaultCellStyle.Font = TraFont;
            textBox2.Font = TraFont;
            if (toolStripComboBox2.Text == "原文")
            {
                toolStripTextBox2.Font = OrgFont;
            }
            else if (toolStripComboBox2.Text == "译文")
            {
                toolStripTextBox2.Font = TraFont;
            }
            if (File.Exists(fp) == true)
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + fp))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        if (DictionaryFont.DictionaryOrgName == "" && DictionaryFont.DictionaryTraName == "")
                        {
                            cmd.CommandText = "update diclanguage set orgfontname ='', orgfontsize =" + 0 + ", trafontname ='', trafontsize =" + 0;
                        }
                        else if (DictionaryFont.DictionaryOrgName != "" && DictionaryFont.DictionaryTraName == "")
                        {
                            cmd.CommandText = "update diclanguage set orgfontname ='" + DictionaryFont.DictionaryOrgName + "', orgfontsize =" + float.Parse(DictionaryFont.DictionaryOrgSize) + ", trafontname ='', trafontsize =" + 0;
                        }
                        else if (DictionaryFont.DictionaryOrgName == "" && DictionaryFont.DictionaryTraName != "")
                        {
                            cmd.CommandText = "update diclanguage set orgfontname ='', orgfontsize =" + 0 + ", trafontname ='" + DictionaryFont.DictionaryTraName + "', trafontsize =" + float.Parse(DictionaryFont.DictionaryTraSize);
                        }
                        else
                        {
                            cmd.CommandText = "update diclanguage set orgfontname ='" + DictionaryFont.DictionaryOrgName + "', orgfontsize =" + float.Parse(DictionaryFont.DictionaryOrgSize) + ", trafontname ='" + DictionaryFont.DictionaryTraName + "', trafontsize =" + float.Parse(DictionaryFont.DictionaryTraSize);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s1 = toolStripComboBox2.Text;
            if (s1 == "原文")
            {
                toolStripTextBox2.Font = textBox1.Font;
            }
            else if (s1 == "译文")
            {
                toolStripTextBox2.Font = textBox2.Font;
            }
        }

        private void DictionaryEdit_Shown(object sender, EventArgs e)
        {
            menuStrip1.Font = this.Font;
            toolStripComboBox1.Text = "小于";
            toolStripComboBox2.Text = "原文";
            DE_SearchHistoryBL = false;
            toolStripComboBox3.Text = "清除历史";
            DE_SearchHistoryBL = true;
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveEdit();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 40)
            {
                int i = dataTable1.Rows.Count;
                if (i > 0)
                {
                    SaveEdit();
                    int x = dataGridView1.CurrentCell.RowIndex + 1;
                    if (x < i)
                    {
                        dataGridView1.CurrentCell = dataGridView1[1, x];
                        textBox1.Text = dataTable1.Rows[x][1].ToString();
                        textBox2.Text = dataTable1.Rows[x][2].ToString();
                    }
                }
            }
            else if (e.KeyValue == 38)
            {
                int i = dataTable1.Rows.Count;
                if (i > 0)
                {
                    SaveEdit();
                    int x = dataGridView1.CurrentCell.RowIndex - 1;
                    if (x >= 0)
                    {
                        dataGridView1.CurrentCell = dataGridView1[1, x];
                        textBox1.Text = dataTable1.Rows[x][1].ToString();
                        textBox2.Text = dataTable1.Rows[x][2].ToString();
                    }
                }
            }
        }

        private void DictionaryEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataTable1.Rows.Clear();
            dataSet1.Dispose();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                RowEnterDis(dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected));
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(textBox1.Text);
        }

        private bool DE_SearchHistoryBL = true;

        private void toolStripComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DE_SearchHistoryBL)
            {
                DE_SearchHistoryBL = false;
                string s1 = toolStripComboBox3.Text;
                if (s1 == "清除历史")
                {
                    int i1 = toolStripComboBox3.Items.Count - 1;
                    for (int i = i1; i > 3; i--)
                    {
                        toolStripComboBox3.Items.RemoveAt(i);
                    }
                }
                else
                {
                    toolStripTextBox2.Text = s1;
                }
                DE_SearchHistoryBL = true;
            }
        }
    }
}
