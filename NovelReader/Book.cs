using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace 小说阅读器
{
    class Book
    {
        public List<string> titles = new List<string>();
        public List<string> chapters = new List<string>();
        public string filePath;
        public int chaptersCount;
        private Config config;
        public Book(string filepath, Config c)
        {
            filePath = filepath;
            config = c;
            chaptersCount = 0;
        }
        public void OpenText()
        {
            var text = File.ReadAllText(filePath, Encoding.Default);
            string title = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            MatchCollection matchCollection = new Regex(config.Rg, RegexOptions.Multiline | RegexOptions.Compiled).Matches(text);
            if (matchCollection.Count > 0 && config.ChapterDivide)
            {
                chaptersCount = matchCollection.Count;
                int num = chaptersCount - 1;
                ChapterGenerate(title, "\n" + text.Substring(0, matchCollection[0].Index));
                for (int i = 0; i < num; i++)
                {
                    int num2 = matchCollection[i].Index + matchCollection[i].Value.Length + 1;
                    ChapterGenerate(matchCollection[i].Value, text.Substring(num2, matchCollection[i + 1].Index - num2));
                }
                ChapterGenerate(matchCollection[num].Value, text.Substring(matchCollection[num].Index + matchCollection[num].Value.Length + 1));
            }
            else
            {
                ChapterGenerate(title, "\n" + text);
            }
            config.OnPropertyChanged("ChaptersCount");
        }
        private void ChapterGenerate(string title, string context)
        {
            if (context.Length > config.DivideLimit && config.LongDivide)
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                while (num + config.DivideLimit < context.Length)
                {
                    titles.Add(title + string.Format("—第{0:D}部分", num3 + 1));
                    int num4 = context.LastIndexOf("\r\n", num + config.DivideLimit, config.DivideLimit);
                    int num5 = (num4 == -1) ? config.DivideLimit : (num4 - num + 1);
                    chapters.Add("\n" + context.Substring(num, num5));
                    num += num5;
                    num2++;
                    num3++;
                }
                titles.Add(title + string.Format("—第{0:D}部分", ++num2));
                chapters.Add("\n" + context.Substring(num));
                chaptersCount += num2-1;
            }
            else
            {
                titles.Add(title);
                chapters.Add(context);
            }
        }
    }
}
