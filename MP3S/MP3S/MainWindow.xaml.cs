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
using MahApps.Metro.Controls;
using System.Windows.Forms;
using System.IO;

namespace MP3S
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<string> folders;
        private List<string> files;
        Debug debug;
        public MainWindow()
        {
            InitializeComponent();
            debug = new Debug();
            folders = new List<string>();
            files = new List<string>();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if(result.ToString() == "Cancel")
            {
                debug.richTextBox1.AppendText("Aborting add folder...\n");
                debug.richTextBox1.ScrollToEnd();
                return;
            }
            int index = dialog.SelectedPath.Split('\\').Length - 1;
            if (dialog.SelectedPath.Split('\\')[index] == "")
            {
                debug.richTextBox1.AppendText("Path ended with slash, handling it...\n");
                debug.richTextBox1.ScrollToEnd();
                listBox.Items.Add(dialog.SelectedPath.Split('\\')[index - 1]);
                folders.Add(dialog.SelectedPath);
            }
            else
            {
                listBox.Items.Add(dialog.SelectedPath.Split('\\')[index]);
                folders.Add(dialog.SelectedPath);
            }
            //Console.WriteLine("+" + result.ToString() + "+");
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if(listBox.SelectedIndex == -1)
            {
                debug.richTextBox1.AppendText("No item selected, aborting remove...\n");
                return;
            }
            folders.RemoveAt(listBox.SelectedIndex);
            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }

        private void print(string s)
        {
            Console.WriteLine(s);
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            changeButtonsState(false);
            prog.IsIndeterminate = true;
            try
            {
                await Task.Run(() =>
                {
                    listBox1.Dispatcher.BeginInvoke((Action)(() => { listBox1.Items.Clear(); }));
                    files.Clear();
                    
                    foreach (string folder in folders)
                    {
                        checkFolder(folder);
                    }

                label1.Dispatcher.BeginInvoke((Action)(() => { label1.Content = "Files found: " + files.Count; }));
                });
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            prog.IsIndeterminate = false;
            listFiles();
            changeButtonsState(true);
            debug.richTextBox1.AppendText("Found " + files.Count + " files in " + (int)DateTime.Now.Subtract(start).TotalMilliseconds + " ms (" + ((double)DateTime.Now.Subtract(start).TotalSeconds).ToString("0.0") + " seconds). An average of " + (int)(files.Count / (DateTime.Now.Subtract(start)).TotalSeconds) + " files per second.\n");
        }

        private void changeButtonsState(bool boolean)
        {
            if(boolean == false)
            {
                button.IsEnabled = false;
                button1.IsEnabled = false;
                button2.IsEnabled = false;
                checkBox.IsEnabled = false;
            }
            else
            {
                button.IsEnabled = true;
                button1.IsEnabled = true;
                button2.IsEnabled = true;
                checkBox.IsEnabled = true;
            }
        }

        private void checkFolder(string folder)
        {
            try
            {
                Directory.GetFiles(folder, "*.mp3").ToList().ForEach(s => files.Add(s));
                Directory.GetDirectories(folder).ToList().ForEach(s => checkFolder(s));
            }
            catch (UnauthorizedAccessException ex)
            {
                debug.Dispatcher.BeginInvoke((Action)(() => { debug.richTextBox1.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] Access denied to: " + folder + "\n"); }));
                debug.Dispatcher.BeginInvoke((Action)(() => { debug.richTextBox1.ScrollToEnd(); }));
            }
            
        }

        private void listFiles()
        {
            if (checkBox.IsChecked == true)
            {
                foreach (string file in files)
                {
                    listBox1.Items.Add(file.Split('\\')[file.Split('\\').Length - 1]);
                }
            }
            else
            {
                foreach (string file in files)
                {
                    listBox1.Dispatcher.BeginInvoke((Action)(() => { listBox1.Items.Add(file); })) ;
                }
            }
        }


        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {
            label1.Visibility = Visibility.Hidden;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if(label1 != null)
            label1.Visibility = Visibility.Visible;
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            debug.Left = this.Left;
            debug.Top = this.Top + (this.ActualHeight);
            debug.Show();
            this.Focus();
        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            if(checkBox2 != null)
            debug.Hide();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MetroWindow_LocationChanged(object sender, EventArgs e)
        {
            debug.Left = this.Left;
            debug.Top = this.Top + (this.ActualHeight );
        }
    }
}
