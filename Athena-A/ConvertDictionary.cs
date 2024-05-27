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
    public partial class ConvertDictionary : Form
    {
        public ConvertDictionary()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//指定字典文件
        {
            textBox1.Text = CommonCode.Open_Dictionary_File(textBox1.Text);
        }

        private void button2_Click(object sender, EventArgs e)//指定输出的文件
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            save.OverwritePrompt = false;
            if (save.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = save.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void button3_Click(object sender, EventArgs e)//转换字典
        {
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            if (str1 == "")
            {
                MessageBox.Show("请指定需要转换的字典。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (str2 == "")
            {
                MessageBox.Show("请指定输出文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (File.Exists(str1) == false)
            {
                MessageBox.Show("指定的原始字典文件不存在，无法进行转换。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DisableControl();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + textBox1.Text))
            {
                MyAccess.Open();
                using (DataTable dataTable2 = MyAccess.GetSchema("Tables"))
                {
                    ArrayList AL = new ArrayList();
                    int i2 = dataTable2.Rows.Count;
                    for (int i = 0; i < i2; i++)
                    {
                        AL.Add(dataTable2.Rows[i][2].ToString());
                    }
                    if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                    {
                        this.Invoke(new Action(delegate
                        {
                            MessageBox.Show("指定的字典不是由该程序创建的字典文件，因此无法进行导出。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    else
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                            {
                                using (DataTable dataTable1 = new DataTable("DTTmp"))
                                {
                                    cmd.CommandText = "select org, tra from tbl";
                                    ad.Fill(dataTable1);
                                    int i1 = dataTable1.Rows.Count;
                                    if (i1 == 0)
                                    {
                                        this.Invoke(new Action(delegate
                                        {
                                            MessageBox.Show("指定的字典中没有任何内容，导出操作被终止。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }));
                                    }
                                    else
                                    {
                                        int abc = 0;
                                        progressBar1.Maximum = i1;
                                        using (StreamWriter sw = new StreamWriter(textBox2.Text, false))
                                        {
                                            for (int i = 0; i < i1; i++)
                                            {
                                                abc++;
                                                if (abc == 100)
                                                {
                                                    progressBar1.Value = i;
                                                    abc = 0;
                                                }
                                                sw.WriteLine(dataTable1.Rows[i][0].ToString().Replace("\t", "[\\t]").Replace("\r", "[\\r]").Replace("\n", "[\\n]") + "\t" + dataTable1.Rows[i][1].ToString().Replace("\t", "[\\t]").Replace("\r", "[\\r]").Replace("\n", "[\\n]"));
                                            }
                                        }
                                        progressBar1.Value = progressBar1.Maximum;
                                        this.Invoke(new Action(delegate
                                        {
                                            MessageBox.Show("字典转换成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }));
                                        progressBar1.Value = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            EnableControl();
        }

        private void ConvertDictionary_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy == true)
            {
                e.Cancel = true;
            }
        }

        private void ConvertDictionary_Shown(object sender, EventArgs e)
        {
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
            button2.Location = new Point(button2.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - button2.Height / 2D));
            label4.Location = new Point(label4.Location.X, progressBar1.Location.Y + (int)(progressBar1.Height / 2D - label4.Height / 2D));
            if (mainform.MyDpi > 96F)
            {
                label2.Location = new Point(label2.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label2.Height / 2D));
                label3.Location = new Point(label3.Location.X, textBox2.Location.Y + (int)(textBox2.Height / 2D - label3.Height / 2D));
            }
        }
    }
}
