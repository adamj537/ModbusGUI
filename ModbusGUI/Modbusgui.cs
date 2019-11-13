using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Modbus;

namespace ModbusGUI
{
	public partial class Modbusgui : Form
	{
		#region Enumerations

		/// <summary>
		/// Data types that can be transmitted by MODBUS
		/// </summary>
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

		/// <summary>
		/// Types of registers that MODBUS can use
		/// </summary>
		enum MBRegisterType
		{
			Input,
			Holding,
		}

		/// <summary>
		/// Types of MODBUS transactions
		/// </summary>
		enum MBActionType
		{
			Read,
			Write,
			WriteSingle,
		}

		#endregion

		#region Fields

		private ModbusMasterSerial _mbMaster = null;
		private Thread _pollThread = null;

		#endregion

		#region Constructor

		/// <summary>
		/// Runs when the GUI loads.
		/// </summary>
		public Modbusgui()
		{
			InitializeComponent();

			// Set some default settings.
			comboBoxMode.SelectedIndex = 0;
			comboBoxParity.SelectedIndex = 0;
			comboBoxBaudRate.SelectedIndex = 0;
			comboBoxStopBits.SelectedIndex = 0;

			// Populate serial port names.
			comboBoxSerialPort.Items.AddRange(SerialPort.GetPortNames());

			// Set the control values to the previously saved selections.
			comboBoxSerialPort.Text = Properties.Settings.Default.Port;
			comboBoxBaudRate.Text = Properties.Settings.Default.BaudRate.ToString();
			comboBoxParity.Text = Properties.Settings.Default.Parity;
			comboBoxStopBits.Text = Properties.Settings.Default.StopBits.ToString();
			comboBoxMode.Text = Properties.Settings.Default.Mode;
			textBoxSlaveAddr.Text = Properties.Settings.Default.Address.ToString();
		}

		#endregion

		#region Form Closing Methods

		/// <summary>
		/// Runs when the GUI is closed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Dispose of the MODBUS class.
			if (_mbMaster != null)
			{
				_mbMaster.Disconnect();
			}

			// Save settings used by the program.
			Properties.Settings.Default.Port = comboBoxSerialPort.Text;
			Properties.Settings.Default.BaudRate = Convert.ToUInt16(comboBoxBaudRate.Text);
			Properties.Settings.Default.Parity = comboBoxParity.Text;
			Properties.Settings.Default.StopBits = Convert.ToUInt16(comboBoxStopBits.Text);
			Properties.Settings.Default.Mode = comboBoxMode.Text;
			Properties.Settings.Default.Address = Convert.ToUInt16(textBoxSlaveAddr.Text);

			// Store the current values of the application settings properties.
			// If this call is omitted, then the settings will not be saved after the program quits.
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// Runs when the user clicks File -> Exit in the Menu.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Serial Port Methods

		/// <summary>
		/// When the user clicks the Refresh Button, find serial ports.
		/// Refreshes the COM ports available to the user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonPortRefresh_Click(object sender, EventArgs e)
		{
			comboBoxSerialPort.Items.Clear();
			comboBoxSerialPort.SelectedIndex = -1;
			comboBoxSerialPort.Text = "";
			comboBoxSerialPort.Items.AddRange(SerialPort.GetPortNames());
		}

