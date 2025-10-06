using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace MiView.Common.Fonts
{
    internal sealed class FontLoader
    {
        /// <summary>
        /// フォントコレクション（アプリ全体で共有）
        /// </summary>
        private readonly PrivateFontCollection _Fonts = new();

        /// <summary>
        /// 読み込んだ FontFamily のキャッシュ
        /// </summary>
        private readonly Dictionary<FONT_SELECTOR, FontFamily> _CachedFamilies = new();

        /// <summary>
        /// サイズごとの Font インスタンスキャッシュ
        /// </summary>
        private readonly Dictionary<string, Font> _FontCache = new();

        /// <summary>
        /// フォント格納ディレクトリ
        /// </summary>
        private const string _FontDirectory = @"./Common/Fonts";

        /// <summary>
        /// フォント識別名のプリフィクス
        /// </summary>
        private const string _Font_Prefix = "_Font_";

        /// <summary>
        /// フォントファイル定義
        /// </summary>
        private const string _Font_MaterialIcons = @"/Material/MaterialIcons-Regular.ttf";

        /// <summary>
        /// ロード対象のペア
        /// </summary>
        private readonly Dictionary<FONT_SELECTOR, string> _FontPair = new()
        {
            { FONT_SELECTOR.MATERIALICONS, _Font_MaterialIcons },
        };

        /// <summary>
        /// フォント選択肢
        /// </summary>
        public enum FONT_SELECTOR
        {
            UNSELECT = -1,
            MATERIALICONS = 0
        }

        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static FontLoader Instance { get; } = new FontLoader();

        /// <summary>
        /// コンストラクタ（外部から呼び出し不可）
        /// </summary>
        private FontLoader()
        {
            // 起動時にすべてのフォントを登録
            foreach (var kvp in _FontPair)
            {
                string fullPath = Path.GetFullPath(Path.Combine(_FontDirectory, kvp.Value.TrimStart('/')));
                if (File.Exists(fullPath))
                {
                    _Fonts.AddFontFile(fullPath);
                    _CachedFamilies[kvp.Key] = _Fonts.Families[^1];
                }
                else
                {
                    Console.WriteLine($"[FontLoader] Missing font file: {fullPath}");
                }
            }
        }

        /// <summary>
        /// 指定フォントを取得（キャッシュを優先）
        /// </summary>
        /// <param name="selector">フォント種別</param>
        /// <param name="size">サイズ</param>
        /// <returns>Font</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public Font LoadFontFromFile(FONT_SELECTOR selector, float size)
        {
            if (!_CachedFamilies.ContainsKey(selector))
                throw new KeyNotFoundException($"Font not found for selector {selector}");

            string cacheKey = $"{selector}_{size}";

            if (_FontCache.TryGetValue(cacheKey, out var cached))
                return cached;

            // キャッシュになければ新規生成して保存
            var font = new Font(_CachedFamilies[selector], size, FontStyle.Regular, GraphicsUnit.Point);
            _FontCache[cacheKey] = font;
            return font;
        }
    }
}
