using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    class CarsBindingList : BindingList<Car>
    {
        private ArrayList selectedIndices;
        private PropertyDescriptor sortPropertyValue;
        private ListSortDirection sortDirectionValue;
        private bool isSortedValue = false;

        public CarsBindingList(List<Car> cars)
        {
            foreach (var car in cars)
            {
                Add(car);
            }
        }

        protected override bool SupportsSortingCore { get { return true; }}

        protected override PropertyDescriptor SortPropertyCore { get { return sortPropertyValue; } }

        protected override bool IsSortedCore { get { return isSortedValue; } }

        protected override bool SupportsSearchingCore { get { return true; } }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            var sortedList = new ArrayList();
            var unsortedList = new ArrayList(Count);
            if (prop.PropertyType.GetInterface("IComparable") != null)
            {
                sortPropertyValue = prop;
                sortDirectionValue = direction;

                foreach (var car in Items)
                {
                    if (!sortedList.Contains(prop.GetValue(car)))
                    {
                        sortedList.Add(prop.GetValue(car));
                    }

                }
                sortedList.Sort();
                if (direction == ListSortDirection.Descending)
                {
                    sortedList.Reverse();
                }

                for (int i = 0; i < sortedList.Count; i++)
                {
                    var indices = FindIndices(prop.Name, sortedList[i]);
                    if (indices != null)
                    {
                        foreach (var index in indices)
                        {
                            unsortedList.Add(Items[index]);
                        }
                    }
                }

                if (unsortedList != null)
                {
                    Clear();
                    isSortedValue = true;
                    foreach (Car elem in unsortedList)
                    {
                        Add(elem);
                    }
                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }

        public void Sort(string propertyName, ListSortDirection direction)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(Car));
            PropertyDescriptor property = properties.Find(propertyName, true);
            if (property != null)
            {
                ApplySortCore(property, direction);
            }
            else
            {
                throw new NotSupportedException($"Cannot sort by {propertyName}.");
            }
        }

        private int FindCore(PropertyDescriptor property, object key, bool isEngine)
        {
            PropertyInfo propertyInfo;
            if (isEngine)
            {
                propertyInfo = typeof(Engine).GetProperty(property.Name);
            }
            else
            {
                propertyInfo = typeof(Car).GetProperty(property.Name);
            }
            selectedIndices = new ArrayList();
            int found = -1;

            if (key != null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (isEngine)
                    {
                        if (Double.TryParse(key.ToString(), out double d))
                        {
                            if (propertyInfo.GetValue(Items[i].motor, null).Equals(Double.Parse(key.ToString())))
                            {
                                selectedIndices.Add(i);
                                ++found;
                            }
                        }
                        else
                        {
                            if (propertyInfo.GetValue(Items[i].motor, null).Equals(key))
                            {
                                selectedIndices.Add(i);
                                ++found;
                            }
                        }

                    }
                    else
                    {
                        if (propertyInfo.GetValue(Items[i], null).Equals(key))
                        {
                            selectedIndices.Add(i);
                            ++found;
                        }
                    }
                }
            }
            return found;
        }

        public int[] FindIndices(string propertyName, object key)
        {
            PropertyDescriptorCollection properties;
            bool isEngine = propertyName.Contains("motor.");
            if (isEngine)
            {

                properties = TypeDescriptor.GetProperties(typeof(Engine));
                propertyName = propertyName.Split('.').Last();
            }
            else
            {
                properties = TypeDescriptor.GetProperties(typeof(Car));
            }
            PropertyDescriptor property = properties.Find(propertyName, true);
            if (property != null)
            {
                if (FindCore(property, key, isEngine) >= 0)
                {
                    return (int[])(selectedIndices.ToArray(typeof(int)));
                }
            }
            return null;
        }

        public List<Car> FindCars(string propertyName, object key)
        {
            var findResult = new List<Car>();
            int[] indices = FindIndices(propertyName, key);
            if (indices != null)
            {
                foreach (var index in FindIndices(propertyName, key))
                {
                    findResult.Add(Items[index]);
                }
                return findResult;
            }
            else return null;
        }
    }
}
