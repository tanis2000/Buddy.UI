using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

    public StatusControl()
    {
      InitializeComponent();
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
      var password = tbPassword.Text;
      var location = tbLocation.Text;
      var args = $"--provider {provider} -u {username} -p {password} --location \"{location}\" -c";
      Console.WriteLine(args);
      try
      {
        var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pgo-client.exe");
        p = Process.Start(path, args);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
      Console.ReadLine();
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
      if (p == null)
      {
        return;
      }

      p.Kill();
      p = null;
    }
  }
}
