using System.Security.Principal;

namespace WindowsDnsSwitcher.WinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!IsAdministrator())
        {
            MessageBox.Show("需要管理员权限。", "Windows DNS 模式切换工具", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Application.Run(new MainForm());
    }

    private static bool IsAdministrator()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }
}


