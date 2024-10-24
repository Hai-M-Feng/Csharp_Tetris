using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace 俄罗斯方块promax
{
    public class DataRow : INotifyPropertyChanged
    {
        private string column1;
        private string column2;
        private string column3;

        public string Column1
        {
            get => column1;
            set
            {
                column1 = value;
                OnPropertyChanged();
            }
        }

        public string Column2
        {
            get => column2;
            set
            {
                column2 = value;
                OnPropertyChanged();
            }
        }

        public string Column3
        {
            get => column3;
            set
            {
                column3 = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// HistoryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWindow : Window
    {
        MainWindow mainWindow1;

        public ObservableCollection<DataRow> DataRows { get; set; }

        public HistoryWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            mainWindow1 = mainWindow;
            DataRows = new ObservableCollection<DataRow>();
            dataGrid.ItemsSource = DataRows;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < mainWindow1.list; i++)
            {
                DataRows.Add(new DataRow
                {
                    Column1 = mainWindow1.array[i, 0].ToString(),
                    Column2 = FromUnixTimestamp(mainWindow1.array[i, 1]),
                    Column3 = FromUnixTimestamp(mainWindow1.array[i, 2])
                });
            }
        }

        static string FromUnixTimestamp(int unixTimestamp)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTimeUtc = epochStart.AddSeconds(unixTimestamp);
            DateTime dateTimeLocal = dateTimeUtc.ToLocalTime();
            return dateTimeLocal.ToString("yyyy-MM-dd HH:mm:ss"); // 你可以根据需要调整格式
        }
    }
}