		/// <summary>
		/// When the user selects a radio button, open or close the serial port.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RadioButton_CheckedChanged(object sender, EventArgs e)
		{
			// Do stuff only if the radio button is checked.
			// (Otherwise the actions will run twice.)
			if (((RadioButton)sender).Checked)
			{
				try
				{
					// If the "Open" radio button has been checked...
					if (((RadioButton)sender) == radioButtonOpen)
					{
						// Alert the user.
						toolStripStatusLabel1.Text = "Opening serial port...";

						// If the MODBUS object exists (i.e. the port is open)...
						if (_mbMaster != null)
						{
							// Discard the MODBUS object (close the port).
							_mbMaster.Disconnect();
							_mbMaster = null;

							// Enable the MODBUS configuration controls.
							comboBoxSerialPort.Enabled = true;
							comboBoxBaudRate.Enabled = true;
							comboBoxParity.Enabled = true;
							comboBoxMode.Enabled = true;
							comboBoxStopBits.Enabled = true;

							// Disable the MODBUS read/write/poll buttons.
							buttonInputRegisterRead.Enabled = false;
							buttonInputRegisterPoll.Enabled = false;
							buttonHoldingRegisterRead.Enabled = false;
							buttonHoldingRegisterWrite.Enabled = false;
							buttonCoilRead.Enabled = false;
							buttonCoilWrite.Enabled = false;
						}
						// If the MODBUS port is closed...
						else
						{
							// Fetch the user's MODBUS configuration selections.
							object portName = comboBoxSerialPort.SelectedItem;
							object baudName = comboBoxBaudRate.SelectedItem;
							object parityName = comboBoxParity.SelectedItem;
							object mbMode = comboBoxMode.SelectedItem;
							object stopBits = comboBoxStopBits.SelectedItem;

							// Check for invalid configuration selections.
							if (portName == null)
							{
								ShowError("Serial Port not selected.");
							}
							else if (baudName == null)
							{
								ShowError("Baud Rate not selected.");
							}
							else if (parityName == null)
							{
								ShowError("Parity type not selected");
							}
							else if (mbMode == null)
							{
								ShowError("Modbus Mode not selected");
							}
							else if (stopBits == null)
							{
								ShowError("Stop Bits not selected");
							}
							// If the settings seem valid...
							else
							{
								// Convert the baud rate to an integer.
								int baudRate = int.Parse(baudName.ToString());

								// Convert stop bits string to comm port setting.
								StopBits stopbits = StopBits.One;
								if (stopBits.ToString() == "2")
								{
									stopbits = StopBits.Two;
								}

								// Convert parity string to comm port setting.
								Parity parity = Parity.Even;
								if (parityName.ToString() == "None")
								{
									parity = Parity.None;
								}
								else if (parityName.ToString() == "Odd")
								{
									parity = Parity.Odd;
								}

								// Select the MODBUS packet size based on the selected mode.
								int datasize = 8;
								if (mbMode.ToString() == "ASCII")
								{
									datasize = 7;
								}

								// Select the MODBUS mode.
								ModbusSerialType mode = ModbusSerialType.RTU;
								if (mbMode.ToString() == "ASCII")
								{
									mode = ModbusSerialType.ASCII;
								}

								// Create and open serial port.
								try
								{
									_mbMaster = new ModbusMasterSerial(mode, portName.ToString(), baudRate, datasize, parity, stopbits, Handshake.None);
									_mbMaster.Connect();

									// Disable the controls for MODBUS configuration settings.
									comboBoxSerialPort.Enabled = false;
									comboBoxBaudRate.Enabled = false;
									comboBoxParity.Enabled = false;
									comboBoxMode.Enabled = false;
									comboBoxStopBits.Enabled = false;

									// Enable the MODBUS read/write/poll buttons.
									buttonInputRegisterRead.Enabled = true;
									buttonInputRegisterPoll.Enabled = true;
									buttonHoldingRegisterRead.Enabled = true;
									buttonHoldingRegisterWrite.Enabled = true;
									buttonCoilRead.Enabled = true;
									buttonCoilWrite.Enabled = true;
								}
								catch (Exception exp)
								{
									ShowError("Unable to open serial port\n\n" + exp.ToString());
								}
							}
						}

						// Update the user interface.
						comboBoxSerialPort.Enabled = false;
						buttonPortRefresh.Enabled = false;
						groupBoxCommunication.Enabled = true;
						toolStripStatusLabel1.Text = "Port open.";
					}
					else if (((RadioButton)sender) == radioButtonClosed)
					{
						// Alert the user.
						toolStripStatusLabel1.Text = "Closing serial port...";

						// Dispose of the MODBUS class.
						if (_mbMaster != null)
						{
							_mbMaster.Disconnect();
						}

						// Update user interface.
						comboBoxSerialPort.Enabled = true;
						buttonPortRefresh.Enabled = true;
						groupBoxCommunication.Enabled = false;
						toolStripStatusLabel1.Text = "Port closed.";
					}
				}
				// If an error occurs...
				catch (Exception ex)
				{
					// Alert the user.
					MessageBox.Show(ex.Message, ex.GetType().Name.ToString());

					// Undo the user action.
					radioButtonClosed.Checked = true;
				}
			}
		}

		#endregion

		#region Slave Address Methods

