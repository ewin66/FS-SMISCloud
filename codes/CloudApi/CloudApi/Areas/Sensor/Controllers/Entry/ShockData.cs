using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Areas.Sensor.Controllers
{
    public class ShockData
    {
        private string _sensorid;
        public string sensorid
        {
            get { return _sensorid; }
            set { _sensorid = value; }
        }

        private string _collecttime;
        public string collecttime
        {
            get { return _collecttime; }
            set { _collecttime = value; }
        }

        private double _x;
        public double x
        {
            get { return _x; }
            set { _x = value; }
        }

        private double _y;
        public double y
        {
            get { return _y; }
            set { _y = value; }
        }

        private double _z;
        public double z
        {
            get { return _z; }
            set { _z = value; }
        }

        private double _t;
        public double t
        {
            get { return _t; }
            set { _t = value; }
        }

        private double _speed;
        public double speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        private string _intensity;
        public string intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        private string _occurrent;
        public string occurrent
        {
            get { return _occurrent; }
            set { _occurrent = value; }
        }

        public double T
        {
            set { _t=value*1000.0; }
            get { return _t / 1000.0; }
        }



        private double _tci;

        public double tci
        {
            get { return _tci; }
            set { _tci = value; }
        }

        private double ri;

        public double Ri
        {
            get { return ri; }
            set { ri = value; }
        }
        private double yi;

        public double Yi
        {
            get { return yi; }
            set { yi = value; }
        }

        public double[] calc(Matrix minMt)
        {
            double[] doubleArray = new double[4];

            doubleArray[0] = -1 * (x - minMt[0,0]) / (speed * Ri);
            doubleArray[1] = -1 * (y - minMt[0,1]) / (speed * Ri);
            doubleArray[2] = -1 * (z - minMt[0,2]) / (speed * Ri);
           
            doubleArray[3] = 1;
            return doubleArray;
        }
        public double[] getArray() 
        {
            double[] doubleArray = new double[4];
            doubleArray[0]=x;
            doubleArray[1]=y;
            doubleArray[2]=z;
            doubleArray[3]=T;
            return doubleArray;
        }
        public bool isCalc { set; get; }
    }
}
