using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LiveTV.Models
{
    [DataContractAttribute]
    public class Listing
    {
        [DataMemberAttribute]
        private String channelName;

        [DataMemberAttribute]
        private String channelNumber;

        [DataMemberAttribute]
        private List<ProgramInfo> programs = new List<ProgramInfo>();

        public string ChannelName { get => channelName; set => channelName = value; }
        public string ChannelNumber { get => channelNumber; set => channelNumber = value; }
        public List<ProgramInfo> Programs { get => programs; set => programs = value; }

        public void addProgramInfo(ProgramInfo info) { programs.Add(info); }
    }
}