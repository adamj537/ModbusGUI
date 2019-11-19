using System;

namespace Modbus
{
	/// <summary>
	/// Datastore class
	/// </summary>
	public sealed class Datastore
	{
		#region Constants

		/// <summary>
		/// Max DB elements
		/// </summary>
		private const int MAX_ELEMENTS = 65536;

		#endregion

		#region Properties

		/// <summary>
		/// Dicrete Input registers (read-only - 1 bit)
		/// </summary>
		public bool[] DiscreteInputs { get; set; }

		/// <summary>
		/// Coils registers (read/write - 1 bit)
		/// </summary>
		public bool[] Coils { get; set; }

		/// <summary>
		/// Input registers (read-only - 16 bit)
		/// </summary>
		public ushort[] InputRegisters { get; set; }

		/// <summary>
		/// Holding registers (read/write - 16 bit)
		/// </summary>
		public ushort[] HoldingRegisters { get; set; }

		/// <summary>
		/// Device ID
		/// </summary>
		public byte UnitID { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="deviceId">Device ID</param>
		/// <param name="numDiscreteInputs">Input registers (read-only - 1 bit)</param>
		/// <param name="numCoils">Coils registers (read/write - 1 bit)</param>
		/// <param name="numInputRegisters">Input registers (read-only - 16 bit)</param>
		/// <param name="numHoldingRegisters">Holding registers (read/write - 16 bit)</param>
		public Datastore(byte deviceId, int numDiscreteInputs, int numCoils, int numInputRegisters, int numHoldingRegisters)
		{
			// Set device ID
			UnitID = deviceId;
			// Validate values and set db length
			if ((numDiscreteInputs >= 0) && (numDiscreteInputs <= MAX_ELEMENTS) &&
				(numCoils >= 0) && (numCoils <= MAX_ELEMENTS) &&
				(numInputRegisters >= 0) && (numInputRegisters <= MAX_ELEMENTS) &&
				(numHoldingRegisters >= 0) && (numHoldingRegisters <= MAX_ELEMENTS))
			{
				DiscreteInputs = new bool[numDiscreteInputs];
				Coils = new bool[numCoils];
				InputRegisters = new ushort[numInputRegisters];
				HoldingRegisters = new ushort[numHoldingRegisters];
			}
			else
				throw new Exception("Each set of records must be between 0 and " + MAX_ELEMENTS.ToString() + "!");
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="deviceId">Device ID</param>
		/// <remarks>The database is initialized at the maximum capacity allowed</remarks>
		public Datastore(byte deviceId)
		{
			// Set device ID
			UnitID = deviceId;
			// Set DB length
			DiscreteInputs = new bool[MAX_ELEMENTS];
			Coils = new bool[MAX_ELEMENTS];
			InputRegisters = new ushort[MAX_ELEMENTS];
			HoldingRegisters = new ushort[MAX_ELEMENTS];
		}

		#endregion
	}
}
