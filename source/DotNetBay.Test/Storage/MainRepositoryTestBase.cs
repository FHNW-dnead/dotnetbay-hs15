using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DotNetBay.Interfaces;
using DotNetBay.Model;

using NUnit.Framework;

namespace DotNetBay.Test.Storage
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "This are tests")]
    public abstract class MainRepositoryTestBase
    {
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenAnEmptyRepo_AddOneAuction_NotEmptyAnymore()
        {
            var createdAuction = CreateAnAuction();
            createdAuction.Seller = CreateAMember();

            Auction auctionFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();

                auctionFromRepo = testRepo.GetAuctions().FirstOrDefault();
            }

            Assert.IsNotNull(auctionFromRepo);
        }

        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ANew", Justification = "This is correct")]
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenANewRepository_CanBeSaved_WithNoIssues()
        {
            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.SaveChanges();
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenAnEmptyRepo_AddAuctionWithSeller_AuctionAndMemberAreInRepoIndividually()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();

            createdAuction.Seller = createdMember;

            Member memberFromRepo;
            Auction auctionFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();

                auctionFromRepo = testRepo.GetAuctions().FirstOrDefault();
                memberFromRepo = testRepo.GetMembers().FirstOrDefault();
            }

            Assert.IsNotNull(auctionFromRepo, "auctionFromRepo != null");
            Assert.IsNotNull(auctionFromRepo.Seller, "auctionFromRepo.Seller != null");

            Assert.IsNotNull(memberFromRepo, "memberForRepo != null");
            Assert.IsNotNull(memberFromRepo.Auctions, "memberForRepo.Auctions != null");
            Assert.AreEqual(1, memberFromRepo.Auctions.Count, "There should be exact one auction for this member");

            Assert.AreEqual(createdAuction.Title, auctionFromRepo.Title, "Auction's title is not the same");
            Assert.AreEqual(createdMember.UniqueId, memberFromRepo.UniqueId, "Member's uniqueId is not the same");
            Assert.AreEqual(1, memberFromRepo.Auctions.Count, "There should be exact one euction for this member");
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenAnEmptyRepo_AddAMemberWithAuctions_MemberAndAuctionsAreInRepoIndividually()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            Member memberForRepo;
            Auction auctionFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdMember);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();

                memberForRepo = testRepo.GetMembers().FirstOrDefault();
                auctionFromRepo = testRepo.GetAuctions().FirstOrDefault();
            }

            Assert.IsNotNull(memberForRepo, "memberForRepo != null");
            Assert.IsNotNull(memberForRepo.Auctions, "memberForRepo.Auctions != null");
            Assert.AreEqual(1, memberForRepo.Auctions.Count, "There should be exact one euction for this member");

            Assert.IsNotNull(auctionFromRepo, "auctionFromRepo != null");
            Assert.IsNotNull(auctionFromRepo.Seller, "auctionFromRepo.Seller != null");
            Assert.AreEqual(auctionFromRepo.Seller.UniqueId, createdMember.UniqueId);
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenAnExistingMember_AddAuctionWithExistingMemberAsSeller_AuctionIsAttachedToMember()
        {
            var createdMember = CreateAMember();
            var createdAuction = CreateAnAuction();

            List<Member> allMembersFromRepo;
            List<Auction> allAuctionsFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdMember);
                initRepo.SaveChanges();

                var secondRepo = factory.CreateMainRepository();

                // References
                createdAuction.Seller = secondRepo.GetMembers().FirstOrDefault();
                secondRepo.Add(createdAuction);
                secondRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();
                allAuctionsFromRepo = testRepo.GetAuctions().ToList();
                allMembersFromRepo = testRepo.GetMembers().ToList();
            }

            Assert.AreEqual(1, allAuctionsFromRepo.Count(), "There should be exact 1 auction");
            Assert.AreEqual(1, allMembersFromRepo.Count(), "There should be exact 1 member");

            Assert.IsNotNull(allMembersFromRepo, "memberForRepo != null");
            Assert.IsNotNull(allMembersFromRepo.First().Auctions, "memberForRepo.Auctions != null");

            Assert.AreEqual(1, allMembersFromRepo.First().Auctions.Count(), "There should be a auction attached to the member");
            Assert.AreEqual(createdAuction.Id, allMembersFromRepo.First().Auctions.First().Id);
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenARepoWithAuctionAndMember_AddBid_BidGetsListedInAuction()
        {
            var theSeller = CreateAMember();
            var createdAuction = CreateAnAuction();

            // References
            createdAuction.Seller = theSeller;

            var theBidder = CreateAMember();
            var bid = new Bid()
            {
                Auction = createdAuction,
                Bidder = theBidder,
                Amount = 12
            };

            List<Auction> allAuctionsFromRepo;

            using (var factory = this.CreateFactory())
            {
                var testRepo = factory.CreateMainRepository();

                testRepo.Add(createdAuction);
                testRepo.Add(theBidder);
                testRepo.Add(bid);
                testRepo.SaveChanges();

                allAuctionsFromRepo = testRepo.GetAuctions().ToList();
            }

            // Sanity check
            Assert.AreEqual(1, allAuctionsFromRepo.Count());
            Assert.IsNotNull(allAuctionsFromRepo[0].Bids);

            Assert.AreEqual(1, allAuctionsFromRepo[0].Bids.Count);
            Assert.AreEqual(bid, allAuctionsFromRepo[0].Bids.First());
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenARepoWithAuctionAndMember_AddBid_AuctionIsReferencedFromBidder()
        {
            var theSeller = CreateAMember();
            var createdAuction = CreateAnAuction();

            // References
            createdAuction.Seller = theSeller;

            var theBidder = CreateAMember();

            var bid = new Bid()
            {
                Auction = createdAuction,
                Bidder = theBidder,
                Amount = 12
            };

            List<Member> allMembersFromRepo;

            using (var factory = this.CreateFactory())
            {
                var testRepo = factory.CreateMainRepository();

                testRepo.Add(createdAuction);
                testRepo.Add(theBidder);
                testRepo.Add(bid);
                testRepo.SaveChanges();

                allMembersFromRepo = testRepo.GetMembers().ToList();
            }

            // Sanity check
            Assert.AreEqual(2, allMembersFromRepo.Count());

            // Take the bidder to test
            var bidderMember = allMembersFromRepo.FirstOrDefault(b => b.UniqueId == theBidder.UniqueId);
            Assert.IsNotNull(bidderMember);
            Assert.IsNotNull(bidderMember.Bids);

            Assert.AreEqual(1, bidderMember.Bids.Count);
            Assert.AreEqual(bid, bidderMember.Bids.First());
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenARepoWithAuctionAndMember_AddBid_CanBeRetrievedByTransactionId()
        {
            var theSeller = CreateAMember();
            var createdAuction = CreateAnAuction();

            // References
            createdAuction.Seller = theSeller;

            var theBidder = CreateAMember();
            var bid = new Bid()
            {
                Auction = createdAuction,
                Bidder = theBidder,
                Amount = 12
            };

            Bid retrievedBid;

            using (var factory = this.CreateFactory())
            {
                var testRepo = factory.CreateMainRepository();
                testRepo.Add(theBidder);
                testRepo.Add(createdAuction);
                testRepo.Add(bid);
                testRepo.SaveChanges();

                retrievedBid = testRepo.GetBidByTransactionId(bid.TransactionId);
            }

            Assert.IsNotNull(retrievedBid);
            Assert.AreEqual(bid, retrievedBid);
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenARepoWithMember_AddMemberAgain_ShouldNotAddTwice()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            List<Member> allMembers;

            using (var factory = this.CreateFactory())
            {
                var firstRepo = factory.CreateMainRepository();
                firstRepo.Add(createdMember);
                firstRepo.Add(createdMember);

                firstRepo.SaveChanges();

                allMembers = firstRepo.GetMembers().ToList();
            }

            Assert.NotNull(allMembers);
            Assert.AreEqual(1, allMembers.Count(), "There should be only one member");
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenARepoWithAuction_AddAuctionAgain_ShouldNotAddTwice()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            List<Auction> allAuctions;

            using (var factory = this.CreateFactory())
            {
                var testRepo = factory.CreateMainRepository();
                testRepo.Add(createdAuction);
                testRepo.Add(createdAuction);

                testRepo.SaveChanges();
                allAuctions = testRepo.GetAuctions().ToList();
            }

            Assert.NotNull(allAuctions);
            Assert.AreEqual(1, allAuctions.Count(), "There should be only one auction");
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        [ExpectedException]
        public void GivenEmptyRepo_AddMemberWithAuctionsFromOtherInstance_ShouldRaiseException()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();
            var otherMember = CreateAMember();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            otherMember.Auctions = new List<Auction>(new[] { createdAuction });

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdMember);
                initRepo.SaveChanges();

                var testSore = factory.CreateMainRepository();
                testSore.Add(otherMember);
                testSore.SaveChanges();
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void GivenAnEmptyRepo_AddAuctionAndMember_ReferencesShouldBeEqual()
        {
            var createdMember = CreateAMember();
            var createdAuction = CreateAnAuction();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            List<Member> allMembersFromRepo;
            List<Auction> allAuctionFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();
                allAuctionFromRepo = testRepo.GetAuctions().ToList();
                allMembersFromRepo = testRepo.GetMembers().ToList();
            }

            Assert.AreEqual(1, allAuctionFromRepo.Count(), "There should be exact 1 auction");
            Assert.AreEqual(1, allMembersFromRepo.Count(), "There should be exact 1 member");

            Assert.AreEqual(allAuctionFromRepo.FirstOrDefault().Seller, allMembersFromRepo.FirstOrDefault());
            Assert.AreEqual(allMembersFromRepo.FirstOrDefault().Auctions.FirstOrDefault(), allAuctionFromRepo.FirstOrDefault());
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void AuctionWithImage_IsSavedInRepo_CanBeRetrievedAfterwards()
        {
            var createdMember = CreateAMember();
            var createdAuction = CreateAnAuction();
            createdAuction.Seller = createdMember;

            var emptyImage = Guid.NewGuid().ToByteArray();
            createdAuction.Image = emptyImage;

            byte[] imageFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();
                imageFromRepo = testRepo.GetAuctions().First().Image;
            }

            Assert.AreEqual(emptyImage, imageFromRepo);
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        public void AuctionWithImage_IsUpdatedWithNoImage_ImageIsGone()
        {
            var createdMember = CreateAMember();
            var createdAuction = CreateAnAuction();
            createdAuction.Seller = createdMember;

            var emptyImage = Guid.NewGuid().ToByteArray();
            createdAuction.Image = emptyImage;

            byte[] imageFromRepo;

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var secondRepo = factory.CreateMainRepository();
                var auctionFromRepo = secondRepo.GetAuctions().First();
                auctionFromRepo.Image = null;
                secondRepo.Update(auctionFromRepo);
                secondRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();
                imageFromRepo = testRepo.GetAuctions().First().Image;
            }

            Assert.IsNull(imageFromRepo);
        }

        #region Create Helpers

        protected static Auction CreateAnAuction()
        {
            return new Auction()
            {
                Title = "TitleOfTheAuction",
                StartPrice = 50.5,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(10),
            };
        }

        protected static Member CreateAMember()
        {
            return new Member()
            {
                DisplayName = "GeneratedMember",
                UniqueId = "UniqueId" + Guid.NewGuid()
            };
        }

        #endregion

        protected abstract IRepositoryFactory CreateFactory();
    }
}
