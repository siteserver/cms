using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ComponentEditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<IntResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialComponent))
            {
                return Unauthorized();
            }

            var componentId = request.ComponentId;
            if (componentId == 0)
            {
                if (await _materialComponentRepository.IsExistsAsync(request.Title))
                {
                    return this.Error("组件保存失败，已存在同名组件！");
                }

                componentId = await _materialComponentRepository.InsertAsync(new MaterialComponent
                {
                    GroupId = request.GroupId,
                    Title = request.Title,
                    Description = request.Description,
                    ImageUrl = request.ImageUrl,
                    Content = request.Content,
                    Parameters = request.Parameters
                });
            }
            else
            {
                var component = await _materialComponentRepository.GetAsync(componentId);

                if (component.Title != request.Title && await _materialComponentRepository.IsExistsAsync(request.Title))
                {
                    return this.Error("组件保存失败，已存在同名组件！");
                }
                
                component.Title = request.Title;
                component.Description = request.Description;
                component.ImageUrl = request.ImageUrl;
                component.Content = request.Content;
                component.Parameters = request.Parameters;
                await _materialComponentRepository.UpdateAsync(component);
            }

            return new IntResult
            {
                Value = componentId
            };
        }
    }
}