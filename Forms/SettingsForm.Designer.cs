namespace Ntk.Chrome.Forms;

partial class SettingsForm
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
        this.Text = "تنظیمات نرم افزار";
        this.Size = new System.Drawing.Size(700, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        this.Font = new Font("Tahoma", 9F, FontStyle.Regular);

        // ChromeDriver Path
        var lblChromeDriver = new Label();
        lblChromeDriver.Text = "آدرس فایل ChromeDriver:";
        lblChromeDriver.AutoSize = false;
        lblChromeDriver.Location = new Point(520, 30);
        lblChromeDriver.Size = new Size(160, 25);
        lblChromeDriver.TextAlign = ContentAlignment.MiddleRight;

        txtChromeDriverPath = new TextBox();
        txtChromeDriverPath.Location = new Point(150, 30);
        txtChromeDriverPath.Size = new Size(350, 25);
        txtChromeDriverPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        btnBrowseChromeDriver = new Button();
        btnBrowseChromeDriver.Text = "انتخاب مسیر";
        btnBrowseChromeDriver.Location = new Point(30, 30);
        btnBrowseChromeDriver.Size = new Size(110, 25);
        btnBrowseChromeDriver.Click += btnBrowseChromeDriver_Click;

        // Program Path
        var lblProgramPath = new Label();
        lblProgramPath.Text = "مسیر کش برنامه:";
        lblProgramPath.AutoSize = false;
        lblProgramPath.Location = new Point(520, 80);
        lblProgramPath.Size = new Size(160, 25);
        lblProgramPath.TextAlign = ContentAlignment.MiddleRight;

        txtProgramPath = new TextBox();
        txtProgramPath.Location = new Point(150, 80);
        txtProgramPath.Size = new Size(350, 25);
        txtProgramPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        btnBrowseProgram = new Button();
        btnBrowseProgram.Text = "انتخاب مسیر";
        btnBrowseProgram.Location = new Point(30, 80);
        btnBrowseProgram.Size = new Size(110, 25);
        btnBrowseProgram.Click += btnBrowseProgram_Click;

        // Log Path
        var lblLogPath = new Label();
        lblLogPath.Text = "مسیر لاگ‌ها:";
        lblLogPath.AutoSize = false;
        lblLogPath.Location = new Point(520, 130);
        lblLogPath.Size = new Size(160, 25);
        lblLogPath.TextAlign = ContentAlignment.MiddleRight;

        txtLogPath = new TextBox();
        txtLogPath.Location = new Point(150, 130);
        txtLogPath.Size = new Size(350, 25);
        txtLogPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        btnBrowseLogPath = new Button();
        btnBrowseLogPath.Text = "انتخاب مسیر";
        btnBrowseLogPath.Location = new Point(30, 130);
        btnBrowseLogPath.Size = new Size(110, 25);
        btnBrowseLogPath.Click += btnBrowseLogPath_Click;

        // Chromium Version
        var lblChromiumVersion = new Label();
        lblChromiumVersion.Text = "نسخه Chromium:";
        lblChromiumVersion.AutoSize = false;
        lblChromiumVersion.Location = new Point(520, 180);
        lblChromiumVersion.Size = new Size(160, 25);
        lblChromiumVersion.TextAlign = ContentAlignment.MiddleRight;

        txtChromiumVersion = new TextBox();
        txtChromiumVersion.Location = new Point(150, 180);
        txtChromiumVersion.Size = new Size(350, 25);
        txtChromiumVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtChromiumVersion.ReadOnly = true;

        // ChromeDriver Version
        var lblChromeDriverVersion = new Label();
        lblChromeDriverVersion.Text = "نسخه ChromeDriver:";
        lblChromeDriverVersion.AutoSize = false;
        lblChromeDriverVersion.Location = new Point(520, 230);
        lblChromeDriverVersion.Size = new Size(160, 25);
        lblChromeDriverVersion.TextAlign = ContentAlignment.MiddleRight;

        txtChromeDriverVersion = new TextBox();
        txtChromeDriverVersion.Location = new Point(150, 230);
        txtChromeDriverVersion.Size = new Size(350, 25);
        txtChromeDriverVersion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtChromeDriverVersion.ReadOnly = true;

        // Program Status Checkbox
        chkEnableProgramStatus = new CheckBox();
        chkEnableProgramStatus.Text = "فعال کردن وضعیت کش برنامه";
        chkEnableProgramStatus.Location = new Point(150, 280);
        chkEnableProgramStatus.AutoSize = true;
        chkEnableProgramStatus.Size = new Size(250, 25);

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(350, 320);
        btnSave.Size = new Size(120, 35);
        btnSave.Click += btnSave_Click;

        // Cancel Button
        var btnCancel = new Button();
        btnCancel.Text = "لغو";
        btnCancel.Location = new Point(220, 320);
        btnCancel.Size = new Size(100, 35);
        btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        // Add controls to form
        this.Controls.AddRange(new Control[] 
        {
            lblChromeDriver,
            txtChromeDriverPath,
            btnBrowseChromeDriver,
            lblProgramPath,
            txtProgramPath,
            btnBrowseProgram,
            lblLogPath,
            txtLogPath,
            btnBrowseLogPath,
            lblChromiumVersion,
            txtChromiumVersion,
            lblChromeDriverVersion,
            txtChromeDriverVersion,
            chkEnableProgramStatus,
            btnSave,
            btnCancel
        });
    }

    private TextBox txtChromeDriverPath;
    private Button btnBrowseChromeDriver;
    private TextBox txtProgramPath;
    private Button btnBrowseProgram;
    private TextBox txtLogPath;
    private Button btnBrowseLogPath;
    private TextBox txtChromiumVersion;
    private TextBox txtChromeDriverVersion;
    private CheckBox chkEnableProgramStatus;
    private Button btnSave;
}
