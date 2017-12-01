using System;

namespace BaiRong.Core.Model
{
	public class DepartmentInfo
	{
	    public DepartmentInfo()
		{
			DepartmentId = 0;
			DepartmentName = string.Empty;
            Code = string.Empty;
			ParentId = 0;
			ParentsPath = string.Empty;
			ParentsCount = 0;
			ChildrenCount = 0;
			IsLastNode = false;
			Taxis = 0;
			AddDate = DateTime.Now;
			Summary = string.Empty;
            CountOfAdmin = 0;
		}

        public DepartmentInfo(int departmentId, string departmentName, string code, int parentId, string parentsPath, int parentsCount, int childrenCount, bool isLastNode, int taxis, DateTime addDate, string summary, int countOfAdmin) 
		{
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            Code = code;
            ParentId = parentId;
            ParentsPath = parentsPath;
            ParentsCount = parentsCount;
            ChildrenCount = childrenCount;
            IsLastNode = isLastNode;
            Taxis = taxis;
            AddDate = addDate;
            Summary = summary;
            CountOfAdmin = countOfAdmin;
		}

        public int DepartmentId { get; set; }

	    public string DepartmentName { get; set; }

	    public string Code { get; set; }

	    public int ParentId { get; set; }

	    public string ParentsPath { get; set; }

	    public int ParentsCount { get; set; }

	    public int ChildrenCount { get; set; }

	    public bool IsLastNode { get; set; }

	    public int Taxis { get; set; }

	    public DateTime AddDate { get; set; }

	    public string Summary { get; set; }

	    public int CountOfAdmin { get; set; }
	}
}
