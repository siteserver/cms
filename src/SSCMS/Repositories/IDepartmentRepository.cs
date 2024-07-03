using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public interface IDepartmentRepository : IRepository
    {
        Task InsertAsync(Department parentDepartment, Department department);

        Task<int> InsertAsync(int parentId, string departmentName);

        Task UpdateAsync(Department department);

        Task UpdateCountAsync(int departmentId, int count);

        Task DeleteAsync(int departmentId);

        Task<int> GetDepartmentIdAsync(string name1, string name2);

        Task<Department> GetAsync(int departmentId);

        Task<List<Department>> GetDepartmentsAsync();

        Task DropAsync(int sourceId, int targetId, string dropType);

        Task<Cascade<int>> GetCascadeAsync(Department department, Func<Department, Task<object>> func);

        Task<List<Cascade<int>>> GetCascadesAsync(int parentId, Func<Department, Task<object>> func);

        Task<Cascade<int>> GetCascadeAsync(Department department);

        Task<IList<Department>> GetChildrenAsync(int parentId);

        Task<List<int>> GetDepartmentIdsAsync(int departmentId, ScopeType scopeType, Query query = null);

        Task<string> GetNameAsync(int departmentId);

        Task<string> GetFullNameAsync(int departmentId);

        Task RemoveListCacheAsync();

        Task<Department> GetAsync(int parentId, string name);

        Task<int> GetAllCountAsync(Department department);
    }
}