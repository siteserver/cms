using System;
using System.Collections.Generic;

namespace SS.CMS.Services.ICacheManager
{
    public partial interface ICacheManager
    {
        void ClearAll();

        void RemoveByStartString(string startString);

        void Insert(string key, object obj);

        void Insert(string key, object obj, string filePath);

        void Insert(string key, object obj, TimeSpan timeSpan, string filePath);

        void InsertHours(string key, object obj, int hours);

        void InsertMinutes(string key, object obj, int minutes);

        object Get(string key);

        bool Exists(string key);

        List<string> AllKeys { get; }
    }
}
