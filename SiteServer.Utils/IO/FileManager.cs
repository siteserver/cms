using System.IO;

namespace SiteServer.Utils.IO
{
	public class FileManager
	{
		public static FileSystemInfoExtendCollection GetFileSystemInfoExtendCollection(string rootPath, bool reflesh)
		{
			FileSystemInfoExtendCollection retval = null;
			if (Directory.Exists(rootPath))
			{
				string cacheKey = $"BaiRong.Core.IO.FileManagement.FileManager:{rootPath}";
				if (CacheUtils.Get(cacheKey) == null || reflesh) 
				{
					var currentRoot = new DirectoryInfo(rootPath);
					var files = currentRoot.GetFileSystemInfos();
					var fsies = new FileSystemInfoExtendCollection(files);

					CacheUtils.Insert(cacheKey, fsies, rootPath);
				}

				retval = CacheUtils.Get(cacheKey) as FileSystemInfoExtendCollection ;
			}

			return retval;
		}
	}
}
