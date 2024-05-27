using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Athena_A
{
    class CommonCode
    {
        static byte[] btstr = new byte[4];

        public static bool Check(string s)//检查是否是该翻译的字符串，正则表达式
        {
            string patten = "[\x01\x02\x03\x04\x05\x06\x07\x08\x0e\x0f\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1a\x1b\x1c\x1d\x1e\x1f]";
            Regex r = new Regex(patten);
            Match m = r.Match(s);
            if (m.Success == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool CheckEnglish(int i)//英语检查可见字符
        {
            if (i >= 32 && i <= 127)
            {
                return true;
            }
            else if (i == 10 || i == 13 || i == 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool Check_Unicode_English(int i)//英语检查可见字符
        {
            if (i >= 32 && i <= 255)
            {
                return true;
            }
            else if (i == 10 || i == 13 || i == 9)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool PE(string s)//判断一个文件是否是标准的 PE 文件
        {
            FileStream fs = new FileStream(s, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            bool bl = false;
            if (fs.Length > 63)
            {
                int i = br.ReadUInt16();
                if (i == 23117)
                {
                    fs.Seek(60, SeekOrigin.Begin);
                    i = (int)br.ReadUInt32();
                    if (i < fs.Length)
                    {
                        fs.Seek(i, SeekOrigin.Begin);
                        i = br.ReadInt32();
                        if (i == 17744)
                        {
                            bl = true;
                        }
                    }
                }
            }
            br.Close();
            fs.Close();
            return bl;
        }

        public static void SetupFolder()//创建配置文件夹
        {
            string s = mainform.CDirectory + "配置";
            if (Directory.Exists(s) == false)
            {
                Directory.CreateDirectory(s);
            }
        }

        public static void DictionaryFolder()//创建默认目录
        {
            string s = mainform.CDirectory + "字典";
            if (Directory.Exists(s) == false)
            {
                Directory.CreateDirectory(s);
            }
            s = mainform.CDirectory + "工程";
            if (Directory.Exists(s) == false)
            {
                Directory.CreateDirectory(s);
            }
        }

        public static bool File_Version_Info(string s1, string s2)//验证程序的版本是否相同，当然，先要验证是否是 PE 文件
        {
            if (FileVersionInfo.GetVersionInfo(s1).FileVersion != FileVersionInfo.GetVersionInfo(s2).FileVersion)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string Open_Exe_File(string s)//打开PE文件夹
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "DLL 文件或 EXE 文件(*.DLL;*.EXE)|*.DLL;*.EXE|所有文件(*.*)|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                return open.FileName;
            }
            else
            {
                return s;
            }
        }

        public static string Open_Dictionary_File(string s)//打开字典文件
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = mainform.CDirectory + "字典";
            open.Filter = "Athena-A 字典文件(*.db)|*.db";
            if (open.ShowDialog() == DialogResult.OK)
            {
                return open.FileName;
            }
            else
            {
                return s;
            }
        }

        public static string FormatStr(string s)//格式化十六进制值字符串
        {
            string str = "";
            int i1 = s.Length;
            if (Math.IEEERemainder(i1, 2) == 0)
            {
                str = System.Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier).ToString("X" + s.Length.ToString());
            }
            else
            {
                str = System.Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier).ToString("X" + (s.Length + 1).ToString());
            }
            i1 = str.Length;
            for (int i = 0; i < i1; i = i + 2)
            {
                if (str.Substring(0, 2) == "00")
                {
                    str = str.Remove(0, 2);
                }
                else
                {
                    break;
                }
            }
            return str;
        }

        public static string FormatStrHex(string s)//格式化十六进制值字符串
        {
            return System.Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier).ToString("X8");
        }

        public static long HexToLong(string s)//十六进制值转换为整数，返回值是 long 值
        {
            bool bl = false;
            long l = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s.Substring(i, 1) != "0")
                {
                    bl = true;
                }
            }
            if (bl)
            {
                l = System.Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return l;
        }

        public static string InvertHex(string s)//反转十六进制值
        {
            string str = "";
            for (int i = s.Length - 2; i >= 0; i = i - 2)
            {
                str += s.Substring(i, 2);
            }
            return str;
        }

        public static bool Is_Hex(string s)//判断一个字符串是否是一个十六进制值字符串
        {
            try
            {
                long val = System.Int64.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                return true;//十六进制值字符串转换为数值，如果出错则不是十六进制值
            }
            catch
            {
                return false;
            }
        }

        public static long GetVirtualAddress(long L)
        {
            int i5 = mainform.l1.Count;//文件头信息总行数
            long L3 = 0;
            long L4 = 0;
            long offset = 0;
            for (int i = 3; i < i5; i++)//所处段
            {
                L3 = long.Parse(mainform.l4[i].ToString());
                L4 = L3 + long.Parse(mainform.l3[i].ToString());
                if (L >= L3 && L < L4)
                {
                    offset = long.Parse(mainform.l2[i].ToString()) - L3 + long.Parse(mainform.l1[0].ToString());//获得偏移量
                    break;
                }
            }
            return offset + L;//获得当前字符串虚拟地址
        }
    }
}
