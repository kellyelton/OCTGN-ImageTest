using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using ImageTest.Annotations;
using Microsoft.Win32;

namespace ImageTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _notRunningTests;

        public bool NotRunningTests
        {
            get { return _notRunningTests; }
            set
            {
                if (value.Equals(_notRunningTests)) return;
                _notRunningTests = value;
                OnPropertyChanged("NotRunningTests");
            }
        }

        public MainWindow()
        {
            NotRunningTests = true;
            Const.OnLogMessage += ConstOnOnLogMessage;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NotRunningTests = false;
            Task.Factory.StartNew(RunTests);
        }

        private void RunTests()
        {
            try
            {
                RunTest<Test1>();
                RunTest<Test2>();
                RunTest<Test3>();
            }
            catch (Exception e)
            {
                LogError("RunTests Error", e);
            }
            finally
            {
                NotRunningTests = true;
            }
        }

        private void RunTest<T>() where T : Test
        {
            try
            {
                var t1 = Activator.CreateInstance<T>();
                if (!TestWindow.RunTest(t1))
                {
                    LogError(t1.GetType().Name + " Failed", t1.Error);
                }
                else
                {
                    LogSuccess(t1.GetType().Name + " Succeeded");
                }
            }
            catch (Exception e)
            {
                LogError("RunTest Error", e);
            }
        }

        private void ConstOnOnLogMessage(string s)
        {
            LogEvent(s);
        }

        private void LogError(string blah, Exception e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => LogError(blah, e)));
                return;
            }

            var para = new Paragraph(new Bold(new Run(blah) { Foreground = Brushes.DarkRed }));
            var ex = e;
            while (ex != null)
            {
                para.Inlines.Add(Environment.NewLine);
                para.Inlines.Add(new Run("Message: " + ex.Message) {Foreground = Brushes.IndianRed});
                para.Inlines.Add(Environment.NewLine);
                para.Inlines.Add(new Run(ex.StackTrace) { Foreground = Brushes.IndianRed });
                ex = ex.InnerException;
            }
            LogText.Document.Blocks.Add(para);
        }

        private void LogSuccess(string blah)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => LogSuccess(blah)));
                return;
            }

            var para = new Paragraph(new Bold(new Run(blah) { Foreground = Brushes.ForestGreen}));
            LogText.Document.Blocks.Add(para);
        }

        private void LogEvent(string blah)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => LogEvent(blah)));
                return;
            }

            var para = new Paragraph(new Bold(new Run(blah) { Foreground = Brushes.DarkGray})){LineStackingStrategy = LineStackingStrategy.BlockLineHeight,Margin = new Thickness(0),Padding = new Thickness(0)};
            LogText.Document.Blocks.Add(para);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SaveLogClick(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = "RTF Log (*.Rtf) | *.Rtf" };
            if (sfd.ShowDialog().GetValueOrDefault(false))
            {
                var tr = new TextRange(LogText.Document.ContentStart, LogText.Document.ContentEnd);
                using (var stream = sfd.OpenFile())
                {
                    tr.Save(stream, DataFormats.Rtf);
                    stream.Flush();
                }
            }
        }
    }
}
