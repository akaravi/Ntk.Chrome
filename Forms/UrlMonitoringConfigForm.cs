using Ntk.Chrome.Models;
using System.ComponentModel;

namespace Ntk.Chrome.Forms;

public partial class UrlMonitoringConfigForm : Form
{
    private UrlMonitoringConfig _config;
    private readonly BindingList<MonitoringParameter> _parameters;

    public UrlMonitoringConfigForm(UrlMonitoringConfig config)
    {
        _config = config ?? new UrlMonitoringConfig();
        _parameters = new BindingList<MonitoringParameter>();
        
        InitializeComponent();
        
        LoadConfig();
        parametersDataGridView.DataSource = _parameters;
    }

    private void LoadConfig()
    {
        txtName.Text = _config.Name;
        txtUrl.Text = _config.Url;
        txtPopupTitle.Text = _config.PopupTitle;
        chkShowPopup.Checked = _config.ShowPopup;
        chkLogToFile.Checked = _config.LogToFile;
        
        _parameters.Clear();
        foreach (var param in _config.Parameters)
        {
            _parameters.Add(param);
        }
    }

    private void btnAddParameter_Click(object sender, EventArgs e)
    {
        _parameters.Add(new MonitoringParameter());
    }

    private void btnRemoveParameter_Click(object sender, EventArgs e)
    {
        if (parametersDataGridView.SelectedRows.Count > 0)
        {
            foreach (DataGridViewRow row in parametersDataGridView.SelectedRows)
            {
                parametersDataGridView.Rows.Remove(row);
            }
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        _config.Name = txtName.Text;
        _config.Url = txtUrl.Text;
        _config.PopupTitle = txtPopupTitle.Text;
        _config.ShowPopup = chkShowPopup.Checked;
        _config.LogToFile = chkLogToFile.Checked;
        _config.Parameters = _parameters.ToList();

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
