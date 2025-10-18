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
        this.Size = new System.Drawing.Size(900, 700);
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
        lblWebsiteUrl.Location = new Point(750, 30);
        lblWebsiteUrl.Size = new Size(120, 20);

        txtWebsiteUrl = new TextBox();
        txtWebsiteUrl.Location = new Point(150, 27);
        txtWebsiteUrl.Size = new Size(580, 25);
        txtWebsiteUrl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // Fields DataGridView
        fieldsDataGridView = new DataGridView();
        fieldsDataGridView.Location = new Point(30, 80);
        fieldsDataGridView.Size = new Size(840, 450);
        fieldsDataGridView.AutoGenerateColumns = false;
        fieldsDataGridView.AllowUserToAddRows = true;
        fieldsDataGridView.AllowUserToDeleteRows = true;
        fieldsDataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        fieldsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        fieldsDataGridView.MultiSelect = false;

        // Add columns to DataGridView
        fieldsDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Name",
                HeaderText = "نام فیلد",
                Width = 400,
                MinimumWidth = 200
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Value",
                HeaderText = "مقدار فیلد",
                Width = 400,
                MinimumWidth = 200
            }
        });

        // Add Field Button
        btnAddField = new Button();
        btnAddField.Text = "اضافه کردن فیلد جدید";
        btnAddField.Location = new Point(720, 550);
        btnAddField.Size = new Size(150, 35);
        btnAddField.Click += btnAddField_Click;

        // Remove Field Button
        btnRemoveField = new Button();
        btnRemoveField.Text = "حذف فیلد انتخاب شده";
        btnRemoveField.Location = new Point(550, 550);
        btnRemoveField.Size = new Size(150, 35);
        btnRemoveField.Click += btnRemoveField_Click;

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(400, 600);
        btnSave.Size = new Size(120, 35);
        btnSave.Click += btnSave_Click;

        // Cancel Button
        var btnCancel = new Button();
        btnCancel.Text = "لغو";
        btnCancel.Location = new Point(270, 600);
        btnCancel.Size = new Size(100, 35);
        btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        // Add controls to form
        this.Controls.AddRange(new Control[] 
        {
            lblWebsiteUrl,
            txtWebsiteUrl,
            fieldsDataGridView,
            btnAddField,
            btnRemoveField,
            btnSave,
            btnCancel
        });
    }

    private TextBox txtWebsiteUrl;
    private DataGridView fieldsDataGridView;
    private Button btnAddField;
    private Button btnRemoveField;
    private Button btnSave;
}
