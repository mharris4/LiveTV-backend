using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTV.Services
{
    interface IChannelService
    {
        bool processChannelChangeRequest(int channelNumber);
    }
}
