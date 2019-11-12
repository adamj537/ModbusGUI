using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modbus
{
	/// <summary>
	/// LRC Class
	/// </summary>
	internal static class LRC
	{
		/// <summary>
		/// Compute Buffer LRC
		/// </summary>
		/// <param name="buffer">Data buffer</param>
		/// <param name="offset">Buffer offset</param>
		/// <param name="length">Data length</param>
		/// <returns>Computed LRC</returns>
		public static byte CalcLRC(byte[] buffer, int offset, int length)
		{
			byte lrc = 0;
			for (int ii = 0; ii < length; ii++)
				lrc += buffer[ii + offset];
			return (byte)(-(sbyte)lrc);
		}
	}
}
