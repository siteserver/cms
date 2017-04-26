using System.IO;
using System.Web.Caching;

namespace BaiRong.Core.IO.FileManagement
{
	public class FileManager
	{
		public FileManager()
		{
		}

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

					CacheUtils.Max(cacheKey, fsies, new CacheDependency(rootPath));
				}

				retval = CacheUtils.Get(cacheKey) as FileSystemInfoExtendCollection ;
			}

			return retval;
		}
	}
}
