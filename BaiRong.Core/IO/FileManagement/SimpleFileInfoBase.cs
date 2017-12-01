using System;

namespace BaiRong.Core.IO.FileManagement
{
	public abstract class SimpleFileInfoBase
	{
		public abstract string Name { get ; }
		public abstract string FullName { get ; }
		public abstract DateTime LastWriteTime { get ; }
		public abstract DateTime CreationTime { get ; }
		public abstract long Size { get ; }
	}
}
