using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Timer = System.Timers.Timer;

namespace TaskManagerHW
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Process> Processes { get; set; }
        public List<string> BlackList { get; set; }
        public ICommand BlackListCommand { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Processes = new(Process.GetProcesses());
            BlackList = new();
            BlackListCommand = new RelayCommand(ExecuteBlackListCommand);

            var timer = new Timer();
            timer.Interval = 5000;

            timer.Elapsed += Timer_ElapsedRefreshProcesses;
            timer.Elapsed += Timer_ElapsedBlackList;
            timer.Start();
        }

        private void ExecuteBlackListCommand(object? parameter)
        {
            if (Datas.SelectedItem is null)
                return;

            if (Datas.SelectedItem is Process p)
            {
                if (!BlackList.Contains(p.ProcessName))
                    BlackList.Add(p.ProcessName);
            }
        }

        private void Timer_ElapsedRefreshProcesses(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Processes.Clear();
                foreach (var p in Process.GetProcesses())
                    Processes.Add(p);
            }
            );

        }

        private void Timer_ElapsedBlackList(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var p in Processes)
                {
                    if (BlackList.Any(s => s == p.ProcessName))
                    {
                        try
                        {
                            p.Kill();
                            Processes.Remove(p);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message); ;
                        }
                    }
                }
            }
            );

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Datas.SelectedItem is null)
                return;

            if (Datas.SelectedItem is Process process)
            {
                try
                {
                    process.Kill();
                    Processes.Remove(process);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTxt.Text))
            {
                MessageBox.Show("Enter Process Name");
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new(SearchTxt.Text);

                startInfo.WindowStyle = ProcessWindowStyle.Minimized;

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BLTxt.Text))
            {
                MessageBox.Show("Enter Process Name");
                return;
            }

            BlackList.Add(BLTxt.Text);
        }
    }
}
