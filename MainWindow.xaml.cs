using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AntSimulator;

public partial class MainWindow : Window
{
    private const int Size = 45;
    private const double CellSize = 12;
    private readonly bool[,] cells = new bool[Size, Size];
    private readonly Rectangle[,] rectangles = new Rectangle[Size, Size];
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(50) };
    private readonly Polygon ant = new() { Fill = new SolidColorBrush(Color.FromRgb(218, 117, 75)) };
    private int x;
    private int y;
    private int direction;
    private int steps;

    public MainWindow()
    {
        InitializeComponent();
        CreateField();
        timer.Tick += (_, _) => Step();
        ResetField();
    }

    private void CreateField()
    {
        for (int row = 0; row < Size; row++)
        {
            for (int column = 0; column < Size; column++)
            {
                var rectangle = new Rectangle
                {
                    Width = CellSize,
                    Height = CellSize,
                    Stroke = new SolidColorBrush(Color.FromRgb(232, 238, 232)),
                    StrokeThickness = 0.5
                };
                System.Windows.Controls.Canvas.SetLeft(rectangle, column * CellSize);
                System.Windows.Controls.Canvas.SetTop(rectangle, row * CellSize);
                rectangles[column, row] = rectangle;
                Field.Children.Add(rectangle);
            }
        }

        Field.Children.Add(ant);
    }

    private void ResetField()
    {
        timer.Stop();
        Array.Clear(cells);
        for (int row = 0; row < Size; row++)
            for (int column = 0; column < Size; column++)
                rectangles[column, row].Fill = Brushes.White;

        x = Size / 2;
        y = Size / 2;
        direction = 0;
        steps = 0;
        StepsText.Text = "0";
        StatusText.Text = "Готов к запуску";
        StartButton.IsEnabled = true;
        PauseButton.IsEnabled = false;
        DrawAnt();
    }

    private void Step()
    {
        // Направления: вверх, вправо, вниз, влево.
        direction = (direction + (cells[x, y] ? 3 : 1)) % 4;
        cells[x, y] = !cells[x, y];
        rectangles[x, y].Fill = cells[x, y] ? Brushes.Black : Brushes.White;

        switch (direction)
        {
            case 0: y = (y - 1 + Size) % Size; break;
            case 1: x = (x + 1) % Size; break;
            case 2: y = (y + 1) % Size; break;
            case 3: x = (x - 1 + Size) % Size; break;
        }

        StepsText.Text = (++steps).ToString("N0");
        DrawAnt();
    }

    private void DrawAnt()
    {
        // Треугольник показывает положение муравья и направление его движения.
        Point[] points = direction switch
        {
            0 => [new(6, 1), new(1, 11), new(11, 11)],
            1 => [new(11, 6), new(1, 1), new(1, 11)],
            2 => [new(6, 11), new(1, 1), new(11, 1)],
            _ => [new(1, 6), new(11, 1), new(11, 11)]
        };
        ant.Points = new PointCollection(points);
        System.Windows.Controls.Canvas.SetLeft(ant, x * CellSize);
        System.Windows.Controls.Canvas.SetTop(ant, y * CellSize);
    }

    private void Generate_Click(object sender, RoutedEventArgs e) => ResetField();

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        timer.Start();
        StartButton.IsEnabled = false;
        PauseButton.IsEnabled = true;
        StatusText.Text = "Симуляция запущена";
    }

    private void Pause_Click(object sender, RoutedEventArgs e)
    {
        timer.Stop();
        StartButton.IsEnabled = true;
        PauseButton.IsEnabled = false;
        StatusText.Text = "Симуляция на паузе";
    }
}
