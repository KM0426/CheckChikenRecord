using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Concurrent;
using System.Text;

namespace CheckChikenRecord
{
    class Program
    {
        static string rootDirectory = string.Empty;
        static ConcurrentBag<string[]> groupResults = new ConcurrentBag<string[]>();
        static string[] searchStrings = new[] { "@", "#", "▲", "治験" };
        static string[] excludeStrings = new[] { "NEUTRO#", "LYMPH#", "MONO#", "EO#", "BA#", "未登録薬外用" };

        [STAThread]
        static async Task Main(string[] args)
        {
            rootDirectory = GetFolder("MCDRsデータフォルダを選択してください");
            if (string.IsNullOrWhiteSpace(rootDirectory))
            {
                Console.WriteLine("MCDRsデータフォルダが選択されなかったため、終了します。");
                return;
            }

            var files = Directory.GetFiles(rootDirectory, "*.csv", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                Console.WriteLine("CSVファイルが見つかりませんでした。");
                return;
            }


            await Task.Run(() =>
            {
                Parallel.ForEach(files, file =>
                {
                    // file(フルパス)からファイル名を取得
                    var fileName = Path.GetFileName(file);
                    Console.WriteLine($"{fileName}を処理中...");
                    ProcessFile(file);
                    Console.WriteLine($"{fileName}の処理が完了しました。");
                });
            });

            Console.WriteLine("全処理が完了しました。");

            // 結果をCSV出力
            var outputPath = Path.Combine(rootDirectory, "解析結果.csv");
            File.WriteAllLines(outputPath, groupResults.Select(r => string.Join(",", r)), Encoding.UTF8);
            Console.WriteLine($"結果を出力しました: {outputPath}");
            Console.ReadLine();
        }

        static void ProcessFile(string filePath)
        {
            try
            {
                var encoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding("shift-jis") ?? Encoding.UTF8;
                var lines = File.ReadAllLines(filePath, encoding);

                foreach (var line in lines)
                {
                    if (searchStrings.AsParallel().Any(s => line.Contains(s)) &&
                        !excludeStrings.AsParallel().Any(e => line.Contains(e)))
                    {
                        var split = line.Split(',');
                        if (split.Length > 1)
                        {
                            groupResults.Add(new[] { split[1], line });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ファイル {filePath} の処理中にエラー: {ex.Message}");
            }
        }

        static string GetFolder(string title)
        {
            using var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = title
            };
            return dialog.ShowDialog() == CommonFileDialogResult.Ok ? dialog.FileName : string.Empty;
        }
    }
}
