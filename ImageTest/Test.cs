using System;
using System.Windows.Media;

namespace ImageTest
{
    public abstract class Test
    {
        public bool Success { get; set; }
        public Exception Error { get; set; }
        public abstract ImageSource LoadImage();
    }
}