using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Athena_A
{
    public partial class DigitalSignature : Form
    {
        ArrayList AL = new ArrayList();
        BlockingCollection<int> CountNum = new BlockingCollection<int>();

        public DigitalSignature()
        {
            InitializeComponent();
        }

        private void DeleteSignature(string s)
        {
            FileInfo f = new FileInfo(s);
            bool bl = true;
            if (f.Attributes.ToString().ToLower().Contains("readonly") == true)
            {
                try
                {
                    f.Attributes = FileAttributes.Normal;
                }
                catch
                {
                    bl = false;
                }
            }
            if (bl == true)
            {
                try
                {
                    FileStream fs = new FileStream(s, FileMode.Open, FileAccess.ReadWrite);
                    BinaryReader br = new BinaryReader(fs);
                    BinaryWriter bw = new BinaryWriter(fs);
                    int i = br.ReadUInt16();
                    if (i == 23117)
                    {
                        if (fs.Length > 63)
                        {
                            fs.Seek(60, SeekOrigin.Begin);
                            i = (int)br.ReadUInt32();
                            if ((i + 6) < fs.Length)
                            {
                                fs.Seek(i, SeekOrigin.Begin);
                                if (br.ReadInt32() == 17744)
                                {
                                    fs.Seek(i + 88, SeekOrigin.Begin);
                                    bw.Write(0);//校验和
                                    fs.Seek(i + 4, SeekOrigin.Begin);
                                    if (br.ReadUInt16() == 332)
                                    {
                                        fs.Seek(i + 152, SeekOrigin.Begin);
                                    }
                                    else
                                    {
                                        fs.Seek(i + 168, SeekOrigin.Begin);
                                    }
                                    uint u1 = br.ReadUInt32();
                                    uint u2 = br.ReadUInt32();
                                    if (u1 > 0 && u1 == (fs.Length - u2))
                                    {
                                        fs.SetLength(u1);
                                        fs.Seek(-8, SeekOrigin.Current);
                                        bw.Write(0);
                                        bw.Write(0);
                                    }
                                }
                            }
                        }
                    }
                    br.Close();
                    bw.Close();
                    fs.Close();
                }
                catch
                { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                int i1 = (int)openFileDialog1.FileNames.LongLength;
                if (i1 > 0)
                {
                    AL.Clear();
                    for (int i = 0; i < i1; i++)
                    {
                        AL.Add(openFileDialog1.FileNames[i]);
                    }
                    button1.Enabled = false;
                    button2.Enabled = false;
                    progressBar1.Maximum = AL.Count;
                    timer1.Enabled = true;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int i1 = AL.Count;
            Parallel.For(0, i1, (x) =>
            {
                CountNum.TryAdd(x);
                DeleteSignature(AL[x].ToString());
            });
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timer1.Enabled = false;
            MessageBox.Show("处理完成。");
            progressBar1.Value = 0;
            int i1 = AL.Count;
            Parallel.For(0, i1, (x) =>
            {
                CountNum.TryTake(out x);
            });
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void SearchFile(string s)
        {
            DirectoryInfo di = new DirectoryInfo(s);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach (FileSystemInfo i in fsi)
            {
                if (i is DirectoryInfo)
                {
                    try
                    {
                        SearchFile(i.FullName);
                    }
                    catch
                    {
                        continue;
                    }
                }
                else
                {
                    AL.Add(i.FullName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                AL.Clear();
                SearchFile(folderBrowserDialog1.SelectedPath);
                if (AL.Count > 0)
                {
                    progressBar1.Maximum = AL.Count;
                    timer1.Enabled = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = CountNum.Count;
        }

        private void DigitalSignature_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                e.Cancel = true;
            }
        }

        private void DigitalSignature_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                Clipboard.Clear();
                StringCollection SC = new StringCollection();
                string[] filenames = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames.Length > 0)
                {
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        SC.Add(filenames[i]);
                    }
                    Clipboard.SetFileDropList(SC);
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void DigitalSignature_DragDrop(object sender, DragEventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                StringCollection SC = Clipboard.GetFileDropList();
                AL.Clear();
                for (int i = 0; i < SC.Count; i++)
                {
                    if (Directory.Exists(SC[i]))
                    {
                        SearchFile(SC[i]);
                    }
                    else
                    {
                        AL.Add(SC[i]);
                    }
                }
                if (AL.Count > 0)
                {
                    progressBar1.Maximum = AL.Count;
                    timer1.Enabled = true;
                    button1.Enabled = false;
                    button2.Enabled = false;
                    backgroundWorker1.RunWorkerAsync();
                }
            }
        }

        private void DigitalSignature_Shown(object sender, EventArgs e)
        {
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
        }
    }
}
