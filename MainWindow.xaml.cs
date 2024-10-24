using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
using System.Windows.Threading;

namespace 俄罗斯方块promax
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[,] array = new int[1000, 3];
        public int list = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        //载入
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = "array.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string[] values = lines[i].Split(',');
                    for (int j = 0; j < values.Length; j++)
                    {
                        array[i, j] = int.Parse(values[j]);
                    }
                    if (array[i, 0] != 0)
                    {
                        list++;
                    }
                }
            }
        }

        //按下开始按钮
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // 实例化新窗口
            GameWindow newWindow = new GameWindow(this);

            // 显示新窗口
            newWindow.Show();

            this.Hide();
        }

        //按下帮助按钮
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AD左右，S向下，J翻滚，请使用英文输入法游玩\n我们鼓励多消，双消4分，三消8分，四消16分\n随着分数的增长，下落速度会显著加快");
        }

        //关闭时的操作
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //保存文件
            string filePath = "array.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        writer.Write(array[i, j]);
                        if (j < array.GetLength(1) - 1)
                        {
                            writer.Write(",");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            HistoryWindow newWindow = new HistoryWindow(this);

            // 显示新窗口
            newWindow.Show();
        }
    }  
}
