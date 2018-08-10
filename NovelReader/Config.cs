using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media;
namespace 小说阅读器
{
    class Config : INotifyPropertyChanged
    {
        private int currentChapter;
        internal bool NightMode { get; set; }
        private double fontSize;
        private string fontName;
        private double maxDisplayWidth;
        private string backgroundColor;
        private int divideLimit;
        private double paraGap;
        private double lineHeight;
        private string rg;
        private int interval;
        private int waitBefore;
        private int waitAfter;
        private bool chapterDivide;
        private bool longDivide;
        internal string tempColor;
        private string fontColor;
        private string isItalic;
        private string isBold;
        internal string Shelfpath { get; set; }
        private Configuration config;
        private Book book;
        internal Timer timer;
        internal List<string> shelfBookFilePaths = new List<string>();
        internal ObservableCollection<BookMark> HistoryBooks { get; } = new ObservableCollection<BookMark>();
        public Config(Timer t, Book b)
        {
            fontSize = double.Parse(ConfigurationManager.AppSettings["fontsize"] ?? "14");
            maxDisplayWidth = int.Parse(ConfigurationManager.AppSettings["maxdisplaywidth"] ?? "900");
            divideLimit = int.Parse(ConfigurationManager.AppSettings["dividelimit"] ?? "12800");
            paraGap = double.Parse(ConfigurationManager.AppSettings["paragap"] ?? "15");
            lineHeight = double.Parse(ConfigurationManager.AppSettings["lineHeight"] ?? "20");
            interval = int.Parse(ConfigurationManager.AppSettings["interval"] ?? "1000");
            waitBefore = int.Parse(ConfigurationManager.AppSettings["waitbefore"] ?? "8");
            waitAfter = int.Parse(ConfigurationManager.AppSettings["waitafter"] ?? "20");
            backgroundColor = ConfigurationManager.AppSettings["backgroundcolor"] ?? "#cce8d0";
            fontName = ConfigurationManager.AppSettings["fontname"] ?? "Microsoft YaHei UI";
            rg = ConfigurationManager.AppSettings["rg"] ?? "\\b第([0-9]{1,4}|[一二三四五六七八九十两百千零○〇廿卅卌]{1,7})[章节回]( .*)?(?=\\r\\n)";
            chapterDivide = (ConfigurationManager.AppSettings["chapterdivide"] ?? "True") == "True";
            longDivide = (ConfigurationManager.AppSettings["longdivide"] ?? "True") == "True";
            fontColor = ConfigurationManager.AppSettings["fontcolor"] ?? "#000000";
            isItalic = ConfigurationManager.AppSettings["isitalic"] ?? "Normal";
            isBold = ConfigurationManager.AppSettings["isbold"] ?? "Normal";
            Shelfpath = ConfigurationManager.AppSettings["shelfpath"] ?? "";
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            timer = t;
            book = b;
        }
        public bool CanNextChapter
        {
            get
            {
                return currentChapter + 1 <= ChaptersCount;
            }
        }
        public bool CanPreChapter
        {
            get
            {
                return currentChapter > 0;
            }
        }
        public string CurrentTitle
        {
            get
            {
                if (ChaptersCount <= 0)
                {
                    return "我喜欢在泥坑里玩";
                }
                return book.titles[currentChapter];
            }
        }
        public bool CanOpenCatalog
        {
            get
            {
                return ChaptersCount > 0;
            }
        }
        public int ChaptersCount
        {
            get
            {
                return book.chaptersCount;
            }
        }
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }
        public string FontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = value;
                OnPropertyChanged("FontName");
            }
        }
        public double MaxDisplayWidth
        {
            get
            {
                return maxDisplayWidth;
            }
            set
            {
                maxDisplayWidth = value;
                OnPropertyChanged("MaxDisplayWidth");
            }
        }
        public string BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }
        public double LineHeight
        {
            get
            {
                return lineHeight;
            }
            set
            {
                lineHeight = value;
                OnPropertyChanged("LineHeight");
            }
        }
        public double ParaGap
        {
            get
            {
                return paraGap;
            }
            set
            {
                paraGap = value;
                OnPropertyChanged("ParaGap");
                OnPropertyChanged("ParaMargin");
            }
        }
        public string ParaMargin
        {
            get => "15," + paraGap;
        }
        public string IsBold
        {
            get
            {
                return isBold;
            }
            set
            {
                isBold = value;
                OnPropertyChanged("IsBold");
            }
        }
        public string IsItalic
        {
            get
            {
                return isItalic;
            }
            set
            {
                isItalic = value;
                OnPropertyChanged("IsItalic");
            }
        }
        public SolidColorBrush FontBrush
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb(Convert.ToByte(fontColor.Substring(1, 2), 16), Convert.ToByte(fontColor.Substring(3, 2), 16), Convert.ToByte(fontColor.Substring(5, 2), 16)));
            }
            set
            {
                fontColor = string.Format("#{0:X2}{1:X2}{2:X2}", value.Color.R, value.Color.G, value.Color.B);
                OnPropertyChanged("FontBrush");
            }
        }
        public int Interval
        {
            get
            {
                return interval;
            }
            set
            {
                interval = value;
                timer.Interval = interval;
                OnPropertyChanged("Interval");
            }
        }
        public int WaitBefore
        {
            get
            {
                return waitBefore * interval / 1000;
            }
            set
            {
                waitBefore = value * 1000 / interval;
                OnPropertyChanged("WaitBefore");
            }
        }
        public int WaitAfter
        {
            get
            {
                return waitAfter * interval / 1000;
            }
            set
            {
                waitAfter = value * 1000 / interval;
                OnPropertyChanged("WaitAfter");
            }
        }
        public bool ChapterDivide
        {
            get
            {
                return chapterDivide;
            }
            set
            {
                chapterDivide = value;
                OnPropertyChanged("ChapterDivide");
            }
        }
        public bool LongDivide
        {
            get
            {
                return longDivide;
            }
            set
            {
                longDivide = value;
                OnPropertyChanged("LongDivide");
            }
        }
        public int DivideLimit
        {
            get
            {
                return divideLimit;
            }
            set
            {
                divideLimit = value;
                OnPropertyChanged("DivideLimit");
            }
        }
        public string Rg
        {
            get
            {
                return rg;
            }
            set
            {
                rg = value;
                OnPropertyChanged("Rg");
            }
        }
        public SolidColorBrush LabelColor
        {
            get
            {
                if (NightMode)
                {
                    return Brushes.DimGray;
                }
                return Brushes.Black;
            }
        }
        public int CurrentChapter
        {
            get => currentChapter;
            set
            {
                currentChapter = value;
                OnPropertyChanged("CurrentChapter");
                OnPropertyChanged("CanPreChapter");
                OnPropertyChanged("CanNextChapter");
                OnPropertyChanged("CurrentTitle");
            }
        }
        public Book Book
        {
            get => book;
            set
            {
                book = value;
                OnPropertyChanged("CanOpenCatalog");
                OnPropertyChanged("ChaptersCount");
            }
        }
        public static double DoubleParse(string val)
        {
            if (double.TryParse(val, out double result))
            {
                return result;
            }
            System.Windows.MessageBox.Show(val + "让你瞎填 我也瞎给你填成233\n" + val);
            return 233;
        }
        public static int IntParse(string val)
        {
            if (int.TryParse(val, out int result))
            {
                return result;
            }
            System.Windows.MessageBox.Show(val + "让你瞎填 我也瞎给你填成233\n" + val);
            return 233;
        }
        public void BackToDefault()
        {
            FontSize = 14;
            FontName = "Microsoft YaHei UI";
            MaxDisplayWidth = 900f;
            BackgroundColor = "#cce8d0";
            ParaGap = 15f;
            LineHeight = 20f;
            Interval = 1000;
            WaitBefore = 8;
            WaitAfter = 20;
            if (!longDivide || !chapterDivide || rg != "\\b第([0-9]{1,4}|[一二三四五六七八九十两百千零○〇廿卅卌]{1,7})[章节回]( .*)?(?=\\r\\n)" || 12800 != divideLimit)
            {
                ChapterDivide = true;
                LongDivide = true;
                Rg = "\\b第([0-9]{1,4}|[一二三四五六七八九十两百千零○〇廿卅卌]{1,7})[章节回]( .*)?(?=\\r\\n)";
                DivideLimit = 12800;
            }
            IsItalic = "Normal";
            IsBold = "Normal";
            FontBrush = Brushes.Black;
        }
        public void Save()
        {
            if (config.HasFile)
            {
                if (NightMode)
                {
                    backgroundColor = tempColor;
                }
                if (Shelfpath != null)
                {
                    config.AppSettings.Settings["shelfpath"].Value = (Shelfpath ?? "");
                }
                config.AppSettings.Settings["fontname"].Value = fontName;
                config.AppSettings.Settings["fontsize"].Value = fontSize.ToString();
                config.AppSettings.Settings["backgroundcolor"].Value = backgroundColor;
                config.AppSettings.Settings["maxdisplaywidth"].Value = maxDisplayWidth.ToString();
                config.AppSettings.Settings["dividelimit"].Value = divideLimit.ToString();
                config.AppSettings.Settings["paragap"].Value = paraGap.ToString();
                config.AppSettings.Settings["lineHeight"].Value = lineHeight.ToString();
                config.AppSettings.Settings["rg"].Value = rg;
                config.AppSettings.Settings["interval"].Value = interval.ToString();
                config.AppSettings.Settings["waitbefore"].Value = waitBefore.ToString();
                config.AppSettings.Settings["waitafter"].Value = waitAfter.ToString();
                config.AppSettings.Settings["chapterdivide"].Value = chapterDivide.ToString();
                config.AppSettings.Settings["longdivide"].Value = longDivide.ToString();
                config.AppSettings.Settings["isitalic"].Value = isItalic;
                config.AppSettings.Settings["isbold"].Value = isBold;
                config.AppSettings.Settings["fontcolor"].Value = fontColor;
                config.Save();
            }
        }
        public void ReadHistory()
        {
            string[] all = config.AppSettings.Settings.AllKeys;
            foreach (var i in all)
            {
                if (i.StartsWith("history_"))
                {
                    var m = Regex.Match(config.AppSettings.Settings[i].Value, @"(.+\.txt) ([0-9]+) ([0-9\.]+) (.+)");
                    HistoryBooks.Add(new BookMark(m.Groups[1].Value, IntParse(m.Groups[2].Value), DoubleParse(m.Groups[3].Value), m.Groups[4].Value).Notice());
                    config.AppSettings.Settings.Remove(i);
                }
            }
            config.Save();
        }
        public void WriteHistory(double currentOffSet)
        {
            if (book.filePath != "")
            {
                HistoryBooks.Add(new BookMark(book.filePath, currentChapter, currentOffSet, DateTime.Now.ToString("f")).Notice());
            }
            for (int i = 0; i < HistoryBooks.Count; i++)
            {
                config.AppSettings.Settings.Add("history_" + i.ToString(), string.Format("{0} {1} {2} {3}", HistoryBooks[i].BookPath, HistoryBooks[i].CurrentChapter, HistoryBooks[i].CurrentOffSet, HistoryBooks[i].SaveDate));
            }
            config.Save();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
