using Microsoft.AspNetCore.SignalR;

namespace cuteDVDNet.SignalR
{
    public class HubDiskErr : Hub
    {
        public async Task SendDiskError(string message)
        {
            await Clients.All.SendAsync(
                "DiskError",
                message);
        }
    }
}
