using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simpson
{
    /// <summary>
    /// Логика взаимодействия для ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
        }

        public double MonteCarlo(Bitmap bitmap, double length)
        {
            Random rnd = new Random();
            Bitmap newBitmap = new Bitmap(bitmap);
            int range = 200000;
            int hit = 0;
            double area = length * length;
            Console.WriteLine("AREA: " + area);
            for (int i = 0; i < range; i++)
            {
                int x = rnd.Next(38, bitmap.Width - 8);
                int y = rnd.Next(38, bitmap.Height - 35);
                System.Drawing.Color col = bitmap.GetPixel(x, y);
                if (col.G >= 200 && col.R < 200)
                {
                    hit++;
                    newBitmap.SetPixel(x, y, System.Drawing.Color.Blue);
                }
                else if(col.B >= 200 && col.R < 200)
                {
                    hit--;
                    newBitmap.SetPixel(x, y, System.Drawing.Color.Red);
                }
                else 
                {
                   newBitmap.SetPixel(x, y, System.Drawing.Color.Black);
                }
            }

            using (MemoryStream memory = new MemoryStream())
            {
                newBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                Image.Source = bitmapimage;
            }
            Console.WriteLine("Покрытие: " + (double)hit / (double)range * 100 + "%");
            return area * hit / range;
        }
    }
}
