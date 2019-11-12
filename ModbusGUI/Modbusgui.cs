using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Modbus;

namespace ModbusGUI
{
	public partial class Modbusgui : Form
	{
		// Data types that can be transmitted by MODBUS
		enum DataType
		{
			None,
			Float,
			Int32,
			UInt32,
			Int16,
			UInt16,
			String,
		}

		// Types of registers that MODBUS can use
		enum MBRegisterType
		{
			Input,
			Holding,
		}

		// Types of MODBUS transactions
		enum MBActionType
		{
			Read,
			Write,
			WriteSingle,
		}

		ModbusMasterSerial mbMaster = null;
		Thread pollThread = null;

		/// <summary>
		/// Runs when the GUI loads.
		/// </summary>
		public Modbusgui()
		{
			InitializeComponent();

			// Set some default settings.
			this.modeCb.SelectedIndex = 0;
			this.parityCb.SelectedIndex = 0;
			this.baudCb.SelectedIndex = 0;
			this.stopBitsCb.SelectedIndex = 0;

			// Retrieve System Serial port names.
			this.comPortCb.Items.AddRange(SerialPort.GetPortNames());

			// Set the control values.
			comPortCb.Text = Properties.Settings.Default.Port;
			baudCb.Text = Properties.Settings.Default.BaudRate.ToString();
			parityCb.Text = Properties.Settings.Default.Parity;
			stopBitsCb.Text = Properties.Settings.Default.StopBits.ToString();
			modeCb.Text = Properties.Settings.Default.Mode;
			slaveAddrTb.Text = Properties.Settings.Default.Address.ToString();
		}

		/// <summary>
		/// Runs when the GUI is closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Dispose of the MODBUS class.
			if (mbMaster != null)
			{
				this.mbMaster.Disconnect();
			}

			// Save settings used by the program.
			Properties.Settings.Default.Port = comPortCb.Text;
			Properties.Settings.Default.BaudRate = Convert.ToUInt16(baudCb.Text);
			Properties.Settings.Default.Parity = parityCb.Text;
			Properties.Settings.Default.StopBits = Convert.ToUInt16(stopBitsCb.Text);
			Properties.Settings.Default.Mode = modeCb.Text;
			Properties.Settings.Default.Address = Convert.ToUInt16(slaveAddrTb.Text);

			// Store the current values of the application settings properties.
			// If this call is omitted, then the settings will not be saved after the program quits.
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Runs when the user clicks File -> Exit in the Menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Formats the contents of the address textbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void slaveAddrTb_Leave(object sender, EventArgs e)
		{
			string contents = this.slaveAddrTb.Text;

			// Remmove leading and trailing whitespace from text.
			contents = contents.Trim();

			// Remove "0x" from text if it's there.
			if (contents.StartsWith("0x"))
			{
				contents = contents.Substring(2);
				this.slaveAddrTb.Text = contents;
			}
		}

		private void FormatCb_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox formatCb = (ComboBox)sender;
			
			// Find the corresponding "String Length" textbox.
			TextBox stringLengthTextBox = this.inputRegStringLenTb;
			if (formatCb.Equals(this.holdingRegFormatCb))
			{
				stringLengthTextBox = this.holdingRegStringLenTb;
			}

			// Check for empty string.
			if (formatCb.SelectedItem == null)
			{
				// Do nothing.
			}

			// If the selected format is "String"...
			else if (formatCb.SelectedItem.ToString() == "String")
			{
				// Enable the "String Length" textbox.
				stringLengthTextBox.Enabled = true;
			}

			// If any other format is selected...
			else
			{
				// Disable the "String Length" textbox.
				stringLengthTextBox.Enabled = false;
			}


		}

