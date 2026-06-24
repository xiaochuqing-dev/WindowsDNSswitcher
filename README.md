# WindowsDnsSwitcher

`WindowsDnsSwitcher` 是一个 Windows 桌面小工具，用于把选中的物理 WLAN/Wi-Fi 网卡在自动获取 DNS 和已保存的自定义 DNS 之间切换。

## 作用

- 自动 DNS 模式：把选中网卡的 IPv4 DNS 改为自动获取。
- 自定义 DNS 模式：把选中网卡的 IPv4 DNS 改为用户保存的 IPv4 DNS。
- 刷新 DNS：执行 `ipconfig /flushdns`。
- 不修改 IP 地址、不修改 IPv6、不启动或关闭代理软件、不修改系统代理。

## 如何运行

开发运行需要 .NET SDK 10 或兼容版本：

```bat
dotnet run --project src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj
```

程序需要管理员权限。发布后的 exe 也会自动请求管理员权限。

## 如何发布

运行：

```bat
build.bat
```

发布输出在：

```text
dist
```

## 复制到另一台电脑

把整个 `dist` 文件夹复制到另一台 Windows x64 电脑，运行里面的 `WindowsDnsSwitcher.exe`。目标电脑不需要预先安装 .NET Runtime。

## 常见问题

- 找不到网卡：点击 `显示所有网卡` 或 `刷新网卡列表`，确认选择真实 WLAN/Wi-Fi 网卡。
- 选到 TUN/虚拟网卡：不建议修改 `singbox_tun`、`xray_tun`、`Wintun`、`TAP` 等虚拟网卡。
- 操作失败：确认程序以管理员权限运行，并确认网卡名称没有被 Windows 网络设置页面改动。
- IPv6 泄露：本工具只显示 IPv6 状态，不自动关闭或恢复 IPv6。


