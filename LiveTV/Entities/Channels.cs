using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveTV.Entities
{
    public class Channels
    {
        public virtual long ChannelID { get; set; }
        public virtual string SDName { get; set; }
        public virtual string DisplayName { get; set; }
    }
}