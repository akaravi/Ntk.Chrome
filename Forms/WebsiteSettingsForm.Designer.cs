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
        this.Size = new System.Drawing.Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.RightToLeft = RightToLeft.Yes;
        this.RightToLeftLayout = true;

        // Website URL
        var lblWebsiteUrl = new Label();
        lblWebsiteUrl.Text = "آدرس وب سایت";
        lblWebsiteUrl.AutoSize = true;
        lblWebsiteUrl.Location = new Point(650, 30);

        txtWebsiteUrl = new TextBox();
        txtWebsiteUrl.Location = new Point(120, 27);
        txtWebsiteUrl.Size = new Size(500, 23);

        // Fields DataGridView
        fieldsDataGridView = new DataGridView();
        fieldsDataGridView.Location = new Point(20, 80);
        fieldsDataGridView.Size = new Size(760, 400);
        fieldsDataGridView.AutoGenerateColumns = false;
        fieldsDataGridView.AllowUserToAddRows = false;
        fieldsDataGridView.AllowUserToDeleteRows = false;

        // Add columns to DataGridView
        fieldsDataGridView.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Name",
                HeaderText = "مقدار فیلد اول",
                Width = 375
            },
            new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "Value",
                HeaderText = "مقدار فیلد دوم",
                Width = 375
            }
        });

        // Add Field Button
        btnAddField = new Button();
        btnAddField.Text = "اضافه کردن فیلد جدید";
        btnAddField.Location = new Point(640, 500);
        btnAddField.Size = new Size(140, 30);
        btnAddField.Click += btnAddField_Click;

        // Remove Field Button
        btnRemoveField = new Button();
        btnRemoveField.Text = "حذف این فیلد";
        btnRemoveField.Location = new Point(480, 500);
        btnRemoveField.Size = new Size(140, 30);
        btnRemoveField.Click += btnRemoveField_Click;

        // Save Button
        btnSave = new Button();
        btnSave.Text = "ذخیره";
        btnSave.Location = new Point(20, 500);
        btnSave.Size = new Size(100, 30);
        btnSave.Click += btnSave_Click;

        // Add controls to form
        this.Controls.AddRange(new Control[] 
        {
            lblWebsiteUrl,
            txtWebsiteUrl,
            fieldsDataGridView,
            btnAddField,
            btnRemoveField,
            btnSave
        });
    }

    private TextBox txtWebsiteUrl;
    private DataGridView fieldsDataGridView;
    private Button btnAddField;
    private Button btnRemoveField;
    private Button btnSave;
}
