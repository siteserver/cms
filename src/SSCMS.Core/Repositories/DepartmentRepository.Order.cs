using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Models;
using Datory;

namespace SSCMS.Core.Repositories
{
    public partial class DepartmentRepository
    {
        public async Task DropAsync(int sourceId, int targetId, string dropType)
        {
            var summaries = new List<Department>(await GetDepartmentsAsync());
            var source = summaries.FirstOrDefault(x => x.Id == sourceId);
            var target = summaries.FirstOrDefault(x => x.Id == targetId);
            if (source == null || target == null) return;

            if (dropType == "after")
            {
                if (source.ParentId != target.ParentId)
                {
                    var parent = summaries.FirstOrDefault(x => x.Id == target.ParentId);

                    await _repository.DecrementAsync(nameof(Department.ChildrenCount), Q
                        .Where(nameof(Department.Id), source.ParentId)
                    );

                    await _repository.IncrementAsync(nameof(Department.ChildrenCount), Q
                        .Where(nameof(Department.Id), target.ParentId)
                    );

                    await UpdateParentAsync(summaries, sourceId, parent);
                }

                var childIds = summaries
                    .Where(x => x.ParentId == target.ParentId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Remove(sourceId);
                var index = childIds.IndexOf(targetId);
                childIds.Insert(index + 1, sourceId);
                await UpdateTaxisAsync(childIds);
            }
            else if (dropType == "before")
            {
                if (source.ParentId != target.ParentId)
                {
                    var parent = summaries.FirstOrDefault(x => x.Id == target.ParentId);

                    await _repository.DecrementAsync(nameof(Department.ChildrenCount), Q
                        .Where(nameof(Department.Id), source.ParentId)
                    );

                    await _repository.IncrementAsync(nameof(Department.ChildrenCount), Q
                        .Where(nameof(Department.Id), target.ParentId)
                    );

                    await UpdateParentAsync(summaries, sourceId, parent);
                }

                var childIds = summaries
                    .Where(x => x.ParentId == target.ParentId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Remove(sourceId);
                var index = childIds.IndexOf(targetId);
                childIds.Insert(index, sourceId);
                await UpdateTaxisAsync(childIds);
            }
            else if (dropType == "inner")
            {
                var parentId = source.ParentId;
                var childIds = summaries
                    .Where(x => x.ParentId == targetId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Add(sourceId);
                await UpdateTaxisAsync(childIds);

                await _repository.DecrementAsync(nameof(Department.ChildrenCount), Q
                    .Where(nameof(Department.Id), parentId)
                );

                await _repository.IncrementAsync(nameof(Department.ChildrenCount), Q
                    .Where(nameof(Department.Id), target.Id)
                );

                await UpdateParentAsync(summaries, sourceId, target);
            }
        }

        private async Task SetTaxisAsync(int departmentId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Department.Taxis), taxis)
                .Where(nameof(Department.Id), departmentId)
                .CachingRemove(GetListKey())
            );
        }

        private async Task<int> GetMaxTaxisAsync(int parentId)
        {
            var maxTaxis = await _repository.MaxAsync(nameof(Department.Taxis), Q
                .Where(nameof(Department.ParentId), parentId)
            );
            return maxTaxis ?? 0;
        }

        private async Task UpdateParentAsync(List<Department> summaries, int sourceId, Department parent)
        {
            var source = await GetAsync(sourceId);

            source.ParentId = parent.Id;
            var parentIds = new List<int>
            {
                parent.Id
            };
            GetParentIdsRecursive(summaries, parentIds, parent.Id);
            parentIds.Reverse();
            source.ParentsPath = parentIds;
            source.ParentsCount = source.ParentsPath.Count;

            await UpdateAsync(source);

            var childIds = summaries
                .Where(x => x.ParentId == source.Id)
                .Select(x => x.Id)
                .ToList();

            if (childIds.Count > 0)
            {
                foreach (var childId in childIds)
                {
                    await UpdateParentAsync(summaries, childId, source);
                }
            }
        }

        private async Task UpdateTaxisAsync(List<int> departmentIds)
        {
            for (var taxis = 1; taxis <= departmentIds.Count; taxis++)
            {
                await SetTaxisAsync(departmentIds[taxis - 1], taxis);
            }
        }
    }
}
