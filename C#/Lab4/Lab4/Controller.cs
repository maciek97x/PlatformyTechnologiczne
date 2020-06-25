using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lab4
{
    class Controller
    {
        private delegate int CompareCarsHorsePowerDelegate(Car car1, Car car2);
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
        public static void LinqStatements()
        {
            var methodBasedSyntaxQuery = myCars
                .Where(s => s.model.Equals("A6"))
                .Select(car =>
                    new
                    {
                        engineType = String.Compare(car.motor.model, "TDI") == 0
                            ? "diesel"
                            : "petrol",
                        hppl = car.motor.horsePower / car.motor.displacement,
                    })
                    .GroupBy(elem => elem.engineType)
                    .Select(elem => new
                    {
                        name = elem.First().engineType.ToString(),
                        value = elem.Average(s => s.hppl).ToString()
                    })
                    .OrderByDescending(t => t.value)
                    .Select(elem => $"{elem.name} = {elem.value}");

            var queryExpresionSyntax = from elem
                                       in (from car in myCars
                                           where car.model.Equals("A6")
                                           select new
                                           {
                                               engineType = String.Compare(car.motor.model, "TDI") == 0
                                                           ? "diesel"
                                                           : "petrol",
                                               hppl = car.motor.horsePower / car.motor.displacement,
                                           })
                                       group elem by elem.engineType into elemGrouped
                                       select new
                                       {
                                           name = elemGrouped.First().engineType.ToString(),
                                           value = elemGrouped.Average(s => s.hppl).ToString()
                                       } into elemSelected
                                       orderby elemSelected.value descending
                                       select $"{elemSelected.name} = {elemSelected.value}";


            Out.Write("______________________________________________________________________________");
            Out.Write("a) query expression syntax");
            Out.Write(queryExpresionSyntax);

            Out.Write("______________________________________________________________________________");
            Out.Write("b) method-based query syntax");
            Out.Write(methodBasedSyntaxQuery);
        }

        public static void NoLambdaDefinitions()
        {
            List<Car> myCarsCopy = new List<Car>(myCars);
            CompareCarsHorsePowerDelegate arg1 = CompareCarsPowers;
            Predicate<Car> arg2 = IsTDI;
            Action<Car> arg3 = ShowElementsInMessageBox;

            myCarsCopy.Sort(new Comparison<Car>(arg1));

            Out.Write("______________________________________________________________________________");
            Out.Write("myCars sorted by horsePower desc");
            Out.Write(myCarsCopy);

            myCarsCopy.FindAll(arg2).ForEach(arg3);
        }
        private static int CompareCarsPowers(Car car1, Car car2)
        {
            return car1.motor.horsePower - car2.motor.horsePower;
        }

        private static bool IsTDI(Car car)
        {
            return car.motor.model.Equals("TDI");
        }

        private static void ShowElementsInMessageBox(Car car)
        {
            MessageBox.Show(car.ToString(), "Car", MessageBoxButton.OK);
        }

        public static void CarsBindingListSearchingAndSorting()
        {
            CarsBindingList myCarsBindingList = new CarsBindingList(myCars);

            // sorting
            myCarsBindingList.Sort("year", System.ComponentModel.ListSortDirection.Ascending);
            Out.Write("______________________________________________________________________________");
            Out.Write("sorted by year");
            Out.Write(myCarsBindingList);

            // searching
            Out.Write("______________________________________________________________________________");
            Out.Write("searching year = 2012");
            var searchResult = myCarsBindingList.FindCars("year", 2012);
            if (searchResult != null)
            {
                Out.Write(searchResult);
            }
            else Out.Write("null");

            Out.Write("______________________________________________________________________________");
            Out.Write("searching motor.model TFSI");
            searchResult = myCarsBindingList.FindCars("motor.model", "TFSI");
            if (searchResult != null)
            {
                Out.Write(searchResult);
            }
            else
            {
                Out.Write("null");
            }
        }
    }
}
