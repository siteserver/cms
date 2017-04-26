using System;
using System.Collections;

namespace BaiRong.Core.IO.FileManagement
{
    public class FTPFileSystemInfo : SimpleFileInfoBase
    {
        private string ftpString;
        ArrayList arraylist = new ArrayList();

        public FTPFileSystemInfo(string ftpString)
        {
            this.ftpString = ftpString;
            var array = ftpString.Split(' ');
            foreach (var arr in array)
            {
                if (!string.IsNullOrEmpty(arr))
                {
                    arraylist.Add(arr);
                }
            }
        }

        override public string Name
        {
            get
            {
                var name = string.Empty;
                if (arraylist.Count >= 9)
                {
                    for (var i = 8; i < arraylist.Count; i++)
                    {
                        name = name + arraylist[i] + " ";
                    }
                }
                return name.Trim();
            }
        }

        override public string FullName => Name;

        public bool IsDirectory => ((ftpString[0] == 'd') || (ftpString.ToUpper().IndexOf("<DIR>") >= 0));

        public string Type => IsDirectory ? "" : PathUtils.GetExtension(Name);

        override public long Size
        {
            get
            {
                if (IsDirectory)
                    return 0L;
                else
                {
                    if (arraylist.Count >= 5)
                    {
                        return TranslateUtils.ToLong((string)arraylist[4]);
                    }
                    return 0L;
                }
            }
        }

        override public DateTime LastWriteTime
        {
            get
            {
                if (arraylist.Count >= 8)
                {
                    var str = (string)arraylist[5] + " " + (string)arraylist[6] + " " + (string)arraylist[7];
                    if (((string)arraylist[7]).IndexOf(":") != -1)
                    {
                        str = DateTime.Now.Year.ToString() + " " + (string)arraylist[5] + " " + (string)arraylist[6] + " " + (string)arraylist[7];
                    }

                    return TranslateUtils.ToDateTime(str);
                }
                return DateTime.Now;
            }
        }

        public override DateTime CreationTime => LastWriteTime;
    }
}
