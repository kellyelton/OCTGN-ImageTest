using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageTest.Annotations;

namespace ImageTest
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : INotifyPropertyChanged
    {
        private bool _enableButtons;

        public static bool RunTest(Test test)
        {
            Const.Log("Running {0}", test.GetType().Name);
            ImageSource source = null;
            bool? earlyRet = null;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    Const.Log("Loading Image Source");
                    source = test.LoadImage();
                    Const.Log("Loaded Image Source");
                }
                catch (Exception e)
                {
                    test.Error = new Exception("Error loading image for test " + test.GetType().Name, e);
                    earlyRet = false;
                }
            }));

            if (earlyRet != null)
                return earlyRet.Value;
            TestWindow win = null;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    Const.Log("Creating Window");
                    win = new TestWindow(source, test.GetType().Name);
                    Const.Log("Created Window");
                }
                catch (Exception e)
                {
                    test.Error = new Exception("Error initializing window for test " + test.GetType().Name, e);
                    earlyRet = false;
                }
            }));
            if (earlyRet != null)
                return earlyRet.Value;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    Const.Log("Showing Window");
                    win.Show();
                    Const.Log("Window Shown");
                }
                catch (Exception e)
                {
                    test.Error = new Exception("Error showing window for test " + test.GetType().Name, e);
                    earlyRet = false;
                }
            }));
            if (earlyRet != null)
                return earlyRet.Value;

            Const.Log("Waiting for window to close...");
            while (!win.IsClosed)
            {
                Thread.Sleep(1);
            }
            Const.Log("Window closed Success={0}",win.Success);
            return win.Success;
        }

        public bool EnableButtons
        {
            get { return _enableButtons; }
            set
            {
                if (value.Equals(_enableButtons)) return;
                _enableButtons = value;
                OnPropertyChanged("EnableButtons");
            }
        }

        public bool Success { get; set; }
        public ImageSource ImageSource { get; set; }
        public bool IsClosed { get; set; }
        public TestWindow(ImageSource source, string title)
        {
            this.Closed += OnClosed;
            EnableButtons = false;
            if (ImageSource is BitmapImage)
            {
                Const.Log("Creating Image Download Events");
                (ImageSource as BitmapImage).DownloadCompleted += OnDownloadCompleted;
                (ImageSource as BitmapImage).DownloadFailed += OnDownloadFailed;
            }
            else
            {
                EnableButtons = true;
            }
            ImageSource = source;
            Const.Log("InitializingComponent");
            InitializeComponent();
            Title = title;

        }

        private void OnDownloadFailed(object sender, ExceptionEventArgs exceptionEventArgs)
        {
            Const.Log("OnDownloadFailed");
            throw exceptionEventArgs.ErrorException;
        }

        private void OnDownloadCompleted(object sender, EventArgs eventArgs)
        {
            Const.Log("Image download complete");
            EnableButtons = true;
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            Const.Log("Window OnClosed");
            IsClosed = true;
            this.Closed -= OnClosed;
            if (ImageSource is BitmapImage)
            {
                (ImageSource as BitmapImage).DownloadCompleted -= OnDownloadCompleted;
                (ImageSource as BitmapImage).DownloadFailed -= OnDownloadFailed;
            }
        }

        private void Failure_Click(object sender, RoutedEventArgs e)
        {
            Success = false;
            this.Close();
        }

        private void Success_Click(object sender, RoutedEventArgs e)
        {
            Success = true;
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            EnableButtons = true;
            if (e.ErrorException != null)
            {
                throw e.ErrorException;
            }
        }
    }
}
