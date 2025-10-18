namespace Ntk.Chrome.Forms;

partial class UrlMonitoringConfigForm
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
        this.Text = "تنظیمات نظارت بر URL";
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;
        this.Font = new Font("Tahoma", 9F, FontStyle.Regular);

        // Name
        var lblName = new Label();
        lblName.Text = "نام نظارت:";
        lblName.AutoSize = true;
        lblName.Location = new Point(700, 30);
        lblName.Size = new Size(80, 20);

        txtName = new TextBox();
        txtName.Location = new Point(450, 27);
        txtName.Size = new Size(230, 25);

        // URL
        var lblUrl = new Label();
        lblUrl.Text = "آدرس URL:";
        lblUrl.AutoSize = true;
        lblUrl.Location = new Point(700, 70);
        lblUrl.Size = new Size(80, 20);

        txtUrl = new TextBox();
        txtUrl.Location = new Point(50, 67);
        txtUrl.Size = new Size(630, 25);
        txtUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // Popup Title
        var lblPopupTitle = new Label();
        lblPopupTitle.Text = "عنوان پاپ‌آپ:";
        lblPopupTitle.AutoSize = true;
        lblPopupTitle.Location = new Point(700, 110);
        lblPopupTitle.Size = new Size(90, 20);

        txtPopupTitle = new TextBox();
        txtPopupTitle.Location = new Point(450, 107);
        txtPopupTitle.Size = new Size(230, 25);

        // Show Popup Checkbox
        chkShowPopup = new CheckBox();
        chkShowPopup.Text = "نمایش پاپ‌آپ";
        chkShowPopup.Location = new Point(650, 150);
        chkShowPopup.Size = new Size(120, 20);
        chkShowPopup.Checked = true;

        // Log to File Checkbox
        chkLogToFile = new CheckBox();
        chkLogToFile.Text = "ذخیره در فایل لاگ";
        chkLogToFile.Location = new Point(450, 150);
        chkLogToFile.Size = new Size(120, 20);
        chkLogToFile.Checked = true;

        // Parameters Section
        var lblParameters = new Label();
        lblParameters.Text = "پارامترهای مورد نظر:";
        lblParameters.AutoSize = true;
        lblParameters.Location = new Point(700, 190);
        lblParameters.Size = new Size(140, 20);
        lblParameters.Font = new Font("Tahoma", 9F, FontStyle.Bold);

        // Parameters DataGridView
        parametersDataGridView = new DataGridView();
        parametersDataGridView.Location = new Point(50, 220);
        parametersDataGridView.Size = new Size(700, 280);
        parametersDataGridView.AutoGenerateColumns = false;
        parametersDataGridView.AllowUserToAddRows = false;
        parametersDataGridView.AllowUserToDeleteRows = false;
        parametersDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        parametersDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        parametersDataGridView.MultiSelect = false;

        // Add columns to Parameters DataGridView
        parametersDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Name",
                HeaderText = "نام پارامتر",
                Width = 200,
                MinimumWidth = 150
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Description",
                HeaderText = "توضیحات",
                Width = 350,
                MinimumWidth = 200
            },
            new DataGridViewCheckBoxColumn 
            { 
                DataPropertyName = "Required",
                HeaderText = "اجباری",
                Width = 80
            }
        });

        // Add Parameter Button
        btnAddParameter = new Button();
        btnAddParameter.Text = "اضافه کردن پارامتر";
        btnAddParameter.Location = new Point(570, 520);
        btnAddParameter.Size = new Size(130, 30);
        btnAddParameter.Click += btnAddParameter_Click;

        // Remove Parameter Button
        btnRemoveParameter = new Button();
        btnRemoveParameter.Text = "حذف پارامتر";
        btnRemoveParameter.Location = new Point(430, 520);
        btnRemoveParameter.Size = new Size(120, 30);
        btnRemoveParameter.Click += btnRemoveParameter_Click;

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(300, 520);
        btnSave.Size = new Size(100, 30);
        btnSave.Click += btnSave_Click;

        // Cancel Button
        btnCancel = new Button();
        btnCancel.Text = "لغو";
        btnCancel.Location = new Point(180, 520);
        btnCancel.Size = new Size(100, 30);
        btnCancel.Click += btnCancel_Click;

        // Add controls to form
        this.Controls.AddRange(new Control[] 
        {
            lblName, txtName,
            lblUrl, txtUrl,
            lblPopupTitle, txtPopupTitle,
            chkShowPopup, chkLogToFile,
            lblParameters,
            parametersDataGridView,
            btnAddParameter, btnRemoveParameter,
            btnSave, btnCancel
        });
    }

    private TextBox txtName;
    private TextBox txtUrl;
    private TextBox txtPopupTitle;
    private CheckBox chkShowPopup;
    private CheckBox chkLogToFile;
    private DataGridView parametersDataGridView;
    private Button btnAddParameter;
    private Button btnRemoveParameter;
    private Button btnSave;
    private Button btnCancel;
}
