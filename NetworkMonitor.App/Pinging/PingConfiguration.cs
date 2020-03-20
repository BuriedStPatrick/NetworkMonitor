namespace NetworkMonitor.App.Pinging
{
    public interface IPingConfiguration
    {
        string[] Addresses { get; set; }
    }

    public class PingConfiguration : IPingConfiguration
    {
        public string[] Addresses { get; set; }

        public PingConfiguration()
        {
            Addresses = new string [] {};
        }
    }
}
