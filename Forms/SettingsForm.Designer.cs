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
        this.Size = new System.Drawing.Size(600, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;

        // ChromeDriver Path
        var lblChromeDriver = new Label();
        lblChromeDriver.Text = "آدرس فایل chromdrive";
        lblChromeDriver.AutoSize = true;
        lblChromeDriver.Location = new Point(450, 30);

        txtChromeDriverPath = new TextBox();
        txtChromeDriverPath.Location = new Point(120, 27);
        txtChromeDriverPath.Size = new Size(300, 23);

        btnBrowseChromeDriver = new Button();
        btnBrowseChromeDriver.Text = "انتخاب مسیر";
        btnBrowseChromeDriver.Location = new Point(20, 26);
        btnBrowseChromeDriver.Size = new Size(90, 25);
        btnBrowseChromeDriver.Click += btnBrowseChromeDriver_Click;

        // Program Path
        var lblProgramPath = new Label();
        lblProgramPath.Text = "مسیر کش برنامه";
        lblProgramPath.AutoSize = true;
        lblProgramPath.Location = new Point(450, 80);

        txtProgramPath = new TextBox();
        txtProgramPath.Location = new Point(120, 77);
        txtProgramPath.Size = new Size(300, 23);

        btnBrowseProgram = new Button();
        btnBrowseProgram.Text = "انتخاب مسیر";
        btnBrowseProgram.Location = new Point(20, 76);
        btnBrowseProgram.Size = new Size(90, 25);
        btnBrowseProgram.Click += btnBrowseProgram_Click;

        // Log Path
        var lblLogPath = new Label();
        lblLogPath.Text = "مسیر لاگ‌ها";
        lblLogPath.AutoSize = true;
        lblLogPath.Location = new Point(450, 130);

        txtLogPath = new TextBox();
        txtLogPath.Location = new Point(120, 127);
        txtLogPath.Size = new Size(300, 23);

        btnBrowseLogPath = new Button();
        btnBrowseLogPath.Text = "انتخاب مسیر";
        btnBrowseLogPath.Location = new Point(20, 126);
        btnBrowseLogPath.Size = new Size(90, 25);
        btnBrowseLogPath.Click += btnBrowseLogPath_Click;

        // Chromium Version
        var lblChromiumVersion = new Label();
        lblChromiumVersion.Text = "نسخه کرومیوم";
        lblChromiumVersion.AutoSize = true;
        lblChromiumVersion.Location = new Point(450, 130);

        txtChromiumVersion = new TextBox();
        txtChromiumVersion.Location = new Point(120, 127);
        txtChromiumVersion.Size = new Size(300, 23);

        // ChromeDriver Version
        var lblChromeDriverVersion = new Label();
        lblChromeDriverVersion.Text = "نسخه ChromeDriver";
        lblChromeDriverVersion.AutoSize = true;
        lblChromeDriverVersion.Location = new Point(450, 180);

        txtChromeDriverVersion = new TextBox();
        txtChromeDriverVersion.Location = new Point(120, 177);
        txtChromeDriverVersion.Size = new Size(300, 23);

        // Program Status Checkbox
        chkEnableProgramStatus = new CheckBox();
        chkEnableProgramStatus.Text = "وضعیت کش برنامه";
        chkEnableProgramStatus.Location = new Point(350, 230);
        chkEnableProgramStatus.AutoSize = true;

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(250, 200);
        btnSave.Size = new Size(100, 30);
        btnSave.Click += btnSave_Click;

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
            btnSave
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
