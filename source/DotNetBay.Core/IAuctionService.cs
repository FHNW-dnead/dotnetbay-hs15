using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DotNetBay.Model;

namespace DotNetBay.Core
{
    public interface IAuctionService
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Keep this as is to mainfest the dynamic of this acess")]
        IQueryable<Auction> GetAll();

        Auction Save(Auction auction);

        Bid PlaceBid(Auction auction, double amount);
    }
}
