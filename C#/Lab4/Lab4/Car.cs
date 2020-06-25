using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class Car
    {
        public string model { get; set; }
        public Engine motor { get; set; }
        public int year { get; set; }

        public Car()
        {
            motor = new Engine();
        }

        public Car(string model, Engine motor, int year)
        {
            this.model = model;
            this.motor = motor;
            this.year = year;
        }

        public override string ToString()
        {
            return $"Car(model={model}, motor={motor}, year={year})";
        }
    }
}
