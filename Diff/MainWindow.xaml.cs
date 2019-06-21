/////////////////////////////////////////////////////////////////////
// MainWindow.xaml.cs - event handlers for Diff_WPF                //
// ver 1.1                                                         //
// Jim Fawcett, Summer 2018                                        //
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ver 1.1 : 06 Aug 2018
 * - fixed bug in startup directory handling
 * Ver 1.0 : 06 Aug 2018
 * - first release
 */
using System;
using System.Collections.Generic;
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
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace Diff_WPF
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private string fileText = "";
    private string startDir = Properties.Settings.Default.StartPath1;
    private string startDate = Properties.Settings.Default.StartDate1;

    public MainWindow()
    {
      InitializeComponent();
      CompareButton.IsEnabled = true;
      FirstFileDate.Text = Properties.Settings.Default.StartDate1.ToString();
      FirstFileNameTextBox.Text = Properties.Settings.Default.StartPath1.ToString();
      FirstFileNameTextBox.Width = this.Width - 60;
      SecondFileDate.Text = Properties.Settings.Default.StartDate2.ToString();
      SecondFileNameTextBox.Text = Properties.Settings.Default.StartPath2.ToString();
      SecondFileNameTextBox.Width = this.Width - 60;
    }
    //----< browse for first file >----------------------------------

    private void Browse1Button_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.InitialDirectory = System.IO.Path.GetDirectoryName(FirstFileNameTextBox.Text);
      if (dlg.ShowDialog() == true)
      {
        FileInfo fi = new FileInfo(dlg.FileName);
        string firstDate = fi.LastWriteTime.ToString();
        FirstFileDate.Text = firstDate;
        fileText = File.ReadAllText(dlg.FileName);
        FileTextBlock.Text = fileText;
        FirstFileNameTextBox.Text = dlg.FileName;
        startDir = System.IO.Path.GetDirectoryName(dlg.FileName);
        Properties.Settings.Default.StartPath1 = startDir;
        Properties.Settings.Default.StartDate1 = firstDate;
        Properties.Settings.Default.Save();
        CompareButton.IsEnabled = true;
      }
    }
    //----< browse for second file >---------------------------------

    private void Browse2Button_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.InitialDirectory = System.IO.Path.GetDirectoryName(SecondFileNameTextBox.Text);
      if (dlg.ShowDialog() == true)
      {
        FileInfo fi = new FileInfo(dlg.FileName);
        string secondDate = fi.LastWriteTime.ToString();
        SecondFileDate.Text = secondDate;
        fileText = File.ReadAllText(dlg.FileName);
        FileTextBlock.Text = fileText;
        SecondFileNameTextBox.Text = dlg.FileName;
        startDir = System.IO.Path.GetDirectoryName(dlg.FileName);
        Properties.Settings.Default.StartPath2 = startDir;
        Properties.Settings.Default.StartDate2 = secondDate;
        Properties.Settings.Default.Save();
        CompareButton.IsEnabled = true;
      }
    }
    //----< compute differences >------------------------------------

    private void CompareButton_Click(object sender, RoutedEventArgs e)
    {
      string args = FirstFileNameTextBox.Text + " " + SecondFileNameTextBox.Text;
      Process proc = new Process();
      proc.StartInfo.FileName = "c:\\windows\\system32\\fc.exe";
      proc.StartInfo.Arguments = args;
      proc.StartInfo.UseShellExecute = false;
      proc.StartInfo.RedirectStandardOutput = true;
      proc.StartInfo.CreateNoWindow = true;

      proc.Start();
      string output = proc.StandardOutput.ReadToEnd();
      int endLine = output.IndexOf('\n');
      string cutFirstLine = output.Substring(endLine + 1);
      FileTextBlock.Text = cutFirstLine;
      proc.WaitForExit();
    }
    //----< make file text width track window width >----------------

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      FirstFileNameTextBox.Width = this.Width - 60;
      SecondFileNameTextBox.Width = this.Width - 60;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //Properties.Settings.Default.Save();
    }
  }
}
