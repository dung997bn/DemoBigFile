using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoBigFile.Hubs
{
    public class DocumentHub : Hub
    {
        public async Task SendStatus(int total, int current)
        {
            await Clients.All.SendAsync("ReceiveStatus", total, current);
        }
    }
}
