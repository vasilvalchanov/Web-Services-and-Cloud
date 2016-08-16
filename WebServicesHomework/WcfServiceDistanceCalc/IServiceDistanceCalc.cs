using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfServiceDistanceCalc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceDistanceCalc" in both code and config file together.
    [ServiceContract]
    public interface IServiceDistanceCalc
    {

        [OperationContract]
        double CalcDistance(Point startPoint, Point endPoint);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class Point
    {
        private int x;

        private int y;

        [DataMember]
        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        [DataMember]
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
    }
}
