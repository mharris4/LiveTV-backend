using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace LiveTV.Models
{
    [DataContractAttribute]
    public class ListingInfo
    {
        [DataMemberAttribute]
        List<string> listingOffsets = new List<string>();

        [DataMemberAttribute]
        List<Listing> listings = new List<Listing>();

        public List<string> ListingOffsets { get => listingOffsets; set => listingOffsets = value; }
        public List<Listing> Listings { get => listings; set => listings = value; }

        public void addListingOffset(String offset)
        {
            listingOffsets.Add(offset);
        }
    }
}