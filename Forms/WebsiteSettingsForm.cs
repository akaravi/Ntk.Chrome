using Ntk.Chrome.Models;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Ntk.Chrome.Forms;

public partial class WebsiteSettingsForm : Form
{
    private WebsiteSettings _settings = new();
    private readonly BindingList<WebsiteField> _fields;
    private readonly BindingList<UrlMonitoringConfig> _urlMonitoringConfigs;

    public WebsiteSettingsForm()
    {
        InitializeComponent();
        _fields = new BindingList<WebsiteField>();
        _urlMonitoringConfigs = new BindingList<UrlMonitoringConfig>();
        LoadSettings();
        
        fieldsDataGridView.DataSource = _fields;
        urlMonitoringDataGridView.DataSource = _urlMonitoringConfigs;
    }

    private void LoadSettings()
    {
        if (File.Exists("website_settings.json"))
        {
            var json = File.ReadAllText("website_settings.json");
            _settings = JsonConvert.DeserializeObject<WebsiteSettings>(json) ?? new WebsiteSettings();
            
            // Load form fields
            _fields.Clear();
            foreach (var field in _settings.Fields)
            {
                _fields.Add(field);
            }
            
            // Load URL monitoring configurations
            _urlMonitoringConfigs.Clear();
            foreach (var config in _settings.UrlMonitoring)
            {
                _urlMonitoringConfigs.Add(config);
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

    private void btnAddUrlMonitoring_Click(object sender, EventArgs e)
    {
        _urlMonitoringConfigs.Add(new UrlMonitoringConfig());
    }

    private void btnRemoveUrlMonitoring_Click(object sender, EventArgs e)
    {
        if (urlMonitoringDataGridView.SelectedRows.Count > 0)
        {
            foreach (DataGridViewRow row in urlMonitoringDataGridView.SelectedRows)
            {
                urlMonitoringDataGridView.Rows.Remove(row);
            }
        }
    }

    private void btnConfigureUrlMonitoring_Click(object sender, EventArgs e)
    {
        if (urlMonitoringDataGridView.SelectedRows.Count > 0)
        {
            var selectedConfig = (UrlMonitoringConfig)urlMonitoringDataGridView.SelectedRows[0].DataBoundItem;
            using var configForm = new UrlMonitoringConfigForm(selectedConfig);
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the DataGridView to show updated data
                urlMonitoringDataGridView.Refresh();
            }
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        _settings.WebsiteUrl = txtWebsiteUrl.Text;
        _settings.Fields = _fields.ToList();
        _settings.UrlMonitoring = _urlMonitoringConfigs.ToList();

        var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
        File.WriteAllText("website_settings.json", json);

        DialogResult = DialogResult.OK;
        Close();
    }
}
