using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class Engine : IComparable
    {
        public double displacement { set; get; }
        public int horsePower { set; get; }
        public string model { set; get; }

        public Engine() { }

        public Engine(double displacement, int horsePower, string model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

        public int CompareTo(Object obj)
        {
            Engine other = (Engine)obj;
            return horsePower.CompareTo(other.horsePower);
        }
        public override string ToString()
        {
            return $"Engine(displacement={displacement}, horsePower={horsePower}, model={model})";
        }
    }
}
