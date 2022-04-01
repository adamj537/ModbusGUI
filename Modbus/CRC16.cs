namespace Modbus
{
	/// <summary>
	/// Static class for CRC16 compute
	/// </summary>
	internal static class CRC16
	{
		/// <summary>
		/// base poly
		/// </summary>
		private const ushort POLY = 0xA001;

		/// <summary>
		/// CRC table
		/// </summary>
		private static ushort[] _crcTable;

		/// <summary>
		/// Initialize CRC table
		/// </summary>
		private static void InitCRC16Table()
		{
			// If the CRC table hasn't been populated...
			if (_crcTable == null)
			{
				// Create a new array.
				_crcTable = new ushort[256];

				// For each element in the array...
				for (int i = 0; i < 256; i++)
				{
					ushort crc = 0;
					ushort c = (ushort)i;

					for (int j = 0; j < 8; j++)
					{

						if (((crc ^ c) & 0x0001) == 0x0001)
							crc = (ushort)((crc >> 1) ^ POLY);
						else
							crc = (ushort)(crc >> 1);

						c = (ushort)(c >> 1);
					}

					_crcTable[i] = crc;
				}
			}
		}

		/// <summary>
		/// Update CRC value
		/// </summary>
		/// <param name="crc">Actual CRC value</param>
		/// <param name="data">Data byte</param>
		/// <returns>Computed CRC</returns>
		private static ushort UpdateCRC16(ushort crc, byte data)
		{
			ushort tmp;
			ushort shortData;

			shortData = (ushort)(0x00FF & data);

			InitCRC16Table();

			tmp = (ushort)(crc ^ shortData);
			crc = (ushort)((crc >> 8) ^ _crcTable[tmp & 0xff]);

			return crc;
		}

		/// <summary>
		/// Calc buffer CRC16
		/// </summary>
		/// <param name="buffer">Data Buffer</param>
		/// <param name="offset">Buffer offset</param>
		/// <param name="length">Data length</param>
		/// <returns>Computed CRC</returns>
		public static ushort CalcCRC16(byte[] buffer, int offset, int length)
		{
			ushort crc = 0xFFFF;
			for (int ii = 0; ii < length; ii++)
				crc = UpdateCRC16(crc, buffer[offset + ii]);
			return crc;
		}
	}
}
