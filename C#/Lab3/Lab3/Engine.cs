using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Lab3
{
    [XmlRoot(ElementName = "engine")]
    public class Engine
    {
        public double displacement;
        public double horsePower;
        [XmlAttribute]
        public String model;

        public Engine() { }

        public Engine(double displacement, double horsePower, String model)
        {
            this.displacement = displacement;
            this.horsePower = horsePower;
            this.model = model;
        }

        public override String ToString()
        {
            return $"Engine({this.displacement}, {this.horsePower}, {this.model})";
        }
    }
}
