using Ntk.Chrome.Models;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Ntk.Chrome.Forms;

public partial class WebsiteSettingsForm : Form
{
    private WebsiteSettings _settings = new();
    private readonly BindingList<WebsiteField> _fields;

    public WebsiteSettingsForm()
    {
        InitializeComponent();
        _fields = new BindingList<WebsiteField>();
        LoadSettings();
        
        fieldsDataGridView.DataSource = _fields;
    }

    private void LoadSettings()
    {
        if (File.Exists("website_settings.json"))
        {
            var json = File.ReadAllText("website_settings.json");
            _settings = JsonConvert.DeserializeObject<WebsiteSettings>(json) ?? new WebsiteSettings();
            _fields.Clear();
            foreach (var field in _settings.Fields)
            {
                _fields.Add(field);
            }
        }
        else
        {
            _settings = new WebsiteSettings();
        }

        txtWebsiteUrl.Text = _settings.WebsiteUrl;
    }

    private void btnAddField_Click(object sender, EventArgs e)
    {
        _fields.Add(new WebsiteField());
    }

    private void btnRemoveField_Click(object sender, EventArgs e)
    {
        if (fieldsDataGridView.SelectedRows.Count > 0)
        {
            foreach (DataGridViewRow row in fieldsDataGridView.SelectedRows)
            {
                fieldsDataGridView.Rows.Remove(row);
            }
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        _settings.WebsiteUrl = txtWebsiteUrl.Text;
        _settings.Fields = _fields.ToList();

        var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
        File.WriteAllText("website_settings.json", json);

        DialogResult = DialogResult.OK;
        Close();
    }
}
