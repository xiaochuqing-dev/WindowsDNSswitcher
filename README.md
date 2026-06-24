# WindowsDnsSwitcher

A lightweight Windows DNS mode switcher for quickly switching a network adapter between automatic DNS and custom IPv4 DNS.

[简体中文](./README.zh-CN.md)

## Screenshot

> Screenshot will be added later.

## Features

- Switch between automatic DNS and custom IPv4 DNS.
- Save custom DNS settings locally.
- Refresh the Windows DNS cache.
- View selected network adapter information.
- Open Windows Network Connections.
- Open common network diagnostic websites in the default browser.
- Focus only on Windows IPv4 DNS settings for the selected adapter.
- Switch the app UI between English and Simplified Chinese.

## Use Cases

- Public Wi-Fi.
- Captive portal networks.
- DNS troubleshooting.
- Temporary network diagnostics.
- Switching between automatic and custom DNS.
- Development and network configuration testing.

## Download / Installation

A prebuilt release will be added later. You can build from source for now.

When a release package is available:

1. Download the release package from this repository.
2. Extract the archive.
3. Run the application as administrator when changing DNS settings.
4. Select a network adapter.
5. Switch between automatic DNS and custom IPv4 DNS as needed.

## Usage

1. Select the target network adapter.
2. Use Automatic DNS Mode to restore DNS settings from the network.
3. Use Custom DNS Mode to apply saved custom IPv4 DNS servers.
4. Use Set Custom DNS to configure the primary and optional secondary DNS server.
5. Use Refresh DNS to flush the Windows DNS cache.
6. Use diagnostic website shortcuts only when you need to manually inspect network information.
7. Use the language selector to switch the interface between English and Simplified Chinese.

## Administrator Permission

Administrator permission is required because Windows requires elevated privileges to change network adapter DNS settings.

## Privacy and Diagnostic Websites

- The app stores custom DNS settings locally.
- Diagnostic buttons only open third-party websites in the default browser.
- Third-party diagnostic sites may see the user's public IP address and browser or network information.
- The app does not scrape, parse, upload, or store diagnostic results.
- Users should review third-party websites' own privacy policies before using them.

## Security Notes

- Download the app only from the official repository or official Releases page.
- Review the source code before building if needed.
- Do not commit local configuration files containing private DNS settings or personal information.
- Use the tool only on devices and networks you are authorized to manage.
- Changing DNS settings may affect network access. Restore Automatic DNS Mode if a network stops working.

## What It Does Not Do

- It does not provide traffic forwarding services.
- It does not create network tunnels.
- It does not control application traffic routing.
- It does not modify system-wide traffic forwarding settings.
- It does not modify IPv6 settings.
- It does not collect or upload diagnostic results.

## Build From Source

Requirements:

- Windows 10/11.
- .NET SDK 10.0 or another SDK version compatible with the project files.

Restore and build:

```powershell
dotnet restore
dotnet build WindowsDnsSwitcher.slnx
```

Run the WinForms app:

```powershell
dotnet run --project src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj
```

Publish a self-contained Windows x64 build:

```powershell
dotnet publish src\WindowsDnsSwitcher.WinForms\WindowsDnsSwitcher.WinForms.csproj -c Release -r win-x64 --self-contained true
```

You can also use the included build script:

```bat
build.bat
```

The build script publishes to `dist/`. Release packages are not committed to the repository.

## Configuration

Local settings are stored in a local `config.json` file next to the app executable. Real local configuration files should not be committed.

Use `config.example.json` only as a structure example.

Configuration fields:

- `selectedAdapter`: last selected adapter name.
- `primaryDns`: saved primary custom IPv4 DNS server.
- `secondaryDns`: optional saved secondary custom IPv4 DNS server.
- `showAllAdapters`: whether the adapter list should show additional adapters.
- `language`: `en` or `zh-CN`.

## License

This project is licensed under the GNU General Public License v3.0.

Official releases are provided only from this repository.

## Official Release Note

No official prebuilt release has been published yet. Do not download binaries from unofficial sources.
