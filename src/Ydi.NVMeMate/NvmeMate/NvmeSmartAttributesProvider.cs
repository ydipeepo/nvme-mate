using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.IO;
using System.Management;
using static System.Runtime.InteropServices.Marshal;

namespace Ydi.NvmeMate
{
	public sealed class NvmeSmartAttributesProvider : DisposableBase
	{
		readonly SafeFileHandle hDrive;
		readonly uint cbStoragePropertyQueryLength;
		readonly uint cbStorageProtocolSpecificDataLength;
		readonly uint cbStorageProtocolSpecificDataOffset;
		readonly uint cbStorageProtocolDataDescriptorLength;
		readonly uint cbNvmeHealthInfoLogLength;
		readonly uint cbNvmeHealthInfoLogOffset;
		readonly uint cbBuffer;
		readonly IntPtr lpStoragePropertyQuery;
		readonly IntPtr lpStorageProtocolSpecificData;
		readonly IntPtr lpStorageProtocolDataDescriptor;
		readonly IntPtr lpNvmeHealthInfoLog;
		readonly byte[] lpBuffer;

		NvmeSmartAttributesProvider(SafeFileHandle hDrive, string deviceId, string moniker)
		{
			try
			{
#pragma warning disable CA1416
				using var searcher = new ManagementObjectSearcher($"SELECT Model, SerialNumber FROM Win32_DiskDrive WHERE DeviceID=\"{deviceId.Replace("\\", "\\\\")}\"");
				foreach (var item in searcher.Get())
				{
					SerialNumber = item["SerialNumber"].ToString().Replace("_", string.Empty).Replace(".", string.Empty);
					Model = item["Model"].ToString();
					break;
				}
#pragma warning restore CA1416
			}
			catch
			{
			}
			Moniker = moniker;
			DeviceId = deviceId;

			this.hDrive = hDrive;

			cbStoragePropertyQueryLength = (uint)SizeOf<STORAGE_PROPERTY_QUERY>();
			cbStorageProtocolSpecificDataLength = (uint)SizeOf<STORAGE_PROTOCOL_SPECIFIC_DATA>();
			cbStorageProtocolSpecificDataOffset = (uint)OffsetOf<STORAGE_PROPERTY_QUERY>("AdditionalParameters").ToInt32();
			cbStorageProtocolDataDescriptorLength = (uint)SizeOf<STORAGE_PROTOCOL_DATA_DESCRIPTOR>();
			cbNvmeHealthInfoLogLength = (uint)SizeOf<NVME_HEALTH_INFO_LOG>();
			cbNvmeHealthInfoLogOffset = (uint)OffsetOf<STORAGE_PROTOCOL_DATA_DESCRIPTOR>("ProtocolSpecificData").ToInt32();
			cbBuffer = cbStorageProtocolSpecificDataOffset + cbStorageProtocolSpecificDataLength + Constants.NVME_MAX_LOG_SIZE;
			lpStoragePropertyQuery = AllocHGlobal((int)cbStoragePropertyQueryLength);
			lpStorageProtocolSpecificData = AllocHGlobal((int)cbStorageProtocolSpecificDataLength);
			lpStorageProtocolDataDescriptor = AllocHGlobal((int)cbStorageProtocolDataDescriptorLength);
			lpNvmeHealthInfoLog = AllocHGlobal((int)cbNvmeHealthInfoLogLength);
			lpBuffer = new byte[cbBuffer];

			var storagePropertyQuery = new STORAGE_PROPERTY_QUERY
			{
				PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProtocolSpecificProperty,
				QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery,
			};

			var storageSpecificData = new STORAGE_PROTOCOL_SPECIFIC_DATA
			{
				ProtocolType = STORAGE_PROTOCOL_TYPE.ProtocolTypeNvme,
				DataType = (uint)STORAGE_PROTOCOL_NVME_DATA_TYPE.NVMeDataTypeLogPage,
				ProtocolDataRequestValue = (uint)NVME_LOG_PAGES.NVME_LOG_PAGE_HEALTH_INFO,
				ProtocolDataOffset = cbStorageProtocolSpecificDataLength,
				ProtocolDataLength = cbNvmeHealthInfoLogLength,
			};

			StructureToPtr(storagePropertyQuery, lpStoragePropertyQuery, false);
			StructureToPtr(storageSpecificData, lpStorageProtocolSpecificData, false);
		}

