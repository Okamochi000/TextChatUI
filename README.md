# TextChatUI
【Unity】LINE風のテキストチャットUIサンプル


https://user-images.githubusercontent.com/49199105/116881451-3b7ec880-ac5e-11eb-80e4-e06c8eef56b3.mp4

(動画はiPhoneSEで撮影した画面です)

このプロジェクトは下記の機能を組み込んでいます。<br>
・セーフエリア調整<br>
・スクロールバーのフェード<br>
・入力欄をモバイルキーボードの高さに合わせる(iosのみ)<br>
・モバイルキーボードを出した状態でUnity上のUIを操作する(iosのみ)<br>

iosのテキスト入力欄にはネイティブで作成したUITextViewを使用しています。<br>
UITextViewは常時UnityのUIより前面に表示されるため、モバイルキーボードが表示されていない間は非表示にしてUnityのTextで表示しています。<br>
InutFieldを使用する場合はNativeTextField.IsNative()でfalseを返すように書き換えてください。<br>

Androidのモバイルキーバードの高さ取得処理には、こちらのリポジトリをを参考にさせていただきました。<br>
<a href="https://github.com/baba-s/UniSoftwareKeyboardArea">UniSoftwareKeyboardArea</a>
