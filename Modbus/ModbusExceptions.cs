using System;

namespace Modbus
{
	/// <summary>
	/// Custom class of exceptions for MODBUS devices
	/// </summary>
	/// <remarks>
	/// Don't throw this exception; best practice is to throw one of the derived classes below.
	/// Catch this exception in application-level code so your application handles all device errors.
	/// 
	/// The error codes (and their descriptions) were taken from MODBUS Application Protocol Specification V1.1b.
	/// </remarks>
	[Serializable]
	public class ModbusException : Exception
	{
		public ModbusException() { }
		public ModbusException(string message) : base(message) { }
		public ModbusException(string message, Exception inner) : base(message, inner) { }

		protected ModbusException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Timeout waiting for response.
	/// </summary>
	[Serializable]
	public class ModbusTimeoutException : ModbusException
	{
		public ModbusTimeoutException() { }
		public ModbusTimeoutException(string message) : base(message) { }
		public ModbusTimeoutException(string message, Exception inner) : base(message, inner) { }

		protected ModbusTimeoutException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Invalid parameter in a request message.
	/// </summary>
	[Serializable]
	public class ModbusRequestException : ModbusException
	{
		public ModbusRequestException() { }
		public ModbusRequestException(string message) : base(message) { }
		public ModbusRequestException(string message, Exception inner) : base(message, inner) { }

		protected ModbusRequestException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Invalid parameter in a response message.
	/// </summary>
	[Serializable]
	public class ModbusResponseException : ModbusException
	{
		public ModbusResponseException() { }
		public ModbusResponseException(string message) : base(message) { }
		public ModbusResponseException(string message, Exception inner) : base(message, inner) { }

		protected ModbusResponseException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Illegal Function Code (MODBUS error code 1)
	/// </summary>
	/// <remarks>
	/// The function code received in the query is not an allowable action for the device.
	/// This may be because the function code is only applicable to newer devices, and was not
	/// implemented in the unit selected.  It could also indicate that the device is in the
	/// wrong state to process a request of this type, for example because it is unconfigured
	/// and is being asked to return register values.
	/// </remarks>
	[Serializable]
	public class ModbusIllegalFunctionException : ModbusException
	{
		public ModbusIllegalFunctionException() { }
		public ModbusIllegalFunctionException(string message) : base(message) { }
		public ModbusIllegalFunctionException(string message, Exception inner) : base(message, inner) { }
		protected ModbusIllegalFunctionException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Illegal Data Address (Invalid Coil/Register Address) (MODBUS error code 2)
	/// </summary>
	/// <remarks>
	/// The data address received in the query is not an allowable address for the device.
	/// More specifically, the combination of reference number and transfer length is invalid.
	/// For a controller with 100 registers, the PDU addresses the first register as 0, and the
	/// last one as 99. If a request is submitted with a starting register address of 96 and a
	/// quantity of registers of 4, then this request will successfully operate (address-wise at
	/// least) on registers 96, 97, 98, 99. If a request is submitted with a starting register
	/// address of 96 and a quantity of registers of 5, then this request will fail with 
	/// Exception Code 0x02 “Illegal Data Address” since it attempts to operate on registers 
	/// 96, 97, 98, 99 and 100, and there is no register with address 100.
	/// </remarks>
	[Serializable]
	public class ModbusIllegalDataAddressException : ModbusException
	{
		public ModbusIllegalDataAddressException() { }
		public ModbusIllegalDataAddressException(string message) : base(message) { }
		public ModbusIllegalDataAddressException(string message, Exception inner) : base(message, inner) { }
		protected ModbusIllegalDataAddressException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Illegal Data Value (MODBUS error code 3)
	/// </summary>
	/// <remarks>
	/// A value contained in the query data field is not an allowable value for the device. This
	/// indicates a fault in the structure of the remainder of a complex request, such as that the
	/// implied length is incorrect. It specifically does NOT mean that a data item submitted for
	/// storage in a register has a value outside the expectation of the application program,
	/// since the MODBUS protocol is unaware of the significance of any particular value of any
	/// particular register.
	/// </remarks>
	[Serializable]
	public class ModbusIllegalDataValueException : ModbusException
	{
		public ModbusIllegalDataValueException() { }
		public ModbusIllegalDataValueException(string message) : base(message) { }
		public ModbusIllegalDataValueException(string message, Exception inner) : base(message, inner) { }
		protected ModbusIllegalDataValueException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Slave Device Failure (MODBUS error code 4)
	/// </summary>
	/// <remarks>
	/// An unrecoverable error occurred while the server (or slave) was attempting to perform the
	/// requested action.
	/// </remarks>
	[Serializable]
	public class ModbusSlaveDeviceFailureException : ModbusException
	{
		public ModbusSlaveDeviceFailureException() { }
		public ModbusSlaveDeviceFailureException(string message) : base(message) { }
		public ModbusSlaveDeviceFailureException(string message, Exception inner) : base(message, inner) { }
		protected ModbusSlaveDeviceFailureException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
		{
			throw new NotImplementedException();
		}
	}


}
