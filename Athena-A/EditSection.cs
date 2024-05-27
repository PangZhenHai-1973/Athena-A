using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class EditSection : Form
    {
        int CPU = 332;//程序类型
        object[] obs = new object[8];
        uint PEAddress = 0;////PE标识位置，确定这个位置，其他位置都可以确定
        int NumberOfSections = 0;//节区数目
        int AvailableSections = 0;//可用节数
        uint SectionAlignment = 0;//内存对齐
        uint FileAlignment = 0;//文件对齐
        uint SizeOfImage = 0;//在内存中的映射大小，必须是内存对齐的整数倍
        uint SizeOfHeaders = 0;//所有头 + 节表的总大小，严格按照 200h 对齐
        uint DS = 0;//数字签名
        uint SizeOfDS = 0;//数字签名大小
        public static int AddBytes = 0;
        public static string SectionCharacteristics = "只读";
        byte[] NameOfNewSection = Encoding.ASCII.GetBytes(".athenaa");

        public EditSection()
        {
            InitializeComponent();
        }

        void Analyze_PE()//提取并保存 PE 头文件信息
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            toolTip1.Active = false;
            dataGridView1.Rows.Clear();
            for (int i = 0; i < 8; i++)
            {
                obs[i] = "";
            }
            try
            {
                using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        fs.Seek(60, SeekOrigin.Begin);//寻找PE标识
                        byte[] bn = new byte[8];
                        PEAddress = br.ReadUInt32();//PE标识位置
                        fs.Seek(PEAddress + 4, SeekOrigin.Begin);//CPU 类型
                        CPU = br.ReadUInt16();//读出 CPU 类型
                        if (CPU == 332)
                        {
                            fs.Seek(PEAddress + 6, SeekOrigin.Begin);//读出文件段数
                            NumberOfSections = br.ReadUInt16();//文件段数
                            fs.Seek(PEAddress + 52, SeekOrigin.Begin);//基址
                            obs[1] = "基址";
                            obs[2] = br.ReadUInt32().ToString("X8");//基址
                            dataGridView1.Rows.Add(obs);
                            //
                            fs.Seek(PEAddress + 56, SeekOrigin.Begin);
                            SectionAlignment = br.ReadUInt32();//内存对齐大小
                            if (SectionAlignment < 4096)
                            {
                                SectionAlignment = 4096;
                            }
                            else if (SectionAlignment > 8192)
                            {
                                SectionAlignment = 8192;
                            }
                            FileAlignment = br.ReadUInt32();//文件对齐大小
                            if (FileAlignment < 512)
                            {
                                FileAlignment = 512;
                            }
                            else if (FileAlignment > 4096)
                            {
                                FileAlignment = 4096;
                            }
                            //
                            fs.Seek(PEAddress + 80, SeekOrigin.Begin);
                            SizeOfImage = br.ReadUInt32();//在内存中的映射大小，必须是内存对齐的整数倍
                            SizeOfHeaders = br.ReadUInt32();//这里要判断扩展节区的话，空间是否不足
                                                            //
                            fs.Seek(PEAddress + 136, SeekOrigin.Begin);//资源段
                            obs[1] = "资源";
                            obs[2] = br.ReadUInt32().ToString("X8");//资源段虚拟地址
                            dataGridView1.Rows.Add(obs);
                            //
                            fs.Seek(PEAddress + 152, SeekOrigin.Begin);
                            DS = br.ReadUInt32();
                            SizeOfDS = br.ReadUInt32();
                            //
                            fs.Seek(PEAddress + 248, SeekOrigin.Begin);//各个段
                            for (int i = 0; i < NumberOfSections; i++)
                            {
                                obs[0] = i + 1;
                                for (int x = 0; x < 8; x++)
                                {
                                    bn[x] = br.ReadByte();
                                }
                                obs[1] = Encoding.ASCII.GetString(bn);
                                obs[2] = br.ReadUInt32().ToString("X8");
                                obs[3] = br.ReadUInt32().ToString("X8");
                                obs[4] = br.ReadUInt32().ToString("X8");
                                obs[5] = br.ReadUInt32().ToString("X8");
                                fs.Seek(fs.Position + 12, SeekOrigin.Begin);
                                obs[6] = br.ReadUInt32().ToString("X8");
                                dataGridView1.Rows.Add(obs);
                            }
                            AvailableSections = (int)(SizeOfHeaders - PEAddress - 248) / 40;
                        }
                        else
                        {
                            fs.Seek(PEAddress + 6, SeekOrigin.Begin);//读出文件段数
                            NumberOfSections = br.ReadUInt16();//文件段数
                            fs.Seek(PEAddress + 48, SeekOrigin.Begin);//基址
                            obs[1] = "基址";
                            obs[2] = br.ReadUInt64().ToString("X16");//基址
                            dataGridView1.Rows.Add(obs);
                            //
                            fs.Seek(PEAddress + 56, SeekOrigin.Begin);
                            SectionAlignment = br.ReadUInt32();//内存对齐大小
                            if (SectionAlignment < 4096)
                            {
                                SectionAlignment = 4096;
                            }
                            else if (SectionAlignment > 8192)
                            {
                                SectionAlignment = 8192;
                            }
                            FileAlignment = br.ReadUInt32();//文件对齐大小
                            if (FileAlignment < 512)
                            {
                                FileAlignment = 512;
                            }
                            else if (FileAlignment > 4096)
                            {
                                FileAlignment = 4096;
                            }
                            //
                            fs.Seek(PEAddress + 80, SeekOrigin.Begin);
                            SizeOfImage = br.ReadUInt32();//在内存中的映射大小，必须是内存对齐的整数倍
                            SizeOfHeaders = br.ReadUInt32();//这里要判断扩展节区的话，空间是否不足
                                                            //
                            fs.Seek(PEAddress + 152, SeekOrigin.Begin);//资源段
                            obs[1] = "资源";
                            obs[2] = br.ReadUInt32().ToString("X8");//资源段虚拟地址
                            dataGridView1.Rows.Add(obs);
                            //
                            fs.Seek(PEAddress + 168, SeekOrigin.Begin);
                            DS = br.ReadUInt32();
                            SizeOfDS = br.ReadUInt32();
                            //
                            fs.Seek(PEAddress + 264, SeekOrigin.Begin);//各个段
                            for (int i = 0; i < NumberOfSections; i++)
                            {
                                obs[0] = i + 1;
                                for (int x = 0; x < 8; x++)
                                {
                                    bn[x] = br.ReadByte();
                                }
                                obs[1] = Encoding.ASCII.GetString(bn);
                                obs[2] = br.ReadUInt32().ToString("X8");
                                obs[3] = br.ReadUInt32().ToString("X8");
                                obs[4] = br.ReadUInt32().ToString("X8");
                                obs[5] = br.ReadUInt32().ToString("X8");
                                fs.Seek(fs.Position + 12, SeekOrigin.Begin);
                                obs[6] = br.ReadUInt32().ToString("X8");
                                dataGridView1.Rows.Add(obs);
                            }
                            AvailableSections = (int)(SizeOfHeaders - PEAddress - 264) / 40;
                        }
                        if (DS > 0 && DS == (fs.Length - SizeOfDS))
                        {
                            obs[0] = obs[1] = obs[2] = obs[3] = "";
                            obs[4] = SizeOfDS.ToString("X8");
                            obs[5] = DS.ToString("X8");
                            obs[6] = "";
                            obs[7] = "数字签名";
                            dataGridView1.Rows.Add(obs);
                        }
                        //MessageBox.Show("映射大小：" + SizeOfImage.ToString("X8")
                        //    + "\r\n" + "计算得出的大小："
                        //    + (CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Value.ToString()) + CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value.ToString())).ToString("X8"));
                        //MessageBox.Show("文件头大小：" + SizeOfHeaders.ToString("X8"));
                        //MessageBox.Show("校验和：" + CheckSum.ToString("X8"));
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message, "错误");
            }
            toolTip1.Active = true;
        }

        private void EditSection_Shown(object sender, EventArgs e)
        {
            if (mainform.MyDpi > 96F)
            {
                label1.Location = new Point(label1.Location.X, textBox1.Location.Y + (int)(textBox1.Height / 2D - label1.Height / 2D));
                textBox1.Width = button1.Location.X + button1.Width - textBox1.Location.X;
            }
            if (File.Exists(mainform.FilePath))
            {
                if (mainform.PEbool)
                {
                    textBox1.Text = mainform.FilePath;
                    Analyze_PE();
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int i1 = dataGridView1.Rows.Count - 1;
            int r = e.RowIndex;
            int c = e.ColumnIndex;
            int d = dataGridView1.Rows.Count;
            if (r > 1 && r < d)
            {
                if (c > 3 && c < 6)
                {
                    toolTip1.RemoveAll();
                    string s = dataGridView1.Rows[r].Cells[c].Value.ToString();
                    toolTip1.SetToolTip(dataGridView1, CommonCode.HexToLong(s).ToString());
                    if (c == 5)
                    {
                        Clipboard.Clear();
                        Clipboard.SetText(s);
                    }
                }
            }
            if (r == i1)
            {
                if (dataGridView1.Rows[r].Cells[7].Value.ToString() == "数字签名")
                {
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                }
                else if (dataGridView1.Rows[r].Cells[1].Value.ToString() == ".athenaa")
                {
                    button2.Enabled = false;
                    button3.Enabled = true;
                    button4.Enabled = true;
                }
                else
                {
                    if (AvailableSections > NumberOfSections)
                    {
                        button2.Enabled = true;
                        button3.Enabled = false;
                        button4.Enabled = false;
                    }
                }
            }
            else
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (CommonCode.PE(openFileDialog1.FileName))
                {
                    textBox1.Text = openFileDialog1.FileName;
                    Analyze_PE();
                }
            }
        }

        private void EditSection_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    if (CommonCode.PE(filenames[0]))
                    {
                        Clipboard.SetDataObject(filenames[0]);
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void EditSection_DragDrop(object sender, DragEventArgs e)
        {
            textBox1.Text = (string)Clipboard.GetData(DataFormats.Text);
            Analyze_PE();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toolTip1.RemoveAll();
            AddBytes = 0;
            if (File.Exists(textBox1.Text))
            {
                AddSection AS = new AddSection();
                AS.Font = mainform.MyNewFont;
                AS.ShowDialog();
                if (AddBytes > 0)
                {
                    int i1 = 0;
                    int i2 = 0;
                    i1 = Math.DivRem(AddBytes, (int)SectionAlignment, out i2);
                    if (i2 > 0)
                    {
                        i1++;
                    }
                    int AddBytesOfImage = (int)SectionAlignment * i1;
                    //SizeOfImage 在内存中的映射大小，必须是内存对齐的整数倍
                    using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            fs.Seek(PEAddress + 6, SeekOrigin.Begin);
                            bw.Write((Int16)(NumberOfSections + 1));//增加节区数
                            fs.Seek(PEAddress + 80, SeekOrigin.Begin);//内存映像大小
                            bw.Write((UInt32)(SizeOfImage + AddBytesOfImage));
                            if (CPU == 332)
                            {
                                fs.Seek(PEAddress + 248 + NumberOfSections * 40, SeekOrigin.Begin);//新节开始位置
                            }
                            else
                            {
                                fs.Seek(PEAddress + 264 + NumberOfSections * 40, SeekOrigin.Begin);//新节开始位置
                            }
                            bw.Write(NameOfNewSection);
                            bw.Write((UInt32)AddBytesOfImage);
                            bw.Write(SizeOfImage);
                            i1 = Math.DivRem(AddBytes, (int)FileAlignment, out i2);
                            if (i2 > 0)
                            {
                                i1++;
                            }
                            int AddBytesOfFile = (int)FileAlignment * i1;
                            bw.Write((UInt32)AddBytesOfFile);
                            i1 = Math.DivRem((int)fs.Length, 16, out i2);
                            if (i2 > 0)
                            {
                                i1++;
                            }
                            i1 = i1 * 16;
                            bw.Write((UInt32)i1);
                            fs.Seek(fs.Position + 12, SeekOrigin.Begin);
                            if (SectionCharacteristics == "只读")
                            {
                                bw.Write((UInt32)1073741888);
                            }
                            else if (SectionCharacteristics == "可执行")
                            {
                                bw.Write((UInt32)1610612768);
                            }
                            i1 = i1 + AddBytesOfFile;
                            fs.SetLength(i1);
                        }
                    }
                    Analyze_PE();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            toolTip1.RemoveAll();
            AddBytes = 0;
            if (File.Exists(textBox1.Text))
            {
                AddSection AS = new AddSection();
                AS.Font = mainform.MyNewFont;
                AS.ShowDialog();
                if (AddBytes > 0)
                {
                    int i1 = 0;
                    int i2 = 0;
                    i1 = Math.DivRem(AddBytes, (int)SectionAlignment, out i2);
                    if (i2 > 0)
                    {
                        i1++;
                    }
                    int EditBytesOfImage = (int)SectionAlignment * i1;
                    //SizeOfImage 在内存中的映射大小，必须是内存对齐的整数倍
                    using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Write, FileShare.None))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            fs.Seek(PEAddress + 80, SeekOrigin.Begin);//内存映像大小
                            bw.Write((UInt32)(CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value.ToString()) + EditBytesOfImage));
                            if (CPU == 332)
                            {
                                fs.Seek(PEAddress + 248 + (NumberOfSections - 1) * 40 + 8, SeekOrigin.Begin);//新节开始位置
                            }
                            else
                            {
                                fs.Seek(PEAddress + 264 + (NumberOfSections - 1) * 40 + 8, SeekOrigin.Begin);//新节开始位置
                            }
                            bw.Write((UInt32)EditBytesOfImage);
                            i1 = Math.DivRem(AddBytes, (int)FileAlignment, out i2);
                            if (i2 > 0)
                            {
                                i1++;
                            }
                            int EditBytesOfFile = (int)FileAlignment * i1;
                            fs.Seek(fs.Position + 4, SeekOrigin.Begin);
                            bw.Write((UInt32)EditBytesOfFile);
                            fs.Seek(fs.Position + 16, SeekOrigin.Begin);
                            if (SectionCharacteristics == "只读")
                            {
                                bw.Write((UInt32)1073741888);
                            }
                            else if (SectionCharacteristics == "可执行")
                            {
                                bw.Write((UInt32)1610612768);
                            }
                            i1 = (int)CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value.ToString()) + EditBytesOfFile;
                            fs.SetLength(i1);
                        }
                    }
                    Analyze_PE();
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(textBox1.Text))
            {
                int i1 = 0;
                //SizeOfImage 在内存中的映射大小，必须是内存对齐的整数倍
                using (FileStream fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        fs.Seek(PEAddress + 6, SeekOrigin.Begin);
                        bw.Write((Int16)(NumberOfSections - 1));//增加节区数
                        fs.Seek(PEAddress + 80, SeekOrigin.Begin);//内存映像大小
                        i1 = (int)CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[3].Value.ToString());
                        bw.Write((UInt32)i1);
                        if (CPU == 332)
                        {
                            fs.Seek(PEAddress + 248 + (NumberOfSections - 1) * 40, SeekOrigin.Begin);//新节开始位置
                        }
                        else
                        {
                            fs.Seek(PEAddress + 264 + (NumberOfSections - 1) * 40, SeekOrigin.Begin);//新节开始位置
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            bw.Write((byte)0);
                        }
                        i1 = (int)CommonCode.HexToLong(dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[5].Value.ToString());
                        fs.SetLength(i1);
                    }
                }
                Analyze_PE();
            }
        }
    }
}
