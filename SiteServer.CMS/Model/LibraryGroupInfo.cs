using System;
using Datory;
using Newtonsoft.Json;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class LibraryGroupInfo
    {
        public int Id { get; set; }

        [JsonIgnore]
        private string Type { get; set; }

        public LibraryType LibraryType
        {
            get => GetEnumType(Type);
            set => Type = GetValue(value);
        }

        public string GroupName { get; set; }

        public static LibraryType GetEnumType(string typeStr)
        {
            var retVal = LibraryType.Text;

            if (Equals(LibraryType.Text, typeStr))
            {
                retVal = LibraryType.Text;
            }
            else if (Equals(LibraryType.Image, typeStr))
            {
                retVal = LibraryType.Image;
            }

            return retVal;
        }

        public static bool Equals(LibraryType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static string GetValue(LibraryType type)
        {
            if (type == LibraryType.Text)
            {
                return "Text";
            }
            if (type == LibraryType.Image)
            {
                return "Image";
            }
            throw new Exception();
        }
    }
}