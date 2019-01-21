using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveTV.Entities
{
    public class Programs
    {
        public virtual int ProgramID { get; set; }
        public virtual int ChannelID { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime StopTime { get; set; }
        public virtual string Title { get; set; }
        public virtual string Desc { get; set; }
        public virtual string Category { get; set; }
        public virtual string SubTitle { get; set; }
    }
}