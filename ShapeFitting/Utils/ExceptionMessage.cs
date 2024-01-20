using System;
using System.Globalization;

namespace ShapeFitting {
    public static class ExceptionMessage {
        private enum Lang { Default, JP }

        private static readonly Lang lang;

        static ExceptionMessage() {
            string culture_name = CultureInfo.CurrentCulture.Name;
            lang = culture_name switch {
                "ja-JP" => Lang.JP,
                _ => Lang.Default,
            };
        }

        public static string MismatchLength =>
            (lang == Lang.Default) ? "Mismatch length" :
            (lang == Lang.JP) ? "配列の長さが不一致です" :
            throw new NotImplementedException();

    }
}
