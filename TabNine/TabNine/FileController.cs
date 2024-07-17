using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TabNine
{
    internal class FileController
    {
        public static string GetPath()
        {
            return $"{Environment.CurrentDirectory}\\ProgFiles\\";
        }
        public static List<string> Read(string path = "summary.txt")
        {

            // Получение потока из ресурсов
            if (File.Exists(GetPath() + path))
            {
                List<string> content = File.ReadLines(GetPath() + path).ToList();
                return content;
            }
            else
            {
                throw new Exception("The file doesn't exist.");
            }
        }

        public static void Write(string content, string path = "summary.txt")
        {
            if (!File.Exists(GetPath() + path))
            {
                using (StreamWriter sw = File.CreateText(GetPath() + path))
                {
                    sw.Write(content);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(GetPath() + path))
                {
                    sw.Write(content);
                }
            }
        }

        public static void Rewriteline(string searchWord, int pos, string addString = null, string path = "\\summary.txt")
        {
            string fileName = MainWindow.pathDictionary[searchWord[0].ToString()];
            string line = Find(searchWord, fileName);
            string[] elements = line.Split(' ');
            if (pos == 1 && addString == null)
            {
                List<string> lines = Read("\\" + fileName);
                string newLine = "";
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i] != "")
                    {
                        if (i == 1)
                            newLine += (Convert.ToInt32(elements[1]) + 1).ToString() + " ";
                        else
                            newLine += elements[i] + " ";
                    }
                }
                newLine = newLine.Remove(newLine.Length - 1);
                Console.WriteLine($"New line - \"{line}\"");
                for (global::System.Int32 i = 0; i < lines.Count; i++)
                {
                    if (lines[i] == line)
                        lines[i] = newLine;
                    if (lines[i].Split(' ')[1] != "0")
                        Console.WriteLine(lines[i]);
                }
                lines.Sort(new CustomComparer());

                StreamWriter writer = new StreamWriter(GetPath() + fileName);
                writer.Write(String.Join("\n", lines));
                writer.Close();
            }

        }

        public static void AddWord(string word)
        {
            string path = MainWindow.pathDictionary[word[0].ToString().ToLower()];
            List<string> lines = Read(path);
            lines.Add(word + " 1 {}");
            lines.Sort(new CustomComparer());
            string content = String.Join("\n", lines);
            StreamWriter writer = new StreamWriter(GetPath() + path);
            writer.Write(content);
            writer.Close();
        }

        public static void RemoveLine(string word)
        {
            string fileName = MainWindow.pathDictionary[word[0].ToString().ToLower()];
            string line = Find(word, fileName);
            Console.WriteLine(line);
            List<string> lines = Read("\\" + fileName);
            lines.Remove(line);

            StreamWriter writer = new StreamWriter(GetPath() + fileName);
            writer.Write(String.Join("\n", lines));
            writer.Close();
        }

        public static List<string> GetRalatedWords(string word)
        {
            if ("1234567890".Contains(word[0]))
                return null;
            string fileName = MainWindow.pathDictionary[word[0].ToString().ToLower()];
            string line = Find(word, fileName);
            if (line == null) return null;
            string ralatedWordsString = line.Substring(line.IndexOf('{') + 1).Replace("}", "");
            if (String.IsNullOrEmpty(ralatedWordsString))
                return null;
            List<string> ralatedWordsList = ralatedWordsString.Split(',').ToList();
            List<string> words = new List<string>();
            foreach (var item in ralatedWordsList)
            {
                string currentLine = Find(item, MainWindow.pathDictionary[item[0].ToString()]);
                if (currentLine == null) continue;
                words.Add(currentLine);
            }
            words.Sort(new CustomComparer());
            for (global::System.Int32 i = 0; i < words.Count; i++)
            {
                words[i] = words[i].Split(' ')[0];
                Console.WriteLine(words[i]);
            }
            return words;
        }

        public static void AddRalatedWord(string word, string ralatedWord)
        {
            List<string> ralatedWords = GetRalatedWords(word) != null ? GetRalatedWords(word) : new List<string>();
            List<string> ralatedWordsLines = new List<string>();
            if (ralatedWords.Contains(ralatedWord)) return;
            ralatedWords.Add(ralatedWord);
            if (ralatedWords.Count > 0)
            {
                for (global::System.Int32 i = 0; i < ralatedWords.Count; i++)
                    ralatedWordsLines.Add(Find(ralatedWords[i], MainWindow.pathDictionary[ralatedWords[i][0].ToString().ToLower()]));
            }
            if (ralatedWords.Count > 9)
            {
                ralatedWordsLines.Sort(new CustomComparer());
                ralatedWordsLines.Remove(ralatedWords[ralatedWords.Count - 1]);
            }

            for (int i = 0; i < ralatedWords.Count; i++)
            {
                if (i > 8)
                {
                    ralatedWords.Remove(ralatedWords[i]);
                    continue;
                }
                ralatedWords[i] = ralatedWordsLines[i].Split(' ')[0];
            }

            string fileName = MainWindow.pathDictionary[word[0].ToString().ToLower()];
            string line = Find(word, fileName);
            string newRalatedLinesString = String.Join(",", ralatedWords);
            Console.WriteLine("new^ " + newRalatedLinesString);
            List<string> lines = Read("\\" + fileName);
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == line)
                {
                    lines[i] = lines[i].Replace(lines[i].Substring(lines[i].IndexOf('{')), "{" + newRalatedLinesString + "}");
                    Console.WriteLine(lines[i]);
                    break;
                }
            }

            StreamWriter writer = new StreamWriter(GetPath() + fileName);
            writer.Write(String.Join("\n", lines));
            writer.Close();
        }

        public static string Find(string searchWord, string path)
        {
            List<string> lines = Read(path);
            var line = lines.Where(word => word.StartsWith(searchWord + " ")).ToList();
            return line.Count > 0 ? line[0] : null;
        }

    }


    // Кастомный компаратор
    class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            // Разбиваем строки на части: слово и число
            string[] partsX = x.Split(' ');
            string[] partsY = y.Split(' ');

            // Извлекаем числа
            int numberX = int.Parse(partsX[1]);
            int numberY = int.Parse(partsY[1]);

            // Сначала сортируем по числу (по убыванию)
            int compareNumbers = numberY.CompareTo(numberX); // Сравниваем по убыванию

            if (compareNumbers != 0)
            {
                return compareNumbers; // Сортируем по числу
            }
            else
            {
                // Если числа равны, сравниваем по слову (по алфавиту)
                return partsX[0].CompareTo(partsY[0]); // Сортируем по слову
            }
        }
    }
}