using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CarsBindingList myCarsBindingList;
        private BindingSource carsBindingSource;
        private Dictionary<string, ListSortDirection> sortingMode = new Dictionary<string, ListSortDirection>();
        public MainWindow()
        {
            Out.Clear();
            Controller.LinqStatements();
            Controller.NoLambdaDefinitions();
            Controller.CarsBindingListSearchingAndSorting();

            InitializeComponent();
            myCarsBindingList = new CarsBindingList(Controller.myCars);
            carsBindingSource = new BindingSource();
            DataGridUpdate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // init comboBox
            BindingList<string> comboBoxList = new BindingList<string>();
            PropertyDescriptorCollection carProperties = TypeDescriptor.GetProperties(typeof(Car));
            foreach (PropertyDescriptor carProperty in carProperties)
            {
                sortingMode.Add(carProperty.Name, ListSortDirection.Ascending);
                if (carProperty.PropertyType.Equals(typeof(Engine)))
                {
                    PropertyDescriptorCollection engineProperties = TypeDescriptor.GetProperties(typeof(Engine));
                    foreach (PropertyDescriptor engineProperty in engineProperties)
                    {
                        comboBoxList.Add($"{carProperty.Name}.{engineProperty.Name}");
                    }
                }
                else
                {
                    comboBoxList.Add(carProperty.Name);
                }
            }
            searchInComboBox.ItemsSource = comboBoxList;
            searchInComboBox.SelectedIndex = 0;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            List<Car> searchResult;
            if (!searchForText.Text.Equals(""))
            {
                string property = searchInComboBox.SelectedItem.ToString();
                Int32 searchTextNum;
                if (Int32.TryParse(searchForText.Text, out searchTextNum))
                {
                    searchResult = myCarsBindingList.FindCars(property, searchTextNum);
                }
                else
                {
                    searchResult = myCarsBindingList.FindCars(property, searchForText.Text);
                }
                myCarsBindingList = new CarsBindingList(searchResult);
                DataGridUpdate();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            for (var visual = sender as Visual; visual != null; visual = VisualTreeHelper.GetParent(visual) as Visual)
            {
                if (visual is DataGridRow)
                {
                    var car = (Car)((DataGridRow)visual).Item;

                    myCarsBindingList.Remove(car);
                    Controller.myCars.Remove(car);

                    DataGridUpdate();
                    break;
                }
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            myCarsBindingList = new CarsBindingList(Controller.myCars);
            DataGridUpdate();
        }

        private void DataGridUpdate()
        {
            carsBindingSource.DataSource = myCarsBindingList;
            carsDataGrid.ItemsSource = carsBindingSource;
        }
        private void SortColumn(object sender, RoutedEventArgs e)
        {
            string columnName = ((DataGridColumnHeader)sender).ToString().Split(' ')[1].ToLower();

            myCarsBindingList.Sort(columnName, sortingMode[columnName]);

            sortingMode[columnName] = sortingMode[columnName] == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            DataGridUpdate();
        }
    }
}
