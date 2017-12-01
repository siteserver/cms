namespace BaiRong.Core.Model
{
    public class AreaInfo
    {
        public AreaInfo()
        {
            AreaId = 0;
            AreaName = string.Empty;
            ParentId = 0;
            ParentsPath = string.Empty;
            ParentsCount = 0;
            ChildrenCount = 0;
            IsLastNode = false;
            Taxis = 0;
            CountOfAdmin = 0;
        }

        public AreaInfo(int areaId, string areaName, int parentId, string parentsPath, int parentsCount, int childrenCount, bool isLastNode, int taxis, int countOfAdmin)
        {
            AreaId = areaId;
            AreaName = areaName;
            ParentId = parentId;
            ParentsPath = parentsPath;
            ParentsCount = parentsCount;
            ChildrenCount = childrenCount;
            IsLastNode = isLastNode;
            Taxis = taxis;
            CountOfAdmin = countOfAdmin;
        }

        public int AreaId { get; set; }

        public string AreaName { get; set; }

        public int ParentId { get; set; }

        public string ParentsPath { get; set; }

        public int ParentsCount { get; set; }

        public int ChildrenCount { get; set; }

        public bool IsLastNode { get; set; }

        public int Taxis { get; set; }

        public int CountOfAdmin { get; set; }
    }
}
