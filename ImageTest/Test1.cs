using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageTest
{
    public class Test1 : Test
    {

        public override System.Windows.Media.ImageSource LoadImage()
        {
            return new BitmapImage(new Uri(Const.Url));
            //return new BitmapImage(new Uri("http://images6.fanpop.com/image/photos/33100000/-funny-man-and-Maria-maria-sharapova-33189378-1618-1024.jpg"));
        }
    }

    public class Test2 : Test
    {
        public override ImageSource LoadImage()
        {
            return new BitmapImage(new Uri("Spoils_logo_trans.png",UriKind.Relative));
        }
    }

    public class Test3 : Test
    {
        public override ImageSource LoadImage()
        {
            var url = Path.Combine(Environment.CurrentDirectory, "Spoils_logo_trans2.png");
            return new BitmapImage(new Uri(url));
        }
    }
}