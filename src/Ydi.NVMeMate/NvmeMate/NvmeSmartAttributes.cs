using System.Numerics;

namespace Ydi.NvmeMate
{
	public struct NvmeSmartAttributes
	{
		readonly NVME_HEALTH_INFO_LOG nvmeHealthInfoLog;

		internal NvmeSmartAttributes(NVME_HEALTH_INFO_LOG nvmeHealthInfoLog) => this.nvmeHealthInfoLog = nvmeHealthInfoLog;

		public NvmeSmartAttributesCriticalWarning CriticalWarning => new(nvmeHealthInfoLog.CriticalWarning);
		public int Temperature => (nvmeHealthInfoLog.Temperature[1] << 8) | nvmeHealthInfoLog.Temperature[0];
		public int AvailableSpare => nvmeHealthInfoLog.AvailableSpare;
		public int AvailableSpareThreshold => nvmeHealthInfoLog.AvailableSpareThreshold;
		public int PercentageUsedEstimate => nvmeHealthInfoLog.PercentageUsed;
		public BigInteger DataUnitsRead => new(nvmeHealthInfoLog.DataUnitRead);
		public BigInteger DataUnitsWrite => new(nvmeHealthInfoLog.DataUnitWritten);
		public BigInteger HostReadCommands => new(nvmeHealthInfoLog.HostReadCommands);
		public BigInteger HostWriteCommands => new(nvmeHealthInfoLog.HostWrittenCommands);
		public BigInteger ControllerBusyTime => new(nvmeHealthInfoLog.ControllerBusyTime);
		public BigInteger PowerCycles => new(nvmeHealthInfoLog.PowerCycle);
		public BigInteger PowerOnHours => new(nvmeHealthInfoLog.PowerOnHours);
		public BigInteger UnsafeShutdowns => new(nvmeHealthInfoLog.UnsafeShutdowns);
		public BigInteger MediaErrors => new(nvmeHealthInfoLog.MediaErrors);
		public BigInteger NumberOfErrorInformationLogEntries => new(nvmeHealthInfoLog.ErrorInfoLogEntryCount);
		public uint WarningCompositeTemperatureTime => nvmeHealthInfoLog.WarningCompositeTemperatureTime;
		public uint CriticalCompositeTemperatureTime => nvmeHealthInfoLog.CriticalCompositeTemperatureTime;
	}
}
