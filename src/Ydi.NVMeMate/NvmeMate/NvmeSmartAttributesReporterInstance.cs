using System;
using System.Diagnostics;
using System.Numerics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

#pragma warning disable CA1416

namespace Ydi.NvmeMate
{
	public sealed class NvmeSmartAttributesReporterInstance : DisposableBase
	{
		readonly PerformanceCounter temperatureKelvinCounter;
		readonly PerformanceCounter temperatureDegCelsiusCounter;
		readonly PerformanceCounter availableSpareCounter;
		readonly PerformanceCounter availableSpareThresholdCounter;
		readonly PerformanceCounter percentageUsedEstimateCounter;
		readonly PerformanceCounter dataUnitsReadCounter;
		readonly PerformanceCounter dataUnitsWriteCounter;
		readonly PerformanceCounter hostReadCommandsCounter;
		readonly PerformanceCounter hostWriteCommandsCounter;
		readonly PerformanceCounter controllerBusyTimeCounter;
		readonly PerformanceCounter powerCyclesCounter;
		readonly PerformanceCounter powerOnHoursCounter;
		readonly PerformanceCounter unsafeShutdownsCounter;
		readonly PerformanceCounter mediaErrorsCounter;
		readonly PerformanceCounter numberOfErrorInformationLogEntriesCounter;
		readonly PerformanceCounter warningCompositeTemperatureTimeCounter;
		readonly PerformanceCounter criticalCompositeTemperatureTimeCounter;
		readonly BehaviorSubject<int> temperatureKelvinBehavior = new(0);
		readonly BehaviorSubject<int> temperatureDegCelsiusBehavior = new(0);
		readonly BehaviorSubject<int> availableSpareBehavior = new(0);
		readonly BehaviorSubject<int> availableSpareThresholdBehavior = new(0);
		readonly BehaviorSubject<int> percentageUsedEstimateBehavior = new(0);
		readonly BehaviorSubject<long> dataUnitsReadBehavior = new(0);
		readonly BehaviorSubject<long> dataUnitsWriteBehavior = new(0);
		readonly BehaviorSubject<long> hostReadCommandsBehavior = new(0);
		readonly BehaviorSubject<long> hostWriteCommandsBehavior = new(0);
		readonly BehaviorSubject<long> controllerBusyTimeBehavior = new(0);
		readonly BehaviorSubject<long> powerCyclesBehavior = new(0);
		readonly BehaviorSubject<long> powerOnHoursBehavior = new(0);
		readonly BehaviorSubject<long> unsafeShutdownsBehavior = new(0);
		readonly BehaviorSubject<long> mediaErrorsBehavior = new(0);
		readonly BehaviorSubject<long> numberOfErrorInformationLogEntriesBehavior = new(0);
		readonly BehaviorSubject<long> warningCompositeTemperatureTimeBehavior = new(0);
		readonly BehaviorSubject<long> criticalCompositeTemperatureTimeBehavior = new(0);

