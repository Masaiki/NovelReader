using System.ComponentModel;
namespace 小说阅读器
{
    class BookMark : INotifyPropertyChanged
    {
        public string BookPath { get; }
        public int CurrentChapter { get; }
        public double CurrentOffSet { get; }//当前位置/总长而得的百分比
        public string SaveDate { get; }
        public BookMark(string bookPath, int currentChapter, double currentOffSet, string saveDate)
        {
            BookPath = bookPath;
            SaveDate = saveDate;
            CurrentChapter = currentChapter;
            CurrentOffSet = currentOffSet;
        }
        public BookMark Notice()
        {
            OnPropertyChanged("BookPath");
            OnPropertyChanged("SaveDate");
            OnPropertyChanged("CurrentChapter");
            OnPropertyChanged("CurrentOffSet");
            return this;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
