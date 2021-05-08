using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ydi.NvmeMate
{
	static class Program
	{
		static Task<int> CleanAsync(
			StringLocalizer stringLocalizer,
			InvocationContext invocationContext,
			CancellationToken _)
		{
			var console = invocationContext.Console;
			var reporter = new NvmeSmartAttributesReporter(stringLocalizer);

			try
			{
				try
				{
					console.Out.Write(stringLocalizer["LOG_UNREGISTERING_PERFORMANCE_COUNTERS"]);
					reporter.DeleteCategory();
				}
				finally
				{
					console.Out.Write(stringLocalizer["LOG_QUITTING"]);
				}
			}
			catch (Exception exception)
			{
				console.Error.Write($"{stringLocalizer["ERROR"]}: {exception}\n");
				return Task.FromResult(1);
			}
			return Task.FromResult(0);
		}
		static async Task<int> WatchAsync(
			StringLocalizer stringLocalizer,
			InvocationContext invocationContext,
			CancellationToken terminationToken,
			uint interval,
			bool plot,
			bool scan,
			byte scanRange,
			byte[] skip,
			bool hydra)
		{
			var console = new VirtualTerminal(invocationContext.Console, VirtualTerminalMode.TryEnable());
			var reporter = new NvmeSmartAttributesReporter(stringLocalizer);
			var attributesProviderMap = new Dictionary<int, NvmeSmartAttributesProvider>();

			try
			{
				try
				{
					if (!scan)
					{
						console.Out.Write(stringLocalizer["LOG_REGISTERING_PERFORMANCE_COUNTERS"]);
						reporter.CreateCategory();
					}

					console.Out.Write(stringLocalizer["LOG_DETECTING_NVME_DRIVES"]);
					for (ushort driveNumber = 0; driveNumber < scanRange; ++driveNumber)
					{
						if (skip.Contains((byte)driveNumber))
						{
							continue;
						}
						var attributesProvider = NvmeSmartAttributesProvider.Create((byte)driveNumber);
						if (attributesProvider == null)
						{
							continue;
						}
						if (attributesProvider.Query(out var _))
						{
							attributesProviderMap.Add(driveNumber, attributesProvider);
							if (!scan)
							{
								reporter.AddInstance(attributesProvider.Moniker, attributesProvider.Model, attributesProvider.SerialNumber);
							}
							console.Out.Write($"\t{attributesProvider.Moniker.Replace("_", string.Empty)}: {attributesProvider.Model ?? stringLocalizer["NOT_AVAILABLE"]} (S/N: {attributesProvider.SerialNumber ?? stringLocalizer["NOT_AVAILABLE"]})\r\n");
						}
						else
						{
							attributesProvider.Dispose();
						}
					}
					if (attributesProviderMap.Count == 0)
					{
						console.Out.Write(stringLocalizer["LOG_CANNOT_DETECT_ANY_NVME_DRIVES"]);
						return 1;
					}
					if (scan)
					{
						return 0;
					}

					console.Out.Write(stringLocalizer["LOG_START_MONITORING"]);
					if (plot)
					{
						if (console is ITerminal terminal)
						{
							terminal.Clear();
							terminal.SetCursorPosition(0, 0);
							terminal.HideCursor();
						}

						var renderer = new ConsoleRenderer(console, invocationContext.BindingContext.OutputMode(), true);

						var table = new TableView<NvmeSmartAttributesReporterInstance>() { Items = reporter.Instances };
						table.AddColumn(item => item.Moniker.Replace("_", string.Empty), new ContentView("#".Underline()));
						table.AddColumn(item => item.Model, new ContentView("Model".Underline()));
						table.AddColumn(item => item.SerialNumber, new ContentView("S/N".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.PercentageUsedEstimate, value => $"{value}%"), new ContentView("PUE".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.TemperatureDegCelsius, value => $"{value}°C"), new ContentView("CTemp".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.WarningCompositeTemperatureTime, value => $"{value} mins."), new ContentView("WCTemp".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.CriticalCompositeTemperatureTime, value => $"{value} mins."), new ContentView("CCTemp".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.HostReadCommands), new ContentView("HRC".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.HostWriteCommands), new ContentView("HWC".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.DataUnitsRead), new ContentView("DUR".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.DataUnitsWrite), new ContentView("DUW".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.MediaErrors), new ContentView("ME".Underline()));
						table.AddColumn(item => ContentView.FromObservable(item.NumberOfErrorInformationLogEntries), new ContentView("EE".Underline()));

						var screen = new ScreenView(renderer, console) { Child = table };
						var region = new Region(0, 0, Console.WindowWidth, Console.WindowHeight, true);
						screen.Render(region);
					}
					while (!terminationToken.IsCancellationRequested)
					{
						foreach (var driveNumber in attributesProviderMap.Keys)
						{
							var attributesProvider = attributesProviderMap[driveNumber];
							if (!attributesProvider.Query(out var attributes))
							{
								console.Error.Write($"\t{attributesProvider.Moniker}: {stringLocalizer["LOG_FAILED_TO_QUERY_SMART"]}\n");
								continue;
							}
							reporter.Next(attributesProvider.Moniker, attributes);
						}
						await Task.Delay((int)interval, terminationToken);
					}
				}
				finally
				{
					if (plot)
					{
						if (console is ITerminal terminal)
						{
							terminal.Clear();
							terminal.SetCursorPosition(0, 0);
							terminal.ShowCursor();
						}
					}
					console.Out.Write(stringLocalizer["LOG_QUITTING"]);
					if (!hydra)
					{
						reporter.DeleteCategory();
					}
					reporter.Dispose();
					foreach (var attributesProvider in attributesProviderMap.Values)
					{
						attributesProvider.Dispose();
					}
				}
			}
			catch (TaskCanceledException)
			{
			}
			return 0;
		}
		public static Task<int> Main(string[] args)
		{
			using var globalMutex = new Mutex(false, $"Global\\Ydi.NvmeMate");

			var stringLocalizer = new StringLocalizer();

			if (!globalMutex.WaitOne(0, false))
			{
				throw new InvalidOperationException(stringLocalizer["ERROR_ALREADY_RUNNING"]);
			}
			if (!OperatingSystem.IsWindows())
			{
				throw new NotSupportedException(stringLocalizer["ERROR_UNSUPPORTED_OPERATING_SYSTEM"]);
			}
			if (!Environment.Is64BitOperatingSystem)
			{
				throw new NotSupportedException(stringLocalizer["ERROR_UNSUPPORTED_BECAUSE_NOT_X64"]);
			}

			var rootCommand = new RootCommand
			{
				new Option<string>("--lang", getDefaultValue: () => "ja-JP", description: stringLocalizer["ARG_LANG_HELP"]),
				new Option<uint>("--interval", description: stringLocalizer["ARG_INTERVAL_HELP"]),
				new Option<bool>("--plot", stringLocalizer["ARG_PLOT_HELP"]),
				new Option<bool>("--scan", stringLocalizer["ARG_SCAN_HELP"]),
				new Option<byte>("--scan-range", () => 8, stringLocalizer["ARG_SCAN_RANGE_HELP"]),
				new Option<byte[]>("--skip", stringLocalizer["ARG_SKIP_HELP"]),
				new Option<bool>("--hydra", stringLocalizer["ARG_HYDRA_HELP"]),
				new Option<bool>("--clean", stringLocalizer["ARG_CLEAN_HELP"]),
			};
			rootCommand.Description = stringLocalizer["DESCRIPTION"];
			rootCommand.Handler = CommandHandler.Create((
				string lang,
				uint interval,
				bool plot,
				bool scan,
				byte scanRange,
				byte[] skip,
				bool hydra,
				bool clean,
				InvocationContext invocationContext,
				CancellationToken terminationToken) =>
				{
					stringLocalizer = new StringLocalizer(lang);
					return clean
						? CleanAsync(stringLocalizer, invocationContext, terminationToken)
						: WatchAsync(stringLocalizer, invocationContext, terminationToken, interval, plot, scan, scanRange, skip, hydra);
				});
			return rootCommand.InvokeAsync(args);
		}
	}
}
