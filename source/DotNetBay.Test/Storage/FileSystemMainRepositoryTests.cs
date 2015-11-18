using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using DotNetBay.Data.FileStorage;
using DotNetBay.Interfaces;
using DotNetBay.Model;

using NUnit.Framework;

namespace DotNetBay.Test.Storage
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "This is a testclass")]
    public class FileSystemMainRepositoryTests : MainRepositoryTestBase
    {
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        [TestCase]
        [ExpectedException(typeof(FileStorageException))]
        public void GivenEmptyRepo_AddAuctionAndMemberFromOtherInstance_ShouldRaiseException()
        {
            var createdAuction = CreateAnAuction();
            var createdMember = CreateAMember();

            // References
            createdAuction.Seller = createdMember;
            createdMember.Auctions = new List<Auction>(new[] { createdAuction });

            using (var factory = this.CreateFactory())
            {
                var initRepo = factory.CreateMainRepository();
                initRepo.Add(createdAuction);
                initRepo.SaveChanges();

                var testRepo = factory.CreateMainRepository();
                testRepo.Add(createdAuction);
            }
        }

        protected override IRepositoryFactory CreateFactory()
        {
            return new TempFileMainRepositoryFactory();
        }

        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "These are tests, thats fine!")]
        public class TempFileMainRepositoryFactory : IRepositoryFactory
        {
            private TempDirectory tempDirectory;

            public TempFileMainRepositoryFactory()
            {
                this.tempDirectory = new TempDirectory();
            }

            public IMainRepository CreateMainRepository()
            {
                return new FileSystemMainRepository(Path.Combine(this.tempDirectory.Root, "data.json"));
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            // The bulk of the clean-up code is implemented in Dispose(bool)
            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    // free managed resources
                    if (this.tempDirectory != null)
                    {
                        this.tempDirectory.Dispose();
                        this.tempDirectory = null;
                    }
                }

                // free native resources if there are any.
            }
        }
    }
}