# WindowsDnsSwitcher Design

## Goal

Build a small Windows desktop GUI tool named `WindowsDnsSwitcher` that switches the selected physical WLAN/Wi-Fi adapter's IPv4 DNS between:

- Automatic DNS mode: automatic DNS through DHCP.
- Custom DNS mode: static DNS `saved custom IPv4 DNS`.

The app does not change IP address settings, IPv6 settings, application routing, or system-wide traffic forwarding settings.

## Platform

- C# WinForms.
- .NET 10 Windows target (`net10.0-windows`) because .NET 10 is the current LTS release on 2026-06-23.
- Windows x64 self-contained folder publishing through `build.bat`.

## Architecture

The project is split into three parts:

- `WindowsDnsSwitcher.Core`: testable logic for adapter classification, DNS mode parsing, command construction, config persistence, and command execution boundaries.
- `WindowsDnsSwitcher.WinForms`: the desktop UI and event handlers.
- `WindowsDnsSwitcher.Tests`: a no-dependency console test runner that exercises core behavior without requiring NuGet test packages.

The UI calls core services instead of embedding parsing and classification logic directly in form event handlers.

## UI

The main window uses a restrained utility layout:

- Title: `Windows DNS 模式切换工具`.
- Short instruction line: `在“自动获取 DNS”和“自定义 DNS”之间快速切换。`
- Selected adapter label.
- Adapter combo box, `刷新网卡列表` button, and `显示所有网卡` checkbox.
- Current mode label.
- IPv6 status label.
- Two large side-by-side mode buttons:
  - Left, light blue: `自动 DNS 模式` / `自动 DNS`.
  - Right, dark blue: `自定义 DNS 模式` / `使用已保存 DNS`.
- Small helper buttons: `刷新 DNS`, `打开网络连接`, `打开 BrowserLeaks`, `打开 ipleak`, `查看网卡信息`.
- Bottom recent-log area with short operation messages.

The UI stays compact and operational. No decorative animation or marketing layout is included.

## Adapter Detection

At startup, the app enumerates `NetworkInterface.GetAllNetworkInterfaces()`.

Default selection priority:

1. `NetworkInterfaceType.Wireless80211`.
2. Name or description contains `WLAN`.
3. Name or description contains `Wi-Fi`.
4. Name or description contains `Wireless`.
5. Name or description contains `802.11`.
6. `OperationalStatus.Up` is preferred among likely wireless adapters.
7. Last saved adapter name from `config.json` is selected first if still present and not hidden by the current filter.

Default list hides likely virtual adapters. `显示所有网卡` includes them for manual recovery.

Virtual adapter markers include generic adapter categories such as:

- tunnel-like adapters
- TAP-style adapters
- `vEthernet`
- `VMware`
- `VirtualBox`
- `Hyper-V`
- `Bluetooth`
- `Loopback`

If the user selects a suspicious virtual adapter, the app warns before changing DNS and recommends selecting a physical WLAN/Wi-Fi adapter.

## DNS Mode Detection

The app detects current mode through:

```text
netsh interface ipv4 show dnsservers name=<adapter name>
```

The parser maps output to:

- `自动 DNS 模式` when DNS is DHCP/automatic.
- `自定义 DNS 模式` when DNS servers are exactly `saved custom IPv4 DNS` in the first two IPv4 DNS positions.
- `其他 DNS` when DNS is static but not the target pair.
- `检测失败` when the command fails or parsing cannot determine the mode.

`NetworkInterface.DnsAddresses` is only used for informational display, not as the sole source of automatic/static mode detection.

## DNS Operations

All command execution uses `ProcessStartInfo` with explicit executable and argument list. The app does not build a shell command string.

Automatic DNS mode:

```text
netsh interface ipv4 set dnsservers name=<adapter name> source=dhcp
ipconfig /flushdns
```

Custom DNS mode:

```text
netsh interface ipv4 set dnsservers name=<adapter name> static <primary custom IPv4 DNS> primary
netsh interface ipv4 add dnsservers name=<adapter name> <secondary custom IPv4 DNS> index=2
ipconfig /flushdns
```

Repeated clicks are short-circuited:

- Already automatic DNS: `当前已是自动 DNS 模式。`
- Already `saved custom IPv4 DNS`: `当前已是自定义 DNS 模式。`

## IPv6

The app detects and displays IPv6 status only:

- `IPv6：已启用`
- `IPv6：未启用`
- `IPv6：检测失败`

The app does not provide IPv6 disable/restore buttons in this version.

## Config

`config.json` is stored next to the executable and contains only the last selected adapter name:

```json
{
  "lastAdapterName": "WLAN"
}
```

Invalid or unreadable config falls back to auto-selection and a short log message.

## Permissions

The WinForms app embeds `app.manifest` with `requireAdministrator`. If elevation is somehow missing, startup shows `需要管理员权限。`.

## Testing

Automated tests cover:

- Adapter classification and virtual/proxy adapter detection.
- Default adapter sorting and selection.
- DNS mode parser behavior for automatic, target static DNS, other DNS, and failed output.
- Command argument construction for DNS changes and flush DNS.
- Config save/load behavior.

Manual verification covers:

- WinForms build.
- Console test runner.
- `win-x64` self-contained publish to `dist`.

Actual DNS mutation commands are not run during automated tests.




