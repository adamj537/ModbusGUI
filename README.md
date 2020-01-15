# ModbusGUI
C# Desktop utility for communicating with MODBUS devices

This is a simple Windows application which allows the user to connect to a MODBUS device through a serial port.  Right now the application only supports MODBUS RTU and MODBUS ASCII, but the underlying library supports MODBUS TCP/IP as well.  Only MODBUS RTU has been tested, so if you use this for MODBUS ASCII I'd appreciate feedback.  See the list of [known bugs](https://github.com/adamj537/ModbusGUI/issues) for more information about the state of the app.

## Installation
The first release is not yet available, so installation requires building from source.  You need [Visual Studio 2017](https://visualstudio.microsoft.com) or later with Microsoft .NET Framework 4.7.2.  After building, [publish the solution](https://docs.microsoft.com/en-us/dotnet/core/tutorials/publishing-with-visual-studio) to create an installer.  After running the installer, the application will be available from the Windows Start menu.

## Usage
The app will run without any hardware, but to do anything useful, you'll need a serial port and a device that uses the MODBUS RTU protocol.
1. Select the serial port options.
2. Click the "Open" radio button to open the port.
3. Set the device address.  There are text boxes for hex and decimal versions of the address, and as soon as you change one, the other will update.
4. Go to the tab of the type of MODBUS register you'd like to read or write.
5. Set the coil/input/register address.  Like the device address, there are text boxes for hex and decimal input, and as soon as one is changed, the other will update.
6. Click "Read" or "Write" to send the corresponding MODBUS message and listen for a response.
7. When finished, click the "Closed" radio button to close the serial port.  Closing the program will also close the port.

If something goes wrong (e.g. a timeout, corrupted message, etc.) a message will appear.

## Contributing
If you've got something valuable, feel free to submit a pull request.
