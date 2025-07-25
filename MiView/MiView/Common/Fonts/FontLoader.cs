using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace MiView.Common.Fonts
{
    internal class FontLoader
    {
        /// <summary>
        /// フォント格納先基準ディレクトリ
        /// </summary>
        private const string _FontDirectory = @"Common/Fonts";

        private const string _Font_Prefix = "_Font_";

        /// <summary>
        /// フォント格納先/MaterialIcon
        /// </summary>
        private const string _Font_MaterialIcons = @"/Material/MaterialIcons-Regular.ttf";

        /// <summary>
        /// フォント名と定数のペア
        /// </summary>
        private readonly Dictionary<FONT_SELECTOR, string> _FontPair = new Dictionary<FONT_SELECTOR, string>()
        {
            { FONT_SELECTOR.MATERIALICONS, _Font_MaterialIcons },
        };

        /// <summary>
        /// 読み込みフォント指定
        /// </summary>
        public enum FONT_SELECTOR
        {
            UNSELECT = -1,
            MATERIALICONS = 0
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FontLoader()
        {
        }

        /// <summary>
        /// フォントデータをファイルから取得 (Avalonia用)
        /// </summary>
        /// <param name="Selector">フォント指定</param>
        /// <param name="Size">フォントサイズ</param>
        /// <returns>FontFamily</returns>
        /// <exception cref="KeyNotFoundException">対応するフォントがない</exception>
        public FontFamily LoadFontFamilyFromFile(FONT_SELECTOR Selector)
        {
            if (!this._FontPair.ContainsKey(Selector))
            {
                throw new KeyNotFoundException();
            }

            // Avalonia用のフォント読み込み
            var fontPath = _FontPair[Selector];
            
            try
            {
                // Avaloniaのリソースからフォントを読み込む
                return new FontFamily("avares://MiView/Common/Fonts/Material/MaterialIcons-Regular.ttf#Material Icons");
            }
            catch
            {
                // エラーの場合はデフォルトフォントを返す
                return FontFamily.Default;
            }
        }

        /// <summary>
        /// Windows Forms互換のFont風メソッド (Avalonia用)
        /// </summary>
        /// <param name="Selector">フォント指定</param>
        /// <param name="Size">フォントサイズ</param>
        /// <returns>AvaloniaFont</returns>
        /// <exception cref="KeyNotFoundException">対応するフォントがない</exception>
        public AvaloniaFont LoadFontFromFile(FONT_SELECTOR Selector, float Size)
        {
            var fontFamily = LoadFontFamilyFromFile(Selector);
            return new AvaloniaFont(fontFamily, Size);
        }
    }

    /// <summary>
    /// Avalonia用のFont代替クラス
    /// </summary>
    public class AvaloniaFont
    {
        public FontFamily FontFamily { get; }
        public double Size { get; }

        public AvaloniaFont(FontFamily fontFamily, double size)
        {
            FontFamily = fontFamily;
            Size = size;
        }
    }
}
