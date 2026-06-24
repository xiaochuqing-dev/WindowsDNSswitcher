using System.Diagnostics;
using WindowsDnsSwitcher.Core;

namespace WindowsDnsSwitcher.WinForms;

public sealed class MainForm : Form
{
    private static readonly Color ModeButtonInactiveBackColor = Color.FromArgb(212, 235, 255);
    private static readonly Color ModeButtonInactiveForeColor = Color.FromArgb(20, 82, 130);
    private static readonly Color ModeButtonActiveBackColor = Color.FromArgb(28, 75, 145);
    private static readonly Color ModeButtonActiveForeColor = Color.White;

    private readonly ProcessCommandRunner commandRunner = new();
    private readonly string configPath = Path.Combine(AppContext.BaseDirectory, "config.json");

    private readonly Label selectedAdapterLabel = new();
    private readonly ComboBox adapterComboBox = new();
    private readonly Button refreshAdaptersButton = new();
    private readonly CheckBox showAllAdaptersCheckBox = new();
    private readonly Label modeLabel = new();
    private readonly Label ipv6Label = new();
    private readonly Button automaticDnsModeButton = new();
    private readonly Button customDnsModeButton = new();
    private readonly Button customDnsSettingsButton = new();
    private readonly Button flushDnsButton = new();
    private readonly Button networkConnectionsButton = new();
    private readonly Button browserLeaksButton = new();
    private readonly Button ipLeakButton = new();
    private readonly Button claudeIpCheckButton = new();
    private readonly Button scamalyticsButton = new();
    private readonly Button ping0Button = new();
    private readonly Button adapterInfoButton = new();
    private readonly ListBox logListBox = new();

    private IReadOnlyList<AdapterInfo> allAdapters = [];
    private bool isRefreshingAdapters;

    public MainForm()
    {
        Text = "Windows DNS 模式切换工具";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(940, 780);
        ClientSize = new Size(940, 820);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Microsoft YaHei UI", 9F);

        BuildLayout();
        WireEvents();
        LoadAdapters();
    }

    private AdapterInfo? SelectedAdapter => adapterComboBox.SelectedItem as AdapterInfo;

