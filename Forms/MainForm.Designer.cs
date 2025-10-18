namespace Ntk.Chrome.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        
        // Main form properties
        this.Text = "Ntk Chrome Automation";
        this.Size = new System.Drawing.Size(1200, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        this.BackColor = Color.FromArgb(245, 245, 245);
        this.WindowState = FormWindowState.Maximized;
        this.Padding = new Padding(10);
        this.FormClosing += MainForm_FormClosing;

        // Main container panel with right side panel
        var mainContainer = new TableLayoutPanel();
        mainContainer.Dock = DockStyle.Fill;
        mainContainer.ColumnCount = 2;
        mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        mainContainer.BackColor = Color.FromArgb(245, 245, 245);
        mainContainer.Padding = new Padding(0);
        mainContainer.Margin = new Padding(0);

        // Right side panel for buttons
        var rightPanel = new Panel();
        rightPanel.Dock = DockStyle.Fill;
        rightPanel.BackColor = Color.White;
        rightPanel.Padding = new Padding(10);

        // Main content panel
        var mainPanel = new Panel();
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.BackColor = Color.White;
        mainPanel.Padding = new Padding(10);

        // Create buttons for right panel
        btnStart = new Button();
        btnStart.Text = "شروع";
        btnStart.BackColor = Color.FromArgb(76, 175, 80);
        btnStart.ForeColor = Color.White;
        btnStart.Size = new Size(180, 40);
        btnStart.Dock = DockStyle.Top;
        btnStart.Margin = new Padding(0, 5, 0, 5);
        btnStart.FlatStyle = FlatStyle.Flat;
        btnStart.Click += btnStart_Click;

        btnStop = new Button();
        btnStop.Text = "توقیف";
        btnStop.BackColor = Color.FromArgb(255, 99, 99);
        btnStop.ForeColor = Color.White;
        btnStop.Size = new Size(180, 40);
        btnStop.Dock = DockStyle.Top;
        btnStop.Margin = new Padding(0, 5, 0, 5);
        btnStop.FlatStyle = FlatStyle.Flat;
        btnStop.Click += btnStop_Click;
        btnStop.Enabled = false;

        var btnSiteSettings = new Button();
        btnSiteSettings.Text = "تنظیمات سایت";
        btnSiteSettings.BackColor = Color.FromArgb(255, 152, 0);
        btnSiteSettings.ForeColor = Color.White;
        btnSiteSettings.Size = new Size(180, 40);
        btnSiteSettings.Dock = DockStyle.Top;
        btnSiteSettings.Margin = new Padding(0, 5, 0, 5);
        btnSiteSettings.FlatStyle = FlatStyle.Flat;
        btnSiteSettings.Click += (s, e) => {
            using var form = new WebsiteSettingsForm();
            form.ShowDialog();
        };

        var btnAppSettings = new Button();
        btnAppSettings.Text = "تنظیمات نرم افزار";
        btnAppSettings.BackColor = Color.FromArgb(33, 150, 243);
        btnAppSettings.ForeColor = Color.White;
        btnAppSettings.Size = new Size(180, 40);
        btnAppSettings.Dock = DockStyle.Top;
        btnAppSettings.Margin = new Padding(0, 5, 0, 5);
        btnAppSettings.FlatStyle = FlatStyle.Flat;
        btnAppSettings.Click += (s, e) => {
            using var form = new SettingsForm();
            form.ShowDialog();
        };

        // Add buttons to right panel
        rightPanel.Controls.AddRange(new Control[] { 
            btnAppSettings,
            btnSiteSettings,
            btnStop,
            btnStart
        });

        // Log Panel
        var logPanel = new Panel();
        logPanel.Dock = DockStyle.Bottom;
        logPanel.Height = 200;
        logPanel.BackColor = Color.White;
        logPanel.Padding = new Padding(0);

        var logHeaderPanel = new Panel();
        logHeaderPanel.Dock = DockStyle.Top;
        logHeaderPanel.Height = 40;
        logHeaderPanel.BackColor = Color.White;
        logHeaderPanel.Padding = new Padding(10, 5, 10, 5);

        var lblLog = new Label();
        lblLog.Text = "لاگ برنامه";
        lblLog.AutoSize = true;
        lblLog.Font = new Font("Tahoma", 11F, FontStyle.Regular);
        lblLog.Location = new Point(1050, 10);
        lblLog.ForeColor = Color.FromArgb(64, 64, 64);

        logHeaderPanel.Controls.Add(lblLog);

        logTextBox = new RichTextBox();
        logTextBox.Dock = DockStyle.Fill;
        logTextBox.ReadOnly = true;
        logTextBox.BackColor = Color.White;
        logTextBox.Font = new Font("Tahoma", 10F);
        logTextBox.RightToLeft = RightToLeft.No;
        logTextBox.BorderStyle = BorderStyle.None;
        logTextBox.Padding = new Padding(10);

        logPanel.Controls.Add(logTextBox);
        logPanel.Controls.Add(logHeaderPanel);

        // Requests DataGridView
        requestsDataGridView = new DataGridView();
        requestsDataGridView.Dock = DockStyle.Fill;
        requestsDataGridView.AutoGenerateColumns = false;
        requestsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        requestsDataGridView.AllowUserToAddRows = false;
        requestsDataGridView.AllowUserToDeleteRows = false;
        requestsDataGridView.ReadOnly = true;
        requestsDataGridView.SelectionChanged += requestsDataGridView_SelectionChanged;
        requestsDataGridView.BackgroundColor = Color.White;
        requestsDataGridView.BorderStyle = BorderStyle.None;
        requestsDataGridView.GridColor = Color.FromArgb(224, 224, 224);
        requestsDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        requestsDataGridView.RowHeadersVisible = false;
        requestsDataGridView.EnableHeadersVisualStyles = false;
        requestsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
        requestsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
        requestsDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Bold);
        requestsDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        requestsDataGridView.ColumnHeadersHeight = 40;
        requestsDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 9F);
        requestsDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 242, 242);
        requestsDataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
        requestsDataGridView.RowTemplate.Height = 35;
        requestsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

        // Add columns to DataGridView
        requestsDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Timestamp",
                HeaderText = "زمان",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter 
                }
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Method",
                HeaderText = "متد",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter 
                }
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Url",
                HeaderText = "آدرس",
                Width = 600,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleLeft 
                }
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "StatusCode",
                HeaderText = "وضعیت",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter 
                }
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "ContentType",
                HeaderText = "نوع محتوا",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle 
                { 
                    Alignment = DataGridViewContentAlignment.MiddleCenter 
                }
            }
        });

        // Request Details Panel
        var detailsPanel = new Panel();
        detailsPanel.Dock = DockStyle.Bottom;
        detailsPanel.Height = 300;
        detailsPanel.BackColor = Color.White;
        detailsPanel.Padding = new Padding(0);

        // Radio Buttons for display format
        var formatPanel = new Panel();
        formatPanel.Dock = DockStyle.Top;
        formatPanel.Height = 40;
        formatPanel.Padding = new Padding(10, 5, 10, 5);
        formatPanel.BackColor = Color.White;

        var lblDisplayFormat = new Label();
        lblDisplayFormat.Text = "اطلاعات صفحات فراخوانی";
        lblDisplayFormat.AutoSize = true;
        lblDisplayFormat.Font = new Font("Tahoma", 11F, FontStyle.Regular);
        lblDisplayFormat.Dock = DockStyle.Right;
        lblDisplayFormat.ForeColor = Color.FromArgb(64, 64, 64);

        var radioPanel = new FlowLayoutPanel();
        radioPanel.FlowDirection = FlowDirection.RightToLeft;
        radioPanel.Dock = DockStyle.Left;
        radioPanel.AutoSize = true;
        radioPanel.Padding = new Padding(5);

        radioButtonText = new RadioButton();
        radioButtonText.Text = "Text";
        radioButtonText.Checked = true;
        radioButtonText.Font = new Font("Tahoma", 10F);
        radioButtonText.AutoSize = true;
        radioButtonText.Margin = new Padding(5, 0, 5, 0);
        radioButtonText.CheckedChanged += (s, e) => UpdateRequestDetails(GetSelectedRequest());

        radioButtonJson = new RadioButton();
        radioButtonJson.Text = "Json";
        radioButtonJson.Font = new Font("Tahoma", 10F);
        radioButtonJson.AutoSize = true;
        radioButtonJson.Margin = new Padding(5, 0, 5, 0);
        radioButtonJson.CheckedChanged += (s, e) => UpdateRequestDetails(GetSelectedRequest());

        radioButtonHtml = new RadioButton();
        radioButtonHtml.Text = "Html";
        radioButtonHtml.Font = new Font("Tahoma", 10F);
        radioButtonHtml.AutoSize = true;
        radioButtonHtml.Margin = new Padding(5, 0, 5, 0);
        radioButtonHtml.CheckedChanged += (s, e) => UpdateRequestDetails(GetSelectedRequest());

        radioPanel.Controls.AddRange(new Control[] 
        { 
            radioButtonText, 
            radioButtonJson, 
            radioButtonHtml 
        });

        formatPanel.Controls.AddRange(new Control[]
        {
            lblDisplayFormat,
            radioPanel
        });

        // Request Details TextBox
        requestDetailsTextBox = new RichTextBox();
        requestDetailsTextBox.Dock = DockStyle.Fill;
        requestDetailsTextBox.ReadOnly = true;
        requestDetailsTextBox.Font = new Font("Tahoma", 9F);
        requestDetailsTextBox.BackColor = Color.White;
        requestDetailsTextBox.BorderStyle = BorderStyle.None;
        requestDetailsTextBox.Padding = new Padding(5);

        detailsPanel.Controls.Add(requestDetailsTextBox);
        detailsPanel.Controls.Add(formatPanel);

        // Main content layout
        var contentPanel = new TableLayoutPanel();
        contentPanel.Dock = DockStyle.Fill;
        contentPanel.RowCount = 3;
        contentPanel.ColumnCount = 1;
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
        contentPanel.BackColor = Color.White;
        contentPanel.Padding = new Padding(10);
        contentPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

        // Add panels to content layout
        contentPanel.Controls.Add(requestsDataGridView, 0, 0);
        contentPanel.Controls.Add(detailsPanel, 0, 1);
        contentPanel.Controls.Add(logPanel, 0, 2);

        // Add panels to main container
        mainPanel.Controls.Add(contentPanel);
        mainContainer.Controls.Add(mainPanel, 0, 0);
        mainContainer.Controls.Add(rightPanel, 1, 0);
        this.Controls.Add(mainContainer);
    }

    private RichTextBox logTextBox;
    private DataGridView requestsDataGridView;
    private RichTextBox requestDetailsTextBox;
    private RadioButton radioButtonText;
    private RadioButton radioButtonJson;
    private RadioButton radioButtonHtml;
}
