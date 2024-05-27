using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Athena_A
{
    public partial class CodeQuery : Form
    {
        private bool ANSIHexBL = false;
        private bool UnicodeHexBL = false;
        private bool UTF8HexBL = false;
        private char[] hexTrimChar = { '0' };

        public CodeQuery()
        {
            InitializeComponent();
        }

        void Convert()//编码转换
        {
            string s = textBox1.Text;
            if (s != "")
            {
                if (checkBox1.Checked == false)
                {
                    StringBuilder sb1 = new StringBuilder();
                    string s1 = comboBox1.Text;
                    if (s1 == "英语(1252)" || s1 == "德语(1252)" || s1 == "法语(1252)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(1252).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    else if (s1 == "俄语(1251)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(1251).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    else if (s1 == "韩文(949)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(949).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    else if (s1 == "日语(932)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(932).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    else if (s1 == "简体中文(936)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(936).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    else if (s1 == "繁体中文(950)")
                    {
                        byte[] ANSIbytes = Encoding.GetEncoding(950).GetBytes(s);
                        for (int i = 0; i < ANSIbytes.Length; i++)
                        {
                            sb1 = sb1.Append(ANSIbytes[i].ToString("X2"));
                        }
                        if (ANSIHexBL)
                        {
                            textBox8.Text = ANSIbytes.Length.ToString("X8").TrimStart(hexTrimChar);
                        }
                        else
                        {
                            textBox8.Text = ANSIbytes.Length.ToString();
                        }
                    }
                    textBox2.Text = sb1.ToString();
                    sb1.Clear();
                    byte[] Unicodebytes = Encoding.Unicode.GetBytes(s);
                    for (int i = 0; i < Unicodebytes.Length; i++)
                    {
                        sb1 = sb1.Append(Unicodebytes[i].ToString("X2"));
                    }
                    if (UnicodeHexBL)
                    {
                        textBox9.Text = Unicodebytes.Length.ToString("X8").TrimStart(hexTrimChar);
                    }
                    else
                    {
                        textBox9.Text = Unicodebytes.Length.ToString();
                    }
                    textBox3.Text = sb1.ToString();
                    sb1.Clear();
                    byte[] UTF8bytes = Encoding.UTF8.GetBytes(s);
                    for (int i = 0; i < UTF8bytes.Length; i++)
                    {
                        sb1 = sb1.Append(UTF8bytes[i].ToString("X2"));
                    }
                    if (UTF8HexBL)
                    {
                        textBox10.Text = UTF8bytes.Length.ToString("X8").TrimStart(hexTrimChar);
                    }
                    else
                    {
                        textBox10.Text = UTF8bytes.Length.ToString();
                    }
                    textBox4.Text = sb1.ToString();
                }
                else
                {
                    textBox8.Clear();
                    textBox9.Clear();
                    textBox10.Clear();
                    string patten = "[^a-fA-F0-9]";
                    Regex r = new Regex(patten);
                    Match m = r.Match(s);
                    if (m.Success == false)
                    {
                        int i1 = s.Length;
                        int i2 = 0;
                        if (i1 % 2 == 1)
                        {
                            i1--;
                            s = s.Substring(0, i1);
                        }
                        i1 = i1 / 2;
                        byte[] bt = new byte[i1];
                        for (int i = 0; i < i1; i++)
                        {
                            i2 = System.Int16.Parse(s.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
                            bt[i] = (byte)i2;
                        }
                        textBox3.Text = Encoding.Unicode.GetString(bt);
                        textBox4.Text = Encoding.UTF8.GetString(bt);
                        string s1 = comboBox1.Text;
                        if (s1 == "英语(1252)" || s1 == "德语(1252)" || s1 == "法语(1252)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(1252), Encoding.GetEncoding(1200), bt);
                        }
                        else if (s1 == "俄语(1251)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(1251), Encoding.GetEncoding(1200), bt);
                        }
                        else if (s1 == "韩文(949)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(949), Encoding.GetEncoding(1200), bt);
                        }
                        else if (s1 == "日语(932)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(932), Encoding.GetEncoding(1200), bt);
                        }
                        else if (s1 == "简体中文(936)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(936), Encoding.GetEncoding(1200), bt);
                        }
                        else if (s1 == "繁体中文(950)")
                        {
                            bt = Encoding.Convert(Encoding.GetEncoding(950), Encoding.GetEncoding(1200), bt);
                        }
                        textBox2.Text = Encoding.Unicode.GetString(bt);
                    }
                    else
                    {
                        MessageBox.Show("文本内容不是有效的十六进制值，无法转换。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox2.Clear();
                        textBox3.Clear();
                        textBox4.Clear();
                    }
                }
            }
            else
            {
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox8.Clear();
                textBox9.Clear();
                textBox10.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = Clipboard.GetText();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Convert();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = textBox2.Text;
            if (s != "")
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(s);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = textBox3.Text;
            if (s != "")
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(s);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s = textBox4.Text;
            if (s != "")
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(s);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            switch (checkBox1.Checked)
            {
                case true:
                    button6.Enabled = true;
                    break;
                case false:
                    button6.Enabled = false;
                    break;
            }
            Convert();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string s = textBox1.Text;
            if (s != "")
            {
                textBox1.Text = s.Substring(1, s.Length - 1);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
        }

        string CheckHtm(string s)//检测是否匹配字符串，htm
        {
            string patten = "^*&#\\d{3,5};$*";
            Regex r = new Regex(patten);
            Match m = r.Match(s);
            if (m.Success == true)
            {
                return m.ToString();
            }
            else
            {
                return "no";
            }
        }

        string CheckText(string s)//检测是否匹配字符串，单纯文本
        {
            string patten = "^*#\\d{3,5}$*";
            Regex r = new Regex(patten);
            Match m = r.Match(s);
            if (m.Success == true)
            {
                return m.ToString();
            }
            else
            {
                return "no";
            }
        }

        string ConvertGB(string s)//GB2312 编码转换
        {
            string m;
            if (checkBox2.Checked == false)
            {
                while (CheckText(s) != "no")
                {
                    m = CheckText(s);
                    byte[] bt = System.BitConverter.GetBytes(UInt16.Parse(m.Substring(1, m.Length - 1)));
                    s = s.Replace(m, Encoding.Unicode.GetString(bt));
                }
            }
            else
            {
                while (CheckHtm(s) != "no")
                {
                    m = CheckHtm(s);
                    byte[] bt = System.BitConverter.GetBytes(UInt16.Parse(m.Substring(2, m.Length - 3)));
                    s = s.Replace(m, Encoding.Unicode.GetString(bt));
                }
            }
            return s;
        }

        string ConvertISO(string s)//ISO-8859-1 编码转换
        {
            StringBuilder sb = new StringBuilder();
            string m;
            int tem = s.Length;
            for (int i = 0; i < tem; i++)
            {
                m = s[i].ToString();
                if (mainform.AA_Default_Encoding.GetBytes(m).Length == m.Length)
                {
                    sb.Append(m);
                }
                else
                {
                    if (checkBox2.Checked == false)
                    {
                        sb.Append("#" + System.BitConverter.ToUInt16(Encoding.Unicode.GetBytes(m), 0));
                    }
                    else
                    {
                        sb.Append("&#" + System.BitConverter.ToUInt16(Encoding.Unicode.GetBytes(m), 0) + ";");
                    }
                }
            }
            return sb.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox5.Text = Clipboard.GetText();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string s = textBox5.Text;
            if (s == "")
            {
                textBox6.Clear();
            }
            else
            {
                if (radioButton1.Checked == true)
                {
                    textBox6.Text = ConvertGB(s);
                }
                else
                {
                    textBox6.Text = ConvertISO(s);
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string s = textBox5.Text;
            if (s == "")
            {
                textBox6.Clear();
            }
            else
            {
                if (radioButton1.Checked == true)
                {
                    textBox6.Text = ConvertGB(s);
                }
                else
                {
                    textBox6.Text = ConvertISO(s);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(textBox6.Text);
        }

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            textBox1.Font = fontDialog1.Font;
        }

        private void label2_DoubleClick(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            textBox2.Font = fontDialog1.Font;
        }

        private void label3_DoubleClick(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            textBox3.Font = fontDialog1.Font;
        }

        private void label4_DoubleClick(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            textBox4.Font = fontDialog1.Font;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = this.Font;
            textBox1.Font = fontDialog1.Font;
            textBox2.Font = fontDialog1.Font;
            textBox3.Font = fontDialog1.Font;
            textBox4.Font = fontDialog1.Font;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Convert();
        }

        private void CodeQuery_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                int MoveY = label2.Location.Y - textBox2.Location.Y - (int)(textBox2.Height / 2D - label2.Height / 2D);
                label2.Location = new Point(label2.Location.X, label2.Location.Y - MoveY);
                label3.Location = new Point(label3.Location.X, label3.Location.Y - MoveY);
                label4.Location = new Point(label4.Location.X, label4.Location.Y - MoveY);
                label7.Location = new Point(label7.Location.X, label7.Location.Y - MoveY);
            }
            if (mainform.AA_Default_Encoding.CodePage == 936)
            {
                comboBox1.Text = "简体中文(936)";
            }
            else if (mainform.AA_Default_Encoding.CodePage == 950)
            {
                comboBox1.Text = "繁体中文(950)";
            }
            else
            {
                comboBox1.Text = "英语(1252)";
            }
        }

        private void TextBox8_Click(object sender, EventArgs e)
        {
            string s1 = textBox8.Text;
            if (s1 != "")
            {
                if (ANSIHexBL)
                {
                    ANSIHexBL = false;
                    textBox8.Text = CommonCode.HexToLong(s1).ToString();
                }
                else
                {
                    ANSIHexBL = true;
                    textBox8.Text = Int32.Parse(s1).ToString("X8").TrimStart(hexTrimChar);
                }
            }
        }

        private void TextBox9_Click(object sender, EventArgs e)
        {
            string s1 = textBox9.Text;
            if (s1 != "")
            {
                if (UnicodeHexBL)
                {
                    UnicodeHexBL = false;
                    textBox9.Text = CommonCode.HexToLong(s1).ToString();
                }
                else
                {
                    UnicodeHexBL = true;
                    textBox9.Text = Int32.Parse(s1).ToString("X8").TrimStart(hexTrimChar);
                }
            }
        }

        private void TextBox10_Click(object sender, EventArgs e)
        {
            string s1 = textBox10.Text;
            if (s1 != "")
            {
                if (UTF8HexBL)
                {
                    UTF8HexBL = false;
                    textBox10.Text = CommonCode.HexToLong(s1).ToString();
                }
                else
                {
                    UTF8HexBL = true;
                    textBox10.Text = Int32.Parse(s1).ToString("X8").TrimStart(hexTrimChar);
                }
            }
        }
    }
}
