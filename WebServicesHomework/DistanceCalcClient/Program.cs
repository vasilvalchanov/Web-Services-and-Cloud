using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceCalcClient
{
    using DistanceCalcClient.ServiceReferenceDistanceCalc;

    class Program
    {
        static void Main(string[] args)
        {
            ServiceReferenceDistanceCalc.ServiceDistanceCalcClient client = new ServiceDistanceCalcClient();
            Point startPoint = new Point()
                                   {
                                       X = 5,
                                       Y = 4
                                   };

            Point endPoint = new Point()
            {
                X = 22,
                Y = 18
            };

            var result = client.CalcDistance(startPoint, endPoint);
            Console.WriteLine(result);
        }
    }
}