		private DataType GetDataFormat(ComboBox cb, out UInt16 numRegs, TextBox tb = null)
		{
			DataType type = DataType.None;
			numRegs = 1;

			// Fetch the selection from the text box.
			object format_str = cb.SelectedItem;

			// Check for empty string.
			if (format_str == null)
			{
				this.ShowError("No format selected");
				return type;
			}

			// Check for the various allowed selections.
			if (format_str.ToString() == "Float")
			{
				type = DataType.Float;
				numRegs = 2;
			}
			else if (format_str.ToString() == "Int32")
			{
				type = DataType.Int32;
				numRegs = 2;
			}
			else if (format_str.ToString() == "UInt32")
			{
				type = DataType.UInt32;
				numRegs = 2;
			}
			else if (format_str.ToString() == "Int16")
			{
				type = DataType.Int16;
			}
			else if (format_str.ToString() == "String")
			{
				type = DataType.String;
				try
				{
					float length = (float)Math.Round(float.Parse(tb.Text) / 2.0f);
					numRegs = Convert.ToUInt16(length);
				}
				catch (Exception exp)
				{
					this.ShowError("Please enter a valid string length in bytes: " + exp.ToString());
					type = DataType.None;
				}
			}
			else
			{
				type = DataType.UInt16;
			}

			return type;
		}

		/// <summary>
		/// Runs when the user clicks the refresh Button in the GUI.
		/// Refreshes the COM ports available to the user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void portRefreshBtn_Click(object sender, EventArgs e)
		{
			this.comPortCb.Items.Clear();
			this.comPortCb.SelectedIndex = -1;
			this.comPortCb.Text = "";
			this.comPortCb.Items.AddRange(SerialPort.GetPortNames());
		}

