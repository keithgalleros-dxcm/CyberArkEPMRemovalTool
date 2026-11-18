using System.Reflection;

namespace CyberArkEPMRemovalTool;

internal sealed class AboutForm : Form
{
    internal static string AboutText
    {
        get
        {
            var assembly = typeof(AboutForm).Assembly;
            string Metadata(string key) => assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .First(item => item.Key == key).Value ?? string.Empty;

            return $"{assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product}\r\n" +
                $"Version {assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion} | Portable Windows x64\r\n\r\n" +
                $"{assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description}\r\n\r\n" +
                $"Developer: {Metadata("Developer")}\r\n" +
                $"Title: {Metadata("DeveloperTitle")}\r\n" +
                $"Department: {Metadata("Department")}\r\n" +
                $"Company: {assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}\r\n\r\n" +
                "Administrator privileges are required. Agent self-defense may require a valid secure token.\r\n" +
                "Verification checks the selected MSI registration; it does not inspect every remaining file, service, or driver.";
        }
    }

    public AboutForm()
    {
        Text = "About CyberArk EPM Agent Removal Utility";
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(620, 430);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 10F);
        Padding = new Padding(20);

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        var details = new TextBox
        {
            Text = AboutText, Multiline = true, ReadOnly = true,
            BorderStyle = BorderStyle.None, BackColor = Color.White,
            Dock = DockStyle.Fill, ScrollBars = ScrollBars.Vertical,
            TabStop = false
        };
        var close = new Button
        {
            Text = "Close", DialogResult = DialogResult.OK,
            Anchor = AnchorStyles.Right, Size = new Size(100, 34)
        };
        layout.Controls.Add(details, 0, 0);
        layout.Controls.Add(close, 0, 1);
        Controls.Add(layout);
        AcceptButton = close;
        CancelButton = close;
    }
}
