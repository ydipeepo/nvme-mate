using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct STORAGE_PROTOCOL_DATA_DESCRIPTOR
{
	public uint Version;
	public uint Size;
	public STORAGE_PROTOCOL_SPECIFIC_DATA ProtocolSpecificData;
}
