# WindowsDnsSwitcher

[English](./README.md)

WindowsDnsSwitcher 是一个轻量级 Windows DNS 模式切换工具，用于在“自动获取 DNS”和“用户自定义 IPv4 DNS”之间快速切换。

## 截图

> 截图稍后添加。

## 功能特性

- 在自动 DNS 模式和自定义 IPv4 DNS 模式之间切换。
- 在本地保存自定义 DNS 设置。
- 刷新 Windows DNS 缓存。
- 查看所选网卡信息。
- 打开 Windows 网络连接。
- 使用默认浏览器打开常见网络诊断网站。
- 只针对所选网卡的 Windows IPv4 DNS 设置。
- 支持在英文和简体中文界面之间切换。

## 适用场景

- 公共 Wi-Fi。
- 网页登录认证网络。
- DNS 解析排查。
- 临时网络诊断。
- 开发过程中的网络配置测试。
- 需要频繁切换 DNS 配置的 Windows 使用场景。

## 下载与安装

预构建版本会在后续添加。目前可以先从源码构建。

后续发布版本可用后：

1. 从本仓库下载发布包。
2. 解压压缩包。
3. 需要修改 DNS 设置时，以管理员身份运行程序。
4. 选择目标网卡。
5. 按需在自动 DNS 和自定义 IPv4 DNS 之间切换。

## 使用说明

1. 选择目标网卡。
2. 使用“自动 DNS 模式”恢复网络自动分配的 DNS。
3. 使用“自定义 DNS 模式”应用已保存的自定义 IPv4 DNS。
4. 使用“设置自定义 DNS”配置首选 DNS 和可选备用 DNS。
5. 使用“刷新 DNS”刷新 Windows DNS 缓存。
6. 仅在需要手动查看网络信息时使用网络诊断网站入口。
7. 使用语言选择框在英文和简体中文界面之间切换。

## 管理员权限说明

修改 Windows 网卡 DNS 设置需要管理员权限，因此切换 DNS 时需要以管理员身份运行。

## 隐私与第三方检测网站说明

- 工具只在本地保存自定义 DNS 设置。
- 网络诊断按钮只会调用默认浏览器打开第三方网站。
- 第三方诊断网站可能看到你的公网 IP 和浏览器或网络信息。
- 本工具不会抓取、解析、上传或保存第三方网站的诊断结果。
- 使用第三方诊断网站前，请自行查看其隐私政策。

## 安全提示

- 只从官方仓库或官方 Releases 页面下载。
- 如有需要，构建前可以先审查源码。
- 不要提交包含私人 DNS 设置或个人信息的本地配置文件。
- 只在自己有权限管理的设备和网络上使用。
- 修改 DNS 可能影响网络访问；如果网络不可用，可以恢复“自动 DNS 模式”。

## 本工具不做什么

本工具只修改所选 Windows 网卡的 IPv4 DNS 设置，不提供流量转发服务，不创建网络隧道，不控制应用流量路由，不修改 IPv6 配置，也不会收集或上传诊断结果。

## 从源码构建

环境要求：

- Windows 10/11。
- .NET SDK 10.0，或与项目文件兼容的 SDK 版本。

还原并构建：

```powershell
dotnet restore
dotnet build WindowsDnsSwitcher.slnx
```

运行 WinForms 应用：

```powershell
dotnet run --project src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj
```

发布 Windows x64 自包含构建：

```powershell
dotnet publish src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj -c Release -r win-x64 --self-contained true
```

也可以使用项目内构建脚本：

```bat
build.bat
```

构建脚本会发布到 `dist/`。发布包不会提交到仓库。

## 配置说明

本地设置保存在应用程序 exe 同级目录的 `config.json` 文件中。真实本地配置文件不应提交到仓库。

`config.example.json` 仅用于展示配置结构。

配置字段：

- `selectedAdapter`：上次选择的网卡名称。
- `primaryDns`：已保存的首选自定义 IPv4 DNS。
- `secondaryDns`：可选备用自定义 IPv4 DNS。
- `showAllAdapters`：是否显示更多网卡。
- `language`：`en` 或 `zh-CN`。

## 许可证

本项目使用 GNU General Public License v3.0。

官方版本只从本仓库提供。

## 官方版本说明

目前尚未发布官方预构建版本。请不要从非官方来源下载二进制文件。
