using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
struct STORAGE_PROPERTY_QUERY
{
	public STORAGE_PROPERTY_ID PropertyId;
	public STORAGE_QUERY_TYPE QueryType;
	public byte[] AdditionalParameters;
}