		static long ToLong(BigInteger value)
		{
			try
			{
				return (long)value;
			}
			catch
			{
				return long.MaxValue;
			}
		}
		static int ToDegCelsius(int kelvin) => (int)Math.Ceiling(kelvin - 272.15);
		internal NvmeSmartAttributesReporterInstance(string moniker, string model, string serialNumber, StringLocalizer stringLocalizer)
		{
			var categoryName = stringLocalizer["CATEGORY_NAME"];
			temperatureKelvinCounter = new(categoryName, stringLocalizer["TEMPERATURE_KELVIN_NAME"], moniker, false);
			temperatureDegCelsiusCounter = new(categoryName, stringLocalizer["TEMPERATURE_DEG_CELSIUS_NAME"], moniker, false);
			availableSpareCounter = new(categoryName, stringLocalizer["AVAILABLE_SPARE_NAME"], moniker, false);
			availableSpareThresholdCounter = new(categoryName, stringLocalizer["AVAILABLE_SPARE_THRESHOLD_NAME"], moniker, false);
			percentageUsedEstimateCounter = new(categoryName, stringLocalizer["PERCENTAGE_USED_ESTIMATE_NAME"], moniker, false);
			dataUnitsReadCounter = new(categoryName, stringLocalizer["DATA_UNITS_READ_NAME"], moniker, false);
			dataUnitsWriteCounter = new(categoryName, stringLocalizer["DATA_UNITS_WRITE_NAME"], moniker, false);
			hostReadCommandsCounter = new(categoryName, stringLocalizer["HOST_READ_COMMANDS_NAME"], moniker, false);
			hostWriteCommandsCounter = new(categoryName, stringLocalizer["HOST_WRITE_COMMANDS_NAME"], moniker, false);
			controllerBusyTimeCounter = new(categoryName, stringLocalizer["CONTROLLER_BUSY_TIME_NAME"], moniker, false);
			powerCyclesCounter = new(categoryName, stringLocalizer["POWER_CYCLES_NAME"], moniker, false);
			powerOnHoursCounter = new(categoryName, stringLocalizer["POWER_ON_HOURS_NAME"], moniker, false);
			unsafeShutdownsCounter = new(categoryName, stringLocalizer["UNSAFE_SHUTDOWNS_NAME"], moniker, false);
			mediaErrorsCounter = new(categoryName, stringLocalizer["MEDIA_ERRORS_NAME"], moniker, false);
			numberOfErrorInformationLogEntriesCounter = new(categoryName, stringLocalizer["NUMBER_OF_ERROR_INFORMATION_LOG_ENTRIES_NAME"], moniker, false);
			warningCompositeTemperatureTimeCounter = new(categoryName, stringLocalizer["WARNING_COMPOSITE_TEMPERATURE_TIME_NAME"], moniker, false);
			criticalCompositeTemperatureTimeCounter = new(categoryName, stringLocalizer["CRITICAL_COMPOSITE_TEMPERATURE_TIME_NAME"], moniker, false);

			temperatureKelvinBehavior.Skip(1).Subscribe(value => temperatureKelvinCounter.RawValue = value);
			temperatureDegCelsiusBehavior.Skip(1).Subscribe(value => temperatureDegCelsiusCounter.RawValue = value);
			availableSpareBehavior.Skip(1).Subscribe(value => availableSpareCounter.RawValue = value);
			availableSpareThresholdBehavior.Skip(1).Subscribe(value => availableSpareThresholdCounter.RawValue = value);
			percentageUsedEstimateBehavior.Skip(1).Subscribe(value => percentageUsedEstimateCounter.RawValue = value);
			dataUnitsReadBehavior.Skip(1).Subscribe(value => dataUnitsReadCounter.RawValue = value);
			dataUnitsWriteBehavior.Skip(1).Subscribe(value => dataUnitsWriteCounter.RawValue = value);
			hostReadCommandsBehavior.Skip(1).Subscribe(value => hostReadCommandsCounter.RawValue = value);
			hostWriteCommandsBehavior.Skip(1).Subscribe(value => hostWriteCommandsCounter.RawValue = value);
			controllerBusyTimeBehavior.Skip(1).Subscribe(value => controllerBusyTimeCounter.RawValue = value);
			powerCyclesBehavior.Skip(1).Subscribe(value => powerCyclesCounter.RawValue = value);
			powerOnHoursBehavior.Skip(1).Subscribe(value => powerOnHoursCounter.RawValue = value);
			unsafeShutdownsBehavior.Skip(1).Subscribe(value => unsafeShutdownsCounter.RawValue = value);
			mediaErrorsBehavior.Skip(1).Subscribe(value => mediaErrorsCounter.RawValue = value);
			numberOfErrorInformationLogEntriesBehavior.Skip(1).Subscribe(value => numberOfErrorInformationLogEntriesCounter.RawValue = value);
			warningCompositeTemperatureTimeBehavior.Skip(1).Subscribe(value => warningCompositeTemperatureTimeCounter.RawValue = value);
			criticalCompositeTemperatureTimeBehavior.Skip(1).Subscribe(value => criticalCompositeTemperatureTimeCounter.RawValue = value);

			Moniker = moniker;
			Model = model;
			SerialNumber = serialNumber;
		}
		internal void Next(in NvmeSmartAttributes attributes)
		{
			temperatureKelvinBehavior.OnNext(attributes.Temperature);
			temperatureDegCelsiusBehavior.OnNext(ToDegCelsius(attributes.Temperature));
			availableSpareBehavior.OnNext(attributes.AvailableSpare);
			availableSpareThresholdBehavior.OnNext(attributes.AvailableSpareThreshold);
			percentageUsedEstimateBehavior.OnNext(attributes.PercentageUsedEstimate);
			dataUnitsReadBehavior.OnNext(ToLong(attributes.DataUnitsRead));
			dataUnitsWriteBehavior.OnNext(ToLong(attributes.DataUnitsWrite));
			hostReadCommandsBehavior.OnNext(ToLong(attributes.HostReadCommands));
			hostWriteCommandsBehavior.OnNext(ToLong(attributes.HostWriteCommands));
			controllerBusyTimeBehavior.OnNext(ToLong(attributes.ControllerBusyTime));
			powerCyclesBehavior.OnNext(ToLong(attributes.PowerCycles));
			powerOnHoursBehavior.OnNext(ToLong(attributes.PowerOnHours));
			unsafeShutdownsBehavior.OnNext(ToLong(attributes.UnsafeShutdowns));
			mediaErrorsBehavior.OnNext(ToLong(attributes.MediaErrors));
			numberOfErrorInformationLogEntriesBehavior.OnNext(ToLong(attributes.NumberOfErrorInformationLogEntries));
			warningCompositeTemperatureTimeBehavior.OnNext(ToLong(attributes.WarningCompositeTemperatureTime));
			criticalCompositeTemperatureTimeBehavior.OnNext(ToLong(attributes.CriticalCompositeTemperatureTime));
		}

