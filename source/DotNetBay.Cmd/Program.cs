using System;
using System.Linq;

using DotNetBay.Core;
using DotNetBay.Core.Execution;
using DotNetBay.Data.FileStorage;

namespace DotNetBay.Cmd
{
    /// <summary>
    /// Main Entry for program
    /// </summary>
    public static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AuctionRunner")]
        public static void Main()
        {
            Console.WriteLine("DotNetBay Commandline");

            AuctionRunner auctionRunner = null;

            try
            {
                var store = new FileSystemMainRepository("store.json");
                var auctionService = new AuctionService(store, new SimpleMemberService(store));

                auctionRunner = new AuctionRunner(store);

                Console.WriteLine("Started AuctionRunner");
                auctionRunner.Start();

                var allAuctions = auctionService.GetAll();

                Console.WriteLine("Found {0} auctions returned by the service.", allAuctions.Count());

                Console.Write("Press enter to quit");
                Console.ReadLine();
            }
            finally
            {
                if (auctionRunner != null)
                {
                    auctionRunner.Dispose();
                }
            }

            Environment.Exit(0);
        }
    }
}
