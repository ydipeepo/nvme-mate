[日本語](https://github.com/ydipeepo/nvme-mate/blob/master/README.md) | [中文](https://github.com/ydipeepo/nvme-mate/blob/master/README.zh-CN.md) | English



![NvmeMate](https://github.com/ydipeepo/nvme-mate/raw/master/doc/super-ultra-great-logo.png)




[![1.0.2](https://badgen.net/github/release/ydipeepo/nvme-mate)](https://github.com/ydipeepo/nvme-mate/releases/tag/1.0.2)

## What is this

NvmeMate observes the S.M.A.R.T. attributes of NVMe connected SSDs and provides this information to Windows Performance Monitor.
It was created as a PoC minor because the standard functionality was not able to observe SSDs.
Since it uses Performance Monitor, it can be deployed on multiple Windows machines to monitor SSDs across the board.



![Screen in operation](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/nvme-mate.gif)



#### Main feature

NvmeMate provides the following S.M.A.R.T. attributes as counters to Performance Monitor:

* Temperature (K / deg. C) `1`
* Available Spare `3`
* Available Spare Threshold `4`
* Percentage Used Estimate `5`
* Data Units Read (in LBAs) `32` (*)
* Data Units Write (in LBAs) `48` (*)
* Host Read Commands `64` (*)
* Host Write Commands `80` (*)
* Controller Busy Time (in minutes) `96` (*)
* Power Cycles `112` (*)
* Power-On Hours `128` (*)
* Unsafe Shutdowns `144` (*)
* Media Errors `160` (*)
* Number of Error Information Log Entries `176` (*)
* Warning Composite Temperature Time `192`
* Critical Composite Temperature Time `196`

(*) These attributes are truncated in the upper 64 bits due to counter constraints.



#### To be implemented in the future

- Notification via Telegram BOT when any Critical Warning is/are flagged
- Obtain and set the threshold for thermal throttling via Telegram BOT
- Plot some graph on the console





## Supported environment

Requires 64-bit Windows 10 with NVMe SSD connected to run.
You also need to have the .NET 5.0 runtime installed. 
Linux is not supported.







## Quick start

These are the steps to get started quickly.



#### Download / Compile

.NET 5.0 Runtime must be installed:

* [Download .NET (microsoft.com)](https://dotnet.microsoft.com/download) (external site)

The latest compiled files can be downloaded from here:

* [NvmeMate 1.0.2 (Latest)](https://github.com/ydipeepo/nvme-mate/releases/tag/1.0.2)

Alternatively, you can compile it yourself and create the same file.
You can compile it in Visual Studio 2019 or with the following command:

```bash
git clone https://github.com/ydipeepo/nvme-mate.git .
cd ./src/Ydi.NvmeMate
dotnet build --configuration Release
```



#### How to run

Download or build and run the generated `nvme-mate-.exe`.
When a console screen appears, start Performance Monitor (run `perfmon.msc`).

![PerfMon Screen (1)](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/perfmon-1.png)

When `NVMe S.M.A.R.T.` is available as an counter category, this program is working properly.

![PerfMon Screen (2)](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/perfmon-2.png)

As long as `nvme-mate-.exe` is running, the counter will continue to be updated.



#### How to quit

When the observing is no longer needed, quit `nvme-mate.exe` directly with the `Ctrl + C` command.
The counters registered in the Performance Monitor will be automatically deleted when you quit this program.







## Arguments

If you want more detailed control, you can specify the following command line arguments.



#### `--help`

The contents listed here will be output to standard output.



#### `--lang <CULT_CODE>`

Forces the use of the language resource specified by `CULT_CODE`.
`CULT_CODE` can be one of the following:

* `en-US`: English
* `zh-CN`, `zh-Hans-CN`: Simplified Chinese
* `ja-JP`: Japanese

If not specified, it will use the system language by default, or English if there is no language resource corresponding to the system language.



#### `--interval <MSEC>`

Specifies the interval in milliseconds at which the S.M.A.R.T. attribute is observed and the counter is updated.
If not specified, it will be 1 second (`--interval 1000`).



#### `--plot`

Make the S.M.A.R.T. attribute plot to the console as well.



#### `--scan`

It only scans the physical drive(s) and quits immediately.



#### `--scan-range <END>`

Specifies the detection range of the physical drive(s).
The range `[0, END)` will be the target of the detection.



#### `--skip <PHYS_DRV_ID>`

Ignore the physical drive with the number specified by `PHYS_DRV_ID`.
You can specify multiple numbers separated by commas `,`.

E.g: `--ignore 0,1,2` will ignore the 0th, 1st, and 2nd physical drives.



#### `--hydra`

Do not delete the counters registered in Performance Monitor when you quit this program.



#### `--clean`

Deletes all counters registered in Performance Monitor and quits as is.
This is used when counters remain in the Performance Monitor due to some abnormal termination.
This argument takes precedence over any other argument.







## Other

I found more detailed pages about some of the words mentioned in the text, so I've included links to them. (These are all external sites).



#### About S.M.A.R.T.

You can read more about it here:

* [Common SMART Attributes for Client Intel(R) SSD's and Intel(R) Optane(TM) Technology Products (intel.com)](https://www.intel.com/content/www/us/en/support/articles/000056596/memory-and-storage.html)
* [Technical Note: NVMe Basic Management Command](https://www.nvmexpress.org/wp-content/uploads/NVMe_Management_-_Technical_Note_on_Basic_Management_Command.pdf)


#### About Windows Performance Monitor

You can read more about it here:

* [Windows Performance Monitor (forsenergy.com)](https://forsenergy.com/en-us/perfmon/html/53582ab0-24a0-411c-9c7a-7b2466741699.htm)







## License

Created by Ydi ([@ydipeepo](https://twitter.com/ydipeepo)) and released under BSD-3-Clause. Please see LICENSE.md for details.

It complies with the license, but is intended to work together in an environment that handles digital assets.
Please note the following:

> Ydi is not responsible for the distribution of software from sources other than https://github.com/ydipeepo/nvme-mate/releases .
> If you need to download this program from a non-official source, please do so with extreme caution and at your own risk.
> If you are unsure about the official binary, you can check the source code and build it yourself.






If you like it, please donate to `xch186mwc4aqc608a9nru8308ww0eftnp6panzyjlpqrfnvnnu8v4cjq8ely47`.
Then I will be very happy. If you would like to donate, please let Ydi ([@ydipeepo](https://twitter.com/ydipeepo)) know.
