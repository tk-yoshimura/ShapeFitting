using System;
using System.Globalization;

namespace ShapeFitting {
    public static class ExceptionMessage {
        private enum Lang { Default, JP }

        private static readonly Lang lang;

        static ExceptionMessage() {
            string culture_name = CultureInfo.CurrentCulture.Name;
            switch (culture_name) {
                case "ja-JP":
                    lang = Lang.JP;
                    break;
                default:
                    lang = Lang.Default;
                    break;
            }
        }

        public static string MismatchLength =>
            (lang == Lang.Default) ? "Mismatch length" :
            (lang == Lang.JP) ? "配列の長さが不一致です" :
            throw new NotImplementedException();

    }
}
