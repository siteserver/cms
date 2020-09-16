using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerAddMultipleController
    {
        [HttpGet, Route(Route)]
        public ActionResult<GetResult> Get()
        {
            var styles = new List<Style>
            {
                new Style {
                    InputType = InputType.Text
                }
            };

            return new GetResult
            {
                InputTypes = InputTypeUtils.GetInputTypes(),
                Styles = styles
            };
        }
    }
}