		/// <summary>
		/// When the user finishes typing the slave address, format the text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBoxSlaveAddr_Leave(object sender, EventArgs e)
		{
			// Fetch what the user typed.
			string contents = textBoxSlaveAddr.Text;

			// Remmove leading and trailing whitespace.
			contents = contents.Trim();

			// If there's a leading "0x"...
			if (contents.StartsWith("0x"))
			{
				// Remove it.
				contents = contents.Substring(2);
				textBoxSlaveAddr.Text = contents;
			}
		}

		/// <summary>
		/// Read and format the slave device address.
		/// </summary>
		/// <returns></returns>
		private byte GetSlaveAddress()
		{
			byte address = 0;
			bool status;

			try
			{
				// Parse the address the user has entered.
				status = byte.TryParse(textBoxSlaveAddr.Text, NumberStyles.HexNumber,
										CultureInfo.InvariantCulture, out address);

				// If the text is not a number...
				if (status == false)
				{
					// Alert the user.
					ShowError("Invalid Slave Address");
				}
				// If the number is out of range...
				else if ((address < 1) || (address > 247))
				{
					// Alert the user.
					ShowError("Slave Address Must be between 1 and 247");

					// Set the address to something valid.
					address = 0;
				}
			}
			catch (Exception exp)
			{
				ShowError(exp.ToString());
			}

			return address;
		}

		#endregion

		#region Shared Methods

		/// <summary>
		/// Fetch the coil or register address from the form.
		/// </summary>
		/// <param name="tb"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool GetRegisterAddr(TextBox tb, out ushort value)
		{
			value = 0;
			bool retval = false;

			try
			{
				// Try to read the 
				retval = ushort.TryParse(tb.Text, out value);
				if (retval == false)
				{
					ShowError("Invalid Register Address");
				}
			}
			catch (Exception exp)
			{
				ShowError(exp.ToString());
			}

			return retval;
		}

		/// <summary>
		/// When the "Data Format" combo box is changed, enable or disable the
		/// string length textbox depending on whether "string" has been selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ComboBoxFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBoxDataFormat = (ComboBox)sender;

			// Find the corresponding "String Length" textbox.
			TextBox stringLengthTextBox = textBoxInputRegisterStringLength;
			if (comboBoxDataFormat.Equals(comboBoxHoldingRegisterFormat))
			{
				stringLengthTextBox = textBoxHoldingRegisterStringLength;
			}

			// If the user has selected a data format...
			if (comboBoxDataFormat.SelectedItem != null)
			{
				// If the selected format is "String"...
				if (comboBoxDataFormat.SelectedItem.ToString() == "String")
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
		}

		private DataType GetDataFormat(ComboBox cb, out ushort numRegs, TextBox tb = null)
		{
			DataType type = DataType.None;
			numRegs = 1;

			// Fetch the selection from the text box.
			object format_str = cb.SelectedItem;

			// Check for empty string.
			if (format_str == null)
			{
				ShowError("No format selected");
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
					ShowError("Please enter a valid string length in bytes: " + exp.ToString());
					type = DataType.None;
				}
			}
			else
			{
				type = DataType.UInt16;
			}

			return type;
		}

		#endregion

		#region Input Register Methods

		/// <summary>
		/// When the Input Register "Read" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonInputRegisterRead_Click(object sender, EventArgs e)
		{

			byte slaveAddr = GetSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			if (GetRegisterAddr(textBoxInputRegisterAddress, out ushort regAddr) == false)
			{
				return;
			}

			DataType dataType = GetDataFormat(comboBoxInputRegisterFormat, out ushort numRegs, textBoxInputRegisterStringLength);
			if (dataType == DataType.None)
			{
				return;
			}

			if (ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
										null, out ushort[] regValues, out string err_str) == false)
			{
				ShowError("Could not read Input Register");
				return;
			}
			DisplayData(textBoxInputRegisterData, dataType, regValues);

