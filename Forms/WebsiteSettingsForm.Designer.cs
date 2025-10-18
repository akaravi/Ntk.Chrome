namespace Ntk.Chrome.Forms;

partial class WebsiteSettingsForm
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
        
        // Form properties
        this.Text = "تنظیمات سایت";
        this.Size = new System.Drawing.Size(1000, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        this.Font = new Font("Tahoma", 9F, FontStyle.Regular);

        // Website URL
        var lblWebsiteUrl = new Label();
        lblWebsiteUrl.Text = "آدرس وب سایت:";
        lblWebsiteUrl.AutoSize = true;
        lblWebsiteUrl.Location = new Point(850, 30);
        lblWebsiteUrl.Size = new Size(120, 20);

        txtWebsiteUrl = new TextBox();
        txtWebsiteUrl.Location = new Point(150, 27);
        txtWebsiteUrl.Size = new Size(680, 25);
        txtWebsiteUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // TabControl
        var tabControl = new TabControl();
        tabControl.Location = new Point(30, 80);
        tabControl.Size = new Size(940, 580);
        tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

        // Form Fields Tab
        var formFieldsTab = new TabPage("فیلدهای فرم");
        
        // Fields DataGridView
        fieldsDataGridView = new DataGridView();
        fieldsDataGridView.Location = new Point(20, 20);
        fieldsDataGridView.Size = new Size(890, 480);
        fieldsDataGridView.AutoGenerateColumns = false;
        fieldsDataGridView.AllowUserToAddRows = true;
        fieldsDataGridView.AllowUserToDeleteRows = true;
        fieldsDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        fieldsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        fieldsDataGridView.MultiSelect = false;

        // Add columns to Fields DataGridView
        fieldsDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Name",
                HeaderText = "نام فیلد",
                Width = 440,
                MinimumWidth = 200
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Value",
                HeaderText = "مقدار فیلد",
                Width = 430,
                MinimumWidth = 200
            }
        });

        // Add Field Button
        btnAddField = new Button();
        btnAddField.Text = "اضافه کردن فیلد جدید";
        btnAddField.Location = new Point(760, 520);
        btnAddField.Size = new Size(150, 35);
        btnAddField.Click += btnAddField_Click;

        // Remove Field Button
        btnRemoveField = new Button();
        btnRemoveField.Text = "حذف فیلد انتخاب شده";
        btnRemoveField.Location = new Point(590, 520);
        btnRemoveField.Size = new Size(150, 35);
        btnRemoveField.Click += btnRemoveField_Click;

        formFieldsTab.Controls.AddRange(new Control[] { fieldsDataGridView, btnAddField, btnRemoveField });

        // URL Monitoring Tab
        var urlMonitoringTab = new TabPage("نظارت بر URL");

        // URL Monitoring DataGridView
        urlMonitoringDataGridView = new DataGridView();
        urlMonitoringDataGridView.Location = new Point(20, 20);
        urlMonitoringDataGridView.Size = new Size(890, 480);
        urlMonitoringDataGridView.AutoGenerateColumns = false;
        urlMonitoringDataGridView.AllowUserToAddRows = false;
        urlMonitoringDataGridView.AllowUserToDeleteRows = false;
        urlMonitoringDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        urlMonitoringDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        urlMonitoringDataGridView.MultiSelect = false;

        // Add columns to URL Monitoring DataGridView
        urlMonitoringDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Name",
                HeaderText = "نام نظارت",
                Width = 200,
                MinimumWidth = 150
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Url",
                HeaderText = "آدرس URL",
                Width = 400,
                MinimumWidth = 200
            },
            new DataGridViewCheckBoxColumn 
            { 
                DataPropertyName = "ShowPopup",
                HeaderText = "نمایش پاپ‌آپ",
                Width = 120
            },
            new DataGridViewCheckBoxColumn 
            { 
                DataPropertyName = "LogToFile",
                HeaderText = "ذخیره در فایل",
                Width = 120
            }
        });

        // Add URL Monitoring Button
        btnAddUrlMonitoring = new Button();
        btnAddUrlMonitoring.Text = "اضافه کردن نظارت جدید";
        btnAddUrlMonitoring.Location = new Point(760, 520);
        btnAddUrlMonitoring.Size = new Size(150, 35);
        btnAddUrlMonitoring.Click += btnAddUrlMonitoring_Click;

        // Remove URL Monitoring Button
        btnRemoveUrlMonitoring = new Button();
        btnRemoveUrlMonitoring.Text = "حذف نظارت انتخاب شده";
        btnRemoveUrlMonitoring.Location = new Point(590, 520);
        btnRemoveUrlMonitoring.Size = new Size(150, 35);
        btnRemoveUrlMonitoring.Click += btnRemoveUrlMonitoring_Click;

        // Configure URL Monitoring Button
        btnConfigureUrlMonitoring = new Button();
        btnConfigureUrlMonitoring.Text = "تنظیم پارامترها";
        btnConfigureUrlMonitoring.Location = new Point(420, 520);
        btnConfigureUrlMonitoring.Size = new Size(150, 35);
        btnConfigureUrlMonitoring.Click += btnConfigureUrlMonitoring_Click;

        urlMonitoringTab.Controls.AddRange(new Control[] { 
            urlMonitoringDataGridView, 
            btnAddUrlMonitoring, 
            btnRemoveUrlMonitoring, 
            btnConfigureUrlMonitoring 
        });

        // Add tabs to TabControl
        tabControl.TabPages.AddRange(new TabPage[] { formFieldsTab, urlMonitoringTab });

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(500, 680);
        btnSave.Size = new Size(120, 35);
        btnSave.Click += btnSave_Click;

        // Cancel Button
        var btnCancel = new Button();
        btnCancel.Text = "لغو";
        btnCancel.Location = new Point(370, 680);
        btnCancel.Size = new Size(100, 35);
        btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        // Add controls to form
        this.Controls.AddRange(new Control[] 
        {
            lblWebsiteUrl,
            txtWebsiteUrl,
            tabControl,
            btnSave,
            btnCancel
        });
    }

    private TextBox txtWebsiteUrl;
    private DataGridView fieldsDataGridView;
    private DataGridView urlMonitoringDataGridView;
    private Button btnAddField;
    private Button btnRemoveField;
    private Button btnAddUrlMonitoring;
    private Button btnRemoveUrlMonitoring;
    private Button btnConfigureUrlMonitoring;
    private Button btnSave;
}
