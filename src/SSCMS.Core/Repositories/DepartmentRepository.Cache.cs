using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class DepartmentRepository
    {
        public async Task<Cascade<int>> GetCascadeAsync(Department department, Func<Department, Task<object>> func)
        {
            object extra = null;
            if (func != null)
            {
                extra = await func(department);
            }

            if (extra == null) return null;

            var cascade = new Cascade<int>
            {
                Value = department.Id,
                Label = department.Name,
                Children = await GetCascadesAsync(department.Id, func)
            };

            var dict = TranslateUtils.ToDictionary(extra);
            foreach (var o in dict)
            {
                cascade[o.Key] = o.Value;
            }

            return cascade;
        }

        public async Task<List<Cascade<int>>> GetCascadesAsync(int parentId, Func<Department, Task<object>> func)
        {
            var list = new List<Cascade<int>>();

            var departments = await GetDepartmentsAsync();
            foreach (var department in departments)
            {
                if (department == null) continue;
                if (department.ParentId == parentId)
                {
                    var cascade = await GetCascadeAsync(department, func);
                    if (cascade != null)
                    {
                        list.Add(cascade);
                    }
                }
            }

            return list;
        }

        public async Task<Cascade<int>> GetCascadeAsync(Department department)
        {
            return new Cascade<int>
            {
                Value = department.Id,
                Label = department.Name,
                Children = await GetCascadesAsync(department.Id)
            };
        }

        private async Task<List<Cascade<int>>> GetCascadesAsync(int parentId)
        {
            var list = new List<Cascade<int>>();

            var departments = await GetDepartmentsAsync();

            foreach (var department in departments)
            {
                if (department == null) continue;
                if (department.ParentId == parentId)
                {
                    list.Add(await GetCascadeAsync(department));
                }
            }

            return list;
        }

        public async Task<IList<Department>> GetChildrenAsync(int parentId)
        {
            var list = new List<Department>();

            var departments = await GetDepartmentsAsync();

            foreach (var department in departments)
            {
                if (department == null) continue;
                if (department.ParentId == parentId)
                {
                    department.Children = await GetChildrenAsync(department.Id);
                    list.Add(department);
                }
            }

            return list;
        }

        private void GetParentIdsRecursive(List<Department> departments, List<int> list, int departmentId)
        {
            var department = departments.FirstOrDefault(x => x.Id == departmentId);
            if (department != null && department.ParentId > 0)
            {
                list.Add(department.ParentId);
                GetParentIdsRecursive(departments, list, department.ParentId);
            }
        }

        private List<int> GetChildIds(List<Department> departments, int parentId)
        {
            return departments.Where(x => x.ParentId == parentId).Select(x => x.Id).ToList();
        }

        private void GetChildIdsRecursive(List<Department> departments, List<int> list, int parentId)
        {
            var childIds = departments.Where(x => x.ParentId == parentId).Select(x => x.Id).ToList();
            if (childIds.Count > 0)
            {
                list.AddRange(childIds);
                foreach (var childId in childIds)
                {
                    GetChildIdsRecursive(departments, list, childId);
                }
            }
        }

        public async Task<List<int>> GetDepartmentIdsAsync(int departmentId, ScopeType scopeType, Query query = null)
        {
            var departmentIds = new List<int>();

            if (departmentId == 0) return departmentIds;

            if (scopeType == ScopeType.Self)
            {
                departmentIds = new List<int> { departmentId };
            }
            else if (scopeType == ScopeType.SelfAndChildren)
            {
                var departments = await GetDepartmentsAsync();
                departmentIds = GetChildIds(departments, departmentId);
                departmentIds.Add(departmentId);
            }
            else if (scopeType == ScopeType.Children)
            {
                var departments = await GetDepartmentsAsync();
                departmentIds = GetChildIds(departments, departmentId);
            }
            else if (scopeType == ScopeType.Descendant)
            {
                var departments = await GetDepartmentsAsync();
                GetChildIdsRecursive(departments, departmentIds, departmentId);
            }
            else if (scopeType == ScopeType.All)
            {
                var departments = await GetDepartmentsAsync();
                departmentIds = new List<int> { departmentId };
                GetChildIdsRecursive(departments, departmentIds, departmentId);
            }

            if (query != null)
            {
                var q = query.Clone();
                q.Select(nameof(Department.Id));

                var list = await _repository.GetAllAsync<int>(q);
                departmentIds = departmentIds.Intersect<int>(list).ToList();
            }

            return departmentIds;
        }

        public async Task<string> GetNameAsync(int departmentId)
        {
            var retVal = string.Empty;
            var department = await GetAsync(departmentId);
            if (department != null)
            {
                retVal = department.Name;
            }
            return retVal;
        }

        public async Task<string> GetFullNameAsync(int departmentId)
        {
            if (departmentId <= 0) return string.Empty;

            return await GetFullNameAsync(0, departmentId);
        }

        private async Task<string> GetFullNameAsync(int currentDepartmentId, int departmentId)
        {
            if (departmentId <= 0) return string.Empty;

            var departmentNames = new List<string>();

            if (departmentId == currentDepartmentId)
            {
                var department = await GetAsync(currentDepartmentId);
                return department != null ? department.Name : string.Empty;
            }
            if (departmentId == currentDepartmentId)
            {
                var department = await GetAsync(departmentId);
                return department != null ? department.Name : string.Empty;
            }

            var departments = await GetDepartmentsAsync();
            var parentIds = new List<int>
            {
                departmentId
            };
            GetParentIdsRecursive(departments, parentIds, departmentId);
            parentIds.Reverse();

            foreach (var parentId in parentIds)
            {
                if (parentId == currentDepartmentId)
                {
                    departmentNames.Clear();
                }
                var departmentName = await GetNameAsync(parentId);
                if (!string.IsNullOrEmpty(departmentName))
                {
                    departmentNames.Add(departmentName);
                }
            }

            return ListUtils.ToString(departmentNames, " > ");
        }

        public async Task RemoveListCacheAsync()
        {
            await _repository.RemoveCacheAsync(GetListKey());
        }
    }
}