		/// <summary>
		/// Runs when the user clicks the Open button in the GUI.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void portOpenBtn_Click(object sender, EventArgs e)
		{
			// If the MODBUS object exists (i.e. the port is open)...
			if (mbMaster != null)
			{
				// Discard the MODBUS object (close the port).
				this.mbMaster.Disconnect();
				this.mbMaster = null;

				// Enable the MODBUS configuration controls.
				this.comPortCb.Enabled  = true;
				this.baudCb.Enabled     = true;
				this.parityCb.Enabled   = true;
				this.modeCb.Enabled     = true;
				this.stopBitsCb.Enabled = true;

				// Disable the MODBUS read/write/poll buttons.
				this.inputRegReadBtn.Enabled = false;
				this.inputPollBtn.Enabled = false;
				this.holdingRegReadBtn.Enabled = false;
				this.holdingRegWriteBtn.Enabled = false;
				this.coilReadBtn.Enabled = false;
				this.coilWriteBtn.Enabled = false;

				// Change the text on the Open/Close button.
				this.portOpenBtn.Text = "Open";

				// Update the GUI's status message.
				this.UpdateStatus("Port Closed");
			}

			// If the MODBUS port is closed...
			else
			{
				// Fetch the user's MODBUS configuration selections.
				object com_name = this.comPortCb.SelectedItem;
				object baud_name = this.baudCb.SelectedItem;
				object parity_name = this.parityCb.SelectedItem;
				object mb_mode = this.modeCb.SelectedItem;
				object stop_bits = this.stopBitsCb.SelectedItem;

				// Check for invalid configuration selections.
				if (com_name == null)
				{
					this.ShowError("Serial Port not selected.");
				}
				else if (baud_name == null)
				{
					this.ShowError("Baud Rate not selected.");
				}
				else if (parity_name == null)
				{
					this.ShowError("Parity type not selected");
				}
				else if (mb_mode == null)
				{
					this.ShowError("Modbus Mode not selected");
				}
				else if (stop_bits == null)
				{
					this.ShowError("Stop Bits not selected");
				}

				// If the settings seem valid...
				else
				{
					// Convert the baud rate to an integer.
					int baudRate = int.Parse(baud_name.ToString());

					// Convert stop bits string to comm port setting.
					StopBits stopbits = StopBits.One;
					if (stop_bits.ToString() == "2")
					{
						stopbits = StopBits.Two;
					}

					// Convert parity string to comm port setting.
					Parity parity = Parity.Even;
					if (parity_name.ToString() == "None")
					{
						parity = Parity.None;
					}
					else if (parity_name.ToString() == "Odd")
					{
						parity = Parity.Odd;
					}

					// Select the MODBUS packet size based on the selected mode.
					int datasize = 8;
					if (mb_mode.ToString() == "ASCII")
					{
						datasize = 7;
					}

					// Select the MODBUS mode.
					ModbusSerialType mode = ModbusSerialType.RTU;
					if (mb_mode.ToString() == "ASCII")
					{
						mode = ModbusSerialType.ASCII;
					}

					// Create and open serial port.
					try
					{
						this.mbMaster = new ModbusMasterSerial(mode, com_name.ToString(), baudRate, datasize, parity, stopbits, Handshake.None);
						this.mbMaster.Connect();

						// Disable the controls for MODBUS configuration settings.
						this.comPortCb.Enabled = false;
						this.baudCb.Enabled = false;
						this.parityCb.Enabled = false;
						this.modeCb.Enabled = false;
						this.stopBitsCb.Enabled = false;

						// Enable the MODBUS read/write/poll buttons.
						this.inputRegReadBtn.Enabled = true;
						this.inputPollBtn.Enabled = true;
						this.holdingRegReadBtn.Enabled = true;
						this.holdingRegWriteBtn.Enabled = true;
						this.coilReadBtn.Enabled = true;
						this.coilWriteBtn.Enabled = true;

						// Change the text on the Open/Close button.
						this.portOpenBtn.Text = "Close";

						// Update the GUI's status bar.
						this.UpdateStatus("Port Opened");
					}
					catch (Exception exp)
					{
						this.ShowError("Unable to open serial port\n\n" + exp.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Runs when the "Read" button for input registers is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void inputRegReadBtn_Click(object sender, EventArgs e)
		{
			String err_str = "";

			Byte slaveAddr = this.getSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			UInt16 regAddr;
			if (this.getRegisterAddr(this.inputRegAddrTb, out regAddr) == false)
			{
				return;
			}

			UInt16 numRegs;
			DataType dataType = this.GetDataFormat(this.inputRegFormatCb, out numRegs, this.inputRegStringLenTb);
			if (dataType == DataType.None)
			{
				return;
			}

			UInt16[] regValues;
			if (this.ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
										null, out regValues, out err_str) == false)
			{
				this.ShowError(err_str);
				return;
			}
			this.displayData(this.inputRegDataTb, dataType, regValues);

			this.UpdateStatus("Input Register Read Successfully");
		}

		private void holdingRegReadBtn_Click(object sender, EventArgs e)
		{
			String errorMsg = "";

			Byte slaveAddr = this.getSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			UInt16 regAddr;
			if (this.getRegisterAddr(this.holdingRegAddrTb, out regAddr) == false)
			{
				return;
			}

			UInt16 numRegs;
			DataType dataType = this.GetDataFormat(this.holdingRegFormatCb, out numRegs, this.holdingRegStringLenTb);
			if (dataType == DataType.None)
			{
				return;
			}

			UInt16[] regValues;
			if (this.ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Read, slaveAddr, regAddr, numRegs,
										null, out regValues, out errorMsg) == false)
			{
				this.ShowError(errorMsg);
				return;
			}
			this.displayData(this.holdingRegDataTb, dataType, regValues);

			this.UpdateStatus("Holding Register Read Successfully");
		}

		private void holdingRegWriteBtn_Click(object sender, EventArgs e)
		{
			String errorMsg = "";
			UInt16[] rsp;

			Byte slaveAddr = this.getSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			UInt16 regAddr;
			if (this.getRegisterAddr(this.holdingRegAddrTb, out regAddr) == false)
			{
				return;
			}

			UInt16 numRegs;
			DataType dataType = this.GetDataFormat(this.holdingRegFormatCb, out numRegs, this.holdingRegStringLenTb);
			if (dataType == DataType.None)
			{
				return;
			}

			UInt16[] regs = this.StringToRegisters(this.holdingRegDataTb.Text, dataType, numRegs);
			if (regs == null)
			{
				this.ShowError("Data to write does not match selected format");
				return;
			}

			if (this.ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Write, slaveAddr, regAddr, (UInt16)regs.Length,
										regs, out rsp, out errorMsg) == false)
			{
				this.ShowError(errorMsg);
				return;
			}

			this.UpdateStatus("Holding Register Written Successfully");
		}

		private void coilReadBtn_Click(object sender, EventArgs e)
		{
			String errorMsg = "";

			Byte slaveAddr = this.getSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			UInt16 regAddr;
			if (this.getRegisterAddr(this.coilAddrTb, out regAddr) == false)
			{
				return;
			}

			bool[] coils;
			if (this.ReadWriteCoils(MBActionType.Read, slaveAddr, regAddr, 1, null, out coils, out errorMsg) == false)
			{
				this.ShowError(errorMsg);
				return;
			}

			// Show the value of the coil as T/F
			if (coils.Length > 0)
			{
				if (coils[0] == true)
				{
					this.coilFalseRadioBtn.Checked = false;
					this.coilTrueRadioBtn.Checked = true;
				}
				else
				{
					this.coilTrueRadioBtn.Checked = false;
					this.coilFalseRadioBtn.Checked = true;
				}
			}

			this.UpdateStatus("Coil Read Successfully");
		}

		private void coilWriteBtn_Click(object sender, EventArgs e)
		{
			String errorMsg = "";
			bool[] write_coils = new bool[1];
			bool[] read_coils;

			Byte slaveAddr = this.getSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			UInt16 regAddr;
			if (this.getRegisterAddr(this.coilAddrTb, out regAddr) == false)
			{
				return;
			}

			write_coils[0] = this.coilTrueRadioBtn.Checked;
			if (this.ReadWriteCoils(MBActionType.Write, slaveAddr, regAddr, (UInt16)write_coils.Length,
									write_coils, out read_coils, out errorMsg) == false)
			{
				this.ShowError(errorMsg);
				return;
			}

			this.UpdateStatus("Coil Written Successfully");
		}

		private void inputPollBtn_Click(object sender, EventArgs e)
		{
			if (this.pollThread == null)
			{
				// Start a new thread to poll the requested register
				this.pollThread = new Thread(this.QueryInputRegister);
				this.pollThread.Start();
			}
			else
			{
				try
				{
					// Stop the running thread
					this.pollThread.Abort();
				}
				catch (Exception exp)
				{
					Debug.WriteLine(exp.ToString());
				}
				this.pollThread = null;
			}
		}
		
		private Byte getSlaveAddress()
		{
			Byte address = 0;
			bool status;

			try
			{
				status = Byte.TryParse(this.slaveAddrTb.Text, System.Globalization.NumberStyles.HexNumber,
										CultureInfo.InvariantCulture, out address);
				if (status == false)
				{
					this.ShowError("Invalid Slave Address");
				}
				else if ((address < 1) || (address > 247))
				{
					this.ShowError("Slave Address Must be between 1 and 247");
					address = 0;
				}
			}
			catch (Exception exp)
			{
				this.ShowError(exp.ToString());
			}

			return address;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool getRegisterAddr(TextBox tb, out UInt16 value)
		{
			value = 0;
			bool retval = false;

			try
			{
				retval = UInt16.TryParse(tb.Text, out value);
				if (retval == false)
				{
					this.ShowError("Invalid Register Address");
				}
			}
			catch (Exception exp)
			{
				this.ShowError(exp.ToString());
			}

			return retval;
		}

		private void displayData(TextBox tb, DataType type, UInt16[] registers)
		{
			List<byte> raw = new List<byte>(4);
			byte[] rawRegister;
			String dataString = "";
			
			switch (type)
			{
				case DataType.Float:
				case DataType.Int32:
				case DataType.UInt32:
					rawRegister = BitConverter.GetBytes(registers[0]);
					raw.Add(rawRegister[0]);
					raw.Add(rawRegister[1]);
					rawRegister = BitConverter.GetBytes(registers[1]);
					raw.Add(rawRegister[0]);
					raw.Add(rawRegister[1]);
					break;

				case DataType.Int16:
				case DataType.UInt16:
					rawRegister = BitConverter.GetBytes(registers[0]);
					raw.Add(rawRegister[0]);
					raw.Add(rawRegister[1]);
					break;
				case DataType.String:
					for (int i = 0; i < registers.Length; i++)
					{
						rawRegister = BitConverter.GetBytes(registers[i]);
						raw.Add(rawRegister[1]);
						raw.Add(rawRegister[0]);
					}
					for (int i = raw.Count-1; i >= 0; i--)
					{
						if ((raw[i] < 0x20) || (raw[i] > 126))
						{
							raw.RemoveAt(i);
						}
					}
					break;
			}

			switch (type)
			{
				case DataType.Float:
					dataString = BitConverter.ToSingle(raw.ToArray(), 0).ToString();
					break;
				case DataType.Int32:
					dataString = BitConverter.ToInt32(raw.ToArray(), 0).ToString();
					break;
				case DataType.UInt32:
					dataString = BitConverter.ToUInt32(raw.ToArray(), 0).ToString();
					break;
				case DataType.Int16:
					dataString = BitConverter.ToInt16(raw.ToArray(), 0).ToString();
					break;
				case DataType.UInt16:
					dataString = BitConverter.ToUInt16(raw.ToArray(), 0).ToString();
					break;
				case DataType.String:
					try
					{
						dataString = ASCIIEncoding.ASCII.GetString(raw.ToArray());
					}
					catch (Exception)
					{
						dataString = "<INVALID STRING>";
					}
					break;
			}
			tb.BeginInvoke((MethodInvoker)(() => tb.Text = dataString));
		}

		private UInt16[] StringToRegisters(String str, DataType type, UInt16 numRegs)
		{
			UInt16[] regs = new UInt16[numRegs];
			bool retval;

			switch (type)
			{
				case DataType.Float:
					{
						float value;
						retval = float.TryParse(str, out value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
						regs[1] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
					}
					break;
				case DataType.Int32:
					{
						Int32 value;
						retval = Int32.TryParse(str, out value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
						regs[1] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
					}
					break;
				case DataType.UInt32:
					{
						UInt32 value;
						retval = UInt32.TryParse(str, out value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 0);
						regs[1] = BitConverter.ToUInt16(BitConverter.GetBytes(value), 2);
					}
					break;
				case DataType.Int16:
					{
						Int16 value;
						retval = Int16.TryParse(str, out value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = (UInt16)value;
					}
					break;
				case DataType.UInt16:
					{
						UInt16 value;
						retval = UInt16.TryParse(str, out value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = value;
					}
					break;
				case DataType.String:
					{
						byte[] strBytes = ASCIIEncoding.ASCII.GetBytes(str);
						int j = 0;
						// Copy the string data into the registers
						for (int i = 0; i < numRegs; i++)
						{
							if (j < strBytes.Length)
							{
								regs[i] = (UInt16)((UInt16)strBytes[j] << 8);
								j++;
							}
							else
								regs[i] = 0;

							if (j < strBytes.Length)
							{
								regs[i] |= strBytes[j];
								j++;
							}
						}
					}
					break;
			}

			return regs;
		}

		/// <summary>
		/// Prints a message to the status bar at the bottom of the GUI, and pops up
		/// a message window with the message.
		/// </summary>
		/// <param name="errMsg"></param>
		private void ShowError(String errMsg)
		{
			this.UpdateStatus(errMsg);
			MessageBox.Show(this, errMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		/// <summary>
		/// Prints a message to the status bar at the bottom of the GUI.
		/// </summary>
		/// <param name="statusMsg"></param>
		private void UpdateStatus(String statusMsg)
		{
			this.mainStatusLbl.Text = DateTime.Now.ToShortTimeString() + ": " + statusMsg;
		}

		private void QueryInputRegister()
		{
			String err_str = "";
			Byte slaveAddr;

			while (true)
			{
				slaveAddr = 3;
				if (slaveAddr == 0)
				{
					return;
				}

				UInt16 regAddr;
				regAddr = 4003;

				UInt16 numRegs;
				numRegs = 2;
				DataType dataType = DataType.Float;

				UInt16[] regValues;
				if (this.ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
											null, out regValues, out err_str) == false)
				{
					this.ShowError(err_str);
					return;
				}
				this.displayData(this.inputRegDataTb, dataType, regValues);                
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
		/// <param name="slaveAddress"></param>
		/// <param name="startAddress"></param>
		/// <param name="numberOfCoils"></param>
		/// <param name="writeValues"></param>
		/// <param name="result"></param>
		/// <param name="errorMsg"></param>
		/// <returns></returns>
		private bool ReadWriteCoils(MBActionType action, Byte slaveAddress, UInt16 startAddress, UInt16 numberOfCoils,
									bool[] writeValues, out bool[] result, out String errorMsg)
		{
			bool success = true;

			errorMsg = "No Error";
			result = null;

			try
			{
				if (action == MBActionType.Read)
				{
					result = this.mbMaster.ReadCoils(slaveAddress, (UInt16)(startAddress - 1), numberOfCoils);
				}
				else // write
				{
					if (writeValues.Length == 1)
					{
						this.mbMaster.WriteSingleCoil(slaveAddress, (UInt16)(startAddress - 1), writeValues[0]);
					}
					else
					{
						this.mbMaster.WriteMultipleCoils(slaveAddress, (UInt16)(startAddress - 1), writeValues);
					}
				}
			}
			catch (TimeoutException)
			{
				errorMsg = "Timeout: The Slave failed to respond within the desired time frame";
				success = false;
			}
			catch (Exception exp)
			{
				errorMsg = exp.ToString();
				success = false;
			}

			if (mbMaster.Error != Errors.NO_ERROR)
			{
				success = false;
			}

			return success;
		}

		/*! \brief Read Modbus Input Registers
		 * 
		 * Calls the Modbus library read input registers.  This function will handle the exceptions and
		 * return False and an error string in errorMsg if the commands fails.
		 * @param slaveAddress The target device address on the bus.
		 * @param startAddress The address of the first register to read in generic 1-based addressing
		 * @param result The data read from the specified target
		 * @param errorMsg An error message describing why the command failed.
		 * @return true if success, otherwise false.
		 */
		private bool ReadWriteRegisters(MBRegisterType type, MBActionType action, Byte slaveAddress, UInt16 startAddress,
										UInt16 numberOfRegisters, UInt16[] writeValues, out UInt16[] result, out String errorMsg)
		{
			bool success = true;

			errorMsg = "No Error";
			result = null;

			try
			{
				if (type == MBRegisterType.Input)
				{
					if (action == MBActionType.Read)
					{
						result = this.mbMaster.ReadInputRegisters(slaveAddress, (UInt16)(startAddress - 1), numberOfRegisters);
					}
					else // Write
					{
						success = false;
						errorMsg = "Invalid Request";
					}
				}
				else // Holding
				{
					if (action == MBActionType.Read)
					{
						result = this.mbMaster.ReadHoldingRegisters(slaveAddress, (UInt16)(startAddress - 1), numberOfRegisters);
					}
					else if (action == MBActionType.WriteSingle)
					{
						for (int writeIndex = 0; writeIndex < numberOfRegisters; writeIndex++)
						{
							this.mbMaster.WriteSingleRegister(slaveAddress, (UInt16)(startAddress - 1), writeValues[writeIndex]);
							// Next register address
							startAddress += 1;
						}
					}
					else // Write
					{
						this.mbMaster.WriteMultipleRegisters(slaveAddress, (UInt16)(startAddress - 1), writeValues);
					}
				}
			}
			catch (TimeoutException)
			{
				errorMsg = "Timeout: The Slave failed to respond within the desired time frame";
				success = false;
			}
			catch (Exception exp)
			{
				errorMsg = exp.ToString();
				success = false;
			}

			if (mbMaster.Error == Errors.RX_TIMEOUT)
			{
				errorMsg = "Timeout:  The Slave failed to respond within the desired time frame.";
			}
			if (mbMaster.Error != Errors.NO_ERROR)
			{
				success = false;
			}

			return success;
		}

		private double RegistersToDouble(DataType type, UInt16[] regs)
		{
			List<byte> raw = new List<byte>(4);
			byte[] raw_reg;
			double result = 0;

			switch (type)
			{
				case DataType.Float:
				case DataType.Int32:
				case DataType.UInt32:
					raw_reg = BitConverter.GetBytes(regs[0]);
					raw.Add(raw_reg[0]);
					raw.Add(raw_reg[1]);
					raw_reg = BitConverter.GetBytes(regs[1]);
					raw.Add(raw_reg[0]);
					raw.Add(raw_reg[1]);
					break;

				case DataType.Int16:
				case DataType.UInt16:
					raw_reg = BitConverter.GetBytes(regs[0]);
					raw.Add(raw_reg[0]);
					raw.Add(raw_reg[1]);
					break;
				case DataType.String:
					break;
			}

			switch (type)
			{
				case DataType.Float:
					result = BitConverter.ToSingle(raw.ToArray(), 0);
					break;
				case DataType.Int32:
					result = BitConverter.ToInt32(raw.ToArray(), 0);
					break;
				case DataType.UInt32:
					result = BitConverter.ToUInt32(raw.ToArray(), 0);
					break;
				case DataType.Int16:
					result = BitConverter.ToInt16(raw.ToArray(), 0);
					break;
				case DataType.UInt16:
					result = BitConverter.ToUInt16(raw.ToArray(), 0);
					break;
				case DataType.String:
					break;
			}

			return result;
		}
	}
}
