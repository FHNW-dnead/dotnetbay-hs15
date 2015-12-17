namespace DotNetBay.Core.Execution
{
    public interface IAuctionRunner
    {
        IAuctioneer Auctioneer { get; }

        void Start();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "No issue here, because only tergating C#")]
        void Stop();

        void RunOnce();
    }
}
