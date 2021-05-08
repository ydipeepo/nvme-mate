using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;

static class Kernel32
{
	[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
	public static extern SafeFileHandle CreateFile(
		string lpFileName,
		[MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
		[MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
		IntPtr lpSecurityAttributes,
		[MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
		[MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
		IntPtr hTemplateFile);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool DeviceIoControl(
		SafeFileHandle hDevice,
		uint dwIoControlCode,
		byte[] lpInBuffer,
		uint nInBufferSize,
		byte[] lpOutBuffer,
		uint nOutBufferSize,
		out uint lpBytesReturned,
		IntPtr lpOverlapped);
}
