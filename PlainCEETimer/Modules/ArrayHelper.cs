namespace PlainCEETimer.Modules
{
    public delegate void ForLoopCallback<T>(int index, T element);

    public static class ArrayHelper
    {
        public static void ForLoop<T>(T[] array, ForLoopCallback<T> callback)
        {
            for (int i = 0; i < array.Length; i++)
            {
                callback(i, array[i]);
            }
        }
    }
}
