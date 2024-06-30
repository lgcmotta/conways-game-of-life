namespace Conways.GameOfLife.Infrastructure.Extensions;

public static class TypeExtensions
{
    public static T[,] ToMultiArray<T>(this T[][] array)
    {
        var rows = array.Length;
        
        var columns = array[0].Length;
        
        var multiArray = new T[rows, columns];
        
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                multiArray[i, j] = array[i][j];
            }
        }
        
        return multiArray;
    }
}