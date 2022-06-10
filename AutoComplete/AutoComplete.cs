using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoComplete
{

    public struct FullName
    {
        public string Name;
        public string Surname;
        public string Patronymic;
    }
    public class AutoCompleter
    {
        private List<string> NamesInLine = new List<string>();

        public void AddToSearch(List<FullName> fullNames)
        {
            var fullNameSplit = fullNames.Select(x =>
            {
                string Surname = x.Surname != null ? x.Surname.Trim() : string.Empty;
                string name = x.Name != null ? x.Name.Trim() : string.Empty;
                string Patronymic = x.Patronymic != null ? x.Patronymic.Trim() : string.Empty;

                var fullName = Regex
                    .Split(Surname + " " + name + " " + Patronymic, @"\W+")
                    .Where(x => x != "");

                return string.Join(" ", fullName);
            });

            NamesInLine = fullNameSplit.OrderBy(x => x).ToList();
        }

        public List<string> Search(string prefix)
        {
            if (prefix == string.Empty
                || prefix.Length > 100
                || Regex.Replace(prefix, " ", string.Empty) == string.Empty)
                throw new ArgumentException();

            List<string> answer = new List<string>();

            var startCount = GetCountByPrefix(NamesInLine, prefix);
            for (int i = startCount.Item2; i > 0; i--)
                answer.Add(NamesInLine[i + startCount.Item1]);

            return answer;
        }

        public static int GetLeftBorderIndex(IReadOnlyList<string> phrases, string prefix, int left, int right)
        {
            if (left == right - 1) return left;
            var m = (left + right) / 2;
            if (string.Compare(prefix, phrases[m], StringComparison.OrdinalIgnoreCase) < 0
                    || phrases[m].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return GetLeftBorderIndex(phrases, prefix, left, m);
            return GetLeftBorderIndex(phrases, prefix, m, right);
        }

        public static int GetRightBorderIndex(List<string> phrases, string prefix, int left, int right)
        {
            while (left < right - 1)
            {
                var m = (left + right) / 2;
                if (string.Compare(prefix, phrases[m], StringComparison.OrdinalIgnoreCase) > 0
                    || phrases[m].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    left = m;
                else right = m;
            }
            return right;
        }

        public static (int, int) GetCountByPrefix(List<string> phrases, string prefix)
        {
            var right = Array.BinarySearch(phrases.ToArray(), prefix);// GetRightBorderIndex(phrases, prefix, -1, phrases.Count);
            var left = GetLeftBorderIndex(phrases, prefix, -1, phrases.Count);
            return (left, right - left - 1);
        }

        public static void Main()
        {
            var lists = new List<FullName>
            {
                new FullName{Name = "Анна  ", Patronymic = "  Петровна", Surname ="Чингачкук" },
                new FullName{Name = "Марина", Patronymic = "Сергеевна", Surname ="Чичваркина" },
                new FullName{Name = "Вася", Patronymic = "Александрович", Surname ="Петров" },
                new FullName{Name = "Яков", Patronymic = null, Surname ="Семёнов" },
                new FullName{Name = null, Patronymic = null, Surname ="Чингачкук" },
                new FullName{Name = "Марина", Patronymic = null, Surname =null },
                new FullName{Name = "   Сергей", Patronymic = null, Surname =null },
                new FullName{Name = "Егор", Patronymic = null, Surname =null },
                new FullName{Name = null, Patronymic = "Алексеевич", Surname =null },
            };
            var newExemplar = new AutoCompleter();
            newExemplar.AddToSearch(lists);

            string inputData = "F";
            while (inputData != "")
            {
                Console.WriteLine("проверка начата введите пустую строку для выхода");
                inputData = Console.ReadLine();
                foreach (var element in newExemplar.Search(inputData))
                    Console.WriteLine(element + "\n");
            }
        }
    }
}