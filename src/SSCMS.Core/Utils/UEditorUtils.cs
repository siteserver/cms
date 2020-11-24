using SSCMS.Core.StlParser.StlElement;

namespace SSCMS.Core.Utils
{
    public static class UEditorUtils
    {
        public static string TranslateToStlElement(string html)
        {
            var retVal = html;
            if (!string.IsNullOrEmpty(retVal))
            {
                retVal = retVal.Replace($"<img {StlPlayer.EditorPlaceHolder} ", $"<{StlPlayer.ElementName} ");
                retVal = retVal.Replace($"<img {StlAudio.EditorPlaceHolder} ", $"<{StlAudio.ElementName} ");
                retVal = retVal.Replace($"<img {StlMaterial.EditorPlaceHolder} ", $"<{StlMaterial.ElementName} ");
            }
            return retVal;
        }

        public static string TranslateToHtml(string html)
        {
            var retVal = html;
            if (!string.IsNullOrEmpty(retVal))
            {
                retVal = retVal.Replace($"<{StlPlayer.ElementName} ", $"<img {StlPlayer.EditorPlaceHolder} ");
                retVal = retVal.Replace($"<{StlAudio.ElementName} ", $"<img {StlAudio.EditorPlaceHolder} ");
                retVal = retVal.Replace($"<{StlMaterial.ElementName} ", $"<img {StlMaterial.EditorPlaceHolder} ");
            }
            return retVal;
        }
    }
}
