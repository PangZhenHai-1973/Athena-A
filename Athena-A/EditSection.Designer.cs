namespace Athena_A
{
    partial class EditSection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.序号 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.名称 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.虚拟大小 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.虚拟偏移 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.物理大小 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.物理偏移 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.属性 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.特殊说明 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.序号,
            this.名称,
            this.虚拟大小,
            this.虚拟偏移,
            this.物理大小,
            this.物理偏移,
            this.属性,
            this.特殊说明});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridView1.Location = new System.Drawing.Point(0, 69);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.Size = new System.Drawing.Size(670, 386);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(583, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "浏览(&B)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(59, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ShortcutsEnabled = false;
            this.textBox1.Size = new System.Drawing.Size(599, 21);
            this.textBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "文件：";
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(340, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "增加(&A)";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(421, 40);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "编辑(&E)";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(502, 40);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "删除(&D)";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.InitialDelay = 300;
            this.toolTip1.ReshowDelay = 100;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "EXE 文件(*.exe)|*.exe|DLL 文件(*.dll)|*.dll|所有文件(*.*)|*.*";
            // 
            // 序号
            // 
            this.序号.FillWeight = 40F;
            this.序号.HeaderText = "序号";
            this.序号.MinimumWidth = 40;
            this.序号.Name = "序号";
            this.序号.ReadOnly = true;
            this.序号.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.序号.Width = 40;
            // 
            // 名称
            // 
            this.名称.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.名称.FillWeight = 70F;
            this.名称.HeaderText = "名称";
            this.名称.MinimumWidth = 86;
            this.名称.Name = "名称";
            this.名称.ReadOnly = true;
            this.名称.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.名称.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.名称.Width = 86;
            // 
            // 虚拟大小
            // 
            this.虚拟大小.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.虚拟大小.FillWeight = 110F;
            this.虚拟大小.HeaderText = "虚拟大小";
            this.虚拟大小.MinimumWidth = 116;
            this.虚拟大小.Name = "虚拟大小";
            this.虚拟大小.ReadOnly = true;
            this.虚拟大小.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.虚拟大小.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.虚拟大小.Width = 116;
            // 
            // 虚拟偏移
            // 
            this.虚拟偏移.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.虚拟偏移.FillWeight = 80F;
            this.虚拟偏移.HeaderText = "虚拟偏移";
            this.虚拟偏移.MinimumWidth = 80;
            this.虚拟偏移.Name = "虚拟偏移";
            this.虚拟偏移.ReadOnly = true;
            this.虚拟偏移.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.虚拟偏移.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 物理大小
            // 
            this.物理大小.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.物理大小.FillWeight = 80F;
            this.物理大小.HeaderText = "物理大小";
            this.物理大小.MinimumWidth = 80;
            this.物理大小.Name = "物理大小";
            this.物理大小.ReadOnly = true;
            this.物理大小.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.物理大小.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 物理偏移
            // 
            this.物理偏移.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.物理偏移.FillWeight = 80F;
            this.物理偏移.HeaderText = "物理偏移";
            this.物理偏移.MinimumWidth = 80;
            this.物理偏移.Name = "物理偏移";
            this.物理偏移.ReadOnly = true;
            this.物理偏移.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.物理偏移.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 属性
            // 
            this.属性.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.属性.FillWeight = 80F;
            this.属性.HeaderText = "属性";
            this.属性.MinimumWidth = 80;
            this.属性.Name = "属性";
            this.属性.ReadOnly = true;
            this.属性.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.属性.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // 特殊说明
            // 
            this.特殊说明.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.特殊说明.FillWeight = 80F;
            this.特殊说明.HeaderText = "特殊说明";
            this.特殊说明.MinimumWidth = 80;
            this.特殊说明.Name = "特殊说明";
            this.特殊说明.ReadOnly = true;
            this.特殊说明.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.特殊说明.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EditSection
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(670, 455);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dataGridView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditSection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "编辑 PE 节区";
            this.Shown += new System.EventHandler(this.EditSection_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.EditSection_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.EditSection_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 序号;
        private System.Windows.Forms.DataGridViewTextBoxColumn 名称;
        private System.Windows.Forms.DataGridViewTextBoxColumn 虚拟大小;
        private System.Windows.Forms.DataGridViewTextBoxColumn 虚拟偏移;
        private System.Windows.Forms.DataGridViewTextBoxColumn 物理大小;
        private System.Windows.Forms.DataGridViewTextBoxColumn 物理偏移;
        private System.Windows.Forms.DataGridViewTextBoxColumn 属性;
        private System.Windows.Forms.DataGridViewTextBoxColumn 特殊说明;
    }
}