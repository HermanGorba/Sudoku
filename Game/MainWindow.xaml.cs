using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Sudoku sudoku;
        TextBox[,] textBoxes;
        int unguessed;
        int incorrectAnswers;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateSudokuGrid();
            NewGameButton.Focus();
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = (sender as TextBox)?.Text.Length == 1 || !int.TryParse(e.Text, out _);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int row = Grid.GetRow(textBox);
                int col = Grid.GetColumn(textBox);

                // because of separators
                int solutionRow = row > 2 ? (row > 6 ? row - 2 : row - 1) : row;
                int solutionCol = col > 2 ? (col > 6 ? col - 2 : col - 1) : col;

                if (int.TryParse(textBox.Text, out int num) && sudoku.Solution[solutionRow, solutionCol] == num)
                {
                    ToGuessedTextBoxStyle(textBox);

                    if (sudoku.Grid[solutionRow, solutionCol] == 0)
                    {
                        textBox.Background = Brushes.White;
                        textBox.Foreground = Brushes.Blue;
                        unguessed--;

                        if (unguessed == 0)
                        {
                            SolutionButton.IsEnabled = false;
                            NewGameButton.IsEnabled = true;

                            if (IsSolvedByUser())
                                MessageBox.Show($"You won with {incorrectAnswers} incorrect answers. Well played!", "Sudoku solved!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            else
                                MessageBox.Show($"Game ended with {incorrectAnswers} incorrect answers.\nDon\'t get upset!", $"Sudoku solved!", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                    }
                } 
                else if (textBox.Text == string.Empty)
                {
                    ResetTextBoxStyle(textBox);
                }
                else 
                {
                    ToIncorrectTextBoxStyle(textBox);
                    incorrectAnswers++;
                }

            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            incorrectAnswers = 0;
            unguessed = 40;
            sudoku = new Sudoku();

            sudoku.Generate(unguessed);
            SolutionButton.IsEnabled = true;
            NewGameButton.IsEnabled = false;
            
            foreach (var textBox in textBoxes)
                ResetTextBoxStyle(textBox);
            

            for (int i = 0; i < Sudoku.GridSize; i++) 
            {
                for (int j = 0; j < Sudoku.GridSize; j++) 
                {
                    TextBox textBox = textBoxes[i, j];
                    int num = sudoku.Grid[i, j];

                    if (num > 0)
                        textBox.Text = num.ToString();
                }
            }
        }
        private void ShowSolutionButton_Click(object sender, RoutedEventArgs e)
        {
            SolutionButton.IsEnabled = false;
            NewGameButton.IsEnabled = true;

            for (int i = 0; i < Sudoku.GridSize; i++)
            {
                for (int j = 0; j < Sudoku.GridSize; j++)
                {
                    TextBox textBox = textBoxes[i, j];

                    if (textBox.IsEnabled == true)
                    {
                        textBox.Text = sudoku.Solution[i, j].ToString();
                        ToSolvedTextBoxStyle(textBox);
                    }
                }
            }
        }


        private void GenerateSudokuGrid()
        {
            int gridSize = Sudoku.GridSize;
            textBoxes = new TextBox[gridSize, gridSize];
            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    // because of separators
                    int currentRow = row > 2 ? (row > 5 ? row + 2 : row + 1) : row;
                    int currentCol = col > 2 ? (col > 5 ? col + 2 : col + 1) : col;

                    TextBox textBox = new()
                    {
                        IsEnabled = false,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        FontSize = this.Width / 20,
                        BorderBrush = new SolidColorBrush(Color.FromRgb(242, 242, 242)),
                        BorderThickness = new Thickness(3)
                        
                    };
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBoxes[row, col] = textBox;

                    Grid.SetRow(textBox, currentRow);
                    Grid.SetColumn(textBox, currentCol);

                    SudokuGrid.Children.Add(textBox);
                }
            }
        }
        private bool IsSolvedByUser() 
        {
            for (int i = 0; i < textBoxes.GetLength(0); i++)
                for (int j = 0; j < textBoxes.GetLength(1); j++)
                    if (textBoxes[i, j].Foreground == Brushes.Red)
                        return false;
            return true;
        }
        private static void ToGuessedTextBoxStyle(in TextBox textBox)
        {
            textBox.IsEnabled = false;
            textBox.FontWeight = FontWeights.Bold;
        }
        private static void ResetTextBoxStyle(in TextBox textBox)
        {
            textBox.Text = string.Empty;
            textBox.IsEnabled = true;
            textBox.FontWeight = FontWeights.Normal;
            textBox.Foreground = Brushes.Black;
            textBox.Background = Brushes.White;
        }
        private static void ToSolvedTextBoxStyle(in TextBox textBox)
        {
            textBox.IsEnabled = false;
            textBox.FontWeight = FontWeights.Bold;
            textBox.Foreground = Brushes.Red;
        }
        private static void ToIncorrectTextBoxStyle(in TextBox textBox) 
        {
            textBox.Background = Brushes.LightCoral;
        }
    }
}