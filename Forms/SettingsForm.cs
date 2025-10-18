using Ntk.Chrome.Models;
using Newtonsoft.Json;

namespace Ntk.Chrome.Forms;

public partial class SettingsForm : Form
{
    private AppSettings _settings = new();

    public SettingsForm()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists("settings.json"))
        {
            var json = File.ReadAllText("settings.json");
            _settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
        }
        else
        {
            _settings = new AppSettings
            {
                ChromeDriverPath = Path.Combine(Application.StartupPath, "ChromeDriver"),
                ProgramPath = Application.StartupPath,
                LogPath = Path.Combine(Path.GetTempPath(), "NtkChromeDriver", "logs"),
                EnableProgramStatus = true
            };
        }

        txtChromeDriverPath.Text = _settings.ChromeDriverPath;
        txtProgramPath.Text = _settings.ProgramPath;
        txtLogPath.Text = _settings.LogPath;
        txtChromiumVersion.Text = _settings.ChromiumVersion;
        txtChromeDriverVersion.Text = _settings.ChromeDriverVersion;
        chkEnableProgramStatus.Checked = _settings.EnableProgramStatus;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        _settings.ChromeDriverPath = txtChromeDriverPath.Text;
        _settings.ProgramPath = txtProgramPath.Text;
        _settings.LogPath = txtLogPath.Text;
        _settings.ChromiumVersion = txtChromiumVersion.Text;
        _settings.ChromeDriverVersion = txtChromeDriverVersion.Text;
        _settings.EnableProgramStatus = chkEnableProgramStatus.Checked;

        var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
        File.WriteAllText("settings.json", json);

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnBrowseChromeDriver_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtChromeDriverPath.Text = dialog.SelectedPath;
        }
    }

    private void btnBrowseProgram_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtProgramPath.Text = dialog.SelectedPath;
        }
    }

    private void btnBrowseLogPath_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtLogPath.Text = dialog.SelectedPath;
        }
    }
}
