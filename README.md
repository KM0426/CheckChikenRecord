# 使用方法
1. dist.zipをダウンロード、解凍
2. 解凍したフォルダ内の「CheckChikenRecord.exe」を実行
3. フォルダ選択ダイアログでMCDRsから抽出したファイルを保存したフォルダを選択
5. 再度実行し完了まで放置（数時間かかります）
6. 解析結果.csvが出力されます
7. 解析結果.csvにIDがある場合は、MCDRsの抽出条件から削除します

# 検索基準
検索文字列(含む列データは解析結果にIDを追加)<br>
　static string[] searchStrings = new[] { "@", "#", "▲", "治験" };)<br>
除外文字列(ただし、下記の文字列を含む場合は除外))<br>
　static string[] excludeStrings = new[] { "NEUTRO#", "LYMPH#", "MONO#", "EO#", "BA#", "未登録薬外用" };)<br>
