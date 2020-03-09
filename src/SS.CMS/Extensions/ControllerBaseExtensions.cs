using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;

namespace SS.CMS.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static FileContentResult Download(this ControllerBase controller, string filePath, string fileName)
        {
            //return controller.File(filePath, "application/force-download", fileName);

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return controller.File(fileBytes, "application/force-download", fileName);
        }

        public static FileContentResult Download(this ControllerBase controller, string filePath)
        {
            var fileName = PathUtils.GetFileName(filePath);
            return Download(controller, filePath, fileName);
        }

        public static BadRequestObjectResult Error(this ControllerBase controller, string message)
        {
            return controller.BadRequest(new
            {
                Message = message
            });
        }
    }
}