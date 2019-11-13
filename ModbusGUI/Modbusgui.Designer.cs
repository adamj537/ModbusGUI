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
			this.groupBoxSerialPort = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanelSerialPort = new System.Windows.Forms.TableLayoutPanel();
			this.labelComPort = new System.Windows.Forms.Label();
			this.comboBoxSerialPort = new System.Windows.Forms.ComboBox();
			this.buttonPortRefresh = new System.Windows.Forms.Button();
			this.labelBaudRate = new System.Windows.Forms.Label();
			this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
			this.labelStopBits = new System.Windows.Forms.Label();
			this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
			this.labelParity = new System.Windows.Forms.Label();
			this.comboBoxParity = new System.Windows.Forms.ComboBox();
			this.labelMode = new System.Windows.Forms.Label();
			this.comboBoxMode = new System.Windows.Forms.ComboBox();
			this.radioButtonOpen = new System.Windows.Forms.RadioButton();
			this.radioButtonClosed = new System.Windows.Forms.RadioButton();
			this.tabControlData = new System.Windows.Forms.TabControl();
			this.tabPageCoils = new System.Windows.Forms.TabPage();
			this.tableLayoutPanelCoils = new System.Windows.Forms.TableLayoutPanel();
			this.labelCoilValue = new System.Windows.Forms.Label();
			this.labelCoilAddress = new System.Windows.Forms.Label();
			this.textBoxCoilAddr = new System.Windows.Forms.TextBox();
			this.buttonCoilRead = new System.Windows.Forms.Button();
			this.buttonCoilWrite = new System.Windows.Forms.Button();
			this.radioButtonCoilTrue = new System.Windows.Forms.RadioButton();
			this.radioButtonCoilFalse = new System.Windows.Forms.RadioButton();
			this.tabPageInputRegisters = new System.Windows.Forms.TabPage();
			this.tableLayoutPanelInputRegisters = new System.Windows.Forms.TableLayoutPanel();
			this.buttonInputRegisterPoll = new System.Windows.Forms.Button();
			this.labelInputRegisterAddress = new System.Windows.Forms.Label();
			this.buttonInputRegisterRead = new System.Windows.Forms.Button();
			this.textBoxInputRegisterStringLength = new System.Windows.Forms.TextBox();
			this.textBoxInputRegisterAddress = new System.Windows.Forms.TextBox();
			this.labelInputRegisterStringLength = new System.Windows.Forms.Label();
			this.comboBoxInputRegisterFormat = new System.Windows.Forms.ComboBox();
			this.labelInputRegisterDataFormat = new System.Windows.Forms.Label();
			this.labelInputRegisterData = new System.Windows.Forms.Label();
			this.textBoxInputRegisterData = new System.Windows.Forms.TextBox();
			this.tabPageHoldingRegisters = new System.Windows.Forms.TabPage();
			this.tableLayoutPanelHoldingRegisters = new System.Windows.Forms.TableLayoutPanel();
			this.buttonHoldingRegisterWrite = new System.Windows.Forms.Button();
			this.textBoxHoldingRegisterStringLength = new System.Windows.Forms.TextBox();
			this.buttonHoldingRegisterRead = new System.Windows.Forms.Button();
			this.labelHoldingRegisterAddress = new System.Windows.Forms.Label();
			this.textBoxHoldingRegisterData = new System.Windows.Forms.TextBox();
			this.labelHoldingRegisterStringLength = new System.Windows.Forms.Label();
			this.labelHoldingRegisterData = new System.Windows.Forms.Label();
			this.textBoxHoldingRegisterAddress = new System.Windows.Forms.TextBox();
			this.labelHoldingRegisterFormat = new System.Windows.Forms.Label();
			this.comboBoxHoldingRegisterFormat = new System.Windows.Forms.ComboBox();
			this.labelSlaveAddress = new System.Windows.Forms.Label();
			this.textBoxSlaveAddr = new System.Windows.Forms.TextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.labelMainStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupBoxCommunication = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanelCommunication = new System.Windows.Forms.TableLayoutPanel();
			this.mainMenuStrip.SuspendLayout();
			this.groupBoxSerialPort.SuspendLayout();
			this.tableLayoutPanelSerialPort.SuspendLayout();
			this.tabControlData.SuspendLayout();
			this.tabPageCoils.SuspendLayout();
			this.tableLayoutPanelCoils.SuspendLayout();
			this.tabPageInputRegisters.SuspendLayout();
			this.tableLayoutPanelInputRegisters.SuspendLayout();
			this.tabPageHoldingRegisters.SuspendLayout();
			this.tableLayoutPanelHoldingRegisters.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.groupBoxCommunication.SuspendLayout();
			this.tableLayoutPanelCommunication.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(387, 24);
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
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
			// 
			// groupBoxSerialPort
			// 
			this.groupBoxSerialPort.AutoSize = true;
			this.groupBoxSerialPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBoxSerialPort.Controls.Add(this.tableLayoutPanelSerialPort);
			this.groupBoxSerialPort.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBoxSerialPort.Location = new System.Drawing.Point(0, 24);
			this.groupBoxSerialPort.Name = "groupBoxSerialPort";
			this.groupBoxSerialPort.Size = new System.Drawing.Size(387, 102);
			this.groupBoxSerialPort.TabIndex = 1;
			this.groupBoxSerialPort.TabStop = false;
			this.groupBoxSerialPort.Text = "Serial Port";
			// 
			// tableLayoutPanelSerialPort
			// 
			this.tableLayoutPanelSerialPort.AutoSize = true;
			this.tableLayoutPanelSerialPort.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelSerialPort.ColumnCount = 6;
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelSerialPort.Controls.Add(this.labelComPort, 0, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.comboBoxSerialPort, 1, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.buttonPortRefresh, 2, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.labelBaudRate, 0, 1);
			this.tableLayoutPanelSerialPort.Controls.Add(this.comboBoxBaudRate, 1, 1);
			this.tableLayoutPanelSerialPort.Controls.Add(this.labelStopBits, 3, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.comboBoxStopBits, 4, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.labelParity, 3, 1);
			this.tableLayoutPanelSerialPort.Controls.Add(this.comboBoxParity, 4, 1);
			this.tableLayoutPanelSerialPort.Controls.Add(this.labelMode, 3, 2);
			this.tableLayoutPanelSerialPort.Controls.Add(this.comboBoxMode, 4, 2);
			this.tableLayoutPanelSerialPort.Controls.Add(this.radioButtonOpen, 5, 0);
			this.tableLayoutPanelSerialPort.Controls.Add(this.radioButtonClosed, 5, 1);
			this.tableLayoutPanelSerialPort.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelSerialPort.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanelSerialPort.Name = "tableLayoutPanelSerialPort";
			this.tableLayoutPanelSerialPort.RowCount = 3;
			this.tableLayoutPanelSerialPort.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelSerialPort.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelSerialPort.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelSerialPort.Size = new System.Drawing.Size(381, 83);
			this.tableLayoutPanelSerialPort.TabIndex = 12;
			// 
			// labelComPort
			// 
			this.labelComPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelComPort.AutoSize = true;
			this.labelComPort.Location = new System.Drawing.Point(3, 8);
			this.labelComPort.Name = "labelComPort";
			this.labelComPort.Size = new System.Drawing.Size(56, 13);
			this.labelComPort.TabIndex = 0;
			this.labelComPort.Text = "COM Port:";
			// 
			// comboBoxSerialPort
			// 
			this.comboBoxSerialPort.FormattingEnabled = true;
			this.comboBoxSerialPort.Location = new System.Drawing.Point(70, 3);
			this.comboBoxSerialPort.Name = "comboBoxSerialPort";
			this.comboBoxSerialPort.Size = new System.Drawing.Size(73, 21);
			this.comboBoxSerialPort.TabIndex = 1;
			// 
			// buttonPortRefresh
			// 
			this.buttonPortRefresh.Image = ((System.Drawing.Image)(resources.GetObject("buttonPortRefresh.Image")));
			this.buttonPortRefresh.Location = new System.Drawing.Point(149, 3);
			this.buttonPortRefresh.Name = "buttonPortRefresh";
			this.buttonPortRefresh.Size = new System.Drawing.Size(28, 23);
			this.buttonPortRefresh.TabIndex = 2;
			this.buttonPortRefresh.UseVisualStyleBackColor = true;
			this.buttonPortRefresh.Click += new System.EventHandler(this.ButtonPortRefresh_Click);
			// 
			// labelBaudRate
			// 
			this.labelBaudRate.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelBaudRate.AutoSize = true;
			this.labelBaudRate.Location = new System.Drawing.Point(3, 36);
			this.labelBaudRate.Name = "labelBaudRate";
			this.labelBaudRate.Size = new System.Drawing.Size(61, 13);
			this.labelBaudRate.TabIndex = 3;
			this.labelBaudRate.Text = "Baud Rate:";
			// 
			// comboBoxBaudRate
			// 
			this.comboBoxBaudRate.FormattingEnabled = true;
			this.comboBoxBaudRate.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600",
            "76800",
            "115200"});
			this.comboBoxBaudRate.Location = new System.Drawing.Point(70, 32);
			this.comboBoxBaudRate.Name = "comboBoxBaudRate";
			this.comboBoxBaudRate.Size = new System.Drawing.Size(73, 21);
			this.comboBoxBaudRate.TabIndex = 4;
			// 
			// labelStopBits
			// 
			this.labelStopBits.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelStopBits.AutoSize = true;
			this.labelStopBits.Location = new System.Drawing.Point(183, 8);
			this.labelStopBits.Name = "labelStopBits";
			this.labelStopBits.Size = new System.Drawing.Size(52, 13);
			this.labelStopBits.TabIndex = 10;
			this.labelStopBits.Text = "Stop Bits:";
			// 
			// comboBoxStopBits
			// 
			this.comboBoxStopBits.FormattingEnabled = true;
			this.comboBoxStopBits.Items.AddRange(new object[] {
            "1",
            "2"});
			this.comboBoxStopBits.Location = new System.Drawing.Point(241, 3);
			this.comboBoxStopBits.Name = "comboBoxStopBits";
			this.comboBoxStopBits.Size = new System.Drawing.Size(73, 21);
			this.comboBoxStopBits.TabIndex = 11;
			// 
			// labelParity
			// 
			this.labelParity.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelParity.AutoSize = true;
			this.labelParity.Location = new System.Drawing.Point(183, 36);
			this.labelParity.Name = "labelParity";
			this.labelParity.Size = new System.Drawing.Size(36, 13);
			this.labelParity.TabIndex = 5;
			this.labelParity.Text = "Parity:";
			// 
			// comboBoxParity
			// 
			this.comboBoxParity.FormattingEnabled = true;
			this.comboBoxParity.Items.AddRange(new object[] {
            "Even",
            "Odd",
            "None"});
			this.comboBoxParity.Location = new System.Drawing.Point(241, 32);
			this.comboBoxParity.Name = "comboBoxParity";
			this.comboBoxParity.Size = new System.Drawing.Size(73, 21);
			this.comboBoxParity.TabIndex = 6;
			// 
			// labelMode
			// 
			this.labelMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelMode.AutoSize = true;
			this.labelMode.Location = new System.Drawing.Point(183, 63);
			this.labelMode.Name = "labelMode";
			this.labelMode.Size = new System.Drawing.Size(37, 13);
			this.labelMode.TabIndex = 7;
			this.labelMode.Text = "Mode:";
			// 
			// comboBoxMode
			// 
			this.comboBoxMode.FormattingEnabled = true;
			this.comboBoxMode.Items.AddRange(new object[] {
            "RTU",
            "ASCII"});
			this.comboBoxMode.Location = new System.Drawing.Point(241, 59);
			this.comboBoxMode.Name = "comboBoxMode";
			this.comboBoxMode.Size = new System.Drawing.Size(73, 21);
			this.comboBoxMode.TabIndex = 8;
			// 
			// radioButtonOpen
			// 
			this.radioButtonOpen.AutoSize = true;
			this.radioButtonOpen.Location = new System.Drawing.Point(320, 3);
			this.radioButtonOpen.Name = "radioButtonOpen";
			this.radioButtonOpen.Size = new System.Drawing.Size(51, 17);
			this.radioButtonOpen.TabIndex = 12;
			this.radioButtonOpen.Text = "Open";
			this.radioButtonOpen.UseVisualStyleBackColor = true;
			this.radioButtonOpen.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// radioButtonClosed
			// 
			this.radioButtonClosed.AutoSize = true;
			this.radioButtonClosed.Checked = true;
			this.radioButtonClosed.Location = new System.Drawing.Point(320, 32);
			this.radioButtonClosed.Name = "radioButtonClosed";
			this.radioButtonClosed.Size = new System.Drawing.Size(57, 17);
			this.radioButtonClosed.TabIndex = 13;
			this.radioButtonClosed.TabStop = true;
			this.radioButtonClosed.Text = "Closed";
			this.radioButtonClosed.UseVisualStyleBackColor = true;
			this.radioButtonClosed.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
			// 
			// tabControlData
			// 
			this.tableLayoutPanelCommunication.SetColumnSpan(this.tabControlData, 2);
			this.tabControlData.Controls.Add(this.tabPageCoils);
			this.tabControlData.Controls.Add(this.tabPageInputRegisters);
			this.tabControlData.Controls.Add(this.tabPageHoldingRegisters);
			this.tabControlData.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControlData.Location = new System.Drawing.Point(3, 29);
			this.tabControlData.Name = "tabControlData";
			this.tabControlData.SelectedIndex = 0;
			this.tabControlData.Size = new System.Drawing.Size(375, 165);
			this.tabControlData.TabIndex = 2;
			// 
			// tabPageCoils
			// 
			this.tabPageCoils.Controls.Add(this.tableLayoutPanelCoils);
			this.tabPageCoils.Location = new System.Drawing.Point(4, 22);
			this.tabPageCoils.Name = "tabPageCoils";
			this.tabPageCoils.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCoils.Size = new System.Drawing.Size(367, 139);
			this.tabPageCoils.TabIndex = 2;
			this.tabPageCoils.Text = "Coils";
			this.tabPageCoils.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanelCoils
			// 
			this.tableLayoutPanelCoils.AutoSize = true;
			this.tableLayoutPanelCoils.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelCoils.ColumnCount = 2;
			this.tableLayoutPanelCoils.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelCoils.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelCoils.Controls.Add(this.labelCoilValue, 0, 1);
			this.tableLayoutPanelCoils.Controls.Add(this.labelCoilAddress, 0, 0);
			this.tableLayoutPanelCoils.Controls.Add(this.textBoxCoilAddr, 1, 0);
			this.tableLayoutPanelCoils.Controls.Add(this.buttonCoilRead, 0, 3);
			this.tableLayoutPanelCoils.Controls.Add(this.buttonCoilWrite, 1, 3);
			this.tableLayoutPanelCoils.Controls.Add(this.radioButtonCoilTrue, 1, 1);
			this.tableLayoutPanelCoils.Controls.Add(this.radioButtonCoilFalse, 1, 2);
			this.tableLayoutPanelCoils.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelCoils.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelCoils.Name = "tableLayoutPanelCoils";
			this.tableLayoutPanelCoils.RowCount = 4;
			this.tableLayoutPanelCoils.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCoils.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCoils.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCoils.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCoils.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanelCoils.Size = new System.Drawing.Size(361, 133);
			this.tableLayoutPanelCoils.TabIndex = 7;
			// 
			// labelCoilValue
			// 
			this.labelCoilValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelCoilValue.AutoSize = true;
			this.labelCoilValue.Location = new System.Drawing.Point(3, 31);
			this.labelCoilValue.Name = "labelCoilValue";
			this.labelCoilValue.Size = new System.Drawing.Size(37, 13);
			this.labelCoilValue.TabIndex = 6;
			this.labelCoilValue.Text = "Value:";
			// 
			// labelCoilAddress
			// 
			this.labelCoilAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelCoilAddress.AutoSize = true;
			this.labelCoilAddress.Location = new System.Drawing.Point(3, 6);
			this.labelCoilAddress.Name = "labelCoilAddress";
			this.labelCoilAddress.Size = new System.Drawing.Size(48, 13);
			this.labelCoilAddress.TabIndex = 0;
			this.labelCoilAddress.Text = "Address:";
			// 
			// textBoxCoilAddr
			// 
			this.textBoxCoilAddr.Location = new System.Drawing.Point(84, 3);
			this.textBoxCoilAddr.Name = "textBoxCoilAddr";
			this.textBoxCoilAddr.Size = new System.Drawing.Size(100, 20);
			this.textBoxCoilAddr.TabIndex = 1;
			// 
			// buttonCoilRead
			// 
			this.buttonCoilRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCoilRead.Enabled = false;
			this.buttonCoilRead.Location = new System.Drawing.Point(3, 107);
			this.buttonCoilRead.Name = "buttonCoilRead";
			this.buttonCoilRead.Size = new System.Drawing.Size(75, 23);
			this.buttonCoilRead.TabIndex = 4;
			this.buttonCoilRead.Text = "Read";
			this.buttonCoilRead.UseVisualStyleBackColor = true;
			this.buttonCoilRead.Click += new System.EventHandler(this.ButtonCoilRead_Click);
			// 
			// buttonCoilWrite
			// 
			this.buttonCoilWrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonCoilWrite.Enabled = false;
			this.buttonCoilWrite.Location = new System.Drawing.Point(84, 107);
			this.buttonCoilWrite.Name = "buttonCoilWrite";
			this.buttonCoilWrite.Size = new System.Drawing.Size(75, 23);
			this.buttonCoilWrite.TabIndex = 5;
			this.buttonCoilWrite.Text = "Write";
			this.buttonCoilWrite.UseVisualStyleBackColor = true;
			this.buttonCoilWrite.Click += new System.EventHandler(this.ButtonCoilWrite_Click);
			// 
			// radioButtonCoilTrue
			// 
			this.radioButtonCoilTrue.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.radioButtonCoilTrue.AutoSize = true;
			this.radioButtonCoilTrue.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButtonCoilTrue.Checked = true;
			this.radioButtonCoilTrue.Location = new System.Drawing.Point(84, 29);
			this.radioButtonCoilTrue.Name = "radioButtonCoilTrue";
			this.radioButtonCoilTrue.Size = new System.Drawing.Size(47, 17);
			this.radioButtonCoilTrue.TabIndex = 2;
			this.radioButtonCoilTrue.TabStop = true;
			this.radioButtonCoilTrue.Text = "True";
			this.radioButtonCoilTrue.UseVisualStyleBackColor = true;
			// 
			// radioButtonCoilFalse
			// 
			this.radioButtonCoilFalse.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.radioButtonCoilFalse.AutoSize = true;
			this.radioButtonCoilFalse.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioButtonCoilFalse.Location = new System.Drawing.Point(84, 52);
			this.radioButtonCoilFalse.Name = "radioButtonCoilFalse";
			this.radioButtonCoilFalse.Size = new System.Drawing.Size(50, 17);
			this.radioButtonCoilFalse.TabIndex = 3;
			this.radioButtonCoilFalse.Text = "False";
			this.radioButtonCoilFalse.UseVisualStyleBackColor = true;
			// 
			// tabPageInputRegisters
			// 
			this.tabPageInputRegisters.Controls.Add(this.tableLayoutPanelInputRegisters);
			this.tabPageInputRegisters.Location = new System.Drawing.Point(4, 22);
			this.tabPageInputRegisters.Name = "tabPageInputRegisters";
			this.tabPageInputRegisters.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageInputRegisters.Size = new System.Drawing.Size(367, 139);
			this.tabPageInputRegisters.TabIndex = 0;
			this.tabPageInputRegisters.Text = "Input Registers";
			this.tabPageInputRegisters.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanelInputRegisters
			// 
			this.tableLayoutPanelInputRegisters.AutoSize = true;
			this.tableLayoutPanelInputRegisters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelInputRegisters.ColumnCount = 2;
			this.tableLayoutPanelInputRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelInputRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelInputRegisters.Controls.Add(this.buttonInputRegisterPoll, 1, 4);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.labelInputRegisterAddress, 0, 0);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.buttonInputRegisterRead, 0, 4);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.textBoxInputRegisterStringLength, 1, 2);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.textBoxInputRegisterAddress, 1, 0);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.labelInputRegisterStringLength, 0, 2);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.comboBoxInputRegisterFormat, 1, 1);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.labelInputRegisterDataFormat, 0, 1);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.labelInputRegisterData, 0, 3);
			this.tableLayoutPanelInputRegisters.Controls.Add(this.textBoxInputRegisterData, 1, 3);
			this.tableLayoutPanelInputRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelInputRegisters.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelInputRegisters.Name = "tableLayoutPanelInputRegisters";
			this.tableLayoutPanelInputRegisters.RowCount = 5;
			this.tableLayoutPanelInputRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelInputRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelInputRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelInputRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelInputRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelInputRegisters.Size = new System.Drawing.Size(361, 133);
			this.tableLayoutPanelInputRegisters.TabIndex = 7;
			// 
			// buttonInputRegisterPoll
			// 
			this.buttonInputRegisterPoll.Enabled = false;
			this.buttonInputRegisterPoll.Location = new System.Drawing.Point(84, 108);
			this.buttonInputRegisterPoll.Name = "buttonInputRegisterPoll";
			this.buttonInputRegisterPoll.Size = new System.Drawing.Size(75, 23);
			this.buttonInputRegisterPoll.TabIndex = 9;
			this.buttonInputRegisterPoll.Text = "Poll";
			this.buttonInputRegisterPoll.UseVisualStyleBackColor = true;
			this.buttonInputRegisterPoll.Click += new System.EventHandler(this.ButtonInputPoll_Click);
			// 
			// labelInputRegisterAddress
			// 
			this.labelInputRegisterAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelInputRegisterAddress.AutoSize = true;
			this.labelInputRegisterAddress.Location = new System.Drawing.Point(3, 6);
			this.labelInputRegisterAddress.Name = "labelInputRegisterAddress";
			this.labelInputRegisterAddress.Size = new System.Drawing.Size(48, 13);
			this.labelInputRegisterAddress.TabIndex = 0;
			this.labelInputRegisterAddress.Text = "Address:";
			// 
			// buttonInputRegisterRead
			// 
			this.buttonInputRegisterRead.Enabled = false;
			this.buttonInputRegisterRead.Location = new System.Drawing.Point(3, 108);
			this.buttonInputRegisterRead.Name = "buttonInputRegisterRead";
			this.buttonInputRegisterRead.Size = new System.Drawing.Size(75, 23);
			this.buttonInputRegisterRead.TabIndex = 6;
			this.buttonInputRegisterRead.Text = "Read";
			this.buttonInputRegisterRead.UseVisualStyleBackColor = true;
			this.buttonInputRegisterRead.Click += new System.EventHandler(this.ButtonInputRegisterRead_Click);
			// 
			// textBoxInputRegisterStringLength
			// 
			this.textBoxInputRegisterStringLength.Enabled = false;
			this.textBoxInputRegisterStringLength.Location = new System.Drawing.Point(84, 56);
			this.textBoxInputRegisterStringLength.Name = "textBoxInputRegisterStringLength";
			this.textBoxInputRegisterStringLength.Size = new System.Drawing.Size(101, 20);
			this.textBoxInputRegisterStringLength.TabIndex = 8;
			// 
			// textBoxInputRegisterAddress
			// 
			this.textBoxInputRegisterAddress.Location = new System.Drawing.Point(84, 3);
			this.textBoxInputRegisterAddress.Name = "textBoxInputRegisterAddress";
			this.textBoxInputRegisterAddress.Size = new System.Drawing.Size(100, 20);
			this.textBoxInputRegisterAddress.TabIndex = 1;
			// 
			// labelInputRegisterStringLength
			// 
			this.labelInputRegisterStringLength.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelInputRegisterStringLength.AutoSize = true;
			this.labelInputRegisterStringLength.Location = new System.Drawing.Point(3, 59);
			this.labelInputRegisterStringLength.Name = "labelInputRegisterStringLength";
			this.labelInputRegisterStringLength.Size = new System.Drawing.Size(73, 13);
			this.labelInputRegisterStringLength.TabIndex = 7;
			this.labelInputRegisterStringLength.Text = "String Length:";
			// 
			// comboBoxInputRegisterFormat
			// 
			this.comboBoxInputRegisterFormat.FormattingEnabled = true;
			this.comboBoxInputRegisterFormat.Items.AddRange(new object[] {
            "Float",
            "Int32",
            "UInt32",
            "Int16",
            "UInt16",
            "String"});
			this.comboBoxInputRegisterFormat.Location = new System.Drawing.Point(84, 29);
			this.comboBoxInputRegisterFormat.Name = "comboBoxInputRegisterFormat";
			this.comboBoxInputRegisterFormat.Size = new System.Drawing.Size(121, 21);
			this.comboBoxInputRegisterFormat.TabIndex = 2;
			this.comboBoxInputRegisterFormat.SelectedIndexChanged += new System.EventHandler(this.ComboBoxFormat_SelectedIndexChanged);
			// 
			// labelInputRegisterDataFormat
			// 
			this.labelInputRegisterDataFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelInputRegisterDataFormat.AutoSize = true;
			this.labelInputRegisterDataFormat.Location = new System.Drawing.Point(3, 33);
			this.labelInputRegisterDataFormat.Name = "labelInputRegisterDataFormat";
			this.labelInputRegisterDataFormat.Size = new System.Drawing.Size(68, 13);
			this.labelInputRegisterDataFormat.TabIndex = 3;
			this.labelInputRegisterDataFormat.Text = "Data Format:";
			// 
			// labelInputRegisterData
			// 
			this.labelInputRegisterData.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelInputRegisterData.AutoSize = true;
			this.labelInputRegisterData.Location = new System.Drawing.Point(3, 85);
			this.labelInputRegisterData.Name = "labelInputRegisterData";
			this.labelInputRegisterData.Size = new System.Drawing.Size(37, 13);
			this.labelInputRegisterData.TabIndex = 4;
			this.labelInputRegisterData.Text = "Value:";
			// 
			// textBoxInputRegisterData
			// 
			this.textBoxInputRegisterData.Location = new System.Drawing.Point(84, 82);
			this.textBoxInputRegisterData.Name = "textBoxInputRegisterData";
			this.textBoxInputRegisterData.Size = new System.Drawing.Size(141, 20);
			this.textBoxInputRegisterData.TabIndex = 5;
			// 
			// tabPageHoldingRegisters
			// 
			this.tabPageHoldingRegisters.Controls.Add(this.tableLayoutPanelHoldingRegisters);
			this.tabPageHoldingRegisters.Location = new System.Drawing.Point(4, 22);
			this.tabPageHoldingRegisters.Name = "tabPageHoldingRegisters";
			this.tabPageHoldingRegisters.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageHoldingRegisters.Size = new System.Drawing.Size(367, 139);
			this.tabPageHoldingRegisters.TabIndex = 1;
			this.tabPageHoldingRegisters.Text = "Holding Registers";
			this.tabPageHoldingRegisters.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanelHoldingRegisters
			// 
			this.tableLayoutPanelHoldingRegisters.AutoSize = true;
			this.tableLayoutPanelHoldingRegisters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelHoldingRegisters.ColumnCount = 2;
			this.tableLayoutPanelHoldingRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelHoldingRegisters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.buttonHoldingRegisterWrite, 1, 4);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.textBoxHoldingRegisterStringLength, 1, 2);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.buttonHoldingRegisterRead, 0, 4);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.labelHoldingRegisterAddress, 0, 0);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.textBoxHoldingRegisterData, 1, 3);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.labelHoldingRegisterStringLength, 0, 2);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.labelHoldingRegisterData, 0, 3);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.textBoxHoldingRegisterAddress, 1, 0);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.labelHoldingRegisterFormat, 0, 1);
			this.tableLayoutPanelHoldingRegisters.Controls.Add(this.comboBoxHoldingRegisterFormat, 1, 1);
			this.tableLayoutPanelHoldingRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelHoldingRegisters.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanelHoldingRegisters.Name = "tableLayoutPanelHoldingRegisters";
			this.tableLayoutPanelHoldingRegisters.RowCount = 5;
			this.tableLayoutPanelHoldingRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelHoldingRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelHoldingRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelHoldingRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelHoldingRegisters.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelHoldingRegisters.Size = new System.Drawing.Size(361, 133);
			this.tableLayoutPanelHoldingRegisters.TabIndex = 7;
			// 
			// buttonHoldingRegisterWrite
			// 
			this.buttonHoldingRegisterWrite.Enabled = false;
			this.buttonHoldingRegisterWrite.Location = new System.Drawing.Point(84, 108);
			this.buttonHoldingRegisterWrite.Name = "buttonHoldingRegisterWrite";
			this.buttonHoldingRegisterWrite.Size = new System.Drawing.Size(75, 23);
			this.buttonHoldingRegisterWrite.TabIndex = 7;
			this.buttonHoldingRegisterWrite.Text = "Write";
			this.buttonHoldingRegisterWrite.UseVisualStyleBackColor = true;
			this.buttonHoldingRegisterWrite.Click += new System.EventHandler(this.ButtonHoldingRegisterWrite_Click);
			// 
			// textBoxHoldingRegisterStringLength
			// 
			this.textBoxHoldingRegisterStringLength.Enabled = false;
			this.textBoxHoldingRegisterStringLength.Location = new System.Drawing.Point(84, 56);
			this.textBoxHoldingRegisterStringLength.Name = "textBoxHoldingRegisterStringLength";
			this.textBoxHoldingRegisterStringLength.Size = new System.Drawing.Size(100, 20);
			this.textBoxHoldingRegisterStringLength.TabIndex = 9;
			// 
			// buttonHoldingRegisterRead
			// 
			this.buttonHoldingRegisterRead.Enabled = false;
			this.buttonHoldingRegisterRead.Location = new System.Drawing.Point(3, 108);
			this.buttonHoldingRegisterRead.Name = "buttonHoldingRegisterRead";
			this.buttonHoldingRegisterRead.Size = new System.Drawing.Size(75, 23);
			this.buttonHoldingRegisterRead.TabIndex = 6;
			this.buttonHoldingRegisterRead.Text = "Read";
			this.buttonHoldingRegisterRead.UseVisualStyleBackColor = true;
			this.buttonHoldingRegisterRead.Click += new System.EventHandler(this.ButtonHoldingRegisterRead_Click);
			// 
			// labelHoldingRegisterAddress
			// 
			this.labelHoldingRegisterAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelHoldingRegisterAddress.AutoSize = true;
			this.labelHoldingRegisterAddress.Location = new System.Drawing.Point(3, 6);
			this.labelHoldingRegisterAddress.Name = "labelHoldingRegisterAddress";
			this.labelHoldingRegisterAddress.Size = new System.Drawing.Size(48, 13);
			this.labelHoldingRegisterAddress.TabIndex = 0;
			this.labelHoldingRegisterAddress.Text = "Address:";
			// 
			// textBoxHoldingRegisterData
			// 
			this.textBoxHoldingRegisterData.Location = new System.Drawing.Point(84, 82);
			this.textBoxHoldingRegisterData.Name = "textBoxHoldingRegisterData";
			this.textBoxHoldingRegisterData.Size = new System.Drawing.Size(141, 20);
			this.textBoxHoldingRegisterData.TabIndex = 5;
			// 
			// labelHoldingRegisterStringLength
			// 
			this.labelHoldingRegisterStringLength.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelHoldingRegisterStringLength.AutoSize = true;
			this.labelHoldingRegisterStringLength.Location = new System.Drawing.Point(3, 59);
			this.labelHoldingRegisterStringLength.Name = "labelHoldingRegisterStringLength";
			this.labelHoldingRegisterStringLength.Size = new System.Drawing.Size(73, 13);
			this.labelHoldingRegisterStringLength.TabIndex = 8;
			this.labelHoldingRegisterStringLength.Text = "String Length:";
			// 
			// labelHoldingRegisterData
			// 
			this.labelHoldingRegisterData.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelHoldingRegisterData.AutoSize = true;
			this.labelHoldingRegisterData.Location = new System.Drawing.Point(3, 85);
			this.labelHoldingRegisterData.Name = "labelHoldingRegisterData";
			this.labelHoldingRegisterData.Size = new System.Drawing.Size(37, 13);
			this.labelHoldingRegisterData.TabIndex = 4;
			this.labelHoldingRegisterData.Text = "Value:";
			// 
			// textBoxHoldingRegisterAddress
			// 
			this.textBoxHoldingRegisterAddress.Location = new System.Drawing.Point(84, 3);
			this.textBoxHoldingRegisterAddress.Name = "textBoxHoldingRegisterAddress";
			this.textBoxHoldingRegisterAddress.Size = new System.Drawing.Size(100, 20);
			this.textBoxHoldingRegisterAddress.TabIndex = 1;
			// 
			// labelHoldingRegisterFormat
			// 
			this.labelHoldingRegisterFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelHoldingRegisterFormat.AutoSize = true;
			this.labelHoldingRegisterFormat.Location = new System.Drawing.Point(3, 33);
			this.labelHoldingRegisterFormat.Name = "labelHoldingRegisterFormat";
			this.labelHoldingRegisterFormat.Size = new System.Drawing.Size(68, 13);
			this.labelHoldingRegisterFormat.TabIndex = 2;
			this.labelHoldingRegisterFormat.Text = "Data Format:";
			// 
			// comboBoxHoldingRegisterFormat
			// 
			this.comboBoxHoldingRegisterFormat.FormattingEnabled = true;
			this.comboBoxHoldingRegisterFormat.Items.AddRange(new object[] {
            "Float",
            "Int32",
            "UInt32",
            "Int16",
            "UInt16",
            "String"});
			this.comboBoxHoldingRegisterFormat.Location = new System.Drawing.Point(84, 29);
			this.comboBoxHoldingRegisterFormat.Name = "comboBoxHoldingRegisterFormat";
			this.comboBoxHoldingRegisterFormat.Size = new System.Drawing.Size(121, 21);
			this.comboBoxHoldingRegisterFormat.TabIndex = 3;
			this.comboBoxHoldingRegisterFormat.SelectedIndexChanged += new System.EventHandler(this.ComboBoxFormat_SelectedIndexChanged);
			// 
			// labelSlaveAddress
			// 
			this.labelSlaveAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.labelSlaveAddress.AutoSize = true;
			this.labelSlaveAddress.Location = new System.Drawing.Point(3, 6);
			this.labelSlaveAddress.Name = "labelSlaveAddress";
			this.labelSlaveAddress.Size = new System.Drawing.Size(95, 13);
			this.labelSlaveAddress.TabIndex = 3;
			this.labelSlaveAddress.Text = "Slave Address:  0x";
			// 
			// textBoxSlaveAddr
			// 
			this.textBoxSlaveAddr.Location = new System.Drawing.Point(104, 3);
			this.textBoxSlaveAddr.Name = "textBoxSlaveAddr";
			this.textBoxSlaveAddr.Size = new System.Drawing.Size(69, 20);
			this.textBoxSlaveAddr.TabIndex = 4;
			this.textBoxSlaveAddr.Leave += new System.EventHandler(this.TextBoxSlaveAddr_Leave);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelMainStatus,
            this.toolStripStatusLabel1});
			this.statusStrip1.Location = new System.Drawing.Point(0, 364);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(387, 22);
			this.statusStrip1.TabIndex = 5;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// labelMainStatus
			// 
			this.labelMainStatus.Name = "labelMainStatus";
			this.labelMainStatus.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
			this.toolStripStatusLabel1.Text = "Ready";
			// 
			// groupBoxCommunication
			// 
			this.groupBoxCommunication.AutoSize = true;
			this.groupBoxCommunication.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBoxCommunication.Controls.Add(this.tableLayoutPanelCommunication);
			this.groupBoxCommunication.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBoxCommunication.Location = new System.Drawing.Point(0, 126);
			this.groupBoxCommunication.Name = "groupBoxCommunication";
			this.groupBoxCommunication.Size = new System.Drawing.Size(387, 216);
			this.groupBoxCommunication.TabIndex = 6;
			this.groupBoxCommunication.TabStop = false;
			this.groupBoxCommunication.Text = "Communication";
			// 
			// tableLayoutPanelCommunication
			// 
			this.tableLayoutPanelCommunication.AutoSize = true;
			this.tableLayoutPanelCommunication.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanelCommunication.ColumnCount = 2;
			this.tableLayoutPanelCommunication.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelCommunication.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanelCommunication.Controls.Add(this.textBoxSlaveAddr, 1, 0);
			this.tableLayoutPanelCommunication.Controls.Add(this.labelSlaveAddress, 0, 0);
			this.tableLayoutPanelCommunication.Controls.Add(this.tabControlData, 0, 1);
			this.tableLayoutPanelCommunication.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanelCommunication.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanelCommunication.Name = "tableLayoutPanelCommunication";
			this.tableLayoutPanelCommunication.RowCount = 2;
			this.tableLayoutPanelCommunication.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCommunication.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanelCommunication.Size = new System.Drawing.Size(381, 197);
			this.tableLayoutPanelCommunication.TabIndex = 0;
			// 
			// Modbusgui
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(387, 386);
			this.Controls.Add(this.groupBoxCommunication);
			this.Controls.Add(this.groupBoxSerialPort);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.mainMenuStrip);
			this.MainMenuStrip = this.mainMenuStrip;
			this.MaximizeBox = false;
			this.Name = "Modbusgui";
			this.Text = "Modbus Interface";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.groupBoxSerialPort.ResumeLayout(false);
			this.groupBoxSerialPort.PerformLayout();
			this.tableLayoutPanelSerialPort.ResumeLayout(false);
			this.tableLayoutPanelSerialPort.PerformLayout();
			this.tabControlData.ResumeLayout(false);
			this.tabPageCoils.ResumeLayout(false);
			this.tabPageCoils.PerformLayout();
			this.tableLayoutPanelCoils.ResumeLayout(false);
			this.tableLayoutPanelCoils.PerformLayout();
			this.tabPageInputRegisters.ResumeLayout(false);
			this.tabPageInputRegisters.PerformLayout();
			this.tableLayoutPanelInputRegisters.ResumeLayout(false);
			this.tableLayoutPanelInputRegisters.PerformLayout();
			this.tabPageHoldingRegisters.ResumeLayout(false);
			this.tabPageHoldingRegisters.PerformLayout();
			this.tableLayoutPanelHoldingRegisters.ResumeLayout(false);
			this.tableLayoutPanelHoldingRegisters.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.groupBoxCommunication.ResumeLayout(false);
			this.groupBoxCommunication.PerformLayout();
			this.tableLayoutPanelCommunication.ResumeLayout(false);
			this.tableLayoutPanelCommunication.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxSerialPort;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.Label labelParity;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label labelBaudRate;
        private System.Windows.Forms.Button buttonPortRefresh;
        private System.Windows.Forms.ComboBox comboBoxSerialPort;
        private System.Windows.Forms.Label labelComPort;
        private System.Windows.Forms.ComboBox comboBoxMode;
        private System.Windows.Forms.Label labelMode;
        private System.Windows.Forms.TabControl tabControlData;
        private System.Windows.Forms.TabPage tabPageInputRegisters;
        private System.Windows.Forms.Button buttonInputRegisterRead;
        private System.Windows.Forms.TextBox textBoxInputRegisterData;
        private System.Windows.Forms.Label labelInputRegisterData;
        private System.Windows.Forms.Label labelInputRegisterDataFormat;
        private System.Windows.Forms.ComboBox comboBoxInputRegisterFormat;
        private System.Windows.Forms.TextBox textBoxInputRegisterAddress;
        private System.Windows.Forms.Label labelInputRegisterAddress;
        private System.Windows.Forms.TabPage tabPageHoldingRegisters;
        private System.Windows.Forms.Button buttonHoldingRegisterWrite;
        private System.Windows.Forms.Button buttonHoldingRegisterRead;
        private System.Windows.Forms.TextBox textBoxHoldingRegisterData;
        private System.Windows.Forms.Label labelHoldingRegisterData;
        private System.Windows.Forms.ComboBox comboBoxHoldingRegisterFormat;
        private System.Windows.Forms.Label labelHoldingRegisterFormat;
        private System.Windows.Forms.TextBox textBoxHoldingRegisterAddress;
        private System.Windows.Forms.Label labelHoldingRegisterAddress;
        private System.Windows.Forms.Label labelSlaveAddress;
        private System.Windows.Forms.TextBox textBoxSlaveAddr;
        private System.Windows.Forms.TabPage tabPageCoils;
        private System.Windows.Forms.Button buttonCoilWrite;
        private System.Windows.Forms.Button buttonCoilRead;
        private System.Windows.Forms.RadioButton radioButtonCoilFalse;
        private System.Windows.Forms.RadioButton radioButtonCoilTrue;
        private System.Windows.Forms.TextBox textBoxCoilAddr;
        private System.Windows.Forms.Label labelCoilAddress;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelMainStatus;
        private System.Windows.Forms.TextBox textBoxInputRegisterStringLength;
        private System.Windows.Forms.Label labelInputRegisterStringLength;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
		private System.Windows.Forms.Label labelStopBits;
		private System.Windows.Forms.Button buttonInputRegisterPoll;
        private System.Windows.Forms.TextBox textBoxHoldingRegisterStringLength;
		private System.Windows.Forms.Label labelHoldingRegisterStringLength;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSerialPort;
		private System.Windows.Forms.GroupBox groupBoxCommunication;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelInputRegisters;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHoldingRegisters;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCoils;
		private System.Windows.Forms.Label labelCoilValue;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelCommunication;
		private System.Windows.Forms.RadioButton radioButtonOpen;
		private System.Windows.Forms.RadioButton radioButtonClosed;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
	}
}

