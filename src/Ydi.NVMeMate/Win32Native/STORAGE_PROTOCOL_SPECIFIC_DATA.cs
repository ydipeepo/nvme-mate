using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct STORAGE_PROTOCOL_SPECIFIC_DATA
{
	public STORAGE_PROTOCOL_TYPE ProtocolType;
	public uint DataType;
	public uint ProtocolDataRequestValue;
	public uint ProtocolDataRequestSubValue;
	public uint ProtocolDataOffset;
	public uint ProtocolDataLength;
	public uint FixedProtocolReturnData;
	public uint ProtocolDataRequestSubValue2;
	public uint ProtocolDataRequestSubValue3;
	public uint Reserved;
}
