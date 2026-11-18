using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace CyberArkEPMRemovalTool
{
    public partial class Form1 : Form
    {
        // Dexcom company brand colors
        private static readonly Color DexcomGreen =
            Color.FromArgb(48, 171, 13);       // #30AB0D

        private static readonly Color ContrastGreen =
            Color.FromArgb(19, 125, 0);        // #137D00

        private static readonly Color SubtleGray =
            Color.FromArgb(246, 246, 246);     // #F6F6F6

        private static readonly Color CoolGray =
            Color.FromArgb(217, 216, 214);     // #D9D8D6

        private static readonly Color BoldGray =
            Color.FromArgb(67, 65, 65);        // #434141

        private static readonly Color AccentPlum =
            Color.FromArgb(65, 38, 54);        // #412636

        private static readonly Color Bone =
            Color.FromArgb(245, 244, 234);     // #F5F4EA

        private static readonly Color SuccessGreen =
            Color.FromArgb(19, 125, 0);

        private static readonly Color WarningColor =
            Color.FromArgb(186, 112, 0);

        private static readonly Color ErrorColor =
            Color.FromArgb(180, 42, 42);
        private readonly Label lblAdminValue = new();
        private readonly Label lblStatusValue = new();

        private readonly TextBox txtComputer = new();
        private readonly TextBox txtDisplayName = new();
        private readonly TextBox txtVersion = new();
        private readonly TextBox txtPublisher = new();
        private readonly TextBox txtGuid = new();
        private readonly TextBox txtRegistryView = new();
        private readonly TextBox txtInstallLocation = new();
        private readonly TextBox txtSecureToken = new();

        private readonly Button btnDetect = new();
        private readonly Button btnUninstall = new();
        private readonly Button btnOpenLog = new();

        private readonly ProgressBar progressBar = new();
        private readonly RichTextBox txtActivity = new();

        private EpmAgentInfo? detectedAgent;
        private string logFilePath = string.Empty;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();

            Shown += async (_, _) =>
            {
                InitializeLog();

                Log("CyberArk EPM Agent Removal Utility started.");
                Log($"Computer: {Environment.MachineName}");
                Log($"Process architecture: {RuntimeInformationText()}");
                Log($"Administrator: {IsAdministrator()}");

                await DetectAgentAsync();
            };
        }

        private void BuildInterface()
        {
            SuspendLayout();

            Text = "Dexcom | CyberArk EPM Agent Removal Utility";
            StartPosition = FormStartPosition.CenterScreen;

            // Fixed application size
            ClientSize = new Size(1050, 990);
            MinimumSize = new Size(1050, 990);
            MaximumSize = new Size(1050, 990);

            // Prevent resizing
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;

            BackColor = SubtleGray;
            Font = new Font("Arial", 9F, FontStyle.Regular);
            AutoScaleMode = AutoScaleMode.Dpi;

            
            MinimizeBox = true;

            // Main form layout keeps the header, content and footer separated.
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,

                ColumnCount = 1,
                RowCount = 3,

                BackColor = SubtleGray,

                Margin = Padding.Empty,
                Padding = Padding.Empty
            };


            rootLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    125F));

            rootLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            rootLayout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    36F));

            Panel headerPanel = BuildBrandedHeader();
            TableLayoutPanel contentPanel = BuildMainContent();
            Panel footerPanel = BuildBrandedFooter();

            rootLayout.Controls.Add(headerPanel, 0, 0);
            rootLayout.Controls.Add(contentPanel, 0, 1);
            rootLayout.Controls.Add(footerPanel, 0, 2);

            Controls.Clear();
            Controls.Add(rootLayout);

            ResumeLayout(true);
        }
        private Panel BuildBrandedHeader()
        {
            var headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = DexcomGreen,
                Padding = new Padding(28, 14, 28, 14),
                Margin = Padding.Empty
            };

            var headerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = DexcomGreen,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            // Give the application title most of the header
            headerLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 72F));

            // Reserve the right side for Dexcom branding
            headerLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 28F));



            // ============================================================
            // LEFT SIDE - APPLICATION TITLE
            // ============================================================

            var titleLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = DexcomGreen,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            titleLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 58F));

            titleLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 42F));


            var lblTitle = new Label
            {
                Text = "CyberArk EPM Agent Removal Utility",

                ForeColor = Color.White,

                Font = new Font(
                    "Arial",
                    19F,
                    FontStyle.Bold),

                Dock = DockStyle.Fill,

                TextAlign = ContentAlignment.BottomLeft,

                AutoEllipsis = true,

                Margin = Padding.Empty
            };


            var lblSubtitle = new Label
            {
                Text =
                    "Detect the CyberArk EPM agent and request removal through Windows Installer.",

                ForeColor = Color.White,

                Font = new Font(
                    "Arial",
                    9.5F,
                    FontStyle.Regular),

                Dock = DockStyle.Fill,

                TextAlign = ContentAlignment.TopLeft,

                AutoEllipsis = true,

                Padding = new Padding(1, 4, 0, 0),

                Margin = Padding.Empty
            };


            titleLayout.Controls.Add(lblTitle, 0, 0);
            titleLayout.Controls.Add(lblSubtitle, 0, 1);



            // ============================================================
            // RIGHT SIDE - DEXCOM BRANDING
            // ============================================================

            var logoOuterPanel = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor = DexcomGreen,

                Padding = new Padding(
                    30,
                    12,
                    0,
                    12),

                Margin = Padding.Empty
            };


            // White logo card
            var logoCard = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor = Color.White,

                Padding = new Padding(
                    18,
                    12,
                    18,
                    12),

                Margin = Padding.Empty
            };


            try
            {
                var logo = new PictureBox
                {
                    Dock = DockStyle.Fill,

                    Image = LoadDexcomLogo(),

                    SizeMode = PictureBoxSizeMode.Zoom,

                    BackColor = Color.White,

                    Margin = Padding.Empty,

                    TabStop = false
                };

                logoCard.Controls.Add(logo);
            }
            catch
            {
                // Fallback if loading the resource fails.
                var logoFallback = new Label
                {
                    Text = "DEXCOM",

                    Dock = DockStyle.Fill,

                    ForeColor = ContrastGreen,

                    BackColor = Color.White,

                    Font = new Font(
                        "Arial",
                        18F,
                        FontStyle.Bold),

                    TextAlign = ContentAlignment.MiddleCenter,

                    Margin = Padding.Empty
                };

                logoCard.Controls.Add(logoFallback);
            }


            logoOuterPanel.Controls.Add(logoCard);


            // ============================================================
            // ASSEMBLE HEADER
            // ============================================================

            headerLayout.Controls.Add(
                titleLayout,
                0,
                0);

            headerLayout.Controls.Add(
                logoOuterPanel,
                1,
                0);

            headerPanel.Controls.Add(
                headerLayout);


            return headerPanel;
        }
        private TableLayoutPanel BuildMainContent()
        {
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = SubtleGray,
                Padding = new Padding(24, 10, 24, 10),
                Margin = Padding.Empty,
                ColumnCount = 1,
                RowCount = 5,
                AutoScroll = false
            };

            mainPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            // Give every information section enough room.
            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 125F)); // Endpoint Status

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 275F)); // Detected Agent

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 120F)); // Self-defense

            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 65F));  // Buttons

            // Activity Log receives all remaining height.
            mainPanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            var environmentGroup = BuildEnvironmentGroup();
            var agentGroup = BuildAgentGroup();
            var tokenGroup = BuildTokenGroup();
            var actionPanel = BuildActionPanel();
            var activityGroup = BuildActivityGroup();

            environmentGroup.Dock = DockStyle.Fill;
            agentGroup.Dock = DockStyle.Fill;
            tokenGroup.Dock = DockStyle.Fill;
            actionPanel.Dock = DockStyle.Fill;
            activityGroup.Dock = DockStyle.Fill;

            environmentGroup.Margin = new Padding(0, 0, 0, 6);
            agentGroup.Margin = new Padding(0, 0, 0, 6);
            tokenGroup.Margin = new Padding(0, 0, 0, 6);
            actionPanel.Margin = new Padding(0, 0, 0, 6);
            activityGroup.Margin = Padding.Empty;

            mainPanel.Controls.Add(environmentGroup, 0, 0);
            mainPanel.Controls.Add(agentGroup, 0, 1);
            mainPanel.Controls.Add(tokenGroup, 0, 2);
            mainPanel.Controls.Add(actionPanel, 0, 3);
            mainPanel.Controls.Add(activityGroup, 0, 4);

            return mainPanel;
        }
        private GroupBox BuildEnvironmentGroup()
        {
            var group = new GroupBox
            {
                Text = "Endpoint status",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ForeColor = BoldGray,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold),

                // More space below the GroupBox caption
                Padding = new Padding(12, 24, 12, 10),
                Margin = Padding.Empty
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,

                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            // Left label column
            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    145F));

            // Right value column
            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            // IMPORTANT:
            // Give both rows equal available space instead
            // of forcing them into small fixed-height rows.
            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    50F));

            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    50F));


            // =====================================================
            // Computer
            // =====================================================

            var lblComputer = new Label
            {
                Text = "Computer:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,

                ForeColor = BoldGray,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Regular),

                Margin = new Padding(6, 0, 3, 0)
            };

            txtComputer.Text =
                Environment.MachineName;

            txtComputer.ReadOnly = true;

            txtComputer.Anchor =
                AnchorStyles.Left |
                AnchorStyles.Right;

            txtComputer.BackColor =
                SubtleGray;

            txtComputer.ForeColor =
                BoldGray;

            txtComputer.BorderStyle =
                BorderStyle.FixedSingle;

            txtComputer.Font =
                new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold);

            txtComputer.Margin =
                new Padding(3, 2, 3, 2);


            // =====================================================
            // Administrator
            // =====================================================

            var lblAdministrator = new Label
            {
                Text = "Administrator:",
                AutoSize = true,

                Anchor = AnchorStyles.Left,

                ForeColor = BoldGray,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Regular),

                Margin = new Padding(6, 0, 3, 0)
            };


            bool administrator =
                IsAdministrator();


            lblAdminValue.Text =
                administrator
                    ? "Yes - elevated"
                    : "No - Administrator required";

            lblAdminValue.ForeColor =
                administrator
                    ? SuccessGreen
                    : ErrorColor;

            lblAdminValue.Font =
                new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold);

            lblAdminValue.AutoSize = true;

            // Most important part:
            // vertically centers the label in its row.
            lblAdminValue.Anchor =
                AnchorStyles.Left;

            lblAdminValue.Margin =
                new Padding(3, 0, 3, 0);


            // =====================================================
            // Add controls
            // =====================================================

            layout.Controls.Add(
                lblComputer,
                0,
                0);

            layout.Controls.Add(
                txtComputer,
                1,
                0);

            layout.Controls.Add(
                lblAdministrator,
                0,
                1);

            layout.Controls.Add(
                lblAdminValue,
                1,
                1);


            group.Controls.Add(layout);

            return group;
        }

        private GroupBox BuildAgentGroup()
        {
            var group = CreateGroupBox("Detected EPM agent", 275);

            var layout = CreateTwoColumnLayout();
            layout.RowCount = 7;

            AddRow(layout, 0, "Detection status:", lblStatusValue);
            AddRow(layout, 1, "Display name:", txtDisplayName);
            AddRow(layout, 2, "Version:", txtVersion);
            AddRow(layout, 3, "Publisher:", txtPublisher);
            AddRow(layout, 4, "Product GUID:", txtGuid);
            AddRow(layout, 5, "Registry view:", txtRegistryView);
            AddRow(layout, 6, "Install location:", txtInstallLocation);

            ConfigureReadOnlyTextBox(txtDisplayName);
            ConfigureReadOnlyTextBox(txtVersion);
            ConfigureReadOnlyTextBox(txtPublisher);
            ConfigureReadOnlyTextBox(txtGuid);
            ConfigureReadOnlyTextBox(txtRegistryView);
            ConfigureReadOnlyTextBox(txtInstallLocation);

            lblStatusValue.Text = "Not scanned";
            lblStatusValue.ForeColor = WarningColor;

            group.Controls.Add(layout);
            return group;
        }

        private GroupBox BuildTokenGroup()
        {
            var group = CreateGroupBox(
                "Agent self-defense",
                120);

            group.Dock = DockStyle.Fill;
            group.Padding = new Padding(12, 20, 12, 10);


            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            // Label column
            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    145F));

            // Input column
            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));


            // Token textbox row
            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    36F));

            // Checkbox row
            layout.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    32F));


            // -------------------------------------------------
            // Secure token label
            // -------------------------------------------------

            var lblToken = new Label
            {
                Text = "Secure token:",

                AutoSize = true,

                Anchor = AnchorStyles.Left,

                ForeColor = BoldGray,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Regular),

                Margin = new Padding(
                    3,
                    8,
                    3,
                    3)
            };


            // -------------------------------------------------
            // Secure token textbox
            // -------------------------------------------------

            txtSecureToken.Dock = DockStyle.Fill;

            txtSecureToken.UseSystemPasswordChar = true;

            txtSecureToken.BackColor = Color.White;

            txtSecureToken.ForeColor = BoldGray;

            txtSecureToken.BorderStyle =
                BorderStyle.FixedSingle;

            txtSecureToken.Font = new Font(
                "Arial",
                9F,
                FontStyle.Regular);

            txtSecureToken.PlaceholderText =
                "Enter EPM secure token if Agent self-defense is enabled";

            txtSecureToken.Margin =
                new Padding(
                    3,
                    5,
                    3,
                    5);


            // -------------------------------------------------
            // Show Token checkbox
            // -------------------------------------------------

            var chkShowToken = new CheckBox
            {
                Text = "Show token",

                AutoSize = true,

                Anchor = AnchorStyles.Left,

                ForeColor = BoldGray,

                BackColor = Color.White,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold),

                Margin = new Padding(
                    3,
                    2,
                    3,
                    2),

                TabStop = true
            };


            chkShowToken.CheckedChanged += (_, _) =>
            {
                txtSecureToken.UseSystemPasswordChar =
                    !chkShowToken.Checked;
            };


            // -------------------------------------------------
            // Add controls
            // -------------------------------------------------

            layout.Controls.Add(
                lblToken,
                0,
                0);

            layout.Controls.Add(
                txtSecureToken,
                1,
                0);


            // Empty cell under Secure Token label
            layout.Controls.Add(
                new Label(),
                0,
                1);

            layout.Controls.Add(
                chkShowToken,
                1,
                1);


            group.Controls.Add(layout);

            return group;
        }

        private Panel BuildActionPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = SubtleGray,
                Margin = new Padding(0, 2, 0, 8),
                Padding = Padding.Empty
            };

            var buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 46,
                ColumnCount = 7,
                RowCount = 1,
                BackColor = SubtleGray,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 150));

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 12));

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 175));

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 12));

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 130));

            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 12));
            buttonLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 130));

            StyleSecondaryButton(btnDetect);
            btnDetect.Text = "Rescan Agent";
            btnDetect.Dock = DockStyle.Fill;
            btnDetect.Click += async (_, _) => await DetectAgentAsync();

            StyleDangerButton(btnUninstall);
            btnUninstall.Text = "Full Uninstall";
            btnUninstall.Dock = DockStyle.Fill;
            btnUninstall.Enabled = false;
            btnUninstall.Click += async (_, _) => await UninstallAgentAsync();

            StyleSecondaryButton(btnOpenLog);
            btnOpenLog.Text = "Open Log";
            btnOpenLog.Dock = DockStyle.Fill;
            btnOpenLog.Enabled = false;
            btnOpenLog.Click += (_, _) => OpenLog();

            buttonLayout.Controls.Add(btnDetect, 0, 0);
            buttonLayout.Controls.Add(btnUninstall, 2, 0);
            buttonLayout.Controls.Add(btnOpenLog, 4, 0);

            var btnAbout = new Button { Text = "About", Dock = DockStyle.Fill };
            StyleSecondaryButton(btnAbout);
            btnAbout.Click += (_, _) => ShowAbout();
            buttonLayout.Controls.Add(btnAbout, 6, 0);

            progressBar.Dock = DockStyle.Bottom;
            progressBar.Height = 8;
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.ForeColor = DexcomGreen;
            progressBar.BackColor = CoolGray;

            panel.Controls.Add(buttonLayout);
            panel.Controls.Add(progressBar);

            return panel;
        }
        private static void StyleSecondaryButton(Button button)
        {
            button.Height = 40;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = ContrastGreen;
            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(236, 249, 232);

            button.BackColor = Color.White;
            button.ForeColor = ContrastGreen;

            button.Font = new Font(
                "Arial",
                9F,
                FontStyle.Bold);

            button.Cursor = Cursors.Hand;
            button.Margin = Padding.Empty;
            button.UseVisualStyleBackColor = false;
        }

        private static void StyleDangerButton(Button button)
        {
            button.Height = 40;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(145, 30, 30);

            button.BackColor = ErrorColor;
            button.ForeColor = Color.White;

            button.Font = new Font(
                "Arial",
                9F,
                FontStyle.Bold);

            button.Cursor = Cursors.Hand;
            button.Margin = Padding.Empty;
            button.UseVisualStyleBackColor = false;
        }

        private GroupBox BuildActivityGroup()
        {
            var group = new GroupBox
            {
                Text = "Activity log",
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ForeColor = BoldGray,

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold),

                Padding = new Padding(
                    10,
                    22,
                    10,
                    10),

                Margin = Padding.Empty
            };

            txtActivity.Dock =
                DockStyle.Fill;

            txtActivity.ReadOnly =
                true;

            txtActivity.BackColor =
                Color.FromArgb(28, 35, 31);

            txtActivity.ForeColor =
                Color.FromArgb(220, 235, 224);

            txtActivity.Font =
                new Font(
                    "Consolas",
                    8.25F,
                    FontStyle.Regular);

            txtActivity.BorderStyle =
                BorderStyle.FixedSingle;

            txtActivity.WordWrap =
                false;

            txtActivity.ScrollBars =
                RichTextBoxScrollBars.Vertical;

            txtActivity.DetectUrls =
                false;

            txtActivity.TabStop =
                false;

            group.Controls.Add(txtActivity);

            return group;
        }
        private Panel BuildBrandedFooter()
        {
            var footer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ContrastGreen,
                Padding = new Padding(24, 0, 24, 0),
                Margin = Padding.Empty
            };

            var footerLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = ContrastGreen,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            footerLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 70));

            footerLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 30));

            var lblFooterLeft = new Label
            {
                Text = "IT-Infosec-Manila | Developed by Keith Galleros",
                ForeColor = Color.White,
                Font = new Font(
                    "Arial",
                    8.5F,
                    FontStyle.Regular),

                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblFooterRight = new Label
            {
                Text = "DEXCOM INTERNAL USE",
                ForeColor = Color.White,
                Font = new Font(
                    "Arial",
                    8.5F,
                    FontStyle.Bold),

                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            footerLayout.Controls.Add(lblFooterLeft, 0, 0);
            footerLayout.Controls.Add(lblFooterRight, 1, 0);

            footer.Controls.Add(footerLayout);

            return footer;
        }
        private async Task DetectAgentAsync()
        {
            SetBusy(true, "Scanning Windows uninstall registry...");
            detectedAgent = null;
            ClearAgentInformation();

            try
            {
                Log("Scanning 64-bit and 32-bit uninstall registry views.");

                List<EpmAgentInfo> agents = await Task.Run(FindEpmAgents);

                if (agents.Count == 0)
                {
                    detectedAgent = null;
                    lblStatusValue.Text = "Not detected";
                    lblStatusValue.ForeColor = WarningColor;
                    btnUninstall.Enabled = false;

                    Log("No supported CyberArk or Idira EPM agent registration was found.");
                    return;
                }

                if (agents.Count > 1)
                {
                    Log($"Multiple EPM-related uninstall registrations were found: {agents.Count}.");
                    Log("The first valid MSI product registration will be displayed.");
                }

                detectedAgent = agents[0];

                lblStatusValue.Text = "Detected";
                lblStatusValue.ForeColor = SuccessGreen;

                txtDisplayName.Text = detectedAgent.DisplayName;
                txtVersion.Text = detectedAgent.DisplayVersion;
                txtPublisher.Text = detectedAgent.Publisher;
                txtGuid.Text = detectedAgent.ProductCode;
                txtRegistryView.Text = detectedAgent.RegistryView;
                txtInstallLocation.Text = detectedAgent.InstallLocation;

                btnUninstall.Enabled = IsAdministrator();

                Log($"Detected: {detectedAgent.DisplayName}");
                Log($"Version: {ValueOrUnknown(detectedAgent.DisplayVersion)}");
                Log($"Product GUID: {detectedAgent.ProductCode}");
                Log($"Registry view: {detectedAgent.RegistryView}");

                if (!IsAdministrator())
                {
                    Log("Uninstall disabled because the utility is not elevated.");
                }
            }
            catch (Exception ex)
            {
                detectedAgent = null;
                lblStatusValue.Text = "Detection failed";
                lblStatusValue.ForeColor = ErrorColor;
                btnUninstall.Enabled = false;

                Log($"Detection error: {ex.Message}");
                ShowError("The EPM detection scan failed.", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private List<EpmAgentInfo> FindEpmAgents()
        {
            var results = new List<EpmAgentInfo>();

            ScanRegistryView(RegistryView.Registry64, results);
            ScanRegistryView(RegistryView.Registry32, results);

            return results
                .GroupBy(item => item.ProductCode, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderByDescending(item => IsExactExpectedName(item.DisplayName))
                .ThenBy(item => item.DisplayName)
                .ToList();
        }

        private static void ScanRegistryView(
            RegistryView registryView,
            List<EpmAgentInfo> results)
        {
            const string uninstallPath =
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            using RegistryKey baseKey =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);

            using RegistryKey? uninstallKey =
                baseKey.OpenSubKey(uninstallPath, writable: false);

            if (uninstallKey == null)
            {
                return;
            }

            foreach (string subKeyName in uninstallKey.GetSubKeyNames())
            {
                using RegistryKey? productKey =
                    uninstallKey.OpenSubKey(subKeyName, writable: false);

                if (productKey == null)
                {
                    continue;
                }

                string displayName =
                    productKey.GetValue("DisplayName")?.ToString()?.Trim()
                    ?? string.Empty;

                if (!IsSupportedEpmDisplayName(displayName))
                {
                    continue;
                }

                string productCode = ExtractProductCode(
                    subKeyName,
                    productKey.GetValue("UninstallString")?.ToString());

                if (string.IsNullOrWhiteSpace(productCode))
                {
                    continue;
                }

                results.Add(new EpmAgentInfo
                {
                    DisplayName = displayName,
                    DisplayVersion =
                        productKey.GetValue("DisplayVersion")?.ToString()?.Trim()
                        ?? string.Empty,

                    Publisher =
                        productKey.GetValue("Publisher")?.ToString()?.Trim()
                        ?? string.Empty,

                    InstallLocation =
                        productKey.GetValue("InstallLocation")?.ToString()?.Trim()
                        ?? string.Empty,

                    ProductCode = productCode,
                    RegistryView = registryView.ToString()
                });
            }
        }

        private async Task UninstallAgentAsync()
        {
            if (detectedAgent == null)
            {
                MessageBox.Show(
                    this,
                    "No supported EPM agent is currently selected.",
                    "Agent not detected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (!IsAdministrator())
            {
                MessageBox.Show(
                    this,
                    "Close the utility and restart it by using Run as administrator.",
                    "Administrator privileges required",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            string productCode = detectedAgent.ProductCode;
            string agentName = detectedAgent.DisplayName;
            string version = ValueOrUnknown(detectedAgent.DisplayVersion);

            string tokenNotice = string.IsNullOrWhiteSpace(txtSecureToken.Text)
                ? "No secure token has been entered. If Agent self-defense is enabled, the uninstall may fail."
                : "A secure token will be supplied to Windows Installer.";

            DialogResult result = MessageBox.Show(
                this,
                $"You are about to fully uninstall:\n\n" +
                $"Agent: {agentName}\n" +
                $"Version: {version}\n" +
                $"GUID: {productCode}\n\n" +
                $"{tokenNotice}\n\n" +
                "CyberArk states that a full uninstall removes EPM files and data. " +
                "The endpoint may also lose its previous custom group or policy targeting.\n\n" +
                "Continue with the full uninstall?",
                "Confirm full EPM uninstall",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                Log("Uninstall canceled by the operator.");
                return;
            }

            btnUninstall.Enabled = false;
            btnDetect.Enabled = false;
            txtSecureToken.Enabled = false;
            progressBar.Style = ProgressBarStyle.Marquee;

            try
            {
                Log("Starting vendor-supported full EPM uninstall.");
                Log($"Target GUID: {productCode}");

                string arguments = BuildMsiexecArguments(
                    productCode,
                    txtSecureToken.Text);

                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Path.Combine(
                            Environment.SystemDirectory,
                            "msiexec.exe"),

                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                int exitCode = process.ExitCode;
                Log($"Windows Installer exit code: {exitCode}");

                switch (exitCode)
                {
                    case 0:
                        Log("Windows Installer reports that the uninstall completed successfully.");
                        break;

                    case 3010:
                        Log("Windows Installer reports success and requires a restart.");
                        break;

                    case 1603:
                        Log("Windows Installer reported a fatal installation error.");
                        Log("Possible causes include Agent self-defense, an invalid token, or an MSI failure.");
                        break;

                    case 1618:
                        Log("Another Windows Installer transaction is already running.");
                        break;

                    default:
                        Log("Windows Installer returned a non-success result.");
                        break;
                }

                // Clear the input as soon as the process has ended.
                txtSecureToken.Clear();

                await Task.Delay(1500);

                List<EpmAgentInfo> remainingAgents =
                    await Task.Run(FindEpmAgents);

                bool stillRegistered = remainingAgents.Any(
                    item => string.Equals(
                        item.ProductCode,
                        productCode,
                        StringComparison.OrdinalIgnoreCase));

                if ((exitCode == 0 || exitCode == 3010) && !stillRegistered)
                {
                    detectedAgent = null;
                    ClearAgentInformation();

                    lblStatusValue.Text = "Removal verified";
                    lblStatusValue.ForeColor = SuccessGreen;

                    Log("Verification passed: the selected EPM product registration is no longer present.");
                    Log("Restart Windows before attempting to install a replacement EPM agent.");

                    MessageBox.Show(
                        this,
                        "The EPM uninstall completed and its selected MSI registration is no longer present.\n\n" +
                        "Restart Windows before reinstalling the EPM agent.",
                        "Uninstall completed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    Log("Verification did not confirm complete removal.");
                    Log("Do not manually delete drivers, services, folders or registry keys from this utility.");

                    if (stillRegistered)
                    {
                        Log("The EPM MSI product registration is still present.");
                    }

                    MessageBox.Show(
                        this,
                        "Complete removal could not be verified.\n\n" +
                        "Review the activity log. If Agent self-defense is enabled, obtain a valid unique secure token and try again. " +
                        "If the vendor-supported uninstall continues to fail, use the approved CyberArk cleanup/escalation procedure.",
                        "Removal not verified",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    await DetectAgentAsync();
                }
            }
            catch (Exception ex)
            {
                Log($"Uninstall error: {ex.Message}");
                ShowError("The EPM uninstall failed.", ex);
            }
            finally
            {
                txtSecureToken.Clear();
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;

                btnDetect.Enabled = true;
                txtSecureToken.Enabled = true;
                btnUninstall.Enabled =
                    detectedAgent != null && IsAdministrator();
            }
        }

        private void ShowAbout()
        {
            using var dialog = new AboutForm();
            dialog.ShowDialog(this);
        }

        private static string BuildMsiexecArguments(
            string productCode,
            string secureToken)
        {
            if (!IsValidProductCode(productCode))
            {
                throw new InvalidOperationException(
                    "The detected MSI product GUID is invalid.");
            }

            var arguments = new StringBuilder();

            arguments.Append("/x ");
            arguments.Append(productCode);
            arguments.Append(" FULL_UNINSTALL=\"YES\"");

            if (!string.IsNullOrWhiteSpace(secureToken))
            {
                string sanitizedToken = secureToken.Trim();

                if (sanitizedToken.Contains('"') ||
                    sanitizedToken.Contains('\r') ||
                    sanitizedToken.Contains('\n'))
                {
                    throw new InvalidOperationException(
                        "The secure token contains an invalid character.");
                }

                arguments.Append(" SECURE_TOKEN=\"");
                arguments.Append(sanitizedToken);
                arguments.Append('"');
            }

            // CyberArk documents SECURE_TOKEN for silent uninstall.
            arguments.Append(" /qn /norestart");

            return arguments.ToString();
        }

        private static bool IsSupportedEpmDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            string normalized = displayName.Trim();

            return normalized.Contains(
                       "CyberArk Endpoint Privilege Manager Agent",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   normalized.Contains(
                       "Idira Endpoint Privilege Manager Agent",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   normalized.Contains(
                       "Endpoint Privilege Manager Agent",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsExactExpectedName(string displayName)
        {
            return displayName.Equals(
                       "CyberArk Endpoint Privilege Manager Agent",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   displayName.Equals(
                       "Idira Endpoint Privilege Manager Agent",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtractProductCode(
            string registrySubKeyName,
            string? uninstallString)
        {
            if (IsValidProductCode(registrySubKeyName))
            {
                return registrySubKeyName.ToUpperInvariant();
            }

            Match match = Regex.Match(
                uninstallString ?? string.Empty,
                @"\{[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\}");

            return match.Success
                ? match.Value.ToUpperInvariant()
                : string.Empty;
        }

        private static bool IsValidProductCode(string value)
        {
            return Regex.IsMatch(
                value ?? string.Empty,
                @"^\{[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}\}$");
        }

        private static bool IsAdministrator()
        {
            using WindowsIdentity identity =
                WindowsIdentity.GetCurrent();

            var principal = new WindowsPrincipal(identity);

            return principal.IsInRole(
                WindowsBuiltInRole.Administrator);
        }

        private void InitializeLog()
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.CommonApplicationData),
                    "CyberArkEPMRemovalTool",
                    "Logs");

                Directory.CreateDirectory(folder);

                logFilePath = Path.Combine(
                    folder,
                    $"EPMRemoval_{Environment.MachineName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");

                File.WriteAllText(
                    logFilePath,
                    "CyberArk EPM Agent Removal Utility" +
                    Environment.NewLine,
                    Encoding.UTF8);

                btnOpenLog.Enabled = true;
            }
            catch
            {
                logFilePath = string.Empty;
                btnOpenLog.Enabled = false;
            }
        }

        private void Log(string message)
        {
            string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            txtActivity.AppendText(line + Environment.NewLine);
            txtActivity.SelectionStart = txtActivity.TextLength;
            txtActivity.ScrollToCaret();

            if (!string.IsNullOrWhiteSpace(logFilePath))
            {
                try
                {
                    File.AppendAllText(
                        logFilePath,
                        line + Environment.NewLine,
                        Encoding.UTF8);
                }
                catch
                {
                    // Continue operating even if logging becomes unavailable.
                }
            }
        }

        private void OpenLog()
        {
            if (string.IsNullOrWhiteSpace(logFilePath) ||
                !File.Exists(logFilePath))
            {
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = logFilePath,
                UseShellExecute = true
            });
        }

        private void ClearAgentInformation()
        {
            txtDisplayName.Clear();
            txtVersion.Clear();
            txtPublisher.Clear();
            txtGuid.Clear();
            txtRegistryView.Clear();
            txtInstallLocation.Clear();
        }

        private void SetBusy(bool busy, string? status = null)
        {
            btnDetect.Enabled = !busy;
            btnUninstall.Enabled =
                !busy && detectedAgent != null && IsAdministrator();

            if (busy)
            {
                progressBar.Style = ProgressBarStyle.Marquee;

                if (!string.IsNullOrWhiteSpace(status))
                {
                    lblStatusValue.Text = status;
                    lblStatusValue.ForeColor = ContrastGreen;
                }
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
            }
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show(
                this,
                $"{message}\n\n{ex.Message}",
                "CyberArk EPM Removal Utility",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static GroupBox CreateGroupBox(
     string title,
     int height)
        {
            return new GroupBox
            {
                Text = title,

                Dock = DockStyle.Fill,

                Height = height,

                Padding = new Padding(
                    12,
                    18,
                    12,
                    10),

                Margin = new Padding(
                    0,
                    0,
                    0,
                    8),

                Font = new Font(
                    "Arial",
                    9F,
                    FontStyle.Bold),

                ForeColor = BoldGray,

                BackColor = Color.White
            };
        }

        private static TableLayoutPanel CreateTwoColumnLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                AutoSize = false,
                Padding = new Padding(3)
            };

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 145));

            layout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            return layout;
        }

        private static void AddRow(
            TableLayoutPanel layout,
            int row,
            string labelText,
            Control control)
        {
            while (layout.RowStyles.Count <= row)
            {
                layout.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 31));
            }

            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 9F)
            };

            control.Anchor =
                AnchorStyles.Left |
                AnchorStyles.Right |
                AnchorStyles.Top;

            control.Margin = new Padding(3, 3, 3, 3);

            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(control, 1, row);
        }

        private static void ConfigureReadOnlyTextBox(TextBox textBox)
        {
            textBox.ReadOnly = true;
            textBox.Dock = DockStyle.Fill;
            textBox.BackColor = SubtleGray;
            textBox.ForeColor = BoldGray;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            textBox.Font = new Font(
                "Arial",
                9F,
                FontStyle.Regular);

            textBox.TabStop = false;
        }

        private static string ValueOrUnknown(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "Not reported"
                : value;
        }

        private static string RuntimeInformationText()
        {
            return Environment.Is64BitProcess
                ? "64-bit"
                : "32-bit";
        }
        private static Image LoadDexcomLogo()
        {
            using var stream =
                new MemoryStream(Properties.Resources.DexcomLogo);

            using var source = Image.FromStream(stream);

            return new Bitmap(source);
        }

        private sealed class EpmAgentInfo
        {
            public string DisplayName { get; init; } = string.Empty;
            public string DisplayVersion { get; init; } = string.Empty;
            public string Publisher { get; init; } = string.Empty;
            public string ProductCode { get; init; } = string.Empty;
            public string RegistryView { get; init; } = string.Empty;
            public string InstallLocation { get; init; } = string.Empty;
        }
    }
}
