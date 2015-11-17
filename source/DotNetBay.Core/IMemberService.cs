using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using DotNetBay.Model;

namespace DotNetBay.Core
{
    public interface IMemberService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Keep this as is to mainfest the dynamic of this acess")]
        Member GetCurrentMember();

        Member GetByUniqueId(string uniqueId);

        Member Add(string displayName, string mail);

        Member Save(Member member);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Keep this as is to mainfest the dynamic of this acess")]
        IEnumerable<Member> GetAll();
    }
}