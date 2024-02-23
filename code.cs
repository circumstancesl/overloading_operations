using System;
using System.Drawing;
using System.Numerics;
using System.Text;

// исключение для несовместимых матриц
public class IncompatibleMatrixException : Exception
{
    public IncompatibleMatrixException(string message) : base(message) { }
}

// исключение для невозможности вычисления обратной матрицы
public class NonInvertibleMatrixException : Exception
{
    public NonInvertibleMatrixException(string message) : base(message) { }
}

class SquareMatrix : ICloneable
{
    private int Size;
    public int[,] Matrix;

    public SquareMatrix(int Size)
    {
        this.Size = Size;
        this.Matrix = new int[Size, Size];
    }

    public SquareMatrix(int Size, int MinValue, int MaxValue) : this(Size) // построение рандомной матрицы
    {
        Random rand = new Random();
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                this.Matrix[RowIndex, ColumnIndex] = rand.Next(MinValue, MaxValue);
            }
        }
    }

    public void PrintMatrix() // вывод матрицы
    {
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                Console.Write(Matrix[RowIndex, ColumnIndex] + " ");
            }
            Console.WriteLine();
        }
    }

    // Конструктор для создания копии другой матрицы
    public SquareMatrix(SquareMatrix other)
    {
        Size = other.Size;
        Matrix = new int[Size, Size];
        Array.Copy(other.Matrix, Matrix, other.Matrix.Length);
    }

    // Реализация интерфейса ICloneable
    public object Clone()
    {
        return new SquareMatrix(this);
    }

    public static SquareMatrix operator +(SquareMatrix Matrix1, SquareMatrix Matrix2) // Перегрузка оператора +
    {
        SquareMatrix result = new SquareMatrix(Matrix1.Size);

        for (int RowIndex = 0; RowIndex < Matrix1.Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Matrix1.Size; ++ColumnIndex)
            {
                result.Matrix[RowIndex, ColumnIndex] = Matrix1.Matrix[RowIndex, ColumnIndex] + Matrix2.Matrix[RowIndex, ColumnIndex];
            }
        }

        return result;
    }
    public static SquareMatrix operator *(SquareMatrix Matrix1, SquareMatrix Matrix2) // Перегрузка оператора *
    {
        SquareMatrix result = new SquareMatrix(Matrix1.Size);

        for (int RowIndex = 0; RowIndex < Matrix1.Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Matrix1.Size; ++ColumnIndex)
            {
                for (int ElementIndex = 0; ElementIndex < Matrix1.Size; ElementIndex++)
                {
                    result.Matrix[RowIndex, ColumnIndex] += Matrix1.Matrix[RowIndex, ElementIndex] * Matrix2.Matrix[ElementIndex, ColumnIndex];
                }
            }
        }

        return result;
    }

    // Перегрузка оператора ==
    public static bool operator ==(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        if (ReferenceEquals(Matrix1, Matrix2))
        {
            return true;
        }

        if ((object)Matrix1 == null || (object)Matrix2 == null)
        {
            return false;
        }

        return Matrix1.Equals(Matrix2);
    }

    // Перегрузка оператора !=
    public static bool operator !=(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        return !(Matrix1 == Matrix2);
    }

    // Перегрузка CompareTo
    public int CompareTo(SquareMatrix other)
    {
        if (other == null)
        {
            return 1;
        }

        if (Size != other.Size)
        {
            return Size.CompareTo(other.Size);
        }

        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                int Comparison = Matrix[RowIndex, ColumnIndex].CompareTo(other.Matrix[RowIndex, ColumnIndex]);
                if (Comparison != 0)
                {
                    return Comparison;
                }
            }
        }

        return 0;
    }

    // перегрузка оператора <
    public static bool operator <(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        return Matrix1.CompareTo(Matrix2) < 0;
    }

    // перегрузка оператора >
    public static bool operator >(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        return Matrix1.CompareTo(Matrix2) > 0;
    }

    // перегрузка оператора <=
    public static bool operator <=(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        return Matrix1.CompareTo(Matrix2) <= 0;
    }

    // перегрузка оператора >=
    public static bool operator >=(SquareMatrix Matrix1, SquareMatrix Matrix2)
    {
        return Matrix1.CompareTo(Matrix2) >= 0;
    }

    // Перегрузка метода ToString
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                sb.Append(Matrix[RowIndex, ColumnIndex]);
                sb.Append(" ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static explicit operator SquareMatrix(int[,] Array) // перегрузка приведения типов
    {
        int size = Array.GetLength(0);
        SquareMatrix Result = new SquareMatrix(size);
        Result.Matrix = Array;
        return Result;
    }

    public SquareMatrix Inverse()
    {
        double[,] AugmentedMatrix = new double[Size, 2 * Size];
        double[,] InverseMatrix = new double[Size, Size];

        // Создаем расширенную матрицу
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                AugmentedMatrix[RowIndex, ColumnIndex] = Matrix[RowIndex, ColumnIndex];
            }
            AugmentedMatrix[RowIndex, RowIndex + Size] = 1; // Добавляем единичную матрицу
        }

        // Приведение к верхнему треугольному виду
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            // Если элемент на главной диагонали равен нулю, меняем строки местами
            if (AugmentedMatrix[RowIndex, RowIndex] == 0)
            {
                for (int ColumnIndex = RowIndex + 1; ColumnIndex < Size; ++ColumnIndex)
                {
                    if (AugmentedMatrix[ColumnIndex, RowIndex] != 0)
                    {
                        SwapRows(AugmentedMatrix, RowIndex, ColumnIndex);
                        break;
                    }
                }
            }

            // Если после перестановки элемент на главной диагонали все еще равен нулю, матрица необратима
            if (AugmentedMatrix[RowIndex, RowIndex] == 0)
            {
                throw new NonInvertibleMatrixException("Матрица необратима.");
            }

            // Нормализация элемента на главной диагонали
            double Factor = AugmentedMatrix[RowIndex, RowIndex];
            for (int ColumnIndex = RowIndex; ColumnIndex < 2 * Size; ++ColumnIndex)
            {
                AugmentedMatrix[RowIndex, ColumnIndex] /= Factor;
            }

            // Приведение к верхнему треугольному виду
            for (int ColumnIndex = RowIndex + 1; ColumnIndex < Size; ++ColumnIndex)
            {
                double Factor2 = AugmentedMatrix[ColumnIndex, RowIndex];
                for (int ElementIndex = RowIndex; ElementIndex < 2 * Size; ++ElementIndex)
                {
                    AugmentedMatrix[ColumnIndex, ElementIndex] -= Factor2 * AugmentedMatrix[RowIndex, ElementIndex];
                }
            }
        }

        // Приведение к единичному виду
        for (int RowIndex = Size - 1; RowIndex >= 0; --RowIndex)
        {
            for (int ColumnIndex = RowIndex - 1; ColumnIndex >= 0; --ColumnIndex)
            {
                double Factor3 = AugmentedMatrix[ColumnIndex, RowIndex];
                for (int ElementIndex = 0; ElementIndex < 2 * Size; ++ElementIndex)
                {
                    AugmentedMatrix[ColumnIndex, ElementIndex] -= Factor3 * AugmentedMatrix[RowIndex, ElementIndex];
                }
            }
        }

        // Извлечение обратной матрицы из расширенной матрицы
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                InverseMatrix[RowIndex, ColumnIndex] = (int)AugmentedMatrix[RowIndex, ColumnIndex + Size];
            }
        }

        return InverseMatrix;
    }

    // Метод для обмена строк в матрице
    private void SwapRows(double[,] Matrix, int Row1, int Row2)
    {
        for (int ElementIndex = 0; ElementIndex < Size; ++ElementIndex)
        {
            double Temp = matrix[Row1, ElementIndex];
            Matrix[Row1, ElementIndex] = Matrix[Row2, ElementIndex];
            Matrix[Row2, ElementIndex] = Temp;
        }
    }


    // Метод для нахождения детерминанта
    public int Determinant()
    {
        return CalculateDeterminant(Matrix, Size);
    }

    // Рекурсивный метод для вычисления определителя
    private static int CalculateDeterminant(int[,] Matrix, int Size)
    {
        if (Size == 1)
        {
            return Matrix[0, 0];
        }

        int Determinant = 0;
        int Sign = 1;

        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            int[,] SubMatrix = new int[Size - 1, Size - 1];
            for (int ColumnIndex = 1; ColumnIndex < Size; ++ColumnIndex)
            {
                int columnIndex = 0;
                for (int ElementIndex = 0; ElementIndex < Size; ++ElementIndex)
                {
                    if (ElementIndex == RowIndex) continue;
                    SubMatrix[ColumnIndex - 1, columnIndex] = Matrix[ColumnIndex, ElementIndex];
                    columnIndex++;
                }
            }

            Determinant += Sign * Matrix[0, RowIndex] * CalculateDeterminant(SubMatrix, Size - 1);
            Sign = -Sign;
        }

        return Determinant;
    }

    // Перегрузка оператора true
    public static bool operator true(SquareMatrix Matrix)
    {
        return !Matrix.Equals(Matrix);
    }

    // Перегрузка оператора false
    public static bool operator false(SquareMatrix Matrix)
    {
        return Matrix.Equals(Matrix);
    }

    // Реализация метода Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        SquareMatrix other = (SquareMatrix)obj;

        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                if (Matrix[RowIndex, ColumnIndex] != other.Matrix[RowIndex, ColumnIndex])
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Реализация метода GetHashCode
    public override int GetHashCode()
    {
        int Hash = 17;
        for (int RowIndex = 0; RowIndex < Size; ++RowIndex)
        {
            for (int ColumnIndex = 0; ColumnIndex < Size; ++ColumnIndex)
            {
                Hash = Hash * 31 + Matrix[RowIndex, ColumnIndex];
            }
        }
        return Hash;
    }
}

