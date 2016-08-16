using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DistanceCalcRestService.Controllers
{
    public class DistanceCalcController : ApiController
    {
        // GET: api/DistanceCalc
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/DistanceCalc/5
        public IHttpActionResult Get(int x1, int x2, int y1, int y2)
        {
            var distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return this.Ok(distance);
        }

        // POST: api/DistanceCalc
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/DistanceCalc/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/DistanceCalc/5
        public void Delete(int id)
        {
        }
    }
}
