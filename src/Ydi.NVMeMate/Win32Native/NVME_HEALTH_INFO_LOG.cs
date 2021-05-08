using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct NVME_HEALTH_INFO_LOG
{
	// union
	// {
	// 	struct
	// 	{
	// 		UCHAR AvailableSpaceLow : 1;
	// 		UCHAR TemperatureThreshold : 1;
	// 		UCHAR ReliabilityDegraded : 1;
	// 		UCHAR ReadOnly : 1;
	// 		UCHAR VolatileMemoryBackupDeviceFailed : 1;
	// 		UCHAR Reserved : 3;
	// 	};
	// 	UCHAR AsUchar;
	// }
	// CriticalWarning;
	public byte CriticalWarning;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
	public byte[] Temperature;
	public byte AvailableSpare;
	public byte AvailableSpareThreshold;
	public byte PercentageUsed;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
	public byte[] Reserved0;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] DataUnitRead;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] DataUnitWritten;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] HostReadCommands;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] HostWrittenCommands;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] ControllerBusyTime;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] PowerCycle;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] PowerOnHours;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] UnsafeShutdowns;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] MediaErrors;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
	public byte[] ErrorInfoLogEntryCount;
	public uint WarningCompositeTemperatureTime;
	public uint CriticalCompositeTemperatureTime;
	public ushort TemperatureSensor1;
	public ushort TemperatureSensor2;
	public ushort TemperatureSensor3;
	public ushort TemperatureSensor4;
	public ushort TemperatureSensor5;
	public ushort TemperatureSensor6;
	public ushort TemperatureSensor7;
	public ushort TemperatureSensor8;
	[MarshalAs(UnmanagedType.ByValArray, SizeConst = 296)]
	public byte[] Reserved1;
}
