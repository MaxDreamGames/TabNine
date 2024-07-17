using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

namespace TabNine
{
    public partial class MainWindow : Form
    {
        //public List<string> dictionary;
        string[] letters = new string[]
{
    "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l",
    "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
    "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к",
    "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц",
    "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я"
};
        string[] punctuationMarks = new string[] {
            ".", ",", ";", ":", "!", "?", "(", ")", "[", "]", "{", "}", "...", "/", "@", "#", "&", "^", "%", "$", "№", "_", "=", "+"
        };
        string[] punctuationMarksForSpace = new string[] {
            ".", ",", ";", ":", "!", "?", ")", "]", "}"
        };

        string[] endPunctuationMarks = new string[] {
            ".", "!", "?", "..."
         };

        public static Dictionary<string, List<string>> dictionary;
        public static Dictionary<string, string> pathDictionary;
        string query;
        string perviousQuery;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            StartingInit();
            SetAutoRunValue(true, Assembly.GetExecutingAssembly().Location);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FillSuggestionList();
        }


        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        Console.WriteLine(query);
                        EnterWord();
                        break;
                    case Keys.Down:
                        if (listBox.SelectedIndex < listBox.Items.Count - 1)
                            listBox.SelectedItem = listBox.Items[listBox.SelectedIndex + 1];
                        else
                            listBox.SelectedItem = listBox.Items[0];
                        break;
                    case Keys.Up:
                        if (listBox.SelectedIndex > 0)
                            listBox.SelectedItem = listBox.Items[listBox.SelectedIndex - 1];
                        else
                            listBox.SelectedItem = listBox.Items[listBox.Items.Count - 1];
                        textBox1.SelectionStart = textBox1.TextLength;
                        textBox1.SelectionLength = textBox1.TextLength;
                        break;
                    default:
                        break;
                }

            }
            if (this.ActiveControl == sender && e.KeyData == Keys.Space)
            {

            }
        }
        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (this.ActiveControl == sender && listBox.Visible)
            {
                switch (e.KeyData)
                {
                    case Keys.Tab:
                        textBox1.Text += listBox.Text.Substring(query.Length) + " ";
                        textBox1.SelectionStart = textBox1.TextLength;
                        textBox1.SelectionLength = textBox1.TextLength;
                        break;
                    case Keys.Space:
                        EnterWord();
                        textBox1.Text = textBox1.Text.Remove(textBox1.TextLength - 1);
                        textBox1.SelectionStart = textBox1.TextLength;
                        textBox1.SelectionLength = textBox1.TextLength;
                        Console.WriteLine(listBox.Items[0].ToString());
                        break;
                    default:
                        break;
                }

            }

            if (this.ActiveControl == sender && e.KeyData == Keys.PageUp)
            {
                textBox1.SelectAll();
                textBox1.Copy();
                textBox1.Text = "";
            }
            if (this.ActiveControl == sender && e.KeyData == Keys.PageDown)
            {
                textBox1.Select();
                textBox1.Paste();
            }
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = e.KeyChar == (char)Keys.Enter;
        }

        private void listBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox.Visible && listBox.SelectedItem != null)
                EnterWord();
        }
        private void copyBtn_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
            textBox1.Copy();
            textBox1.Select();
        }
        private void pasteBtn_Click(object sender, EventArgs e)
        {
            textBox1.Select();
            textBox1.Paste();

        }
        private void removeBtn_Click(object sender, EventArgs e)
        {
            FileController.RemoveLine(listBox.Items[listBox.SelectedIndex].ToString().ToLower());
            RefreshDictionary(query[0].ToString().ToLower());
            FillSuggestionList();
        }

        void StartingInit()
        {

            dictionary = new Dictionary<string, List<string>>();
            dictionary["Useful"] = new List<string>();
            pathDictionary = new Dictionary<string, string>();
            for (int i = 0; i < letters.Length; i++)
            {
                var wordList = FileController.Read(letters[i].ToUpper() + ".txt");
                wordList.Sort(new CustomComparer());
                for (global::System.Int32 j = 0; j < wordList.Count; j++)
                {
                    if (Convert.ToInt32(wordList[j].Split()[1]) > 0)
                        dictionary["Useful"].Add(wordList[j].Split()[0]);
                    wordList[j] = wordList[j].Split(' ')[0];
                }
                dictionary[letters[i]] = wordList;
                pathDictionary[letters[i]] = (letters[i].ToUpper() + ".txt");
            }
            Console.WriteLine("Loading done!");

            listBox.Visible = false;
        }

        void FillSuggestionList()
        {
            string[] words = textBox1.Text.Split(' ');
            query = words[words.Length - 1];
            if (textBox1.TextLength == 1 && textBox1.Text != " ")
            {
                query = query.Replace(query[0].ToString(), query[0].ToString().ToUpper());
                textBox1.Text = textBox1.Text.Replace(textBox1.Text[textBox1.TextLength - 1].ToString(), query);
                textBox1.SelectionStart = textBox1.TextLength;
                textBox1.SelectionLength = textBox1.TextLength;

            }
            foreach (var item in punctuationMarksForSpace)
            {
                if (query == item && words.Length > 1)
                {
                    if (textBox1.Text.Contains(" " + item))
                    {

                        textBox1.Text = textBox1.Text.Replace(" " + item, item + " ");
                        textBox1.SelectionStart = textBox1.TextLength;
                        textBox1.SelectionLength = textBox1.TextLength;
                        return;
                    }
                }
            }
            foreach (var item in endPunctuationMarks)
            {
                if (words.Length > 1)
                {
                    if (words[words.Length - 2].Contains(item) && query.Length == 1 && letters.ToList().Contains(query[0].ToString()))
                    {
                        query = query.ToUpper();
                        textBox1.Text = textBox1.Text.Remove(textBox1.TextLength - 1) + query;
                        textBox1.SelectionStart = textBox1.TextLength;
                        textBox1.SelectionLength = textBox1.TextLength;

                        return;
                    }
                }
            }

            if (words.Length > 1)
                perviousQuery = words[words.Length - 2];
            if (string.IsNullOrEmpty(query))
            {
                if (dictionary["Useful"] != null && !string.IsNullOrEmpty(perviousQuery))
                    Console.WriteLine("G");
                else
                    listBox.Visible = false;
                return;

            }
            else if (string.IsNullOrEmpty(query))
            {
                return;
            }
            if ("1234567890".Contains(query[0]))
            {
                listBox.Visible = false;
                return;
            }

            foreach (var item in punctuationMarks)
            {
                if (query.Contains(item))
                {
                    query = query.Remove(query.IndexOf(item), item.Length);

                }
            }
            if (punctuationMarks.Contains(textBox1.Text[textBox1.TextLength - 1].ToString())) return;
            if (letters.Contains(query[0].ToString().ToLower()))
            {
                List<string> suggestions = new List<string>();
                /*if (dictionary["Useful"].Count > 0)
                {
                    suggestions.AddRange(dictionary["Useful"].Where(word => word.StartsWith(query.ToLower())).ToList());
                }*/

                suggestions.AddRange(dictionary[query[0].ToString().ToLower()].Where(word => word.StartsWith(query.ToLower())).ToList());
                if (Char.IsUpper(query[0]) && suggestions.Count > 0)
                {
                    for (global::System.Int32 i = 0; i < suggestions.Count; i++)
                    {
                        string newLine = suggestions[i][0].ToString().ToUpper();
                        if (suggestions[i].Length > 1)
                            newLine += suggestions[i].Substring(1);
                        suggestions[i] = newLine;
                    }
                }
                suggestions.Add(query);
                //Console.WriteLine("1");
                //}

                if (suggestions.Count > 0)
                {
                    listBox.DataSource = suggestions;
                    listBox.Visible = true;
                }
                else
                {
                    listBox.Visible = false;
                }
            }
        }

        void RefreshDictionary(string letter)
        {
            var wordList = FileController.Read(pathDictionary[letter]);
            for (int i = 0; i < wordList.Count; i++) wordList[i] = wordList[i].Split(' ')[0];
            dictionary[letter] = wordList;
        }

        void EnterWord(bool isExact = false)
        {
            textBox1.Text += listBox.Text.Substring(query.Length) + " ";
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.SelectionLength = textBox1.TextLength;
            string[] words = textBox1.Text.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                foreach (string mark in punctuationMarks)
                {
                    if (words[i].Contains(mark))
                        words[i] = words[i].Replace(mark, "");
                }
                Console.WriteLine(words[i]);
            }
            foreach (var item in punctuationMarks)
            {
                if (perviousQuery.Contains(item))
                    perviousQuery = perviousQuery.Replace(item, "");
            }
            Console.WriteLine(perviousQuery.ToLower());
            if (listBox.SelectedItem == listBox.Items[listBox.Items.Count - 1])
            {
                FileController.AddWord(perviousQuery.ToLower());
                RefreshDictionary(perviousQuery[0].ToString().ToLower());
            }
            else
            {
                FileController.Rewriteline(perviousQuery.ToLower(), 1);
                RefreshDictionary(perviousQuery[0].ToString().ToLower());
            }
            if (words.Length >= 3)
            {
                if (!("1234567890".Contains(words[words.Length - 3]) || "1234567890".Contains(words[words.Length - 3])))
                {
                    FileController.AddRalatedWord(words[words.Length - 3].ToLower(), words[words.Length - 2].ToLower());

                }
            }
            List<string> ralatedWords = FileController.GetRalatedWords(perviousQuery.ToLower());
            if (ralatedWords == null)
            {
                dictionary["Useful"] = null;
                listBox.Visible = false;
                return;
            }

            dictionary["Useful"] = ralatedWords;

            listBox.DataSource = dictionary["Useful"];
            listBox.Visible = true;
        }

        private void addExactBtn_Click(object sender, EventArgs e)
        {

        }

        bool SetAutoRunValue(bool autoRun, string path)
        {
            const string name = "TabNine";
            string exePath = path;
            RegistryKey reg;

            reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            try
            {
                if (autoRun)
                    reg.SetValue(name, exePath);
                else
                    reg.DeleteValue(name);

                reg.Flush();
                reg.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}
