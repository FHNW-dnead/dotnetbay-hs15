using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DotNetBay.Model
{
    public class Member
    {
        public Member()
        {
            this.Auctions = new List<Auction>();
        }

        public long Id { get; set; }

        public string UniqueId { get; set; }

        public string DisplayName { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", Justification = "Keep it as is for compatibility reasons")]
        public string EMail { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Cannot reomve setter, because needs to be accessible by ORM")]
        public ICollection<Auction> Auctions { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Cannot reomve setter, because needs to be accessible by ORM")]
        public ICollection<Bid> Bids { get; set; } 
    }
}