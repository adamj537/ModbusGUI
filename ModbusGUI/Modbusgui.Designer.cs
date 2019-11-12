namespace ModbusGUI
{
    partial class Modbusgui
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Modbusgui));
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configGroupBox = new System.Windows.Forms.GroupBox();
			this.stopBitsCb = new System.Windows.Forms.ComboBox();
			this.stopBitsLbl = new System.Windows.Forms.Label();
			this.portOpenBtn = new System.Windows.Forms.Button();
			this.modeCb = new System.Windows.Forms.ComboBox();
			this.modeLbl = new System.Windows.Forms.Label();
			this.parityCb = new System.Windows.Forms.ComboBox();
			this.parityLbl = new System.Windows.Forms.Label();
			this.baudCb = new System.Windows.Forms.ComboBox();
			this.baudLbl = new System.Windows.Forms.Label();
			this.portRefreshBtn = new System.Windows.Forms.Button();
			this.comPortCb = new System.Windows.Forms.ComboBox();
			this.comPortLbl = new System.Windows.Forms.Label();
			this.dataTabControl = new System.Windows.Forms.TabControl();
			this.inputRegistersTabPage = new System.Windows.Forms.TabPage();
			this.inputPollBtn = new System.Windows.Forms.Button();
			this.inputRegStringLenTb = new System.Windows.Forms.TextBox();
			this.inputRegStringLenLbl = new System.Windows.Forms.Label();
			this.inputRegReadBtn = new System.Windows.Forms.Button();
			this.inputRegDataTb = new System.Windows.Forms.TextBox();
			this.inputRegDataLbl = new System.Windows.Forms.Label();
			this.inputRegFormatLbl = new System.Windows.Forms.Label();
			this.inputRegFormatCb = new System.Windows.Forms.ComboBox();
			this.inputRegAddrTb = new System.Windows.Forms.TextBox();
			this.inputRegAddrLbl = new System.Windows.Forms.Label();
			this.holdingRegistersTabPage = new System.Windows.Forms.TabPage();
			this.holdingRegStringLenTb = new System.Windows.Forms.TextBox();
			this.holdingRegStringLenLbl = new System.Windows.Forms.Label();
			this.holdingRegWriteBtn = new System.Windows.Forms.Button();
			this.holdingRegReadBtn = new System.Windows.Forms.Button();
			this.holdingRegDataTb = new System.Windows.Forms.TextBox();
			this.holdingRegDataLbl = new System.Windows.Forms.Label();
			this.holdingRegFormatCb = new System.Windows.Forms.ComboBox();
			this.holdingRegFormatLbl = new System.Windows.Forms.Label();
			this.holdingRegAddrTb = new System.Windows.Forms.TextBox();
			this.holdingRegAddrLbl = new System.Windows.Forms.Label();
			this.coilsTabPage = new System.Windows.Forms.TabPage();
			this.coilWriteBtn = new System.Windows.Forms.Button();
			this.coilReadBtn = new System.Windows.Forms.Button();
			this.coilFalseRadioBtn = new System.Windows.Forms.RadioButton();
			this.coilTrueRadioBtn = new System.Windows.Forms.RadioButton();
			this.coilAddrTb = new System.Windows.Forms.TextBox();
			this.coilAddrLbl = new System.Windows.Forms.Label();
			this.slaveAddrLbl = new System.Windows.Forms.Label();
			this.slaveAddrTb = new System.Windows.Forms.TextBox();
			this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
			this.mainStatusLbl = new System.Windows.Forms.ToolStripStatusLabel();
			this.mainMenuStrip.SuspendLayout();
			this.configGroupBox.SuspendLayout();
			this.dataTabControl.SuspendLayout();
			this.inputRegistersTabPage.SuspendLayout();
			this.holdingRegistersTabPage.SuspendLayout();
			this.coilsTabPage.SuspendLayout();
			this.mainStatusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(647, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// configGroupBox
			// 
			this.configGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.configGroupBox.Controls.Add(this.stopBitsCb);
			this.configGroupBox.Controls.Add(this.stopBitsLbl);
			this.configGroupBox.Controls.Add(this.portOpenBtn);
			this.configGroupBox.Controls.Add(this.modeCb);
			this.configGroupBox.Controls.Add(this.modeLbl);
			this.configGroupBox.Controls.Add(this.parityCb);
			this.configGroupBox.Controls.Add(this.parityLbl);
			this.configGroupBox.Controls.Add(this.baudCb);
			this.configGroupBox.Controls.Add(this.baudLbl);
			this.configGroupBox.Controls.Add(this.portRefreshBtn);
			this.configGroupBox.Controls.Add(this.comPortCb);
			this.configGroupBox.Controls.Add(this.comPortLbl);
			this.configGroupBox.Location = new System.Drawing.Point(12, 27);
			this.configGroupBox.Name = "configGroupBox";
			this.configGroupBox.Size = new System.Drawing.Size(623, 87);
			this.configGroupBox.TabIndex = 1;
			this.configGroupBox.TabStop = false;
			this.configGroupBox.Text = "Configuration";
			// 
			// stopBitsCb
			// 
			this.stopBitsCb.FormattingEnabled = true;
			this.stopBitsCb.Items.AddRange(new object[] {
            "1",
            "2"});
			this.stopBitsCb.Location = new System.Drawing.Point(243, 49);
			this.stopBitsCb.Name = "stopBitsCb";
			this.stopBitsCb.Size = new System.Drawing.Size(73, 21);
			this.stopBitsCb.TabIndex = 11;
			// 
			// stopBitsLbl
			// 
			this.stopBitsLbl.AutoSize = true;
			this.stopBitsLbl.Location = new System.Drawing.Point(185, 52);
			this.stopBitsLbl.Name = "stopBitsLbl";
			this.stopBitsLbl.Size = new System.Drawing.Size(52, 13);
			this.stopBitsLbl.TabIndex = 10;
			this.stopBitsLbl.Text = "Stop Bits:";
			// 
			// portOpenBtn
			// 
			this.portOpenBtn.Location = new System.Drawing.Point(471, 20);
			this.portOpenBtn.Name = "portOpenBtn";
			this.portOpenBtn.Size = new System.Drawing.Size(64, 23);
			this.portOpenBtn.TabIndex = 9;
			this.portOpenBtn.Text = "Open";
			this.portOpenBtn.UseVisualStyleBackColor = true;
			this.portOpenBtn.Click += new System.EventHandler(this.portOpenBtn_Click);
			// 
			// modeCb
			// 
			this.modeCb.FormattingEnabled = true;
			this.modeCb.Items.AddRange(new object[] {
            "RTU",
            "ASCII"});
			this.modeCb.Location = new System.Drawing.Point(367, 49);
			this.modeCb.Name = "modeCb";
			this.modeCb.Size = new System.Drawing.Size(73, 21);
			this.modeCb.TabIndex = 8;
			// 
			// modeLbl
			// 
			this.modeLbl.AutoSize = true;
			this.modeLbl.Location = new System.Drawing.Point(324, 52);
			this.modeLbl.Name = "modeLbl";
			this.modeLbl.Size = new System.Drawing.Size(37, 13);
			this.modeLbl.TabIndex = 7;
			this.modeLbl.Text = "Mode:";
			// 
			// parityCb
			// 
			this.parityCb.FormattingEnabled = true;
			this.parityCb.Items.AddRange(new object[] {
            "Even",
            "Odd",
            "None"});
			this.parityCb.Location = new System.Drawing.Point(367, 22);
			this.parityCb.Name = "parityCb";
			this.parityCb.Size = new System.Drawing.Size(73, 21);
			this.parityCb.TabIndex = 6;
			// 
			// parityLbl
			// 
			this.parityLbl.AutoSize = true;
			this.parityLbl.Location = new System.Drawing.Point(325, 25);
			this.parityLbl.Name = "parityLbl";
			this.parityLbl.Size = new System.Drawing.Size(36, 13);
			this.parityLbl.TabIndex = 5;
			this.parityLbl.Text = "Parity:";
			// 
			// baudCb
			// 
			this.baudCb.FormattingEnabled = true;
			this.baudCb.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "76800",
            "115200"});
			this.baudCb.Location = new System.Drawing.Point(243, 22);
			this.baudCb.Name = "baudCb";
			this.baudCb.Size = new System.Drawing.Size(73, 21);
			this.baudCb.TabIndex = 4;
			// 
			// baudLbl
			// 
			this.baudLbl.AutoSize = true;
			this.baudLbl.Location = new System.Drawing.Point(176, 25);
			this.baudLbl.Name = "baudLbl";
			this.baudLbl.Size = new System.Drawing.Size(61, 13);
			this.baudLbl.TabIndex = 3;
			this.baudLbl.Text = "Baud Rate:";
			// 
			// portRefreshBtn
			// 
			this.portRefreshBtn.Image = ((System.Drawing.Image)(resources.GetObject("portRefreshBtn.Image")));
			this.portRefreshBtn.Location = new System.Drawing.Point(142, 20);
			this.portRefreshBtn.Name = "portRefreshBtn";
			this.portRefreshBtn.Size = new System.Drawing.Size(28, 23);
			this.portRefreshBtn.TabIndex = 2;
			this.portRefreshBtn.UseVisualStyleBackColor = true;
			this.portRefreshBtn.Click += new System.EventHandler(this.portRefreshBtn_Click);
			// 
			// comPortCb
			// 
			this.comPortCb.FormattingEnabled = true;
			this.comPortCb.Location = new System.Drawing.Point(68, 22);
			this.comPortCb.Name = "comPortCb";
			this.comPortCb.Size = new System.Drawing.Size(68, 21);
			this.comPortCb.TabIndex = 1;
			// 
			// comPortLbl
			// 
			this.comPortLbl.AutoSize = true;
			this.comPortLbl.Location = new System.Drawing.Point(6, 25);
			this.comPortLbl.Name = "comPortLbl";
			this.comPortLbl.Size = new System.Drawing.Size(56, 13);
			this.comPortLbl.TabIndex = 0;
			this.comPortLbl.Text = "COM Port:";
			// 
			// dataTabControl
			// 
			this.dataTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataTabControl.Controls.Add(this.inputRegistersTabPage);
			this.dataTabControl.Controls.Add(this.holdingRegistersTabPage);
			this.dataTabControl.Controls.Add(this.coilsTabPage);
			this.dataTabControl.Location = new System.Drawing.Point(12, 146);
			this.dataTabControl.Name = "dataTabControl";
			this.dataTabControl.SelectedIndex = 0;
			this.dataTabControl.Size = new System.Drawing.Size(623, 141);
			this.dataTabControl.TabIndex = 2;
			// 
			// inputRegistersTabPage
			// 
			this.inputRegistersTabPage.Controls.Add(this.inputPollBtn);
			this.inputRegistersTabPage.Controls.Add(this.inputRegStringLenTb);
			this.inputRegistersTabPage.Controls.Add(this.inputRegStringLenLbl);
			this.inputRegistersTabPage.Controls.Add(this.inputRegReadBtn);
			this.inputRegistersTabPage.Controls.Add(this.inputRegDataTb);
			this.inputRegistersTabPage.Controls.Add(this.inputRegDataLbl);
			this.inputRegistersTabPage.Controls.Add(this.inputRegFormatLbl);
			this.inputRegistersTabPage.Controls.Add(this.inputRegFormatCb);
			this.inputRegistersTabPage.Controls.Add(this.inputRegAddrTb);
			this.inputRegistersTabPage.Controls.Add(this.inputRegAddrLbl);
			this.inputRegistersTabPage.Location = new System.Drawing.Point(4, 22);
			this.inputRegistersTabPage.Name = "inputRegistersTabPage";
			this.inputRegistersTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.inputRegistersTabPage.Size = new System.Drawing.Size(615, 115);
			this.inputRegistersTabPage.TabIndex = 0;
			this.inputRegistersTabPage.Text = "Input Registers";
			this.inputRegistersTabPage.UseVisualStyleBackColor = true;
			// 
			// inputPollBtn
			// 
			this.inputPollBtn.Enabled = false;
			this.inputPollBtn.Location = new System.Drawing.Point(376, 60);
			this.inputPollBtn.Name = "inputPollBtn";
			this.inputPollBtn.Size = new System.Drawing.Size(75, 23);
			this.inputPollBtn.TabIndex = 9;
			this.inputPollBtn.Text = "Poll";
			this.inputPollBtn.UseVisualStyleBackColor = true;
			this.inputPollBtn.Click += new System.EventHandler(this.inputPollBtn_Click);
			// 
			// inputRegStringLenTb
			// 
			this.inputRegStringLenTb.Enabled = false;
			this.inputRegStringLenTb.Location = new System.Drawing.Point(505, 16);
			this.inputRegStringLenTb.Name = "inputRegStringLenTb";
			this.inputRegStringLenTb.Size = new System.Drawing.Size(101, 20);
			this.inputRegStringLenTb.TabIndex = 8;
			// 
			// inputRegStringLenLbl
			// 
			this.inputRegStringLenLbl.AutoSize = true;
			this.inputRegStringLenLbl.Location = new System.Drawing.Point(426, 19);
			this.inputRegStringLenLbl.Name = "inputRegStringLenLbl";
			this.inputRegStringLenLbl.Size = new System.Drawing.Size(73, 13);
			this.inputRegStringLenLbl.TabIndex = 7;
			this.inputRegStringLenLbl.Text = "String Length:";
			// 
			// inputRegReadBtn
			// 
			this.inputRegReadBtn.Enabled = false;
			this.inputRegReadBtn.Location = new System.Drawing.Point(295, 60);
			this.inputRegReadBtn.Name = "inputRegReadBtn";
			this.inputRegReadBtn.Size = new System.Drawing.Size(75, 23);
			this.inputRegReadBtn.TabIndex = 6;
			this.inputRegReadBtn.Text = "Read";
			this.inputRegReadBtn.UseVisualStyleBackColor = true;
			this.inputRegReadBtn.Click += new System.EventHandler(this.inputRegReadBtn_Click);
			// 
			// inputRegDataTb
			// 
			this.inputRegDataTb.Location = new System.Drawing.Point(91, 62);
			this.inputRegDataTb.Name = "inputRegDataTb";
			this.inputRegDataTb.Size = new System.Drawing.Size(158, 20);
			this.inputRegDataTb.TabIndex = 5;
			// 
			// inputRegDataLbl
			// 
			this.inputRegDataLbl.AutoSize = true;
			this.inputRegDataLbl.Location = new System.Drawing.Point(6, 65);
			this.inputRegDataLbl.Name = "inputRegDataLbl";
			this.inputRegDataLbl.Size = new System.Drawing.Size(79, 13);
			this.inputRegDataLbl.TabIndex = 4;
			this.inputRegDataLbl.Text = "Register Value:";
			// 
			// inputRegFormatLbl
			// 
			this.inputRegFormatLbl.AutoSize = true;
			this.inputRegFormatLbl.Location = new System.Drawing.Point(221, 19);
			this.inputRegFormatLbl.Name = "inputRegFormatLbl";
			this.inputRegFormatLbl.Size = new System.Drawing.Size(68, 13);
			this.inputRegFormatLbl.TabIndex = 3;
			this.inputRegFormatLbl.Text = "Data Format:";
			// 
			// inputRegFormatCb
			// 
			this.inputRegFormatCb.FormattingEnabled = true;
			this.inputRegFormatCb.Items.AddRange(new object[] {
            "Float",
            "Int32",
            "UInt32",
            "Int16",
            "UInt16",
            "String"});
			this.inputRegFormatCb.Location = new System.Drawing.Point(295, 16);
			this.inputRegFormatCb.Name = "inputRegFormatCb";
			this.inputRegFormatCb.Size = new System.Drawing.Size(121, 21);
			this.inputRegFormatCb.TabIndex = 2;
			this.inputRegFormatCb.SelectedIndexChanged += new System.EventHandler(this.FormatCb_SelectedIndexChanged);
			// 
			// inputRegAddrTb
			// 
			this.inputRegAddrTb.Location = new System.Drawing.Point(102, 16);
			this.inputRegAddrTb.Name = "inputRegAddrTb";
			this.inputRegAddrTb.Size = new System.Drawing.Size(100, 20);
			this.inputRegAddrTb.TabIndex = 1;
			// 
			// inputRegAddrLbl
			// 
			this.inputRegAddrLbl.AutoSize = true;
			this.inputRegAddrLbl.Location = new System.Drawing.Point(6, 19);
			this.inputRegAddrLbl.Name = "inputRegAddrLbl";
			this.inputRegAddrLbl.Size = new System.Drawing.Size(90, 13);
			this.inputRegAddrLbl.TabIndex = 0;
			this.inputRegAddrLbl.Text = "Register Address:";
			// 
			// holdingRegistersTabPage
			// 
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegStringLenTb);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegStringLenLbl);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegWriteBtn);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegReadBtn);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegDataTb);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegDataLbl);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegFormatCb);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegFormatLbl);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegAddrTb);
			this.holdingRegistersTabPage.Controls.Add(this.holdingRegAddrLbl);
			this.holdingRegistersTabPage.Location = new System.Drawing.Point(4, 22);
			this.holdingRegistersTabPage.Name = "holdingRegistersTabPage";
			this.holdingRegistersTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.holdingRegistersTabPage.Size = new System.Drawing.Size(615, 115);
			this.holdingRegistersTabPage.TabIndex = 1;
			this.holdingRegistersTabPage.Text = "Holding Registers";
			this.holdingRegistersTabPage.UseVisualStyleBackColor = true;
			// 
			// holdingRegStringLenTb
			// 
			this.holdingRegStringLenTb.Enabled = false;
			this.holdingRegStringLenTb.Location = new System.Drawing.Point(502, 16);
			this.holdingRegStringLenTb.Name = "holdingRegStringLenTb";
			this.holdingRegStringLenTb.Size = new System.Drawing.Size(100, 20);
			this.holdingRegStringLenTb.TabIndex = 9;
			// 
			// holdingRegStringLenLbl
			// 
			this.holdingRegStringLenLbl.AutoSize = true;
			this.holdingRegStringLenLbl.Location = new System.Drawing.Point(423, 19);
			this.holdingRegStringLenLbl.Name = "holdingRegStringLenLbl";
			this.holdingRegStringLenLbl.Size = new System.Drawing.Size(73, 13);
			this.holdingRegStringLenLbl.TabIndex = 8;
			this.holdingRegStringLenLbl.Text = "String Length:";
			// 
			// holdingRegWriteBtn
			// 
			this.holdingRegWriteBtn.Enabled = false;
			this.holdingRegWriteBtn.Location = new System.Drawing.Point(377, 61);
			this.holdingRegWriteBtn.Name = "holdingRegWriteBtn";
			this.holdingRegWriteBtn.Size = new System.Drawing.Size(75, 23);
			this.holdingRegWriteBtn.TabIndex = 7;
			this.holdingRegWriteBtn.Text = "Write";
			this.holdingRegWriteBtn.UseVisualStyleBackColor = true;
			this.holdingRegWriteBtn.Click += new System.EventHandler(this.holdingRegWriteBtn_Click);
			// 
			// holdingRegReadBtn
			// 
			this.holdingRegReadBtn.Enabled = false;
			this.holdingRegReadBtn.Location = new System.Drawing.Point(296, 61);
			this.holdingRegReadBtn.Name = "holdingRegReadBtn";
			this.holdingRegReadBtn.Size = new System.Drawing.Size(75, 23);
			this.holdingRegReadBtn.TabIndex = 6;
			this.holdingRegReadBtn.Text = "Read";
			this.holdingRegReadBtn.UseVisualStyleBackColor = true;
			this.holdingRegReadBtn.Click += new System.EventHandler(this.holdingRegReadBtn_Click);
			// 
			// holdingRegDataTb
			// 
			this.holdingRegDataTb.Location = new System.Drawing.Point(91, 63);
			this.holdingRegDataTb.Name = "holdingRegDataTb";
			this.holdingRegDataTb.Size = new System.Drawing.Size(162, 20);
			this.holdingRegDataTb.TabIndex = 5;
			// 
			// holdingRegDataLbl
			// 
			this.holdingRegDataLbl.AutoSize = true;
			this.holdingRegDataLbl.Location = new System.Drawing.Point(6, 66);
			this.holdingRegDataLbl.Name = "holdingRegDataLbl";
			this.holdingRegDataLbl.Size = new System.Drawing.Size(79, 13);
			this.holdingRegDataLbl.TabIndex = 4;
			this.holdingRegDataLbl.Text = "Register Value:";
			// 
			// holdingRegFormatCb
			// 
			this.holdingRegFormatCb.FormattingEnabled = true;
			this.holdingRegFormatCb.Items.AddRange(new object[] {
            "Float",
            "Int32",
            "UInt32",
            "Int16",
            "UInt16",
            "String"});
			this.holdingRegFormatCb.Location = new System.Drawing.Point(296, 16);
			this.holdingRegFormatCb.Name = "holdingRegFormatCb";
			this.holdingRegFormatCb.Size = new System.Drawing.Size(121, 21);
			this.holdingRegFormatCb.TabIndex = 3;
			this.holdingRegFormatCb.SelectedIndexChanged += new System.EventHandler(this.FormatCb_SelectedIndexChanged);
			// 
			// holdingRegFormatLbl
			// 
			this.holdingRegFormatLbl.AutoSize = true;
			this.holdingRegFormatLbl.Location = new System.Drawing.Point(222, 19);
			this.holdingRegFormatLbl.Name = "holdingRegFormatLbl";
			this.holdingRegFormatLbl.Size = new System.Drawing.Size(68, 13);
			this.holdingRegFormatLbl.TabIndex = 2;
			this.holdingRegFormatLbl.Text = "Data Format:";
			// 
			// holdingRegAddrTb
			// 
			this.holdingRegAddrTb.Location = new System.Drawing.Point(102, 16);
			this.holdingRegAddrTb.Name = "holdingRegAddrTb";
			this.holdingRegAddrTb.Size = new System.Drawing.Size(100, 20);
			this.holdingRegAddrTb.TabIndex = 1;
			// 
			// holdingRegAddrLbl
			// 
			this.holdingRegAddrLbl.AutoSize = true;
			this.holdingRegAddrLbl.Location = new System.Drawing.Point(6, 19);
			this.holdingRegAddrLbl.Name = "holdingRegAddrLbl";
			this.holdingRegAddrLbl.Size = new System.Drawing.Size(90, 13);
			this.holdingRegAddrLbl.TabIndex = 0;
			this.holdingRegAddrLbl.Text = "Register Address:";
			// 
			// coilsTabPage
			// 
			this.coilsTabPage.Controls.Add(this.coilWriteBtn);
			this.coilsTabPage.Controls.Add(this.coilReadBtn);
			this.coilsTabPage.Controls.Add(this.coilFalseRadioBtn);
			this.coilsTabPage.Controls.Add(this.coilTrueRadioBtn);
			this.coilsTabPage.Controls.Add(this.coilAddrTb);
			this.coilsTabPage.Controls.Add(this.coilAddrLbl);
			this.coilsTabPage.Location = new System.Drawing.Point(4, 22);
			this.coilsTabPage.Name = "coilsTabPage";
			this.coilsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.coilsTabPage.Size = new System.Drawing.Size(615, 115);
			this.coilsTabPage.TabIndex = 2;
			this.coilsTabPage.Text = "Coils";
			this.coilsTabPage.UseVisualStyleBackColor = true;
			// 
			// coilWriteBtn
			// 
			this.coilWriteBtn.Enabled = false;
			this.coilWriteBtn.Location = new System.Drawing.Point(290, 19);
			this.coilWriteBtn.Name = "coilWriteBtn";
			this.coilWriteBtn.Size = new System.Drawing.Size(75, 23);
			this.coilWriteBtn.TabIndex = 5;
			this.coilWriteBtn.Text = "Write";
			this.coilWriteBtn.UseVisualStyleBackColor = true;
			this.coilWriteBtn.Click += new System.EventHandler(this.coilWriteBtn_Click);
			// 
			// coilReadBtn
			// 
			this.coilReadBtn.Enabled = false;
			this.coilReadBtn.Location = new System.Drawing.Point(209, 19);
			this.coilReadBtn.Name = "coilReadBtn";
			this.coilReadBtn.Size = new System.Drawing.Size(75, 23);
			this.coilReadBtn.TabIndex = 4;
			this.coilReadBtn.Text = "Read";
			this.coilReadBtn.UseVisualStyleBackColor = true;
			this.coilReadBtn.Click += new System.EventHandler(this.coilReadBtn_Click);
			// 
			// coilFalseRadioBtn
			// 
			this.coilFalseRadioBtn.AutoSize = true;
			this.coilFalseRadioBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.coilFalseRadioBtn.Location = new System.Drawing.Point(27, 77);
			this.coilFalseRadioBtn.Name = "coilFalseRadioBtn";
			this.coilFalseRadioBtn.Size = new System.Drawing.Size(50, 17);
			this.coilFalseRadioBtn.TabIndex = 3;
			this.coilFalseRadioBtn.Text = "False";
			this.coilFalseRadioBtn.UseVisualStyleBackColor = true;
			// 
			// coilTrueRadioBtn
			// 
			this.coilTrueRadioBtn.AutoSize = true;
			this.coilTrueRadioBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.coilTrueRadioBtn.Checked = true;
			this.coilTrueRadioBtn.Location = new System.Drawing.Point(30, 54);
			this.coilTrueRadioBtn.Name = "coilTrueRadioBtn";
			this.coilTrueRadioBtn.Size = new System.Drawing.Size(47, 17);
			this.coilTrueRadioBtn.TabIndex = 2;
			this.coilTrueRadioBtn.TabStop = true;
			this.coilTrueRadioBtn.Text = "True";
			this.coilTrueRadioBtn.UseVisualStyleBackColor = true;
			// 
			// coilAddrTb
			// 
			this.coilAddrTb.Location = new System.Drawing.Point(80, 21);
			this.coilAddrTb.Name = "coilAddrTb";
			this.coilAddrTb.Size = new System.Drawing.Size(100, 20);
			this.coilAddrTb.TabIndex = 1;
			// 
			// coilAddrLbl
			// 
			this.coilAddrLbl.AutoSize = true;
			this.coilAddrLbl.Location = new System.Drawing.Point(6, 24);
			this.coilAddrLbl.Name = "coilAddrLbl";
			this.coilAddrLbl.Size = new System.Drawing.Size(68, 13);
			this.coilAddrLbl.TabIndex = 0;
			this.coilAddrLbl.Text = "Coil Address:";
			// 
			// slaveAddrLbl
			// 
			this.slaveAddrLbl.AutoSize = true;
			this.slaveAddrLbl.Location = new System.Drawing.Point(6, 123);
			this.slaveAddrLbl.Name = "slaveAddrLbl";
			this.slaveAddrLbl.Size = new System.Drawing.Size(95, 13);
			this.slaveAddrLbl.TabIndex = 3;
			this.slaveAddrLbl.Text = "Slave Address:  0x";
			// 
			// slaveAddrTb
			// 
			this.slaveAddrTb.Location = new System.Drawing.Point(107, 120);
			this.slaveAddrTb.Name = "slaveAddrTb";
			this.slaveAddrTb.Size = new System.Drawing.Size(69, 20);
			this.slaveAddrTb.TabIndex = 4;
			this.slaveAddrTb.Leave += new System.EventHandler(this.slaveAddrTb_Leave);
			// 
			// mainStatusStrip
			// 
			this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainStatusLbl});
			this.mainStatusStrip.Location = new System.Drawing.Point(0, 290);
			this.mainStatusStrip.Name = "mainStatusStrip";
			this.mainStatusStrip.Size = new System.Drawing.Size(647, 22);
			this.mainStatusStrip.TabIndex = 5;
			this.mainStatusStrip.Text = "statusStrip1";
			// 
			// mainStatusLbl
			// 
			this.mainStatusLbl.Name = "mainStatusLbl";
			this.mainStatusLbl.Size = new System.Drawing.Size(0, 17);
			// 
			// Modbusgui
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(647, 312);
			this.Controls.Add(this.mainStatusStrip);
			this.Controls.Add(this.slaveAddrTb);
			this.Controls.Add(this.slaveAddrLbl);
			this.Controls.Add(this.dataTabControl);
			this.Controls.Add(this.configGroupBox);
			this.Controls.Add(this.mainMenuStrip);
			this.MainMenuStrip = this.mainMenuStrip;
			this.Name = "Modbusgui";
			this.Text = "Modbus Register Interface";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.configGroupBox.ResumeLayout(false);
			this.configGroupBox.PerformLayout();
			this.dataTabControl.ResumeLayout(false);
			this.inputRegistersTabPage.ResumeLayout(false);
			this.inputRegistersTabPage.PerformLayout();
			this.holdingRegistersTabPage.ResumeLayout(false);
			this.holdingRegistersTabPage.PerformLayout();
			this.coilsTabPage.ResumeLayout(false);
			this.coilsTabPage.PerformLayout();
			this.mainStatusStrip.ResumeLayout(false);
			this.mainStatusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox configGroupBox;
        private System.Windows.Forms.ComboBox parityCb;
        private System.Windows.Forms.Label parityLbl;
        private System.Windows.Forms.ComboBox baudCb;
        private System.Windows.Forms.Label baudLbl;
        private System.Windows.Forms.Button portRefreshBtn;
        private System.Windows.Forms.ComboBox comPortCb;
        private System.Windows.Forms.Label comPortLbl;
        private System.Windows.Forms.ComboBox modeCb;
        private System.Windows.Forms.Label modeLbl;
        private System.Windows.Forms.Button portOpenBtn;
        private System.Windows.Forms.TabControl dataTabControl;
        private System.Windows.Forms.TabPage inputRegistersTabPage;
        private System.Windows.Forms.Button inputRegReadBtn;
        private System.Windows.Forms.TextBox inputRegDataTb;
        private System.Windows.Forms.Label inputRegDataLbl;
        private System.Windows.Forms.Label inputRegFormatLbl;
        private System.Windows.Forms.ComboBox inputRegFormatCb;
        private System.Windows.Forms.TextBox inputRegAddrTb;
        private System.Windows.Forms.Label inputRegAddrLbl;
        private System.Windows.Forms.TabPage holdingRegistersTabPage;
        private System.Windows.Forms.Button holdingRegWriteBtn;
        private System.Windows.Forms.Button holdingRegReadBtn;
        private System.Windows.Forms.TextBox holdingRegDataTb;
        private System.Windows.Forms.Label holdingRegDataLbl;
        private System.Windows.Forms.ComboBox holdingRegFormatCb;
        private System.Windows.Forms.Label holdingRegFormatLbl;
        private System.Windows.Forms.TextBox holdingRegAddrTb;
        private System.Windows.Forms.Label holdingRegAddrLbl;
        private System.Windows.Forms.Label slaveAddrLbl;
        private System.Windows.Forms.TextBox slaveAddrTb;
        private System.Windows.Forms.TabPage coilsTabPage;
        private System.Windows.Forms.Button coilWriteBtn;
        private System.Windows.Forms.Button coilReadBtn;
        private System.Windows.Forms.RadioButton coilFalseRadioBtn;
        private System.Windows.Forms.RadioButton coilTrueRadioBtn;
        private System.Windows.Forms.TextBox coilAddrTb;
        private System.Windows.Forms.Label coilAddrLbl;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mainStatusLbl;
        private System.Windows.Forms.TextBox inputRegStringLenTb;
        private System.Windows.Forms.Label inputRegStringLenLbl;
        private System.Windows.Forms.ComboBox stopBitsCb;
		private System.Windows.Forms.Label stopBitsLbl;
		private System.Windows.Forms.Button inputPollBtn;
        private System.Windows.Forms.TextBox holdingRegStringLenTb;
		private System.Windows.Forms.Label holdingRegStringLenLbl;
    }
}

