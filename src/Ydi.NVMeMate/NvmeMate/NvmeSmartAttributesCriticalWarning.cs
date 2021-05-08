namespace Ydi.NvmeMate
{
	public struct NvmeSmartAttributesCriticalWarning
	{
		readonly byte bits;

		internal NvmeSmartAttributesCriticalWarning(byte bits) => this.bits = bits;

		public bool AvailableSpaceLow => (bits & 1) != 0;
		public bool TemperatureThreshold => (bits & 2) != 0;
		public bool ReliabilityDegraded => (bits & 4) != 0;
		public bool ReadOnly => (bits & 8) != 0;
		public bool VolatileMemoryBackupDeviceFailed => (bits & 16) != 0;
	}
}
