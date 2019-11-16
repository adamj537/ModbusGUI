using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Modbus;

namespace ModbusGUI
{
	public partial class FormModbus : Form
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

		// MODBUS serial device
		private ModbusMasterSerial _mbMaster = null;

		// thread for polling MODBUS data
		private Thread _pollThread = null;

		#endregion

		#region Form Methods

		/// <summary>
		/// Runs when the GUI loads.
		/// </summary>
		public FormModbus()
		{
			InitializeComponent();

			// Populate serial port names.
			comboBoxSerialPort.Items.AddRange(SerialPort.GetPortNames());

			// Set the control values to the previously saved selections.
			comboBoxSerialPort.Text = Properties.Settings.Default.Port;
			comboBoxBaudRate.Text = Properties.Settings.Default.BaudRate.ToString();
			comboBoxParity.Text = Properties.Settings.Default.Parity;
			comboBoxStopBits.Text = Properties.Settings.Default.StopBits.ToString();
			comboBoxMode.Text = Properties.Settings.Default.Mode;
			textBoxDeviceAddressHex.Text = Properties.Settings.Default.Address.ToString();

			// Populate the device address decimal textbox.
			TextBoxDeviceAddressHex_Leave(null, null);
		}

		/// <summary>
		/// When the application closes, save settings and close serial port.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormModbus_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Close the Modbus device.
			_mbMaster?.Disconnect();

			// Update application settings from the form.
			Properties.Settings.Default.Port = comboBoxSerialPort.Text;
			Properties.Settings.Default.BaudRate = Convert.ToUInt16(comboBoxBaudRate.Text);
			Properties.Settings.Default.StopBits = Convert.ToUInt16(comboBoxStopBits.Text);
			Properties.Settings.Default.Parity = comboBoxParity.Text;
			Properties.Settings.Default.Mode = comboBoxMode.Text;
			Properties.Settings.Default.Address = Convert.ToUInt16(textBoxDeviceAddressDecimal.Text);

