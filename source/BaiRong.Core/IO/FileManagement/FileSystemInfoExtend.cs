using System;
using System.IO;

namespace BaiRong.Core.IO.FileManagement
{
	/// <summary>
	/// FileSystemInfoExtend ��ժҪ˵����
	/// </summary>
	public class FileSystemInfoExtend : SimpleFileInfoBase
	{
		private FileSystemInfo _file ;

		public FileSystemInfoExtend(FileSystemInfo file)
		{
			_file = file ;
		}

		override public string Name => _file.Name;

	    override public string FullName => _file.FullName;

	    public bool IsDirectory => (_file.Attributes & FileAttributes.Directory)
	                               ==FileAttributes.Directory;

	    public string Type => IsDirectory ? "" : _file.Extension;

	    override public long Size
		{
			get
			{
				if ( IsDirectory )
					return 0L ;
				else
					return ((FileInfo)_file).Length  ;
			}
		}

		override public DateTime LastWriteTime => _file.LastWriteTime;

	    public override DateTime CreationTime => _file.CreationTime;
	}
}
