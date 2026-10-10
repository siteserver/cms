using System.IO;

namespace SSCMS.Utils
{
    /// <summary>
    /// 资源/模板文件路径的安全校验，防止路径遍历（../）、绝对路径、空字节等输入跳出允许目录。
    /// </summary>
    public static class AssetPathUtils
    {
        /// <summary>
        /// 文件名：非空，不包含路径分隔符、..、空字节、盘符及非法字符。
        /// </summary>
        public static bool IsSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            if (fileName.IndexOf('\0') >= 0) return false;
            if (fileName.Contains("/") || fileName.Contains("\\") || fileName.Contains(":")) return false;
            if (fileName == "." || fileName.Contains("..")) return false;
            return fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        /// <summary>
        /// 相对路径：允许为空；不得为绝对路径、不得含盘符/空字节，任何一级都不能是 . 或 ..（含 URL 编码形式）。
        /// </summary>
        public static bool IsSafeRelativePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return true;
            if (relativePath.IndexOf('\0') >= 0 || relativePath.Contains(":")) return false;
            if (relativePath.IndexOf('%') >= 0) return false; // 拒绝残留的 URL 编码变体（如 %2e%2e）
            if (relativePath.StartsWith("/") || relativePath.StartsWith("\\") || Path.IsPathRooted(relativePath)) return false;

            foreach (var segment in relativePath.Split('/', '\\'))
            {
                if (segment == ".." || segment == ".") return false;
                // 拒绝仅由点和空格组成的段（如 "..." 或 ". ."，Windows 下可能被解析为上级目录）
                var trimmed = segment.Trim();
                if (trimmed.Length > 0 && trimmed.Trim('.').Length == 0) return false;
            }
            return true;
        }

        /// <summary>
        /// 规范化后 path 是否位于 rootPath 目录内（不含 rootPath 自身）。
        /// </summary>
        public static bool IsUnder(string rootPath, string path)
        {
            if (string.IsNullOrEmpty(rootPath) || string.IsNullOrEmpty(path)) return false;
            if (!DirectoryUtils.IsInDirectory(rootPath, path)) return false;
            try
            {
                var root = Path.GetFullPath(rootPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var full = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                return full.Length > root.Length;
            }
            catch
            {
                return false;
            }
        }
    }
}