			// Store the current values of the application settings properties.
			// If this call is omitted, then the settings will not be saved after the program quits.
			Properties.Settings.Default.Save();
		}

		/// <summary>
		/// When File -> Exit in the Menu, close the application.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

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

						// Fetch the selected serial port.
						string portName = comboBoxSerialPort.SelectedItem.ToString();
						if (string.IsNullOrEmpty(portName))
						{
							throw new ArgumentException("Serial Port not selected.");
						}

						// Fetch the selected baud rate.
						if (int.TryParse(comboBoxBaudRate.SelectedItem.ToString(), out int baudRate) == false)
						{
							throw new ArgumentException("Invalid baud rate.");
						}

						// Get the selected parity mode.
						string parityName = comboBoxParity.SelectedItem.ToString();
						if (string.IsNullOrEmpty(parityName))
						{
							throw new ArgumentException("Parity type not selected");
						}
						Parity parity = Parity.Even;
						if (parityName == "None")
						{
							parity = Parity.None;
						}
						else if (parityName == "Odd")
						{
							parity = Parity.Odd;
						}

						// Get the selected MODBUS mode (ASCII or RTU).
						string modeString = comboBoxMode.SelectedItem.ToString();
						if (modeString == null)
						{
							throw new ArgumentException("Modbus Mode not selected");
						}
						ModbusSerialType mode = ModbusSerialType.RTU;
						if (modeString == "ASCII")
						{
							mode = ModbusSerialType.ASCII;
						}

						// Select the MODBUS packet size based on the selected mode.
						int datasize = 8;
						if (mode == ModbusSerialType.ASCII)
						{
							datasize = 7;
						}

						// Get the selected number of stop bits.
						object stopBits = comboBoxStopBits.SelectedItem;
						if (stopBits == null)
						{
							throw new ArgumentException("Stop Bits not selected");
						}

						// Convert stop bits string to comm port setting.
						StopBits stopbits = StopBits.One;
						if (stopBits.ToString() == "2")
						{
							stopbits = StopBits.Two;
						}

						// Create and open serial port.
						_mbMaster = new ModbusMasterSerial(mode, portName.ToString(), baudRate, datasize, parity, stopbits, Handshake.None);
						_mbMaster.Connect();

						// Disable the serial port controls.
						buttonPortRefresh.Enabled = false;
						comboBoxSerialPort.Enabled = false;
						comboBoxBaudRate.Enabled = false;
						comboBoxParity.Enabled = false;
						comboBoxMode.Enabled = false;
						comboBoxStopBits.Enabled = false;

						// Enable the MODBUS communication controls.
						groupBoxAddress.Enabled = true;
						groupBoxData.Enabled = true;

						// Update the status bar.
						toolStripStatusLabel1.Text = "Port open.";
					}
					else if (((RadioButton)sender) == radioButtonClosed)
					{
						// Alert the user.
						toolStripStatusLabel1.Text = "Closing serial port...";

						// Dispose of the MODBUS class.
						_mbMaster?.Disconnect();

						// Enable the serial port controls.
						buttonPortRefresh.Enabled = true;
						comboBoxSerialPort.Enabled = true;
						comboBoxBaudRate.Enabled = true;
						comboBoxParity.Enabled = true;
						comboBoxMode.Enabled = true;
						comboBoxStopBits.Enabled = true;

						// Disable the MODBUS communication controls.
						groupBoxAddress.Enabled = false;
						groupBoxData.Enabled = false;

						// Update the status bar.
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

		#region Decimal / Hexidecimal Conversion Methods

		private string StringHexToDecimal(string hexString)
		{
			try
			{
				return Convert.ToInt32(hexString.Trim(), 16).ToString();
			}
			// If the number is invalid, just use an empty string.
			catch (FormatException)
			{
				return string.Empty;
			}
		}

		private string StringDecimalToHex(string decimalString)
		{
			try
			{
				return Convert.ToInt32(decimalString.Trim()).ToString("X");
			}
			// If the number is invalid, just use an empty string.
			catch (FormatException)
			{
				return string.Empty;
			}
		}

		private void TextBoxDeviceAddressHex_Leave(object sender, EventArgs e)
		{
			textBoxDeviceAddressDecimal.Text = StringHexToDecimal(textBoxDeviceAddressHex.Text);
		}

		private void TextBoxDeviceAddressDecimal_Leave(object sender, EventArgs e)
		{
			textBoxDeviceAddressHex.Text = StringDecimalToHex(textBoxDeviceAddressDecimal.Text);
		}

		private void TextBoxInputRegisterAddressHex_Leave(object sender, EventArgs e)
		{
			textBoxInputRegisterAddressDecimal.Text = StringHexToDecimal(textBoxInputRegisterAddressHex.Text);
		}

		private void TextBoxInputRegisterAddressDecimal_Leave(object sender, EventArgs e)
		{
			textBoxInputRegisterAddressHex.Text = StringDecimalToHex(textBoxInputRegisterAddressDecimal.Text);
		}

		private void TextBoxDiscreteInputAddressHex_Leave(object sender, EventArgs e)
		{
			textBoxDiscreteInputAddressDecimal.Text = StringHexToDecimal(textBoxDiscreteInputAddressHex.Text);
		}

		private void TextBoxDiscreteInputAddressDecimal_Leave(object sender, EventArgs e)
		{
			textBoxDiscreteInputAddressHex.Text = StringDecimalToHex(textBoxDiscreteInputAddressDecimal.Text);
		}

		private void TextBoxCoilAddressHex_Leave(object sender, EventArgs e)
		{
			textBoxCoilAddressDecimal.Text = StringHexToDecimal(textBoxCoilAddressHex.Text);
		}

		private void TextBoxCoilAddressDecimal_Leave(object sender, EventArgs e)
		{
			textBoxCoilAddressHex.Text = StringDecimalToHex(textBoxCoilAddressDecimal.Text);
		}

		private void TextBoxHoldingRegisterAddressHex_Leave(object sender, EventArgs e)
		{
			textBoxHoldingRegisterAddressDecimal.Text = StringHexToDecimal(textBoxHoldingRegisterAddressHex.Text);
		}

		private void TextBoxHoldingRegisterAddressDecimal_Leave(object sender, EventArgs e)
		{
			textBoxHoldingRegisterAddressHex.Text = StringDecimalToHex(textBoxHoldingRegisterAddressDecimal.Text);
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Read and format the device address.
		/// </summary>
		/// <returns>device address</returns>
		private byte GetDeviceAddress()
		{
			// Parse the address the user has entered.
			// If the text is not a number...
			if (byte.TryParse(textBoxDeviceAddressDecimal.Text, out byte address) == false)
			{
				throw new ArgumentException("Invalid Slave Address");
			}
			// If the number is out of range...
			else if ((address < 1) || (address > 247))
			{
				throw new ArgumentOutOfRangeException("Slave Address must be between 0d1 and 0d247");
			}

			return address;
		}

		/// <summary>
		/// Fetch the coil or register address from the form.
		/// </summary>
		/// <param name="text"></param>
		/// <returns>register address</returns>
		private ushort GetRegisterAddress(string text)
		{
			if (ushort.TryParse(text, out ushort value) == false)
			{
				throw new ArgumentException("Invalid Register Address");
			}

			return value;
		}

		private ushort GetNumRegisters(string stringLength)
		{
			ushort numRegs;
			try
			{
				float length = (float)Math.Round(float.Parse(stringLength) / 2.0f);
				numRegs = Convert.ToUInt16(length);
			}
			catch (Exception)
			{
				throw new ArgumentException("Please enter a valid string length in bytes.");
			}

			return numRegs;
		}

		private DataType GetDataFormat(ComboBox cb)
		{
			// Fetch the selection from the text box.
			object format_str = cb.SelectedItem;

			// Check for empty string.
			if (format_str == null)
			{
				throw new ArgumentException("No format selected");
			}

			DataType type;
			// Check for the various allowed selections.
			if (format_str.ToString() == "Float")
			{
				type = DataType.Float;
			}
			else if (format_str.ToString() == "Int32")
			{
				type = DataType.Int32;
			}
			else if (format_str.ToString() == "UInt32")
			{
				type = DataType.UInt32;
			}
			else if (format_str.ToString() == "Int16")
			{
				type = DataType.Int16;
			}
			else if (format_str.ToString() == "String")
			{
				type = DataType.String;
			}
			else
			{
				type = DataType.UInt16;
			}

			return type;
		}

		/// <summary>
		/// When the "Data Format" combo box is changed, enable or disable the
		/// string length textbox depending on whether "string" has been selected.
		/// </summary>
		/// <param name="selectedItem">item selected in the Data Formab Combo Box</param>
		/// <returns></returns>
		private bool SetStringLengthEnable(object selectedItem)
		{
			// If the selected format is "String", return true.
			return (selectedItem != null) && (selectedItem.ToString() == "String");
		}

		private void ComboBoxInputRegisterFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBoxInputRegisterStringLength.Enabled = SetStringLengthEnable(((ComboBox)sender).SelectedItem);
		}

		private void ComboBoxHoldingRegisterFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			textBoxHoldingRegisterStringLength.Enabled = SetStringLengthEnable(((ComboBox)sender).SelectedItem);
		}

		/// <summary>
		/// Convert an array of MODBUS register values into a string.
		/// </summary>
		/// <param name="type">type of the data</param>
		/// <param name="registers">data from a MODBUS packet</param>
		/// <returns></returns>
		private string RegistersToString(DataType type, ushort[] registers)
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
						dataString = Encoding.ASCII.GetString(raw.ToArray());
					}
					catch (Exception)
					{
						dataString = "<INVALID STRING>";
					}
					break;
			}

			return dataString;
		}

		/// <summary>
		/// Convert a string into an array of MODBUS register values.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="type"></param>
		/// <param name="numRegs"></param>
		/// <returns></returns>
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
						byte[] strBytes = Encoding.ASCII.GetBytes(str);
						int j = 0;
						// Copy the string data into the registers
						for (int i = 0; i < numRegs; i++)
						{
							if (j < strBytes.Length)
							{
								regs[i] = (ushort)(strBytes[j] << 8);
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

		#endregion

		#region Coil / Register Methods

		/// <summary>
		/// Read Input Registers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonInputRegisterRead_Click(object sender, EventArgs e)
		{
			try
			{
				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxInputRegisterAddressHex.Text);

				// Get the selected data format.
				DataType dataType = GetDataFormat(comboBoxInputRegisterFormat);

				// Calculate the number of registers to read.
				ushort numRegs = 2;
				if (dataType == DataType.String)
				{
					numRegs = GetNumRegisters(textBoxInputRegisterStringLength.Text);
				}

				// Read from the device.
				if (ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
											null, out ushort[] regValues, out string err_str) == false)
				{
					throw new Exception("Could not read Input Register");
				}

				// Format and display the data.
				textBoxInputRegisterData.Text = RegistersToString(dataType, regValues);

				// Update the status.
				toolStripStatusLabel1.Text = "Input Register Read Successfully";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
			}
		}

		/// <summary>
		/// Read Holding Registers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonHoldingRegisterRead_Click(object sender, EventArgs e)
		{
			try
			{
				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxHoldingRegisterAddressHex.Text);

				// Get the selected data format.
				DataType dataType = GetDataFormat(comboBoxHoldingRegisterFormat);

				// Calculate the number of registers to read.
				ushort numRegs = 2;
				if (dataType == DataType.String)
				{
					numRegs = GetNumRegisters(textBoxHoldingRegisterStringLength.Text);
				}

				// Read from the device.
				if (ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Read, slaveAddr, regAddr, numRegs,
				null, out ushort[] regValues, out string errorMsg) == false)
				{
					throw new Exception(errorMsg);
				}

				// Format and display the data.
				textBoxHoldingRegisterData.Text = RegistersToString(dataType, regValues);

				// Update the status.
				toolStripStatusLabel1.Text = "Holding Register Read Successfully";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
			}
		}

		/// <summary>
		/// Write Holding Registers.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonHoldingRegisterWrite_Click(object sender, EventArgs e)
		{
			try
			{
				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxHoldingRegisterAddressHex.Text);

				// Get the selected data format.
				DataType dataType = GetDataFormat(comboBoxHoldingRegisterFormat);

				// Calculate the number of registers to write.
				ushort numRegs = 2;
				if (dataType == DataType.String)
				{
					numRegs = GetNumRegisters(textBoxHoldingRegisterStringLength.Text);
				}

				// Convert the desired data into an array of register values.
				ushort[] regs = StringToRegisters(textBoxHoldingRegisterData.Text, dataType, numRegs);
				if (regs == null)
				{
					throw new ArgumentException("Data to write does not match selected format");
				}

				// Write the register values to the device.
				if (ReadWriteRegisters(MBRegisterType.Holding, MBActionType.Write, slaveAddr, regAddr, (ushort)regs.Length,
											regs, out ushort[] rsp, out string errorMsg) == false)
				{
					throw new Exception(errorMsg);
				}

				// Update the status.
				toolStripStatusLabel1.Text = "Holding Register Written Successfully";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
			}
		}

		/// <summary>
		/// Read Coils.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCoilRead_Click(object sender, EventArgs e)
		{
			try
			{
				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxCoilAddressHex.Text);

				// Read the selected coil.
				if (ReadWriteCoils(MBActionType.Read, slaveAddr, regAddr, 1, null, out bool[] coils, out string errorMsg) == false)
				{
					throw new Exception(errorMsg);
				}

				// Show the value of the coil as true or false.
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

				// Update the status.
				toolStripStatusLabel1.Text = "Coil Read Successfully";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
			}
		}

		/// <summary>
		/// Write Coils.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCoilWrite_Click(object sender, EventArgs e)
		{
			try
			{
				bool[] writeCoils = new bool[1];

				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxCoilAddressHex.Text);

				// Write the selected coil to the device.
				writeCoils[0] = radioButtonCoilTrue.Checked;
				if (ReadWriteCoils(MBActionType.Write, slaveAddr, regAddr, (ushort)writeCoils.Length,
										writeCoils, out bool[] read_coils, out string errorMsg) == false)
				{
					throw new Exception(errorMsg);
				}

				// Update the status.
				toolStripStatusLabel1.Text = "Coil Written Successfully";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
			}
		}

		/// <summary>
		/// Start/Stop polling an Input Register.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonInputPoll_Click(object sender, EventArgs e)
		{
			if (_pollThread == null)
			{
				// Get the selected device address.
				byte slaveAddr = GetDeviceAddress();

				// Get the selected register address.
				ushort regAddr = GetRegisterAddress(textBoxInputRegisterAddressHex.Text);

				// Get the selected data format.
				DataType dataType = GetDataFormat(comboBoxInputRegisterFormat);

				// Calculate the number of registers to read.
				ushort numRegs = 2;
				if (dataType == DataType.String)
				{
					numRegs = GetNumRegisters(textBoxInputRegisterStringLength.Text);
				}

				// Start a new thread to poll the requested register.
				_pollThread = new Thread(() =>
				{
					while (true)
					{
						// Read the input registers from the device.
						if (ReadWriteRegisters(MBRegisterType.Input, MBActionType.Read, slaveAddr, regAddr, numRegs,
													null, out ushort[] regValues, out string err_str) == false)
						{
							throw new Exception(err_str);
						}

						// Format the data.
						string dataString = RegistersToString(dataType, regValues);

						// Display the data.
						textBoxInputRegisterData.BeginInvoke((MethodInvoker)(() => textBoxInputRegisterData.Text = dataString));
					}
				});
				_pollThread.Start();
			}
			else
			{
				try
				{
					// Stop the running thread.
					_pollThread.Abort();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, ex.GetType().Name.ToString());
				}
				_pollThread = null;
			}
		}

		#endregion

		#region Get Rid Of These Methods

		/// <summary>
		/// Read or write coils.
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
		/// Read or write input registers.
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

		#endregion
	}
}