			toolStripStatusLabel1.Text = "Input Register Read Successfully";
		}

		/// <summary>
		/// When the Input Register "Poll" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonInputPoll_Click(object sender, EventArgs e)
		{
			if (_pollThread == null)
			{
				// Start a new thread to poll the requested register.
				_pollThread = new Thread(QueryInputRegister);
				_pollThread.Start();
			}
			else
			{
				try
				{
					// Stop the running thread.
					_pollThread.Abort();
				}
				catch (Exception exp)
				{
					ShowError(exp.ToString());
				}
				_pollThread = null;
			}
		}

		#endregion

		#region Holding Register Methods

		/// <summary>
		/// When the Holding Register "Read" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonHoldingRegisterRead_Click(object sender, EventArgs e)
		{
			byte slaveAddr = GetSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			if (GetRegisterAddr(textBoxHoldingRegisterAddress, out ushort regAddr) == false)
			{
				return;
			}

			DataType dataType = GetDataFormat(comboBoxHoldingRegisterFormat, out ushort numRegs, textBoxHoldingRegisterStringLength);
			if (dataType == DataType.None)
			{
				return;
			}

			if (ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Read, slaveAddr, regAddr, numRegs,
			null, out ushort[] regValues, out string errorMsg) == false)
			{
				ShowError(errorMsg);
				return;
			}
			DisplayData(textBoxHoldingRegisterData, dataType, regValues);

			toolStripStatusLabel1.Text = "Holding Register Read Successfully";
		}

		/// <summary>
		/// When the Holding Register "Write" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonHoldingRegisterWrite_Click(object sender, EventArgs e)
		{
			byte slaveAddr = GetSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			if (GetRegisterAddr(textBoxHoldingRegisterAddress, out ushort regAddr) == false)
			{
				return;
			}

			DataType dataType = GetDataFormat(comboBoxHoldingRegisterFormat, out ushort numRegs, textBoxHoldingRegisterStringLength);
			if (dataType == DataType.None)
			{
				return;
			}

			ushort[] regs = StringToRegisters(textBoxHoldingRegisterData.Text, dataType, numRegs);
			if (regs == null)
			{
				ShowError("Data to write does not match selected format");
				return;
			}

			if (ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Write, slaveAddr, regAddr, (ushort)regs.Length,
										regs, out ushort[] rsp, out string errorMsg) == false)
			{
				ShowError(errorMsg);
				return;
			}

			toolStripStatusLabel1.Text = "Holding Register Written Successfully";
		}

		#endregion

		#region Coil Methods

		/// <summary>
		/// When the Coil "Read" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCoilRead_Click(object sender, EventArgs e)
		{
			byte slaveAddr = GetSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			if (GetRegisterAddr(textBoxCoilAddr, out ushort regAddr) == false)
			{
				return;
			}

			if (ReadWriteCoils(MBActionType.Read, slaveAddr, regAddr, 1, null, out bool[] coils, out string errorMsg) == false)
			{
				ShowError(errorMsg);
				return;
			}

			// Show the value of the coil as T/F
			if (coils.Length > 0)
			{
				if (coils[0] == true)
				{
					radioButtonCoilFalse.Checked = false;
					radioButtonCoilTrue.Checked = true;
				}
				else
				{
					radioButtonCoilTrue.Checked = false;
					radioButtonCoilFalse.Checked = true;
				}
			}

			toolStripStatusLabel1.Text = "Coil Read Successfully";
		}

		/// <summary>
		/// When the Coil "Write" button is clicked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCoilWrite_Click(object sender, EventArgs e)
		{
			bool[] write_coils = new bool[1];

			byte slaveAddr = GetSlaveAddress();
			if (slaveAddr == 0)
			{
				return;
			}

			if (GetRegisterAddr(textBoxCoilAddr, out ushort regAddr) == false)
			{
				return;
			}

			write_coils[0] = radioButtonCoilTrue.Checked;
			if (ReadWriteCoils(MBActionType.Write, slaveAddr, regAddr, (ushort)write_coils.Length,
									write_coils, out bool[] read_coils, out string errorMsg) == false)
			{
				ShowError(errorMsg);
				return;
			}

			toolStripStatusLabel1.Text = "Coil Written Successfully";
		}

		#endregion


		private void DisplayData(TextBox tb, DataType type, ushort[] registers)
		{
			List<byte> raw = new List<byte>(4);
			byte[] rawRegister;
			string dataString = "";

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
					for (int i = raw.Count - 1; i >= 0; i--)
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

		private ushort[] StringToRegisters(string str, DataType type, ushort numRegs)
		{
			ushort[] regs = new ushort[numRegs];
			bool retval;

			switch (type)
			{
				case DataType.Float:
					{
						retval = float.TryParse(str, out float value);
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
						retval = int.TryParse(str, out int value);
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
						retval = uint.TryParse(str, out uint value);
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
						retval = short.TryParse(str, out short value);
						if (retval == false)
						{
							return null;
						}
						regs[0] = (ushort)value;
					}
					break;
				case DataType.UInt16:
					{
						retval = ushort.TryParse(str, out ushort value);
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
								regs[i] = (ushort)((ushort)strBytes[j] << 8);
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

		private void QueryInputRegister()
		{
			string err_str = "";
			byte slaveAddr;

			while (true)
			{
				slaveAddr = 3;
				if (slaveAddr == 0)
				{
					return;
				}

				ushort regAddr;
				regAddr = 4003;

				ushort numRegs;
				numRegs = 2;
				DataType dataType = DataType.Float;

				if (ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
											null, out ushort[] regValues, out err_str) == false)
				{
					ShowError(err_str);
					return;
				}
				DisplayData(textBoxInputRegisterData, dataType, regValues);
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
		private bool ReadWriteCoils(MBActionType action, byte slaveAddress, ushort startAddress, ushort numberOfCoils,
									bool[] writeValues, out bool[] result, out string errorMsg)
		{
			bool success = true;

			errorMsg = "No Error";
			result = null;

			try
			{
				if (action == MBActionType.Read)
				{
					result = _mbMaster.ReadCoils(slaveAddress, (ushort)(startAddress - 1), numberOfCoils);
				}
				else // write
				{
					if (writeValues.Length == 1)
					{
						_mbMaster.WriteSingleCoil(slaveAddress, (ushort)(startAddress - 1), writeValues[0]);
					}
					else
					{
						_mbMaster.WriteMultipleCoils(slaveAddress, (ushort)(startAddress - 1), writeValues);
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

			if (_mbMaster.Error != Errors.NO_ERROR)
			{
				success = false;
			}

			return success;
		}

		/// <summary>
		/// Read Modbus Input Registers
		/// </summary>
		/// <remarks>
		/// Calls the Modbus library read input registers.  This function will handle the exceptions and
		/// return false and an error string in errorMsg if the commands fails.
		/// </remarks>
		/// <param name="type"></param>
		/// <param name="action"></param>
		/// <param name="slaveAddress">target device address on the bus</param>
		/// <param name="startAddress">address of the first register to read in generic 1-based addressing</param>
		/// <param name="numberOfRegisters"></param>
		/// <param name="writeValues"></param>
		/// <param name="result">data read from the specified target</param>
		/// <param name="errorMsg">error message describing why the command failed</param>
		/// <returns>true if success, otherwise false</returns>
		private bool ReadWriteRegisters(MBRegisterType type, MBActionType action, byte slaveAddress, ushort startAddress,
										ushort numberOfRegisters, ushort[] writeValues, out ushort[] result, out string errorMsg)
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
						result = _mbMaster.ReadInputRegisters(slaveAddress, (ushort)(startAddress - 1), numberOfRegisters);
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
						result = _mbMaster.ReadHoldingRegisters(slaveAddress, (ushort)(startAddress - 1), numberOfRegisters);
					}
					else if (action == MBActionType.WriteSingle)
					{
						for (int writeIndex = 0; writeIndex < numberOfRegisters; writeIndex++)
						{
							_mbMaster.WriteSingleRegister(slaveAddress, (ushort)(startAddress - 1), writeValues[writeIndex]);
							// Next register address
							startAddress += 1;
						}
					}
					else // Write
					{
						_mbMaster.WriteMultipleRegisters(slaveAddress, (ushort)(startAddress - 1), writeValues);
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

			if (_mbMaster.Error == Errors.RX_TIMEOUT)
			{
				errorMsg = "Timeout:  The Slave failed to respond within the desired time frame.";
			}
			if (_mbMaster.Error != Errors.NO_ERROR)
			{
				success = false;
			}

			return success;
		}

		private double RegistersToDouble(DataType type, ushort[] regs)
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

		#region Helper Methods

		/// <summary>
		/// Prints a message to the status bar at the bottom of the GUI, and pops up
		/// a message window with the message.
		/// </summary>
		/// <param name="errMsg"></param>
		private void ShowError(string errMsg)
		{
			toolStripStatusLabel1.Text = errMsg;
			MessageBox.Show(this, errMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		#endregion
	}
}
