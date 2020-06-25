using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Lab3
{
    [XmlType("car")]
    [XmlRoot("cars")]
    public class Car
    {
        public String model;
        [XmlElement(ElementName = "engine")]
        public Engine motor;
        public int year;

        public Car() { }

        public Car(String model, Engine motor, int year)
        {
            this.model = model;
            this.motor = motor;
            this.year = year;
        }

        public override String ToString()
        {
            return $"Car({this.model}, {this.motor.ToString()}, {this.year})";
        }
    }
}
