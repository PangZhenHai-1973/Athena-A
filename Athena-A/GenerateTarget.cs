using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;

namespace Athena_A
{
    public partial class GenerateTarget : Form
    {

        bool BLC = false;

        public GenerateTarget()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//设定保存目录
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.Description = "请选择要保存目标文件的文件夹。";
            if (fb.ShowDialog() == DialogResult.OK)
            {
                string s = fb.SelectedPath;
                string tem = s.Substring(0, 3);
                DriveInfo di = new DriveInfo(tem);
                if (di.DriveType.ToString() == "CDRom")
                {
                    MessageBox.Show("无法在光驱中保存文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (s.Length == 3)
                    {
                        textBox1.Text = fb.SelectedPath + Path.GetFileName(mainform.FilePath);
                    }
                    else
                    {
                        textBox1.Text = fb.SelectedPath + "\\" + Path.GetFileName(mainform.FilePath);
                    }
                }
            }
        }

        private void UnPeW1(string s1, string s2)//非PE文件写入方式 1
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true)
                    {
                        continue;
                    }
                    else
                    {
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        bw.Write((byte)0);
                        fsorg.Seek(fsorg.Position + (int)tem[3] + 1, SeekOrigin.Begin);
                    }
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                br.Close();
                bw.Close();
                fsorg.Close();
                fstra.Close();
                BLC = true;
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void UnPeW2(string s1, string s2)//非PE文件写入方式 2
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true)
                    {
                        continue;
                    }
                    else
                    {
                        fstra.Seek(-4, SeekOrigin.Current);
                        bw.Write((int)tem[4] + 1);
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        bw.Write((byte)0);
                        fsorg.Seek(fsorg.Position + (int)tem[3] + 1, SeekOrigin.Begin);
                    }
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                br.Close();
                bw.Close();
                fsorg.Close();
                fstra.Close();
                BLC = true;
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void UnPeW3(string s1, string s2)//非PE文件写入方式 3
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true)
                    {
                        continue;
                    }
                    else if ((int)tem[7] < 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    else
                    {
                        for (int x = 0; x < (int)tem[3]; x++)
                        {
                            bw.Write((byte)0);
                        }
                        fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                    }
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                    }
                    else if ((int)tem[4] > 0 && (bool)tem[12] == true)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                    }
                }
                br.Close();
                bw.Close();
                fsorg.Close();
                fstra.Close();
                BLC = true;
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void UnPeW4(string s1, string s2)//非PE文件写入方式 4
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true)
                    {
                        continue;
                    }
                    else if ((int)tem[7] < 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    else
                    {
                        for (int x = 0; x < (int)tem[3]; x++)
                        {
                            bw.Write((byte)0);
                        }
                        fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                    }
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                    }
                    else if ((int)tem[4] > 0 && (bool)tem[12] == true)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                    }
                }
                br.Close();
                bw.Close();
                fsorg.Close();
                fstra.Close();
                BLC = true;
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void Skyrim(string s1, string s2)//非PE文件写入方式 5
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fsorg);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            br.Close();
                            fsorg.Close();
                            return;
                        }
                    }
                }
                int iCount = br.ReadInt32();
                if (iCount != dataTable5.Rows.Count)
                {
                    br.Close();
                    fsorg.Close();
                    MessageBox.Show("工程文件与源文件不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fstra);
                int iL = br.ReadInt32();
                int iStart = (int)fsorg.Length - iL;
                fsorg.Seek(0, SeekOrigin.Begin);
                bw.Write(br.ReadBytes(iStart));
                ArrayList al1 = new ArrayList();
                ArrayList al2 = new ArrayList();
                int iorg = 0;
                int itra = 0;
                int x = 0;
                progressBar1.Maximum = dataTable1.Rows.Count;
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    progressBar1.Value = i;
                    al1.Add(dataTable1.Rows[i][0].ToString());
                    al2.Add(x);
                    itra = (int)dataTable1.Rows[i][4];
                    if (itra > 0)
                    {
                        itra++;
                        if ((bool)dataTable1.Rows[i][13] == true)
                        {
                            x = x + 4 + itra;
                            bw.Write(itra);
                            bw.Write(Encoding.UTF8.GetBytes(dataTable1.Rows[i][2].ToString()));
                            bw.Write((byte)0);
                        }
                        else
                        {
                            x = x + itra;
                            bw.Write(Encoding.UTF8.GetBytes(dataTable1.Rows[i][2].ToString()));
                            bw.Write((byte)0);
                        }
                    }
                    else
                    {
                        iorg = (int)dataTable1.Rows[i][3];
                        iorg++;
                        if ((bool)dataTable1.Rows[i][13] == true)
                        {
                            x = x + 4 + iorg;
                            bw.Write(iorg);
                            bw.Write(Encoding.GetEncoding(mainform.ProOrgCode).GetBytes(dataTable1.Rows[i][1].ToString()));
                            bw.Write((byte)0);
                        }
                        else
                        {
                            x = x + iorg;
                            bw.Write(Encoding.GetEncoding(mainform.ProOrgCode).GetBytes(dataTable1.Rows[i][1].ToString()));
                            bw.Write((byte)0);
                        }
                    }
                }
                iL = (int)fstra.Length - iStart;
                fstra.Seek(4, SeekOrigin.Begin);
                bw.Write(iL);
                string m = "";
                progressBar1.Maximum = iCount;
                for (int i = 0; i < iCount; i++)
                {
                    progressBar1.Value = i;
                    iorg = int.Parse(dataTable5.Rows[i][2].ToString());
                    m = dataTable5.Rows[i][0].ToString();
                    for (int y = 0; y < al1.Count; y++)
                    {
                        if (m == al1[y].ToString())
                        {
                            fstra.Seek(iorg * 8 + 12, SeekOrigin.Begin);
                            bw.Write((int)al2[y]);
                            break;
                        }
                    }
                }
                br.Close();
                bw.Close();
                fsorg.Close();
                fstra.Close();
                BLC = true;
                progressBar1.Maximum = 0;
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void QTpo(string s1)//非PE文件写入方式 6
        {
            if (mainform.F6BL == false)
            {
                if (File.Exists(s1) == true)
                {
                    DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (dr == DialogResult.Cancel)
                    {
                        return;
                    }
                }
            }
            FileStream fstra = new FileStream(s1, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fstra);
            progressBar1.Maximum = dataTable1.Rows.Count;
            for (int i = 0; i < dataTable1.Rows.Count; i++)
            {
                progressBar1.Value = i;
                if (dataTable1.Rows[i][2].ToString() == "")
                {
                    if (dataTable1.Rows[i][1].ToString() == "")
                    {
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes("\n"));
                    }
                    else
                    {
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(dataTable1.Rows[i][1].ToString()));
                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes("\n"));
                    }
                }
                else
                {
                    bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(dataTable1.Rows[i][2].ToString()));
                    bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes("\n"));
                }
            }
            bw.Close();
            fstra.Close();
            progressBar1.Maximum = 0;
            if (mainform.F6BL == false)
            {
                MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }

        private void PeW(string s1, string s2)//PE 写入，代码看起来很长，但很容易理解，写的太短却容易出错
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                try
                {
                    File.Delete(s1);
                }
                catch (Exception MyEx)
                {
                    fsorg.Close();
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                int i3 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)//先写入不需要翻译的部分
                {
                    //   0       1     2       3         4        5        6        7       8          9          10            11
                    //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                    //   12         13      14      15      16        17            18           19          20          21
                    //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                    //            实际是                           没有使用        0 无
                    //           长度标识                                         1 不变
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true || (int)tem[4] == 0)
                    {
                        continue;
                    }
                    if ((int)tem[7] < 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    if ((int)tem[9] > 0 || (bool)tem[16] == true)
                    {
                        continue;
                    }
                    for (int x = 0; x < (int)tem[3]; x++)
                    {
                        bw.Write((byte)0);
                    }
                    fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0 && (int)tem[9] == 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false && (bool)tem[16] == false)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入前移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[10] > 0)
                    {
                        i3 = i2 - (int)tem[10];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入后移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[11] > 0)
                    {
                        i3 = i2 + (int)tem[11];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入超写部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((bool)tem[12] == true)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入迁移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    if ((int)tem[9] > 0)
                    {
                        for (int m = 0; m < i1; m++)
                        {
                            if (tem[8].ToString() == dataTable1.Rows[m][0].ToString())
                            {
                                i3 = (int)dataTable1.Rows[m][4];
                                break;
                            }
                        }
                        if ((bool)tem[14] == true)//UTF8
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 2;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                long l1 = 0;
                long l2 = 0;
                if (dataTable6.Rows.Count > 0)//写入矩阵
                {
                    i2 = dataTable7.Rows.Count;
                    for (int i = 0; i < i2; i++)//初始化矩阵区域
                    {
                        l1 = CommonCode.HexToLong(dataTable7.Rows[i][0].ToString());
                        l2 = l1 + (int)dataTable7.Rows[i][1];
                        fstra.Seek(l1, SeekOrigin.Begin);
                        while (fstra.Position < l2)
                        {
                            bw.Write((byte)0);
                        }
                    }
                    for (int i = 0; i < i1; i++)
                    {
                        tem = dataTable1.Rows[i].ItemArray;
                        if ((bool)tem[16] == true)//矩阵
                        {
                            if ((bool)tem[14] == true)//utf8
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else if ((bool)tem[15] == true)//Unicode
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int x = 0; x < dataTable5.Rows.Count; x++)//写入偏移量
                {
                    fstra.Seek(CommonCode.HexToLong(dataTable5.Rows[x][1].ToString()), SeekOrigin.Begin);
                    bw.Write(int.Parse(dataTable5.Rows[x][2].ToString()));
                }
                BLC = true;
                br.Close();
                bw.Close();
                fstra.Close();
                fsorg.Close();
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void PeWStandard(string s1, string s2)//PE 写入，代码看起来很长，但很容易理解，写的太短却容易出错
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                try
                {
                    File.Delete(s1);
                }
                catch (Exception MyEx)
                {
                    fsorg.Close();
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                int i3 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)//先写入不需要翻译的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true || (int)tem[4] == 0)
                    {
                        continue;
                    }
                    if ((int)tem[7] < 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    if ((int)tem[9] > 0 || (bool)tem[16] == true)
                    {
                        continue;
                    }
                    for (int x = 0; x < (int)tem[3]; x++)
                    {
                        bw.Write((byte)0);
                    }
                    fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0 && (int)tem[9] == 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false && (bool)tem[16] == false)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入前移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[10] > 0)
                    {
                        i3 = i2 - (int)tem[10];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入后移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[11] > 0)
                    {
                        i3 = i2 + (int)tem[11];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                for (int y = 0; y < (int)tem[11]; y++)
                                {
                                    bw.Write((byte)0);
                                }
                                fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入超写部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((bool)tem[12] == true)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入迁移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    if ((int)tem[9] > 0)
                    {
                        for (int m = 0; m < i1; m++)
                        {
                            if (tem[8].ToString() == dataTable1.Rows[m][0].ToString())
                            {
                                i3 = (int)dataTable1.Rows[m][4];
                                break;
                            }
                        }
                        if ((bool)tem[14] == true)//UTF8
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 2;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            if ((bool)tem[13] == true)
                            {
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                long l1 = 0;
                long l2 = 0;
                if (dataTable6.Rows.Count > 0)//写入矩阵
                {
                    i2 = dataTable7.Rows.Count;
                    for (int i = 0; i < i2; i++)//初始化矩阵区域
                    {
                        l1 = CommonCode.HexToLong(dataTable7.Rows[i][0].ToString());
                        l2 = l1 + (int)dataTable7.Rows[i][1];
                        fstra.Seek(l1, SeekOrigin.Begin);
                        while (fstra.Position < l2)
                        {
                            bw.Write((byte)0);
                        }
                    }
                    for (int i = 0; i < i1; i++)
                    {
                        tem = dataTable1.Rows[i].ItemArray;
                        if ((bool)tem[16] == true)//矩阵
                        {
                            if ((bool)tem[14] == true)//utf8
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else if ((bool)tem[15] == true)
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        if ((bool)tem[13] == true)
                                        {
                                            fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                            bw.Write((int)tem[4] / 2);
                                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        }
                                        else
                                        {
                                            fstra.Seek(i3, SeekOrigin.Begin);
                                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int x = 0; x < dataTable5.Rows.Count; x++)//写入偏移量
                {
                    fstra.Seek(CommonCode.HexToLong(dataTable5.Rows[x][1].ToString()), SeekOrigin.Begin);
                    bw.Write(int.Parse(dataTable5.Rows[x][2].ToString()));
                }
                BLC = true;
                br.Close();
                bw.Close();
                fstra.Close();
                fsorg.Close();
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void PeWStandard2(string s1, string s2)//PE 写入，代码看起来很长，但很容易理解，写的太短却容易出错
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                try
                {
                    File.Delete(s1);
                }
                catch (Exception MyEx)
                {
                    fsorg.Close();
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                int i3 = 0;
                object[] tem = new object[22];
                for (int i = 0; i < i1; i++)//先写入不需要翻译的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true || (int)tem[4] == 0)
                    {
                        continue;
                    }
                    if ((int)tem[7] < 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    if ((int)tem[9] > 0 || (bool)tem[16] == true)
                    {
                        continue;
                    }
                    for (int x = 0; x < (int)tem[3]; x++)
                    {
                        bw.Write((byte)0);
                    }
                    fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0 && (int)tem[9] == 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false && (bool)tem[16] == false)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4]);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入前移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[10] > 0)
                    {
                        i3 = i2 - (int)tem[10];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4]);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入后移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[11] > 0)
                    {
                        i3 = i2 + (int)tem[11];
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                for (int y = 0; y < (int)tem[11]; y++)
                                {
                                    bw.Write((byte)0);
                                }
                                fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4]);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入超写部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((bool)tem[12] == true)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[14] == true)
                        {
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4]);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入迁移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    if ((int)tem[9] > 0)
                    {
                        for (int m = 0; m < i1; m++)
                        {
                            if (tem[8].ToString() == dataTable1.Rows[m][0].ToString())
                            {
                                i3 = (int)dataTable1.Rows[m][4];
                                break;
                            }
                        }
                        if ((bool)tem[14] == true)//UTF8
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[15] == true)
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 2;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            if ((bool)tem[13] == true)
                            {
                                bw.Write((int)tem[4]);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                long l1 = 0;
                long l2 = 0;
                if (dataTable6.Rows.Count > 0)//写入矩阵
                {
                    i2 = dataTable7.Rows.Count;
                    for (int i = 0; i < i2; i++)//初始化矩阵区域
                    {
                        l1 = CommonCode.HexToLong(dataTable7.Rows[i][0].ToString());
                        l2 = l1 + (int)dataTable7.Rows[i][1];
                        fstra.Seek(l1, SeekOrigin.Begin);
                        while (fstra.Position < l2)
                        {
                            bw.Write((byte)0);
                        }
                    }
                    for (int i = 0; i < i1; i++)
                    {
                        tem = dataTable1.Rows[i].ItemArray;
                        if ((bool)tem[16] == true)//矩阵
                        {
                            if ((bool)tem[14] == true)//utf8
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.UTF8.GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else if ((bool)tem[15] == true)
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        if ((bool)tem[13] == true)
                                        {
                                            fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                            bw.Write((int)tem[4]);
                                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        }
                                        else
                                        {
                                            fstra.Seek(i3, SeekOrigin.Begin);
                                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        }
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int x = 0; x < dataTable5.Rows.Count; x++)//写入偏移量
                {
                    fstra.Seek(CommonCode.HexToLong(dataTable5.Rows[x][1].ToString()), SeekOrigin.Begin);
                    bw.Write(int.Parse(dataTable5.Rows[x][2].ToString()));
                }
                BLC = true;
                br.Close();
                bw.Close();
                fstra.Close();
                fsorg.Close();
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        private void PeWDelphi(string s1, string s2)//PE 写入，代码看起来很长，但很容易理解，写的太短却容易出错
        {
            FileStream fsorg = new FileStream(s2, FileMode.Open, FileAccess.Read);
            long l = long.Parse(dataTable4.Rows[2][1].ToString()) - fsorg.Length;
            if (l != 0)
            {
                fsorg.Close();
                MessageBox.Show("当前文件的大小与创建这个工程时所使用文件的大小不匹配，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (mainform.F6BL == false)
                {
                    if (File.Exists(s1) == true)
                    {
                        DialogResult dr = MessageBox.Show("目标文件已存在，需要覆盖它吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                        if (dr == DialogResult.Cancel)
                        {
                            fsorg.Close();
                            return;
                        }
                    }
                }
                try
                {
                    File.Delete(s1);
                }
                catch (Exception MyEx)
                {
                    fsorg.Close();
                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FileStream fstra = new FileStream(s1, FileMode.Create);
                BinaryReader br = new BinaryReader(fsorg);
                BinaryWriter bw = new BinaryWriter(fstra);
                long orglong = fsorg.Length;
                int i1 = dataTable1.Rows.Count;
                int i2 = 0;
                int i3 = 0;
                object[] tem = new object[22];
                //   0       1     2       3         4        5        6        7       8          9          10            11
                //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                //   12         13      14      15      16        17            18           19          20          21
                //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                //            实际是                           没有使用        0 无
                //           长度标识                                         1 不变
                for (int i = 0; i < i1; i++)//先写入不需要翻译的部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    while (fsorg.Position < i2)
                    {
                        bw.Write((byte)fsorg.ReadByte());
                    }
                    if (tem[1].Equals(tem[2]) == true || (int)tem[4] == 0)
                    {
                        continue;
                    }
                    if ((int)tem[7] < 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false)
                    {
                        continue;
                    }
                    if ((int)tem[9] > 0 || (bool)tem[16] == true)
                    {
                        continue;
                    }
                    for (int x = 0; x < (int)tem[3]; x++)
                    {
                        bw.Write((byte)0);
                    }
                    fsorg.Seek(fstra.Position, SeekOrigin.Begin);
                }
                while (fsorg.Position < orglong)
                {
                    bw.Write((byte)fsorg.ReadByte());
                }
                for (int i = 0; i < i1; i++)//再写入不需要处理的部分
                {
                    //   0       1     2       3         4        5        6        7       8          9          10            11
                    //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                    //   12         13      14      15      16        17            18           19          20          21
                    //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                    //            实际是                           没有使用        0 无
                    //           长度标识                                         1 不变
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[4] > 0 && (int)tem[7] >= 0 && (int)tem[9] == 0 && (int)tem[10] == 0 && (int)tem[11] == 0 && (bool)tem[12] == false && (bool)tem[16] == false)
                    {
                        fstra.Seek(i2, SeekOrigin.Begin);
                        if ((bool)tem[15] == true)//Unicode
                        {
                            if ((bool)tem[13] == true)//Delphi
                            {
                                fstra.Seek(i2 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                            else
                            {
                                bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                            }
                        }
                        else if ((bool)tem[13] == true)
                        {
                            if ((int)tem[18] > 1)//代码页
                            {
                                fstra.Seek(i2 - 12, SeekOrigin.Begin);
                                bw.Write(Int16.Parse(tem[18].ToString()));
                            }
                            fstra.Seek(i2 - 4, SeekOrigin.Begin);
                            bw.Write((int)tem[4]);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入前移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());//地址
                    if ((int)tem[10] > 0)
                    {
                        i3 = i2 - (int)tem[10];//新的地址
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[15] == true)//Unicode
                        {
                            if ((bool)tem[13] == true)//Delphi
                            {
                                if ((int)tem[18] == 1)//代码页
                                {
                                    fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                else
                                {
                                    fstra.Seek(i3 - 8, SeekOrigin.Begin);
                                }
                                bw.Write(4294967295);
                                bw.Write((int)tem[4] / 2);
                            }
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[13] == true)//Delphi
                        {
                            if ((int)tem[18] > 0)//代码页
                            {
                                fstra.Seek(i3 - 12, SeekOrigin.Begin);//新地址
                                if ((int)tem[18] == 1)
                                {
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                else
                                {
                                    bw.Write(Int16.Parse(tem[18].ToString()));
                                    fsorg.Seek(i2 - 10, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                            }
                            else
                            {
                                fstra.Seek(i3 - 8, SeekOrigin.Begin);
                            }
                            bw.Write(4294967295);
                            bw.Write((int)tem[4]);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入后移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i2 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((int)tem[11] > 0)
                    {
                        i3 = i2 + (int)tem[11];//新地址
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[15] == true)//Unicode
                        {
                            if ((bool)tem[13] == true)//Delphi
                            {
                                if ((int)tem[18] == 1)
                                {
                                    fstra.Seek(i2 - 12, SeekOrigin.Begin);
                                    for (int y = 0; y < 12; y++)
                                    {
                                        bw.Write((byte)0);
                                    }
                                    fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                else
                                {
                                    fstra.Seek(i2 - 8, SeekOrigin.Begin);
                                    for (int y = 0; y < 8; y++)
                                    {
                                        bw.Write((byte)0);
                                    }
                                    fstra.Seek(i3 - 8, SeekOrigin.Begin);
                                }
                                bw.Write(4294967295);
                                bw.Write((int)tem[4] / 2);
                            }
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[13] == true)
                        {
                            if ((int)tem[18] > 0)
                            {
                                fstra.Seek(i2 - 12, SeekOrigin.Begin);
                                for (int y = 0; y < 12; y++)
                                {
                                    bw.Write((byte)0);
                                }
                                fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                if ((int)tem[18] == 1)
                                {
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                else
                                {
                                    bw.Write(Int16.Parse(tem[18].ToString()));
                                    fsorg.Seek(i2 - 10, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                            }
                            else
                            {
                                fstra.Seek(i2 - 8, SeekOrigin.Begin);
                                for (int y = 0; y < 8; y++)
                                {
                                    bw.Write((byte)0);
                                }
                                fstra.Seek(i3 - 8, SeekOrigin.Begin);
                            }
                            bw.Write(4294967295);
                            bw.Write((int)tem[4]);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入超写部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    i3 = (int)CommonCode.HexToLong(tem[0].ToString());
                    if ((bool)tem[12] == true)
                    {
                        fstra.Seek(i3, SeekOrigin.Begin);
                        if ((bool)tem[15] == true)
                        {
                            if ((bool)tem[13] == true)
                            {
                                fstra.Seek(i3 - 4, SeekOrigin.Begin);
                                bw.Write((int)tem[4] / 2);
                            }
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[13] == true)
                        {
                            if ((int)tem[18] > 1)
                            {
                                fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                bw.Write(Int16.Parse(tem[18].ToString()));
                            }
                            fstra.Seek(i3 - 4, SeekOrigin.Begin);
                            bw.Write((int)tem[4]);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                for (int i = 0; i < i1; i++)//写入迁移部分
                {
                    tem = dataTable1.Rows[i].ItemArray;
                    if ((int)tem[9] > 0)
                    {
                        for (int m = 0; m < i1; m++)
                        {
                            if (tem[8].ToString() == dataTable1.Rows[m][0].ToString())
                            {
                                i3 = (int)dataTable1.Rows[m][4];//翻译长度
                                i2 = (int)CommonCode.HexToLong(dataTable1.Rows[m][0].ToString());//地址
                                break;
                            }
                        }
                        if ((bool)tem[15] == true)//Unicode
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 2;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            if ((bool)tem[13] == true)
                            {
                                if ((int)tem[18] == 1)
                                {
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                bw.Write(4294967295);
                                bw.Write((int)tem[4] / 2);
                            }
                            bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                        }
                        else if ((bool)tem[13] == true)
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            if ((int)tem[18] > 0)
                            {
                                if ((int)tem[18] == 1)
                                {
                                    fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                                else
                                {
                                    bw.Write(Int16.Parse(tem[18].ToString()));
                                    fsorg.Seek(i2 - 10, SeekOrigin.Begin);
                                    bw.Write((byte)fsorg.ReadByte());
                                    bw.Write((byte)fsorg.ReadByte());
                                }
                            }
                            bw.Write(4294967295);
                            bw.Write((int)tem[4]);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                        else
                        {
                            i3 = (int)CommonCode.HexToLong(tem[8].ToString()) + i3 + 1;
                            fstra.Seek(i3, SeekOrigin.Begin);
                            bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                        }
                    }
                }
                long l1 = 0;
                long l2 = 0;
                if (dataTable6.Rows.Count > 0)//写入矩阵
                {
                    i2 = dataTable7.Rows.Count;
                    for (int i = 0; i < i2; i++)//初始化矩阵区域
                    {
                        l1 = CommonCode.HexToLong(dataTable7.Rows[i][0].ToString());
                        l2 = l1 + (int)dataTable7.Rows[i][1];
                        fstra.Seek(l1, SeekOrigin.Begin);
                        while (fstra.Position < l2)
                        {
                            bw.Write((byte)0);
                        }
                    }
                    for (int i = 0; i < i1; i++)
                    {
                        tem = dataTable1.Rows[i].ItemArray;
                        if ((bool)tem[16] == true)//矩阵
                        {
                            if ((bool)tem[15] == true)//Unicode
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());//字符串地址
                                        if ((bool)tem[13] == true)//Delphi
                                        {
                                            if ((int)tem[18] == 1)//代码页
                                            {
                                                i2 = (int)CommonCode.HexToLong(dataTable6.Rows[x][0].ToString());
                                                fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                                fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                            }
                                            else
                                            {
                                                fstra.Seek(i3 - 8, SeekOrigin.Begin);
                                            }
                                            bw.Write(4294967295);
                                            bw.Write((int)tem[4] / 2);
                                        }
                                        else
                                        {
                                            fstra.Seek(i3, SeekOrigin.Begin);
                                        }
                                        bw.Write(Encoding.Unicode.GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else if ((bool)tem[13] == true)//Delphi
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());//字符串地址
                                        if ((int)tem[18] > 0)//代码页
                                        {
                                            i2 = (int)CommonCode.HexToLong(dataTable6.Rows[x][0].ToString());
                                            fstra.Seek(i3 - 12, SeekOrigin.Begin);
                                            if ((int)tem[18] == 1)
                                            {
                                                fsorg.Seek(i2 - 12, SeekOrigin.Begin);
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                            }
                                            else
                                            {
                                                bw.Write(Int16.Parse(tem[18].ToString()));
                                                fsorg.Seek(i2 - 10, SeekOrigin.Begin);
                                                bw.Write((byte)fsorg.ReadByte());
                                                bw.Write((byte)fsorg.ReadByte());
                                            }
                                        }
                                        else
                                        {
                                            fstra.Seek(i3 - 8, SeekOrigin.Begin);
                                        }
                                        bw.Write(4294967295);
                                        bw.Write((int)tem[4]);
                                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int x = 0; x < dataTable6.Rows.Count; x++)
                                {
                                    if (dataTable6.Rows[x][0].ToString() == tem[0].ToString())
                                    {
                                        i3 = (int)CommonCode.HexToLong(dataTable6.Rows[x][2].ToString());
                                        fstra.Seek(i3, SeekOrigin.Begin);
                                        bw.Write(Encoding.GetEncoding(mainform.ProTraCode).GetBytes(tem[2].ToString()));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int x = 0; x < dataTable5.Rows.Count; x++)//写入偏移量
                {
                    fstra.Seek(CommonCode.HexToLong(dataTable5.Rows[x][1].ToString()), SeekOrigin.Begin);
                    bw.Write(int.Parse(dataTable5.Rows[x][2].ToString()));
                }
                BLC = true;
                br.Close();
                bw.Close();
                fstra.Close();
                fsorg.Close();
                if (mainform.F6BL == false)
                {
                    MessageBox.Show("创建目标文件成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.Close();
            }
        }

        public bool BuildTargetFile(string s1)
        {
            if (s1 == "")
            {
                MessageBox.Show("请指定要保存的文件。", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
                string s2 = Path.GetDirectoryName(s1);
                if (Directory.Exists(s2) == false)
                {
                    MessageBox.Show("文件夹“" + s2 + "”不存在，无法保存文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (File.Exists(mainform.ProjectFileName) == false)
                {
                    MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    dataSet1.Clear();
                    SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName);
                    MyAccess.Open();
                    SQLiteCommand cmd = new SQLiteCommand(MyAccess);
                    SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                    try
                    {
                        cmd.Transaction = MyAccess.BeginTransaction();
                        if (s1 != mainform.TargetPath)
                        {
                            cmd.CommandText = "update fileinfo set detail = '" + s1.Replace(mainform.CDirectory + "工程\\文件\\", "") + "' where infoname = '目标'";
                            cmd.ExecuteNonQuery();
                            mainform.TargetPath = s1;
                        }
                        if (mainform.UnPEType == "5" || mainform.UnPEType == "6")
                        {
                            cmd.CommandText = "select * from athenaa";
                        }
                        else
                        {
                            cmd.CommandText = "select * from athenaa where tralong > 0 and Ignoretra = 0 ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        }
                        ad.Fill(dataTable1);
                        cmd.CommandText = "select * from fileinfo";
                        ad.Fill(dataTable4);
                        cmd.CommandText = "select * from calladd";
                        ad.Fill(dataTable5);
                        cmd.CommandText = "select * from matrixzone";
                        ad.Fill(dataTable6);
                        cmd.CommandText = "select * from matrix";
                        ad.Fill(dataTable7);
                        cmd.Transaction.Commit();
                    }
                    catch (Exception MyEx)
                    {
                        MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        MyAccess.Close();
                    }
                    s2 = mainform.FilePath;
                    if (File.Exists(s2) == false && mainform.UnPEType != "6")
                    {
                        MessageBox.Show("没有找到原始文件“" + s2 + "”，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (s1 == s2 && mainform.UnPEType != "6")
                    {
                        MessageBox.Show("目标文件位置不能与原始文件位置相同。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (mainform.PEbool == true)//PE 文件写入
                    {
                        string oldstr = dataTable4.Rows[1][1].ToString();
                        string newstr = FileVersionInfo.GetVersionInfo(s2).FileVersion;
                        if (newstr == null)
                        {
                            newstr = "";
                        }
                        if (oldstr != newstr)
                        {
                            MessageBox.Show("当前文件的版本与创建这个工程时所使用文件的版本不相同，无法生成目标文件。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            if (mainform.StrLenCategory == "无")
                            {
                                PeW(s1, s2);
                            }
                            else if (mainform.StrLenCategory == "标准")
                            {
                                PeWStandard(s1, s2);
                            }
                            else if (mainform.StrLenCategory == "标准2")
                            {
                                PeWStandard2(s1, s2);
                            }
                            else if (mainform.StrLenCategory == "Delphi")
                            {
                                PeWDelphi(s1, s2);
                            }
                        }
                    }
                    else
                    {
                        if (mainform.UnPEType == "1")
                        {
                            UnPeW1(s1, s2);
                        }
                        else if (mainform.UnPEType == "2")
                        {
                            UnPeW2(s1, s2);
                        }
                        else if (mainform.UnPEType == "3")
                        {
                            UnPeW3(s1, s2);
                        }
                        else if (mainform.UnPEType == "4")
                        {
                            UnPeW4(s1, s2);
                        }
                        else if (mainform.UnPEType == "5")
                        {
                            Skyrim(s1, s2);
                        }
                        else if (mainform.UnPEType == "6")
                        {
                            QTpo(s1);
                        }
                    }
                }
                button2.Enabled = true;
                button3.Enabled = true;
            }
            if (BLC == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button2_Click(object sender, EventArgs e)//生成目标文件
        {
            string s = textBox1.Text;
            mainform.F6BL = false;
            BuildTargetFile(s);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GenerateTarget_Shown(object sender, EventArgs e)
        {
            textBox1.Text = mainform.TargetPath;
            label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
            button1.Location = new Point(button1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - button1.Height / 2D));
        }
    }
}
