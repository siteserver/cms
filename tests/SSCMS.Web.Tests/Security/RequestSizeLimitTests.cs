using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Web.Controllers.Admin;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class RequestSizeLimitTests
    {
        [Fact]
        public void ControllersDoNotDisableRequestSizeLimits()
        {
            var offenders = typeof(InstallController).Assembly
                .GetTypes()
                .Where(type => type.Namespace != null && type.Namespace.StartsWith("SSCMS.Web.Controllers", StringComparison.Ordinal))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                .Select(method => new
                {
                    Method = method,
                    Attribute = method.GetCustomAttribute<RequestSizeLimitAttribute>()
                })
                .Where(x => ((IRequestSizeLimitMetadata)x.Attribute)?.MaxRequestBodySize == long.MaxValue)
                .Select(x => $"{x.Method.DeclaringType?.FullName}.{x.Method.Name}")
                .OrderBy(x => x)
                .ToList();

            Assert.Empty(offenders);
        }

        [Fact]
        public void FormOptionsDoNotUseUnboundedValueLengthLimit()
        {
            var startupPath = FindRepositoryFile("src/SSCMS.Web/Startup.cs");
            var source = File.ReadAllText(startupPath);

            Assert.DoesNotContain("ValueLengthLimit = int.MaxValue", source, StringComparison.Ordinal);
        }

        private static string FindRepositoryFile(string relativePath)
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                var candidate = Path.Combine(directory.FullName, relativePath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new FileNotFoundException(relativePath);
        }
    }
}
