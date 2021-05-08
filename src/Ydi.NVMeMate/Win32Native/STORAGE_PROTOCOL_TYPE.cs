enum STORAGE_PROTOCOL_TYPE : uint
{
	ProtocolTypeUnknown,
	ProtocolTypeScsi,
	ProtocolTypeAta,
	ProtocolTypeNvme,
	ProtocolTypeSd,
	ProtocolTypeUfs,
	ProtocolTypeProprietary = 0x7E,
	ProtocolTypeMaxReserved = 0x7F
}
