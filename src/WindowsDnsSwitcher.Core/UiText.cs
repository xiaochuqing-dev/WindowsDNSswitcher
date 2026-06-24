namespace WindowsDnsSwitcher.Core;

public static class UiText
{
    public static string Title(AppLanguage language)
    {
        return language == AppLanguage.English ? "Windows DNS Mode Switcher" : "Windows DNS 模式切换工具";
    }

    public static string Subtitle(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "Quickly switch between \"Automatic DNS\" and \"Custom DNS\"."
            : "在“自动获取 DNS”和“自定义 DNS”之间快速切换。";
    }

    public static string LanguageLabel(AppLanguage language)
    {
        return language == AppLanguage.English ? "UI" : "语言";
    }

    public static string RefreshAdapters(AppLanguage language)
    {
        return language == AppLanguage.English ? "Refresh" : "刷新网卡";
    }

    public static string ShowAllAdapters(AppLanguage language)
    {
        return language == AppLanguage.English ? "Show all" : "显示所有网卡";
    }

    public static string AutomaticDnsButton(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "Automatic DNS Mode\r\nUse automatic DNS"
            : "自动 DNS 模式\r\n使用自动获取的 DNS";
    }

    public static string CustomDnsButton(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "Custom DNS Mode\r\nUse saved DNS"
            : "自定义 DNS 模式\r\n使用已保存的 DNS";
    }

    public static string SetCustomDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Set Custom DNS" : "设置自定义 DNS";
    }

    public static string RefreshDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Refresh DNS" : "刷新 DNS";
    }

    public static string NetworkConnections(AppLanguage language)
    {
        return language == AppLanguage.English ? "Network Connections" : "网络连接";
    }

    public static string AdapterInfo(AppLanguage language)
    {
        return language == AppLanguage.English ? "Adapter Info" : "网卡信息";
    }

    public static string NoAdapterSelected(AppLanguage language)
    {
        return language == AppLanguage.English ? "Current adapter: not selected" : "当前网卡：未选择";
    }

    public static string CurrentAdapter(AppLanguage language, string displayName)
    {
        return language == AppLanguage.English ? $"Current adapter: {displayName}" : $"当前网卡：{displayName}";
    }

    public static string SelectAdapterFirst(AppLanguage language)
    {
        return language == AppLanguage.English ? "Please select a network adapter first." : "请先选择网卡。";
    }

    public static string NoWirelessAdapter(AppLanguage language)
    {
        return language == AppLanguage.English ? "No wireless adapter found." : "找不到无线网卡。";
    }

    public static string AdapterReadFailed(AppLanguage language, string message)
    {
        return language == AppLanguage.English ? $"Failed to read adapters: {message}" : $"读取网卡失败：{message}";
    }

    public static string AlreadyAutomaticDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Current mode is already Automatic DNS Mode." : "当前已是自动 DNS 模式。";
    }

    public static string SwitchedToAutomaticDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Switched to Automatic DNS Mode." : "已切换到自动 DNS 模式。";
    }

    public static string SetCustomDnsFirst(AppLanguage language)
    {
        return language == AppLanguage.English ? "Please set custom DNS first." : "请先设置自定义 DNS。";
    }

    public static string InvalidIpv4Dns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Enter a valid IPv4 DNS address." : "请输入合法的 IPv4 DNS 地址。";
    }

    public static string AlreadyCustomDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Current mode is already Custom DNS Mode." : "当前已是自定义 DNS 模式。";
    }

    public static string SwitchedToCustomDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Switched to Custom DNS Mode." : "已切换到自定义 DNS 模式。";
    }

    public static string CustomDnsDialogHelp(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "Enter IPv4 DNS addresses. IPv6, DoH, DoT, and domain names are not supported."
            : "请填写 IPv4 DNS 地址；暂不支持 IPv6、DoH、DoT 或域名形式。";
    }

    public static string PrimaryDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Primary DNS" : "首选 DNS";
    }

    public static string SecondaryDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Secondary DNS" : "备用 DNS";
    }

    public static string Save(AppLanguage language)
    {
        return language == AppLanguage.English ? "Save" : "保存";
    }

    public static string Cancel(AppLanguage language)
    {
        return language == AppLanguage.English ? "Cancel" : "取消";
    }

    public static string FillPrimaryDns(AppLanguage language)
    {
        return language == AppLanguage.English ? "Please enter a primary DNS server." : "请填写首选 DNS。";
    }

    public static string DnsSettingsSaved(AppLanguage language)
    {
        return language == AppLanguage.English ? "DNS settings saved." : "DNS 设置已保存。";
    }

    public static string DnsRefreshed(AppLanguage language)
    {
        return language == AppLanguage.English ? "DNS refreshed." : "DNS 已刷新。";
    }

    public static string RefreshDnsFailed(AppLanguage language)
    {
        return language == AppLanguage.English ? "Refresh DNS failed" : "刷新 DNS 失败";
    }

    public static string OperationFailed(AppLanguage language)
    {
        return language == AppLanguage.English ? "Operation failed" : "操作失败";
    }

    public static string VirtualAdapterWarning(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "The selected adapter may be virtual. Changing its DNS is not recommended. Select a physical WLAN/Wi-Fi adapter if possible.\r\n\r\nContinue anyway?"
            : "当前选择的可能是虚拟网卡，不建议修改它的 DNS。请先选择真实 WLAN/Wi-Fi 网卡。\r\n\r\n仍要继续吗？";
    }

    public static string NonWirelessAdapterWarning(AppLanguage language)
    {
        return language == AppLanguage.English
            ? "The selected adapter may not be a physical WLAN/Wi-Fi adapter. Please confirm.\r\n\r\nContinue anyway?"
            : "当前选择的可能不是物理 WLAN/Wi-Fi 网卡，请确认。\r\n\r\n仍要继续吗？";
    }

    public static string FailureWithExitCode(AppLanguage language, int exitCode)
    {
        return language == AppLanguage.English ? $"Exit code: {exitCode}" : $"退出代码：{exitCode}";
    }

    public static string OpenFailed(AppLanguage language)
    {
        return language == AppLanguage.English ? "Open failed" : "打开失败";
    }

    public static string OpenDiagnosticFailed(AppLanguage language)
    {
        return language == AppLanguage.English ? "Failed to open diagnostic website" : "打开检测网站失败";
    }
}
