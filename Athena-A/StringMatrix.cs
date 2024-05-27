using System;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Athena_A
{
    public partial class StringMatrix : Form
    {
        public StringMatrix()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            IDataObject cb = Clipboard.GetDataObject();//剪贴板操作
            if (cb.GetDataPresent(DataFormats.Text))//判断剪贴板中的内容是否是文本内容
            {
                string str1 = (string)cb.GetData(DataFormats.Text);
                if (CommonCode.Is_Hex(str1) == false)
                {
                    MessageBox.Show("粘贴板中的数据不是有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    textBox1.Text = str1;
                }
            }
            else
            {
                MessageBox.Show("粘贴板中没有数据，或是其中的数据不是文本内容。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool Compare(long Ltmp, int i2)//比较输入的内容和存在的内容之间的关系
        {
            long l1 = 0;
            long l2 = 0;
            string str1 = "";
            string str2 = "";
            int i3 = dataGridView1.Rows.Count;
            int i4 = 0;
            if (i3 > 0)
            {
                for (int i = 0; i < i3; i++)
                {
                    str1 = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    str2 = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    l1 = CommonCode.HexToLong(str1);
                    l2 = long.Parse(str2);
                    if (Ltmp >= l1 && Ltmp <= l1 + l2)
                    {
                        MessageBox.Show("需要添加的区域地址已包含在地址 " + str1 + " 定义的矩阵区域中。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        i4 = 1;
                        break;
                    }
                    else if (Ltmp < l1 && Ltmp + i2 > l1)
                    {
                        MessageBox.Show("需要添加的区域与地址 " + str1 + " 定义的矩阵区域中存在重叠。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        i4 = 1;
                        break;
                    }
                    else if (Ltmp < l1 + l2 && Ltmp + i2 > l1 + l2)
                    {
                        MessageBox.Show("需要添加的区域与地址 " + str1 + " 定义的矩阵区域中存在重叠。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        i4 = 1;
                        break;
                    }
                }
                if (i4 == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        bool ZoneData(long L1, int i2)
        {
            if (File.Exists(mainform.FilePath) == true)
            {
                FileStream fs = new FileStream(mainform.FilePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                fs.Seek(L1, SeekOrigin.Begin);
                int i3 = 0;
                while (fs.Position < L1 + i2)
                {
                    i3 = br.ReadByte();
                    if (i3 > 0)
                    {
                        break;
                    }
                }
                br.Close();
                fs.Close();
                if (i3 == 0)
                {
                    return true;
                }
                else
                {
                    DialogResult dr = MessageBox.Show("检测到在设定的区域中包含数据，确实还要设置为矩阵区域吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                MessageBox.Show("没有找到创建工程时所使用的文件，无法创建矩阵。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        bool SaveZone(string s1, int i1)
        {
            if (File.Exists(mainform.ProjectFileName) == false)
            {
                MessageBox.Show("没有找到功能工程文件，无法创建矩阵区域。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else
            {
                SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                MyAccess.Open();
                SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                try
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    cmd.CommandText = "Insert Into matrix (mataddress, addlong, freelong) Values ('" + s1 + "'," + i1 + "," + i1 + ")";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    MyAccess.Close();
                    return true;
                }
                catch
                {
                    MyAccess.Close();
                    return false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str1 = textBox1.Text;
            string str2 = textBox2.Text;
            if (str1 == "")
            {
                MessageBox.Show("请输入矩阵区域的起始地址。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (CommonCode.Is_Hex(str1) == false)
            {
                MessageBox.Show("输入的地址不是有效的十六进制值，请重新输入。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (str2 == "")
            {
                MessageBox.Show("请输入矩阵区域的长度。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                int int1 = int.Parse(mainform.FileSize);
                int int2 = int.Parse(str2);
                long L3 = CommonCode.HexToLong(str1);
                if (L3 == 0)
                {
                    MessageBox.Show("矩阵地址不能为零，请重新输入。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (int2 <= 2)
                {
                    MessageBox.Show("区域长度要至少大于 2，请重新输入。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (L3 > int1)
                {
                    MessageBox.Show("输入的地址已超过文件的大小，无法创建矩阵。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if ((L3 + int2) > int1)
                {
                    MessageBox.Show("定义的矩阵区域已超过文件的大小，无法创建矩阵。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (Compare(L3, int2) == true)
                {
                    if ((int2 + L3) > (int.Parse(mainform.l4[mainform.l4.Count - 1].ToString()) + int.Parse(mainform.l3[mainform.l3.Count - 1].ToString())))
                    {
                        MessageBox.Show("定义的矩阵区域已超出可用段外，无法创建矩阵。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (ZoneData(L3, int2) == true)
                    {
                        str1 = CommonCode.FormatStrHex(str1);//格式化十六进制值字符串
                        if (SaveZone(str1, int2) == true)
                        {
                            string[] subItem = { str1, str2, str2 };
                            dataGridView1.Rows.Add(subItem);
                            textBox1.Clear();
                            textBox2.Clear();
                        }
                    }
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string s2 = textBox2.Text;
            if (s2 != "")
            {
                try
                {
                    uint u = uint.Parse(s2);
                }
                catch
                {
                    MessageBox.Show("输入的长度不是有效的整数值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void StringMatrix_Shown(object sender, EventArgs e)
        {
            contextMenuStrip1.Font = this.Font;
            if (mainform.MyDpi < 144F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning32;
            }
            else if (mainform.MyDpi >= 144F && mainform.MyDpi < 192F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning48;
            }
            else if (mainform.MyDpi >= 192F)
            {
                pictureBox1.Image = Athena_A.Properties.Resources.Warning64;
            }
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from matrix";
                        ad.Fill(dataTable1);
                    }
                }
            }
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    dataGridView1.Rows.Add(dataTable1.Rows[i].ItemArray);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("删除矩阵区域将取消区域内所有挪移字符串。\r\n确实要删除此矩阵区域吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.Cancel)
            {
                dataGridView1.AllowUserToDeleteRows = false;
            }
            else
            {
                string s1 = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select * from matrixzone where mataddress = '" + s1 + "'";
                            dataTable2.Clear();
                            ad.Fill(dataTable2);
                            int i1 = dataTable2.Rows.Count;
                            try
                            {
                                cmd.Transaction = MyAccess.BeginTransaction();
                                if (dataGridView1.SelectedRows[0].Index == 0)
                                {
                                    cmd.CommandText = "delete from Resolve32";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "delete from Resolve64";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "delete from ResolveAddress";
                                    cmd.ExecuteNonQuery();
                                }
                                for (int i = 0; i < i1; i++)
                                {
                                    cmd.CommandText = "update athenaa set zonebl = 0 where address = '" + dataTable2.Rows[i][0].ToString() + "'";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "delete from calladd where address = '" + dataTable2.Rows[i][0].ToString() + "'";
                                    cmd.ExecuteNonQuery();
                                }
                                cmd.CommandText = "delete from matrixzone where mataddress = '" + s1 + "'";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "delete from matrix where mataddress = '" + s1 + "'";
                                cmd.ExecuteNonQuery();
                                cmd.Transaction.Commit();
                            }
                            catch (Exception MyEx)
                            {
                                MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                mainform.MatrixDel = true;
                dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.ContextMenuStrip = contextMenuStrip1;
                }
                else
                {
                    dataGridView1.ContextMenuStrip = null;
                }
            }
        }
    }
}
