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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 俄罗斯方块promax
{
    /// <summary>
    /// GameWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GameWindow : Window
    {
        private DispatcherTimer gameTimer;

        private const int Rows = 20;
        private const int Columns = 10;
        private const int BlockSize = 30;

        private int[,] area = new int[31, 12];
        private int speed = 1000, score = 0; // 分数
        private int[,,] block = new int[7, 4, 4]
        {
            {{0,0,0,0},
             {0,1,1,0},
             {0,1,1,0},
             {0,0,0,0}},

            {{0,1,0,0},
             {0,1,0,0},
             {0,1,0,0},
             {0,1,0,0}},

            {{0,0,0,0},
             {0,0,1,1},
             {0,1,1,0},
             {0,0,0,0}},

            {{0,0,0,0},
             {1,1,0,0},
             {0,1,1,0},
             {0,0,0,0}},

            {{0,1,0,0},
             {0,1,0,0},
             {0,1,1,0},
             {0,0,0,0}},

            {{0,0,0,0},
             {0,0,1,0},
             {0,0,1,0},
             {0,1,1,0}},

            {{0,0,0,0},
             {0,0,1,0},
             {0,1,1,1},
             {0,0,0,0}}
        }; // 每个方块的数据
        private bool totalstop = false; // 全局停止信号
        private bool isHandlingInput = false;
        public MainWindow mainwindowscopy;

        private DateTime startTime;


        public GameWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.PreviewKeyDown += GameWindow_PreviewKeyDown; // 绑定PreviewKeyDown事件
            mainwindowscopy = mainWindow;
        }

        //创建游戏界面
        private void InitializeGame()
        {
            // 动态创建行和列
            for (int i = 0; i < Rows; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BlockSize) });
            }

            for (int i = 0; i < Columns; i++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(BlockSize) });
            }

            // 创建并放置矩形
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var block = new Rectangle
                    {
                        Width = BlockSize,
                        Height = BlockSize,
                        Stroke = Brushes.Gray
                    };
                    Grid.SetRow(block, row);
                    Grid.SetColumn(block, col);
                    GameGrid.Children.Add(block);
                }
            }

        }

        // 初始化游戏区域数组
        private void InitializeArea()
        {
            for (int row = 0; row <= 30; row++)
            {
                for (int col = 0; col <= 11; col++)
                {
                    area[row, col] = 0; // 0 表示空单元格
                }
            }
            //绘制边框
            for (int i = 0; i <= 10; i++)
                area[0, i] = 1;//底边框
            for (int i = 0; i <= 30; i++)
                area[i, 0] = 1;//左边框
            for (int i = 0; i <= 30; i++)
                area[i, 11] = 1;//右边框
        }

        // 更新游戏界面，使其反映area数组的状态
        private void UpdateGameGrid()
        {
            ScoreBlock.Text = "SCORE : " + score;

            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    // 获取对应的Rectangle控件
                    var block = (Rectangle)GameGrid.Children.Cast<UIElement>().FirstOrDefault(e =>
                        Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                    if (block != null)
                    {
                        // 映射area的1-20行和1-10列到GameGrid的0-19行和0-9列
                        int areaRow = 20 - row;
                        int areaCol = col + 1;

                        switch (area[areaRow, areaCol])
                        {
                            case 0:
                                block.Fill = Brushes.White; // 空单元格设为白色
                                break;
                            case 1:
                                block.Fill = new SolidColorBrush(Color.FromRgb(53, 206, 238)); // 1 号方块设为蓝色
                                break;
                            case 2:
                                block.Fill = new SolidColorBrush(Color.FromRgb(238, 100, 53)); // 2 号方块设为红色
                                break;
                            // 其他情况可以根据需要添加
                            default:
                                block.Fill = Brushes.White; // 默认颜色设为白色
                                break;
                        }
                    }
                }
            }
        }

        //启动操作
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGame();
            InitializeArea();
            UpdateGameGrid();

            startTime = DateTime.UtcNow;

            gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(speed)
            };
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

        }

        //创建方块
        public void Create()
        {
            // C++中的srand和time在C#中可以通过System.Random类实现
            Random random = new Random();

            // 该变量用于记录当前界面内有没有移动的方块
            bool flag = false;
            // 遍历判断有没有移动的方块
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (area[i, j] == 2)
                    {
                        flag = true;
                    }
                }
                // 有方块则不再生成方块
                if (flag)
                    return;
            }

            int kuai = random.Next(7); // 随机选择一个方块
            int xp = random.Next(2) == 0 ? 1 : -1; // 随机左右方向
            int yp = random.Next(2) == 0 ? 1 : -1; // 随机上下方向

            // 计算初始位置
            int startX = 4; // 从屏幕顶部中央开始生成
            int startY = 22; // 从第22行开始生成

            // 生成块于界面顶部
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int x = startX + (i * xp); // 计算横向位置
                    int y = startY + (j * yp); // 计算纵向位置

                    // 检查是否超出边界
                    if (x < 1 || x > 10 || y < 1 || y > 30) continue;

                    if (block[kuai, i, j] == 1)
                    {
                        area[y, x] = 2;
                    }
                }
            }
        }

        //下落
        public void drop()
        {

            //判断该方块底部是否有别的方块
            bool move = true;
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (area[i, j] == 2)
                    {
                        if (area[i - 1, j] == 1)
                            move = false;
                    }
                }
            }

            //如果有方块底部有别的方块，则冻结所有方块
            if (move == false)
            {
                iceall();
                return;
            }

            //将所有移动方块下移
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (area[i, j] == 2)
                    {
                        area[i - 1, j] = 2;
                        area[i, j] = 0;
                    }
                }
            }
        }

        //冻结
        public void iceall()
        {
            for (int i = 1; i <= 30; i++)
                for (int j = 1; j <= 10; j++)
                    if (area[i, j] == 2)
                        area[i, j] = 1;
        }

        //停止
        public void stop()
        {
            bool s = false;
            //判断20行（顶端）有没有静止的方块
            for (int i = 1; i <= 10; i++)
            {
                if (area[20, i] == 1)
                    s = true;
            }
            //有的话发出全局终止信号
            if (s)
                totalstop = true;
        }

        //用于清除完整的行
        void del()
        {
            int cou = 0;

            //遍历每一行
            for (int i = 1; i <= 20; i++)
            {
                int count = 0;
                //对该行所有静止方块进行统计
                for (int j = 1; j <= 10; j++)
                {
                    count += (area[i, j] == 1) ? 1 : 0;
                }
                //如果该行补齐，将该行以上的行全体下移，分数+1
                if (count == 10)
                {
                    cou++;
                    if (speed > 200)
                        speed -= 10;
                    for (int k = i; k <= 20; k++)
                    {
                        for (int l = 1; l <= 10; l++)
                        {
                            area[k, l] = area[k + 1, l];
                        }
                    }
                    //重新判断该行（连续消除情况）
                    i--;
                }
            }

            score += cou > 1 ? (int)Math.Pow(2, cou) : cou;
        }

        //用于左移所有移动方块
        void leftmove()
        {
            //判断该方块左边是否有别的方块
            bool move = true;
            for (int j = 1; j <= 10; j++)
            {
                for (int i = 1; i <= 30; i++)
                {
                    if (area[i, j] == 2)
                    {
                        if (area[i, j - 1] == 1)
                            move = false;
                    }
                }
                //有方块不进行操作
                if (move == false)
                {
                    return;
                }
            }

            //将所有移动方块左移
            for (int j = 1; j <= 10; j++)
            {
                for (int i = 1; i <= 30; i++)
                {
                    if (area[i, j] == 2)
                    {
                        area[i, j - 1] = 2;
                        area[i, j] = 0;
                    }
                }
            }
        }

        //用于右移所有移动方块
        void rightmove()
        {
            //判断该方块右边是否有别的方块
            bool move = true;
            for (int j = 10; j >= 1; j--)
            {
                for (int i = 1; i <= 30; i++)
                {
                    if (area[i, j] == 2)
                    {
                        if (area[i, j + 1] == 1)
                            move = false;
                    }
                }
                //有的话不操作直接返回
                if (move == false)
                {
                    return;
                }
            }

            //将所有移动方块右移
            for (int j = 10; j >= 1; j--)
            {
                for (int i = 1; i <= 30; i++)
                {
                    if (area[i, j] == 2)
                    {
                        area[i, j + 1] = 2;
                        area[i, j] = 0;
                    }
                }
            }
        }

        //旋转
        public void Rotate()
        {
            // 找到所有移动方块的位置
            List<Tuple<int, int>> currentBlock = new List<Tuple<int, int>>();
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (area[i, j] == 2)
                    {
                        currentBlock.Add(Tuple.Create(i, j));
                    }
                }
            }

            if (currentBlock.Count == 0)
            {
                return; // 没有移动方块，直接返回
            }

            // 找到当前方块的中心点
            int centerX = currentBlock[0].Item1;
            int centerY = currentBlock[0].Item2;

            // 临时存储旋转后的方块位置
            List<Tuple<int, int>> rotatedBlock = new List<Tuple<int, int>>();

            // 计算旋转后的坐标
            foreach (var pos in currentBlock)
            {
                // 套用旋转公式
                int newX = centerX + (centerY - pos.Item2);
                int newY = centerY + (pos.Item1 - centerX);
                rotatedBlock.Add(Tuple.Create(newX, newY));
            }

            // 检查旋转后的方块是否与现有方块或边界冲突
            bool canRotate = true;
            foreach (var pos in rotatedBlock)
            {
                if (pos.Item2 < 1 || pos.Item2 > 10 || pos.Item1 < 1 || pos.Item1 > 30 || area[pos.Item1, pos.Item2] == 1)
                {
                    canRotate = false;
                    break;
                }
            }

            if (canRotate)
            {
                // 清除原来的方块位置
                foreach (var pos in currentBlock)
                {
                    area[pos.Item1, pos.Item2] = 0;
                }

                // 更新旋转后的方块位置
                foreach (var pos in rotatedBlock)
                {
                    area[pos.Item1, pos.Item2] = 2;
                }
            }
        }

        //游戏时钟
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // 计算新的间隔时间
            //int newInterval = Math.Max(speed - score * 10, 100);
            //gameTimer.Interval = TimeSpan.FromMilliseconds(newInterval);

            // 执行游戏逻辑
            drop();
            UpdateGameGrid();
            del();
            Create();
            stop();

            // 如果游戏结束
            if (totalstop)
            {
                gameTimer.Stop();
                MessageBox.Show("游戏结束！");
                this.Close();
            }
        }

        //关闭时的操作
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainwindowscopy.array[mainwindowscopy.list, 0] = score;
            mainwindowscopy.array[mainwindowscopy.list, 1] = ToUnixTimestamp(startTime);
            mainwindowscopy.array[mainwindowscopy.list, 2] = ToUnixTimestamp(DateTime.UtcNow);
            mainwindowscopy.list++;
            mainwindowscopy.Show();
        }

        //时间戳转换
        static int ToUnixTimestamp(DateTime dateTime)
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = dateTime - epochStart;
            return (int)timeSpan.TotalSeconds;
        }

        //键盘事件
        private void GameWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (totalstop || isHandlingInput) return; // 如果游戏已停止或正在处理输入，不处理键盘输入

            isHandlingInput = true; // 设置防抖标志

            switch (e.Key)
            {
                case Key.A: // 左移
                    leftmove();
                    UpdateGameGrid(); // 更新界面
                    break;
                case Key.D: // 右移
                    rightmove();
                    UpdateGameGrid(); // 更新界面
                    break;
                case Key.J: // 旋转
                    Rotate();
                    UpdateGameGrid(); // 更新界面
                    break;
                case Key.S: // 下降
                    drop();
                    UpdateGameGrid(); // 更新界面
                    break;
            }

            isHandlingInput = false; // 重置防抖标志
            e.Handled = true; // 标记事件已处理，防止进一步传播
        }
    }
}
