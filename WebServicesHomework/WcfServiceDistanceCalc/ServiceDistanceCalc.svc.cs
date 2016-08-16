using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServiceDistanceCalc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceDistanceCalc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ServiceDistanceCalc.svc or ServiceDistanceCalc.svc.cs at the Solution Explorer and start debugging.
    public class ServiceDistanceCalc : IServiceDistanceCalc
    {
   
        public double CalcDistance(Point startPoint, Point endPoint)
        {
            var distance = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
            return distance;
        }
    }
}
