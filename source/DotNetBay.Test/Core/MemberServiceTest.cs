using System.Diagnostics.CodeAnalysis;

using DotNetBay.Core;
using DotNetBay.Data.FileStorage;

using NUnit.Framework;

namespace DotNetBay.Test.Core
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "This is a testclass")]
    public class MemberServiceTest
    {
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "These are tests, thats fine!")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void WhenMemberIsNotInRepo_GetCurrentUser_AlwaysGetAMember()
        {
            var repo = new InMemoryMainRepository();
            var service = new SimpleMemberService(repo);

            var currentMember = service.GetCurrentMember();

            Assert.NotNull(currentMember);
            Assert.IsNotNullOrEmpty(currentMember.DisplayName);
            Assert.IsNotNullOrEmpty(currentMember.EMail);
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "These are tests, thats fine!")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GettingCurrentMemberTwice_IsSame()
        {
            var repo = new InMemoryMainRepository();
            var service = new SimpleMemberService(repo);

            var currentMember1 = service.GetCurrentMember();
            var currentMember2 = service.GetCurrentMember();
            
            Assert.AreEqual(currentMember1, currentMember2);
        }
    }
}
