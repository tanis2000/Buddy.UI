using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Buddy.UI.Controls
{
  /// <summary>
  /// Interaction logic for StatusControl.xaml
  /// </summary>
  public partial class StatusControl : UserControl
  {
    private Process p;
    private bool IsRunning;

    public StatusControl()
    {
      InitializeComponent();
      // Load the current configuration
      var provider = ConfigurationManager.AppSettings.Get("provider");
      if (provider != null)
      {
        foreach(var item in lbProvider.Items)
        {
          if (((ListBoxItem)item).Content.ToString() == provider)
          {
            lbProvider.SelectedItem = item;
          }
        }
      }
      var username = ConfigurationManager.AppSettings.Get("username");
      if (username != null)
      {
        tbUsername.Text = username;
      }
      var password = ConfigurationManager.AppSettings.Get("password");
      if (password != null)
      {
        tbPassword.Password = password;
      }
      var location = ConfigurationManager.AppSettings.Get("location");
      if (location != null)
      {
        tbLocation.Text = location;
      }

    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (p != null)
      {
        MessageBox.Show("Bot is already running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      if (lbProvider.SelectedItem == null)
      {
        MessageBox.Show("You must select a provider", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }
      var provider = ((ListBoxItem)lbProvider.SelectedItem).Content.ToString();
      var username = tbUsername.Text;
      var password = tbPassword.Password;
      var location = tbLocation.Text;

      // Save the current configuration
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      config.AppSettings.Settings["provider"].Value = provider;
      config.AppSettings.Settings["username"].Value = username;
      config.AppSettings.Settings["password"].Value = password;
      config.AppSettings.Settings["location"].Value = location;
      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection("appSettings");

      var args = $"--provider {provider} -u {username} -p {password} --location \"{location}\" -c";
      Console.WriteLine(args);
      try
      {
        var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pgo-client.exe");
        ProcessStartInfo si = new ProcessStartInfo(path, args);
        si.WindowStyle = ProcessWindowStyle.Hidden;
        si.CreateNoWindow = true;
        si.RedirectStandardOutput = true;
        si.RedirectStandardError = true;
        si.RedirectStandardInput = true;
        si.UseShellExecute = false;
        p = Process.Start(si);
        IsRunning = true;
        logBox.Document.Blocks.Clear();
        p.OutputDataReceived += (s, ev) =>
        {
          this.Dispatcher.Invoke((Action)(() =>
          {
            logBox.AppendText(ev.Data);
          }));
        };

        p.BeginOutputReadLine();
        //p.WaitForExit();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
      if (p == null)
      {
        return;
      }

      if (p.HasExited)
      {
        p = null;
        return;
      }

      p.Kill();
      p = null;
    }
  }
}
