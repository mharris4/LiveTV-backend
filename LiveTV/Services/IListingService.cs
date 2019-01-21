using LiveTV.Entities;
using LiveTV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveTV.Services
{
    interface IListingService
    {
        ListingInfo getCurrentListings(int hours);
        List<Channels> getAllChannels();
        Programs getProgramById(int id);
    }
}