		public string Moniker { get; }
		public string Model { get; }
		public string SerialNumber { get; }
		public IObservable<int> TemperatureKelvin => temperatureKelvinBehavior;
		public IObservable<int> TemperatureDegCelsius => temperatureDegCelsiusBehavior;
		public IObservable<int> AvailableSpare => availableSpareBehavior;
		public IObservable<int> AvailableSpareThreshold => availableSpareThresholdBehavior;
		public IObservable<int> PercentageUsedEstimate => percentageUsedEstimateBehavior;
		public IObservable<long> DataUnitsRead => dataUnitsReadBehavior;
		public IObservable<long> DataUnitsWrite => dataUnitsWriteBehavior;
		public IObservable<long> HostReadCommands => hostReadCommandsBehavior;
		public IObservable<long> HostWriteCommands => hostWriteCommandsBehavior;
		public IObservable<long> ControllerBusyTime => controllerBusyTimeBehavior;
		public IObservable<long> PowerCycles => powerCyclesBehavior;
		public IObservable<long> PowerOnHours => powerOnHoursBehavior;
		public IObservable<long> UnsafeShutdowns => unsafeShutdownsBehavior;
		public IObservable<long> MediaErrors => mediaErrorsBehavior;
		public IObservable<long> NumberOfErrorInformationLogEntries => numberOfErrorInformationLogEntriesBehavior;
		public IObservable<long> WarningCompositeTemperatureTime => warningCompositeTemperatureTimeBehavior;
		public IObservable<long> CriticalCompositeTemperatureTime => criticalCompositeTemperatureTimeBehavior;

		protected override void OnDispose(bool disposing)
		{
			temperatureKelvinBehavior.Dispose();
			temperatureDegCelsiusBehavior.Dispose();
			availableSpareBehavior.Dispose();
			availableSpareThresholdBehavior.Dispose();
			percentageUsedEstimateBehavior.Dispose();
			dataUnitsReadBehavior.Dispose();
			dataUnitsWriteBehavior.Dispose();
			hostReadCommandsBehavior.Dispose();
			hostWriteCommandsBehavior.Dispose();
			controllerBusyTimeBehavior.Dispose();
			powerCyclesBehavior.Dispose();
			powerOnHoursBehavior.Dispose();
			unsafeShutdownsBehavior.Dispose();
			mediaErrorsBehavior.Dispose();
			numberOfErrorInformationLogEntriesBehavior.Dispose();
			warningCompositeTemperatureTimeBehavior.Dispose();
			criticalCompositeTemperatureTimeBehavior.Dispose();

			temperatureKelvinCounter.Dispose();
			temperatureDegCelsiusCounter.Dispose();
			availableSpareCounter.Dispose();
			availableSpareThresholdCounter.Dispose();
			percentageUsedEstimateCounter.Dispose();
			dataUnitsReadCounter.Dispose();
			dataUnitsWriteCounter.Dispose();
			hostReadCommandsCounter.Dispose();
			hostWriteCommandsCounter.Dispose();
			controllerBusyTimeCounter.Dispose();
			powerCyclesCounter.Dispose();
			powerOnHoursCounter.Dispose();
			unsafeShutdownsCounter.Dispose();
			mediaErrorsCounter.Dispose();
			numberOfErrorInformationLogEntriesCounter.Dispose();
			warningCompositeTemperatureTimeCounter.Dispose();
			criticalCompositeTemperatureTimeCounter.Dispose();
		}
	}
}

#pragma warning restore CA1416
