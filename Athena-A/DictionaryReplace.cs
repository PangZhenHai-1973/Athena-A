using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Athena_A
{
    public partial class DictionaryReplace : Form
    {
        public DictionaryReplace()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s1 = textBox1.Text;
            string s2 = textBox2.Text;
            if (radioButton2.Checked == true)
            {
                bool bl = true;
                if (s1 != "")
                {
                    if (CommonCode.Is_Hex(s1) == false)
                    {
                        MessageBox.Show("查找内容不是一个有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    for (int i = 0; i < s1.Length; i = i + 2)
                    {
                        if (s1.Substring(i, 2) == "00")
                        {
                            bl = false;
                            MessageBox.Show("查找内容中不能包含“00”值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
                if (bl == false)
                {
                    return;
                }
                if (s2 != "")
                {
                    if (CommonCode.Is_Hex(s2) == false)
                    {
                        MessageBox.Show("替换内容不是一个有效的十六进制值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    for (int i = 0; i < s2.Length; i = i + 2)
                    {
                        if (s2.Substring(i, 2) == "00")
                        {
                            bl = false;
                            MessageBox.Show("替换内容中不能包含“00”值。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
                if (bl == false)
                {
                    return;
                }
                if (s1.Length >= 2)
                {
                    int i1 = s1.Length;
                    int x = 0;
                    if (i1 % 2 == 1)
                    {
                        i1--;
                        s1 = s1.Substring(0, i1);
                    }
                    i1 = i1 / 2;
                    byte[] bt1 = new byte[i1];
                    for (int i = 0; i < i1; i++)
                    {
                        x = System.Int16.Parse(s1.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        bt1[i] = (byte)x;
                    }
                    if (radioButton10.Checked == true)
                    {
                        s1 = comboBox1.Text;
                        if (s1 == "英语(1252)" || s1 == "德语(1252)" || s1 == "法语(1252)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.GetEncoding(1200), bt1);
                        }
                        else if (s1 == "俄语(1251)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.GetEncoding(1200), bt1);
                        }
                        else if (s1 == "韩文(949)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(949), Encoding.GetEncoding(1200), bt1);
                        }
                        else if (s1 == "日语(932)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(932), Encoding.GetEncoding(1200), bt1);
                        }
                        else if (s1 == "简体中文(936)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(936), Encoding.GetEncoding(1200), bt1);
                        }
                        else if (s1 == "繁体中文(950)")
                        {
                            bt1 = Encoding.Convert(Encoding.GetEncoding(950), Encoding.GetEncoding(1200), bt1);
                        }
                        s1 = Encoding.Unicode.GetString(bt1);
                    }
                    else if (radioButton9.Checked == true)
                    {
                        s1 = Encoding.Unicode.GetString(bt1);
                    }
                    else
                    {
                        s1 = Encoding.UTF8.GetString(bt1);
                    }
                }
                if (s2.Length >= 2)
                {
                    int i1 = s2.Length;
                    int x = 0;
                    if (i1 % 2 == 1)
                    {
                        i1--;
                        s2 = s2.Substring(0, i1);
                    }
                    i1 = i1 / 2;
                    byte[] bt2 = new byte[i1];
                    for (int i = 0; i < i1; i++)
                    {
                        x = System.Int16.Parse(s2.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                        bt2[i] = (byte)x;
                    }
                    if (radioButton10.Checked == true)
                    {
                        s2 = comboBox2.Text;
                        if (s2 == "英语(1252)" || s2 == "德语(1252)" || s2 == "法语(1252)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.GetEncoding(1200), bt2);
                        }
                        else if (s2 == "俄语(1251)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.GetEncoding(1200), bt2);
                        }
                        else if (s2 == "韩文(949)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(949), Encoding.GetEncoding(1200), bt2);
                        }
                        else if (s2 == "日语(932)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(932), Encoding.GetEncoding(1200), bt2);
                        }
                        else if (s2 == "简体中文(936)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(936), Encoding.GetEncoding(1200), bt2);
                        }
                        else if (s2 == "繁体中文(950)")
                        {
                            bt2 = Encoding.Convert(Encoding.GetEncoding(950), Encoding.GetEncoding(1200), bt2);
                        }
                        s2 = Encoding.Unicode.GetString(bt2);
                    }
                    else if (radioButton9.Checked == true)
                    {
                        s2 = Encoding.Unicode.GetString(bt2);
                    }
                    else
                    {
                        s2 = Encoding.UTF8.GetString(bt2);
                    }
                }
            }
            DictionaryEdit.RStr1 = s1;
            DictionaryEdit.RStr2 = s2;
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                groupBox4.Enabled = false;
                groupBox5.Enabled = false;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                groupBox4.Enabled = true;
                if (radioButton10.Checked == true)
                {
                    groupBox5.Enabled = true;
                }
                else
                {
                    groupBox5.Enabled = false;
                }
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked == true)
            {
                DictionaryEdit.TraBL = 1;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked == true)
            {
                DictionaryEdit.TraBL = 2;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked == true)
            {
                DictionaryEdit.TraBL = 3;
            }
        }

        private void DictionaryReplace_Shown(object sender, EventArgs e)
        {
            comboBox1.Text = "简体中文(936)";
            comboBox2.Text = "简体中文(936)";
            label3.Location = new Point(label3.Location.X, comboBox1.Location.Y + (int)(comboBox1.Height / 2D - label3.Height / 2D));
            label4.Location = new Point(label4.Location.X, comboBox2.Location.Y + (int)(comboBox2.Height / 2D - label4.Height / 2D));
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton10.Checked == true)
            {
                groupBox5.Enabled = true;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked == true)
            {
                groupBox5.Enabled = false;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked == true)
            {
                groupBox5.Enabled = false;
            }
        }
    }
}