    private void BuildLayout()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 7,
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 124));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 172));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleLabel = new Label
        {
            Text = "Windows DNS 模式切换工具",
            Dock = DockStyle.Fill,
            Font = new Font(Font.FontFamily, 15F, FontStyle.Bold),
            TextAlign = ContentAlignment.TopCenter,
            AutoEllipsis = true,
            Padding = new Padding(0),
            Margin = new Padding(0),
        };

        var descriptionLabel = new Label
        {
            Text = "在“自动获取 DNS”和“自定义 DNS”之间快速切换。",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(70, 78, 88),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = new Padding(0, 0, 0, 8),
        };

        var adapterPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            Margin = new Padding(0, 10, 0, 6),
        };
        adapterPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        adapterPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        adapterPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        adapterPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        adapterPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 178));
        adapterPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 164));

        adapterComboBox.Dock = DockStyle.Fill;
        adapterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        adapterComboBox.DisplayMember = nameof(AdapterInfo.DisplayName);
        adapterComboBox.Margin = new Padding(0, 2, 0, 4);

        refreshAdaptersButton.Text = "刷新网卡";
        refreshAdaptersButton.Dock = DockStyle.Fill;
        refreshAdaptersButton.Margin = new Padding(0, 2, 12, 2);
        refreshAdaptersButton.AutoEllipsis = true;
        refreshAdaptersButton.Font = new Font("Microsoft YaHei UI", 8.5F);

        showAllAdaptersCheckBox.Text = "显示所有网卡";
        showAllAdaptersCheckBox.Dock = DockStyle.Fill;
        showAllAdaptersCheckBox.TextAlign = ContentAlignment.MiddleLeft;
        showAllAdaptersCheckBox.Margin = new Padding(0, 2, 0, 2);

        selectedAdapterLabel.Dock = DockStyle.Fill;
        selectedAdapterLabel.TextAlign = ContentAlignment.MiddleLeft;
        selectedAdapterLabel.ForeColor = Color.FromArgb(70, 78, 88);
        selectedAdapterLabel.AutoEllipsis = true;
        selectedAdapterLabel.Margin = new Padding(0, 2, 0, 0);

        adapterPanel.Controls.Add(adapterComboBox, 0, 0);
        adapterPanel.Controls.Add(selectedAdapterLabel, 0, 1);
        adapterPanel.Controls.Add(refreshAdaptersButton, 1, 2);
        adapterPanel.Controls.Add(showAllAdaptersCheckBox, 2, 2);
        adapterPanel.SetColumnSpan(adapterComboBox, 3);
        adapterPanel.SetColumnSpan(selectedAdapterLabel, 3);

        var statusPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Margin = new Padding(0, 4, 0, 10),
        };
        statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
        statusPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));

        modeLabel.Text = "当前模式：检测失败";
        modeLabel.Dock = DockStyle.Fill;
        modeLabel.Font = new Font(Font.FontFamily, 10F, FontStyle.Bold);
        modeLabel.TextAlign = ContentAlignment.MiddleLeft;
        modeLabel.AutoEllipsis = true;

        ipv6Label.Text = "IPv6：检测失败";
        ipv6Label.Dock = DockStyle.Fill;
        ipv6Label.TextAlign = ContentAlignment.MiddleRight;
        ipv6Label.AutoEllipsis = true;

        statusPanel.Controls.Add(modeLabel, 0, 0);
        statusPanel.Controls.Add(ipv6Label, 1, 0);

        var mainButtonPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            Margin = new Padding(0, 8, 0, 14),
        };
        mainButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        mainButtonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        ConfigureMainButton(automaticDnsModeButton, "自动 DNS 模式\r\n使用自动获取的 DNS");
        ConfigureMainButton(customDnsModeButton, "自定义 DNS 模式\r\n使用已保存的 DNS");
        automaticDnsModeButton.Margin = new Padding(0, 8, 8, 8);
        customDnsModeButton.Margin = new Padding(8, 8, 0, 8);

        mainButtonPanel.Controls.Add(automaticDnsModeButton, 0, 0);
        mainButtonPanel.Controls.Add(customDnsModeButton, 1, 0);

        var helperPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            Margin = new Padding(0, 4, 0, 4),
        };
        for (var column = 0; column < helperPanel.ColumnCount; column++)
        {
            helperPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
        }

        helperPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        helperPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        helperPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));

        var helperButtons = new[]
        {
            customDnsSettingsButton,
            flushDnsButton,
            networkConnectionsButton,
            adapterInfoButton,
            browserLeaksButton,
            ipLeakButton,
            claudeIpCheckButton,
            scamalyticsButton,
            ping0Button,
        };

        foreach (var button in helperButtons)
        {
            ConfigureHelperButton(button);
        }

        helperPanel.Controls.Add(customDnsSettingsButton, 0, 0);
        helperPanel.Controls.Add(flushDnsButton, 1, 0);
        helperPanel.Controls.Add(networkConnectionsButton, 2, 0);
        helperPanel.Controls.Add(adapterInfoButton, 0, 1);
        helperPanel.Controls.Add(browserLeaksButton, 1, 1);
        helperPanel.Controls.Add(ipLeakButton, 2, 1);
        helperPanel.Controls.Add(claudeIpCheckButton, 0, 2);
        helperPanel.Controls.Add(scamalyticsButton, 1, 2);
        helperPanel.Controls.Add(ping0Button, 2, 2);

        customDnsSettingsButton.Text = "设置自定义 DNS";
        flushDnsButton.Text = "刷新 DNS";
        networkConnectionsButton.Text = "网络连接";
        browserLeaksButton.Text = "BrowserLeaks";
        ipLeakButton.Text = "ipleak";
        claudeIpCheckButton.Text = "IP.Net.Coffee";
        scamalyticsButton.Text = "Scamalytics";
        ping0Button.Text = "ping0";
        adapterInfoButton.Text = "网卡信息";

        logListBox.Dock = DockStyle.Fill;
        logListBox.IntegralHeight = false;
        logListBox.Margin = new Padding(0, 8, 0, 0);

        root.Controls.Add(titleLabel, 0, 0);
        root.Controls.Add(descriptionLabel, 0, 1);
        root.Controls.Add(adapterPanel, 0, 2);
        root.Controls.Add(statusPanel, 0, 3);
        root.Controls.Add(mainButtonPanel, 0, 4);
        root.Controls.Add(helperPanel, 0, 5);
        root.Controls.Add(logListBox, 0, 6);

        Controls.Add(root);
    }

    private void WireEvents()
    {
        refreshAdaptersButton.Click += (_, _) => LoadAdapters();
        showAllAdaptersCheckBox.CheckedChanged += (_, _) =>
        {
            if (isRefreshingAdapters)
            {
                return;
            }

            SaveConfig(AppConfig.Load(configPath).WithShowAllAdapters(showAllAdaptersCheckBox.Checked));
            LoadAdapters();
        };
        adapterComboBox.SelectedIndexChanged += (_, _) => AdapterSelectionChanged();
        automaticDnsModeButton.Click += async (_, _) => await SwitchAutomaticDnsModeAsync();
        customDnsModeButton.Click += async (_, _) => await SwitchCustomDnsModeAsync();
        customDnsSettingsButton.Click += async (_, _) => await ShowCustomDnsSettingsDialogAsync();
        flushDnsButton.Click += async (_, _) => await FlushDnsAsync(showDialog: true);
        networkConnectionsButton.Click += (_, _) => StartShell("ncpa.cpl");
        browserLeaksButton.Click += (_, _) => StartShell("https://browserleaks.com/ip", "打开检测网站失败");
        ipLeakButton.Click += (_, _) => StartShell("https://ipleak.net", "打开检测网站失败");
        claudeIpCheckButton.Click += (_, _) => StartShell("https://ip.net.coffee/", "打开检测网站失败");
        scamalyticsButton.Click += (_, _) => StartShell("https://scamalytics.com/ip", "打开检测网站失败");
        ping0Button.Click += (_, _) => StartShell("https://ping0.cc", "打开检测网站失败");
        adapterInfoButton.Click += (_, _) => ShowAdapterInfo();
    }

    private static void ConfigureMainButton(Button button, string text)
    {
        button.Text = text;
        button.Dock = DockStyle.Fill;
        button.Margin = new Padding(0, 8, 10, 8);
        button.BackColor = ModeButtonInactiveBackColor;
        button.ForeColor = ModeButtonInactiveForeColor;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = ModeButtonInactiveForeColor;
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(198, 226, 249);
        button.UseVisualStyleBackColor = false;
        button.Font = new Font("Microsoft YaHei UI", 12.5F, FontStyle.Bold);
        button.TextAlign = ContentAlignment.MiddleCenter;
    }

    private static void ConfigureHelperButton(Button button)
    {
        button.Dock = DockStyle.Fill;
        button.Height = 38;
        button.Margin = new Padding(0, 0, 10, 8);
        button.AutoEllipsis = true;
        button.Font = new Font("Microsoft YaHei UI", 8.5F);
    }

    private void LoadAdapters()
    {
        isRefreshingAdapters = true;
        try
        {
            allAdapters = NetworkAdapterReader.GetAdapters();
            var config = AppConfig.Load(configPath);
            if (showAllAdaptersCheckBox.Checked != config.ShowAllAdapters)
            {
                showAllAdaptersCheckBox.Checked = config.ShowAllAdapters;
            }

            var displayAdapters = AdapterClassifier.GetDisplayAdapters(allAdapters, config.ShowAllAdapters, config.LastAdapterName).ToArray();

            adapterComboBox.Items.Clear();
            foreach (var adapter in displayAdapters)
            {
                adapterComboBox.Items.Add(adapter);
            }

            adapterComboBox.SelectedItem = AdapterClassifier.SelectDefaultAdapter(allAdapters, config.ShowAllAdapters, config.LastAdapterName);

            if (adapterComboBox.SelectedItem is null && adapterComboBox.Items.Count > 0)
            {
                adapterComboBox.SelectedIndex = 0;
            }

            if (adapterComboBox.Items.Count == 0)
            {
                SetStatus(null, DnsMode.DetectionFailed);
                AddLog("找不到无线网卡。");
            }
        }
        catch (Exception ex)
        {
            AddLog($"读取网卡失败：{ex.Message}");
        }
        finally
        {
            isRefreshingAdapters = false;
        }

        AdapterSelectionChanged();
    }

    private async void AdapterSelectionChanged()
    {
        var adapter = SelectedAdapter;
        SetStatus(adapter, DnsMode.DetectionFailed);

        if (adapter is null || isRefreshingAdapters)
        {
            return;
        }

        SaveConfig(AppConfig.Load(configPath)
            .WithSelectedAdapter(adapter.Name)
            .WithShowAllAdapters(showAllAdaptersCheckBox.Checked));
        await RefreshModeAsync();
    }

    private async Task RefreshModeAsync()
    {
        var adapter = SelectedAdapter;
        if (adapter is null)
        {
            return;
        }

        var command = DnsCommandFactory.CreateShowDnsServersCommand(adapter.Name);
        var result = await commandRunner.RunAsync(command);
        var mode = DnsModeParser.Parse(result.Succeeded, result.CombinedOutput, AppConfig.Load(configPath).CustomDnsSettings);
        SetStatus(adapter, mode);
    }

    private async Task SwitchAutomaticDnsModeAsync()
    {
        var adapter = SelectedAdapter;
        if (adapter is null)
        {
            AddLog("请先选择网卡。");
            return;
        }

        if (!ConfirmAdapterChoice(adapter))
        {
            return;
        }

        if (!ConfirmModeSwitch(DnsMode.AutomaticDns, adapter))
        {
            return;
        }

        var currentMode = await GetCurrentModeAsync(adapter);
        if (currentMode == DnsMode.AutomaticDns)
        {
            AddLog("当前已是自动 DNS 模式。");
            MessageBox.Show("当前已是自动 DNS 模式。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await RunCommandsAsync(DnsCommandFactory.CreateAutomaticDnsModeCommands(adapter.Name), "已切换到自动 DNS 模式。");
    }

    private async Task SwitchCustomDnsModeAsync()
    {
        var adapter = SelectedAdapter;
        if (adapter is null)
        {
            AddLog("请先选择网卡。");
            return;
        }

        var config = AppConfig.Load(configPath);
        var validationStatus = CustomDnsValidator.Validate(config.PrimaryDns, config.SecondaryDns);
        if (validationStatus == CustomDnsValidationStatus.MissingPrimary)
        {
            AddLog("请先设置自定义 DNS。");
            MessageBox.Show("请先设置自定义 DNS。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (validationStatus == CustomDnsValidationStatus.InvalidAddress)
        {
            MessageBox.Show("请输入合法的 IPv4 DNS 地址。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ConfirmAdapterChoice(adapter))
        {
            return;
        }

        if (!ConfirmModeSwitch(DnsMode.CustomDns, adapter))
        {
            return;
        }

        var currentMode = await GetCurrentModeAsync(adapter);
        if (currentMode == DnsMode.CustomDns)
        {
            AddLog("当前已是自定义 DNS 模式。");
            MessageBox.Show("当前已是自定义 DNS 模式。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await RunCommandsAsync(DnsCommandFactory.CreateCustomDnsModeCommands(adapter.Name, config.CustomDnsSettings), "已切换到自定义 DNS 模式。");
    }

    private async Task<DnsMode> GetCurrentModeAsync(AdapterInfo adapter)
    {
        var result = await commandRunner.RunAsync(DnsCommandFactory.CreateShowDnsServersCommand(adapter.Name));
        return DnsModeParser.Parse(result.Succeeded, result.CombinedOutput, AppConfig.Load(configPath).CustomDnsSettings);
    }

    private async Task ShowCustomDnsSettingsDialogAsync()
    {
        var config = AppConfig.Load(configPath);

        using var dialog = new Form
        {
            Text = "设置自定义 DNS",
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            ClientSize = new Size(520, 250),
            Font = Font,
        };

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 2,
            RowCount = 5,
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

        var helpLabel = new Label
        {
            Text = "请填写 IPv4 DNS 地址；暂不支持 IPv6、DoH、DoT 或域名形式。",
            Dock = DockStyle.Fill,
            ForeColor = Color.FromArgb(70, 78, 88),
        };
        root.Controls.Add(helpLabel, 0, 0);
        root.SetColumnSpan(helpLabel, 2);

        var primaryLabel = new Label
        {
            Text = "首选 DNS",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var primaryTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = config.PrimaryDns,
            Margin = new Padding(0, 6, 0, 6),
        };
        root.Controls.Add(primaryLabel, 0, 1);
        root.Controls.Add(primaryTextBox, 1, 1);

        var secondaryLabel = new Label
        {
            Text = "备用 DNS",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        var secondaryTextBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = config.SecondaryDns,
            Margin = new Padding(0, 6, 0, 6),
        };
        root.Controls.Add(secondaryLabel, 0, 2);
        root.Controls.Add(secondaryTextBox, 1, 2);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
        };
        var saveButton = new Button
        {
            Text = "保存",
            Width = 86,
            Height = 32,
            DialogResult = DialogResult.None,
        };
        var cancelButton = new Button
        {
            Text = "取消",
            Width = 86,
            Height = 32,
            DialogResult = DialogResult.Cancel,
        };
        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        root.Controls.Add(buttonPanel, 0, 4);
        root.SetColumnSpan(buttonPanel, 2);

        saveButton.Click += (_, _) =>
        {
            var validation = CustomDnsValidator.Validate(primaryTextBox.Text, secondaryTextBox.Text);
            if (validation == CustomDnsValidationStatus.MissingPrimary)
            {
                MessageBox.Show("请填写首选 DNS。", dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (validation == CustomDnsValidationStatus.InvalidAddress)
            {
                MessageBox.Show("请输入合法的 IPv4 DNS 地址。", dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var settings = new CustomDnsSettings(primaryTextBox.Text, secondaryTextBox.Text).Normalize();
            SaveConfig(AppConfig.Load(configPath)
                .WithCustomDns(settings)
                .WithShowAllAdapters(showAllAdaptersCheckBox.Checked));
            AddLog("DNS 设置已保存。");
            MessageBox.Show("DNS 设置已保存。", dialog.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            dialog.DialogResult = DialogResult.OK;
            dialog.Close();
        };

        dialog.Controls.Add(root);
        dialog.AcceptButton = saveButton;
        dialog.CancelButton = cancelButton;
        dialog.ShowDialog(this);
        await RefreshModeAsync();
    }

    private async Task FlushDnsAsync(bool showDialog)
    {
        var result = await commandRunner.RunAsync(DnsCommandFactory.CreateFlushDnsCommand());
        if (result.Succeeded)
        {
            AddLog("DNS 已刷新。");
            if (showDialog)
            {
                MessageBox.Show("DNS 已刷新。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        ShowFailure("刷新 DNS 失败", result);
    }

    private async Task RunCommandsAsync(IEnumerable<CommandSpec> commands, string successMessage)
    {
        SetBusy(true);
        try
        {
            foreach (var command in commands)
            {
                var result = await commandRunner.RunAsync(command);
                if (!result.Succeeded)
                {
                    ShowFailure("操作失败", result);
                    return;
                }
            }

            AddLog(successMessage);
            MessageBox.Show(successMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            await RefreshModeAsync();
        }
        finally
        {
            SetBusy(false);
        }
    }

    private bool ConfirmAdapterChoice(AdapterInfo adapter)
    {
        if (AdapterClassifier.IsProxyTunAdapter(adapter))
        {
            return MessageBox.Show(
                "当前选择的可能是虚拟网卡，不建议修改它的 DNS。请先选择真实 WLAN/Wi-Fi 网卡。\r\n\r\n仍要继续吗？",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        if (AdapterClassifier.IsLikelyVirtual(adapter) || !AdapterClassifier.IsLikelyWireless(adapter))
        {
            return MessageBox.Show(
                "当前选择的可能不是物理 WLAN/Wi-Fi 网卡，请确认。\r\n\r\n仍要继续吗？",
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes;
        }

        return true;
    }

    private bool ConfirmModeSwitch(DnsMode targetMode, AdapterInfo adapter)
    {
        var confirmation = ModeSwitchConfirmation.ForMode(targetMode, adapter.Name);
        return MessageBox.Show(
            confirmation.Message,
            confirmation.Title,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question) == DialogResult.Yes;
    }

    private void ShowFailure(string title, CommandResult result)
    {
        var detail = string.IsNullOrWhiteSpace(result.CombinedOutput)
            ? $"退出代码：{result.ExitCode}"
            : result.CombinedOutput;

        AddLog($"{title}：{detail}");
        MessageBox.Show($"{title}：{detail}", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void SetStatus(AdapterInfo? adapter, DnsMode mode)
    {
        selectedAdapterLabel.Text = adapter is null ? "当前网卡：未选择" : $"当前网卡：{adapter.DisplayName}";
        modeLabel.Text = NetworkInfoFormatter.FormatMode(mode);
        ipv6Label.Text = NetworkInfoFormatter.FormatIpv6Status(adapter);
        UpdateModeButtonStyles(mode);
    }

    private void UpdateModeButtonStyles(DnsMode mode)
    {
        ApplyModeButtonStyle(automaticDnsModeButton, mode == DnsMode.AutomaticDns);
        ApplyModeButtonStyle(customDnsModeButton, mode == DnsMode.CustomDns);
    }

    private static void ApplyModeButtonStyle(Button button, bool isActive)
    {
        button.BackColor = isActive ? ModeButtonActiveBackColor : ModeButtonInactiveBackColor;
        button.ForeColor = isActive ? ModeButtonActiveForeColor : ModeButtonInactiveForeColor;
        button.FlatAppearance.BorderColor = isActive ? ModeButtonActiveBackColor : ModeButtonInactiveForeColor;
        button.FlatAppearance.MouseOverBackColor = isActive ? Color.FromArgb(23, 66, 128) : Color.FromArgb(198, 226, 249);
    }

    private void SetBusy(bool isBusy)
    {
        foreach (Control control in Controls)
        {
            control.Enabled = !isBusy;
        }
    }

    private void AddLog(string message)
    {
        logListBox.Items.Insert(0, $"{DateTime.Now:HH:mm:ss}  {message}");
        while (logListBox.Items.Count > 8)
        {
            logListBox.Items.RemoveAt(logListBox.Items.Count - 1);
        }
    }

    private void SaveConfig(AppConfig config)
    {
        AppConfig.Save(configPath, config);
    }

    private void ShowAdapterInfo()
    {
        var adapter = SelectedAdapter;
        if (adapter is null)
        {
            MessageBox.Show("请先选择网卡。", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        MessageBox.Show(NetworkInfoFormatter.FormatAdapterInfo(adapter), "网卡信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void StartShell(string target, string failureTitle = "打开失败")
    {
        try
        {
            Process.Start(new ProcessStartInfo(target)
            {
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            AddLog($"{failureTitle}：{ex.Message}");
            MessageBox.Show($"{failureTitle}。", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}


