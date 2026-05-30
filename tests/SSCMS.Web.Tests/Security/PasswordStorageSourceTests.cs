using System;
using System.IO;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class PasswordStorageSourceTests
    {
        [Fact]
        public void NewAndChangedPasswordsAreNotStoredAsSm4()
        {
            var administratorRepository = File.ReadAllText(FindRepositoryFile("src/SSCMS.Core/Repositories/AdministratorRepository.cs"));
            var userRepository = File.ReadAllText(FindRepositoryFile("src/SSCMS.Core/Repositories/UserRepository.cs"));
            var source = administratorRepository + userRepository;

            Assert.DoesNotContain("PasswordFormat = PasswordFormat.SM4", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EncodePassword(password, PasswordFormat.SM4", source, StringComparison.Ordinal);
            Assert.Contains("PasswordFormat = PasswordFormat.Hashed", source, StringComparison.Ordinal);
            Assert.Contains("EncodePassword(password, PasswordFormat.Hashed", source, StringComparison.Ordinal);
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