		public string Moniker { get; }
		public string DeviceId { get; }
		public string Model { get; }
		public string SerialNumber { get; }

		public static NvmeSmartAttributesProvider Create(byte driveNumber)
		{
			var deviceId = @$"\\.\PhysicalDrive{driveNumber}";
			var hDrive = Kernel32.CreateFile(
				deviceId,
				0,
				0,
				IntPtr.Zero,
				FileMode.Open,
				FileAttributes.Normal,
				IntPtr.Zero);
			return hDrive.IsInvalid
				? null
				: new NvmeSmartAttributesProvider(hDrive, deviceId, $"_{driveNumber}");
		}

		public bool Query(out NvmeSmartAttributes nvmeSmart)
		{
			Copy(
				lpStoragePropertyQuery,
				lpBuffer,
				0,
				(int)cbStoragePropertyQueryLength);
			Copy(
				lpStorageProtocolSpecificData,
				lpBuffer,
				(int)cbStorageProtocolSpecificDataOffset,
				(int)cbStorageProtocolSpecificDataLength);

			var result = Kernel32.DeviceIoControl(
				hDrive,
				Constants.IOCTL_STORAGE_QUERY_PROPERTY,
				lpBuffer,
				cbBuffer,
				lpBuffer,
				cbBuffer,
				out var returnedLength,
				IntPtr.Zero);
			if (!result || returnedLength == 0)
			{
				var errorCode = GetLastWin32Error();
				if (errorCode != Constants.ERROR_INVALID_PARAMETER)
				{
					throw new Win32Exception(errorCode);
				}
				nvmeSmart = default;
				return false;
			}

			Copy(
				lpBuffer,
				0,
				lpStorageProtocolDataDescriptor,
				(int)cbStorageProtocolDataDescriptorLength);

			var storageProtocolDataDescriptor = PtrToStructure<STORAGE_PROTOCOL_DATA_DESCRIPTOR>(lpStorageProtocolDataDescriptor);
			if (storageProtocolDataDescriptor.Version != cbStorageProtocolDataDescriptorLength ||
				storageProtocolDataDescriptor.Size != cbStorageProtocolDataDescriptorLength)
			{
				throw new BadImageFormatException($"Critical: invalid data descriptor header. {{Version={storageProtocolDataDescriptor.Version}, Size={storageProtocolDataDescriptor.Size}}}");
			}

			var storageProtocolSpecificData = storageProtocolDataDescriptor.ProtocolSpecificData;
			if (storageProtocolSpecificData.ProtocolDataOffset < cbStorageProtocolSpecificDataLength ||
				storageProtocolSpecificData.ProtocolDataLength < cbNvmeHealthInfoLogLength)
			{
				throw new BadImageFormatException($"Critical: invalid protocol data offset/length. {{Offset={storageProtocolSpecificData.ProtocolDataOffset}, Length={storageProtocolSpecificData.ProtocolDataLength}}}");
			}

			Copy(
				lpBuffer,
				(int)(cbNvmeHealthInfoLogOffset + storageProtocolSpecificData.ProtocolDataOffset),
				lpNvmeHealthInfoLog,
				(int)cbNvmeHealthInfoLogLength);
			var nvmeHealthInfoLog = PtrToStructure<NVME_HEALTH_INFO_LOG>(lpNvmeHealthInfoLog);

			nvmeSmart = new(nvmeHealthInfoLog);
			return true;
		}
		protected override void OnDispose(bool disposing)
		{
			FreeHGlobal(lpStoragePropertyQuery);
			FreeHGlobal(lpStorageProtocolSpecificData);
			FreeHGlobal(lpStorageProtocolDataDescriptor);
			FreeHGlobal(lpNvmeHealthInfoLog);
			hDrive.Dispose();
		}
	}
}
