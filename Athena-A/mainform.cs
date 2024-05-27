using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Athena_A
{
    public partial class mainform : Form
    {
        static int wt = 0;//备份时间
        static int orgl = 0;//原文长度
        static int tral = 0;//译文长度
        public static int ProOrgCode = 0;//原始语言代码页
        public static int ProTraCode = 0;//目标语言代码页
        public static string CDirectory;//当前目录
        public static string ProjectFileName = "";//工程名
        public static string TargetPath = "";//生成目标文件的新路径
        public static string FilePath;//文件的路径
        public static string runtest = "";//记忆执行测试路径
        public static string FileSize;//文件的大小
        public static string AddStr = "";
        public static string RStr1 = "";//替换字符串1
        public static string RStr2 = "";//替换字符串2
        public static string Marker = "";//标记位置
        public static string DictionaryStr = "";//字典
        public static string UnPEType = "";//非 PE 生成目标类型
        public static string CPUType = "";//CPU 类型
        public static string ProEncoding = "";//编码类型
        public static string ProTraName = "";//ANSI 翻译目标语言
        public static string StrLenCategory = "";//字符串长度类别
        public static string DelphiCodePage = "";//记忆代码页
        public static object[] obMS = new object[22];//当前选定行的对象
        public static ArrayList l1 = new ArrayList();//挪移使用 PE 文件头信息
        public static ArrayList l2 = new ArrayList();
        public static ArrayList l3 = new ArrayList();
        public static ArrayList l4 = new ArrayList();
        public static ArrayList SysFont = new ArrayList();
        public static ArrayList PriFont = new ArrayList();
        public static bool AddStringbl = false;//判断添加字符串是否是
        public static bool AutoTrabl = false;//是否完成自动翻译
        public static bool F6BL = false;//F6键创建
        public static bool PEbool = false;//是否是 PE 文件
        public static bool NewPro = false;//是否是新建工程
        public static bool MatrixDel = false;//是否是清除矩阵
        public static bool MatchCase = false;//大小写匹配
        public static bool RowEnterbool = true;//进入一行
        public static bool FPBool = false;//提取的原始文件是否存在
        public static bool hexToolMenuChecked = true;//十六进制显示
        static bool EditItemBL = false;//编辑是否成功
        static bool FilterMessage = false;//是否显示添加过滤成功消息
        static bool SPB = false;//进度栏
        string li = "i";//长度标识，i 为整数，h 为十六进制值
        private BackgroundWorker DeleteRowsbackgroundWorker = new BackgroundWorker();
        private BackgroundWorker FilterProjectBackgroundWorker = new BackgroundWorker();
        private BackgroundWorker AddFilterstringBackgroundWorker = new BackgroundWorker();
        private bool BackupProjectTaskbool = false;
        public static Encoding AA_Default_Encoding;
        //dataTable1 工程
        //dataTable4 工程属性信息表。
        //dataTable5 新的挪移地址
        //dataTable6 字典
        //
        //   0       1     2       3         4        5        6        7       8          9          10            11
        //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
        //   12         13      14      15      16        17            18           19          20          21
        //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2

        //0 文件
        //1 版本
        //2 大小
        //3 目标
        //4 非PE类型
        //5 运行
        //6 字典
        //7 标记
        //8 PE类型
        //9 长度标识
        //10 代码页

        public mainform()
        {
            InitializeComponent();
        }

        private void AutoTest()//生成运行
        {
            if (File.Exists(ProjectFileName) == true)
            {
                if (TargetPath == "")
                {
                    TGGreate();
                }
                else
                {
                    GenerateTarget GT = new GenerateTarget();
                    F6BL = true;
                    if (GT.BuildTargetFile(TargetPath) == true)
                    {
                        if (File.Exists(runtest) == false)
                        {
                            runtestform rtf = new runtestform();
                            if (MyDpi > 96F)
                            {
                                rtf.Font = MyNewFont;
                            }
                            rtf.ShowDialog();
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(runtest);
                        }
                    }
                }
            }
        }

        private void EnablePanelContrl()//启用各个面板
        {
            panel1.Enabled = true;
            menuStrip1.Enabled = true;
        }

        private void DisabledPanelContrl()//禁用各个面板
        {
            panel1.Enabled = false;
            menuStrip1.Enabled = false;
        }

        private void SearchString()//在字典中搜索字符串
        {
            if (dataTable6.Rows.Count > 0)
            {
                toolTip1.SetToolTip(textBox2, "");
                textBox2.Clear();
                EnumerableRowCollection<DataRow> dtResult;
                string s3 = textBox3.Text;
                if (MatchCase)
                {
                    dtResult = from dt6 in dataTable6.AsEnumerable() where dt6.Field<string>(0) == s3 select dt6;
                }
                else
                {
                    string strTmp = s3.ToLower();
                    dtResult = from dt6 in dataTable6.AsEnumerable() where dt6.Field<string>(0).ToLower() == strTmp select dt6;
                }
                int i1 = dtResult.Count() - 1;
                if (i1 > -1)
                {
                    var TmpDataTable = dtResult.CopyToDataTable();
                    ConcurrentDictionary<string, int> CD = new ConcurrentDictionary<string, int>();
                    for (; i1 >= 0; i1--)
                    {
                        if (!CD.TryAdd((TmpDataTable.Rows[i1][0].ToString() + TmpDataTable.Rows[i1][1].ToString()), 0))
                        {
                            TmpDataTable.Rows.RemoveAt(i1);
                        }
                    }
                    i1 = TmpDataTable.Rows.Count;
                    if (i1 == 1)
                    {
                        string s1 = TmpDataTable.Rows[0][1].ToString();
                        textBox2.Text = s1;
                        if (s1.Length < 2000)
                        {
                            toolTip1.SetToolTip(textBox2, s1);
                        }
                        if (SD.Visible && MultiTrabl)
                        {
                            SD.Hide();
                        }
                    }
                    else
                    {
                        Control.CheckForIllegalCrossThreadCalls = false;
                        SD.SearchDictionaryDataTable1.Rows.Clear();
                        SD.dataGridView1.DataSource = null;
                        SD.dataGridView2.DataSource = null;
                        bool bl = true;
                        for (int i = 0; i < i1; i++)
                        {
                            SD.SearchDictionaryDataTable1.Rows.Add(TmpDataTable.Rows[i].ItemArray);
                            if (bl && s3 == TmpDataTable.Rows[i][0].ToString())
                            {
                                string s1 = TmpDataTable.Rows[i][1].ToString();
                                textBox2.Text = s1;
                                if (s1.Length < 2000)
                                {
                                    toolTip1.SetToolTip(textBox2, s1);
                                }
                                bl = false;
                            }
                        }
                        if (bl)
                        {
                            string s1 = TmpDataTable.Rows[0][1].ToString();
                            textBox2.Text = s1;
                            if (s1.Length < 2000)
                            {
                                toolTip1.SetToolTip(textBox2, s1);
                            }
                        }
                        SD.dataGridView1.DataSource = SD.dataSet1;
                        SD.dataGridView2.DataSource = SD.dataSet1;
                        SD.dataGridView1.DataMember = SD.SearchDictionaryDataTable1.TableName;
                        SD.dataGridView2.DataMember = SD.SearchDictionaryDataTable1.TableName;
                        if (SD.Visible == false && MultiTrabl)
                        {
                            Point P = new Point(this.Location.X + this.Width - SD.Width - 2, this.Location.Y + this.Height - SD.Height - 2);
                            SD.Location = P;
                            SD.Show();
                        }
                    }
                }
                else if (SD.Visible && MultiTrabl)
                {
                    SD.Hide();
                }
            }
        }

        private string Backup()//备份工程
        {
            string p = Path.GetFileNameWithoutExtension(ProjectFileName);
            if (Directory.Exists(CDirectory + "工程\\备份\\") == false)
            {
                Directory.CreateDirectory(CDirectory + "工程\\备份");
            }
            p = CDirectory + "工程\\备份\\" + p.Substring(0, p.Length - 10) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
            if (File.Exists(p) == false)
            {
                File.Copy(ProjectFileName, p);
                wt = DateTime.Now.Minute;
                p = "";
                return p;
            }
            else
            {
                return p;
            }
        }

        private void ClearText()//清空文本内容
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            toolStripTextBox4.Clear();
            toolStripTextBox5.Clear();
            toolTip1.RemoveAll();
        }

        private bool OpeningProjectBL = false;

        private void OpenProject()//打开工程
        {
            //dataTable1 工程
            //dataTable4 工程属性信息表。
            //dataTable5 新的挪移地址
            //dataTable6 字典
            //
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (SD.Visible)
            {
                SD.Hide();
            }
            if (File.Exists(ProjectFileName) == true)
            {
                if (OpeningProjectBL == false)
                {
                    OpeningProjectBL = true;
                    dataSet1.Clear();
                    NewPro = false;
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (DataTable TmpDT = MyAccess.GetSchema("Tables"))
                        {
                            ArrayList TmpDTAL = new ArrayList();
                            int iTmp = TmpDT.Rows.Count;
                            for (int i = 0; i < iTmp; i++)
                            {
                                TmpDTAL.Add(TmpDT.Rows[i][2].ToString());
                            }
                            if (!(TmpDTAL.Contains("athenaa") && TmpDTAL.Contains("calladd") && TmpDTAL.Contains("matrix")))
                            {
                                ProjectFileName = "";
                                MessageBox.Show("指定的文件不是该程序创建的工程文件，无法打开。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                OpeningProjectBL = false;
                            }
                            else
                            {
                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                {
                                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                                    {
                                        ClearText();
                                        if (TmpDTAL.Contains("IgnoreTra") == false)
                                        {
                                            cmd.CommandText = "CREATE TABLE `IgnoreTra` ("
                                                + "`address`	TEXT NOT NULL,"
                                                + "`ignorebl`	INTEGER DEFAULT 0"
                                                + ");";
                                            cmd.ExecuteNonQuery();
                                        }
                                        cmd.CommandText = "select count(address) from athenaa";
                                        object addcount = cmd.ExecuteScalar();
                                        int myaddcount = int.Parse(addcount.ToString());
                                        if (myaddcount == 0)
                                        {
                                            this.Text = "Athena-A";
                                            ProjectFileName = "";
                                            panel1.Enabled = false;
                                            textBox4.Enabled = false;
                                            textBox5.Enabled = false;
                                            textBox6.Enabled = false;
                                            dataGridView1.Enabled = false;
                                            生成目标BToolStripMenuItem.Enabled = false;
                                            自动翻译ToolStripMenuItem.Enabled = false;
                                            导出字典ToolStripMenuItem.Enabled = false;
                                            查找字体FToolStripMenuItem.Enabled = false;
                                            再次查找GToolStripMenuItem.Enabled = false;
                                            运行UToolStripMenuItem.Enabled = false;
                                            生成并运行LToolStripMenuItem.Enabled = false;
                                            清除运行路径ToolStripMenuItem.Enabled = false;
                                            属性ToolStripMenuItem.Enabled = false;
                                            统计SToolStripMenuItem.Enabled = false;
                                            delphi代码页DToolStripMenuItem.Enabled = false;
                                            添加过滤FToolStripMenuItem.Enabled = false;
                                            矩阵功能LToolStripMenuItem.Enabled = false;
                                            替换PToolStripMenuItem.Enabled = false;
                                            标记地址CToolStripMenuItem.Enabled = false;
                                            删除标记DToolStripMenuItem.Enabled = false;
                                            转到标记GToolStripMenuItem.Enabled = false;
                                            滚到标记OToolStripMenuItem.Enabled = false;
                                            下一个翻译NToolStripMenuItem.Enabled = false;
                                            查找HToolStripMenuItem.Enabled = false;
                                            查找多译TToolStripMenuItem.Enabled = false;
                                            忽略翻译LToolStripMenuItem.Enabled = false;
                                            删除忽略EToolStripMenuItem.Enabled = false;
                                            显示所有忽略SToolStripMenuItem.Enabled = false;
                                            导出EToolStripMenuItem.Enabled = false;
                                            导入IToolStripMenuItem.Enabled = false;
                                            toolStripTextBox4.BackColor = System.Drawing.SystemColors.Control;
                                            toolStripTextBox5.BackColor = System.Drawing.SystemColors.Control;
                                            label1.BackColor = System.Drawing.SystemColors.Control;
                                            label2.BackColor = System.Drawing.SystemColors.Control;
                                            tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Control;
                                            textBox4.BackColor = System.Drawing.SystemColors.Control;
                                            textBox5.BackColor = System.Drawing.SystemColors.Control;
                                            textBox6.BackColor = System.Drawing.SystemColors.Control;
                                            MessageBox.Show("工程文件中没有任何数据。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            OpeningProjectBL = false;
                                        }
                                        else
                                        {
                                            panel1.Enabled = false;
                                            dataGridView1.DataSource = null;
                                            if (myaddcount > 40000)
                                            {
                                                SPB = true;
                                                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                                toolStripProgressBar1.Visible = true;
                                                ProgressBarTimer.Enabled = true;
                                            }
                                            cmd.CommandText = "select * from pesec";
                                            using (DataTable pesecDT = new DataTable("Pesec"))
                                            {
                                                ad.Fill(pesecDT);//PE 表头
                                                if (pesecDT.Rows.Count > 0)
                                                {
                                                    PEbool = true;
                                                    矩阵功能LToolStripMenuItem.Enabled = true;
                                                    l1.Clear();
                                                    l2.Clear();
                                                    l3.Clear();
                                                    l4.Clear();
                                                    for (int x = 0; x < pesecDT.Rows.Count; x++)
                                                    {
                                                        l1.Add(pesecDT.Rows[x][0].ToString());
                                                        l2.Add(pesecDT.Rows[x][1].ToString());
                                                        l3.Add(pesecDT.Rows[x][2].ToString());
                                                        l4.Add(pesecDT.Rows[x][3].ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    PEbool = false;
                                                    矩阵功能LToolStripMenuItem.Enabled = false;
                                                }
                                            }
                                            cmd.CommandText = "select * from fileinfo";
                                            ad.Fill(dataTable4);//dataTable4 工程属性信息表。
                                            DictionaryStr = dataTable4.Rows[6][1].ToString();
                                            if (DictionaryStr.Contains(":\\") == false && DictionaryStr != "")
                                            {
                                                DictionaryStr = CDirectory + "字典\\" + DictionaryStr;
                                            }
                                            BackgroundWorker OpenProjectBackgroundWorker = new BackgroundWorker();
                                            OpenProjectBackgroundWorker.DoWork += OpenProjectBackgroundWorker_DoWork;
                                            OpenProjectBackgroundWorker.RunWorkerCompleted += OpenProjectBackgroundWorker_RunWorkerCompleted;
                                            OpenProjectBackgroundWorker.RunWorkerAsync();
                                            cmd.CommandText = "select * from calladd";
                                            ad.Fill(dataTable5);//dataTable5 新的挪移地址
                                            cmd.CommandText = "select * from prolanguage";
                                            using (DataTable prolanguageDT = new DataTable("prolanguage"))
                                            {
                                                ad.Fill(prolanguageDT);//代码页显示的字体等
                                                ProEncoding = prolanguageDT.Rows[0][0].ToString();
                                                ProOrgCode = int.Parse(prolanguageDT.Rows[0][2].ToString());
                                                ProTraName = prolanguageDT.Rows[0][5].ToString();
                                                ProTraCode = int.Parse(prolanguageDT.Rows[0][6].ToString());
                                                ProjectFont.ProjectOrgName = prolanguageDT.Rows[0][3].ToString();
                                                ProjectFont.ProjectOrgSize = prolanguageDT.Rows[0][4].ToString();
                                                ProjectFont.ProjectTraName = prolanguageDT.Rows[0][7].ToString();
                                                ProjectFont.ProjectTraSize = prolanguageDT.Rows[0][8].ToString();
                                            }
                                            if (ProjectFont.ProjectOrgName != "")
                                            {
                                                Font temfont = new Font(ProjectFont.ProjectOrgName, float.Parse(ProjectFont.ProjectOrgSize));
                                                orgDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont;
                                                textBox3.Font = temfont;
                                                if (toolStripComboBox2.Text == "原文")
                                                {
                                                    toolStripTextBox2.Font = temfont;
                                                }
                                            }
                                            else
                                            {
                                                ProjectFont.ProjectOrgName = this.Font.Name;
                                                ProjectFont.ProjectOrgSize = this.Font.Size.ToString();
                                                orgDataGridViewTextBoxColumn.DefaultCellStyle.Font = this.Font;
                                                textBox3.Font = this.Font;
                                                if (toolStripComboBox2.Text == "原文")
                                                {
                                                    toolStripTextBox2.Font = this.Font;
                                                }
                                            }
                                            if (ProjectFont.ProjectTraName != "")
                                            {
                                                Font temfont = new Font(ProjectFont.ProjectTraName, float.Parse(ProjectFont.ProjectTraSize));
                                                traDataGridViewTextBoxColumn.DefaultCellStyle.Font = temfont;
                                                textBox1.Font = temfont;
                                                textBox2.Font = temfont;
                                                if (toolStripComboBox2.Text == "译文")
                                                {
                                                    toolStripTextBox2.Font = temfont;
                                                }
                                            }
                                            else
                                            {
                                                ProjectFont.ProjectTraName = this.Font.Name;
                                                ProjectFont.ProjectTraSize = this.Font.Size.ToString();
                                                traDataGridViewTextBoxColumn.DefaultCellStyle.Font = this.Font;
                                                textBox1.Font = this.Font;
                                                textBox2.Font = this.Font;
                                                if (toolStripComboBox2.Text == "译文")
                                                {
                                                    toolStripTextBox2.Font = this.Font;
                                                }
                                            }
                                            this.Text = "Athena-A - " + ProjectFileName;
                                            wt = File.GetLastWriteTime(ProjectFileName).Minute;
                                            FilePath = dataTable4.Rows[0][1].ToString();
                                            if (FilePath.Contains(":\\") == false && FilePath != "")
                                            {
                                                FilePath = CDirectory + "工程\\文件\\" + FilePath;
                                            }
                                            if (File.Exists(FilePath) == true)
                                            {
                                                FPBool = true;//提取的原始文件是否存在
                                            }
                                            else
                                            {
                                                FPBool = false;
                                            }
                                            FileSize = dataTable4.Rows[2][1].ToString();
                                            TargetPath = dataTable4.Rows[3][1].ToString();
                                            if (TargetPath.Contains(":\\") == false && TargetPath != "")
                                            {
                                                TargetPath = CDirectory + "工程\\文件\\" + TargetPath;
                                            }
                                            UnPEType = dataTable4.Rows[4][1].ToString();
                                            Marker = dataTable4.Rows[7][1].ToString();
                                            StrLenCategory = dataTable4.Rows[9][1].ToString();
                                            DelphiCodePage = dataTable4.Rows[10][1].ToString();
                                            //StrLenCategory 字符串长度标识
                                            //ProEncoding 字符串编码标识
                                            if (StrLenCategory == "Delphi")
                                            {
                                                delphi代码页DToolStripMenuItem.Enabled = true;
                                            }
                                            else
                                            {
                                                delphi代码页DToolStripMenuItem.Enabled = false;
                                            }
                                            toolStripComboBox3.Items.Clear();
                                            toolStripComboBox3.Items.Add("超长");
                                            toolStripComboBox3.Text = "超长";
                                            if (UnPEType == "1" || UnPEType == "2" || UnPEType == "5" || UnPEType == "6")
                                            {
                                                toolStripButton3.Enabled = false;
                                                toolStripComboBox3.Enabled = false;
                                            }
                                            else
                                            {
                                                toolStripButton3.Enabled = true;
                                                toolStripComboBox3.Enabled = true;
                                                if (UnPEType == "3" || UnPEType == "4")
                                                {
                                                    toolStripComboBox3.Items.Add("超写");
                                                }
                                                else
                                                {
                                                    toolStripComboBox3.Items.Add("挪移");
                                                    toolStripComboBox3.Items.Add("迁移");
                                                    toolStripComboBox3.Items.Add("前移");
                                                    toolStripComboBox3.Items.Add("后移");
                                                    toolStripComboBox3.Items.Add("超写");
                                                    toolStripComboBox3.Items.Add("矩阵");
                                                }
                                            }
                                            if (UnPEType == "5")
                                            {
                                                toolStripButton15.Enabled = false;
                                                toolStripButton16.Enabled = false;
                                                toolStripButton21.Enabled = false;
                                                toolStripButton23.Enabled = false;
                                                toolStripButton24.Enabled = false;
                                                添加过滤FToolStripMenuItem.Enabled = false;
                                            }
                                            else
                                            {
                                                toolStripButton15.Enabled = true;
                                                toolStripButton16.Enabled = true;
                                                toolStripButton21.Enabled = true;
                                                toolStripButton23.Enabled = true;
                                                toolStripButton24.Enabled = true;
                                                添加过滤FToolStripMenuItem.Enabled = true;
                                            }
                                            if (UnPEType == "6")
                                            {
                                                toolStripButton10.Enabled = false;
                                                toolStripButton15.Enabled = false;
                                                toolStripButton16.Enabled = false;
                                                toolStripButton21.Enabled = false;
                                                toolStripButton23.Enabled = false;
                                                toolStripButton24.Enabled = false;
                                                添加过滤FToolStripMenuItem.Enabled = false;
                                                splitContainer1.SplitterDistance = 0;
                                                hexHToolStripMenuItem.Enabled = false;
                                                导出字典ToolStripMenuItem.Enabled = false;
                                            }
                                            else
                                            {
                                                toolStripButton10.Enabled = true;
                                                toolStripButton15.Enabled = true;
                                                toolStripButton16.Enabled = true;
                                                toolStripButton21.Enabled = true;
                                                toolStripButton23.Enabled = true;
                                                toolStripButton24.Enabled = true;
                                                添加过滤FToolStripMenuItem.Enabled = true;
                                                if (hexHToolStripMenuItem.Checked == false)
                                                {
                                                    splitContainer1.SplitterDistance = 0;
                                                }
                                                else
                                                {
                                                    splitContainer1.SplitterDistance = spl2;
                                                }
                                                hexHToolStripMenuItem.Enabled = true;
                                                导出字典ToolStripMenuItem.Enabled = true;
                                            }
                                            CPUType = dataTable4.Rows[8][1].ToString();
                                            runtest = "";
                                            if (File.Exists(dataTable4.Rows[5][1].ToString()) == true)
                                            {
                                                runtest = dataTable4.Rows[5][1].ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("没有找到工程文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenProjectBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Parallel.Invoke(() =>
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                            ad.Fill(dataTable1);//dataTable1 工程
                        }
                    }
                }
            },
            () =>
            {
                if (File.Exists(DictionaryStr))
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + DictionaryStr))
                    {
                        MyAccess.Open();
                        using (DataTable TmpDT = MyAccess.GetSchema("Tables"))
                        {
                            if (TmpDT.Rows[0][2].ToString() == "diclanguage")
                            {
                                dataTable6.Clear();
                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                {
                                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                                    {
                                        cmd.CommandText = "select org,tra from tbl";
                                        ad.Fill(dataTable6);
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private void OpenProjectBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox4.Enabled = true;
            textBox5.Enabled = true;
            textBox6.Enabled = true;
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                RowEnterDis(0);//进入一行
            }
            panel1.Enabled = true;
            dataGridView1.Enabled = true;
            生成目标BToolStripMenuItem.Enabled = true;
            自动翻译ToolStripMenuItem.Enabled = true;
            导出字典ToolStripMenuItem.Enabled = true;
            查找字体FToolStripMenuItem.Enabled = true;
            再次查找GToolStripMenuItem.Enabled = true;
            运行UToolStripMenuItem.Enabled = true;
            生成并运行LToolStripMenuItem.Enabled = true;
            清除运行路径ToolStripMenuItem.Enabled = true;
            属性ToolStripMenuItem.Enabled = true;
            统计SToolStripMenuItem.Enabled = true;
            替换PToolStripMenuItem.Enabled = true;
            标记地址CToolStripMenuItem.Enabled = true;
            删除标记DToolStripMenuItem.Enabled = true;
            转到标记GToolStripMenuItem.Enabled = true;
            滚到标记OToolStripMenuItem.Enabled = true;
            下一个翻译NToolStripMenuItem.Enabled = true;
            查找HToolStripMenuItem.Enabled = true;
            if (dataTable6.Rows.Count > 0)
            {
                查找多译TToolStripMenuItem.Enabled = true;
            }
            else
            {
                查找多译TToolStripMenuItem.Enabled = false;
            }
            忽略翻译LToolStripMenuItem.Enabled = true;
            删除忽略EToolStripMenuItem.Enabled = true;
            显示所有忽略SToolStripMenuItem.Enabled = true;
            导出EToolStripMenuItem.Enabled = true;
            导入IToolStripMenuItem.Enabled = true;
            toolStripTextBox4.ReadOnly = true;
            toolStripTextBox5.ReadOnly = true;
            toolStripTextBox4.BackColor = System.Drawing.SystemColors.Window;
            toolStripTextBox5.BackColor = System.Drawing.SystemColors.Window;
            label1.BackColor = System.Drawing.SystemColors.Window;
            label2.BackColor = System.Drawing.SystemColors.Window;
            tableLayoutPanel2.BackColor = System.Drawing.SystemColors.Window;
            textBox4.BackColor = System.Drawing.SystemColors.Window;
            textBox5.BackColor = System.Drawing.SystemColors.Window;
            textBox6.BackColor = System.Drawing.SystemColors.Window;
            if (dataTable6.Rows.Count == 0)
            {
                textBox2.Enabled = false;
            }
            else
            {
                textBox2.Enabled = true;
            }
            this.Activate();
            OpenRecPro();
            OpeningProjectBL = false;
        }

        private void OpenRecPro()//打开最近的工程
        {
            CommonCode.SetupFolder();
            string s1 = CDirectory + "配置\\RecentProject.ini";
            if (File.Exists(s1) == false)
            {
                using (StreamWriter sw1 = File.CreateText(s1))
                {
                    //
                }
            }
            ArrayList al = new ArrayList();
            if (ProjectFileName != "")
            {
                al.Add(ProjectFileName.Replace(CDirectory + "工程\\", ""));
            }
            string s2 = "";
            using (StreamReader sr = File.OpenText(s1))//打开配置文件
            {
                while ((s2 = sr.ReadLine()) != null)//判定是否是最后一行
                {
                    if (s2.Contains(":\\") == false && s2 != "")
                    {
                        s2 = CDirectory + "工程\\" + s2;
                    }
                    if (s2 != ProjectFileName && File.Exists(s2) == true)
                    {
                        al.Add(s2.Replace(CDirectory + "工程\\", ""));
                    }
                }
            }
            最近的工程ToolStripMenuItem.DropDownItems.Clear();
            for (int i = 0; i < al.Count; i++)
            {
                ToolStripMenuItem men = new ToolStripMenuItem(al[i].ToString());
                最近的工程ToolStripMenuItem.DropDownItems.Insert(i, men);
                men.Click += new EventHandler(men_Click);
            }
            using (StreamWriter sw2 = new StreamWriter(s1, false))
            {
                for (int i = 0; i < al.Count; i++)
                {
                    sw2.WriteLine(al[i].ToString().Replace(CDirectory + "工程\\", ""));
                }
            }
        }

        private void men_Click(object sender, EventArgs e)
        {
            ProjectFileName = sender.ToString();
            if (ProjectFileName.Contains(":\\") == false)
            {
                ProjectFileName = CDirectory + "工程\\" + ProjectFileName;
            }
            OpenProject();
        }

        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)//打开关于窗口
        {
            About a1 = new About();
            if (MyDpi > 96F)
            {
                if (MyDpi <= 240F)
                {
                    a1.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                else
                {
                    a1.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                a1.pictureBox1.Image = Athena_A.Properties.Resources.Athena_A256;
            }
            a1.ShowDialog();
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)//退出程序
        {
            if (BackupProjectTaskbool == true || FilterProjectBackgroundWorker.IsBusy == true || AddFilterstringBackgroundWorker.IsBusy == true)
            {
                DialogResult dr = MessageBox.Show("程序正忙，确实要退出程序吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        static int spl2 = 0;//十六进制显示介面宽度
        private static int toolSPB1Left = 0;
        public static float MyDpi = 96F;
        public static double MyFontScale = 1D;
        public static Font MyNewFont = new Font("微软雅黑", 9F);

        private void mainform_Load(object sender, EventArgs e)//main 窗口载入时的事件，一般用来初始化一些数据
        {
            AA_Default_Encoding = CodePagesEncodingProvider.Instance.GetEncoding(0);
            bool blTmp = false;
            CDirectory = Directory.GetCurrentDirectory(); //获得当前程序所在目录
            if (CDirectory.Length > 3)
            {
                CDirectory = CDirectory + "\\";
            }
            try
            {
                string strTmp = CDirectory + "配置\\";
                if (Directory.Exists(strTmp) == false)
                {
                    Directory.CreateDirectory(strTmp);
                }
                strTmp = CDirectory + "配置\\RecentProjectTmp.ini";
                using (StreamWriter sw1 = File.CreateText(strTmp))
                { }
                File.Delete(strTmp);
                blTmp = true;
            }
            catch
            {
                WindowsPrincipal WP = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                if (WP.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("访问配置文件失败，你可能没有访问此文件的权限。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("访问配置文件失败，你可能需要以管理员权限运行此程序。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Application.Exit();
            }
            if (blTmp)
            {
                this.Opacity = 1;
                splitContainer1.Panel1.Font = new Font("SimSun", 9F);
                using (Graphics gh = Graphics.FromHwnd(IntPtr.Zero))
                {
                    MyDpi = gh.DpiX;
                    if (MyDpi > 96F)
                    {
                        label1.Font = MyNewFont;
                        this.Font = MyNewFont;
                        MyFontScale = this.FontHeight / 16D;
                        dataGridView1.RowTemplate.Height = (int)(dataGridView1.RowTemplate.Height * MyFontScale);
                        SD.Font = MyNewFont;
                        SD.dataGridView1.RowTemplate.Height = (int)(SD.dataGridView1.RowTemplate.Height * MyFontScale);
                        SD.dataGridView2.RowTemplate.Height = (int)(SD.dataGridView2.RowTemplate.Height * MyFontScale);
                    }
                    else
                    {
                        Font MyFont = new Font("SimSun", 9F);
                        this.Font = MyFont;
                        menuStrip1.Font = MyFont;
                        contextMenuStrip1.Font = MyFont;
                        tableLayoutPanel1.Font = MyFont;
                    }
                    addressDataGridViewTextBoxColumn.Width = (int)gh.MeasureString("DDDDDDDD", this.Font).Width;
                    toolStripTextBox1.Width = (int)gh.MeasureString("DDDDD1", this.Font).Width;
                    toolStripTextBox4.Width = toolStripTextBox1.Width;
                    toolStripTextBox5.Width = toolStripTextBox1.Width;
                    toolStripTextBox6.Width = (int)gh.MeasureString("DDDDDDDDDDDDDDDDD", this.Font).Width;
                    string tmpStr = "D";
                    float f1;
                    float f2;
                    float f3;
                    if (MyDpi == 96F)
                    {
                        for (int x = 0; x < 7; x++)
                        {
                            tmpStr += "D";
                        }
                        f1 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width;
                        for (int x = 0; x < 8; x++)
                        {
                            tmpStr += "D";
                        }
                        f3 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width + 7;
                        for (int x = 0; x < 31; x++)
                        {
                            tmpStr += "D";
                        }
                        f2 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width;
                    }
                    else
                    {
                        for (int x = 0; x < 7; x++)
                        {
                            tmpStr += "D";
                        }
                        f1 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width + 5;
                        for (int x = 0; x < 8; x++)
                        {
                            tmpStr += "D";
                        }
                        f3 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width + 10;
                        for (int x = 0; x < 32; x++)
                        {
                            tmpStr += "D";
                        }
                        f2 = gh.MeasureString(tmpStr, splitContainer1.Panel1.Font).Width + 7;
                    }
                    textBox4.Width = (int)f1;
                    textBox5.Width = (int)f2;
                    textBox6.Width = (int)f3;
                    spl2 = (int)(f1 + f2 + f3) + textBox5.Margin.Left * 6;
                    splitContainer1.SplitterDistance = spl2;
                    if (MyDpi >= 144F && MyDpi < 192F)
                    {
                        toolStripButton1.Image = Athena_A.Properties.Resources.Search241;
                        toolStripButton2.Image = Athena_A.Properties.Resources.Filter24;
                        toolStripButton3.Image = Athena_A.Properties.Resources.Extended24;
                        toolStripButton4.Image = Athena_A.Properties.Resources.Paste24;
                        toolStripButton5.Image = Athena_A.Properties.Resources.Search242;
                        toolStripButton6.Image = Athena_A.Properties.Resources.NoCase24;
                        toolStripButton7.Image = Athena_A.Properties.Resources.Cute24;
                        toolStripButton8.Image = Athena_A.Properties.Resources.Copy24;
                        toolStripButton9.Image = Athena_A.Properties.Resources.Paste24;
                        toolStripButton10.Image = Athena_A.Properties.Resources.Restore24;
                        toolStripButton11.Image = Athena_A.Properties.Resources.Save24;
                        toolStripButton12.Image = Athena_A.Properties.Resources.Backup24;
                        toolStripButton13.Image = Athena_A.Properties.Resources.Dictionary24;
                        toolStripButton14.Image = Athena_A.Properties.Resources.Start24;
                        toolStripButton15.Image = Athena_A.Properties.Resources.Backward24;
                        toolStripButton16.Image = Athena_A.Properties.Resources.Forward24;
                        toolStripButton17.Image = Athena_A.Properties.Resources.End24;
                        toolStripButton18.Image = Athena_A.Properties.Resources.Run24;
                        toolStripButton19.Image = Athena_A.Properties.Resources.BuildAndRun24;
                        toolStripButton20.Image = Athena_A.Properties.Resources.Default24;
                        toolStripButton21.Image = Athena_A.Properties.Resources.FilterProject24;
                        toolStripButton22.Image = Athena_A.Properties.Resources.Copy24;
                        toolStripButton23.Image = Athena_A.Properties.Resources.Space24;
                        toolStripButton24.Image = Athena_A.Properties.Resources.DeleteSpace24;
                        toolStripButton25.Image = Athena_A.Properties.Resources.Info24;
                        运行UToolStripMenuItem.Image = Athena_A.Properties.Resources.Run24;
                        添加过滤FToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject24;
                        编辑过滤FToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject24;
                        删除ToolStripMenuItem.Image = Athena_A.Properties.Resources.Delete24;
                        添加到字典ToolStripMenuItem.Image = Athena_A.Properties.Resources.Dictionary24;
                        添加到过滤器ToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject24;
                        复制地址ToolStripMenuItem.Image = Athena_A.Properties.Resources.Copy24;
                        新建工程NToolStripMenuItem.Image = Athena_A.Properties.Resources.NewProject24;
                        打开工程OToolStripMenuItem.Image = Athena_A.Properties.Resources.OpenProject24;
                        属性ToolStripMenuItem.Image = Athena_A.Properties.Resources.Properties24;
                        编辑矩阵MToolStripMenuItem.Image = Athena_A.Properties.Resources.Matrix24;
                    }
                    else if (MyDpi >= 192F)
                    {
                        toolStripButton1.Image = Athena_A.Properties.Resources.Search321;
                        toolStripButton2.Image = Athena_A.Properties.Resources.Filter32;
                        toolStripButton3.Image = Athena_A.Properties.Resources.Extended32;
                        toolStripButton4.Image = Athena_A.Properties.Resources.Paste32;
                        toolStripButton5.Image = Athena_A.Properties.Resources.Search322;
                        toolStripButton6.Image = Athena_A.Properties.Resources.NoCase32;
                        toolStripButton7.Image = Athena_A.Properties.Resources.Cute32;
                        toolStripButton8.Image = Athena_A.Properties.Resources.Copy32;
                        toolStripButton9.Image = Athena_A.Properties.Resources.Paste32;
                        toolStripButton10.Image = Athena_A.Properties.Resources.Restore32;
                        toolStripButton11.Image = Athena_A.Properties.Resources.Save32;
                        toolStripButton12.Image = Athena_A.Properties.Resources.Backup32;
                        toolStripButton13.Image = Athena_A.Properties.Resources.Dictionary32;
                        toolStripButton14.Image = Athena_A.Properties.Resources.Start32;
                        toolStripButton15.Image = Athena_A.Properties.Resources.Backward32;
                        toolStripButton16.Image = Athena_A.Properties.Resources.Forward32;
                        toolStripButton17.Image = Athena_A.Properties.Resources.End32;
                        toolStripButton18.Image = Athena_A.Properties.Resources.Run32;
                        toolStripButton19.Image = Athena_A.Properties.Resources.BuildAndRun32;
                        toolStripButton20.Image = Athena_A.Properties.Resources.Default32;
                        toolStripButton21.Image = Athena_A.Properties.Resources.FilterProject32;
                        toolStripButton22.Image = Athena_A.Properties.Resources.Copy32;
                        toolStripButton23.Image = Athena_A.Properties.Resources.Space32;
                        toolStripButton24.Image = Athena_A.Properties.Resources.DeleteSpace32;
                        toolStripButton25.Image = Athena_A.Properties.Resources.Info32;
                        运行UToolStripMenuItem.Image = Athena_A.Properties.Resources.Run32;
                        添加过滤FToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject32;
                        编辑过滤FToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject32;
                        删除ToolStripMenuItem.Image = Athena_A.Properties.Resources.Delete32;
                        添加到字典ToolStripMenuItem.Image = Athena_A.Properties.Resources.Dictionary32;
                        添加到过滤器ToolStripMenuItem.Image = Athena_A.Properties.Resources.FilterProject32;
                        复制地址ToolStripMenuItem.Image = Athena_A.Properties.Resources.Copy32;
                        新建工程NToolStripMenuItem.Image = Athena_A.Properties.Resources.NewProject32;
                        打开工程OToolStripMenuItem.Image = Athena_A.Properties.Resources.OpenProject32;
                        属性ToolStripMenuItem.Image = Athena_A.Properties.Resources.Properties32;
                        编辑矩阵MToolStripMenuItem.Image = Athena_A.Properties.Resources.Matrix32;
                    }
                }
                toolStripComboBox1.Text = "小于";
                toolStripComboBox3.Text = "超长";
                toolStripComboBox2.Text = "原文";
                SearchHistoryBL = false;
                toolStripComboBox4.Text = "清除历史";
                SearchHistoryBL = true;
                int i1 = toolStrip1.Items.Count;
                double d1 = 0;
                for (int i = 0; i < i1; i++)
                {
                    if (toolStrip1.Items[i].Name != "toolStripProgressBar1")
                    {
                        d1 += toolStrip1.Items[i].Width;
                    }
                }
                toolSPB1Left = (int)(d1 + i1 * Math.Round(MyFontScale) + MyDpi / 96);
                toolStripProgressBar1.Height = toolStripComboBox3.Height;
                DeleteRowsbackgroundWorker.DoWork += DeleteRowsbackgroundWorker_DoWork;
                DeleteRowsbackgroundWorker.RunWorkerCompleted += DeleteRowsbackgroundWorker_RunWorkerCompleted;
                FilterProjectBackgroundWorker.DoWork += FilterProjectBackgroundWorker_DoWork;
                FilterProjectBackgroundWorker.RunWorkerCompleted += FilterProjectBackgroundWorker_RunWorkerCompleted;
                AddFilterstringBackgroundWorker.DoWork += AddFilterstringBackgroundWorker_DoWork;
                AddFilterstringBackgroundWorker.RunWorkerCompleted += AddFilterstringBackgroundWorker_RunWorkerCompleted;
                ImportBackgroundWorker.DoWork += ImportBackgroundWorker_DoWork;
                ImportBackgroundWorker.RunWorkerCompleted += ImportBackgroundWorker_RunWorkerCompleted;
                OpenRecPro();
            }
        }

        private void BackupTask()
        {
            if (DateTime.Now.Minute < wt || (DateTime.Now.Minute - wt) > 10)
            {
                if (File.Exists(ProjectFileName))
                {
                    BackupProjectTaskbool = true;
                    Backup();
                    BackupProjectTaskbool = false;
                }
            }
        }

        private void hexHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (hexHToolStripMenuItem.Checked == false)
            {
                hexHToolStripMenuItem.Checked = true;
                hexToolMenuChecked = true;
            }
            else
            {
                hexHToolStripMenuItem.Checked = false;
                hexToolMenuChecked = false;
            }
            if (hexHToolStripMenuItem.Checked == false)
            {
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                splitContainer1.SplitterDistance = 0;
            }
            else
            {
                splitContainer1.SplitterDistance = spl2;
            }
        }

        private void 打开工程OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = CDirectory + "工程";
            open.Filter = "Athena-A 工程文件(*.ena)|*.ena";
            if (open.ShowDialog() == DialogResult.OK)
            {
                ProjectFileName = open.FileName;
                OpenProject();
            }
        }

        private void 新建工程NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewProject NP = new NewProject();
            if (MyDpi > 96F)
            {
                NP.Font = MyNewFont;
                NP.dataGridView1.RowTemplate.Height = (int)(NP.dataGridView1.RowTemplate.Height * MyFontScale);
            }
            NP.ShowDialog();
            this.Show();
            this.Activate();
            if (NewPro == true)
            {
                OpenProject();
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)//剪切文本
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(textBox1.Text);
            textBox1.Clear();
            textBox1.Focus();
        }

        private void toolStripButton22_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(textBox3.Text);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)//复制文本
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(textBox1.Text);
        }

        private void toolStripButton9_Click(object sender, EventArgs e)//粘贴文本
        {
            IDataObject cb = Clipboard.GetDataObject();
            if (cb.GetDataPresent(DataFormats.Text))
            {
                string s1 = (string)cb.GetData(DataFormats.Text);
                string s3 = textBox3.Text;
                if (s3.Contains("\r") == true && s3.Contains("\n") == false)
                {
                    s1 = s1.Replace("\r", "\r\n");
                }
                else if (s3.Contains("\r") == false && s3.Contains("\n") == true)
                {
                    s1 = s1.Replace("\n", "\r\n");
                }
                textBox1.Text = s1;
                textBox1.Focus();
                textBox1.DeselectAll();
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            string s = textBox3.Text;
            if (s.Contains("\r") == true && s.Contains("\n") == false)
            {
                s = s.Replace("\r", "\r\n");
            }
            else if (s.Contains("\r") == false && s.Contains("\n") == true)
            {
                s = s.Replace("\n", "\r\n");
            }
            textBox1.Text = s;
            textBox1.Focus();
            textBox1.DeselectAll();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)//粘贴十六进制值
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
                    toolStripTextBox3.Text = str1;
                }
            }
            else
            {
                MessageBox.Show("粘贴板中没有数据，或是其中的数据不是文本内容。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)//搜索字符串
        {
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                string s1 = toolStripTextBox2.Text;
                string com = toolStripComboBox2.Text;
                if (s1 == "")
                {
                    MessageBox.Show("请指定需要搜索的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripTextBox2.Focus();
                }
                else
                {
                    if (toolStripComboBox4.Items.Contains(s1) == false)
                    {
                        toolStripComboBox4.Items.Add(s1);
                    }
                    if (s1 == "换行符 \\n 转义")
                    {
                        s1 = "\n";
                    }
                    else if (s1 == "回车符 \\r 转义")
                    {
                        s1 = "\r";
                    }
                    else if (s1 == "制表符 \\t 转义")
                    {
                        s1 = "\t";
                    }
                    bool bl = false;
                    int i2 = dataGridView1.CurrentRow.Index + 1;
                    if (MatchCase == true)//大小写匹配
                    {
                        if (com == "原文")
                        {
                            for (int i = i2; i < i1; i++)
                            {
                                if (dataTable1.Rows[i][1].ToString().Contains(s1) == true)
                                {
                                    if (i > 9)
                                    {
                                        dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                    }
                                    dataGridView1.CurrentCell = dataGridView1[0, i];
                                    dataGridView1.CurrentCell.Selected = true;
                                    RowEnterDis(i);
                                    bl = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = i2; i < i1; i++)
                            {
                                if (dataTable1.Rows[i][2].ToString().Contains(s1) == true)
                                {
                                    if (i > 9)
                                    {
                                        dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                    }
                                    dataGridView1.CurrentCell = dataGridView1[0, i];
                                    dataGridView1.CurrentCell.Selected = true;
                                    RowEnterDis(i);
                                    bl = true;
                                    break;
                                }
                            }
                        }
                        if (bl == true)
                        {
                            return;
                        }
                        else
                        {
                            if (com == "原文")
                            {
                                for (int i = 0; i < i2; i++)
                                {
                                    if (dataTable1.Rows[i][1].ToString().Contains(s1) == true)
                                    {
                                        if (i > 9)
                                        {
                                            dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                        }
                                        dataGridView1.CurrentCell = dataGridView1[0, i];
                                        dataGridView1.CurrentCell.Selected = true;
                                        RowEnterDis(i);
                                        bl = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < i2; i++)
                                {
                                    if (dataTable1.Rows[i][2].ToString().Contains(s1) == true)
                                    {
                                        if (i > 9)
                                        {
                                            dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                        }
                                        dataGridView1.CurrentCell = dataGridView1[0, i];
                                        dataGridView1.CurrentCell.Selected = true;
                                        RowEnterDis(i);
                                        bl = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (bl == false)
                        {
                            MessageBox.Show("没有找到匹配的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        s1 = s1.ToLower();
                        if (com == "原文")
                        {
                            for (int i = i2; i < i1; i++)
                            {
                                if (dataTable1.Rows[i][1].ToString().ToLower().Contains(s1) == true)
                                {
                                    if (i > 9)
                                    {
                                        dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                    }
                                    dataGridView1.CurrentCell = dataGridView1[0, i];
                                    dataGridView1.CurrentCell.Selected = true;
                                    RowEnterDis(i);
                                    bl = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = i2; i < i1; i++)
                            {
                                if (dataTable1.Rows[i][2].ToString().ToLower().Contains(s1) == true)
                                {
                                    if (i > 9)
                                    {
                                        dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                    }
                                    dataGridView1.CurrentCell = dataGridView1[0, i];
                                    dataGridView1.CurrentCell.Selected = true;
                                    RowEnterDis(i);
                                    bl = true;
                                    break;
                                }
                            }
                        }
                        if (bl == true)
                        {
                            return;
                        }
                        else
                        {
                            if (com == "原文")
                            {
                                for (int i = 0; i < i2; i++)
                                {
                                    if (dataTable1.Rows[i][1].ToString().ToLower().Contains(s1) == true)
                                    {
                                        if (i > 9)
                                        {
                                            dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                        }
                                        dataGridView1.CurrentCell = dataGridView1[0, i];
                                        dataGridView1.CurrentCell.Selected = true;
                                        RowEnterDis(i);
                                        bl = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < i2; i++)
                                {
                                    if (dataTable1.Rows[i][2].ToString().ToLower().Contains(s1) == true)
                                    {
                                        if (i > 9)
                                        {
                                            dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                        }
                                        dataGridView1.CurrentCell = dataGridView1[0, i];
                                        dataGridView1.CurrentCell.Selected = true;
                                        RowEnterDis(i);
                                        bl = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (bl == false)
                        {
                            MessageBox.Show("没有找到匹配的字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)//筛选字符串
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show(ProjectFileName + "\r\n不存在，无法筛选。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string str1 = toolStripTextBox1.Text;//长度
                string str2 = toolStripTextBox2.Text;//内容
                if (str2 != "")
                {
                    if (toolStripComboBox4.Items.Contains(str2) == false)
                    {
                        toolStripComboBox4.Items.Add(str2);
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
                string com1 = toolStripComboBox1.Text;//长度
                string com2 = toolStripComboBox2.Text;//原文译文
                string FBLstr = "";
                bool AllBL = false;
                if (str1 == "")
                {
                    if (com2 == "原文")
                    {
                        if (str2 == "")
                        {
                            AllBL = true;
                            FBLstr = "select * from athenaa ORDER BY address ASC";
                        }
                        else
                        {
                            FBLstr = "select * from athenaa where org Like '%" + str2 + "%' ESCAPE '/' ORDER BY address ASC";
                        }
                    }
                    else
                    {
                        if (str2 == "")
                        {
                            FBLstr = "select * from athenaa where tralong > 0 ORDER BY address ASC";
                        }
                        else
                        {
                            FBLstr = "select * from athenaa where tralong > 0 and tra Like '%" + str2 + "%' ESCAPE '/' ORDER BY address ASC";
                        }
                    }
                }
                else
                {
                    if (com2 == "原文")
                    {
                        if (str2 == "")
                        {
                            if (com1 == "小于")
                            {
                                FBLstr = "select * from athenaa where orglong < " + u1 + " ORDER BY address ASC";
                            }
                            else if (com1 == "等于")
                            {
                                FBLstr = "select * from athenaa where orglong = " + u1 + " ORDER BY address ASC";
                            }
                            else
                            {
                                FBLstr = "select * from athenaa where orglong > " + u1 + " ORDER BY address ASC";
                            }
                        }
                        else
                        {
                            if (com1 == "小于")
                            {
                                FBLstr = "select * from athenaa where org Like '%" + str2 + "%' ESCAPE '/' and orglong < " + u1 + " ORDER BY address ASC";
                            }
                            else if (com1 == "等于")
                            {
                                FBLstr = "select * from athenaa where org Like '%" + str2 + "%' ESCAPE '/' and orglong = " + u1 + " ORDER BY address ASC";
                            }
                            else
                            {
                                FBLstr = "select * from athenaa where org Like '%" + str2 + "%' ESCAPE '/' and orglong > " + u1 + " ORDER BY address ASC";
                            }
                        }
                    }
                    else
                    {
                        if (str2 == "")
                        {
                            if (com1 == "小于")
                            {
                                FBLstr = "select * from athenaa where tralong < " + u1 + " and tralong > 0 ORDER BY address ASC";
                            }
                            else if (com1 == "等于")
                            {
                                FBLstr = "select * from athenaa where tralong = " + u1 + " ORDER BY address ASC";
                            }
                            else
                            {
                                FBLstr = "select * from athenaa where tralong > " + u1 + " ORDER BY address ASC";
                            }
                        }
                        else
                        {
                            if (com1 == "小于")
                            {
                                FBLstr = "select * from athenaa where tra Like '%" + str2 + "%' ESCAPE '/' and tralong < " + u1 + " and tralong > 0 ORDER BY address ASC";
                            }
                            else if (com1 == "等于")
                            {
                                FBLstr = "select * from athenaa where tra Like '%" + str2 + "%' ESCAPE '/' and tralong = " + u1 + " ORDER BY address ASC";
                            }
                            else
                            {
                                FBLstr = "select * from athenaa where tra Like '%" + str2 + "%' ESCAPE '/' and tralong > " + u1 + " ORDER BY address ASC";
                            }
                        }
                    }
                }
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select count(address) from athenaa";
                            object addcount = cmd.ExecuteScalar();
                            int myaddcount = int.Parse(addcount.ToString());
                            if (!(AllBL && myaddcount == dataTable1.Rows.Count))
                            {
                                dataTable1.Clear();
                                dataGridView1.DataSource = null;
                                if (myaddcount > 40000)
                                {
                                    DisabledPanelContrl();
                                    SPB = true;
                                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                    toolStripProgressBar1.Visible = true;
                                    ProgressBarTimer.Enabled = true;
                                }
                                BackgroundWorker FilterByKeyAndOrLongerBackgroundWorker = new BackgroundWorker();
                                FilterByKeyAndOrLongerBackgroundWorker.DoWork += FilterByKeyAndOrLongerBackgroundWorker_DoWork;
                                FilterByKeyAndOrLongerBackgroundWorker.RunWorkerCompleted += FilterByKeyAndOrLongerBackgroundWorker_RunWorkerCompleted;
                                FilterByKeyAndOrLongerBackgroundWorker.RunWorkerAsync(FBLstr);
                            }
                        }
                    }
                }
            }
        }

        private void FilterByKeyAndOrLongerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        if (MatchCase == true)
                        {
                            cmd.CommandText = "PRAGMA case_sensitive_like = 1";
                        }
                        else
                        {
                            cmd.CommandText = "PRAGMA case_sensitive_like = 0";
                        }
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = e.Argument.ToString();
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void FilterByKeyAndOrLongerBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            EnablePanelContrl();
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                RowEnterDis(0);
            }
            else
            {
                ClearText();
                MessageBox.Show("没有找到任何内容。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Activate();
        }

        private string SearchAddress = "";

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName))
            {
                SearchAddress = toolStripTextBox3.Text;
                if (SearchAddress == "")
                {
                    MessageBox.Show("请输入地址。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (CommonCode.Is_Hex(SearchAddress) == false)
                {
                    MessageBox.Show("输入的地址不是有效的十六进制值。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                            {
                                SearchAddress = CommonCode.FormatStrHex(SearchAddress);
                                cmd.CommandText = "select count(address) from athenaa";
                                object addcount = cmd.ExecuteScalar();
                                int myaddcount = int.Parse(addcount.ToString());
                                int i1 = dataTable1.Rows.Count;
                                if (myaddcount == i1)
                                {
                                    bool bl = false;
                                    Parallel.For(0, i1, (i, ParallelLoopState) =>
                                    {
                                        if (dataTable1.Rows[i][0].ToString() == SearchAddress)
                                        {
                                            ParallelLoopState.Break();
                                            if (i > 9)
                                            {
                                                dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                                            }
                                            dataGridView1.CurrentCell = dataGridView1[0, i];
                                            dataGridView1.CurrentCell.Selected = true;
                                            RowEnterDis(i);
                                            bl = true;
                                        }
                                    });
                                    if (bl == false)
                                    {
                                        MessageBox.Show("没有找到查找的地址。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    dataTable1.Clear();
                                    dataGridView1.DataSource = null;
                                    if (myaddcount > 40000)
                                    {
                                        DisabledPanelContrl();
                                        SPB = true;
                                        toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                        toolStripProgressBar1.Visible = true;
                                        ProgressBarTimer.Enabled = true;
                                    }
                                    BackgroundWorker SearchAddressBackgroundWorker = new BackgroundWorker();
                                    SearchAddressBackgroundWorker.DoWork += SearchAddressBackgroundWorker_DoWork;
                                    SearchAddressBackgroundWorker.RunWorkerCompleted += SearchAddressBackgroundWorker_RunWorkerCompleted;
                                    SearchAddressBackgroundWorker.RunWorkerAsync();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("工程文件不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void SearchAddressBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void SearchAddressBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            EnablePanelContrl();
            if (i1 > 0)
            {
                int i2 = -1;
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == SearchAddress)
                        {
                            i2 = i;
                        }
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                else
                {
                    Parallel.For(0, i1, (i, ParallelLoopState) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == SearchAddress)
                        {
                            ParallelLoopState.Break();
                            i2 = i;
                        }
                    });
                }
                if (i2 == -1)
                {
                    MessageBox.Show("没有找到查找的地址。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (i2 > 9)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = i2 - 10;
                    }
                    dataGridView1.CurrentCell = dataGridView1[0, i2];
                    dataGridView1.CurrentCell.Selected = true;
                    RowEnterDis(i2);
                }
            }
            this.Activate();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)//第一行
        {
            if (dataTable1.Rows.Count > 0)
            {
                dataGridView1.CurrentCell = dataGridView1[0, 0];
                dataGridView1.CurrentCell.Selected = true;
                RowEnterDis(0);
            }
        }

        private void toolStripButton17_Click(object sender, EventArgs e)//最后一行
        {
            int i = dataTable1.Rows.Count - 1;
            if (i >= 0)
            {
                dataGridView1.CurrentCell = dataGridView1[0, i];
                dataGridView1.CurrentCell.Selected = true;
                RowEnterDis(i);
            }
        }

        public static void MoveAddressHexView(string s1, int tem)
        {
            if (hexToolMenuChecked)
            {
                if (FPBool)
                {
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    int x = (int)CommonCode.HexToLong(s1);
                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            a = (int)Math.Floor((double)(Application.OpenForms[0].Controls[0].Height / 12 - 2));//面板能显示的行数
                            b = (int)Math.Floor((double)(fs.Length / 16));//文件能显示的行数
                            d = (int)Math.Floor((double)(x / 16));//当前位置
                            c = (int)Math.Floor((double)((x + tem) / 16)) - d;//跨行
                            int i1 = 0;
                            int i2 = 0;
                            if (a > b)
                            {
                                i2 = b;
                            }
                            else if (d <= 2)
                            {
                                i2 = a;
                            }
                            else if (b - d < a)
                            {
                                e = (int)Math.Floor((double)(x / 16 - (b - a)));
                                i1 = (b - a) * 16;
                                i2 = a;
                            }
                            else
                            {
                                i1 = (d - 3) * 16;
                                i2 = a;
                            }
                            //
                            StringBuilder sb1 = new StringBuilder();
                            StringBuilder sb2 = new StringBuilder();
                            StringBuilder sb3 = new StringBuilder();
                            byte[] bt = new byte[16];
                            int f;
                            fs.Seek(i1, SeekOrigin.Begin);
                            for (int i = 0; i < i2; i++)
                            {
                                for (int y = 0; y < 16; y++)
                                {
                                    if (y == 0)
                                    {
                                        f = br.ReadByte();
                                        sb1.Append((fs.Position - 1).ToString("X8") + "\r\n");
                                        sb2.Append(f.ToString("X2"));
                                        if (f < 32 || f == 255 || f == 128)
                                        {
                                            bt[y] = 46;
                                        }
                                        else
                                        {
                                            bt[y] = (byte)f;
                                        }
                                    }
                                    else
                                    {
                                        if (y == 15)
                                        {
                                            f = br.ReadByte();
                                            sb2.Append(" " + f.ToString("X2") + "\r\n");
                                            if (f < 32 || f == 255 || f == 128)
                                            {
                                                bt[y] = 46;
                                            }
                                            else
                                            {
                                                bt[y] = (byte)f;
                                            }
                                        }
                                        else
                                        {
                                            f = br.ReadByte();
                                            sb2.Append(" " + f.ToString("X2"));
                                            if (f < 32 || f == 255 || f == 128)
                                            {
                                                bt[y] = 46;
                                            }
                                            else
                                            {
                                                bt[y] = (byte)f;
                                            }
                                        }
                                    }
                                }
                                sb3.Append(AA_Default_Encoding.GetString(bt) + "\r\n");
                            }
                            int g = (int)(fs.Length - fs.Position);
                            int h = 1;
                            if (g < 16 && g > 0)
                            {
                                f = br.ReadByte();
                                sb1.Append((fs.Position - 1).ToString("X8") + "\r\n");
                                sb2.Append(f.ToString("X2"));
                                if (f < 32 || f == 255 || f == 128)
                                {
                                    bt[0] = 46;
                                }
                                else
                                {
                                    bt[0] = (byte)f;
                                }
                                while (fs.Position < fs.Length)
                                {
                                    f = br.ReadByte();
                                    sb2.Append(" " + f.ToString("X2"));
                                    if (f < 32 || f == 255 || f == 128)
                                    {
                                        bt[h] = 46;
                                    }
                                    else
                                    {
                                        bt[h] = (byte)f;
                                    }
                                    h++;
                                }
                                sb3.Append(AA_Default_Encoding.GetString(bt, 0, g));
                            }
                            Application.OpenForms[0].Controls[0].Controls[0].Controls[0].Controls[2].Text = sb1.ToString();
                            Application.OpenForms[0].Controls[0].Controls[0].Controls[0].Controls[3].Text = sb2.ToString();
                            Application.OpenForms[0].Controls[0].Controls[0].Controls[0].Controls[4].Text = sb3.ToString();
                        }
                    }
                    TextBox TextBoxtmp = (TextBox)Application.OpenForms[0].Controls[0].Controls[0].Controls[0].Controls[3];
                    if (a > b)
                    {
                        TextBoxtmp.Select(x * 3 + d, tem * 3 - 1 + c);
                    }
                    else if (d <= 2)
                    {
                        TextBoxtmp.Select(x * 3 + d, tem * 3 - 1 + c);
                    }
                    else if (b - d < a)
                    {
                        TextBoxtmp.Select((x - (b - a) * 16) * 3 + e, tem * 3 - 1 + c);
                    }
                    else
                    {
                        TextBoxtmp.Select((x - (d - 3) * 16) * 3 + 3, tem * 3 - 1 + c);
                    }
                }
            }
        }

        private void HexView(int i1, int i2)//i1 为起始地址，i2 为行数
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    StringBuilder sb4 = new StringBuilder();
                    StringBuilder sb5 = new StringBuilder();
                    StringBuilder sb6 = new StringBuilder();
                    byte[] bt = new byte[16];
                    int f;
                    fs.Seek(i1, SeekOrigin.Begin);
                    for (int i = 0; i < i2; i++)
                    {
                        for (int y = 0; y < 16; y++)
                        {
                            if (y == 0)
                            {
                                f = br.ReadByte();
                                sb4.Append((fs.Position - 1).ToString("X8") + "\r\n");
                                sb5.Append(f.ToString("X2"));
                                if (f < 32 || f == 255 || f == 128)
                                {
                                    bt[y] = 46;
                                }
                                else
                                {
                                    bt[y] = (byte)f;
                                }
                            }
                            else
                            {
                                if (y == 15)
                                {
                                    f = br.ReadByte();
                                    sb5.Append(" " + f.ToString("X2") + "\r\n");
                                    if (f < 32 || f == 255 || f == 128)
                                    {
                                        bt[y] = 46;
                                    }
                                    else
                                    {
                                        bt[y] = (byte)f;
                                    }
                                }
                                else
                                {
                                    f = br.ReadByte();
                                    sb5.Append(" " + f.ToString("X2"));
                                    if (f < 32 || f == 255 || f == 128)
                                    {
                                        bt[y] = 46;
                                    }
                                    else
                                    {
                                        bt[y] = (byte)f;
                                    }
                                }
                            }
                        }
                        sb6.Append(AA_Default_Encoding.GetString(bt) + "\r\n");
                    }
                    int g = (int)(fs.Length - fs.Position);
                    int h = 1;
                    if (g < 16 && g > 0)
                    {
                        f = br.ReadByte();
                        sb4.Append((fs.Position - 1).ToString("X8") + "\r\n");
                        sb5.Append(f.ToString("X2"));
                        if (f < 32 || f == 255 || f == 128)
                        {
                            bt[0] = 46;
                        }
                        else
                        {
                            bt[0] = (byte)f;
                        }
                        while (fs.Position < fs.Length)
                        {
                            f = br.ReadByte();
                            sb5.Append(" " + f.ToString("X2"));
                            if (f < 32 || f == 255 || f == 128)
                            {
                                bt[h] = 46;
                            }
                            else
                            {
                                bt[h] = (byte)f;
                            }
                            h++;
                        }
                        sb6.Append(AA_Default_Encoding.GetString(bt, 0, g));
                    }
                    textBox4.Text = sb4.ToString();
                    textBox5.Text = sb5.ToString();
                    textBox6.Text = sb6.ToString();
                }
            }
        }

        private void RowEnterDis(int i1)//进入一行
        {
            textBox2.Clear();
            string AddressStr = dataTable1.Rows[i1][0].ToString();
            string s1 = dataTable1.Rows[i1][1].ToString();
            textBox3.Text = s1;
            string s2 = dataTable1.Rows[i1][2].ToString();
            orgl = (int)dataTable1.Rows[i1][3];
            tral = (int)dataTable1.Rows[i1][4];
            if (li == "i")
            {
                toolStripTextBox4.Text = orgl.ToString();
                toolStripTextBox5.Text = tral.ToString();
            }
            else
            {
                toolStripTextBox4.Text = orgl.ToString("X4");
                toolStripTextBox5.Text = tral.ToString("X4");
            }
            toolTip1.SetToolTip(textBox3, "");
            if (s1.Length > 12 && s1.Length < 2000)
            {
                toolTip1.SetToolTip(textBox3, s1);
            }
            if (s1.Contains("\r") == true && s1.Contains("\n") == false)
            {
                s2 = s2.Replace("\r", "\r\n");
            }
            else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
            {
                s2 = s2.Replace("\n", "\r\n");
            }
            textBox1.Text = s2;
            SearchString();
            if (hexHToolStripMenuItem.Checked == true)
            {
                if (FPBool == true)
                {
                    int x = (int)CommonCode.HexToLong(AddressStr);
                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            int a = (int)Math.Floor(splitContainer1.Panel1.Height / 12D - 2);//面板能显示的行数
                            int b = (int)Math.Floor(fs.Length / 16D);//文件能显示的行数
                            int d = (int)Math.Floor(x / 16D);//当前位置
                            int c = (int)Math.Floor((x + orgl) / 16D) - d;//跨行
                            if (a > b)
                            {
                                HexView(0, b);
                                textBox5.Select(x * 3 + d, orgl * 3 - 1 + c);
                            }
                            else if (d <= 2)
                            {
                                HexView(0, a);
                                textBox5.Select(x * 3 + d, orgl * 3 - 1 + c);
                            }
                            else if (b - d < a)
                            {
                                int f = (int)Math.Floor(x / 16D - (b - a));
                                HexView((b - a) * 16, a);
                                textBox5.Select((x - (b - a) * 16) * 3 + f, orgl * 3 - 1 + c);
                            }
                            else
                            {
                                HexView((d - 3) * 16, a);
                                textBox5.Select((x - (d - 3) * 16) * 3 + 3, orgl * 3 - 1 + c);
                            }
                        }
                    }
                }
                else
                {
                    textBox4.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                }
            }
        }

        private void Save_Project()//保存工程
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                int MyIndex = dataGridView1.CurrentRow.Index;
                string s1 = textBox3.Text;
                string s2 = textBox1.Text;
                if (s1.Contains("\r") == true && s1.Contains("\n") == false)
                {
                    s2 = s2.Replace("\r\n", "\r");
                }
                else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
                {
                    s2 = s2.Replace("\r\n", "\n");
                }
                string tra2 = dataTable1.Rows[MyIndex][2].ToString();
                if (s2 != tra2)
                {
                    if (File.Exists(ProjectFileName) == false)
                    {
                        EditItemBL = false;
                        MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if ((bool)dataTable1.Rows[MyIndex][16] == true)
                        {
                            EditItemBL = false;
                            MessageBox.Show("请首先取消此项目矩阵设置再进行修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if ((bool)dataTable1.Rows[MyIndex][12] == true)
                        {
                            EditItemBL = false;
                            MessageBox.Show("请首先取消此项目超写设置再进行修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if ((int)dataTable1.Rows[MyIndex][11] > 0)
                        {
                            EditItemBL = false;
                            MessageBox.Show("请首先取消此项目后移设置再进行修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if ((int)dataTable1.Rows[MyIndex][10] > 0)
                        {
                            EditItemBL = false;
                            MessageBox.Show("请首先取消此项目前移设置再进行修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else if (dataTable1.Rows[MyIndex][8].ToString() != "")
                        {
                            EditItemBL = false;
                            MessageBox.Show("请首先取消此项目迁移设置再进行修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string s5 = dataTable1.Rows[MyIndex][5].ToString();
                            if (s5 != "")
                            {
                                EditItemBL = false;
                                Clipboard.Clear();
                                Clipboard.SetDataObject(s5);
                                MessageBox.Show("地址 " + s5 + " 在此处设置了迁移，请首先取消此处的迁移设置再进行修改。\r\n地址已复制到剪贴板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                bool bltmp = true;
                                if (UnPEType == "6" && s2 != "")
                                {
                                    if (s1.Contains("msgstr") == true)
                                    {
                                        if (s2.Contains("msgstr \"") == false && s2.Contains("msgstr[0] \"") == false && s2.Contains("msgstr[1] \"") == false)
                                        {
                                            bltmp = false;
                                        }
                                        else if (s2.Substring(s2.Length - 1, 1) != "\"")
                                        {
                                            s2 = s2 + "\"";
                                        }
                                        else if (s2.Replace("msgstr", "").Replace("[0]", "").Replace("[1]", "") == " \"")
                                        {
                                            s2 = s2 + "\"";
                                        }
                                        if (bltmp == false)
                                        {
                                            EditItemBL = false;
                                            MessageBox.Show("字符串格式不正确。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        EditItemBL = false;
                                        bltmp = false;
                                    }
                                }
                                if (bltmp == true)
                                {
                                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                                    {
                                        MyAccess.Open();
                                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                        {
                                            int tralong4 = 0;
                                            int free7 = 0;
                                            bool bl14 = (bool)dataTable1.Rows[MyIndex][14];//utf8
                                            bool bl15 = (bool)dataTable1.Rows[MyIndex][15];//Unicode
                                            if (bl14 == true)
                                            {
                                                tralong4 = Encoding.UTF8.GetByteCount(s2);
                                            }
                                            else if (bl15 == true)
                                            {
                                                tralong4 = Encoding.Unicode.GetByteCount(s2);
                                            }
                                            else
                                            {
                                                tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(s2);
                                            }
                                            free7 = (int)dataTable1.Rows[MyIndex][3] - tralong4;//剩余长度
                                            try
                                            {

                                                s2 = s2.Replace("'", "''");
                                                cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + dataTable1.Rows[MyIndex][0].ToString() + "'";
                                                cmd.Transaction = MyAccess.BeginTransaction();
                                                cmd.ExecuteNonQuery();
                                                cmd.Transaction.Commit();
                                                s2 = s2.Replace("''", "'");
                                                dataTable1.Rows[MyIndex][2] = s2;
                                                dataTable1.Rows[MyIndex][4] = tralong4;
                                                dataTable1.Rows[MyIndex][7] = free7;
                                                EditItemBL = true;
                                            }
                                            catch (Exception MyEx)
                                            {
                                                MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            finally
                                            {
                                                if (BackupProjectTaskbool == false)
                                                {
                                                    Task.Factory.StartNew(() => BackupTask());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    EditItemBL = true;
                }
            }
        }

        private void Key_Down()//向下方向键
        {
            int i = dataTable1.Rows.Count;
            if (i != 0)
            {
                Save_Project();//保存工程
                if (EditItemBL == true)//编辑是否成功
                {
                    int x = dataGridView1.CurrentCell.RowIndex;
                    if (x < i - 1)
                    {
                        x++;
                        dataGridView1.CurrentCell = dataGridView1[0, x];
                        RowEnterDis(x);
                        this.Activate();
                    }
                }
            }
        }

        private void Key_Up()//向上方向键
        {
            int i = dataTable1.Rows.Count;
            if (i != 0)
            {
                Save_Project();//保存工程
                if (EditItemBL == true)//编辑是否成功
                {
                    int x = dataGridView1.CurrentCell.RowIndex;
                    if (x < i && x != 0)
                    {
                        x--;
                        dataGridView1.CurrentCell = dataGridView1[0, x];
                        RowEnterDis(x);
                        this.Activate();
                    }
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 40)
            {
                Key_Down();
            }
            else if (e.KeyValue == 38)
            {
                Key_Up();
            }
        }

        private void mainform_DragEnter(object sender, DragEventArgs e)//拖放
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                Clipboard.Clear();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length == 1)
                {
                    string s1 = filenames[0];
                    if (Path.GetExtension(s1).ToLower() == ".ena")
                    {
                        Clipboard.SetDataObject(s1);
                        e.Effect = DragDropEffects.Copy;
                    }
                }
            }
        }

        private void mainform_DragDrop(object sender, DragEventArgs e)//打开工程
        {
            ProjectFileName = (string)Clipboard.GetData(DataFormats.Text);
            OpenProject();
        }

        private string dv1DoubleClickStr = "";

        private void dataGridView1_DoubleClick(object sender, EventArgs e)//展开工程并定位
        {
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    if (File.Exists(ProjectFileName) == false)
                    {
                        MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dv1DoubleClickStr = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                                {
                                    cmd.CommandText = "select count(address) from athenaa";
                                    object addcount = cmd.ExecuteScalar();
                                    int myaddcount = int.Parse(addcount.ToString());
                                    if (i1 != myaddcount)
                                    {
                                        if (SD.Visible)
                                        {
                                            SD.Hide();
                                        }
                                        dataTable1.Clear();
                                        dataGridView1.DataSource = null;
                                        if (myaddcount > 40000)
                                        {
                                            DisabledPanelContrl();
                                            SPB = true;
                                            toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                            toolStripProgressBar1.Visible = true;
                                            ProgressBarTimer.Enabled = true;
                                        }
                                        BackgroundWorker dv1DoubleClickBackgroundWorker = new BackgroundWorker();
                                        dv1DoubleClickBackgroundWorker.DoWork += Dv1DoubleClickBackgroundWorker_DoWork;
                                        dv1DoubleClickBackgroundWorker.RunWorkerCompleted += Dv1DoubleClickBackgroundWorker_RunWorkerCompleted;
                                        dv1DoubleClickBackgroundWorker.RunWorkerAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Dv1DoubleClickBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void Dv1DoubleClickBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            int i2 = 0;
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == dv1DoubleClickStr)
                        {
                            i2 = i;
                        }
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                else
                {
                    Parallel.For(0, i1, (i, ParallelLoopState) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == dv1DoubleClickStr)
                        {
                            ParallelLoopState.Break();
                            i2 = i;
                        }
                    });
                }
                if (i2 > 9)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = i2 - 10;
                }
                dataGridView1.CurrentCell = dataGridView1[0, i2];
                dataGridView1.CurrentCell.Selected = true;
            }
            EnablePanelContrl();
            this.Activate();
        }

        private ConcurrentDictionary<int, string> DeleteRowsIndexAndAddress;

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)//确定删除
        {
            int MyKeyValue = e.KeyValue;
            if (MyKeyValue == 46)
            {
                if (File.Exists(ProjectFileName) == true)
                {
                    if (dataTable1.Rows.Count > 0)
                    {
                        DialogResult dr = MessageBox.Show("确实要删除选定的内容吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dr == DialogResult.OK)
                        {
                            if (DateTime.Now.Minute < wt || (DateTime.Now.Minute - wt) > 10)
                            {
                                BackupTem();
                            }
                            DeleteRowsIndexAndAddress = new ConcurrentDictionary<int, string>();
                            int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                            int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                            for (; intFirstRow <= intLastRow; intFirstRow++)
                            {
                                if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                {
                                    if ((int)dataTable1.Rows[intFirstRow][4] == 0)
                                    {
                                        DeleteRowsIndexAndAddress.TryAdd(intFirstRow, dataTable1.Rows[intFirstRow][0].ToString());
                                    }
                                }
                            }
                            int i1 = DeleteRowsIndexAndAddress.Count;
                            if (i1 > 0)
                            {
                                DisabledPanelContrl();
                                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                toolStripProgressBar1.Visible = true;
                                ProgressBarTimer.Enabled = true;
                                SPB = true;
                                if (i1 > 500)
                                {
                                    dataGridView1.DataSource = null;
                                }
                                DeleteRowsbackgroundWorker.RunWorkerAsync();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("没有找到工程文件，无法进行删除。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (MyKeyValue == 33 || MyKeyValue == 34 || MyKeyValue == 38 || MyKeyValue == 40)//向上、向下翻页
                {
                    AllowRowEnterBL = true;
                    dataGridView1.RowEnter += DataGridView1_RowEnter;
                }
            }
        }

        private bool AllowRowEnterBL = false;

        private void DataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (AllowRowEnterBL)
            {
                AllowRowEnterBL = false;
                int i1 = e.RowIndex;
                if (i1 >= 0)
                {
                    RowEnterDis(i1);
                    this.Activate();
                }
            }
        }

        private void DeleteRowsbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            int i1 = DeleteRowsIndexAndAddress.Count;
            Parallel.Invoke(() =>
            {
                string[] sx = new string[i1];
                DeleteRowsIndexAndAddress.Values.CopyTo(sx, 0);
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        int m = 0;
                        StringBuilder sb = new StringBuilder();
                        cmd.Transaction = MyAccess.BeginTransaction();
                        for (int i = 0; i < i1; i++)
                        {
                            if (m == 0)
                            {
                                sb.Clear();
                                sb.Append("delete from athenaa where address = '" + sx[i] + "'");
                            }
                            else
                            {
                                sb.Append(" or address = '" + sx[i] + "'");
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
            },
            () =>
            {
                int[] ix = new int[i1];
                DeleteRowsIndexAndAddress.Keys.CopyTo(ix, 0);
                Array.Sort(ix);
                dataGridView1.Invoke(new Action(delegate
                {
                    for (int i = i1 - 1; i >= 0; i--)
                    {
                        dataTable1.Rows.RemoveAt(ix[i]);
                    }
                }));
            });
        }

        private void DeleteRowsbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            SPB = false;
            DeleteRowsIndexAndAddress = null;
            if (dataGridView1.DataSource == null)
            {
                dataGridView1.DataSource = dataSet1;
                dataGridView1.DataMember = dataTable1.TableName;
            }
            if (dataTable1.Rows.Count == 0)
            {
                ClearText();
            }
            else
            {
                RowEnterDis(dataGridView1.CurrentRow.Index);
            }
        }

        private void BackupTem()//删除、过滤之前备份工程
        {
            string p = Path.GetFileNameWithoutExtension(ProjectFileName);
            if (Directory.Exists(CDirectory + "工程\\备份\\") == false)
            {
                Directory.CreateDirectory(CDirectory + "工程\\备份");
            }
            p = CDirectory + "工程\\备份\\" + p.Substring(0, p.Length - 10) + string.Format("{0:yyMMddHHmm}", DateTime.Now) + ".ENA";
            if (File.Exists(p) == false)
            {
                File.Copy(ProjectFileName, p);
                wt = DateTime.Now.Minute;
            }
        }

        private void ConvertStringLengthFormat()
        {
            string s1 = toolStripTextBox4.Text;
            string s2 = toolStripTextBox5.Text;
            if (s1 != "")
            {
                if (li == "i")
                {
                    toolStripTextBox4.Text = System.Int32.Parse(s1).ToString("X4");
                    toolStripTextBox5.Text = System.Int32.Parse(s2).ToString("X4");
                    li = "h";
                }
                else
                {
                    toolStripTextBox4.Text = CommonCode.HexToLong(s1).ToString();
                    toolStripTextBox5.Text = CommonCode.HexToLong(s2).ToString();
                    li = "i";
                }
            }
            dataGridView1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)//翻译文本的长度
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            string s1 = textBox3.Text;
            string s2 = textBox1.Text;
            if (dataTable1.Rows.Count > 0)
            {
                if (s1.Contains("\r") == true && s1.Contains("\n") == false)
                {
                    s2 = s2.Replace("\r\n", "\r");
                }
                else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
                {
                    s2 = s2.Replace("\r\n", "\n");
                }
                int row = dataGridView1.CurrentRow.Index;
                if (li == "i")//长度标识，i 为整数，h 为十六进制值
                {
                    if ((Boolean)dataTable1.Rows[row][14] == true)
                    {
                        tral = Encoding.UTF8.GetByteCount(s2);//译文长度
                        toolStripTextBox5.Text = tral.ToString();
                    }
                    else if ((Boolean)dataTable1.Rows[row][15] == true)
                    {
                        tral = Encoding.Unicode.GetByteCount(s2);
                        toolStripTextBox5.Text = tral.ToString();
                    }
                    else
                    {
                        tral = Encoding.GetEncoding(ProTraCode).GetByteCount(s2);
                        toolStripTextBox5.Text = tral.ToString();
                    }
                }
                else
                {
                    if ((Boolean)dataTable1.Rows[row][14] == true)
                    {
                        tral = Encoding.UTF8.GetByteCount(s2);
                        toolStripTextBox5.Text = tral.ToString("X4");
                    }
                    else if ((Boolean)dataTable1.Rows[row][15] == true)
                    {
                        tral = Encoding.Unicode.GetByteCount(s2);
                        toolStripTextBox5.Text = tral.ToString("X4");
                    }
                    else
                    {
                        tral = Encoding.GetEncoding(ProTraCode).GetByteCount(s2);
                        toolStripTextBox5.Text = tral.ToString("X4");
                    }
                }
            }
            else
            {
                toolStripTextBox4.Clear();
                toolStripTextBox5.Clear();
            }
        }

        private void 复制地址ToolStripMenuItem_Click(object sender, EventArgs e)//复制地址
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                Clipboard.Clear();
                Clipboard.SetDataObject(dataTable1.Rows[dataGridView1.CurrentRow.Index][0].ToString());
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (e.Button == MouseButtons.Right)
            {
                int i = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (i == 0)
                {
                    contextMenuStrip1.Enabled = false;
                }
                else
                {
                    contextMenuStrip1.Enabled = true;
                    挪移ToolStripMenuItem.Enabled = false;
                    if (i == 1)
                    {
                        删除ToolStripMenuItem.Enabled = true;
                        复制地址ToolStripMenuItem.Enabled = true;
                        if (PEbool)
                        {
                            string s2 = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                            int i6 = (int)dataGridView1.CurrentRow.Cells[6].Value;
                            int i7 = (int)dataGridView1.CurrentRow.Cells[7].Value;
                            string s8 = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                            int i10 = (int)dataGridView1.CurrentRow.Cells[10].Value;
                            int i11 = (int)dataGridView1.CurrentRow.Cells[11].Value;
                            bool b12 = (bool)dataGridView1.CurrentRow.Cells[12].Value;
                            bool b16 = (bool)dataGridView1.CurrentRow.Cells[16].Value;
                            添加ToolStripMenuItem.Enabled = true;
                            if (s2 == "")
                            {
                                取消翻译ToolStripMenuItem.Enabled = false;
                                添加到字典ToolStripMenuItem.Enabled = false;
                            }
                            else
                            {
                                取消翻译ToolStripMenuItem.Enabled = true;
                                if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == s2)
                                {
                                    添加到字典ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    添加到字典ToolStripMenuItem.Enabled = true;
                                }
                            }
                            引用ToolStripMenuItem.Enabled = true;
                            添加到过滤器ToolStripMenuItem.Enabled = true;
                            设置超写ToolStripMenuItem.Enabled = false;
                            取消超写ToolStripMenuItem.Enabled = false;
                            if (s8 == "" && i10 == 0 && i11 == 0 && b12 == false && b16 == false)
                            {

                                if (i7 >= 0)
                                {
                                    超写ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    超写ToolStripMenuItem.Enabled = true;
                                }
                                if (i6 == 0 && s2 != "")
                                {
                                    挪移ToolStripMenuItem.Enabled = true;
                                    前移ToolStripMenuItem.Enabled = true;
                                    后移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Enabled = true;
                                    矩阵ToolStripMenuItem.Enabled = true;
                                }
                                else
                                {
                                    前移ToolStripMenuItem.Enabled = false;
                                    后移ToolStripMenuItem.Enabled = false;
                                    迁移ToolStripMenuItem.Enabled = false;
                                    矩阵ToolStripMenuItem.Enabled = false;
                                }
                                迁移ToolStripMenuItem.Checked = false;
                                矩阵ToolStripMenuItem.Checked = false;
                                前移ToolStripMenuItem.Checked = false;
                                后移ToolStripMenuItem.Checked = false;
                                超写ToolStripMenuItem.Checked = false;
                            }
                            else
                            {
                                if (s8 != "")
                                {
                                    挪移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Checked = true;
                                    矩阵ToolStripMenuItem.Enabled = false;
                                    矩阵ToolStripMenuItem.Checked = false;
                                    前移ToolStripMenuItem.Enabled = false;
                                    前移ToolStripMenuItem.Checked = false;
                                    后移ToolStripMenuItem.Enabled = false;
                                    后移ToolStripMenuItem.Checked = false;
                                    超写ToolStripMenuItem.Enabled = false;
                                    超写ToolStripMenuItem.Checked = false;
                                }
                                else if (i10 > 0)
                                {
                                    挪移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Enabled = false;
                                    迁移ToolStripMenuItem.Checked = false;
                                    矩阵ToolStripMenuItem.Enabled = false;
                                    矩阵ToolStripMenuItem.Checked = false;
                                    前移ToolStripMenuItem.Enabled = true;
                                    前移ToolStripMenuItem.Checked = true;
                                    后移ToolStripMenuItem.Enabled = false;
                                    后移ToolStripMenuItem.Checked = false;
                                    超写ToolStripMenuItem.Enabled = false;
                                    超写ToolStripMenuItem.Checked = false;
                                }
                                else if (i11 > 0)
                                {
                                    挪移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Enabled = false;
                                    迁移ToolStripMenuItem.Checked = false;
                                    矩阵ToolStripMenuItem.Enabled = false;
                                    矩阵ToolStripMenuItem.Checked = false;
                                    前移ToolStripMenuItem.Enabled = false;
                                    前移ToolStripMenuItem.Checked = false;
                                    后移ToolStripMenuItem.Enabled = true;
                                    后移ToolStripMenuItem.Checked = true;
                                    超写ToolStripMenuItem.Enabled = false;
                                    超写ToolStripMenuItem.Checked = false;
                                }
                                else if (b12)
                                {
                                    超写ToolStripMenuItem.Enabled = true;
                                    超写ToolStripMenuItem.Checked = true;
                                }
                                else if (b16)
                                {
                                    挪移ToolStripMenuItem.Enabled = true;
                                    迁移ToolStripMenuItem.Enabled = false;
                                    迁移ToolStripMenuItem.Checked = false;
                                    矩阵ToolStripMenuItem.Enabled = true;
                                    矩阵ToolStripMenuItem.Checked = true;
                                    前移ToolStripMenuItem.Enabled = false;
                                    前移ToolStripMenuItem.Checked = false;
                                    后移ToolStripMenuItem.Enabled = false;
                                    后移ToolStripMenuItem.Checked = false;
                                    超写ToolStripMenuItem.Enabled = false;
                                    超写ToolStripMenuItem.Checked = false;
                                }
                            }
                        }
                        else
                        {
                            //1   Winhex
                            //2   Goldwave
                            //3   SEGA Rally
                            //4   CMR4
                            //5   Skyrim
                            //6   Qt Linguist
                            if (UnPEType == "1" || UnPEType == "2")
                            {
                                超写ToolStripMenuItem.Enabled = false;
                                超写ToolStripMenuItem.Checked = false;
                                设置超写ToolStripMenuItem.Enabled = false;
                                取消超写ToolStripMenuItem.Enabled = false;
                                添加ToolStripMenuItem.Enabled = true;
                                if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                                {
                                    取消翻译ToolStripMenuItem.Enabled = false;
                                    添加到字典ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    取消翻译ToolStripMenuItem.Enabled = true;
                                    if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == dataGridView1.CurrentRow.Cells[2].Value.ToString())
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = false;
                                    }
                                    else
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = true;
                                    }
                                }
                                添加到过滤器ToolStripMenuItem.Enabled = true;
                                引用ToolStripMenuItem.Enabled = false;
                            }
                            else if (UnPEType == "3" || UnPEType == "4")
                            {
                                if ((int)dataGridView1.CurrentRow.Cells[7].Value < 0)
                                {
                                    超写ToolStripMenuItem.Enabled = true;
                                    if ((bool)dataGridView1.CurrentRow.Cells[12].Value)
                                    {
                                        超写ToolStripMenuItem.Checked = true;
                                        设置超写ToolStripMenuItem.Enabled = false;
                                        取消超写ToolStripMenuItem.Enabled = false;
                                    }
                                    else
                                    {
                                        超写ToolStripMenuItem.Checked = false;
                                        设置超写ToolStripMenuItem.Enabled = true;
                                        取消超写ToolStripMenuItem.Enabled = true;
                                    }
                                }
                                else
                                {
                                    超写ToolStripMenuItem.Enabled = false;
                                    超写ToolStripMenuItem.Checked = false;
                                    设置超写ToolStripMenuItem.Enabled = false;
                                    取消超写ToolStripMenuItem.Enabled = false;
                                }
                                添加ToolStripMenuItem.Enabled = true;
                                if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                                {
                                    取消翻译ToolStripMenuItem.Enabled = false;
                                    添加到字典ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    取消翻译ToolStripMenuItem.Enabled = true;
                                    if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == dataGridView1.CurrentRow.Cells[2].Value.ToString())
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = false;
                                    }
                                    else
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = true;
                                    }
                                }
                                添加到过滤器ToolStripMenuItem.Enabled = true;
                                引用ToolStripMenuItem.Enabled = false;
                            }
                            else if (UnPEType == "5")
                            {
                                超写ToolStripMenuItem.Enabled = false;
                                超写ToolStripMenuItem.Checked = false;
                                设置超写ToolStripMenuItem.Enabled = false;
                                取消超写ToolStripMenuItem.Enabled = false;
                                添加ToolStripMenuItem.Enabled = false;
                                删除ToolStripMenuItem.Enabled = false;
                                if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                                {
                                    取消翻译ToolStripMenuItem.Enabled = false;
                                    添加到字典ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    取消翻译ToolStripMenuItem.Enabled = true;
                                    if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == dataGridView1.CurrentRow.Cells[2].Value.ToString())
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = false;
                                    }
                                    else
                                    {
                                        添加到字典ToolStripMenuItem.Enabled = true;
                                    }
                                }
                                添加到过滤器ToolStripMenuItem.Enabled = false;
                                引用ToolStripMenuItem.Enabled = false;
                            }
                            else if (UnPEType == "6")
                            {
                                超写ToolStripMenuItem.Enabled = false;
                                超写ToolStripMenuItem.Checked = false;
                                设置超写ToolStripMenuItem.Enabled = false;
                                取消超写ToolStripMenuItem.Enabled = false;
                                添加ToolStripMenuItem.Enabled = false;
                                删除ToolStripMenuItem.Enabled = false;
                                添加到过滤器ToolStripMenuItem.Enabled = false;
                                if (dataGridView1.CurrentRow.Cells[1].Value.ToString() == dataGridView1.CurrentRow.Cells[2].Value.ToString() || dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                                {
                                    添加到字典ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    添加到字典ToolStripMenuItem.Enabled = true;
                                }
                                if (dataGridView1.CurrentRow.Cells[1].Value.ToString().Contains("msgstr") == false)
                                {
                                    恢复ToolStripMenuItem.Enabled = false;
                                    翻译ToolStripMenuItem.Enabled = false;
                                    取消翻译ToolStripMenuItem.Enabled = false;
                                }
                                else
                                {
                                    恢复ToolStripMenuItem.Enabled = true;
                                    翻译ToolStripMenuItem.Enabled = true;
                                    if (dataGridView1.CurrentRow.Cells[2].Value.ToString() == "")
                                    {
                                        取消翻译ToolStripMenuItem.Enabled = false;
                                    }
                                    else
                                    {
                                        取消翻译ToolStripMenuItem.Enabled = true;
                                    }
                                }
                                引用ToolStripMenuItem.Enabled = false;
                            }
                        }
                    }
                    else
                    {
                        删除ToolStripMenuItem.Enabled = false;
                        引用ToolStripMenuItem.Enabled = false;
                        复制地址ToolStripMenuItem.Enabled = false;
                        超写ToolStripMenuItem.Enabled = false;
                        超写ToolStripMenuItem.Checked = false;
                        添加到过滤器ToolStripMenuItem.Enabled = true;
                        添加到字典ToolStripMenuItem.Enabled = true;
                        取消翻译ToolStripMenuItem.Enabled = true;
                        if (PEbool)
                        {
                            设置超写ToolStripMenuItem.Enabled = true;
                            取消超写ToolStripMenuItem.Enabled = true;
                        }
                        else
                        {
                            if (UnPEType == "3" || UnPEType == "4")
                            {
                                设置超写ToolStripMenuItem.Enabled = true;
                                取消超写ToolStripMenuItem.Enabled = true;
                            }
                            else
                            {
                                设置超写ToolStripMenuItem.Enabled = false;
                                取消超写ToolStripMenuItem.Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void ZiDianPath(string s)//设定字典位置
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (DataTable TmpDT = MyAccess.GetSchema("Tables"))
                {
                    if (TmpDT.Rows[0][2].ToString() == "athenaa")
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.CommandText = "update fileinfo set detail = '" + s.Replace(CDirectory + "字典\\", "").Replace("'", "''") + "' where infoname = '字典'";
                            cmd.Transaction = MyAccess.BeginTransaction();
                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                            dataTable4.Rows[6][1] = s;
                        }
                    }
                }
            }
        }

        private void toolStripButton13_Click(object sender, EventArgs e)//挂接字典
        {
            dataGridView1.Enabled = false;
            DictionaryStr = CommonCode.Open_Dictionary_File(DictionaryStr);
            if (File.Exists(DictionaryStr) == true)
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + DictionaryStr))
                {
                    MyAccess.Open();
                    using (DataTable TmpDT = MyAccess.GetSchema("Tables"))
                    {
                        ArrayList AL = new ArrayList();
                        int iTmp = TmpDT.Rows.Count;
                        for (int i = 0; i < iTmp; i++)
                        {
                            AL.Add(TmpDT.Rows[i][2].ToString());
                        }
                        if (!(AL.Contains("diclanguage") && AL.Contains("tbl")))
                        {
                            MessageBox.Show("指定的字典文件不是由该程序创建的，无法挂接。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            dataTable6.Clear();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                cmd.CommandText = "select org,tra from tbl";
                                SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                                ad.Fill(dataTable6);
                            }
                            if (dataTable6.Rows.Count == 0)
                            {
                                查找多译TToolStripMenuItem.Enabled = false;
                                MessageBox.Show("这是一个空字典文件，无法挂接。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                查找多译TToolStripMenuItem.Enabled = true;
                                ZiDianPath(DictionaryStr);
                                textBox2.Enabled = true;
                            }
                        }
                    }
                }
            }
            dataGridView1.Enabled = true;
        }

        private void textBox2_Click(object sender, EventArgs e)//应用翻译
        {
            string s1 = textBox3.Text;
            string s2 = textBox2.Text;
            if (s2 != "")
            {
                if (s1.Contains("\r") == true && s1.Contains("\n") == false)
                {
                    s2 = s2.Replace("\r", "\r\n");
                }
                else if (s1.Contains("\r") == false && s1.Contains("\n") == true)
                {
                    s2 = s2.Replace("\n", "\r\n");
                }
                textBox1.Text = s2;
            }
            textBox1.Focus();
            textBox1.DeselectAll();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)//快速备份
        {
            if (File.Exists(ProjectFileName))
            {
                string s = Backup();
                if (s != "")
                {
                    MessageBox.Show("工程备份文件\r\n“" + s + "”\r\n已存在，请稍候再试。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("快速备份工程成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton20_Click(object sender, EventArgs e)//清空
        {
            toolStripTextBox1.Clear();
            toolStripTextBox2.Clear();
            toolStripTextBox3.Clear();
            toolStripComboBox1.Text = "小于";
            toolStripComboBox2.Text = "原文";
            toolStripComboBox3.Text = "超长";
            if (MyDpi < 144F)
            {
                toolStripButton6.Image = Athena_A.Properties.Resources.NoCase18;
            }
            else if (MyDpi >= 144F && MyDpi < 192F)
            {
                toolStripButton6.Image = Athena_A.Properties.Resources.NoCase24;
            }
            else if (MyDpi >= 192F)
            {
                toolStripButton6.Image = Athena_A.Properties.Resources.NoCase32;
            }
            MatchCase = false;//大小写匹配
        }

        private void 属性ToolStripMenuItem_Click(object sender, EventArgs e)//显示属性窗口
        {
            ProjectProperties PP = new ProjectProperties();
            if (MyDpi > 96F)
            {
                PP.comboBox1.Font = MyNewFont;
                PP.Font = MyNewFont;
            }
            PP.textBox2.Text = dataTable4.Rows[1][1].ToString();
            FileSize = dataTable4.Rows[2][1].ToString();
            PP.textBox3.Text = int.Parse(FileSize).ToString("#,#") + " 字节";
            if (PEbool == false)
            {
                PP.textBox2.Enabled = false;
                PP.textBox2.BackColor = System.Drawing.SystemColors.Control;
            }
            PP.ShowDialog();//显示工程属性窗口
            dataTable4.Rows[0][1] = FilePath;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)//保存工程
        {
            if (dataTable1.Rows.Count != 0)
            {
                Save_Project();
            }
        }

        private void 编码查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CodeQuery CQ = new CodeQuery();
            if (MyDpi > 96F)
            {
                CQ.Font = MyNewFont;
                CQ.comboBox1.Font = MyNewFont;
            }
            CQ.ShowDialog();
        }

        private void 地址计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Calculator C1 = new Calculator();
            if (MyDpi > 96F)
            {
                C1.Font = MyNewFont;
            }
            C1.ShowDialog();
        }

        private void 自动翻译ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoTranslate AT = new AutoTranslate();
            if (MyDpi > 96F)
            {
                AT.Font = MyNewFont;
            }
            AT.ShowDialog();
            if (AutoTrabl == true)
            {
                AutoTrabl = false;
                dataTable1.Clear();
                DisabledPanelContrl();
                dataGridView1.DataSource = null;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select count(address) from athenaa";
                            object addcount = cmd.ExecuteScalar();
                            int myaddcount = int.Parse(addcount.ToString());
                            if (myaddcount > 40000)
                            {
                                SPB = true;
                                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                toolStripProgressBar1.Visible = true;
                                ProgressBarTimer.Enabled = true;
                            }
                            BackgroundWorker AutoTraBackgroundWorker = new BackgroundWorker();
                            AutoTraBackgroundWorker.DoWork += AutoTraBackgroundWorker_DoWork;
                            AutoTraBackgroundWorker.RunWorkerCompleted += AutoTraBackgroundWorker_RunWorkerCompleted;
                            AutoTraBackgroundWorker.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private void AutoTraBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void AutoTraBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            if (PEbool)
            {
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
            }
            EnablePanelContrl();
            this.Activate();
        }

        private void startrun()//运行程序进行测试
        {
            if (File.Exists(runtest) == false)
            {
                runtestform rtf = new runtestform();
                if (MyDpi > 96F)
                {
                    rtf.Font = MyNewFont;
                }
                rtf.ShowDialog();
            }
            else
            {
                System.Diagnostics.Process.Start(runtest);
            }
        }

        private void toolStripButton18_Click(object sender, EventArgs e)
        {
            startrun();
        }

        private void toolStripButton19_Click(object sender, EventArgs e)
        {
            AutoTest();
        }

        private void 运行UToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startrun();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string com = toolStripComboBox3.Text;
                dataTable1.Rows.Clear();
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            if (com == "超长")
                            {
                                cmd.CommandText = "select * from athenaa where free < 0 ORDER BY address ASC";
                            }
                            else if (com == "挪移")
                            {
                                cmd.CommandText = "select * from athenaa where free < 0 and outadd = '' and moveforward = 0 and movebackward = 0 and superlong = 0 and zonebl = 0 ORDER BY address ASC";
                            }
                            else if (com == "迁移")
                            {
                                cmd.CommandText = "select * from athenaa where outaddfree > 0 ORDER BY address ASC";
                            }
                            else if (com == "前移")
                            {
                                cmd.CommandText = "select * from athenaa where moveforward > 0 ORDER BY address ASC";
                            }
                            else if (com == "后移")
                            {
                                cmd.CommandText = "select * from athenaa where movebackward > 0 ORDER BY address ASC";
                            }
                            else if (com == "超写")
                            {
                                cmd.CommandText = "select * from athenaa where superlong = 1 ORDER BY address ASC";
                            }
                            else if (com == "矩阵")
                            {
                                cmd.CommandText = "select * from athenaa where zonebl = 1 ORDER BY address ASC";
                            }
                            ad.Fill(dataTable1);
                            int i1 = dataTable1.Rows.Count;
                            if (i1 > 0)
                            {
                                if (PEbool)
                                {
                                    for (int i = 0; i < i1; i++)
                                    {
                                        if ((bool)dataTable1.Rows[i][12] || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || dataTable1.Rows[i][8].ToString() != "" || (bool)dataTable1.Rows[i][16])
                                        {
                                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                                        }
                                        if (dataTable1.Rows[i][5].ToString() != "")
                                        {
                                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                                        }
                                    }
                                }
                                RowEnterDis(0);
                            }
                            else
                            {
                                ClearText();
                                MessageBox.Show("没有搜索到符合要求的字符串。", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
        }

        private void 导出字典ToolStripMenuItem_Click(object sender, EventArgs e)//导出字典
        {
            ExportDictionary ED = new ExportDictionary();
            if (MyDpi > 96F)
            {
                ED.Font = MyNewFont;
            }
            ED.ShowDialog();
        }

        private void UpdateTB5()//更新表5
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from calladd";
                        dataTable5.Clear();
                        ad.Fill(dataTable5);
                    }
                }
            }
        }

        private void 迁移ToolStripMenuItem_Click(object sender, EventArgs e)//打开迁移对话框
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MoveString MS = new MoveString();
                if (MyDpi > 96F)
                {
                    MS.Font = MyNewFont;
                }
                int ii = dataTable5.Rows.Count;
                int MyIndex = dataGridView1.CurrentRow.Index;
                for (int x = 0; x < 22; x++)
                {
                    obMS[x] = dataTable1.Rows[MyIndex][x];
                }
                string ss = obMS[0].ToString();
                if (ii > 0)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        if (dataTable5.Rows[i][0].ToString() == ss)
                        {
                            if (dataTable5.Rows[i][3].ToString() == "32")
                            {
                                MS.listBox2.Items.Add(dataTable5.Rows[i][1].ToString());
                            }
                            else
                            {
                                MS.listBox3.Items.Add(dataTable5.Rows[i][1].ToString());
                            }

                        }
                    }
                }
                //   0       1     2       3         4        5        6        7       8          9          10            11
                //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                //   12         13      14      15      16        17            18           19          20          21
                //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                //            实际是                           没有使用        0 无
                //           长度标识                                         1 不变
                int y = dataTable1.Rows.Count;
                string s8 = obMS[8].ToString();
                MS.ShowDialog();
                MoveAddressHexView(ss, (int)obMS[3]);
                if (obMS[8].ToString() != "")
                {
                    for (int i = 0; i < y; i++)
                    {
                        if (dataTable1.Rows[i][0].ToString() == obMS[8].ToString())
                        {
                            dataTable1.Rows[i][5] = obMS[0].ToString();
                            dataTable1.Rows[i][6] = (int)obMS[4];
                            dataTable1.Rows[i][19] = false;
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                            break;
                        }
                    }
                }
                else if (s8 != "" && obMS[8].ToString() == "")
                {
                    for (int i = 0; i < y; i++)
                    {
                        if (dataTable1.Rows[i][0].ToString() == s8)
                        {
                            dataTable1.Rows[i][5] = "";
                            dataTable1.Rows[i][6] = 0;
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                            break;
                        }
                    }
                }
                dataTable1.Rows[MyIndex][8] = obMS[8].ToString();
                dataTable1.Rows[MyIndex][9] = (int)obMS[9];
                dataTable1.Rows[MyIndex][18] = obMS[18];
                dataTable1.Rows[MyIndex][19] = false;
                if (obMS[8].ToString() == "")
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                }
                else
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                }
                UpdateTB5();
            }
        }

        private void 前移ToolStripMenuItem_Click(object sender, EventArgs e)//字符串前移设置
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(FilePath) == false)
            {
                MessageBox.Show("需要翻译的文件不存在，无法完成检测，此功能不可用。\r\n可以打开工程属性窗口了解相关信息。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                moveforward MF = new moveforward();
                if (MyDpi > 96F)
                {
                    MF.Font = MyNewFont;
                }
                int ii = dataTable5.Rows.Count;
                int MyIndex = dataGridView1.CurrentRow.Index;
                for (int x = 0; x < 22; x++)
                {
                    obMS[x] = dataTable1.Rows[MyIndex][x];
                }
                string ss = obMS[0].ToString();
                if (ii > 0)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        if (dataTable5.Rows[i][0].ToString() == ss)
                        {
                            if (dataTable5.Rows[i][3].ToString() == "32")
                            {
                                MF.listBox1.Items.Add(dataTable5.Rows[i][1].ToString());
                            }
                            else
                            {
                                MF.listBox2.Items.Add(dataTable5.Rows[i][1].ToString());
                            }
                        }
                    }
                }
                MF.ShowDialog();
                MoveAddressHexView(ss, (int)obMS[3]);
                dataTable1.Rows[MyIndex][10] = obMS[10];
                dataTable1.Rows[MyIndex][18] = obMS[18];
                dataTable1.Rows[MyIndex][19] = false;
                ii = int.Parse(obMS[10].ToString());
                if (ii == 0)
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                }
                else
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                }
                UpdateTB5();
            }
        }

        private void 后移ToolStripMenuItem_Click(object sender, EventArgs e)//后移设置
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(FilePath) == false)
            {
                MessageBox.Show("需要翻译的文件不存在，无法完成检测，此功能不可用。\r\n可以打开工程属性窗口了解相关信息。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                movebackward MB = new movebackward();
                if (MyDpi > 96F)
                {
                    MB.Font = MyNewFont;
                }
                int ii = dataTable5.Rows.Count;
                int MyIndex = dataGridView1.CurrentRow.Index;
                for (int x = 0; x < 22; x++)
                {
                    obMS[x] = dataTable1.Rows[MyIndex][x];
                }
                string ss = obMS[0].ToString();
                if (ii > 0)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        if (dataTable5.Rows[i][0].ToString() == ss)
                        {
                            if (dataTable5.Rows[i][3].ToString() == "32")
                            {
                                MB.listBox1.Items.Add(dataTable5.Rows[i][1].ToString());
                            }
                            else
                            {
                                MB.listBox2.Items.Add(dataTable5.Rows[i][1].ToString());
                            }
                        }
                    }
                }
                MB.ShowDialog();
                MoveAddressHexView(ss, (int)obMS[3]);
                dataTable1.Rows[MyIndex][11] = obMS[11];
                dataTable1.Rows[MyIndex][18] = obMS[18];
                dataTable1.Rows[MyIndex][19] = false;
                ii = int.Parse(obMS[11].ToString());
                if (ii == 0)
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                }
                else
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                }
                UpdateTB5();
            }
        }

        private void 超写ToolStripMenuItem_Click(object sender, EventArgs e)//超写
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int MyIndex = dataGridView1.CurrentRow.Index;
                for (int x = 0; x < 22; x++)
                {
                    obMS[x] = dataTable1.Rows[MyIndex][x];
                }
                if (超写ToolStripMenuItem.Checked)
                {
                    dataTable1.Rows[MyIndex][12] = false;
                    dataTable1.Rows[MyIndex][18] = 0;
                    超写ToolStripMenuItem.Checked = false;
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            try
                            {
                                cmd.Transaction = MyAccess.BeginTransaction();
                                cmd.CommandText = "update athenaa set superlong = 0,codepage = 0,delphicodepage = 0 where address = '" + obMS[0].ToString() + "'";
                                cmd.ExecuteNonQuery();
                                cmd.Transaction.Commit();
                                dataGridView1.Rows[MyIndex].DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                            }
                            catch (Exception MyEx)
                            {
                                MessageBox.Show(MyEx.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else
                {
                    //   0       1     2       3         4        5        6        7       8          9          10            11
                    //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
                    //   12         13      14      15      16        17            18           19          20          21
                    //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
                    //            实际是                           没有使用        0 无
                    //           长度标识                                         1 不变
                    if (StrLenCategory == "Delphi" && (bool)obMS[13] == true)
                    {
                        SuperLongCodePage SLCP = new SuperLongCodePage();
                        if (MyDpi > 96F)
                        {
                            SLCP.Font = MyNewFont;
                        }
                        SLCP.ShowDialog();
                        dataTable1.Rows[MyIndex][18] = obMS[18];
                    }
                    dataTable1.Rows[MyIndex][12] = true;
                    超写ToolStripMenuItem.Checked = true;
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            try
                            {
                                int i1 = int.Parse(obMS[18].ToString());
                                if (i1 == 0)
                                {
                                    cmd.CommandText = "update athenaa set codepage = 0,Ignoretra = 0,superlong = 1,delphicodepage = " + i1 + " where address = '" + obMS[0].ToString() + "'";
                                }
                                else
                                {
                                    cmd.CommandText = "update athenaa set codepage = 1,Ignoretra = 0,superlong = 1,delphicodepage = " + i1 + " where address = '" + obMS[0].ToString() + "'";
                                }
                                cmd.ExecuteNonQuery();
                                dataGridView1.Rows[MyIndex].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                            }
                            catch (Exception MyEx)
                            {
                                MessageBox.Show(MyEx.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void 设置超写ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                        int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                        if (PEbool)
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            for (; intFirstRow <= intLastRow; intFirstRow++)
                            {
                                if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                {
                                    //翻译超长，没有设置为超长,没有进入矩阵挪移
                                    if ((int)dataTable1.Rows[intFirstRow][7] < 0 && (bool)dataTable1.Rows[intFirstRow][12] == false && (bool)dataTable1.Rows[intFirstRow][16] == false)
                                    {
                                        //没有挪移，没有前移，没有后移
                                        if (dataTable1.Rows[intFirstRow][8].ToString() == "" && (int)dataTable1.Rows[intFirstRow][10] == 0 && (int)dataTable1.Rows[intFirstRow][11] == 0)
                                        {
                                            string s1 = dataTable1.Rows[intFirstRow][0].ToString();
                                            try
                                            {
                                                dataTable1.Rows[intFirstRow][12] = true;
                                                cmd.CommandText = "update athenaa set Ignoretra = 0,superlong = 1 where address = '" + s1 + "'";
                                                cmd.ExecuteNonQuery();
                                                cmd.CommandText = "delete from IgnoreTra where address = '" + s1 + "'";
                                                cmd.ExecuteNonQuery();

                                                dataGridView1.Rows[intFirstRow].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                                            }
                                            catch (Exception MyEx)
                                            {
                                                MessageBox.Show("地址 " + s1 + " 所在行出错：\r\n" + MyEx.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            cmd.Transaction.Commit();
                        }
                        else if (UnPEType == "3" || UnPEType == "4")
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            for (; intFirstRow <= intLastRow; intFirstRow++)
                            {
                                if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                {
                                    //翻译超长，没有设置为超长,没有进入矩阵挪移
                                    if ((int)dataTable1.Rows[intFirstRow][7] < 0 && (bool)dataTable1.Rows[intFirstRow][12] == false)
                                    {
                                        string s1 = dataTable1.Rows[intFirstRow][0].ToString();
                                        try
                                        {
                                            dataTable1.Rows[intFirstRow][12] = true;
                                            cmd.CommandText = "update athenaa set Ignoretra = 0,superlong = 1 where address = '" + s1 + "'";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "delete from IgnoreTra where address = '" + s1 + "'";
                                            cmd.ExecuteNonQuery();
                                            dataGridView1.Rows[intFirstRow].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                                        }
                                        catch (Exception MyEx)
                                        {
                                            MessageBox.Show("地址 " + s1 + " 所在行出错：\r\n" + MyEx.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            break;
                                        }
                                    }
                                }
                            }
                            cmd.Transaction.Commit();
                        }
                    }
                }
            }
        }

        private void 取消超写ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                        int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                        cmd.Transaction = MyAccess.BeginTransaction();
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                string s1 = dataTable1.Rows[intFirstRow][0].ToString();
                                try
                                {
                                    dataTable1.Rows[intFirstRow][12] = false;
                                    dataTable1.Rows[intFirstRow][18] = 0;
                                    cmd.CommandText = "update athenaa set superlong = 0,codepage = 0,delphicodepage = 0 where address = '" + s1 + "'";
                                    cmd.ExecuteNonQuery();
                                    dataGridView1.Rows[intFirstRow].DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                                }
                                catch (Exception MyEx)
                                {
                                    MessageBox.Show("地址 " + s1 + " 所在行出错：\r\n" + MyEx.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    break;
                                }
                            }
                        }
                        cmd.Transaction.Commit();
                    }
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                if ((bool)dataGridView1.CurrentRow.Cells[16].Value == true)
                {
                    MessageBox.Show("请首先取消此项目矩阵设置再进行删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if ((int)dataGridView1.CurrentRow.Cells[11].Value > 0)
                {
                    MessageBox.Show("请首先取消此项目后移设置再进行删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if ((int)dataGridView1.CurrentRow.Cells[10].Value > 0)
                {
                    MessageBox.Show("请首先取消此项目前移设置再进行删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (dataGridView1.CurrentRow.Cells[8].Value.ToString() != "")
                {
                    MessageBox.Show("请首先取消此项目迁移设置再进行删除。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string s5 = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                    if (s5 != "")
                    {
                        Clipboard.Clear();
                        Clipboard.SetDataObject(s5);
                        MessageBox.Show("地址 " + s5 + " 在此处设置了迁移，请首先取消此处的迁移设置再进行删除。\r\n地址已复制到剪贴板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                try
                                {
                                    cmd.Transaction = MyAccess.BeginTransaction();
                                    cmd.CommandText = "delete from athenaa where address = '" + dataGridView1.CurrentRow.Cells[0].Value.ToString() + "'";
                                    cmd.ExecuteNonQuery();
                                    cmd.Transaction.Commit();
                                    int i1 = dataGridView1.CurrentRow.Index;
                                    dataTable1.Rows.RemoveAt(i1);
                                    if (dataTable1.Rows.Count > 0)
                                    {
                                        RowEnterDis(dataGridView1.CurrentRow.Index);
                                    }
                                    else
                                    {
                                        ClearText();
                                    }
                                }
                                catch (Exception MyEx)
                                {
                                    MessageBox.Show(MyEx.Message, "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void TGGreate()//生成目标
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                GenerateTarget GT = new GenerateTarget();
                if (MyDpi > 96F)
                {
                    GT.Font = MyNewFont;
                }
                GT.ShowDialog();
            }
        }

        private void 生成目标ToolStripMenuItem_Click(object sender, EventArgs e)//生成目标
        {
            TGGreate();
        }

        private void 添加ToolStripMenuItem_Click(object sender, EventArgs e)//自定义添加一条新的内容
        {
            AddString AS = new AddString();
            if (MyDpi > 96F)
            {
                AS.Font = MyNewFont;
            }
            AS.ShowDialog();
            if (AddStringbl == true)
            {
                AddStringbl = false;
                dataTable1.Clear();
                DisabledPanelContrl();
                dataGridView1.DataSource = null;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select count(address) from athenaa";
                            object addcount = cmd.ExecuteScalar();
                            int myaddcount = int.Parse(addcount.ToString());
                            if (myaddcount > 40000)
                            {
                                SPB = true;
                                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                toolStripProgressBar1.Visible = true;
                                ProgressBarTimer.Enabled = true;
                            }
                            BackgroundWorker AddStringBackgroundWorker = new BackgroundWorker();
                            AddStringBackgroundWorker.DoWork += AddStringBackgroundWorker_DoWork;
                            AddStringBackgroundWorker.RunWorkerCompleted += AddStringBackgroundWorker_RunWorkerCompleted;
                            AddStringBackgroundWorker.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private void AddStringBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void AddStringBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            int i2 = 0;
            if (PEbool)
            {
                Parallel.For(0, i1, (i) =>
                {
                    if (dataTable1.Rows[i][0].ToString() == AddStr)
                    {
                        i2 = i;
                    }
                    if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                    }
                    if (dataTable1.Rows[i][5].ToString() != "")
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                    }
                });
            }
            else
            {
                Parallel.For(0, i1, (i, ParallelLoopState) =>
                {
                    if (dataTable1.Rows[i][0].ToString() == AddStr)
                    {
                        ParallelLoopState.Break();
                        i2 = i;
                    }
                });
            }
            EnablePanelContrl();
            if (i2 > 9)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = i2 - 10;
            }
            dataGridView1.CurrentCell = dataGridView1[0, i2];
            dataGridView1.CurrentCell.Selected = true;
            RowEnterDis(i2);
            AddStr = "";
            this.Activate();
        }

        private void 编辑过滤FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterEdit FE = new FilterEdit();
            if (MyDpi > 96F)
            {
                FE.Font = MyNewFont;
                FE.dataGridView1.RowTemplate.Height = (int)(FE.dataGridView1.RowTemplate.Height * MyFontScale);
            }
            FE.ShowDialog();
        }

        private void 添加到过滤器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterMessage = true;
            AddFilter();
        }

        private void 过滤器FToolStripMenuItem_Click(object sender, EventArgs e)//过滤器添加
        {
            FilterMessage = false;
            AddFilter();
        }

        private void AddFilter()
        {
            int i1 = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            if (i1 > 0 && AddFilterstringBackgroundWorker.IsBusy == false)
            {
                string s1 = mainform.CDirectory + "过滤\\Filter.ini";
                if (File.Exists(s1) == false)
                {
                    MessageBox.Show("过滤配置文件不存在，无法添加过滤字符串。请打开过滤编辑器进行设定。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    using (StreamReader sr = File.OpenText(s1))
                    {
                        s1 = sr.ReadLine();
                    }
                    if (File.Exists(s1) == false)
                    {
                        MessageBox.Show("过滤文件不存在，请打开过滤编辑器进行设定。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (i1 > 1000)
                        {
                            DisabledPanelContrl();
                            toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                            toolStripProgressBar1.Visible = true;
                            ProgressBarTimer.Enabled = true;
                            SPB = true;
                        }
                        AddFilterstringBackgroundWorker.RunWorkerAsync(s1);
                    }
                }
            }
        }

        private void AddFilterstringBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string s1 = e.Argument.ToString();
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + s1))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                    for (; intFirstRow <= intLastRow; intFirstRow++)
                    {
                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                        {
                            if ((int)dataTable1.Rows[intFirstRow][4] == 0)
                            {
                                cmd.CommandText = "Insert Into filter (filterstr) Values ('" + dataTable1.Rows[intFirstRow][1].ToString().Replace("'", "''") + "')";
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
        }

        private void AddFilterstringBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            if (FilterMessage == true)
            {
                MessageBox.Show("过滤字符串添加成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            SPB = false;
            this.Activate();
        }

        private void toolStripButton21_Click(object sender, EventArgs e)//工程过滤
        {
            DialogResult dr = MessageBox.Show("确实要对当前工程进行过滤吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                string s1 = mainform.CDirectory + "过滤\\Filter.ini";
                if (File.Exists(s1) == false)
                {
                    MessageBox.Show("过滤设置不正确，请设置过滤器。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    using (StreamReader sr = File.OpenText(s1))
                    {
                        s1 = sr.ReadLine();
                    }
                    if (File.Exists(s1) == false)
                    {
                        MessageBox.Show("过滤文件不存在，请设置过滤器。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        int FilterCount = 0;
                        using (SQLiteConnection FilterAccess = new SQLiteConnection("Data Source=" + s1))
                        {
                            FilterAccess.Open();
                            using (SQLiteCommand FilterCMD = new SQLiteCommand(FilterAccess))
                            {
                                FilterCMD.CommandText = "select count(filterstr) from filter";
                                FilterCount = int.Parse(FilterCMD.ExecuteScalar().ToString());
                            }
                        }
                        if (FilterCount == 0)
                        {
                            MessageBox.Show("过滤文件中没有任何可用的内容，无法使用此功能。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (DateTime.Now.Minute < wt || (DateTime.Now.Minute - wt) > 10)
                            {
                                BackupTem();
                            }
                            DisabledPanelContrl();
                            toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                            toolStripProgressBar1.Visible = true;
                            ProgressBarTimer.Enabled = true;
                            SPB = true;
                            dataTable1.Rows.Clear();
                            dataGridView1.DataSource = null;
                            FilterProjectBackgroundWorker.RunWorkerAsync(s1);
                        }
                    }
                }
            }
        }

        private void FilterProjectBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)//过滤
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "ATTACH DATABASE '" + e.Argument.ToString() + "' AS Filterdata";
                    cmd.ExecuteNonQuery();
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "delete from athenaa where athenaa.address in (select athenaa.address from athenaa join Filterdata.filter on athenaa.org = Filterdata.filter.filterstr) and athenaa.tralong = 0";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "DETACH DATABASE Filterdata";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "VACUUM";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void FilterProjectBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                RowEnterDis(dataGridView1.CurrentRow.Index);
            }
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            MessageBox.Show("过滤完成。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            SPB = false;
            this.Activate();
        }

        private void mainform_FormClosing(object sender, FormClosingEventArgs e)//退出程序询问
        {
            if (BackupProjectTaskbool == true || FilterProjectBackgroundWorker.IsBusy == true || AddFilterstringBackgroundWorker.IsBusy == true)
            {
                DialogResult dr = MessageBox.Show("程序正忙，确实要退出程序吗？", "确定", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void 添加到字典ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(DictionaryStr) == false)
            {
                MessageBox.Show("请首先挂接一个字典文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string s1 = "";
                string s2 = "";
                ArrayList tem1 = new ArrayList();
                ArrayList tem2 = new ArrayList();
                int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                for (; intFirstRow <= intLastRow; intFirstRow++)
                {
                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                    {
                        s1 = dataTable1.Rows[intFirstRow][1].ToString();
                        s2 = dataTable1.Rows[intFirstRow][2].ToString();
                        if (s2 != "" && s1 != s2)
                        {
                            tem1.Add(s1);
                            tem2.Add(s2);
                        }
                    }
                }
                int i1 = tem1.Count;
                if (i1 > 0)
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + DictionaryStr))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            for (int y = 0; y < i1; y++)
                            {
                                s1 = tem1[y].ToString().Replace("'", "''");
                                s2 = tem2[y].ToString().Replace("'", "''");
                                cmd.CommandText = "Insert Into tbl (org, tra) Values ('" + s1 + "','" + s2 + "')";
                                cmd.ExecuteNonQuery();
                            }
                            for (int y = 0; y < i1; y++)
                            {
                                dataTable6.Rows.Add(tem1[y].ToString(), tem2[y].ToString());
                            }
                            cmd.Transaction.Commit();
                        }
                    }
                    MessageBox.Show("添加到字典成功。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void 生成并运行LToolStripMenuItem_Click(object sender, EventArgs e)//生成运行
        {
            AutoTest();
        }

        private void toolStripTextBox4_Click(object sender, EventArgs e)//转换字符长度格式
        {
            ConvertStringLengthFormat();
        }

        private void toolStripTextBox5_Click(object sender, EventArgs e)
        {
            ConvertStringLengthFormat();
        }

        private void 矩阵ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Matrix Mx = new Matrix();
                if (MyDpi > 96F)
                {
                    Mx.Font = MyNewFont;
                    Mx.dataGridView1.RowTemplate.Height = (int)(Mx.dataGridView1.RowTemplate.Height * MyFontScale);
                    Mx.dataGridView1.Columns[0].Width = (int)Graphics.FromHwnd(IntPtr.Zero).MeasureString("DDDDDDDD", MyNewFont).Width;
                }
                int MyIndex = dataGridView1.CurrentRow.Index;
                for (int x = 0; x < 22; x++)
                {
                    obMS[x] = dataTable1.Rows[MyIndex][x];
                }
                Mx.ShowDialog();
                MoveAddressHexView(obMS[0].ToString(), (int)obMS[3]);
                dataTable1.Rows[MyIndex][16] = obMS[16];
                dataTable1.Rows[MyIndex][18] = obMS[18];
                dataTable1.Rows[MyIndex][19] = false;
                if ((bool)obMS[16])
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                }
                else
                {
                    dataGridView1.CurrentRow.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
                }
            }
        }

        private void 文本删除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            TextDelete TD = new TextDelete();
            if (MyDpi > 96F)
            {
                TD.comboBox1.Font = MyNewFont;
                TD.comboBox2.Font = MyNewFont;
                TD.comboBox3.Font = MyNewFont;
                TD.Font = MyNewFont;
            }
            TD.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 文本替换RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            TextReplace TR = new TextReplace();
            if (MyDpi > 96F)
            {
                TR.comboBox1.Font = MyNewFont;
                TR.Font = MyNewFont;
            }
            TR.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 取消翻译ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识   
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int i1 = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (i1 > 0 && i1 < 2000)
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            cmd.Transaction = MyAccess.BeginTransaction();
                            int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                            int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                            if (PEbool)
                            {
                                for (; intFirstRow <= intLastRow; intFirstRow++)
                                {
                                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                    {
                                        if (dataTable1.Rows[intFirstRow][2].ToString() != "")
                                        {
                                            if ((bool)dataTable1.Rows[intFirstRow][16] == false)//不是矩阵
                                            {
                                                if ((bool)dataTable1.Rows[intFirstRow][12] == false)//不是超写
                                                {
                                                    if ((int)dataTable1.Rows[intFirstRow][11] == 0)//不是后移
                                                    {
                                                        if ((int)dataTable1.Rows[intFirstRow][10] == 0)//不是前移
                                                        {
                                                            if (dataTable1.Rows[intFirstRow][8].ToString() == "")//不是迁移
                                                            {
                                                                if (dataTable1.Rows[intFirstRow][5].ToString() == "")//也没有被迁移借用
                                                                {
                                                                    cmd.CommandText = "update athenaa set tra = ''," + "tralong = 0,free = " + (int)dataTable1.Rows[intFirstRow][3] + " where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                                                    cmd.ExecuteNonQuery();
                                                                    dataTable1.Rows[intFirstRow][2] = "";
                                                                    dataTable1.Rows[intFirstRow][4] = 0;
                                                                    dataTable1.Rows[intFirstRow][7] = dataTable1.Rows[intFirstRow][3];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (; intFirstRow <= intLastRow; intFirstRow++)
                                {
                                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                    {
                                        cmd.CommandText = "update athenaa set tra = ''," + "tralong = 0,free = " + (int)dataTable1.Rows[intFirstRow][3] + " where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                        cmd.ExecuteNonQuery();
                                        dataTable1.Rows[intFirstRow][2] = "";
                                        dataTable1.Rows[intFirstRow][4] = 0;
                                        dataTable1.Rows[intFirstRow][7] = dataTable1.Rows[intFirstRow][3];
                                    }
                                }
                            }
                            cmd.Transaction.Commit();
                        }
                        textBox1.Clear();
                    }
                }
                else if (i1 >= 2000)
                {
                    DisabledPanelContrl();
                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                    toolStripProgressBar1.Visible = true;
                    SPB = true;
                    ProgressBarTimer.Enabled = true;
                    using (BackgroundWorker UndoTranslated = new BackgroundWorker())
                    {
                        UndoTranslated.DoWork += UndoTranslated_DoWork;
                        UndoTranslated.RunWorkerCompleted += UndoTranslated_RunWorkerCompleted;
                        UndoTranslated.RunWorkerAsync();
                    }
                }
            }
        }

        private void UndoTranslated_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.Transaction = MyAccess.BeginTransaction();
                    int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                    if (PEbool)
                    {
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                if (dataTable1.Rows[intFirstRow][2].ToString() != "")
                                {
                                    if ((bool)dataTable1.Rows[intFirstRow][16] == false)//不是矩阵
                                    {
                                        if ((bool)dataTable1.Rows[intFirstRow][12] == false)//不是超写
                                        {
                                            if ((int)dataTable1.Rows[intFirstRow][11] == 0)//不是后移
                                            {
                                                if ((int)dataTable1.Rows[intFirstRow][10] == 0)//不是前移
                                                {
                                                    if (dataTable1.Rows[intFirstRow][8].ToString() == "")//不是迁移
                                                    {
                                                        if (dataTable1.Rows[intFirstRow][5].ToString() == "")//也没有被迁移借用
                                                        {
                                                            cmd.CommandText = "update athenaa set tra = ''," + "tralong = 0,free = " + (int)dataTable1.Rows[intFirstRow][3] + " where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                                            cmd.ExecuteNonQuery();
                                                            dataTable1.Rows[intFirstRow][2] = "";
                                                            dataTable1.Rows[intFirstRow][4] = 0;
                                                            dataTable1.Rows[intFirstRow][7] = dataTable1.Rows[intFirstRow][3];
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                cmd.CommandText = "update athenaa set tra = ''," + "tralong = 0,free = " + (int)dataTable1.Rows[intFirstRow][3] + " where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                cmd.ExecuteNonQuery();
                                dataTable1.Rows[intFirstRow][2] = "";
                                dataTable1.Rows[intFirstRow][4] = 0;
                                dataTable1.Rows[intFirstRow][7] = dataTable1.Rows[intFirstRow][3];
                            }
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
        }

        private void UndoTranslated_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            textBox1.Clear();
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
        }

        private void 翻译ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int i1 = dataTable6.Rows.Count;
                if (i1 == 0)
                {
                    MessageBox.Show("没有挂接字典或字典为空，无法进行翻译。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    int selInt = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                    if (selInt > 0 && selInt < 2000)
                    {
                        using (DataTable dtTmp = new DataTable("dtTmp"))
                        {
                            object[] obTmp = new object[4];
                            obTmp[3] = "";
                            dtTmp.Columns.Add(new DataColumn("MyIndex", typeof(Int32)));
                            dtTmp.Columns.Add(new DataColumn("address", typeof(string)));
                            dtTmp.Columns.Add(new DataColumn("org", typeof(string)));
                            dtTmp.Columns.Add(new DataColumn("tra", typeof(string)));
                            int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                            int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                            for (; intFirstRow <= intLastRow; intFirstRow++)
                            {
                                if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                {
                                    string org = dataTable1.Rows[intFirstRow][1].ToString();//原文
                                    string tra = dataTable1.Rows[intFirstRow][2].ToString();//译文
                                    if (tra == "" || tra == org)
                                    {
                                        obTmp[0] = intFirstRow;
                                        obTmp[1] = dataTable1.Rows[intFirstRow][0].ToString();
                                        obTmp[2] = org;
                                        dtTmp.Rows.Add(obTmp);
                                    }
                                }
                            }
                            if (dtTmp.Rows.Count > 0)
                            {
                                if (MatchCase)
                                {
                                    var dtResult = from dt1 in dtTmp.AsEnumerable()
                                                   join dt2 in dataTable6.AsEnumerable()
                                                   on dt1.Field<string>("org") equals dt2.Field<string>("org")
                                                   select new
                                                   {
                                                       v1 = dt1.Field<Int32>("MyIndex"),
                                                       v2 = dt1.Field<string>("address"),
                                                       v3 = dt1.Field<string>("org"),
                                                       v4 = dt2.Field<string>("tra")
                                                   };
                                    ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                                    {
                                        MyAccess.Open();
                                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                        {
                                            int tralong4 = 0;
                                            int free7 = 0;
                                            cmd.Transaction = MyAccess.BeginTransaction();
                                            foreach (var x in dtResult)
                                            {
                                                int y = x.v1;
                                                if (cdTmp.TryAdd(y, 0))
                                                {
                                                    string org = x.v3;
                                                    string tra = x.v4;
                                                    if (tra != "")
                                                    {
                                                        if (org.Contains("\r") == true && org.Contains("\n") == false)
                                                        {
                                                            tra = tra.Replace("\r\n", "\r");
                                                        }
                                                        else if (org.Contains("\r") == false && org.Contains("\n") == true)
                                                        {
                                                            tra = tra.Replace("\r\n", "\n");
                                                        }
                                                        if ((bool)dataTable1.Rows[y][14])//utf8
                                                        {
                                                            tralong4 = Encoding.UTF8.GetByteCount(tra);
                                                        }
                                                        else if ((bool)dataTable1.Rows[y][15])//Unicode
                                                        {
                                                            tralong4 = Encoding.Unicode.GetByteCount(tra);
                                                        }
                                                        else
                                                        {
                                                            tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(tra);
                                                        }
                                                        free7 = (int)dataTable1.Rows[y][3] - tralong4;//剩余长度
                                                        dataTable1.Rows[y][2] = tra;
                                                        dataTable1.Rows[y][4] = tralong4;
                                                        dataTable1.Rows[y][7] = free7;
                                                        tra = tra.Replace("'", "''");
                                                        cmd.CommandText = "update athenaa set tra = '" + tra + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + x.v2 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            cmd.Transaction.Commit();
                                            string s2 = textBox2.Text;
                                            string s3 = textBox3.Text;
                                            if (s3.Contains("\r") == true && s3.Contains("\n") == false)
                                            {
                                                s2 = s2.Replace("\r", "\r\n");
                                            }
                                            else if (s3.Contains("\r") == false && s3.Contains("\n") == true)
                                            {
                                                s2 = s2.Replace("\n", "\r\n");
                                            }
                                            textBox1.Text = s2;
                                        }
                                    }
                                }
                                else
                                {
                                    var dtResult = from dt1 in dtTmp.AsEnumerable()
                                                   join dt2 in dataTable6.AsEnumerable()
                                                   on dt1.Field<string>("org").ToLower() equals dt2.Field<string>("org").ToLower()
                                                   select new
                                                   {
                                                       v1 = dt1.Field<Int32>("MyIndex"),
                                                       v2 = dt1.Field<string>("address"),
                                                       v3 = dt1.Field<string>("org"),
                                                       v4 = dt2.Field<string>("tra")
                                                   };
                                    ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                                    {
                                        MyAccess.Open();
                                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                        {
                                            int tralong4 = 0;
                                            int free7 = 0;
                                            cmd.Transaction = MyAccess.BeginTransaction();
                                            foreach (var x in dtResult)
                                            {
                                                int y = x.v1;
                                                if (cdTmp.TryAdd(y, 0))
                                                {
                                                    string org = x.v3;
                                                    string tra = x.v4;
                                                    if (tra != "")
                                                    {
                                                        if (org.Contains("\r") == true && org.Contains("\n") == false)
                                                        {
                                                            tra = tra.Replace("\r\n", "\r");
                                                        }
                                                        else if (org.Contains("\r") == false && org.Contains("\n") == true)
                                                        {
                                                            tra = tra.Replace("\r\n", "\n");
                                                        }
                                                        if ((bool)dataTable1.Rows[y][14])//utf8
                                                        {
                                                            tralong4 = Encoding.UTF8.GetByteCount(tra);
                                                        }
                                                        else if ((bool)dataTable1.Rows[y][15])//Unicode
                                                        {
                                                            tralong4 = Encoding.Unicode.GetByteCount(tra);
                                                        }
                                                        else
                                                        {
                                                            tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(tra);
                                                        }
                                                        free7 = (int)dataTable1.Rows[y][3] - tralong4;//剩余长度
                                                        dataTable1.Rows[y][2] = tra;
                                                        dataTable1.Rows[y][4] = tralong4;
                                                        dataTable1.Rows[y][7] = free7;
                                                        tra = tra.Replace("'", "''");
                                                        cmd.CommandText = "update athenaa set tra = '" + tra + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + x.v2 + "'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                }
                                            }
                                            cmd.Transaction.Commit();
                                            string s2 = textBox2.Text;
                                            string s3 = textBox3.Text;
                                            if (s3.Contains("\r") == true && s3.Contains("\n") == false)
                                            {
                                                s2 = s2.Replace("\r", "\r\n");
                                            }
                                            else if (s3.Contains("\r") == false && s3.Contains("\n") == true)
                                            {
                                                s2 = s2.Replace("\n", "\r\n");
                                            }
                                            textBox1.Text = s2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (selInt >= 2000)
                    {
                        DisabledPanelContrl();
                        toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                        toolStripProgressBar1.Visible = true;
                        SPB = true;
                        ProgressBarTimer.Enabled = true;
                        using (BackgroundWorker TranslateBackgroundWorker = new BackgroundWorker())
                        {
                            TranslateBackgroundWorker.DoWork += TranslateBackgroundWorker_DoWork;
                            TranslateBackgroundWorker.RunWorkerCompleted += TranslateBackgroundWorker_RunWorkerCompleted;
                            TranslateBackgroundWorker.RunWorkerAsync();
                        }
                    }
                }
            }
        }

        private void TranslateBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DataTable dtTmp = new DataTable("dtTmp"))
            {
                object[] obTmp = new object[4];
                obTmp[3] = "";
                dtTmp.Columns.Add(new DataColumn("MyIndex", typeof(Int32)));
                dtTmp.Columns.Add(new DataColumn("address", typeof(string)));
                dtTmp.Columns.Add(new DataColumn("org", typeof(string)));
                dtTmp.Columns.Add(new DataColumn("tra", typeof(string)));
                int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                for (; intFirstRow <= intLastRow; intFirstRow++)
                {
                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                    {
                        string org = dataTable1.Rows[intFirstRow][1].ToString();//原文
                        string tra = dataTable1.Rows[intFirstRow][2].ToString();//译文
                        if (tra == "" || tra == org)
                        {
                            obTmp[0] = intFirstRow;
                            obTmp[1] = dataTable1.Rows[intFirstRow][0].ToString();
                            obTmp[2] = org;
                            dtTmp.Rows.Add(obTmp);
                        }
                    }
                }
                if (dtTmp.Rows.Count > 0)
                {
                    if (MatchCase)
                    {
                        var dtResult = from dt1 in dtTmp.AsEnumerable()
                                       join dt2 in dataTable6.AsEnumerable()
                                       on dt1.Field<string>("org") equals dt2.Field<string>("org")
                                       select new
                                       {
                                           v1 = dt1.Field<Int32>("MyIndex"),
                                           v2 = dt1.Field<string>("address"),
                                           v3 = dt1.Field<string>("org"),
                                           v4 = dt2.Field<string>("tra")
                                       };
                        ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                int tralong4 = 0;
                                int free7 = 0;
                                cmd.Transaction = MyAccess.BeginTransaction();
                                foreach (var x in dtResult)
                                {
                                    int y = x.v1;
                                    if (cdTmp.TryAdd(y, 0))
                                    {
                                        string org = x.v3;
                                        string tra = x.v4;
                                        if (tra != "")
                                        {
                                            if (org.Contains("\r") == true && org.Contains("\n") == false)
                                            {
                                                tra = tra.Replace("\r\n", "\r");
                                            }
                                            else if (org.Contains("\r") == false && org.Contains("\n") == true)
                                            {
                                                tra = tra.Replace("\r\n", "\n");
                                            }
                                            if ((bool)dataTable1.Rows[y][14])//utf8
                                            {
                                                tralong4 = Encoding.UTF8.GetByteCount(tra);
                                            }
                                            else if ((bool)dataTable1.Rows[y][15])//Unicode
                                            {
                                                tralong4 = Encoding.Unicode.GetByteCount(tra);
                                            }
                                            else
                                            {
                                                tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(tra);
                                            }
                                            free7 = (int)dataTable1.Rows[y][3] - tralong4;//剩余长度
                                            dataTable1.Rows[y][2] = tra;
                                            dataTable1.Rows[y][4] = tralong4;
                                            dataTable1.Rows[y][7] = free7;
                                            tra = tra.Replace("'", "''");
                                            cmd.CommandText = "update athenaa set tra = '" + tra + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + x.v2 + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                cmd.Transaction.Commit();
                            }
                        }
                    }
                    else
                    {
                        var dtResult = from dt1 in dtTmp.AsEnumerable()
                                       join dt2 in dataTable6.AsEnumerable()
                                       on dt1.Field<string>("org").ToLower() equals dt2.Field<string>("org").ToLower()
                                       select new
                                       {
                                           v1 = dt1.Field<Int32>("MyIndex"),
                                           v2 = dt1.Field<string>("address"),
                                           v3 = dt1.Field<string>("org"),
                                           v4 = dt2.Field<string>("tra")
                                       };
                        ConcurrentDictionary<Int32, Int32> cdTmp = new ConcurrentDictionary<Int32, Int32>();
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                int tralong4 = 0;
                                int free7 = 0;
                                cmd.Transaction = MyAccess.BeginTransaction();
                                foreach (var x in dtResult)
                                {
                                    int y = x.v1;
                                    if (cdTmp.TryAdd(y, 0))
                                    {
                                        string org = x.v3;
                                        string tra = x.v4;
                                        if (tra != "")
                                        {
                                            if (org.Contains("\r") == true && org.Contains("\n") == false)
                                            {
                                                tra = tra.Replace("\r\n", "\r");
                                            }
                                            else if (org.Contains("\r") == false && org.Contains("\n") == true)
                                            {
                                                tra = tra.Replace("\r\n", "\n");
                                            }
                                            if ((bool)dataTable1.Rows[y][14])//utf8
                                            {
                                                tralong4 = Encoding.UTF8.GetByteCount(tra);
                                            }
                                            else if ((bool)dataTable1.Rows[y][15])//Unicode
                                            {
                                                tralong4 = Encoding.Unicode.GetByteCount(tra);
                                            }
                                            else
                                            {
                                                tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(tra);
                                            }
                                            free7 = (int)dataTable1.Rows[y][3] - tralong4;//剩余长度
                                            dataTable1.Rows[y][2] = tra;
                                            dataTable1.Rows[y][4] = tralong4;
                                            dataTable1.Rows[y][7] = free7;
                                            tra = tra.Replace("'", "''");
                                            cmd.CommandText = "update athenaa set tra = '" + tra + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + x.v2 + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                                cmd.Transaction.Commit();
                            }
                        }
                    }
                }
            }
        }

        private void TranslateBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            string s2 = textBox2.Text;
            string s3 = textBox3.Text;
            if (s3.Contains("\r") == true && s3.Contains("\n") == false)
            {
                s2 = s2.Replace("\r", "\r\n");
            }
            else if (s3.Contains("\r") == false && s3.Contains("\n") == true)
            {
                s2 = s2.Replace("\n", "\r\n");
            }
            textBox1.Text = s2;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
        }

        private void 统计SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Statistics St = new Statistics();
                if (MyDpi > 96F)
                {
                    St.Font = MyNewFont;
                }
                St.ShowDialog();
            }
        }

        private void 替换PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataTable1.Rows.Count > 0)
            {
                if (File.Exists(ProjectFileName) == false)
                {
                    MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    RStr1 = "";
                    RStr2 = "";
                    ReplaceString RS = new ReplaceString();
                    if (MyDpi > 96F)
                    {
                        RS.Font = MyNewFont;
                    }
                    RS.ShowDialog();
                    if (RStr1 != "" && RStr1 != RStr2)
                    {
                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                        {
                            MyAccess.Open();
                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                            {
                                int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                                int i2 = Encoding.GetEncoding(ProTraCode).GetByteCount(RStr1);//原文长度
                                int i3 = Encoding.GetEncoding(ProTraCode).GetByteCount(RStr2);//替换长度
                                int tralong4 = 0;
                                int free7 = 0;
                                string s0 = "";
                                string s2 = "";
                                if (i2 == i3)
                                {
                                    for (; intFirstRow <= intLastRow; intFirstRow++)
                                    {
                                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                        {
                                            s2 = dataTable1.Rows[intFirstRow][2].ToString();
                                            if (s2 != "")
                                            {
                                                s0 = dataTable1.Rows[intFirstRow][0].ToString();//地址
                                                s2 = s2.Replace(RStr1, RStr2);
                                                s2 = s2.Replace("'", "''");
                                                cmd.CommandText = "update athenaa set tra = '" + s2 + "' where address = '" + s0 + "'";
                                                cmd.ExecuteNonQuery();
                                                s2 = s2.Replace("''", "'");
                                                dataTable1.Rows[intFirstRow][2] = s2;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (; intFirstRow <= intLastRow; intFirstRow++)
                                    {
                                        if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                        {
                                            s2 = dataTable1.Rows[intFirstRow][2].ToString();
                                            if (s2 != "" && dataTable1.Rows[intFirstRow][5].ToString() == "" && dataTable1.Rows[intFirstRow][8].ToString() == "" && dataTable1.Rows[intFirstRow][10].ToString() == "0" && dataTable1.Rows[intFirstRow][11].ToString() == "0" && dataTable1.Rows[intFirstRow][12].ToString() == "False" && dataTable1.Rows[intFirstRow][16].ToString() == "False")
                                            {
                                                s0 = dataTable1.Rows[intFirstRow][0].ToString();//地址
                                                s2 = s2.Replace(RStr1, RStr2);
                                                if ((bool)dataTable1.Rows[intFirstRow][14])//utf8
                                                {
                                                    tralong4 = Encoding.UTF8.GetByteCount(s2);
                                                }
                                                else if ((bool)dataTable1.Rows[intFirstRow][15])//Unicode
                                                {
                                                    tralong4 = Encoding.Unicode.GetByteCount(s2);
                                                }
                                                else
                                                {
                                                    tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(s2);
                                                }
                                                free7 = (int)dataTable1.Rows[intFirstRow][3] - tralong4;//剩余长度
                                                dataTable1.Rows[intFirstRow][2] = s2;
                                                dataTable1.Rows[intFirstRow][4] = tralong4;
                                                dataTable1.Rows[intFirstRow][7] = free7;
                                                s2 = s2.Replace("'", "''");
                                                cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + tralong4 + ",free = " + free7 + " where address = '" + s0 + "'";
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
            else
            {
                MessageBox.Show("请选择需要替换的项目。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void 标记位置CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataTable1.Rows.Count > 0)
            {
                if (File.Exists(ProjectFileName) == false)
                {
                    MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            Marker = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                            Marker = dataTable1.Rows[dataGridView1.CurrentRow.Index][0].ToString();
                            cmd.Transaction = MyAccess.BeginTransaction();
                            cmd.CommandText = "update fileinfo set detail = '" + Marker + "' where infoname = '标记'";
                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                        }
                    }
                }
            }
        }

        private void 删除标记DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        cmd.Transaction = MyAccess.BeginTransaction();
                        Marker = "";
                        cmd.CommandText = "update fileinfo set detail = '' where infoname = '标记'";
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                }
            }
        }

        private void 转到标记GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Marker != "")
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select count(address) from athenaa";
                            object addcount = cmd.ExecuteScalar();
                            int myaddcount = int.Parse(addcount.ToString());
                            if (myaddcount > 0)
                            {
                                if (myaddcount == dataTable1.Rows.Count)
                                {
                                    int i2 = -1;
                                    Parallel.For(0, myaddcount, (i, ParallelLoopState) =>
                                    {
                                        if (dataTable1.Rows[i][0].ToString() == Marker)
                                        {
                                            ParallelLoopState.Break();
                                            i2 = i;
                                        }
                                    });
                                    if (i2 > -1)
                                    {
                                        if (i2 > 9)
                                        {
                                            dataGridView1.FirstDisplayedScrollingRowIndex = i2 - 10;
                                        }
                                        dataGridView1.CurrentCell = dataGridView1[0, i2];
                                        dataGridView1.CurrentCell.Selected = true;
                                        RowEnterDis(i2);
                                    }
                                    else
                                    {
                                        MessageBox.Show("没有找到标记的地址，标记地址所在行已经被删除。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    dataTable1.Clear();
                                    DisabledPanelContrl();
                                    dataGridView1.DataSource = null;
                                    if (myaddcount > 40000)
                                    {
                                        SPB = true;
                                        toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                        toolStripProgressBar1.Visible = true;
                                        ProgressBarTimer.Enabled = true;
                                    }
                                    BackgroundWorker MarkerAddressBackgroundWorker = new BackgroundWorker();
                                    MarkerAddressBackgroundWorker.DoWork += MarkerAddressBackgroundWorker_DoWork;
                                    MarkerAddressBackgroundWorker.RunWorkerCompleted += MarkerAddressBackgroundWorker_RunWorkerCompleted;
                                    MarkerAddressBackgroundWorker.RunWorkerAsync();
                                }
                            }
                            else
                            {
                                MessageBox.Show("工程中没有包含任何数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("没有标记的地址。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MarkerAddressBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void MarkerAddressBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            int i2 = -1;
            EnablePanelContrl();
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == Marker)
                        {
                            i2 = i;
                        }
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                else
                {
                    Parallel.For(0, i1, (i, ParallelLoopState) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == Marker)
                        {
                            ParallelLoopState.Break();
                            i2 = i;
                        }
                    });
                }
            }
            if (i2 > -1)
            {
                if (i2 > 9)
                {
                    dataGridView1.FirstDisplayedScrollingRowIndex = i2 - 10;
                }
                dataGridView1.CurrentCell = dataGridView1[0, i2];
                dataGridView1.CurrentCell.Selected = true;
                RowEnterDis(i2);
            }
            else
            {
                if (i1 > 0)
                {
                    RowEnterDis(0);
                }
                MessageBox.Show("没有找到标记的地址，标记地址所在行已经被删除。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Activate();
        }

        private void 恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int i1 = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (i1 > 0 && i1 < 2000)
                {
                    string s2 = "";
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            int i3 = 0;
                            int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                            int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                            cmd.Transaction = MyAccess.BeginTransaction();
                            if (UnPEType == "6")
                            {
                                for (; intFirstRow <= intLastRow; intFirstRow++)
                                {
                                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                    {
                                        if (dataTable1.Rows[intFirstRow][1].ToString().Contains("msgstr") && dataTable1.Rows[intFirstRow][2].ToString() == "")
                                        {
                                            s2 = dataTable1.Rows[intFirstRow][1].ToString();
                                            i3 = (int)dataTable1.Rows[intFirstRow][3];
                                            dataTable1.Rows[intFirstRow][2] = s2;
                                            dataTable1.Rows[intFirstRow][4] = i3;
                                            dataTable1.Rows[intFirstRow][7] = 0;
                                            s2 = s2.Replace("'", "''");
                                            cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + i3 + ",free = 0 where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                for (; intFirstRow <= intLastRow; intFirstRow++)
                                {
                                    if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                                    {
                                        if (dataTable1.Rows[intFirstRow][2].ToString() == "")
                                        {
                                            s2 = dataTable1.Rows[intFirstRow][1].ToString();
                                            i3 = (int)dataTable1.Rows[intFirstRow][3];
                                            dataTable1.Rows[intFirstRow][2] = s2;
                                            dataTable1.Rows[intFirstRow][4] = i3;
                                            dataTable1.Rows[intFirstRow][7] = 0;
                                            s2 = s2.Replace("'", "''");
                                            cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + i3 + ",free = 0 where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                            cmd.Transaction.Commit();
                        }
                    }
                    if (textBox1.Text == "")
                    {
                        s2 = textBox3.Text;
                        if (s2.Contains("\r") == true && s2.Contains("\n") == false)
                        {
                            s2 = s2.Replace("\r", "\r\n");
                        }
                        else if (s2.Contains("\r") == false && s2.Contains("\n") == true)
                        {
                            s2 = s2.Replace("\n", "\r\n");
                        }
                        textBox1.Text = s2;
                    }
                }
                else if (i1 >= 2000)
                {
                    DisabledPanelContrl();
                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                    toolStripProgressBar1.Visible = true;
                    SPB = true;
                    ProgressBarTimer.Enabled = true;
                    using (BackgroundWorker RestoreBackgroundWorker = new BackgroundWorker())
                    {
                        RestoreBackgroundWorker.DoWork += RestoreBackgroundWorker_DoWork;
                        RestoreBackgroundWorker.RunWorkerCompleted += RestoreBackgroundWorker_RunWorkerCompleted;
                        RestoreBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void RestoreBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
                    int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected);
                    int i3 = 0;
                    cmd.Transaction = MyAccess.BeginTransaction();
                    if (UnPEType == "6")
                    {
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                if (dataTable1.Rows[intFirstRow][1].ToString().Contains("msgstr") && dataTable1.Rows[intFirstRow][2].ToString() == "")
                                {
                                    string s2 = dataTable1.Rows[intFirstRow][1].ToString();
                                    i3 = (int)dataTable1.Rows[intFirstRow][3];
                                    dataTable1.Rows[intFirstRow][2] = s2;
                                    dataTable1.Rows[intFirstRow][4] = i3;
                                    dataTable1.Rows[intFirstRow][7] = 0;
                                    s2 = s2.Replace("'", "''");
                                    cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + i3 + ",free = 0 where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        for (; intFirstRow <= intLastRow; intFirstRow++)
                        {
                            if (((int)dataGridView1.Rows.GetRowState(intFirstRow) & 32) == 32)
                            {
                                if (dataTable1.Rows[intFirstRow][2].ToString() == "")
                                {
                                    string s2 = dataTable1.Rows[intFirstRow][1].ToString();
                                    i3 = (int)dataTable1.Rows[intFirstRow][3];
                                    dataTable1.Rows[intFirstRow][2] = s2;
                                    dataTable1.Rows[intFirstRow][4] = i3;
                                    dataTable1.Rows[intFirstRow][7] = 0;
                                    s2 = s2.Replace("'", "''");
                                    cmd.CommandText = "update athenaa set tra = '" + s2 + "'," + "tralong = " + i3 + ",free = 0 where address = '" + dataTable1.Rows[intFirstRow][0].ToString() + "'";
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    cmd.Transaction.Commit();
                }
            }
        }

        private void RestoreBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            if (textBox1.Text == "")
            {
                string s2 = textBox3.Text;
                if (s2.Contains("\r") == true && s2.Contains("\n") == false)
                {
                    s2 = s2.Replace("\r", "\r\n");
                }
                else if (s2.Contains("\r") == false && s2.Contains("\n") == true)
                {
                    s2 = s2.Replace("\n", "\r\n");
                }
                textBox1.Text = s2;
            }
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
        }

        private void 水平滚动ToolStripMenuItem_Click(object sender, EventArgs e)
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
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.Both;
                }
            }
            else
            {
                if (textBox1.ScrollBars == ScrollBars.Both)
                {
                    textBox1.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.None;
                }
            }
        }

        private void 垂直滚动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (垂直滚动VToolStripMenuItem.Checked == false)
            {
                垂直滚动VToolStripMenuItem.Checked = true;
            }
            else
            {
                垂直滚动VToolStripMenuItem.Checked = false;
            }
            if (垂直滚动VToolStripMenuItem.Checked == true)
            {
                if (textBox1.ScrollBars == ScrollBars.None)
                {
                    textBox1.ScrollBars = ScrollBars.Vertical;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.Both;
                }
            }
            else
            {
                if (textBox1.ScrollBars == ScrollBars.Both)
                {
                    textBox1.ScrollBars = ScrollBars.Horizontal;
                }
                else
                {
                    textBox1.ScrollBars = ScrollBars.None;
                }
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (MatchCase == false)
            {
                MatchCase = true;
                if (MyDpi < 144F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.Case18;
                }
                else if (MyDpi >= 144F && MyDpi < 192F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.Case24;
                }
                else if (MyDpi >= 192F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.Case32;
                }
                toolStripButton6.Text = "区分大小写";
            }
            else
            {
                MatchCase = false;
                if (MyDpi < 144F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.NoCase18;
                }
                else if (MyDpi >= 144F && MyDpi < 192F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.NoCase24;
                }
                else if (MyDpi >= 192F)
                {
                    toolStripButton6.Image = Athena_A.Properties.Resources.NoCase32;
                }
                toolStripButton6.Text = "忽略大小写";
            }
        }

        private void 过滤保留SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterSave FS = new FilterSave();
            if (MyDpi > 96F)
            {
                FS.Font = MyNewFont;
                FS.dataGridView1.RowTemplate.Height = (int)(FS.dataGridView1.RowTemplate.Height * MyFontScale);
            }
            FS.ShowDialog();
        }

        private void 显示字体FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (orgDataGridViewTextBoxColumn.DefaultCellStyle.Font != null)
            {
                ProjectFont.ProjectOrgName = orgDataGridViewTextBoxColumn.DefaultCellStyle.Font.Name;
                ProjectFont.ProjectOrgSize = orgDataGridViewTextBoxColumn.DefaultCellStyle.Font.Size.ToString();
            }
            else
            {
                ProjectFont.ProjectOrgName = this.Font.Name;
                ProjectFont.ProjectOrgSize = this.Font.Size.ToString();
            }
            if (traDataGridViewTextBoxColumn.DefaultCellStyle.Font != null)
            {
                ProjectFont.ProjectTraName = traDataGridViewTextBoxColumn.DefaultCellStyle.Font.Name;
                ProjectFont.ProjectTraSize = traDataGridViewTextBoxColumn.DefaultCellStyle.Font.Size.ToString();
            }
            else
            {
                ProjectFont.ProjectTraName = this.Font.Name;
                ProjectFont.ProjectTraSize = this.Font.Size.ToString();
            }
            ProjectFont PF = new ProjectFont();
            if (MyDpi > 96F)
            {
                PF.Font = MyNewFont;
            }
            PF.ShowDialog();
            Font Orgfont = new Font(ProjectFont.ProjectOrgName, float.Parse(ProjectFont.ProjectOrgSize));
            orgDataGridViewTextBoxColumn.DefaultCellStyle.Font = Orgfont;
            textBox3.Font = Orgfont;
            if (toolStripComboBox2.Text == "原文")
            {
                toolStripTextBox2.Font = Orgfont;
            }
            Font Trafont = new Font(ProjectFont.ProjectTraName, float.Parse(ProjectFont.ProjectTraSize));
            traDataGridViewTextBoxColumn.DefaultCellStyle.Font = Trafont;
            textBox1.Font = Trafont;
            textBox2.Font = Trafont;
            if (toolStripComboBox2.Text == "译文")
            {
                toolStripTextBox2.Font = Trafont;
            }
            if (File.Exists(ProjectFileName) == true)
            {
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        if (ProjectFont.ProjectOrgName == "" && ProjectFont.ProjectTraName == "")
                        {
                            cmd.CommandText = "update prolanguage set orgfontname ='', orgfontsize =" + 0 + ", trafontname ='', trafontsize =" + 0;
                        }
                        else if (ProjectFont.ProjectOrgName != "" && ProjectFont.ProjectTraName == "")
                        {
                            cmd.CommandText = "update prolanguage set orgfontname ='" + ProjectFont.ProjectOrgName + "', orgfontsize =" + float.Parse(ProjectFont.ProjectOrgSize) + ", trafontname ='', trafontsize =" + 0;
                        }
                        else if (ProjectFont.ProjectOrgName == "" && ProjectFont.ProjectTraName != "")
                        {
                            cmd.CommandText = "update prolanguage set orgfontname ='', orgfontsize =" + 0 + ", trafontname ='" + ProjectFont.ProjectTraName + "', trafontsize =" + float.Parse(ProjectFont.ProjectTraSize);
                        }
                        else
                        {
                            cmd.CommandText = "update prolanguage set orgfontname ='" + ProjectFont.ProjectOrgName + "', orgfontsize =" + float.Parse(ProjectFont.ProjectOrgSize) + ", trafontname ='" + ProjectFont.ProjectTraName + "', trafontsize =" + float.Parse(ProjectFont.ProjectTraSize);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox2.Text == "原文")
            {
                toolStripTextBox2.Font = textBox3.Font;
            }
            else
            {
                toolStripTextBox2.Font = textBox1.Font;
            }
        }

        private void 提取字典FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            DictionaryExtract DE = new DictionaryExtract();
            if (MyDpi > 96F)
            {
                DE.Font = MyNewFont;
                DE.dataGridView1.RowTemplate.Height = (int)(DE.dataGridView1.RowTemplate.Height * MyFontScale);
            }
            DE.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 编辑字典EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            DictionaryEdit DE = new DictionaryEdit();
            if (MyDpi > 96F)
            {
                DE.Font = MyNewFont;
                DE.dataGridView1.RowTemplate.Height = (int)(DE.dataGridView1.RowTemplate.Height * MyFontScale);
                if (MyDpi >= 144F && MyDpi < 192F)
                {
                    DE.toolStripButton1.Image = Athena_A.Properties.Resources.Default24;
                    DE.toolStripButton2.Image = Athena_A.Properties.Resources.Filter24;
                    DE.toolStripButton3.Image = Athena_A.Properties.Resources.Save24;
                    DE.toolStripButton4.Image = Athena_A.Properties.Resources.Start24;
                    DE.toolStripButton5.Image = Athena_A.Properties.Resources.End24;
                    DE.toolStripButton6.Image = Athena_A.Properties.Resources.Forward24;
                    DE.toolStripButton7.Image = Athena_A.Properties.Resources.Copy24;
                }
                else if (MyDpi >= 192F)
                {
                    DE.toolStripButton1.Image = Athena_A.Properties.Resources.Default32;
                    DE.toolStripButton2.Image = Athena_A.Properties.Resources.Filter32;
                    DE.toolStripButton3.Image = Athena_A.Properties.Resources.Save32;
                    DE.toolStripButton4.Image = Athena_A.Properties.Resources.Start32;
                    DE.toolStripButton5.Image = Athena_A.Properties.Resources.End32;
                    DE.toolStripButton6.Image = Athena_A.Properties.Resources.Forward32;
                    DE.toolStripButton7.Image = Athena_A.Properties.Resources.Copy32;
                }
            }
            DE.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 合并字典MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MergeDictionary MD = new MergeDictionary();
            if (MyDpi > 96F)
            {
                MD.Font = MyNewFont;
            }
            MD.ShowDialog();
        }

        private void 转换字典CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConvertDictionary CoDi = new ConvertDictionary();
            if (MyDpi > 96F)
            {
                CoDi.Font = MyNewFont;
            }
            CoDi.ShowDialog();
        }

        private void 译文交换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Swaps Ss = new Swaps();
            if (MyDpi > 96F)
            {
                Ss.Font = MyNewFont;
            }
            Ss.ShowDialog();
        }

        private void 文本保留AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            TextSave TS = new TextSave();
            if (MyDpi > 96F)
            {
                TS.comboBox1.Font = MyNewFont;
                TS.comboBox2.Font = MyNewFont;
                TS.comboBox3.Font = MyNewFont;
                TS.Font = MyNewFont;
            }
            TS.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 地址搜索EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveTool MT = new MoveTool();
            if (MyDpi > 96F)
            {
                MT.Font = MyNewFont;
            }
            MT.ShowDialog();
        }

        private void 提取字符TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            CharTool CT = new CharTool();
            if (MyDpi > 96F)
            {
                CT.Font = MyNewFont;
            }
            CT.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void 文本转码PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            TextCodePage TCP = new TextCodePage();
            if (MyDpi > 96F)
            {
                TCP.comboBox1.Font = MyNewFont;
                TCP.Font = MyNewFont;
            }
            TCP.ShowDialog();
            this.Show();
            this.Activate();
        }

        private void toolStripButton23_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            int x = orgl - tral;
            if (x > 0)
            {
                string s = textBox1.Text;
                if ((bool)dataTable1.Rows[dataGridView1.CurrentRow.Index][15])
                {
                    for (int i = 0; i < x; i = i + 2)
                    {
                        s = s + " ";
                    }
                }
                else
                {
                    for (int i = 0; i < x; i++)
                    {
                        s = s + " ";
                    }
                }
                textBox1.Text = s;
                textBox1.SelectAll();
            }
        }

        private void toolStripButton24_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.Text = textBox1.Text.TrimEnd();
            textBox1.SelectAll();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show(ProjectFileName + "\r\n不存在，无法进行此操作。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(FilePath) == false)
            {
                MessageBox.Show("没有找到创建这个工程时用到的文件，此功不可用。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    int i1 = dataGridView1.CurrentRow.Index;
                    if ((int)dataTable1.Rows[i1][4] > 0)
                    {
                        MessageBox.Show("请取消翻译内容再进行此操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if ((bool)dataTable1.Rows[i1][13])
                        {
                            MessageBox.Show("这是具有长度标识的字符串，无法进行此操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string s1 = dataTable1.Rows[i1][0].ToString();
                            if (s1 != "00000000")
                            {
                                bool MarkerBL = false;
                                if (Marker == s1)
                                {
                                    MarkerBL = true;
                                }
                                orgl = (int)dataTable1.Rows[i1][3];
                                bool bl = true;
                                if (i1 == 0)
                                {
                                    int i2 = (int)CommonCode.HexToLong(s1);
                                    if ((bool)dataTable1.Rows[i1][15])
                                    {
                                        i2 = i2 - 2;
                                        orgl = orgl + 2;
                                    }
                                    else
                                    {
                                        i2 = i2 - 1;
                                        orgl = orgl + 1;
                                    }
                                    string s2 = i2.ToString("X8");
                                    string s3 = "";
                                    byte[] bt = new byte[orgl];
                                    using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                                    {
                                        using (BinaryReader br = new BinaryReader(fs))
                                        {
                                            fs.Seek(i2, SeekOrigin.Begin);
                                            for (int i = 0; i < orgl; i++)
                                            {
                                                bt[i] = br.ReadByte();
                                            }
                                        }
                                    }
                                    if ((bool)dataTable1.Rows[i1][15])
                                    {
                                        if (bt[0] == 0 && bt[1] == 0)
                                        {
                                            bl = false;
                                        }
                                    }
                                    else
                                    {
                                        if (bt[0] == 0)
                                        {
                                            bl = false;
                                        }
                                    }
                                    if (bl == true)
                                    {
                                        if ((bool)dataTable1.Rows[i1][14])
                                        {
                                            s3 = Encoding.UTF8.GetString(bt);
                                        }
                                        else if ((bool)dataTable1.Rows[i1][15])
                                        {
                                            s3 = Encoding.Unicode.GetString(bt);
                                        }
                                        else
                                        {
                                            s3 = Encoding.GetEncoding(ProOrgCode).GetString(bt);
                                        }
                                        s3 = s3.Replace("'", "''");
                                        using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                                        {
                                            MyAccess.Open();
                                            using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                            {
                                                cmd.Transaction = MyAccess.BeginTransaction();
                                                cmd.CommandText = "update athenaa set address = '" + s2 + "', org = '" + s3 + "', orglong = " + orgl + ",free = " + orgl + " where address = '" + s1 + "'";
                                                cmd.ExecuteNonQuery();
                                                if (MarkerBL)
                                                {
                                                    Marker = s2;
                                                    cmd.CommandText = "update fileinfo set detail = '" + Marker + "' where infoname = '标记'";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                cmd.Transaction.Commit();
                                            }
                                        }
                                        s3 = s3.Replace("''", "'");
                                        dataTable1.Rows[i1][0] = s2;
                                        dataTable1.Rows[i1][1] = s3;
                                        dataTable1.Rows[i1][3] = orgl;
                                        dataTable1.Rows[i1][7] = orgl;
                                        RowEnterDis(i1);
                                    }
                                }
                                else
                                {
                                    int i2 = (int)CommonCode.HexToLong(s1);
                                    int i3 = 0;
                                    if ((bool)dataTable1.Rows[i1][15])
                                    {
                                        i2 = i2 - 2;
                                        orgl = orgl + 2;
                                        i3 = (int)CommonCode.HexToLong(dataTable1.Rows[i1 - 1][0].ToString()) + (int)dataTable1.Rows[i1 - 1][3] + 2;
                                    }
                                    else
                                    {
                                        i2 = i2 - 1;
                                        orgl = orgl + 1;
                                        i3 = (int)CommonCode.HexToLong(dataTable1.Rows[i1 - 1][0].ToString()) + (int)dataTable1.Rows[i1 - 1][3] + 1;
                                    }
                                    if (i3 <= i2)
                                    {
                                        string s2 = i2.ToString("X8");
                                        string s3 = "";
                                        byte[] bt = new byte[orgl];
                                        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                                        {
                                            using (BinaryReader br = new BinaryReader(fs))
                                            {
                                                fs.Seek(i2, SeekOrigin.Begin);
                                                for (int i = 0; i < orgl; i++)
                                                {
                                                    bt[i] = br.ReadByte();
                                                }
                                            }
                                        }
                                        if ((bool)dataTable1.Rows[i1][15])
                                        {
                                            if (bt[0] == 0 && bt[1] == 0)
                                            {
                                                bl = false;
                                            }
                                        }
                                        else
                                        {
                                            if (bt[0] == 0)
                                            {
                                                bl = false;
                                            }
                                        }
                                        if (bl == true)
                                        {
                                            if ((bool)dataTable1.Rows[i1][14])
                                            {
                                                s3 = Encoding.UTF8.GetString(bt);
                                            }
                                            else if ((bool)dataTable1.Rows[i1][15])
                                            {
                                                s3 = Encoding.Unicode.GetString(bt);
                                            }
                                            else
                                            {
                                                s3 = Encoding.GetEncoding(ProOrgCode).GetString(bt);
                                            }
                                            s3 = s3.Replace("'", "''");
                                            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                                            {
                                                MyAccess.Open();
                                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                                {
                                                    cmd.Transaction = MyAccess.BeginTransaction();
                                                    cmd.CommandText = "update athenaa set address = '" + s2 + "', org = '" + s3 + "', orglong = " + orgl + ",free = " + orgl + " where address = '" + s1 + "'";
                                                    cmd.ExecuteNonQuery();
                                                    if (MarkerBL)
                                                    {
                                                        Marker = s2;
                                                        cmd.CommandText = "update fileinfo set detail = '" + Marker + "' where infoname = '标记'";
                                                        cmd.ExecuteNonQuery();
                                                    }
                                                    cmd.Transaction.Commit();
                                                }
                                            }
                                            s3 = s3.Replace("''", "'");
                                            dataTable1.Rows[i1][0] = s2;
                                            dataTable1.Rows[i1][1] = s3;
                                            dataTable1.Rows[i1][3] = orgl;
                                            dataTable1.Rows[i1][7] = orgl;
                                            RowEnterDis(i1);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show(ProjectFileName + "\r\n不存在，无法进行此操作。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(FilePath) == false)
            {
                MessageBox.Show("没有找到创建这个工程时用到的文件，此功能不可用。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
                {
                    int i1 = dataGridView1.CurrentRow.Index;
                    if ((int)dataTable1.Rows[i1][4] > 0)
                    {
                        MessageBox.Show("请取消翻译内容再进行此操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if ((bool)dataTable1.Rows[i1][13])
                        {
                            MessageBox.Show("这是具有长度标识的字符串，无法进行此操作。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string s1 = dataTable1.Rows[i1][0].ToString();
                            bool MarkerBL = false;
                            if (Marker == s1)
                            {
                                MarkerBL = true;
                            }
                            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                            {
                                MyAccess.Open();
                                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                                {
                                    orgl = (int)dataTable1.Rows[i1][3];
                                    if (orgl == 1)
                                    {
                                        cmd.Transaction = MyAccess.BeginTransaction();
                                        cmd.CommandText = "delete from athenaa where address = '" + s1 + "'";
                                        cmd.ExecuteNonQuery();
                                        if (MarkerBL)
                                        {
                                            Marker = "";
                                            cmd.CommandText = "update fileinfo set detail = '' where infoname = '标记'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        cmd.Transaction.Commit();
                                        dataTable1.Rows.RemoveAt(i1);
                                        if (dataTable1.Rows.Count > 0)
                                        {
                                            RowEnterDis(dataGridView1.CurrentRow.Index);
                                        }
                                    }
                                    else if (orgl == 2 && (bool)dataTable1.Rows[i1][15])
                                    {
                                        cmd.Transaction = MyAccess.BeginTransaction();
                                        cmd.CommandText = "delete from athenaa where address = '" + s1 + "'";
                                        cmd.ExecuteNonQuery();
                                        if (MarkerBL)
                                        {
                                            Marker = "";
                                            cmd.CommandText = "update fileinfo set detail = '' where infoname = '标记'";
                                            cmd.ExecuteNonQuery();
                                        }
                                        cmd.Transaction.Commit();
                                        dataTable1.Rows.RemoveAt(i1);
                                        if (dataTable1.Rows.Count > 0)
                                        {
                                            RowEnterDis(dataGridView1.CurrentRow.Index);
                                        }
                                    }
                                    else
                                    {
                                        int i2 = (int)CommonCode.HexToLong(s1);
                                        if ((bool)dataTable1.Rows[i1][15])
                                        {
                                            i2 = i2 + 2;
                                            orgl = orgl - 2;
                                        }
                                        else
                                        {
                                            i2 = i2 + 1;
                                            orgl = orgl - 1;
                                        }
                                        string s2 = i2.ToString("X8");
                                        string s3 = "";
                                        using (FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                                        {
                                            using (BinaryReader br = new BinaryReader(fs))
                                            {
                                                fs.Seek(i2, SeekOrigin.Begin);
                                                byte[] bt = new byte[orgl];
                                                for (int i = 0; i < orgl; i++)
                                                {
                                                    bt[i] = br.ReadByte();
                                                }
                                                if ((bool)dataTable1.Rows[i1][14])
                                                {
                                                    s3 = Encoding.UTF8.GetString(bt);
                                                }
                                                else if ((bool)dataTable1.Rows[i1][15])
                                                {
                                                    s3 = Encoding.Unicode.GetString(bt);
                                                }
                                                else
                                                {
                                                    s3 = Encoding.GetEncoding(ProOrgCode).GetString(bt);
                                                }
                                                s3 = s3.Replace("'", "''");
                                                cmd.Transaction = MyAccess.BeginTransaction();
                                                cmd.CommandText = "update athenaa set address = '" + s2 + "', org = '" + s3 + "', orglong = " + orgl + ",free = " + orgl + " where address = '" + s1 + "'";
                                                cmd.ExecuteNonQuery();
                                                if (MarkerBL)
                                                {
                                                    Marker = s2;
                                                    cmd.CommandText = "update fileinfo set detail = '" + Marker + "' where infoname = '标记'";
                                                    cmd.ExecuteNonQuery();
                                                }
                                                cmd.Transaction.Commit();
                                                s3 = s3.Replace("''", "'");
                                                dataTable1.Rows[i1][0] = s2;
                                                dataTable1.Rows[i1][1] = s3;
                                                dataTable1.Rows[i1][3] = orgl;
                                                dataTable1.Rows[i1][7] = orgl;
                                                RowEnterDis(i1);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void 下一个翻译NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                int i2 = dataGridView1.CurrentRow.Index + 1;
                if (i2 < i1)
                {
                    for (int i = i2; i < i1; i++)
                    {
                        if ((int)dataTable1.Rows[i][4] > 0)
                        {
                            if (i > 9)
                            {
                                dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                            }
                            dataGridView1.CurrentCell = dataGridView1[0, i];
                            dataGridView1.CurrentCell.Selected = true;
                            RowEnterDis(i);
                            break;
                        }
                    }
                }
            }
        }

        private string ExportProjectToTextString = "";

        private void 导出EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog SFD = new SaveFileDialog())
            {
                SFD.Filter = "文本文件(制表符分隔)|*.txt";
                SFD.FileName = Path.GetFileNameWithoutExtension(ProjectFileName);
                if (SFD.ShowDialog() == DialogResult.OK)
                {
                    ExportProjectToTextString = SFD.FileName;
                    dataTable1.Clear();
                    DisabledPanelContrl();
                    dataGridView1.DataSource = null;
                    using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                    {
                        MyAccess.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                        {
                            using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                            {
                                cmd.CommandText = "select count(address) from athenaa";
                                object addcount = cmd.ExecuteScalar();
                                int myaddcount = int.Parse(addcount.ToString());
                                if (myaddcount > 40000)
                                {
                                    SPB = true;
                                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                    toolStripProgressBar1.Visible = true;
                                    ProgressBarTimer.Enabled = true;
                                }
                                BackgroundWorker ExportProjectToTextBackgroundWorker = new BackgroundWorker();
                                ExportProjectToTextBackgroundWorker.DoWork += ExportProjectToTextBackgroundWorker_DoWork;
                                ExportProjectToTextBackgroundWorker.RunWorkerCompleted += ExportProjectToTextBackgroundWorker_RunWorkerCompleted;
                                ExportProjectToTextBackgroundWorker.RunWorkerAsync();
                            }
                        }
                    }
                }
            }
        }

        private void ExportProjectToTextBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa ORDER BY address ASC";//ASC 为升序 DESC 为降序
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
        }

        private void ExportProjectToTextBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            SPB = false;
            using (FileStream fs = new FileStream(ExportProjectToTextString, FileMode.Create))
            {
                Encoding ed = Encoding.UTF8;
                using (StreamWriter sw = new StreamWriter(fs, ed))
                {
                    string s = "";
                    string s1 = "";
                    string s2 = "";
                    for (int i = 0; i < dataTable1.Rows.Count; i++)
                    {
                        s = "'" + dataTable1.Rows[i][0].ToString() + "\t";
                        s1 = dataTable1.Rows[i][1].ToString().Replace("\t", "[\\t]").Replace("\r", "[\\r]").Replace("\n", "[\\n]");
                        s = s + s1;
                        s2 = dataTable1.Rows[i][2].ToString();
                        if (s2 == "")
                        {
                            s = s + "\t" + s1;
                        }
                        else
                        {
                            s2 = s2.Replace("\t", "[\\t]").Replace("\r", "[\\r]").Replace("\n", "[\\n]");
                            s = s + "\t" + s2;
                        }
                        sw.WriteLine(s);
                    }

                }
            }
            MessageBox.Show("导出成功。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (PEbool)
            {
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
            }
            EnablePanelContrl();
            this.Activate();
        }

        private void 导入IToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "文本文件(制表符分隔)|*.txt";
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                DisabledPanelContrl();
                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                toolStripProgressBar1.Visible = true;
                SPB = true;//进度栏
                ProgressBarTimer.Enabled = true;
                ImportBackgroundWorker.RunWorkerAsync(OFD.FileName);
            }
        }

        private BackgroundWorker ImportBackgroundWorker = new BackgroundWorker();

        private void ImportBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            string s1 = "";
            string s2 = "";
            string s3 = "";
            int orglong3 = 0;
            int tralong4 = 0;
            int free7 = 0;
            string[] c = { "\t" };
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        Encoding ed = Encoding.UTF8;
                        using (FileStream fs = new FileStream(e.Argument.ToString(), FileMode.Open, FileAccess.Read))
                        {
                            using (StreamReader sr = new StreamReader(fs, ed))
                            {
                                if (ProEncoding == "Unicode")
                                {
                                    cmd.Transaction = MyAccess.BeginTransaction();
                                    while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                                    {
                                        string[] splits1 = s1.Split(c, StringSplitOptions.None);
                                        if (splits1.Length == 3)
                                        {
                                            s2 = splits1[1];
                                            s3 = splits1[2];
                                            if (s2 != s3)
                                            {
                                                s1 = splits1[0].Replace("'", "");
                                                s2 = s2.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                orglong3 = Encoding.Unicode.GetByteCount(s2);
                                                s2 = s2.Replace("'", "''");
                                                s3 = s3.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                tralong4 = Encoding.Unicode.GetByteCount(s3);
                                                s3 = s3.Replace("'", "''");
                                                free7 = orglong3 - tralong4;
                                                cmd.CommandText = "update athenaa set tra = '" + s3 + "', tralong = " + tralong4 + ", free = " + free7 + " where address = '" + s1 + "' and org = '" + s2 + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    cmd.Transaction.Commit();
                                }
                                else if (ProEncoding == "UTF8")
                                {
                                    cmd.Transaction = MyAccess.BeginTransaction();
                                    while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                                    {
                                        string[] splits1 = s1.Split(c, StringSplitOptions.None);
                                        if (splits1.Length == 3)
                                        {
                                            s2 = splits1[1];
                                            s3 = splits1[2];
                                            if (s2 != s3)
                                            {
                                                s1 = splits1[0].Replace("'", "");
                                                s2 = s2.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                orglong3 = Encoding.UTF8.GetByteCount(s2);
                                                s2 = s2.Replace("'", "''");
                                                s3 = s3.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                tralong4 = Encoding.UTF8.GetByteCount(s3);
                                                s3 = s3.Replace("'", "''");
                                                free7 = orglong3 - tralong4;
                                                cmd.CommandText = "update athenaa set tra = '" + s3 + "', tralong = " + tralong4 + ", free = " + free7 + " where address = '" + s1 + "' and org = '" + s2 + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    cmd.Transaction.Commit();
                                }
                                else if (ProEncoding == "ANSI")
                                {
                                    cmd.Transaction = MyAccess.BeginTransaction();
                                    while ((s1 = sr.ReadLine()) != null)//判定是否是最后一行
                                    {
                                        string[] splits1 = s1.Split(c, StringSplitOptions.None);
                                        if (splits1.Length == 3)
                                        {
                                            s2 = splits1[1];
                                            s3 = splits1[2];
                                            if (s2 != s3)
                                            {
                                                s1 = splits1[0].Replace("'", "");
                                                s2 = s2.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                orglong3 = Encoding.GetEncoding(ProTraCode).GetByteCount(s2);
                                                s2 = s2.Replace("'", "''");
                                                s3 = s3.Replace("[\\t]", "\t").Replace("[\\r]", "\r").Replace("[\\n]", "\n");
                                                tralong4 = Encoding.GetEncoding(ProTraCode).GetByteCount(s3);
                                                s3 = s3.Replace("'", "''");
                                                free7 = orglong3 - tralong4;
                                                cmd.CommandText = "update athenaa set tra = '" + s3 + "', tralong = " + tralong4 + ", free = " + free7 + ",zonebl = 0, superlong = 0, movebackward = 0, moveforward = 0, outaddfree = 0, outadd = '', addrlong = 0, addr = '' where address = '" + s1 + "' and org = '" + s2 + "'";
                                                cmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    cmd.Transaction.Commit();
                                }
                                cmd.Transaction = MyAccess.BeginTransaction();
                                cmd.CommandText = "delete from matrixzone";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "delete from matrix";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "delete from calladd";
                                cmd.ExecuteNonQuery();
                                cmd.CommandText = "select * from athenaa ORDER BY address ASC";
                                dataTable1.Clear();
                                ad.Fill(dataTable1);
                                cmd.Transaction.Commit();
                            }
                        }
                    }
                }
            }
        }

        private void ImportBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            SPB = false;
            MessageBox.Show("导入完成。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            EnablePanelContrl();
            this.Activate();
        }

        private void mainform_SizeChanged(object sender, EventArgs e)
        {
            if (SPB == true)
            {
                toolStripProgressBar1.Visible = false;
                toolStripProgressBar1.Width = 0;
                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                toolStripProgressBar1.Visible = true;
            }
            if (SD.Visible)
            {
                Point P = new Point(this.Location.X + this.Width - SD.Width - 5, this.Location.Y + this.Height - SD.Height - 5);
                SD.Location = P;
            }
        }

        private void ProgressBarTimer_Tick(object sender, EventArgs e)
        {
            if (toolStripProgressBar1.Value == 20)
            {
                toolStripProgressBar1.Value = 0;
            }
            toolStripProgressBar1.Value++;
        }

        private void 整理字典DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Compressdata C1 = new Compressdata();
            if (MyDpi > 96F)
            {
                C1.Font = MyNewFont;
            }
            C1.ShowDialog();
        }

        private SearchDictionary SD = new SearchDictionary();

        private void toolStripTextBox6_TextChanged(object sender, EventArgs e)
        {
            if (dataTable6.Rows.Count > 0)
            {
                if (toolStripTextBox6.Text.Length > 1)
                {
                    timer1.Enabled = false;
                    timer1.Enabled = true;
                }
                else
                {
                    timer1.Enabled = false;
                    SD.SearchDictionaryDataTable1.Rows.Clear();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            timer1.Enabled = false;
            SD.SearchDictionaryDataTable1.Rows.Clear();
            SD.dataGridView1.DataSource = null;
            SD.dataGridView2.DataSource = null;
            EnumerableRowCollection<DataRow> dtResult;
            if (MatchCase)
            {
                string strTmp = toolStripTextBox6.Text;
                dtResult = from dt6 in dataTable6.AsEnumerable() where dt6.Field<string>(0).Contains(strTmp) select dt6;
            }
            else
            {
                string strTmp = toolStripTextBox6.Text.ToLower();
                dtResult = from dt6 in dataTable6.AsEnumerable() where dt6.Field<string>(0).ToLower().Contains(strTmp) select dt6;
            }
            int i1 = dtResult.Count() - 1;
            if (i1 > -1)
            {
                var TmpDataTable = dtResult.CopyToDataTable();
                ConcurrentDictionary<string, int> CD = new ConcurrentDictionary<string, int>();
                for (; i1 >= 0; i1--)
                {
                    if (!CD.TryAdd((TmpDataTable.Rows[i1][0].ToString() + TmpDataTable.Rows[i1][1].ToString()), 0))
                    {
                        TmpDataTable.Rows.RemoveAt(i1);
                    }
                }
                i1 = TmpDataTable.Rows.Count;
                for (int i = 0; i < i1; i++)
                {
                    SD.SearchDictionaryDataTable1.Rows.Add(TmpDataTable.Rows[i].ItemArray);
                }
                SD.dataGridView1.DataSource = SD.dataSet1;
                SD.dataGridView2.DataSource = SD.dataSet1;
                SD.dataGridView1.DataMember = SD.SearchDictionaryDataTable1.TableName;
                SD.dataGridView2.DataMember = SD.SearchDictionaryDataTable1.TableName;
                if (SD.Visible == false)
                {
                    Point P = new Point(this.Location.X + this.Width - SD.Width - 2, this.Location.Y + this.Height - SD.Height - 2);
                    SD.Location = P;
                    SD.Show();
                }
            }
        }

        private void mainform_LocationChanged(object sender, EventArgs e)
        {
            if (SD.Visible)
            {
                Point P = new Point(this.Location.X + this.Width - SD.Width - 5, this.Location.Y + this.Height - SD.Height - 5);
                SD.Location = P;
            }
        }

        private void delphi代码页DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelphiCodePage DCP = new DelphiCodePage();
            if (MyDpi > 96F)
            {
                DCP.Font = MyNewFont;
                DCP.dataGridView1.RowTemplate.Height = (int)(DCP.dataGridView1.RowTemplate.Height * MyFontScale);
            }
            DCP.ShowDialog();
        }

        private void SearchFonts()
        {
            int i1 = SysFont.Count;
            int i2 = PriFont.Count;
            if (i1 > 0 || i2 > 0)
            {
                dataTable1.Rows.Clear();
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            if (i1 > 0 && i2 > 0)
                            {
                                for (int x = i1 - 1; x >= 0; x--)
                                {
                                    for (int y = 0; y < i2; y++)
                                    {
                                        if (SysFont[x].ToString() == PriFont[y].ToString())
                                        {
                                            SysFont.RemoveAt(x);
                                            break;
                                        }
                                    }
                                }
                            }
                            cmd.Transaction = MyAccess.BeginTransaction();
                            StringBuilder sb = new StringBuilder();
                            i1 = SysFont.Count;
                            if (i1 > 0)
                            {
                                sb.Append("org='" + SysFont[0].ToString() + "'");
                                for (int i = 1; i < i1; i++)
                                {
                                    sb.Append(" or org='" + SysFont[i].ToString() + "'");
                                }
                                cmd.CommandText = "select * from athenaa where " + sb.ToString();
                                ad.Fill(dataTable1);
                            }
                            if (i2 > 0)
                            {
                                sb.Clear();
                                sb.Append("org='" + PriFont[0].ToString() + "'");
                                for (int i = 1; i < i2; i++)
                                {
                                    sb.Append(" or org='" + PriFont[i].ToString() + "'");
                                }
                                cmd.CommandText = "select * from athenaa where " + sb.ToString();
                                ad.Fill(dataTable1);
                            }
                            cmd.Transaction.Commit();
                            if (PEbool)
                            {
                                i1 = dataTable1.Rows.Count;
                                if (i1 > 0)
                                {
                                    Parallel.For(0, i1, (i) =>
                                    {
                                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                                        {
                                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                                        }
                                        if (dataTable1.Rows[i][5].ToString() != "")
                                        {
                                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void 查找字体FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            SysFont.Clear();
            PriFont.Clear();
            FindFont FF = new FindFont();
            if (MyDpi > 96F)
            {
                FF.Font = MyNewFont;
            }
            FF.ShowDialog();
            SearchFonts();
        }

        private void 再次查找AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchFonts();
        }

        private void 显示所有忽略SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataTable1.Clear();
            dataGridView1.DataSource = null;
            bool bl = true;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select count(address) from athenaa where Ignoretra = 1";
                        int addCount = int.Parse(cmd.ExecuteScalar().ToString());
                        if (int.Parse(cmd.ExecuteScalar().ToString()) < 10000)
                        {
                            cmd.CommandText = "select * from athenaa where Ignoretra = 1 ORDER BY address ASC";
                            ad.Fill(dataTable1);
                            dataGridView1.DataSource = dataSet1;
                            dataGridView1.DataMember = dataTable1.TableName;
                            bl = false;
                        }
                    }
                }
            }
            if (bl)
            {
                DisabledPanelContrl();
                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Value = 5;
                SPB = true;
                ProgressBarTimer.Enabled = true;
                using (BackgroundWorker ShowAllIgnoreTraBackgroundWorker = new BackgroundWorker())
                {
                    ShowAllIgnoreTraBackgroundWorker.DoWork += ShowAllIgnoreTraBackgroundWorker_DoWork;
                    ShowAllIgnoreTraBackgroundWorker.RunWorkerCompleted += ShowAllIgnoreTraBackgroundWorker_RunWorkerCompleted;
                    ShowAllIgnoreTraBackgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                if (dataTable1.Rows.Count == 0)
                {
                    ClearText();
                }
                else
                {
                    RowEnterDis(0);
                }
            }
        }

        private void ShowAllIgnoreTraBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa where Ignoretra = 1 ORDER BY address ASC";
                        ad.Fill(dataTable1);
                    }
                }
            }
        }

        private void ShowAllIgnoreTraBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
            RowEnterDis(0);
        }

        private void 浮点工具LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FloatTool FT = new FloatTool();
            if (MyDpi > 96F)
            {
                FT.Font = MyNewFont;
            }
            FT.ShowDialog();
        }

        private void 数字签名DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DigitalSignature DS = new DigitalSignature();
            if (MyDpi > 96F)
            {
                DS.Font = MyNewFont;
            }
            DS.ShowDialog();
        }

        private void 批量转码MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BatchText BT = new BatchText();
            if (MyDpi > 96F)
            {
                BT.Font = MyNewFont;
            }
            BT.ShowDialog();
        }

        private void 编辑矩阵MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("没有找到工程文件，无法使用编辑矩阵功能。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                StringMatrix SM = new StringMatrix();
                if (MyDpi > 96F)
                {
                    SM.Font = MyNewFont;
                    SM.dataGridView1.RowTemplate.Height = (int)(SM.dataGridView1.RowTemplate.Height * MyFontScale);
                    SM.dataGridView1.Columns[0].Width = (int)Graphics.FromHwnd(IntPtr.Zero).MeasureString("DDDDDDDD", MyNewFont).Width;
                }
                SM.ShowDialog();
                if (MatrixDel == true)
                {
                    OpenProject();
                    MatrixDel = false;
                }
            }
        }

        private void 批量挪移PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                BatchMove BM = new BatchMove();
                if (MyDpi > 96F)
                {
                    BM.comboBox1.Font = MyNewFont;
                    BM.Font = MyNewFont;
                    BM.dataGridView1.RowTemplate.Height = (int)(BM.dataGridView1.RowTemplate.Height * MyFontScale);
                    BM.dataGridView1.Columns[0].Width = (int)Graphics.FromHwnd(IntPtr.Zero).MeasureString("DDDDDDDD", MyNewFont).Width;
                }
                BM.ShowDialog();
                OpenProject();
            }
        }

        private void 估算长度NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            if (File.Exists(ProjectFileName) == false)
            {
                MessageBox.Show("找不到工程文件，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                dataTable1.Clear();
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select * from athenaa where free < 0 ORDER BY address ASC";
                            ad.Fill(dataTable1);
                        }
                    }
                }
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    if (PEbool)
                    {
                        Parallel.For(0, i1, (i) =>
                        {
                            if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                            {
                                dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                            }
                            if (dataTable1.Rows[i][5].ToString() != "")
                            {
                                dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                            }
                        });
                    }
                    RowEnterDis(dataGridView1.CurrentRow.Index);
                    //ProEncoding = ANSI  UTF8  Unicode
                    int iLen = 0;
                    if (ProEncoding == "Unicode")
                    {
                        if (StrLenCategory == "无")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 2;
                            }
                        }
                        else if (StrLenCategory == "标准" || StrLenCategory == "标准2")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 2;
                                if ((bool)dataTable1.Rows[i][13] == true)
                                {
                                    iLen += 4;
                                }
                            }
                        }
                        else if (StrLenCategory == "Delphi")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 2;
                                if ((bool)dataTable1.Rows[i][13] == true)
                                {
                                    iLen += 8;
                                    if ((int)dataTable1.Rows[i][18] > 0)
                                    {
                                        iLen += 4;
                                    }
                                }
                            }
                        }
                        iLen -= 2;
                    }
                    else if (ProEncoding == "ANSI")
                    {
                        if (StrLenCategory == "无")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 1;
                            }
                        }
                        else if (StrLenCategory == "标准" || StrLenCategory == "标准2")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 1;
                                if ((bool)dataTable1.Rows[i][13] == true)
                                {
                                    iLen += 4;
                                }
                            }
                        }
                        else if (StrLenCategory == "Delphi")
                        {
                            for (int i = 0; i < i1; i++)
                            {
                                iLen += (int)dataTable1.Rows[i][4] + 1;
                                if ((bool)dataTable1.Rows[i][13] == true)
                                {
                                    iLen += 8;
                                    if ((int)dataTable1.Rows[i][18] > 0)
                                    {
                                        iLen += 4;
                                    }
                                }
                            }
                        }
                        iLen -= 1;
                    }
                    else if (ProEncoding == "UTF8")
                    {
                        for (int i = 0; i < i1; i++)
                        {
                            iLen += (int)dataTable1.Rows[i][4] + 1;
                        }
                        iLen -= 1;
                    }
                    int x = 0;
                    iLen = Math.DivRem(iLen, 16, out x);
                    iLen = (iLen + 1) * 16;
                    Clipboard.Clear();
                    Clipboard.SetText(iLen.ToString());
                    MessageBox.Show("请把实际设置的长度比估算的长度要大一些，方便以后可能会向矩阵内添加新的内容。\r\n估算的长度 " + iLen.ToString() + " 已复制到剪贴板。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    ClearText();
                    MessageBox.Show("没有搜索到超长的字符串，无法进行估算。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //
        }

        private void 编辑节区AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditSection ES = new EditSection();
            if (MyDpi > 96F)
            {
                ES.Font = MyNewFont;
                ES.dataGridView1.RowTemplate.Height = (int)(ES.dataGridView1.RowTemplate.Height * MyFontScale);
                ES.dataGridView1.Columns[0].Width = (int)Graphics.FromHwnd(IntPtr.Zero).MeasureString("DDDDDDDD", MyNewFont).Width;
            }
            ES.ShowDialog();
        }

        private void 引用ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                if (File.Exists(ProjectFileName) == false)
                {
                    MessageBox.Show("找不到工程文件\r\n“" + ProjectFileName + "”，请检查工程文件是否存在。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (File.Exists(FilePath) == false)
                {
                    MessageBox.Show("需要翻译的文件不存在，无法完成检索，此功能不可用。\r\n可以打开工程属性窗口了解相关信息。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ReferencedBy RB = new ReferencedBy();
                    if (MyDpi > 96F)
                    {
                        RB.Font = MyNewFont;
                    }
                    for (int x = 0; x < 22; x++)
                    {
                        obMS[x] = dataGridView1.CurrentRow.Cells[x].Value;
                    }
                    string ss = obMS[0].ToString();
                    RB.ShowDialog();
                    MoveAddressHexView(ss, (int)obMS[3]);
                }
            }
        }

        private void 清除运行路径ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runtest = "";
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + mainform.ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    cmd.CommandText = "update fileinfo set detail = '' where infoname = '运行'";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected) == 1)
            {
                恢复ToolStripMenuItem.Enabled = true;
                翻译ToolStripMenuItem.Enabled = true;
                if (PEbool == false)
                {
                    if (UnPEType == "6")
                    {
                        if (dataGridView1.CurrentRow.Cells[1].Value.ToString().Contains("msgstr") == false)
                        {
                            恢复ToolStripMenuItem.Enabled = false;
                            翻译ToolStripMenuItem.Enabled = false;
                        }
                    }
                }
                引用ToolStripMenuItem.Enabled = true;
                复制地址ToolStripMenuItem.Enabled = true;
                取消翻译ToolStripMenuItem.Enabled = true;
                添加到字典ToolStripMenuItem.Enabled = true;
                RowEnterDis(dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected));
                this.Activate();
            }
        }

        private void 滚到标记OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Marker != "")
            {
                int i1 = dataTable1.Rows.Count;
                if (i1 > 0)
                {
                    Control.CheckForIllegalCrossThreadCalls = false;
                    Parallel.For(0, i1, (i, ParallelLoopState) =>
                    {
                        if (dataTable1.Rows[i][0].ToString() == Marker)
                        {
                            ParallelLoopState.Break();
                            if (i > 9)
                            {
                                dataGridView1.FirstDisplayedScrollingRowIndex = i - 10;
                            }
                            dataGridView1[0, i].Selected = true;
                        }
                    });
                }
            }
        }

        private bool SearchHistoryBL = true;

        private void toolStripComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SearchHistoryBL)
            {
                SearchHistoryBL = false;
                string s1 = toolStripComboBox4.Text;
                if (s1 == "清除历史")
                {
                    int i1 = toolStripComboBox4.Items.Count - 1;
                    for (int i = i1; i > 3; i--)
                    {
                        toolStripComboBox4.Items.RemoveAt(i);
                    }
                }
                else
                {
                    toolStripTextBox2.Text = s1;
                }
                SearchHistoryBL = true;
            }
        }

        private ConcurrentDictionary<string, int> IgnoreCD;
        private bool IgnoreTraBL = false;

        private void 新建忽略ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //   0       1     2       3         4        5        6        7       8          9          10            11
            //address   org   tra   orglong   tralong   addr   addrlong   free   outadd   outaddfree  moveforward  movebackward
            //   12         13      14      15      16        17            18           19          20          21
            //superlong   delphi   utf8   ucode   zonebl   codepage   delphicodepage  Ignoretra  Undefined1  Undefined2
            //            实际是                           没有使用        0 无
            //           长度标识                                         1 不变
            int intFirstRow = dataGridView1.Rows.GetFirstRow(DataGridViewElementStates.Selected);
            if (intFirstRow > -1)
            {
                int intLastRow = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.Selected) + 1;
                IgnoreCD = new ConcurrentDictionary<string, int>();
                Parallel.For(intFirstRow, intLastRow, (i) =>
                {
                    if (((int)dataGridView1.Rows.GetRowState(i) & 32) == 32)
                    {
                        if ((int)dataTable1.Rows[i][4] > 0 && (int)dataTable1.Rows[i][10] == 0 && (int)dataTable1.Rows[i][11] == 0 && (bool)dataTable1.Rows[i][12] == false && dataTable1.Rows[i][5].ToString() == "" && dataTable1.Rows[i][8].ToString() == "" && (bool)dataTable1.Rows[i][16] == false)
                        {
                            IgnoreCD.TryAdd(dataTable1.Rows[i][0].ToString(), i);
                        }
                    }
                });
                if (IgnoreCD.Count == 0)
                {
                    MessageBox.Show("没有检测到符合忽略条件的内容。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    using (BackgroundWorker NewIgnoreTraBackgroundWorker = new BackgroundWorker())
                    {
                        NewIgnoreTraBackgroundWorker.DoWork += NewIgnoreTraBackgroundWorker_DoWork;
                        NewIgnoreTraBackgroundWorker.RunWorkerCompleted += NewIgnoreTraBackgroundWorker_RunWorkerCompleted;
                        NewIgnoreTraBackgroundWorker.RunWorkerAsync();
                    }
                }
            }
        }

        private void NewIgnoreTraBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int CDCount = IgnoreCD.Count;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(MyAccess))
                {
                    SCM.CommandText = "select count(address) from IgnoreTra";
                    int iCount = int.Parse(SCM.ExecuteScalar().ToString());
                    IgnoreTraBL = false;
                    if (iCount > 0)
                    {
                        this.Invoke(new Action(delegate
                        {
                            if (MessageBox.Show("新建忽略将删除已存在的忽略；\r\n\r\n确定要删除旧的忽略并建立新的忽略吗？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                            {
                                if (CDCount > 10000)
                                {
                                    DisabledPanelContrl();
                                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                    toolStripProgressBar1.Visible = true;
                                    toolStripProgressBar1.Value = 5;
                                    SPB = true;
                                    ProgressBarTimer.Enabled = true;
                                }
                                IgnoreTraBL = true;
                            }
                        }));
                        if (IgnoreTraBL)
                        {
                            Parallel.Invoke(() =>
                            {
                                int dt1Count = dataTable1.Rows.Count;
                                for (int m = 0; m < dt1Count; m++)
                                {
                                    dataTable1.Rows[m][19] = false;
                                }
                            },
                            () =>
                            {
                                SCM.CommandText = "update athenaa set Ignoretra = 0";
                                SCM.ExecuteNonQuery();
                            });
                            SCM.CommandText = "delete from IgnoreTra";
                            SCM.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        IgnoreTraBL = true;
                    }
                    if (IgnoreTraBL)
                    {
                        if (CDCount > 10000 && SPB == false)
                        {
                            this.Invoke(new Action(delegate
                            {
                                DisabledPanelContrl();
                                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                toolStripProgressBar1.Visible = true;
                                toolStripProgressBar1.Value = 5;
                                SPB = true;
                                ProgressBarTimer.Enabled = true;
                            }));
                        }
                        string[] TmpStr = IgnoreCD.Keys.ToArray<string>();
                        Parallel.Invoke(() =>
                        {
                            for (int i = 0; i < CDCount; i++)
                            {
                                if (IgnoreCD.TryGetValue(TmpStr[i], out int iTmp))
                                {
                                    dataTable1.Rows[iTmp][19] = true;
                                }
                            }
                        },
                        () =>
                        {
                            SCM.Transaction = MyAccess.BeginTransaction();
                            for (int i = 0; i < CDCount; i++)
                            {
                                SCM.CommandText = "Insert Into IgnoreTra(address,ignorebl)Values('" + TmpStr[i] + "',1)";
                                SCM.ExecuteNonQuery();
                            }
                            SCM.Transaction.Commit();
                        });
                        SCM.Transaction = MyAccess.BeginTransaction();
                        SCM.CommandText = "update athenaa set Ignoretra = 1 where athenaa.address in (select IgnoreTra.address from IgnoreTra)";
                        SCM.ExecuteNonQuery();
                        SCM.Transaction.Commit();
                    }
                }
            }
        }

        private void NewIgnoreTraBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
            if (IgnoreTraBL)
            {
                MessageBox.Show("总共忽略了 " + IgnoreCD.Count.ToString() + " 条翻译。", "确定");
            }
        }

        private void 删除忽略EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool bl = false;
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(MyAccess))
                {
                    SCM.CommandText = "select count(address) from IgnoreTra";
                    int iCount = int.Parse(SCM.ExecuteScalar().ToString());
                    if (iCount > 20000)
                    {
                        bl = true;
                    }
                    else if (iCount > 0)
                    {
                        Parallel.Invoke(() =>
                        {
                            int dt1Count = dataTable1.Rows.Count;
                            for (int m = 0; m < dt1Count; m++)
                            {
                                dataTable1.Rows[m][19] = false;
                            }
                        },
                        () =>
                        {
                            SCM.CommandText = "update athenaa set Ignoretra = 0";
                            SCM.ExecuteNonQuery();
                        });
                        SCM.CommandText = "delete from IgnoreTra";
                        SCM.ExecuteNonQuery();
                    }
                }
            }
            if (bl)
            {
                DisabledPanelContrl();
                toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                toolStripProgressBar1.Visible = true;
                toolStripProgressBar1.Value = 5;
                SPB = true;
                ProgressBarTimer.Enabled = true;
                using (BackgroundWorker deleteAllIgnoreTra = new BackgroundWorker())
                {
                    deleteAllIgnoreTra.DoWork += DeleteAllIgnoreTra_DoWork;
                    deleteAllIgnoreTra.RunWorkerCompleted += DeleteAllIgnoreTra_RunWorkerCompleted;
                    deleteAllIgnoreTra.RunWorkerAsync();
                }
            }
        }

        private void DeleteAllIgnoreTra_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(MyAccess))
                {
                    Parallel.Invoke(() =>
                    {
                        int dt1Count = dataTable1.Rows.Count;
                        for (int m = 0; m < dt1Count; m++)
                        {
                            dataTable1.Rows[m][19] = false;
                        }
                    },
                    () =>
                    {
                        SCM.CommandText = "update athenaa set Ignoretra = 0";
                        SCM.ExecuteNonQuery();
                    });
                    SCM.CommandText = "delete from IgnoreTra";
                    SCM.ExecuteNonQuery();
                }
            }
        }

        private void DeleteAllIgnoreTra_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
        }

        private void 固化忽略ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(MyAccess))
                {
                    SCM.CommandText = "delete from IgnoreTra where ignorebl=0";
                    SCM.ExecuteNonQuery();
                }
            }
        }

        private void 反转忽略ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BackgroundWorker InvertIgnoreTraBackgroundWorker = new BackgroundWorker())
            {
                InvertIgnoreTraBackgroundWorker.DoWork += InvertIgnoreTraBackgroundWorker_DoWork;
                InvertIgnoreTraBackgroundWorker.RunWorkerCompleted += InvertIgnoreTraBackgroundWorker_RunWorkerCompleted;
                InvertIgnoreTraBackgroundWorker.RunWorkerAsync();
            }
        }

        private void InvertIgnoreTraBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand SCM = new SQLiteCommand(MyAccess))
                {
                    SCM.CommandText = "select count(address) from IgnoreTra";
                    int addCount = int.Parse(SCM.ExecuteScalar().ToString());
                    using (SQLiteDataAdapter sqlDA = new SQLiteDataAdapter(SCM))
                    {
                        if (addCount > 1)
                        {
                            if (addCount > 10000)
                            {
                                this.Invoke(new Action(delegate
                                {
                                    DisabledPanelContrl();
                                    toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                                    toolStripProgressBar1.Visible = true;
                                    toolStripProgressBar1.Value = 5;
                                    SPB = true;
                                    ProgressBarTimer.Enabled = true;
                                }));
                            }
                            using (DataTable adddt_Ignore = new DataTable())
                            {
                                using (DataTable adddt_Invert = new DataTable())
                                {
                                    adddt_Invert.Columns.Add(new DataColumn("address", typeof(string)));
                                    SCM.CommandText = "select address from IgnoreTra where ignorebl=0 ORDER BY address ASC";
                                    sqlDA.Fill(adddt_Invert);
                                    int InvertCount = adddt_Invert.Rows.Count;
                                    adddt_Ignore.Columns.Add(new DataColumn("address", typeof(string)));
                                    SCM.CommandText = "select address from IgnoreTra where ignorebl=1 ORDER BY address ASC";
                                    sqlDA.Fill(adddt_Ignore);
                                    int IgnoreCount = adddt_Ignore.Rows.Count;
                                    if (addCount == IgnoreCount)
                                    {
                                        IgnoreCount = Math.DivRem(addCount, 2, out int i1);
                                        SCM.Transaction = MyAccess.BeginTransaction();
                                        SCM.CommandText = "delete from IgnoreTra";
                                        SCM.ExecuteNonQuery();
                                        for (int x = 0; x < IgnoreCount; x++)
                                        {
                                            SCM.CommandText = "Insert Into IgnoreTra(address,ignorebl)Values('" + adddt_Ignore.Rows[x][0].ToString() + "',0)";
                                            SCM.ExecuteNonQuery();
                                        }
                                        SCM.CommandText = "update athenaa set Ignoretra = 0 where athenaa.address in (select IgnoreTra.address from IgnoreTra where ignorebl=0)";
                                        SCM.ExecuteNonQuery();
                                        for (int x = IgnoreCount; x < addCount; x++)
                                        {
                                            SCM.CommandText = "Insert Into IgnoreTra(address,ignorebl)Values('" + adddt_Ignore.Rows[x][0].ToString() + "',1)";
                                            SCM.ExecuteNonQuery();
                                        }
                                        SCM.Transaction.Commit();
                                    }
                                    else
                                    {
                                        SCM.Transaction = MyAccess.BeginTransaction();
                                        SCM.CommandText = "delete from IgnoreTra";
                                        SCM.ExecuteNonQuery();
                                        for (int x = 0; x < IgnoreCount; x++)
                                        {
                                            SCM.CommandText = "Insert Into IgnoreTra(address,ignorebl)Values('" + adddt_Ignore.Rows[x][0].ToString() + "',0)";
                                            SCM.ExecuteNonQuery();
                                        }
                                        SCM.CommandText = "update athenaa set Ignoretra = 0 where athenaa.address in (select IgnoreTra.address from IgnoreTra where ignorebl=0)";
                                        SCM.ExecuteNonQuery();
                                        for (int x = 0; x < InvertCount; x++)
                                        {
                                            SCM.CommandText = "Insert Into IgnoreTra(address,ignorebl)Values('" + adddt_Invert.Rows[x][0].ToString() + "',1)";
                                            SCM.ExecuteNonQuery();
                                        }
                                        SCM.CommandText = "update athenaa set Ignoretra = 1 where athenaa.address in (select IgnoreTra.address from IgnoreTra where ignorebl=1)";
                                        SCM.ExecuteNonQuery();
                                        SCM.Transaction.Commit();
                                    }
                                }
                            }
                            this.Invoke(new Action(delegate
                            {
                                dataTable1.Clear();
                                dataGridView1.DataSource = null;
                            }));
                            SCM.CommandText = "select * from athenaa where Ignoretra = 1 ORDER BY address ASC";
                            sqlDA.Fill(dataTable1);
                        }
                        else if (addCount == 1)
                        {
                            this.Invoke(new Action(delegate
                            {
                                dataTable1.Clear();
                                dataGridView1.DataSource = null;
                            }));
                            SCM.CommandText = "select * from athenaa where Ignoretra = 1";
                            sqlDA.Fill(dataTable1);
                        }
                    }
                }
            }
        }

        private void InvertIgnoreTraBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Visible = false;
            EnablePanelContrl();
            this.Activate();
            SPB = false;
            if (dataTable1.Rows.Count > 0)
            {
                RowEnterDis(0);
            }
        }

        private void 查找多译TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(ProjectFileName))
            {
                int myaddcount = 0;
                using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
                {
                    MyAccess.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                    {
                        using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                        {
                            cmd.CommandText = "select count(address) from athenaa where tralong > 0 and org != tra";
                            object addcount = cmd.ExecuteScalar();
                            myaddcount = int.Parse(addcount.ToString());
                        }
                    }
                }
                if (myaddcount > 0)
                {
                    dataTable1.Clear();
                    dataGridView1.DataSource = null;
                    if (myaddcount > 40000)
                    {
                        DisabledPanelContrl();
                        SPB = true;
                        toolStripProgressBar1.Width = textBox3.Width - toolSPB1Left;
                        toolStripProgressBar1.Visible = true;
                        ProgressBarTimer.Enabled = true;
                    }
                    BackgroundWorker FindMultiTraBackgroundWorker = new BackgroundWorker();
                    FindMultiTraBackgroundWorker.DoWork += FindMultiTraBackgroundWorker_DoWork;
                    FindMultiTraBackgroundWorker.RunWorkerCompleted += FindMultiTraBackgroundWorker_RunWorkerCompleted;
                    FindMultiTraBackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void FindMultiTraBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (SQLiteConnection MyAccess = new SQLiteConnection("Data Source=" + ProjectFileName))
            {
                MyAccess.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(MyAccess))
                {
                    using (SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd))
                    {
                        cmd.CommandText = "select * from athenaa where tralong > 0 and org != tra ORDER BY address ASC";
                        ad.Fill(dataTable1);//dataTable1 工程
                    }
                }
            }
            var dtResult = from dt1 in dataTable1.AsEnumerable()
                           join dt6 in dataTable6.AsEnumerable()
                           on dt1.Field<string>(1).ToLower() equals dt6.Field<string>(0).ToLower()
                           group dt1 by dt1.Field<string>(0) into dtTmp
                           where dtTmp.Count() > 1
                           select new
                           {
                               v1 = dtTmp.Key,
                           };
            if (dtResult.Count() > 0)
            {
                ConcurrentDictionary<string, Int32> cdTmp = new ConcurrentDictionary<string, Int32>();
                Parallel.ForEach(dtResult, x =>
                {
                    cdTmp.TryAdd(x.v1, 0);
                });
                int i1 = dataTable1.Rows.Count - 1;
                for (; i1 >= 0; i1--)
                {
                    if (!cdTmp.Keys.Contains(dataTable1.Rows[i1][0].ToString()))
                    {
                        dataTable1.Rows.RemoveAt(i1);
                    }
                }
            }
            else
            {
                dataTable1.Rows.Clear();
            }
        }

        private void FindMultiTraBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dataGridView1.DataSource = dataSet1;
            dataGridView1.DataMember = dataTable1.TableName;
            ProgressBarTimer.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Value = 0;
            EnablePanelContrl();
            SPB = false;
            int i1 = dataTable1.Rows.Count;
            if (i1 > 0)
            {
                if (PEbool)
                {
                    Parallel.For(0, i1, (i) =>
                    {
                        if (dataTable1.Rows[i][8].ToString() != "" || (int)dataTable1.Rows[i][10] > 0 || (int)dataTable1.Rows[i][11] > 0 || (bool)dataTable1.Rows[i][12] || (bool)dataTable1.Rows[i][16])
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.GradientActiveCaption;
                        }
                        if (dataTable1.Rows[i][5].ToString() != "")
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = SystemColors.ControlLight;
                        }
                    });
                }
                if (MultiTrabl == false)
                {
                    MultiTrabl = true;
                    toolStripButton25.Text = "已启用多译提示";
                    if (MyDpi < 144F)
                    {
                        toolStripButton25.Image = Athena_A.Properties.Resources.Info18;
                    }
                    else if (MyDpi >= 144F && MyDpi < 192F)
                    {
                        toolStripButton25.Image = Athena_A.Properties.Resources.Info24;
                    }
                    else if (MyDpi >= 192F)
                    {
                        toolStripButton25.Image = Athena_A.Properties.Resources.Info32;
                    }
                }
                RowEnterDis(0);
            }
            else
            {
                ClearText();
                MessageBox.Show("没有找到符合条件的内容。", "确定", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Activate();
        }

        private bool MultiTrabl = true;
		
        private void toolStripButton25_Click(object sender, EventArgs e)
        {
            if (MultiTrabl)
            {
                MultiTrabl = false;
                toolStripButton25.Text = "已禁用多译提示";
                if (MyDpi < 144F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.InfoG18;
                }
                else if (MyDpi >= 144F && MyDpi < 192F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.InfoG24;
                }
                else if (MyDpi >= 192F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.InfoG32;
                }
                if (SD.Visible)
                {
                    SD.Hide();
                }
            }
            else
            {
                MultiTrabl = true;
                toolStripButton25.Text = "已启用多译提示";
                if (MyDpi < 144F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.Info18;
                }
                else if (MyDpi >= 144F && MyDpi < 192F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.Info24;
                }
                else if (MyDpi >= 192F)
                {
                    toolStripButton25.Image = Athena_A.Properties.Resources.Info32;
                }
            }
        }
    }
}