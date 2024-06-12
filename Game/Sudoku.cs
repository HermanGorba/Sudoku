using System;
using System.Threading.Tasks;

namespace Game
{
    public class Sudoku
    {
        private const int gridSize = 9;
        private const int subGridSize = 3;
        private readonly int[,] grid = new int[GridSize, GridSize];
        private int[,] solution = new int[GridSize, GridSize];
        private static readonly int[] numbers;

        private Random rand = new Random();




        public static int GridSize => gridSize;
        public int[,] Grid => grid;
        public int[,] Solution => solution;
        

        static Sudoku()
        {
            numbers = new int[GridSize];
            for (int i = 1; i <= GridSize; i++)
                numbers[i - 1] = i;
        }

        public async void Generate(int toRemove)
        {
            await Solve(0, 0);
            solution = (int[,])grid.Clone();
            RemoveNumbers(toRemove);
        }

        private int[] Rearrange(int[] array)
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                int j = i + rand.Next(length - i);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return array;
        }

        private async Task<bool> Solve(int row, int col)
        {
            if (row == GridSize)
                return true;

            if (col == GridSize)
                return await Solve(row + 1, 0);


            foreach (int num in Rearrange(numbers))
            {
                if (IsValidMove(row, col, num))
                {
                    grid[row, col] = num;

                    if (await Solve(row, col + 1))
                        return true;

                    grid[row, col] = 0;
                }
            }

            return false;
        }

        private bool IsValidMove(int row, int col, int num)
        {
            return !UsedInRow(row, num) && !UsedInCol(col, num) && !UsedInSubgrid(row - row % subGridSize, col - col % subGridSize, num);
        }

        private bool UsedInRow(int row, int num)
        {
            for (int col = 0; col < GridSize; col++)
                if (grid[row, col] == num)
                    return true;
            return false;
        }

        private bool UsedInCol(int col, int num)
        {
            for (int row = 0; row < GridSize; row++)
                if (grid[row, col] == num)
                    return true;
            return false;
        }

        private bool UsedInSubgrid(int row, int col, int num)
        {
            for (int i = 0; i < subGridSize; i++)
                for (int j = 0; j < subGridSize; j++)
                    if (grid[i + row, j + col] == num)
                        return true;
            return false;
        }

        private void RemoveNumbers(int toRemove)
        {
            while (toRemove > 0)
            {
                int row = rand.Next(GridSize);
                int col = rand.Next(GridSize);
                if (grid[row, col] != 0)
                {
                    grid[row, col] = 0;
                    toRemove--;
                }
            }
        }
    }
}
