namespace SimpleAIChat;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null!;

    private TabControl tabSettings;
    private TabPage tabModels;
    private TabPage tabPresets;

    // ---- Models tab ----
    private ListBox lstModels;

    private Label lblProvider;
    private TextBox txtProvider;

    private Label lblModelName;
    private TextBox txtModelName;

    private Label lblModelType;
    private TextBox txtModelType;

    private Label lblEndpoint;
    private TextBox txtEndpoint;

    private Label lblApiKeyVariable;
    private TextBox txtApiKeyVariable;

    private Label lblTimeout;
    private NumericUpDown numTimeout;

    private Label lblTemperature;
    private NumericUpDown numTemperature;

    private Label lblMaxTokens;
    private NumericUpDown numMaxTokens;

    private CheckBox chkModelEnabled;
    private CheckBox chkUseProxy;

    private Label lblProxyHost;
    private TextBox txtProxyHost;

    private Label lblProxyPort;
    private NumericUpDown numProxyPort;

    private Label lblRemarks;
    private TextBox txtRemarks;

    private Button btnAddModel;
    private Button btnDeleteModel;

    // ---- Presets tab ----
    private ListBox lstPresets;

    private Label lblPresetId;
    private TextBox txtPresetId;

    private Label lblPresetName;
    private TextBox txtPresetName;

    private Label lblPresetSystemPrompt;
    private TextBox txtPresetSystemPrompt;

    private Label lblPresetKnowledgeFile;
    private TextBox txtPresetKnowledgeFile;
    private Button btnBrowseKnowledgeFile;

    private Label lblPresetTemperature;
    private CheckBox chkPresetTemperature;
    private NumericUpDown numPresetTemperature;

    private Label lblPresetMaxTokens;
    private CheckBox chkPresetMaxTokens;
    private NumericUpDown numPresetMaxTokens;

    private Label lblPresetTopP;
    private CheckBox chkPresetTopP;
    private NumericUpDown numPresetTopP;

    private Button btnAddPreset;
    private Button btnDeletePreset;

    // ---- bottom ----
    private Button btnSave;
    private Button btnCancel;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            components?.Dispose();

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tabSettings = new TabControl();
        tabModels = new TabPage();
        lstModels = new ListBox();
        lblModelId = new Label();
        lblProvider = new Label();
        txtModelId = new TextBox();
        txtProvider = new TextBox();
        lblModelName = new Label();
        txtModelName = new TextBox();
        lblModelType = new Label();
        txtModelType = new TextBox();
        lblEndpoint = new Label();
        txtEndpoint = new TextBox();
        lblApiKeyVariable = new Label();
        txtApiKeyVariable = new TextBox();
        lblTimeout = new Label();
        numTimeout = new NumericUpDown();
        lblTemperature = new Label();
        numTemperature = new NumericUpDown();
        lblContextWindow = new Label();
        lblMaxTokens = new Label();
        numContextWindow = new NumericUpDown();
        numMaxTokens = new NumericUpDown();
        chkModelEnabled = new CheckBox();
        chkUseProxy = new CheckBox();
        lblProxyHost = new Label();
        txtProxyHost = new TextBox();
        lblProxyPort = new Label();
        numProxyPort = new NumericUpDown();
        lblRemarks = new Label();
        txtRemarks = new TextBox();
        btnAddModel = new Button();
        button1 = new Button();
        btnModelDown = new Button();
        btnDeleteModel = new Button();
        tabPresets = new TabPage();
        cmbPresetOutputFormat = new ComboBox();
        btnPresetUp = new Button();
        btnPresetDown = new Button();
        lstPresets = new ListBox();
        lblPresetId = new Label();
        txtPresetId = new TextBox();
        lblPresetName = new Label();
        txtPresetName = new TextBox();
        lblPresetSystemPrompt = new Label();
        txtPresetSystemPrompt = new TextBox();
        lblPresetOutputFormat = new Label();
        lblPresetKnowledgeFile = new Label();
        txtPresetKnowledgeFile = new TextBox();
        btnBrowseKnowledgeFile = new Button();
        lblPresetTemperature = new Label();
        chkPresetTemperature = new CheckBox();
        numPresetTemperature = new NumericUpDown();
        lblPresetMaxTokens = new Label();
        chkPresetMaxTokens = new CheckBox();
        numPresetMaxTokens = new NumericUpDown();
        lblPresetTopP = new Label();
        chkPresetTopP = new CheckBox();
        numPresetTopP = new NumericUpDown();
        btnAddPreset = new Button();
        btnDeletePreset = new Button();
        btnSave = new Button();
        btnCancel = new Button();
        chkSuppotTools = new CheckBox();
        tabSettings.SuspendLayout();
        tabModels.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numTimeout).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numTemperature).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numContextWindow).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numMaxTokens).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numProxyPort).BeginInit();
        tabPresets.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numPresetTemperature).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numPresetMaxTokens).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numPresetTopP).BeginInit();
        SuspendLayout();
        // 
        // tabSettings
        // 
        tabSettings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabSettings.Controls.Add(tabModels);
        tabSettings.Controls.Add(tabPresets);
        tabSettings.Location = new Point(12, 12);
        tabSettings.Margin = new Padding(3, 2, 3, 2);
        tabSettings.Name = "tabSettings";
        tabSettings.SelectedIndex = 0;
        tabSettings.Size = new Size(875, 585);
        tabSettings.TabIndex = 0;
        // 
        // tabModels
        // 
        tabModels.Controls.Add(lstModels);
        tabModels.Controls.Add(lblModelId);
        tabModels.Controls.Add(lblProvider);
        tabModels.Controls.Add(txtModelId);
        tabModels.Controls.Add(txtProvider);
        tabModels.Controls.Add(lblModelName);
        tabModels.Controls.Add(txtModelName);
        tabModels.Controls.Add(lblModelType);
        tabModels.Controls.Add(txtModelType);
        tabModels.Controls.Add(lblEndpoint);
        tabModels.Controls.Add(txtEndpoint);
        tabModels.Controls.Add(lblApiKeyVariable);
        tabModels.Controls.Add(txtApiKeyVariable);
        tabModels.Controls.Add(lblTimeout);
        tabModels.Controls.Add(numTimeout);
        tabModels.Controls.Add(lblTemperature);
        tabModels.Controls.Add(numTemperature);
        tabModels.Controls.Add(lblContextWindow);
        tabModels.Controls.Add(lblMaxTokens);
        tabModels.Controls.Add(numContextWindow);
        tabModels.Controls.Add(numMaxTokens);
        tabModels.Controls.Add(chkModelEnabled);
        tabModels.Controls.Add(chkSuppotTools);
        tabModels.Controls.Add(chkUseProxy);
        tabModels.Controls.Add(lblProxyHost);
        tabModels.Controls.Add(txtProxyHost);
        tabModels.Controls.Add(lblProxyPort);
        tabModels.Controls.Add(numProxyPort);
        tabModels.Controls.Add(lblRemarks);
        tabModels.Controls.Add(txtRemarks);
        tabModels.Controls.Add(btnAddModel);
        tabModels.Controls.Add(button1);
        tabModels.Controls.Add(btnModelDown);
        tabModels.Controls.Add(btnDeleteModel);
        tabModels.Location = new Point(4, 24);
        tabModels.Margin = new Padding(3, 2, 3, 2);
        tabModels.Name = "tabModels";
        tabModels.Padding = new Padding(10);
        tabModels.Size = new Size(867, 557);
        tabModels.TabIndex = 0;
        tabModels.Text = "Models";
        // 
        // lstModels
        // 
        lstModels.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lstModels.FormattingEnabled = true;
        lstModels.ItemHeight = 15;
        lstModels.Location = new Point(10, 10);
        lstModels.Margin = new Padding(3, 2, 3, 2);
        lstModels.Name = "lstModels";
        lstModels.Size = new Size(228, 499);
        lstModels.TabIndex = 0;
        lstModels.SelectedIndexChanged += lstModels_SelectedIndexChanged;
        // 
        // lblModelId
        // 
        lblModelId.AutoSize = true;
        lblModelId.Location = new Point(340, 15);
        lblModelId.Name = "lblModelId";
        lblModelId.Size = new Size(57, 15);
        lblModelId.TabIndex = 1;
        lblModelId.Text = "Model Id:";
        // 
        // lblProvider
        // 
        lblProvider.AutoSize = true;
        lblProvider.Location = new Point(341, 46);
        lblProvider.Name = "lblProvider";
        lblProvider.Size = new Size(54, 15);
        lblProvider.TabIndex = 1;
        lblProvider.Text = "Provider:";
        // 
        // txtModelId
        // 
        txtModelId.Location = new Point(404, 12);
        txtModelId.Margin = new Padding(3, 2, 3, 2);
        txtModelId.Name = "txtModelId";
        txtModelId.Size = new Size(438, 23);
        txtModelId.TabIndex = 2;
        // 
        // txtProvider
        // 
        txtProvider.Location = new Point(405, 43);
        txtProvider.Margin = new Padding(3, 2, 3, 2);
        txtProvider.Name = "txtProvider";
        txtProvider.Size = new Size(438, 23);
        txtProvider.TabIndex = 2;
        // 
        // lblModelName
        // 
        lblModelName.AutoSize = true;
        lblModelName.Location = new Point(316, 76);
        lblModelName.Name = "lblModelName";
        lblModelName.Size = new Size(79, 15);
        lblModelName.TabIndex = 3;
        lblModelName.Text = "Model Name:";
        // 
        // txtModelName
        // 
        txtModelName.Location = new Point(405, 73);
        txtModelName.Margin = new Padding(3, 2, 3, 2);
        txtModelName.Name = "txtModelName";
        txtModelName.Size = new Size(438, 23);
        txtModelName.TabIndex = 4;
        // 
        // lblModelType
        // 
        lblModelType.AutoSize = true;
        lblModelType.Location = new Point(360, 106);
        lblModelType.Name = "lblModelType";
        lblModelType.Size = new Size(35, 15);
        lblModelType.TabIndex = 5;
        lblModelType.Text = "Type:";
        // 
        // txtModelType
        // 
        txtModelType.Location = new Point(405, 103);
        txtModelType.Margin = new Padding(3, 2, 3, 2);
        txtModelType.Name = "txtModelType";
        txtModelType.Size = new Size(438, 23);
        txtModelType.TabIndex = 6;
        // 
        // lblEndpoint
        // 
        lblEndpoint.AutoSize = true;
        lblEndpoint.Location = new Point(337, 136);
        lblEndpoint.Name = "lblEndpoint";
        lblEndpoint.Size = new Size(58, 15);
        lblEndpoint.TabIndex = 7;
        lblEndpoint.Text = "Endpoint:";
        // 
        // txtEndpoint
        // 
        txtEndpoint.Location = new Point(405, 133);
        txtEndpoint.Margin = new Padding(3, 2, 3, 2);
        txtEndpoint.Name = "txtEndpoint";
        txtEndpoint.Size = new Size(438, 23);
        txtEndpoint.TabIndex = 8;
        // 
        // lblApiKeyVariable
        // 
        lblApiKeyVariable.AutoSize = true;
        lblApiKeyVariable.Location = new Point(301, 166);
        lblApiKeyVariable.Name = "lblApiKeyVariable";
        lblApiKeyVariable.Size = new Size(94, 15);
        lblApiKeyVariable.TabIndex = 9;
        lblApiKeyVariable.Text = "API Key Variable:";
        // 
        // txtApiKeyVariable
        // 
        txtApiKeyVariable.Location = new Point(405, 163);
        txtApiKeyVariable.Margin = new Padding(3, 2, 3, 2);
        txtApiKeyVariable.Name = "txtApiKeyVariable";
        txtApiKeyVariable.Size = new Size(438, 23);
        txtApiKeyVariable.TabIndex = 10;
        // 
        // lblTimeout
        // 
        lblTimeout.AutoSize = true;
        lblTimeout.Location = new Point(312, 196);
        lblTimeout.Name = "lblTimeout";
        lblTimeout.Size = new Size(83, 15);
        lblTimeout.TabIndex = 11;
        lblTimeout.Text = "Timeout (sec):";
        // 
        // numTimeout
        // 
        numTimeout.Location = new Point(405, 193);
        numTimeout.Margin = new Padding(3, 2, 3, 2);
        numTimeout.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
        numTimeout.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numTimeout.Name = "numTimeout";
        numTimeout.Size = new Size(122, 23);
        numTimeout.TabIndex = 12;
        numTimeout.Value = new decimal(new int[] { 120, 0, 0, 0 });
        // 
        // lblTemperature
        // 
        lblTemperature.AutoSize = true;
        lblTemperature.Location = new Point(318, 226);
        lblTemperature.Name = "lblTemperature";
        lblTemperature.Size = new Size(77, 15);
        lblTemperature.TabIndex = 13;
        lblTemperature.Text = "Temperature:";
        // 
        // numTemperature
        // 
        numTemperature.DecimalPlaces = 2;
        numTemperature.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
        numTemperature.Location = new Point(405, 223);
        numTemperature.Margin = new Padding(3, 2, 3, 2);
        numTemperature.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
        numTemperature.Name = "numTemperature";
        numTemperature.Size = new Size(122, 23);
        numTemperature.TabIndex = 14;
        // 
        // lblContextWindow
        // 
        lblContextWindow.AutoSize = true;
        lblContextWindow.Location = new Point(257, 289);
        lblContextWindow.Name = "lblContextWindow";
        lblContextWindow.Size = new Size(138, 15);
        lblContextWindow.TabIndex = 15;
        lblContextWindow.Text = "Context Window Tokens:";
        // 
        // lblMaxTokens
        // 
        lblMaxTokens.AutoSize = true;
        lblMaxTokens.Location = new Point(323, 256);
        lblMaxTokens.Name = "lblMaxTokens";
        lblMaxTokens.Size = new Size(72, 15);
        lblMaxTokens.TabIndex = 15;
        lblMaxTokens.Text = "Max Tokens:";
        // 
        // numContextWindow
        // 
        numContextWindow.Location = new Point(405, 285);
        numContextWindow.Margin = new Padding(3, 2, 3, 2);
        numContextWindow.Maximum = new decimal(new int[] { 2000000, 0, 0, 0 });
        numContextWindow.Minimum = new decimal(new int[] { 512, 0, 0, 0 });
        numContextWindow.Name = "numContextWindow";
        numContextWindow.Size = new Size(122, 23);
        numContextWindow.TabIndex = 16;
        numContextWindow.Value = new decimal(new int[] { 512, 0, 0, 0 });
        // 
        // numMaxTokens
        // 
        numMaxTokens.Location = new Point(405, 253);
        numMaxTokens.Margin = new Padding(3, 2, 3, 2);
        numMaxTokens.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numMaxTokens.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numMaxTokens.Name = "numMaxTokens";
        numMaxTokens.Size = new Size(122, 23);
        numMaxTokens.TabIndex = 16;
        numMaxTokens.Value = new decimal(new int[] { 500, 0, 0, 0 });
        // 
        // chkModelEnabled
        // 
        chkModelEnabled.AutoSize = true;
        chkModelEnabled.Location = new Point(405, 323);
        chkModelEnabled.Margin = new Padding(3, 2, 3, 2);
        chkModelEnabled.Name = "chkModelEnabled";
        chkModelEnabled.Size = new Size(68, 19);
        chkModelEnabled.TabIndex = 17;
        chkModelEnabled.Text = "Enabled";
        // 
        // chkUseProxy
        // 
        chkUseProxy.AutoSize = true;
        chkUseProxy.Location = new Point(501, 323);
        chkUseProxy.Margin = new Padding(3, 2, 3, 2);
        chkUseProxy.Name = "chkUseProxy";
        chkUseProxy.Size = new Size(77, 19);
        chkUseProxy.TabIndex = 18;
        chkUseProxy.Text = "Use Proxy";
        chkUseProxy.CheckedChanged += chkUseProxy_CheckedChanged;
        // 
        // lblProxyHost
        // 
        lblProxyHost.AutoSize = true;
        lblProxyHost.Location = new Point(328, 353);
        lblProxyHost.Name = "lblProxyHost";
        lblProxyHost.Size = new Size(67, 15);
        lblProxyHost.TabIndex = 19;
        lblProxyHost.Text = "Proxy Host:";
        // 
        // txtProxyHost
        // 
        txtProxyHost.Location = new Point(405, 350);
        txtProxyHost.Margin = new Padding(3, 2, 3, 2);
        txtProxyHost.Name = "txtProxyHost";
        txtProxyHost.Size = new Size(263, 23);
        txtProxyHost.TabIndex = 20;
        // 
        // lblProxyPort
        // 
        lblProxyPort.AutoSize = true;
        lblProxyPort.Location = new Point(676, 353);
        lblProxyPort.Name = "lblProxyPort";
        lblProxyPort.Size = new Size(32, 15);
        lblProxyPort.TabIndex = 21;
        lblProxyPort.Text = "Port:";
        // 
        // numProxyPort
        // 
        numProxyPort.Location = new Point(720, 350);
        numProxyPort.Margin = new Padding(3, 2, 3, 2);
        numProxyPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        numProxyPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numProxyPort.Name = "numProxyPort";
        numProxyPort.Size = new Size(122, 23);
        numProxyPort.TabIndex = 22;
        numProxyPort.Value = new decimal(new int[] { 1088, 0, 0, 0 });
        // 
        // lblRemarks
        // 
        lblRemarks.AutoSize = true;
        lblRemarks.Location = new Point(340, 383);
        lblRemarks.Name = "lblRemarks";
        lblRemarks.Size = new Size(55, 15);
        lblRemarks.TabIndex = 23;
        lblRemarks.Text = "Remarks:";
        // 
        // txtRemarks
        // 
        txtRemarks.Location = new Point(405, 380);
        txtRemarks.Margin = new Padding(3, 2, 3, 2);
        txtRemarks.Name = "txtRemarks";
        txtRemarks.Size = new Size(438, 23);
        txtRemarks.TabIndex = 24;
        // 
        // btnAddModel
        // 
        btnAddModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnAddModel.Font = new Font("Segoe UI", 12F);
        btnAddModel.Location = new Point(10, 521);
        btnAddModel.Margin = new Padding(3, 2, 3, 2);
        btnAddModel.Name = "btnAddModel";
        btnAddModel.Size = new Size(64, 30);
        btnAddModel.TabIndex = 25;
        btnAddModel.Text = "Add";
        btnAddModel.Click += btnAddModel_Click;
        // 
        // button1
        // 
        button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        button1.Font = new Font("Segoe UI", 12F);
        button1.Location = new Point(201, 521);
        button1.Margin = new Padding(3, 2, 3, 2);
        button1.Name = "button1";
        button1.Size = new Size(32, 30);
        button1.TabIndex = 26;
        button1.Text = "▲";
        button1.Click += btnModelUp_Click;
        // 
        // btnModelDown
        // 
        btnModelDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnModelDown.Font = new Font("Segoe UI", 12F);
        btnModelDown.Location = new Point(163, 521);
        btnModelDown.Margin = new Padding(3, 2, 3, 2);
        btnModelDown.Name = "btnModelDown";
        btnModelDown.Size = new Size(32, 30);
        btnModelDown.TabIndex = 26;
        btnModelDown.Text = "▼";
        btnModelDown.Click += btnModelDown_Click;
        // 
        // btnDeleteModel
        // 
        btnDeleteModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeleteModel.Font = new Font("Segoe UI", 12F);
        btnDeleteModel.Location = new Point(83, 521);
        btnDeleteModel.Margin = new Padding(3, 2, 3, 2);
        btnDeleteModel.Name = "btnDeleteModel";
        btnDeleteModel.Size = new Size(64, 30);
        btnDeleteModel.TabIndex = 26;
        btnDeleteModel.Text = "Delete";
        btnDeleteModel.Click += btnDeleteModel_Click;
        // 
        // tabPresets
        // 
        tabPresets.Controls.Add(cmbPresetOutputFormat);
        tabPresets.Controls.Add(btnPresetUp);
        tabPresets.Controls.Add(btnPresetDown);
        tabPresets.Controls.Add(lstPresets);
        tabPresets.Controls.Add(lblPresetId);
        tabPresets.Controls.Add(txtPresetId);
        tabPresets.Controls.Add(lblPresetName);
        tabPresets.Controls.Add(txtPresetName);
        tabPresets.Controls.Add(lblPresetSystemPrompt);
        tabPresets.Controls.Add(txtPresetSystemPrompt);
        tabPresets.Controls.Add(lblPresetOutputFormat);
        tabPresets.Controls.Add(lblPresetKnowledgeFile);
        tabPresets.Controls.Add(txtPresetKnowledgeFile);
        tabPresets.Controls.Add(btnBrowseKnowledgeFile);
        tabPresets.Controls.Add(lblPresetTemperature);
        tabPresets.Controls.Add(chkPresetTemperature);
        tabPresets.Controls.Add(numPresetTemperature);
        tabPresets.Controls.Add(lblPresetMaxTokens);
        tabPresets.Controls.Add(chkPresetMaxTokens);
        tabPresets.Controls.Add(numPresetMaxTokens);
        tabPresets.Controls.Add(lblPresetTopP);
        tabPresets.Controls.Add(chkPresetTopP);
        tabPresets.Controls.Add(numPresetTopP);
        tabPresets.Controls.Add(btnAddPreset);
        tabPresets.Controls.Add(btnDeletePreset);
        tabPresets.Location = new Point(4, 24);
        tabPresets.Margin = new Padding(3, 2, 3, 2);
        tabPresets.Name = "tabPresets";
        tabPresets.Padding = new Padding(10);
        tabPresets.Size = new Size(867, 557);
        tabPresets.TabIndex = 1;
        tabPresets.Text = "Presets";
        // 
        // cmbPresetOutputFormat
        // 
        cmbPresetOutputFormat.FormattingEnabled = true;
        cmbPresetOutputFormat.Items.AddRange(new object[] { "text", "json", "tool" });
        cmbPresetOutputFormat.Location = new Point(392, 257);
        cmbPresetOutputFormat.Name = "cmbPresetOutputFormat";
        cmbPresetOutputFormat.Size = new Size(121, 23);
        cmbPresetOutputFormat.TabIndex = 29;
        // 
        // btnPresetUp
        // 
        btnPresetUp.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnPresetUp.Font = new Font("Segoe UI", 12F);
        btnPresetUp.Location = new Point(194, 521);
        btnPresetUp.Margin = new Padding(3, 2, 3, 2);
        btnPresetUp.Name = "btnPresetUp";
        btnPresetUp.Size = new Size(32, 30);
        btnPresetUp.TabIndex = 27;
        btnPresetUp.Text = "▲";
        btnPresetUp.Click += btnPresetUp_Click;
        // 
        // btnPresetDown
        // 
        btnPresetDown.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnPresetDown.Font = new Font("Segoe UI", 12F);
        btnPresetDown.Location = new Point(156, 521);
        btnPresetDown.Margin = new Padding(3, 2, 3, 2);
        btnPresetDown.Name = "btnPresetDown";
        btnPresetDown.Size = new Size(32, 30);
        btnPresetDown.TabIndex = 28;
        btnPresetDown.Text = "▼";
        btnPresetDown.Click += btnPresetDown_Click;
        // 
        // lstPresets
        // 
        lstPresets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        lstPresets.FormattingEnabled = true;
        lstPresets.ItemHeight = 15;
        lstPresets.Location = new Point(10, 10);
        lstPresets.Margin = new Padding(3, 2, 3, 2);
        lstPresets.Name = "lstPresets";
        lstPresets.Size = new Size(228, 499);
        lstPresets.TabIndex = 0;
        lstPresets.SelectedIndexChanged += lstPresets_SelectedIndexChanged;
        // 
        // lblPresetId
        // 
        lblPresetId.AutoSize = true;
        lblPresetId.Location = new Point(254, 15);
        lblPresetId.Name = "lblPresetId";
        lblPresetId.Size = new Size(20, 15);
        lblPresetId.TabIndex = 1;
        lblPresetId.Text = "Id:";
        // 
        // txtPresetId
        // 
        txtPresetId.Location = new Point(394, 12);
        txtPresetId.Margin = new Padding(3, 2, 3, 2);
        txtPresetId.Name = "txtPresetId";
        txtPresetId.Size = new Size(438, 23);
        txtPresetId.TabIndex = 2;
        // 
        // lblPresetName
        // 
        lblPresetName.AutoSize = true;
        lblPresetName.Location = new Point(254, 45);
        lblPresetName.Name = "lblPresetName";
        lblPresetName.Size = new Size(42, 15);
        lblPresetName.TabIndex = 3;
        lblPresetName.Text = "Name:";
        // 
        // txtPresetName
        // 
        txtPresetName.Location = new Point(394, 42);
        txtPresetName.Margin = new Padding(3, 2, 3, 2);
        txtPresetName.Name = "txtPresetName";
        txtPresetName.Size = new Size(438, 23);
        txtPresetName.TabIndex = 4;
        // 
        // lblPresetSystemPrompt
        // 
        lblPresetSystemPrompt.AutoSize = true;
        lblPresetSystemPrompt.Location = new Point(254, 75);
        lblPresetSystemPrompt.Name = "lblPresetSystemPrompt";
        lblPresetSystemPrompt.Size = new Size(91, 15);
        lblPresetSystemPrompt.TabIndex = 5;
        lblPresetSystemPrompt.Text = "System Prompt:";
        // 
        // txtPresetSystemPrompt
        // 
        txtPresetSystemPrompt.Location = new Point(254, 98);
        txtPresetSystemPrompt.Margin = new Padding(3, 2, 3, 2);
        txtPresetSystemPrompt.Multiline = true;
        txtPresetSystemPrompt.Name = "txtPresetSystemPrompt";
        txtPresetSystemPrompt.ScrollBars = ScrollBars.Vertical;
        txtPresetSystemPrompt.Size = new Size(578, 151);
        txtPresetSystemPrompt.TabIndex = 6;
        // 
        // lblPresetOutputFormat
        // 
        lblPresetOutputFormat.AutoSize = true;
        lblPresetOutputFormat.Location = new Point(255, 260);
        lblPresetOutputFormat.Name = "lblPresetOutputFormat";
        lblPresetOutputFormat.Size = new Size(89, 15);
        lblPresetOutputFormat.TabIndex = 7;
        lblPresetOutputFormat.Text = "Output Format:";
        // 
        // lblPresetKnowledgeFile
        // 
        lblPresetKnowledgeFile.AutoSize = true;
        lblPresetKnowledgeFile.Location = new Point(254, 290);
        lblPresetKnowledgeFile.Name = "lblPresetKnowledgeFile";
        lblPresetKnowledgeFile.Size = new Size(90, 15);
        lblPresetKnowledgeFile.TabIndex = 7;
        lblPresetKnowledgeFile.Text = "Knowledge File:";
        // 
        // txtPresetKnowledgeFile
        // 
        txtPresetKnowledgeFile.Location = new Point(394, 288);
        txtPresetKnowledgeFile.Margin = new Padding(3, 2, 3, 2);
        txtPresetKnowledgeFile.Name = "txtPresetKnowledgeFile";
        txtPresetKnowledgeFile.Size = new Size(368, 23);
        txtPresetKnowledgeFile.TabIndex = 8;
        // 
        // btnBrowseKnowledgeFile
        // 
        btnBrowseKnowledgeFile.Location = new Point(770, 288);
        btnBrowseKnowledgeFile.Margin = new Padding(3, 2, 3, 2);
        btnBrowseKnowledgeFile.Name = "btnBrowseKnowledgeFile";
        btnBrowseKnowledgeFile.Size = new Size(61, 23);
        btnBrowseKnowledgeFile.TabIndex = 9;
        btnBrowseKnowledgeFile.Text = "...";
        btnBrowseKnowledgeFile.Click += btnBrowseKnowledgeFile_Click;
        // 
        // lblPresetTemperature
        // 
        lblPresetTemperature.AutoSize = true;
        lblPresetTemperature.Location = new Point(254, 328);
        lblPresetTemperature.Name = "lblPresetTemperature";
        lblPresetTemperature.Size = new Size(77, 15);
        lblPresetTemperature.TabIndex = 10;
        lblPresetTemperature.Text = "Temperature:";
        // 
        // chkPresetTemperature
        // 
        chkPresetTemperature.AutoSize = true;
        chkPresetTemperature.Location = new Point(394, 328);
        chkPresetTemperature.Margin = new Padding(3, 2, 3, 2);
        chkPresetTemperature.Name = "chkPresetTemperature";
        chkPresetTemperature.Size = new Size(45, 19);
        chkPresetTemperature.TabIndex = 11;
        chkPresetTemperature.Text = "Use";
        // 
        // numPresetTemperature
        // 
        numPresetTemperature.DecimalPlaces = 2;
        numPresetTemperature.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
        numPresetTemperature.Location = new Point(455, 326);
        numPresetTemperature.Margin = new Padding(3, 2, 3, 2);
        numPresetTemperature.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
        numPresetTemperature.Name = "numPresetTemperature";
        numPresetTemperature.Size = new Size(122, 23);
        numPresetTemperature.TabIndex = 12;
        // 
        // lblPresetMaxTokens
        // 
        lblPresetMaxTokens.AutoSize = true;
        lblPresetMaxTokens.Location = new Point(254, 358);
        lblPresetMaxTokens.Name = "lblPresetMaxTokens";
        lblPresetMaxTokens.Size = new Size(72, 15);
        lblPresetMaxTokens.TabIndex = 13;
        lblPresetMaxTokens.Text = "Max Tokens:";
        // 
        // chkPresetMaxTokens
        // 
        chkPresetMaxTokens.AutoSize = true;
        chkPresetMaxTokens.Location = new Point(394, 358);
        chkPresetMaxTokens.Margin = new Padding(3, 2, 3, 2);
        chkPresetMaxTokens.Name = "chkPresetMaxTokens";
        chkPresetMaxTokens.Size = new Size(45, 19);
        chkPresetMaxTokens.TabIndex = 14;
        chkPresetMaxTokens.Text = "Use";
        // 
        // numPresetMaxTokens
        // 
        numPresetMaxTokens.Location = new Point(455, 356);
        numPresetMaxTokens.Margin = new Padding(3, 2, 3, 2);
        numPresetMaxTokens.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
        numPresetMaxTokens.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numPresetMaxTokens.Name = "numPresetMaxTokens";
        numPresetMaxTokens.Size = new Size(122, 23);
        numPresetMaxTokens.TabIndex = 15;
        numPresetMaxTokens.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // lblPresetTopP
        // 
        lblPresetTopP.AutoSize = true;
        lblPresetTopP.Location = new Point(254, 388);
        lblPresetTopP.Name = "lblPresetTopP";
        lblPresetTopP.Size = new Size(40, 15);
        lblPresetTopP.TabIndex = 16;
        lblPresetTopP.Text = "Top P:";
        // 
        // chkPresetTopP
        // 
        chkPresetTopP.AutoSize = true;
        chkPresetTopP.Location = new Point(394, 388);
        chkPresetTopP.Margin = new Padding(3, 2, 3, 2);
        chkPresetTopP.Name = "chkPresetTopP";
        chkPresetTopP.Size = new Size(45, 19);
        chkPresetTopP.TabIndex = 17;
        chkPresetTopP.Text = "Use";
        // 
        // numPresetTopP
        // 
        numPresetTopP.DecimalPlaces = 2;
        numPresetTopP.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
        numPresetTopP.Location = new Point(455, 386);
        numPresetTopP.Margin = new Padding(3, 2, 3, 2);
        numPresetTopP.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
        numPresetTopP.Name = "numPresetTopP";
        numPresetTopP.Size = new Size(122, 23);
        numPresetTopP.TabIndex = 18;
        // 
        // btnAddPreset
        // 
        btnAddPreset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnAddPreset.Location = new Point(10, 521);
        btnAddPreset.Margin = new Padding(3, 2, 3, 2);
        btnAddPreset.Name = "btnAddPreset";
        btnAddPreset.Size = new Size(62, 30);
        btnAddPreset.TabIndex = 19;
        btnAddPreset.Text = "Add";
        btnAddPreset.Click += btnAddPreset_Click;
        // 
        // btnDeletePreset
        // 
        btnDeletePreset.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnDeletePreset.Location = new Point(78, 521);
        btnDeletePreset.Margin = new Padding(3, 2, 3, 2);
        btnDeletePreset.Name = "btnDeletePreset";
        btnDeletePreset.Size = new Size(72, 30);
        btnDeletePreset.TabIndex = 20;
        btnDeletePreset.Text = "Delete";
        btnDeletePreset.Click += btnDeletePreset_Click;
        // 
        // btnSave
        // 
        btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnSave.Location = new Point(682, 605);
        btnSave.Margin = new Padding(3, 2, 3, 2);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(95, 30);
        btnSave.TabIndex = 1;
        btnSave.Text = "Save";
        btnSave.Click += btnSave_Click;
        // 
        // btnCancel
        // 
        btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(786, 605);
        btnCancel.Margin = new Padding(3, 2, 3, 2);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(95, 30);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "Cancel";
        btnCancel.Click += btnCancel_Click;
        // 
        // chkSuppotTools
        // 
        chkSuppotTools.AutoSize = true;
        chkSuppotTools.Location = new Point(579, 285);
        chkSuppotTools.Margin = new Padding(3, 2, 3, 2);
        chkSuppotTools.Name = "chkSuppotTools";
        chkSuppotTools.Size = new Size(95, 19);
        chkSuppotTools.TabIndex = 18;
        chkSuppotTools.Text = "Suppot Tools";
        // 
        // SettingsForm
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(901, 650);
        Controls.Add(tabSettings);
        Controls.Add(btnSave);
        Controls.Add(btnCancel);
        Margin = new Padding(3, 2, 3, 2);
        MinimumSize = new Size(800, 598);
        Name = "SettingsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Settings";
        tabSettings.ResumeLayout(false);
        tabModels.ResumeLayout(false);
        tabModels.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numTimeout).EndInit();
        ((System.ComponentModel.ISupportInitialize)numTemperature).EndInit();
        ((System.ComponentModel.ISupportInitialize)numContextWindow).EndInit();
        ((System.ComponentModel.ISupportInitialize)numMaxTokens).EndInit();
        ((System.ComponentModel.ISupportInitialize)numProxyPort).EndInit();
        tabPresets.ResumeLayout(false);
        tabPresets.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numPresetTemperature).EndInit();
        ((System.ComponentModel.ISupportInitialize)numPresetMaxTokens).EndInit();
        ((System.ComponentModel.ISupportInitialize)numPresetTopP).EndInit();
        ResumeLayout(false);
    }
    private Label lblContextWindow;
    private NumericUpDown numContextWindow;
    private Label lblModelId;
    private TextBox txtModelId;
    private Button btnModelDown;
    private Button button1;
    private Button btnPresetUp;
    private Button btnPresetDown;
    private ComboBox cmbPresetOutputFormat;
    private Label lblPresetOutputFormat;
    private CheckBox chkSuppotTools;
}