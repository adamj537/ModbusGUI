using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using Modbus;

namespace Test
{
	class Program
	{
		#region Test functions

		#region Modbus RTU slave

		/// <summary>
		/// Test a modbus RTU slave
		/// </summary>
		static void Test_ModbusRTUSlave()
		{
			byte unit_id = 1;			// MODBUS address of the slave

			// Created datastore for unit ID 1.
			// This "datastore" will be a MODBUS slave device with address 1.
			Datastore ds = new Datastore(unit_id);

			// Crete instance of modbus serial RTU (replace COMx with a valid serial port - ex. COM5).
			ModbusSlaveSerial ms = new ModbusSlaveSerial(new Datastore[] { ds }, ModbusSerialType.RTU, "COM1", 9600, 8, Parity.Even, StopBits.One, Handshake.None);

			// Start listening.
			ms.StartListen();

			// Generate a random number (used in the test).
			Random rnd = new Random();

			// Print and write some registers...
			while (true)
			{
				// Print some nice formatting to the console.
				Console.WriteLine("---------------------- READING ----------------------");

				// Create MODBUS holding register 1.
				ushort holdingReg1 = ms.ModbusDB.Single(x => x.UnitID == unit_id).HoldingRegisters[0];

				// Print the output to the console.
				Console.WriteLine("Holding register n.1  : " + holdingReg1.ToString("D5"));

				// Create MODBUS holding register 60.
				ushort holdingReg60 = ms.ModbusDB.Single(x => x.UnitID == unit_id).HoldingRegisters[59];

				// Print the output to the console.
				Console.WriteLine("Holding register n.60 : " + holdingReg60.ToString("D5"));

				// Create MODBUS coil 32.
				bool coil32 = ms.ModbusDB.Single(x => x.UnitID == unit_id).Coils[31];

				// Print the output to the console.
				Console.WriteLine("Coil register    n.32 : " + coil32.ToString());

				// Print some nice formatting to the console.
				Console.WriteLine("---------------------- WRITING ----------------------");

				// Do something (what?) to holding register 2.
				ms.ModbusDB.Single(x => x.UnitID == unit_id).HoldingRegisters[1] = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);

				// Do something (what?) to holding register 2.
				ushort holdingReg2 = ms.ModbusDB.Single(x => x.UnitID == unit_id).HoldingRegisters[1];

				// Print the output to the console.
				Console.WriteLine("Holding register n.2  : " + holdingReg2.ToString("D5"));

				// Do something (what?) to coil 15.
				ms.ModbusDB.Single(x => x.UnitID == unit_id).Coils[15] = Convert.ToBoolean(rnd.Next(0, 1));

				// Do something (what?) to coil 15.
				bool coil15 = ms.ModbusDB.Single(x => x.UnitID == unit_id).Coils[15];

				// Print the output to the console.
				Console.WriteLine("Coil register    n.16 : " + coil15.ToString());

				// Repeat every 2 seconds.
				Thread.Sleep(2000);
			}
		}

		#endregion

		#region Modbus RTU master

		/// <summary>
		/// Test modbus RTU master function on a slave RTU id = 5
		/// </summary>
		static void Test_ModbusRTUMaster()
		{
			byte unit_id = 5;
			// Create instance of modbus serial RTU (replace COMx with a free serial port - ex. COM5).
			ModbusMasterSerial mm = new ModbusMasterSerial(ModbusSerialType.RTU, "COM20", 9600, 8, Parity.Even, StopBits.One, Handshake.None);

			// Initialize the MODBUS connection.
			mm.Connect();

			// Read and write some registers on RTU n. 5.
			Random rnd = new Random();
			while (true)
			{
				// Print some nice formatting to the console.
				Console.WriteLine("---------------------- READING ----------------------");

				// Read from a register.
				ushort inputReg1 = mm.ReadHoldingRegisters(unit_id, 0, 1).First();

				// Print the result to the console.
				Console.WriteLine("Holding register n.1  : " + inputReg1.ToString("D5"));

				// Read from another register.
				ushort inputReg41 = mm.ReadInputRegisters(unit_id, 40, 1).First();

				// Print the result to the console.
				Console.WriteLine("Input register   n.41 : " + inputReg41.ToString("D5"));

				// Read from a coil.
				bool coil23 = mm.ReadCoils(unit_id, 22, 1).First();

				// Print the results to the console.
				Console.WriteLine("Coil register    n 23 : " + coil23.ToString());

				// Print some nice formatting to the console.
				Console.WriteLine("---------------------- WRITING ----------------------");

				// Write to a register.
				mm.WriteSingleRegister(unit_id, 4, (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue));

				// ...then read it back.
				ushort holdingReg4 = mm.ReadHoldingRegisters(unit_id, 4, 1).First();

				// Print the result to the console.
				Console.WriteLine("Holding register n.5  : " + holdingReg4.ToString("D5") + Environment.NewLine);

				// Write to another register.
				mm.WriteSingleCoil(unit_id, 2, Convert.ToBoolean(rnd.Next(0, 1)));

				// ...then read it back.
				bool holdingReg3 = mm.ReadCoils(unit_id, 2, 1).First();

				// Print the result to the console.
				Console.WriteLine("Coil register    n.3  : " + holdingReg3.ToString() + Environment.NewLine);

				// Repeat every 2 seconds.
				Thread.Sleep(2000);
			}
		}

		#endregion

		#endregion

		/// <summary>
		/// Program entry point
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			// Enter test code here...

			// Some default tests...uncomment to use.

			Test_ModbusRTUMaster();
			//Test_ModbusRTUSlave();
		}
	}
}
