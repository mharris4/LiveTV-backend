using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LiveTV.Models
{
    [DataContractAttribute]
    public class ProgramInfo
    {
        [DataMemberAttribute]
        private long programID;

        [DataMemberAttribute]
        private string title;

        [DataMemberAttribute]
        private string subTitle;

        [DataMemberAttribute]
        private int timeRangePercent;

        [DataMemberAttribute]
        private string categoryColor;

        public long ProgramID { get => programID; set => programID = value; }
        public string Title { get => title; set => title = value; }
        public string SubTitle { get => subTitle; set => subTitle = value; }
        public int TimeRangePercent { get => timeRangePercent; set => timeRangePercent = value; }
        public string CategoryColor { get => categoryColor; set => categoryColor = value; }
    }
}