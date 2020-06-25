using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Lab3
{
    class Program
    {
        public static List<Car> myCars = new List<Car>(){
                new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
                new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
                new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
                new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
                new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
                new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
                new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
                new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
                new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
            };

        static void Main(string[] args)
        {
            linqQueries();

            serializationAndDeserialization();

            xPathStatements();

            createXmlFromLinq();

            toXhtmlTable();

            modifyCarsCollectionXml();
        }

        private static void linqQueries()
        {
            var myCarToAnonymousTypeQuery = myCars
                .Where(s => s.model.Equals("A6"))
                .Select(car =>
                    new
                    {
                        engineType = String.Compare(car.motor.model, "TDI") == 0 ? "diesel" : "petrol",
                        hppl = car.motor.horsePower / car.motor.displacement,
                    });
            foreach (var elem in myCarToAnonymousTypeQuery)
            {
                Console.WriteLine(elem.ToString());
            }
            Console.WriteLine();

            var groupedQuery = myCarToAnonymousTypeQuery
                .GroupBy(elem => elem.engineType)
                .Select(elem => $"{elem.First().engineType} = {elem.Average(s => s.hppl).ToString()}");

            foreach (var elem in groupedQuery)
            {
                Console.WriteLine(elem);
            }
            Console.WriteLine();
        }

        private static void serializationAndDeserialization()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CarsCollection.xml");
            
            var serializer = new XmlSerializer(typeof(List<Car>), new XmlRootAttribute("cars"));

            var writer = new StreamWriter(filePath);
            serializer.Serialize(writer, myCars);
            writer.Close();

            var deserializedList = new List<Car>();

            var reader = new FileStream(filePath, FileMode.Open);
            deserializedList = (List<Car>)serializer.Deserialize(reader);
            reader.Close();

            Console.WriteLine("Cars from deserialized list: ");
            foreach (var car in deserializedList)
            {
                Console.WriteLine($"{car.ToString()}, ");
            }
            Console.WriteLine();
        }

        private static void xPathStatements()
        {
            var rootNode = XElement.Load("CarsCollection.xml");
            double avgHP = (double)rootNode.XPathEvaluate("sum(//car/engine[@model!=\"TDI\"]/horsePower) div count(//car/engine[@model!=\"TDI\"]/horsePower)");

            Console.WriteLine($"Avg HP = {avgHP}");
            
            var models = rootNode.XPathSelectElements("//car[following-sibling::car/model = model]");
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CarsCollectionUnique.xml");

            var writer = new StreamWriter(filePath);
            foreach (var model in models)
            {
                writer.WriteLine(model);
            }
            writer.Close();
        }

        private static void createXmlFromLinq()
        {
            var nodes = myCars
                .Select(n =>
                new XElement("car",
                    new XElement("model", n.model),
                    new XElement("engine",
                        new XAttribute("model", n.motor.model),
                        new XElement("displacement", n.motor.displacement),
                        new XElement("horsePower", n.motor.horsePower)),
                    new XElement("year", n.year)));
            var rootNode = new XElement("cars", nodes);
            rootNode.Save("CarsCollectionLinq.xml");
        }
        private static void toXhtmlTable()
        {
            var rows = myCars
                .Select(car =>
                new XElement("tr", new XAttribute("style", "border: 2px solid black"),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.model),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.displacement),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.motor.horsePower),
                    new XElement("td", new XAttribute("style", "border: 2px double black"), car.year)));
            var table = new XElement("table", new XAttribute("style", "border: 2px double black"), rows);
            var template = XElement.Load("template.html");
            template.Element("{http://www.w3.org/1999/xhtml}body").Add(table);
            template.Save("table.html");
        }

        private static void modifyCarsCollectionXml()
        {
            var template = XElement.Load("CarsCollection.xml");
            foreach (var car in template.Elements())
            {
                car.Element("engine").Element("horsePower").Name = "hp";
                var yearXElement = car.Element("year");
                car.Element("model").Add(new XAttribute("year", yearXElement.Value));
                yearXElement.Remove();
            }
            template.Save("CarsCollectionModified.xml");
        }
    }
}
