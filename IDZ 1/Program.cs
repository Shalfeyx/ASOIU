using System;

namespace EditDistances
{
    internal class Program
    {
        public static int LevenshteinDistance(string str1Param, string str2Param)
        {
            if (str1Param == null || str2Param == null)
                return -1;

            int str1Len = str1Param.Length;
            int str2Len = str2Param.Length;

            if (str1Len == 0 && str2Len == 0)
                return 0;
            if (str1Len == 0)
                return str2Len;
            if (str2Len == 0)
                return str1Len;

            string str1 = str1Param.ToUpper();
            string str2 = str2Param.ToUpper();

            int[,] matrix = new int[str1Len + 1, str2Len + 1];

            for (int i = 0; i <= str1Len; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= str2Len; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= str1Len; i++)
            {
                for (int j = 1; j <= str2Len; j++)
                {
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;

                    int ins = matrix[i, j - 1] + 1;
                    int del = matrix[i - 1, j] + 1;
                    int subst = matrix[i - 1, j - 1] + cost;

                    matrix[i, j] = Math.Min(Math.Min(ins, del), subst);
                }
            }

            return matrix[str1Len, str2Len];
        }
        public static int DamerauLevenshteinDistance(string str1Param, string str2Param)
        {
            if (str1Param == null || str2Param == null)
                return -1;

            int str1Len = str1Param.Length;
            int str2Len = str2Param.Length;

            if (str1Len == 0 && str2Len == 0)
                return 0;
            if (str1Len == 0)
                return str2Len;
            if (str2Len == 0)
                return str1Len;

            string str1 = str1Param.ToUpper();
            string str2 = str2Param.ToUpper();

            int[,] matrix = new int[str1Len + 1, str2Len + 1];

            for (int i = 0; i <= str1Len; i++)
                matrix[i, 0] = i;
            for (int j = 0; j <= str2Len; j++)
                matrix[0, j] = j;

            for (int i = 1; i <= str1Len; i++)
            {
                for (int j = 1; j <= str2Len; j++)
                {
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;

                    int ins = matrix[i, j - 1] + 1;
                    int del = matrix[i - 1, j] + 1;
                    int subst = matrix[i - 1, j - 1] + cost;

                    matrix[i, j] = Math.Min(Math.Min(ins, del), subst);

                    if (i > 1 && j > 1 &&
                        str1[i - 1] == str2[j - 2] &&
                        str1[i - 2] == str2[j - 1])
                    {
                        matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + 1);
                    }
                }
            }

            return matrix[str1Len, str2Len];
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Программа вычисления редакционных расстояний");
            Console.WriteLine("1 - расстояние Левенштейна");
            Console.WriteLine("2 - расстояние Дамерау-Левенштейна");
            Console.WriteLine("Для выхода введите 'exit' в качестве первой строки\n");

            while (true)
            {
                Console.Write("Выберите алгоритм (1 или 2): ");
                string choice = Console.ReadLine();

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Некорректный выбор. Введите 1 или 2.\n");
                    continue;
                }

                Console.Write("Введите первую строку: ");
                string s1 = Console.ReadLine();

                if (s1?.ToLower() == "exit")
                {
                    Console.WriteLine("Выход из программы.");
                    break;
                }

                Console.Write("Введите вторую строку: ");
                string s2 = Console.ReadLine();

                int result;
                if (choice == "1")
                {
                    result = LevenshteinDistance(s1, s2);
                    Console.WriteLine($"Расстояние Левенштейна: {result}\n");
                }
                else
                {
                    result = DamerauLevenshteinDistance(s1, s2);
                    Console.WriteLine($"Расстояние Дамерау-Левенштейна: {result}\n");
                }
            }
        }
    }
}