using System;
using System.Collections.Generic;
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
using static System.Net.Mime.MediaTypeNames;

namespace ToDoApp2
{
    /// <summary>
    /// ToDoInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ToDoInputWindow : Window
    {
        public ToDoInputWindow()
        {
            InitializeComponent();

            this.Cancel.Click += (s, e) =>
            {
                this.Close();
            };

            this.ImageChoose.Click += (s, e) =>
            {

            };
        }

        private void image_Drop(object sender, DragEventArgs e)
        {
            var fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (fileNames is null) return;                    // nullなら返す

            var fileName = fileNames[0];                      // 画像ファイルのパスを取得
            var ext = System.IO.Path.GetExtension(fileName).ToLower();  // 拡張子の確認

            // ファイルの拡張子が対応しているか確認する
            if (ext != ".bmp" && ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".tiff" && ext != ".gif" && ext != ".icon")
            {
                MessageBox.Show("画像ファイルを選択してください。", "お知らせ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            BitmapImage bmpImage = new BitmapImage();
            using (FileStream stream = File.OpenRead(fileName))
            {
                bmpImage.BeginInit();
                bmpImage.StreamSource = stream;
                bmpImage.DecodePixelWidth = 500;
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.EndInit();
            }

            SelectedImage.Source = bmpImage;
        }
    }
}
