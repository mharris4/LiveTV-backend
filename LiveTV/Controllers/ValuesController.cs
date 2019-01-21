using LiveTV.Entities;
using LiveTV.Models;
using LiveTV.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LiveTV.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public ListingInfo Get()
        {
            ListingService service = new ListingService();
            return service.getCurrentListings(3);
        }

        // GET api/values/5
        public Programs Get(int id)
        {
            ListingService service = new ListingService();
            return service.getProgramById(id);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
