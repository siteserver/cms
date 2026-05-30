using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class DockerExampleSecurityTests
    {
        [Theory]
        [InlineData("docker/mysql/docker-compose.yml")]
        [InlineData("docker/postgres/docker-compose.yml")]
        [InlineData("docker/cluster/docker-compose.yml")]
        [InlineData("docker/README.md")]
        public void DockerExamplesDoNotShipReusableSecrets(string relativePath)
        {
            var source = File.ReadAllText(FindRepositoryFile(relativePath));

            Assert.DoesNotContain("e2a3d303-ac9b-41ff-9154-930710af0845", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("mysql-password", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("postgres-password", source, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void DockerDatabaseExamplesDoNotUseRootApplicationConnectionOrExposeInternalPorts()
        {
            var mysql = File.ReadAllText(FindRepositoryFile("docker/mysql/docker-compose.yml"));
            var postgres = File.ReadAllText(FindRepositoryFile("docker/postgres/docker-compose.yml"));
            var cluster = File.ReadAllText(FindRepositoryFile("docker/cluster/docker-compose.yml"));

            Assert.DoesNotContain("SSCMS_DATABASE_USER: root", mysql, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\"3306:3306\"", mysql, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\"5432:5432\"", postgres, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\"6379:6379\"", cluster, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("SSCMS_SECURITY_KEY: ${SSCMS_SECURITY_KEY:?", mysql, StringComparison.Ordinal);
            Assert.Contains("SSCMS_SECURITY_KEY: ${SSCMS_SECURITY_KEY:?", postgres, StringComparison.Ordinal);
            Assert.Contains("SSCMS_SECURITY_KEY: ${SSCMS_SECURITY_KEY:?", cluster, StringComparison.Ordinal);
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