class Program
{
    static void Main()
    {
        Random rand = new Random();
        int SizeMatrix = rand.Next(1, 5); // диапазон размеров матрицы
        int MinValueElementMatrix = 1; // минимальное значение элемента матрицы
        int MaxValueElementMatrix = 10; // максимальное значение элемента матрицы


        SquareMatrix Matrix1 = new SquareMatrix(SizeMatrix, MinValueElementMatrix, MaxValueElementMatrix); 
        SquareMatrix Matrix2 = new SquareMatrix(SizeMatrix, MinValueElementMatrix, MaxValueElementMatrix);
        SquareMatrix SumMatrix = Matrix1 + Matrix2;
        SquareMatrix ProductMatrix = Matrix1 * Matrix2;
        SquareMatrix InverseMatrix1 = Matrix1.Inverse();
        SquareMatrix InverseMatrix2 = Matrix2.Inverse();
        int DeterminantMatrix1 = Matrix1.Determinant();
        int DeterminantMatrix2 = Matrix2.Determinant();
        
        Console.WriteLine("Первая матрица:");
        Matrix1.PrintMatrix();

        Console.WriteLine("\nВторая матрица:");
        Matrix2.PrintMatrix();

        Console.WriteLine("\nСложение матриц:");
        SumMatrix.PrintMatrix();

        Console.WriteLine("\nУмножение матриц:");
        ProductMatrix.PrintMatrix();

        Console.WriteLine("\nОбратная первая матрица:");
        Console.WriteLine(InverseMatrix1);

        Console.WriteLine("\nОбратная вторая матрица:");
        Console.WriteLine(InverseMatrix2);

        Console.WriteLine("\nМатрицы равны: " + (Matrix1 == Matrix2));
        Console.WriteLine("Матрица 1 меньше матрицы 2: " + (Matrix1 < Matrix2));
        Console.WriteLine("Матрица 1 больше матрицы 2: " + (Matrix1 > Matrix2));
        Console.WriteLine("Матрица 1 меньше или равна матрице 2: " + (Matrix1 <= Matrix2));
        Console.WriteLine("Матрица 1 больше или равна матрице 2: " + (Matrix1 >= Matrix2));

        Console.WriteLine("\nОпределитель первой матрицы: " + DeterminantMatrix1);

        Console.WriteLine("\nОпределитель второй матрицы: " + DeterminantMatrix2);

    }
}
