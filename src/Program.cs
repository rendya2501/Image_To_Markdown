using System.IO;
using System.Text;

// カレントディレクトリを取得
string currentDirectory = Directory.GetCurrentDirectory();

// カレントディレクトリ内の画像ファイルを取得（jpg, png, gifなどの形式）
var imageFiles = Directory
    .GetFiles(currentDirectory, "*.*")
    .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
    .ToList();

// 画像ファイルを最終更新日でソート
//var sortedFiles = imageFiles.OrderBy(file => File.GetCreationTime(file)).ToList();
// ファイル名でソート
var sortedFiles = imageFiles.OrderBy(file => Path.GetFileName(file)).ToList();

StringBuilder sb = new();
// ソートされたファイル名を表示
foreach (var file in sortedFiles)
{
    sb.Append($"![](./{file.Split("\\").LastOrDefault()}){Environment.NewLine}");
}

using (var fileStream = File.OpenWrite($"{currentDirectory}/00_index.md"))
{
    using var streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false));
    await streamWriter.WriteAsync(sb.ToString());
}
