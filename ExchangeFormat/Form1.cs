using System;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using static System.Console;

namespace ExchangeFormat
{
    public partial class Form1 : Form
    {
        // Grep 対象ファイル名格納
        private string[] files;

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     コンストラクタ </summary>
        /// <returns>
        ///     Nothing. </returns>
        /// -----------------------------------------------------------------------------------
        public Form1()
        {
            InitializeComponent();
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     Exchangeボタンが押下された時実行します。 </summary>
        /// <returns>
        ///     Nothing. </returns>
        /// -----------------------------------------------------------------------------------
        private void ExchangeButton_Click(object sender, EventArgs e)
        {
            searchTargetFiles( );
            return;
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     対象ファイルを検索します。 </summary>
        /// <returns>
        ///     Nothing. </returns>
        /// -----------------------------------------------------------------------------------
        private void searchTargetFiles()
        {
            try
            {
                var grepPath = Grep.Text;

                // ファイル名検索の実行
                files = GrepFiles(@"C:\Users\Asterisk\Desktop\test\", "aa", "*.jpg-large", false);

                // 結果の表示
                foreach (string f in files)
                {
                    WriteLine(f);
                    // 拡張子を".jpg"に変更する
                    // 2番目の引数は、"jpg"でも".jpg"でもどちらでも..
                    string newName = Path.ChangeExtension(f, NewFormat.Text);

                    // 実際にファイル名を変更する
                    // fileNameがない場合や、newFileNameが存在する場合は例外がスローされる
                    File.Move(f, newName);

                    #region 別バージョン(却下)
                    /*
                        var targetStr = "";
                        var targetPos = 0;
                        var targetFolder = Path.GetFileName(Path.GetDirectoryName(grepPath));

                        targetPos = f.LastIndexOf(@"\");
                        targetStr = VBRight(f, targetPos - targetFolder.Length - 1);

                        // デバッグ表示
                        WriteLine(targetStr);

                        // ファイル名の変更(コピーで対応)
                         File.Copy(oldName, newName, true);
                         File.Delete(oldName);
                     */
                    #endregion
                }

                MessageBox.Show("変換が完了しました。", "変換完了", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch(Exception e)
            {
                MessageBox.Show("処理対象が存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        #region　Left メソッド
        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の左端から指定された文字数分の文字列を返します。</summary>
        /// <param name="target">
        ///     取り出す元になる文字列。</param>
        /// <param name="len">
        ///     取り出す文字数。</param>
        /// <returns>
        ///     左端から指定された文字数分の文字列。
        ///     文字数を超えた場合は、文字列全体が返されます。</returns>
        /// -----------------------------------------------------------------------------------
        public static string VBLeft(string target, int len)
        {
            if (len <= target.Length)
                return target.Substring(0, len);

            return target;
        }

        #endregion

        #region　Mid メソッド
        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の指定された位置以降のすべての文字列を返します。</summary>
        /// <param name="target">
        ///     取り出す元になる文字列。</param>
        /// <param name="start">
        ///     取り出しを開始する位置。</param>
        /// <returns>
        ///     指定された位置以降のすべての文字列。</returns>
        /// -----------------------------------------------------------------------------------
        public static string VBMid(string target, int start)
        {
            if (start <= target.Length)
                return target.Substring(start - 1);

            return string.Empty;
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の指定された位置から、指定された文字数分の文字列を返します。</summary>
        /// <param name="target">
        ///     取り出す元になる文字列。</param>
        /// <param name="start">
        ///     取り出しを開始する位置。</param>
        /// <param name="len">
        ///     取り出す文字数。</param>
        /// <returns>
        ///     指定された位置から指定された文字数分の文字列。
        ///     文字数を超えた場合は、指定された位置からすべての文字列が返されます。</returns>
        /// -----------------------------------------------------------------------------------
        public static string VBMid(string target, int start, int len)
        {
            if (start <= target.Length)
            {
                if (start + len - 1 <= target.Length)
                    return target.Substring(start - 1, len);

                return target.Substring(start - 1);
            }

            return string.Empty;
        }

        #endregion

        #region　Right メソッド
        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     文字列の右端から指定された文字数分の文字列を返します。</summary>
        /// <param name="target">
        ///     取り出す元になる文字列。</param>
        /// <param name="len">
        ///     取り出す文字数。</param>
        /// <returns>
        ///     右端から指定された文字数分の文字列。
        ///     文字数を超えた場合は、文字列全体が返されます。</returns>
        /// -----------------------------------------------------------------------------------
        public static string VBRight(string target, int len)
        {
            if (len <= target.Length)
                return target.Substring(target.Length - len);

            return target;
        }

        #endregion

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     ファイルの検索を行う </summary>
        /// <param name="dirPath">
        ///     フォルダのパス。</param>
        /// <param name="pattern">
        ///     検索する正規表現のパターン。</param>
        /// <param name="fileWildcards">
        ///     対象とするファイル。</param>
        /// <param name="ignoreCase">
        ///     大文字小文字を区別するか。</param>
        /// <returns>
        ///     見つかったファイルパスの配列。</returns>
        /// -----------------------------------------------------------------------------------
        public static string[] GrepFiles(
            string dirPath, string pattern, string fileWildcards,  bool ignoreCase )
        {
            ArrayList fileCol = new ArrayList();

            // 正規表現のオプションを設定
            RegexOptions opts = RegexOptions.None;

            if (ignoreCase)
                opts |= RegexOptions.IgnoreCase;

            Regex reg = new Regex(pattern, opts);

            // フォルダ内にあるファイルを取得
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            FileInfo[] files  = dir.GetFiles(fileWildcards);

            foreach (FileInfo f in files)
            {
                // 正規表現のパターンを使用して一つずつファイルを調べる
                if (ContainTextInFile(f.FullName, reg))
                    fileCol.Add(f.FullName);
            }

            return (string[])fileCol.ToArray(typeof(string));
        }

        /// -----------------------------------------------------------------------------------
        /// <summary>
        ///     ファイルの内容がpatternに一致するか調べる  </summary>
        /// <param name="filePath">
        ///     検索対象のファイルパス　</param>
        /// <param name="reg">
        ///     正規表現オブジェクト　</param>
        /// <returns>
        ///     一致したファイル名（フルパス）　</returns>
        /// -----------------------------------------------------------------------------------
        private static bool ContainTextInFile(string filePath, Regex reg)
        {
            //ファイルを読み込む
            using (var strm = new StreamReader(filePath, System.Text.Encoding.Default, true))
            {
                string txt = "";
                try
                {
                    txt = strm.ReadToEnd();
                }
                catch(Exception e)
                {
                    WriteLine(e.StackTrace);
                }
                finally
                {
                    strm.Close();
                }
                return reg.IsMatch(txt);
            }
        }
    }
}
