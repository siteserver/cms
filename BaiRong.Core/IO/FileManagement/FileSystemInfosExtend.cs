using System;
using System.IO;
using System.Collections ;

namespace BaiRong.Core.IO.FileManagement
{
	public class FileSystemInfoExtendCollection : ICollection
	{
		private FileSystemInfoExtend[] _files ;
		
		public FileSystemInfoExtendCollection(FileSystemInfo[] files)
		{
			lock(SyncRoot)
			{
				_files = new FileSystemInfoExtend[files.Length] ;
				for (var i = 0 ; i < files.Length ; i++)
					_files[i] = new FileSystemInfoExtend(files[i]) ;
			}
		}

		private FileSystemInfoExtendCollection(FileSystemInfoExtend[] files)
		{
			lock(SyncRoot)
			{
				_files = new FileSystemInfoExtend[files.Length] ;
				files.CopyTo(_files, 0) ;
			}
		}

		public FileSystemInfoExtendCollection Folders
		{
			get 
			{
				var folderAL = new ArrayList() ;
				foreach(var fileItem in _files)
				{
					if (fileItem.IsDirectory)
						folderAL.Add(fileItem) ;
				}

				var folderArray = (FileSystemInfoExtend[])folderAL.ToArray(typeof(FileSystemInfoExtend)) ;

				var folders =  new FileSystemInfoExtendCollection(folderArray) ;

				return folders ;
			}
		}

		public FileSystemInfoExtendCollection Files
		{
			get 
			{
				var fileAL = new ArrayList();
				foreach(var fileItem in _files)
				{
					if (!fileItem.IsDirectory)
						fileAL.Add(fileItem);
				}

				var fileArray = (FileSystemInfoExtend[])fileAL.ToArray(typeof(FileSystemInfoExtend));

				var files = new FileSystemInfoExtendCollection(fileArray);

				return files;
			}
		}


		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{
			lock(SyncRoot)
			{
				return _files.GetEnumerator() ;
			}
		}
		#endregion

		#region ICollection Members

		public bool IsSynchronized => true;

	    public int Count
		{
			get
			{
				lock (SyncRoot)
				{
					return _files.Length ;
				}
			}
		}

		public void CopyTo(Array array, int index)
		{
			lock (SyncRoot)
			{
				_files.CopyTo(array, index) ;
			}
		}

		public object SyncRoot => this;

	    #endregion
	}
}