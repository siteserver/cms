using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public partial class DepartmentRepository : IDepartmentRepository
    {
        private readonly Repository<Department> _repository;

        public DepartmentRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<Department>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetListKey()
        {
            return CacheUtils.GetListKey(_repository.TableName);
        }

        public async Task InsertAsync(Department parentDepartment, Department department)
        {
            if (parentDepartment != null)
            {
                department.ParentsPath = parentDepartment.ParentsPath == null
                    ? new List<int> {parentDepartment.Id}
                    : new List<int>(parentDepartment.ParentsPath) {parentDepartment.Id};

                department.ParentsCount = parentDepartment.ParentsCount + 1;

                var maxTaxis = await GetMaxTaxisAsync(department.ParentId);
                if (maxTaxis == 0)
                {
                    maxTaxis = parentDepartment.Taxis;
                }
                department.Taxis = maxTaxis + 1;
            }
            else
            {
                department.Taxis = 1;
            }

            department.ChildrenCount = 0;

            if (parentDepartment != null)
            {
                await _repository.IncrementAsync(nameof(Department.Taxis), Q
                    .Where(nameof(Department.Taxis), ">=", department.Taxis)
                );
            }

            department.Id = await _repository.InsertAsync(department, Q
                .CachingRemove(GetListKey())
            );

            if (parentDepartment != null)
            {
                await UpdateChildrenCountAsync(parentDepartment.Id);
            }
        }

        public async Task<int> InsertAsync(int parentId, string departmentName)
        {
            var department = new Department
            {
                ParentId = parentId,
                Name = departmentName,
            };

            var parentDepartment = await GetAsync(parentId);
            await InsertAsync(parentDepartment, department);

            return department.Id;
        }

        public async Task UpdateAsync(Department department)
        {
            if (department.ParentId == department.Id)
            {
                department.ParentId = 0;
            }
            await _repository.UpdateAsync(department, Q
                .CachingRemove(GetListKey())
            );
        }

        private async Task<int> GetChildrenCountAsync(int departmentId)
        {
            var departments = await GetDepartmentsAsync();
            return departments.Where(department => department.ParentId == departmentId).Count();
        }

        public async Task UpdateChildrenCountAsync(int departmentId)
        {
          var childrenCount = await GetChildrenCountAsync(departmentId);
          await _repository.UpdateAsync(Q
              .Set(nameof(Department.ChildrenCount), childrenCount)
              .Where(nameof(Department.Id), departmentId)
              .CachingRemove(GetListKey())
          );
        }

        public async Task UpdateCountAsync(int departmentId, int count)
        {
            await _repository.UpdateAsync(Q
              .Set(nameof(Department.Count), count)
              .Where(nameof(Department.Id), departmentId)
              .CachingRemove(GetListKey())
            );
        }

        public async Task DeleteAsync(int departmentId)
        {
            var department = await GetAsync(departmentId);
            if (department == null) return;

            var idList = new List<int>();
            if (department.ChildrenCount > 0)
            {
                idList = await GetDepartmentIdsAsync(departmentId, ScopeType.Descendant);
            }
            idList.Add(departmentId);

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(Department.Id), idList)
                .CachingRemove(GetListKey())
            );

            if (department.ParentId != 0)
            {
                await UpdateChildrenCountAsync(department.ParentId);
            }
        }

        public async Task<int> GetDepartmentIdAsync(string name1, string name2)
        {
            if (string.IsNullOrEmpty(name1)) return 0;

            var parent = await GetAsync(0, name1);
            int parentId;
            if (parent == null)
            {
                parentId = await InsertAsync(0, name1);
            }
            else
            {
                parentId = parent.Id;
            }

            if (string.IsNullOrEmpty(name2))
            {
                return parentId;
            }
            var department = await GetAsync(parentId, name2);
            if (department != null)
            {
                return department.Id;
            }
            return await InsertAsync(parentId, name2);
        }

        public async Task<Department> GetAsync(int parentId, string name)
        {
            var departments = await GetDepartmentsAsync();
            return departments.FirstOrDefault(n => n.ParentId == parentId && n.Name == name);
        }

        public async Task<List<Department>> GetDepartmentsAsync()
        {
            return await _repository.GetAllAsync(Q
                .OrderBy(nameof(Department.Taxis), nameof(Department.Id))
                .CachingGet(GetListKey())
            );
        }

        public async Task<Department> GetAsync(int departmentId)
        {
            var departments = await GetDepartmentsAsync();
            return departments.FirstOrDefault(n => n.Id == departmentId);
        }

        public async Task<int> GetAllCountAsync(Department department)
        {
            if (department == null) return 0;

            var count = department.Count;
            if (department.ChildrenCount > 0)
            {
                var departmentIds = await GetDepartmentIdsAsync(department.Id, ScopeType.Descendant);
                if (departmentIds != null && departmentIds.Count > 0)
                {
                    foreach (var id in departmentIds)
                    {
                        var depart = await GetAsync(id);
                        if (depart != null)
                        {
                            count += depart.Count;
                        }
                    }
                }
            }
            return count;
        }
    }
}
