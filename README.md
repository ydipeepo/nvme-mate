



![NVMeMate](C:\Users\Ydi\Desktop\nvme-mate\doc\super-ultra-great-logo.png)





## 何これ

NVMeMate は NVMe SSD の S.M.A.R.T. 属性を監視し、その情報を Windows Performance Monitor に提供します。
標準の機能では SSD を監視できなかったので PoC マイナーのため作成しました。
Performance Monitor を使用しているため複数の Windows マシンに導入すれば横断的に SSD を監視できます。



![キャプチャ](C:\Users\Ydi\Desktop\nvme-mate\doc\nvme-mate.gif)



#### 主な機能

NVMeMate は次の S.M.A.R.T. 属性をカウンターとして Performance Monitor に提供します:

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

※ これらの属性はカウンタの制約により上位 64 ビットを切り捨てています。



#### 将来実装を予定しています

- Critical Warning が有効になった場合に Telegram BOT を通じて通知
- サーマルスロットリングの閾値を Telegram BOT を通じて取得・設定
- グラフもプロットする





## 動作環境

動作させるためには NVMe SSD が接続された 64 ビット Windows 10 が必要です。
Linux には対応していません。







## クイックスタート

手っ取り早く始めるための手順です。



#### ダウンロード

最新のビルドはこちらからダウンロードできます:

* nvme-mate-0.1.0-2021050601.zip (0.1.0, 最新)

または、自身でビルドし同じファイルを作成することも可能です。
Visual Studio 2019 上もしくは以下のコマンドでのビルドに対応しています:

```bash
git clone https://github.com/ydipeepo/nvme-mate.git .
cd ./NVMeMate
dotnet build --configuration Release
```



#### 起動するには

ダウンロードもしくはビルドし生成された `nvme-mate.exe` を実行します。
コンソール画面が一つ現れたらそのまま Performance Monitor を起動 (`perfmon.msc` を実行) します。

![PerfMon (1)](C:\Users\Ydi\Desktop\nvme-mate\doc\perfmon-1.png)

使用可能なカウンターとして `NVMe S.M.A.R.T.` が追加されていれば正常に動作しています。

![PerfMon (2)](C:\Users\Ydi\Desktop\nvme-mate\doc\perfmon-2.png)

`nvme-mate.exe` が実行されている限りカウンタは更新され続けます。



#### 終了するには

監視が不要になったときは `nvme-mate.exe` を `Ctrl + C` などで直接終了します。
Performance Monitor に登録されたカウンタは終了時に自動的に削除されます。







## コマンドライン引数

より細かく NVMeMate を制御したい場合、以下のコマンドライン引数を指定できます。



#### `--help`

ここに記載されている内容が標準出力に出力されます。



#### `--lang <CULT_CODE>`

`CULT_CODE` で指定した言語リソースを使用するよう強制します。
`CULT_CODE` は以下のどれかを指定します。

* `en-US`: English
* `zh-CN`, `zh-Hans-CN`: 简体中文
* `ja-JP`: 日本語

指定しない場合はデフォルトでシステム言語を、システム言語に対応する言語リソースが存在しなければ英語を使用します。



#### `--interval <MSEC>`

S.M.A.R.T. 属性を監視しカウンタを更新する間隔をミリ秒で指定します。
指定しない場合は 1 秒 (`--interval 1000`) となります。



#### `--plot`

S.M.A.R.T. 属性をコンソールにプロットするようにします。



#### `--scan`

物理ドライブのスキャンのみを行いそのまま終了します。



#### `--scan-range <END>`

物理ドライブのスキャン範囲を指定します。
`[0, END)` の範囲がスキャンの対象になります。



#### `--skip <PHYS_DRV_ID>`

`PHYS_DRV_ID` で指定した番号の物理ドライブを無視します。
カンマで区切り複数指定できます。

例: `--ignore 0,1,2` とすると 0, 1, 2 番目の物理ドライブを無視します。



#### `--hydra`

終了時に Performance Monitor に登録したカウンターを削除しないようにします。



#### `--clean`

Performance Monitor に登録したカウンターをすべて削除しそのまま終了します。
何らかの異常終了などで Performance Monitor にカウンタが残ってしまった場合に使用します。
なお、この引数は他のどの引数よりも優先します。







## その他

文章中に記載のあるいくつかの単語についてより詳しいページがありましたのでリンクを掲載しておきます。(すべて外部のサイトです)



#### S.M.A.R.T. について

こちらのページが詳しいです:

* [Common SMART Attributes for Client Intel(R) SSD's and Intel(R) Optane(TM) Technology Products (intel.com)](https://www.intel.com/content/www/us/en/support/articles/000056596/memory-and-storage.html)
* [Technical Note: NVMe Basic Management Command](https://www.nvmexpress.org/wp-content/uploads/NVMe_Management_-_Technical_Note_on_Basic_Management_Command.pdf)


#### Windows Performance Monitor について

こちらのページが詳しいです:

* [Windows パフォーマンス モニター (forsenergy.com)](https://forsenergy.com/ja-jp/perfmon/html/53582ab0-24a0-411c-9c7a-7b2466741699.htm)







## ライセンス

Ydi ([@ydipeepo](https://twitter.com/ydipeepo)) が作成し、BSD-3-Clause の下公開しています。詳しくは別途付属の LICENSE.md をご覧ください。

ライセンスに準拠しますが、その上で直接デジタル資産を扱う環境で動作することを目的に作ったものです。
以下一点だけ追加の注意とともにご理解くださいますようお願いいたします:

> https://github.com/ydipeepo/nvme-mate 以外から配布されているものについて Ydi は一切関知しておりません。
> 必要な場合は細心の注意を払いご自身の責任のもと管理実行してください。
> 公式ビルド済みバイナリが不安な場合はご自身でソースコードを確認しビルドいただくことも可能です。







もし気に入っていただけましたら、`xch186mwc4aqc608a9nru8308ww0eftnp6panzyjlpqrfnvnnu8v4cjq8ely47` へ
ご寄付いただけますと喜びます。その際は差支えなければ Ydi ([@ydipeepo](https://twitter.com/ydipeepo)) までご一報ください。

