[日本語](https://github.com/ydipeepo/nvme-mate/blob/master/README.md) | 中文 | [English](https://github.com/ydipeepo/nvme-mate/blob/master/README.en-US.md)



![NvmeMate](https://github.com/ydipeepo/nvme-mate/raw/master/doc/super-ultra-great-logo.png)




[![1.0.1](https://badgen.net/github/release/ydipeepo/nvme-mate)](https://github.com/ydipeepo/nvme-mate/releases/tag/1.0.1)

## 这是什么

NvmeMate监视通过NVMe连接的SSD的S.M.A.R.T.属性，并将该信息提供给Windows性能监视器。
因为标准功能无法监视SSD，所以是为了PoC矿工而制作的。
因为使用了性能监视器，如果导入多个Windows机器，可以横断监视SSD。




![动作中的截图](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/nvme-mate.gif)



#### 功能

NvmeMate将以下SMART属性作为计数器提供给性能监视器：

* Temperature (K / deg. C) `1`
* Available Spare `3`
* Available Spare Threshold `4`
* Percentage Used Estimate `5`
* Data Units Read (in LBAs) `32` ※
* Data Units Write (in LBAs) `48` ※
* Host Read Commands `64` ※
* Host Write Commands `80` ※
* Controller Busy Time (in minutes) `96` ※
* Power Cycles `112` ※
* Power-On Hours `128` ※
* Unsafe Shutdowns `144` ※
* Media Errors `160` ※
* Number of Error Information Log Entries `176` ※
* Warning Composite Temperature Time `192`
* Critical Composite Temperature Time `196`

※ 这些属性根据计数器的限制截断了前64位。



#### 计划将来实现

- 当报告Critical Warning时，通过Telegram BOT通知
- 通过Telegram BOT获取、设定热敏插槽环的阈值
- 绘制图表





## 工作环境

为了使其运行，需要连接NVMe SSD的64位Windows 10。
此外，必须安装.NET5.0的运行时间。
不支持Linux。







## 快速入门

这是快速开始的步骤。



#### 下载或编译

必须安装.NET5.0 Runtime：

* [Download .NET (microsoft.com)[https://dotnet.microsoft.com/download] (外部网站)

最新的文件可以从这里下载：

* [NvmeMate 1.0.1 (最新)](https://github.com/ydipeepo/nvme-mate/releases/tag/1.0.1)


或者，也可以自己编译并创建同一个文件。
Visual Studio 2019使用以下命令编译：

```bash
git clone https://github.com/ydipeepo/nvme-mate.git .
cd ./src/Ydi.NvmeMate
dotnet build --configuration Release
```



#### 怎么开始

执行下载或编译后生成的`nvme-mate.exe`。
如果出现一个控制台画面，就那样启动性能监视器（执行`perfmon.msc`）。

![PerfMon 截图 (1)](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/perfmon-1.png)

如果追加了`NVMe S.M.A.R.T.`作为可使用的计数器，则正常运作。

![PerfMon 截图 (2)](https://raw.githubusercontent.com/ydipeepo/nvme-mate/master/doc/perfmon-2.png)

只要执行了`nvme-mate.exe`，计数器就会继续更新。



#### 怎么关闭

不需要监视时，在`nvme-mate.exe`上输入`Ctrl + C`后直接关闭。
性能监视器中注册的计数器在结束时会自动删除。







## 命令行参数

如果想要更详细地控制，可以指定以下命令行参数。



#### `--help`

这里记载的内容输出到标准输出。



#### `--lang <CULT_CODE>`

强制使用`CULT_CODE`指定的语言资源。
`CULT_CODE`指定以下哪一个：

* `en-US`: 英语
* `zh-CN`, `zh-Hans-CN`: 简体中文
* `ja-JP`: 日语

如果没有指定，则默认使用系统语言，如果没有系统语言对应的语言资源，则使用英语。



#### `--interval <MSEC>`

毫秒指定监视S.M.A.R.T.属性并更新计数器的间隔。
未指定时为1秒（`--interval 1000`）。



#### `--plot`

在控制台上绘制S.M.A.R.T.属性。



#### `--scan`

只检测物理驱动器，然后关闭。



#### `--scan-range <END>`

指定物理驱动器的检测范围。
`[0，END)` 的范围为扫描对象。



#### `--skip <PHYS_DRV_ID>`

忽略`PHYS_DRV_ID`中指定编号的物理驱动器。
可以用逗号`,`指定多个分隔符。

例：如果是`--ignore 0,1,2`则忽略0，1，2个物理驱动器。



#### `--hydra`

关闭时不删除注册在性能监视器上的计数器。



#### `--clean`

将性能监视器上注册的计数器全部删除后就结束了。
在某些异常结束后，性能监视器中残留计数器时使用。
此外，该参数优先于其他参数。







## 其他

关于文章中记载的几个单词，有更详细的页面，所以先刊登链接。（全部是外部网站）



#### 关于S.M.A.R.T.

熟悉此页面：

* [客户端英特尔(R)固态盘和英特尔(R)傲腾(TM)技术产品的常见智能属性 (intel.com)](https://www.intel.com/content/www/cn/zh/support/articles/000056596/memory-and-storage.html)
* [Technical Note: NVMe Basic Management Command](https://www.nvmexpress.org/wp-content/uploads/NVMe_Management_-_Technical_Note_on_Basic_Management_Command.pdf)


#### 关于Windows性能监视器

熟悉此页面：

* [Windows 性能监视器 (forsenergy.com)](https://forsenergy.com/zh-cn/perfmon/html/53582ab0-24a0-411c-9c7a-7b2466741699.htm)







## 许可

由Ydi([@ydipeepo](https://twitter.com/ydipeepo))制作，并在BSD-3-Clause下公开。详情请看另外附带的LICENSE.md。

虽然遵循许可，但是为了在直接处理数字资产的环境下工作而制作的。
请在补充注意以下一点的同时理解：

> 关于从 https://github.com/ydipeepo/nvme-mate/releases 以外散发的东西，Ydi完全不相关。
> 如果需要从官方以外的地方下载的话，请小心，并在自己的责任下进行管理。
> 如果您不能信任已编译的二进制文件，您可以自行确认源代码并建立。







如果您喜欢的话，我很高兴您能捐款到`xch186mwc4aqc608a9nru8308ww0eftnp6panzyjlpqrfnvnnu8v4cjq8ely47`。
到时候如果不碍事的话，请通知Ydi ([@ydipeepo](https://twitter.com/ydipeepo))。

