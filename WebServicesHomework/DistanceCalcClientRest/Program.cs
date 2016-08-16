using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceCalcClientRest
{
    using System.Net;

    using RestSharp;

    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("http://localhost:50929");
            var request = new RestRequest("api/DistanceCalc", Method.GET);
            request.AddParameter("x1", 5);
            request.AddParameter("x2", 10);
            request.AddParameter("y1", 5);
            request.AddParameter("y2", 10);

            var response = client.Execute(request);
            var content = response.Content;
            Console.WriteLine(content);

        }
    }
}
