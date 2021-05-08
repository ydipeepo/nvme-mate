using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ydi.NvmeMate
{
	public sealed class NvmeSmartAttributesReporter : DisposableBase
	{
		readonly Dictionary<string, NvmeSmartAttributesReporterInstance> instanceMap = new();
		readonly StringLocalizer stringLocalizer;

		public IReadOnlyList<NvmeSmartAttributesReporterInstance> Instances => instanceMap.Values.ToList();

		public NvmeSmartAttributesReporter(StringLocalizer stringLocalizer) => this.stringLocalizer = stringLocalizer;
		public void DeleteCategory()
		{
			var categoryName = stringLocalizer["CATEGORY_NAME"];
#pragma warning disable CA1416
			if (PerformanceCounterCategory.Exists(categoryName))
			{
				PerformanceCounterCategory.Delete(categoryName);
			}
#pragma warning restore CA1416
		}
		public void CreateCategory()
		{
#pragma warning disable CA1416
			if (PerformanceCounterCategory.Exists(stringLocalizer["CATEGORY_NAME"]))
			{
				return;
			}
			var categoryCreationData = new CounterCreationDataCollection
			{
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["TEMPERATURE_KELVIN_NAME"], CounterHelp = stringLocalizer["TEMPERATURE_KELVIN_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["TEMPERATURE_DEG_CELSIUS_NAME"], CounterHelp = stringLocalizer["TEMPERATURE_DEG_CELSIUS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["AVAILABLE_SPARE_NAME"], CounterHelp = stringLocalizer["AVAILABLE_SPARE_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["AVAILABLE_SPARE_THRESHOLD_NAME"], CounterHelp = stringLocalizer["AVAILABLE_SPARE_THRESHOLD_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["PERCENTAGE_USED_ESTIMATE_NAME"], CounterHelp = stringLocalizer["PERCENTAGE_USED_ESTIMATE_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["DATA_UNITS_READ_NAME"], CounterHelp = stringLocalizer["DATA_UNITS_READ_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["DATA_UNITS_WRITE_NAME"], CounterHelp = stringLocalizer["DATA_UNITS_WRITE_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["HOST_READ_COMMANDS_NAME"], CounterHelp = stringLocalizer["HOST_READ_COMMANDS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["HOST_WRITE_COMMANDS_NAME"], CounterHelp = stringLocalizer["HOST_WRITE_COMMANDS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["CONTROLLER_BUSY_TIME_NAME"], CounterHelp = stringLocalizer["CONTROLLER_BUSY_TIME_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["POWER_CYCLES_NAME"], CounterHelp = stringLocalizer["POWER_CYCLES_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["POWER_ON_HOURS_NAME"], CounterHelp = stringLocalizer["POWER_ON_HOURS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["UNSAFE_SHUTDOWNS_NAME"], CounterHelp = stringLocalizer["UNSAFE_SHUTDOWNS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["MEDIA_ERRORS_NAME"], CounterHelp = stringLocalizer["MEDIA_ERRORS_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems64, CounterName = stringLocalizer["NUMBER_OF_ERROR_INFORMATION_LOG_ENTRIES_NAME"], CounterHelp = stringLocalizer["NUMBER_OF_ERROR_INFORMATION_LOG_ENTRIES_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["WARNING_COMPOSITE_TEMPERATURE_TIME_NAME"], CounterHelp = stringLocalizer["WARNING_COMPOSITE_TEMPERATURE_TIME_HELP"] },
				new CounterCreationData { CounterType = PerformanceCounterType.NumberOfItems32, CounterName = stringLocalizer["CRITICAL_COMPOSITE_TEMPERATURE_TIME_NAME"], CounterHelp = stringLocalizer["CRITICAL_COMPOSITE_TEMPERATURE_TIME_HELP"] },
			};
			PerformanceCounterCategory.Create(stringLocalizer["CATEGORY_NAME"], stringLocalizer["CATEGORY_HELP"], PerformanceCounterCategoryType.MultiInstance, categoryCreationData);
#pragma warning restore CA1416
		}
		public void AddInstance(string moniker, string model, string serialNumber) => instanceMap.Add(moniker, new(moniker, model, serialNumber,stringLocalizer));
		public void AddInstance(string moniker) => AddInstance(moniker, null, null);
		public void Next(string moniker, in NvmeSmartAttributes attributes) => instanceMap[moniker].Next(attributes);
		protected override void OnDispose(bool disposing)
		{
			foreach (var instance in instanceMap.Values)
			{
				instance.Dispose();
			}
		}
	}
}
