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
		/// <param name="uid">Device ID</param>
		/// <param name="n_discrete_inputs">Input registers (read-only - 1 bit)</param>
		/// <param name="n_coils">Coils registers (read/write - 1 bit)</param>
		/// <param name="n_input_registers">Input registers (read-only - 16 bit)</param>
		/// <param name="n_holding_registers">Holding registers (read/write - 16 bit)</param>
		public Datastore(byte uid, int n_discrete_inputs, int n_coils, int n_input_registers, int n_holding_registers)
		{
			// Set device ID
			UnitID = uid;
			// Validate values and set db length
			if (((n_discrete_inputs >= 0) && (n_discrete_inputs <= MAX_ELEMENTS)) &&
				((n_coils >= 0) && (n_coils <= MAX_ELEMENTS)) &&
				((n_input_registers >= 0) && (n_input_registers <= MAX_ELEMENTS)) &&
				((n_holding_registers >= 0) && (n_holding_registers <= MAX_ELEMENTS)))
			{
				DiscreteInputs = new bool[n_discrete_inputs];
				Coils = new bool[n_coils];
				InputRegisters = new ushort[n_input_registers];
				HoldingRegisters = new ushort[n_holding_registers];
			}
			else
				throw new Exception("Database definition wrong , each set of records must be between 0 and e " + MAX_ELEMENTS.ToString() + "!");
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="uid">Device ID</param>
		/// <remarks>The database if initialized at the maximum capacity allowed</remarks>
		public Datastore(byte uid)
		{
			// Set device ID
			UnitID = uid;
			// Set DB length
			DiscreteInputs = new bool[MAX_ELEMENTS];
			Coils = new bool[MAX_ELEMENTS];
			InputRegisters = new ushort[MAX_ELEMENTS];
			HoldingRegisters = new ushort[MAX_ELEMENTS];
		}

		#endregion
	}
}
