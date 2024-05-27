using System;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class Correct : Form
    {
        public Correct()
        {
            InitializeComponent();
        }

        private void listBox1_Click(object sender, EventArgs e)//列表框 1 单击一行事件
        {
            int i1 = listBox1.SelectedIndex;
            if (i1 >= 0)
            {
                toolTip1.RemoveAll();
                toolTip1.SetToolTip(listBox1, listBox1.Items[i1].ToString());
            }
            listBox2.ClearSelected();
            listBox3.ClearSelected();
            listBox4.ClearSelected();
        }

        private void listBox2_Click(object sender, EventArgs e)//列表框 2 单击一行事件
        {
            int i2 = listBox2.SelectedIndex;
            if (i2 >= 0)
            {
                toolTip1.RemoveAll();
                toolTip1.SetToolTip(listBox2, listBox2.Items[i2].ToString());
            }
            listBox1.ClearSelected();
            listBox3.ClearSelected();
            listBox4.ClearSelected();
        }

        private void listBox3_Click(object sender, EventArgs e)//列表框 3 单击一行事件
        {
            int i3 = listBox3.SelectedIndex;
            if (i3 >= 0)
            {
                toolTip1.RemoveAll();
                toolTip1.SetToolTip(listBox3, listBox3.Items[i3].ToString());
            }
            listBox1.ClearSelected();
            listBox2.ClearSelected();
            listBox4.ClearSelected();
        }

        private void listBox4_Click(object sender, EventArgs e)//列表框 4 单击一行事件
        {
            int i4 = listBox4.SelectedIndex;
            if (i4 >= 0)
            {
                toolTip1.RemoveAll();
                toolTip1.SetToolTip(listBox4, listBox4.Items[i4].ToString());
            }
            listBox1.ClearSelected();
            listBox2.ClearSelected();
            listBox3.ClearSelected();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)//转移原文字符串到下面的列表框
        {
            int en = listBox1.SelectedIndex;
            if (en >= 0)
            {
                int i3 = listBox3.Items.Count;
                int i4 = listBox4.Items.Count;
                string s = listBox1.Items[en].ToString();
                if (i3 - i4 <= 0)
                {
                    listBox3.Items.Add(s);
                    listBox1.Items.RemoveAt(en);
                    toolTip1.RemoveAll();
                }
                else
                {
                    MessageBox.Show("请先添加相应的译文翻译。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)//转移译文字符串到下面的列表框
        {
            int chs = listBox2.SelectedIndex;
            if (chs >= 0)
            {
                int i3 = listBox3.Items.Count;
                int i4 = listBox4.Items.Count;
                string s = listBox2.Items[chs].ToString();
                if (i4 - i3 <= 0)
                {
                    listBox4.Items.Add(s);
                    listBox2.Items.RemoveAt(chs);
                    toolTip1.RemoveAll();
                }
                else
                {
                    MessageBox.Show("请先添加原文字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void listBox3_DoubleClick(object sender, EventArgs e)//移除一行
        {
            int i = listBox3.SelectedIndex;
            int i3 = listBox3.Items.Count;
            int i4 = listBox4.Items.Count;
            if (i >= 0)
            {
                if (i == i4)
                {
                    listBox1.Items.Add(listBox3.Items[i].ToString());
                    listBox3.Items.RemoveAt(i);
                }
                else
                {
                    listBox1.Items.Add(listBox3.Items[i].ToString());
                    listBox2.Items.Add(listBox4.Items[i].ToString());
                    listBox3.Items.RemoveAt(i);
                    listBox4.Items.RemoveAt(i);
                }
                toolTip1.RemoveAll();
            }
        }

        private void listBox4_DoubleClick(object sender, EventArgs e)//移除一行
        {
            int i = listBox4.SelectedIndex;
            int i3 = listBox3.Items.Count;
            int i4 = listBox4.Items.Count;
            if (i >= 0)
            {
                if (i == i3)
                {
                    listBox2.Items.Add(listBox4.Items[i].ToString());
                    listBox4.Items.RemoveAt(i);
                }
                else
                {
                    listBox1.Items.Add(listBox3.Items[i].ToString());
                    listBox2.Items.Add(listBox4.Items[i].ToString());
                    listBox3.Items.RemoveAt(i);
                    listBox4.Items.RemoveAt(i);
                }
                toolTip1.RemoveAll();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int i3 = listBox3.Items.Count;
            int i4 = listBox4.Items.Count;
            if (File.Exists(DictionaryExtract.Dictionaryname) == false)
            {
                MessageBox.Show("字典文件不存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (i3 == 0 && i4 == 0)
            {
                MessageBox.Show("请先添加需要保存的内容。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else if (i3 != i4)
            {
                MessageBox.Show("两侧内容没有一一对应，请检查此错误。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + DictionaryExtract.Dictionaryname);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                string str1 = "";
                string str2 = "";
                cmd.Transaction = MyAccess.BeginTransaction();
                for (int i = 0; i < i3; i++)
                {
                    str1 = listBox3.Items[i].ToString();
                    str1 = str1.Replace("'", "''");
                    str2 = listBox4.Items[i].ToString();
                    str2 = str2.Replace("'", "''");
                    cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + str1 + "', '" + str2 + "')";
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
                MyAccess.Close();
                listBox3.Items.Clear();
                listBox4.Items.Clear();
                MessageBox.Show("指定内容已保存成功！建议在全部保存完毕之后打开字典编辑器对字典进行编辑处理。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)//添加原文字符串
        {
            string str1 = textBox1.Text;
            if (str1 == "")
            {
                MessageBox.Show("请输入原文内容后再添加。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int i1 = 0;
                int i3 = listBox3.Items.Count;
                int i4 = listBox4.Items.Count;
                for (int i = 0; i < i3; i++)
                {
                    if (str1 == listBox3.Items[i].ToString())
                    {
                        i1 = 1;
                    }
                }
                if (i1 == 1)
                {
                    MessageBox.Show("不能重复添加相同的原文字符串。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (i3 - i4 <= 0)
                    {
                        listBox3.Items.Add(str1);
                    }
                    else
                    {
                        MessageBox.Show("请先添加对应的译文翻译。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//添加译文字符串
        {
            string str1 = textBox2.Text;
            if (str1 == "")
            {
                MessageBox.Show("请输入译文内容后再添加。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int i1 = 0;
                int i3 = listBox3.Items.Count;
                int i4 = listBox4.Items.Count;
                for (int i = 0; i < i4; i++)
                {
                    if (str1 == listBox4.Items[i].ToString())
                    {
                        i1 = 1;
                    }
                }
                if (i1 == 1)
                {
                    MessageBox.Show("不能重复添加相同的字符串。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    if (i4 - i3 <= 0)
                    {
                        listBox4.Items.Add(str1);
                    }
                    else
                    {
                        MessageBox.Show("请先添加对应的原文内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox1.SelectedItems.Count;
                if (i1 > 0)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                    }
                }
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
            {
                int i1 = listBox2.SelectedItems.Count;
                if (i1 > 0)
                {
                    for (int i = 0; i < i1; i++)
                    {
                        listBox2.Items.RemoveAt(listBox2.SelectedIndex);
                    }
                }
            }
        }
    }
}
