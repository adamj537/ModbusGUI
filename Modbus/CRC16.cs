using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		const ushort POLY = 0xA001;

		/// <summary>
		/// CRC table
		/// </summary>
		static ushort[] crc_tab16;

		/// <summary>
		/// Initialize CRC table
		/// </summary>
		static void InitCRC16Tab()
		{
			ushort crc, c;

			if (crc_tab16 == null)
			{
				crc_tab16 = new ushort[256];
				for (int ii = 0; ii < 256; ii++)
				{
					crc = 0;
					c = (ushort)ii;
					for (int jj = 0; jj < 8; jj++)
					{

						if (((crc ^ c) & 0x0001) == 0x0001)
							crc = (ushort)((crc >> 1) ^ POLY);
						else
							crc = (ushort)(crc >> 1);

						c = (ushort)(c >> 1);
					}

					crc_tab16[ii] = crc;
				}
			}
		}

		/// <summary>
		/// Update CRC value
		/// </summary>
		/// <param name="crc">Actual CRC value</param>
		/// <param name="bt">Data byte</param>
		/// <returns>Computed CRC</returns>
		static ushort UpdateCRC16(ushort crc, byte bt)
		{
			ushort tmp, ushort_bt;

			ushort_bt = (ushort)(0x00FF & (ushort)bt);

			InitCRC16Tab();

			tmp = (ushort)(crc ^ ushort_bt);
			crc = (ushort)((crc >> 8) ^ crc_tab16[tmp & 0xff]);

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
