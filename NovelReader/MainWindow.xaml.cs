// 小说阅读器.MainWindow
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
namespace 小说阅读器
{
    public partial class MainWindow
    {
        internal Book bookReading = new Book("", null);
        private Timer timer = new Timer { };
        private int wait;
        private double preOffSet;
        internal Config config;
        public MainWindow()
        {
            InitializeComponent();
            config = new Config(timer, bookReading);
            DataContext = config;
            timer.Tick += AutoRead;
            timer.Interval = config.Interval;
            BookMark.ItemsSource = config.HistoryBooks;
            var pargs = Environment.GetCommandLineArgs();
            for (int j = 0; j < pargs.Length; j++)
            {
                if (pargs[j].EndsWith(".txt"))
                {
                    addIntoShelf(pargs[j], j);
                }
            }
            if (config.shelfBookFilePaths.Count == 1)
            {
                OpenText(config.shelfBookFilePaths[0]);
                config.shelfBookFilePaths.Clear();
                Shelf.Items.RemoveAt(0);
            }
            if (config.shelfBookFilePaths.Count == 0 && config.Shelfpath != "")
            {
                string[] files = Directory.GetFiles(config.Shelfpath, "*.txt");
                for (int j = 0; j < files.Length; j++)
                {
                    addIntoShelf(files[j], j);
                }
            }
            config.ReadHistory();
        }
        private void addIntoShelf(string path, int order)
        {
            System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem
            {
                Content = Path.GetFileNameWithoutExtension(path),
                Tag = order.ToString()
            };
            listViewItem.MouseDoubleClick += shelf_MouseDoubleClick;
            Shelf.Items.Add(listViewItem);
            config.shelfBookFilePaths.Add(path);
        }
        private void GoChapter(int toGo)
        {
            CatalogGrid.Visibility = Visibility.Collapsed;
            SettingGrid.Visibility = Visibility.Collapsed;
            if (toGo > -1 && toGo <= config.ChaptersCount)
            {
                config.CurrentChapter = toGo;
                Fuck.Document.Blocks.Clear();
                Fuck.AppendText(bookReading.titles[toGo]);
                if (bookReading.chapters[toGo].Length > config.DivideLimit && !config.ChapterDivide)
                {
                    Paragraph graph = new Paragraph();
                    NovelContent.Blocks.Add(graph);
                    Action action = new Action(() =>
                    {
                        ReadText(graph, bookReading.chapters[toGo]);
                    });
                    action.BeginInvoke(null, null);
                }
                else
                    Fuck.AppendText(bookReading.chapters[toGo]);
                Fuck.Selection.Select(Fuck.Document.ContentStart, Fuck.Document.ContentStart);
                Fuck.ScrollToHome();
                wait = -config.WaitBefore;
                preOffSet = 0.0;
                Fuck.Focus();
            }
            else
            {
                GoChapter(0);
            }
        }
        private static void ReadText(Paragraph graph, string text)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(text)))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Default))
                    {
                        while (!reader.EndOfStream)
                        {
                            var sb = new StringBuilder();
                            for (int i = 0; i < 1024 && !reader.EndOfStream; i++)
                            {
                                sb.AppendLine(reader.ReadLine());
                            }
                            if (!string.IsNullOrEmpty(sb.ToString()))
                            {
                                graph.Dispatcher.Invoke(() =>
                                {
                                    graph.Inlines.Add(sb.ToString());
                                });
                            }
                            System.Threading.Thread.Sleep(5);
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private void OpenText(string filepath)
        {
            if (filepath == "") return;
            if (bookReading.filePath != "")
                config.HistoryBooks.Add(new BookMark(bookReading.filePath, config.CurrentChapter, Fuck.VerticalOffset / (Fuck.ExtentHeight - Fuck.ViewportHeight), DateTime.Now.ToString("f")).Notice());
            Catalog.Items.Clear();
            bookReading = new Book(filepath, config);
            config.Book = bookReading;
            if (!config.ChapterDivide && !config.LongDivide)
            {
                var result = System.Windows.MessageBox.Show(this, "既不分章也不分段文件打开可能会很慢, 是否按字数分段。\n选否段间距功能会被献祭，用行间距凑活一下吧。", "警告", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    config.LongDivide = true;
                }
            }
            bookReading.OpenText();
            if (config.ChapterDivide || config.LongDivide)
            {
                for (int j = 0; j < bookReading.titles.Count; j++)
                {
                    System.Windows.Controls.ListViewItem listViewItem = new System.Windows.Controls.ListViewItem
                    {
                        Content = bookReading.titles[j],
                        Tag = j.ToString()
                    };
                    listViewItem.MouseDoubleClick += Gg_MouseDoubleClick;
                    Catalog.Items.Add(listViewItem);
                }
            }
            config.OnPropertyChanged("CanOpenCatalog");
            GC.Collect();
            GoChapter(0);
        }
        private string getAutoReadButtonName()
        {
            if (!timer.Enabled)
            {
                return "自动滚动";
            }
            if (wait < 0 && Fuck.VerticalOffset == 0.0)
            {
                return -wait / (1000.0 / config.Interval) + "秒后开始滚动";
            }
            if (wait > 0 && Math.Abs(Fuck.ExtentHeight - Fuck.VerticalOffset - Fuck.ViewportHeight) < 1E-05)
            {
                return (config.WaitAfter - wait) / (1000.0 / config.Interval) + "秒后进下章";
            }
            return "停止滚动";
        }
        private void AutoRead(object sender, EventArgs e)
        {
            if (Fuck.VerticalOffset == 0.0)
            {
                if (preOffSet != 0.0)
                {
                    wait = -config.WaitBefore;
                    preOffSet = 0.0;
                    AutoBtn.Content = getAutoReadButtonName();
                    return;
                }
                if (wait < 0)
                {
                    wait++;
                    AutoBtn.Content = getAutoReadButtonName();
                    return;
                }
            }
            if (wait < 0 && Fuck.VerticalOffset != 0.0)
            {
                goto IL_00ea;
            }
            if (wait > 0 && Math.Abs(Fuck.ExtentHeight - Fuck.VerticalOffset - Fuck.ViewportHeight) >= 1E-05)
            {
                goto IL_00ea;
            }
            goto IL_00fb;
            IL_00ea:
            AutoBtn.Content = getAutoReadButtonName();
            goto IL_00fb;
            IL_00fb:
            Fuck.ScrollToVerticalOffset(Fuck.VerticalOffset + 20.0);
            if (Math.Abs(Fuck.ExtentHeight - Fuck.VerticalOffset - Fuck.ViewportHeight) < 1E-05)
            {
                if (config.CurrentChapter == config.ChaptersCount)
                {
                    SwitchAuto();
                }
                wait = ((wait >= 0) ? (wait + 1) : 0);
                AutoBtn.Content = getAutoReadButtonName();
                if (wait >= config.WaitAfter)
                {
                    GoChapter(config.CurrentChapter + 1);
                    goto IL_01aa;
                }
                return;
            }
            goto IL_01aa;
            IL_01aa:
            preOffSet = Fuck.VerticalOffset;
        }
        private void SwitchAuto()
        {
            timer.Enabled = !timer.Enabled;
            AutoBtn.Content = getAutoReadButtonName();
            Fuck.Focus();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GoChapter(config.CurrentChapter - 1);
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            GoChapter(config.CurrentChapter + 1);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "文本文件|*.txt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                OpenText(filePath);
            }
        }
        private void ChapterList_Click(object sender, RoutedEventArgs e)
        {
            if (CatalogGrid.Visibility == Visibility.Collapsed)
            {
                CatalogGrid.Visibility = Visibility.Visible;
                Catalog.ScrollIntoView(Catalog.Items[config.CurrentChapter]);
                Catalog.SelectedItem = Catalog.Items[config.CurrentChapter];
            }
            else
            {
                CatalogGrid.Visibility = Visibility.Collapsed;
            }
            Fuck.Focus();
        }
        private void Gg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListViewItem listViewItem = sender as System.Windows.Controls.ListViewItem;
            GoChapter(Config.IntParse(listViewItem.Tag.ToString()));
        }
        private void shelf_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListViewItem listViewItem = sender as System.Windows.Controls.ListViewItem;
            OpenText(config.shelfBookFilePaths[Config.IntParse(listViewItem.Tag.ToString())]);
            MainTab.Focus();
        }
        private void bookMark_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var bookmark = (sender as System.Windows.Controls.ListViewItem).Content as BookMark;
            OpenText(bookmark.BookPath);
            GoChapter(bookmark.CurrentChapter);
            MainTab.Focus();
            if (config.LongDivide || config.ChapterDivide) Fuck.ScrollToVerticalOffset(bookmark.CurrentOffSet * (Fuck.ExtentHeight - Fuck.ViewportHeight));
        }
        private void Fuck_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!Fuck.IsFocused) return;
            if (e.Key == Key.Right)
            {
                if (config.CanNextChapter)
                {
                    GoChapter(config.CurrentChapter + 1);
                }
            }
            else if (e.Key == Key.Left)
            {
                if (config.CanPreChapter)
                {
                    GoChapter(config.CurrentChapter - 1);
                }
            }
            else if (e.Key == Key.Up)
            {
                Fuck.ScrollToVerticalOffset(Fuck.VerticalOffset - Fuck.ViewportHeight / 3.0);
            }
            else if (e.Key == Key.Down)
            {
                Fuck.ScrollToVerticalOffset(Fuck.VerticalOffset + Fuck.ViewportHeight / 3.0);
            }
            else if (e.Key == Key.Space)
            {
                SwitchAuto();
            }
        }
        private void SetFont_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog
            {
                ShowColor = true,
                Font = new System.Drawing.Font(config.FontName, (float)config.FontSize)
            };
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                config.FontName = (fontDialog.Font.Name ?? config.FontName);
                config.FontSize = fontDialog.Font.Size;
                System.Drawing.Color color = fontDialog.Color;
                config.FontBrush = new SolidColorBrush(Color.FromRgb(color.R, color.G, color.B));
                config.IsBold = (fontDialog.Font.Bold ? "Bold" : "Normal");
                config.IsItalic = (fontDialog.Font.Italic ? "Italic" : "Normal");
            }
        }
        private void SetBackgroud_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                FullOpen = true,
                Color = System.Drawing.Color.FromName(config.BackgroundColor)
            };
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                config.BackgroundColor = "#" + string.Format("{0:X8}", colorDialog.Color.ToArgb()).Substring(2);
            }
        }
        private void Fuck_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SwitchAuto();
        }
        private void ReadingArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double num = Math.Max((ReadingArea.ActualWidth - config.MaxDisplayWidth) / 2.0, 5.0);
            Fuck.Document.PagePadding = new Thickness(num, 0.0, num, 0.0);
            Catalog.Width = ActualWidth / 4.0;
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            if (SettingGrid.Visibility == Visibility.Collapsed)
            {
                SettingGrid.Visibility = Visibility.Visible;
            }
            else
            {
                SettingGrid.Visibility = Visibility.Collapsed;
            }
        }
        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            config.MaxDisplayWidth = Config.DoubleParse(MaxDisplayWidthSet.Text);
            config.LineHeight = Config.DoubleParse(LineHeightSet.Text);
            config.ParaGap = Config.DoubleParse(ParaGapSet.Text);
            double num = Math.Max((ReadingArea.ActualWidth - config.MaxDisplayWidth) / 2.0, 5.0);
            Fuck.Document.PagePadding = new Thickness(num, 0.0, num, 0.0);
            config.Interval = Config.IntParse(Gap.Text);
            config.WaitAfter = Config.IntParse(After.Text);
            config.WaitBefore = Config.IntParse(Before.Text);
            if (CutOff.IsChecked != config.LongDivide || DivideChapter.IsChecked != config.ChapterDivide || config.Rg != Principle.Text || LimitNumber.Text != config.DivideLimit.ToString())
            {
                config.LongDivide = (CutOff.IsChecked ?? true);
                config.ChapterDivide = (DivideChapter.IsChecked ?? true);
                config.Rg = Principle.Text;
                config.DivideLimit = Config.IntParse(LimitNumber.Text);
                OpenText(bookReading.filePath);
            }
            SettingGrid.Visibility = Visibility.Collapsed;
            Fuck.Focus();
        }
        private void ToNightMode_Click(object sender, RoutedEventArgs e)
        {
            if (!config.NightMode)
            {
                config.tempColor = config.BackgroundColor;
                MainGrid.Background = OtherGrid.Background = Catalog.Background = Shelf.Background = BookMark.Background = Brushes.Black;
                NovelContent.Foreground = Catalog.Foreground = Shelf.Foreground = BookMark.Foreground = Brushes.DimGray;
                config.BackgroundColor = "#323232";
            }
            else
            {
                config.BackgroundColor = config.tempColor;
                NovelContent.Foreground = config.FontBrush;
                Catalog.Background = MainGrid.Background = Shelf.Background = BookMark.Background = OtherGrid.Background = Brushes.White;
                Catalog.Foreground = Shelf.Foreground = BookMark.Foreground = Brushes.Black;
            }
            config.NightMode = !config.NightMode;
            config.OnPropertyChanged("LabelColor");
            Fuck.Focus();
        }
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            var flag = (!config.LongDivide || !config.ChapterDivide || config.Rg != "\\b第([0-9]{1,4}|[一二三四五六七八九十两百千零○〇廿卅卌]{1,7})[章节回]( .*)?(?=\\r\\n)" || 12800 != config.DivideLimit);
            config.BackToDefault();
            if (flag) OpenText(bookReading.filePath);
            SettingGrid.Visibility = Visibility.Collapsed;
            Fuck.Focus();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            SettingGrid.Visibility = Visibility.Collapsed;
            Fuck.Focus();
        }
        private void AutoBtn_Click(object sender, RoutedEventArgs e)
        {
            SwitchAuto();
        }
        private void Fuck_PreviewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = System.Windows.DragDropEffects.Copy;
            e.Handled = true;
        }
        private void Fuck_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            var list = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
            OpenText(list[list.Length - 1]);
        }
        private void AddBookMark_Click(object sender, RoutedEventArgs e)
        {
            if (bookReading.filePath != "")
                config.HistoryBooks.Add(new BookMark(bookReading.filePath, config.CurrentChapter, (Fuck.VerticalOffset / (Fuck.ExtentHeight - Fuck.ViewportHeight)), DateTime.Now.ToString("f")).Notice());
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (config.HistoryBooks.Count > 0 && bookReading.filePath == "")
            {
                var bookmark = config.HistoryBooks[config.HistoryBooks.Count - 1];
                OpenText(bookmark.BookPath);
                GoChapter(bookmark.CurrentChapter);
                MainTab.Focus();
                if (config.LongDivide || config.ChapterDivide) Fuck.ScrollToVerticalOffset(bookmark.CurrentOffSet * (Fuck.ExtentHeight - Fuck.ViewportHeight));
                config.HistoryBooks.Remove(bookmark);
            }
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            config.WriteHistory((Fuck.VerticalOffset / (Fuck.ExtentHeight - Fuck.ViewportHeight)));
            config.Save();
        }
        private void OpenShelf_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog
            {
                SelectedPath = config.Shelfpath
            };
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                config.Shelfpath = openFolderDialog.SelectedPath;
                config.shelfBookFilePaths.Clear();
                Shelf.Items.Clear();
                string[] files = Directory.GetFiles(config.Shelfpath, "*.txt");
                for (int j = 0; j < files.Length; j++)
                {
                    addIntoShelf(files[j], j);
                }
            }
        }
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            List<int> temp = new List<int>();
            foreach (var i in Shelf.SelectedItems)
            {
                temp.Add(Shelf.Items.IndexOf(i));
            }
            foreach (var i in temp)
            {
                Shelf.Items.RemoveAt(i);
                config.shelfBookFilePaths.RemoveAt(i);
            }
        }
        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            config.shelfBookFilePaths.Clear();
            Shelf.Items.Clear();
        }
        private void HisRemove_Click(object sender, RoutedEventArgs e)
        {
            List<int> temp = new List<int>();
            foreach (BookMark i in BookMark.SelectedItems)
            {
                temp.Add(config.HistoryBooks.IndexOf(i));
            }
            foreach (var i in temp)
            {
                config.HistoryBooks.RemoveAt(i);
            }
        }
        private void HisRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            config.HistoryBooks.Clear();
        }
    }
}
