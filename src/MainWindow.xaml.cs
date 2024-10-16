using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace Image_To_Markdown
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 画像入力ディレクトリダイアログを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputDirectoryDialogButton_Click(object sender, RoutedEventArgs e)
        {
            using CommonOpenFileDialog cofd = new()
            {
                // フォルダを選択できるようにする
                IsFolderPicker = true
            };
            if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.InputDirectoryPath.Text = cofd.FileName;
                this.OutputDirectoryPath.Text = @$"{cofd.FileName}\00_index.md";
            }
        }


        private async void Execute_Click(object sender, RoutedEventArgs e)
        {
            string inputFolderPath = this.InputDirectoryPath.Text;
            string outputFolderPath = this.OutputDirectoryPath.Text;
            // カレントディレクトリ内の画像ファイルを取得（jpg, png, gifなどの形式）
            var imageFiles = Directory
                .GetFiles(inputFolderPath, "*.*")
                .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (imageFiles.Count == 0)
            {
                MessageBox.Show("指定したフォルダに画像がありません。");
                return;
            }
            else if (string.IsNullOrEmpty(outputFolderPath))
            {
                MessageBox.Show("出力先が指定されていません。");
                return;
            }
            else if (!outputFolderPath.EndsWith(".md"))
            {
                MessageBox.Show("拡張子は「.md」としてください。");
                return;
            }

            // ファイル名でソート
            var sortedFiles = imageFiles.OrderBy(file => Path.GetFileName(file)).ToList();

            StringBuilder sb = new();
            // ソートされたファイル名を表示
            foreach (var file in sortedFiles)
            {
                sb.Append($"![](./{file.Split("\\").LastOrDefault()}){Environment.NewLine}");
            }


            // クルクルを表示
            this.LoadingOverlay.Visibility = Visibility.Visible;
            this.Whole.IsEnabled = false;

            using var fileStream = File.OpenWrite(outputFolderPath);
            using var streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false));
            await streamWriter.WriteAsync(sb.ToString());

            // 処理完了後、クルクルを非表示
            this.LoadingOverlay.Visibility = Visibility.Collapsed;
            this.Whole.IsEnabled = true;
            MessageBox.Show("処理終了");
        }
    }
}