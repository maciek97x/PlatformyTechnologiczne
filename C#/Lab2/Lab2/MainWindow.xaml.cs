using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Lab2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select directory to open" };

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var rootDirectory = new DirectoryInfo(dialog.SelectedPath);
                var root = CreateTreeDirectory(rootDirectory);
                treeView.Items.Clear();
                treeView.Items.Add(root);
            }
        }

        private TreeViewItem CreateTreeDirectory(DirectoryInfo directory)
        {
            var item = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };
            item.ContextMenu = new ContextMenu();
            var menuItemCreate = new MenuItem { Header = "Create" };
            menuItemCreate.Click += new RoutedEventHandler(ItemCreate);
            var menuItemDelete = new MenuItem { Header = "Delete" };
            menuItemDelete.Click += new RoutedEventHandler(ItemDelete);
            item.ContextMenu.Items.Add(menuItemCreate);
            item.ContextMenu.Items.Add(menuItemDelete);

            foreach (var subdir in directory.GetDirectories())
            {
                item.Items.Add(CreateTreeDirectory(subdir));
            }
            foreach (var file in directory.GetFiles())
            {
                item.Items.Add(CreateTreeFile(file));
            }

            item.Selected += new RoutedEventHandler(StatusBarUpdate);
            return item;
        }

        private TreeViewItem CreateTreeFile(FileInfo file)
        {
            var item = new TreeViewItem
            {
                Header = file.Name,
                Tag = file.FullName
            };
            item.ContextMenu = new ContextMenu();
            var menuItemOpen = new MenuItem { Header = "Open" };
            menuItemOpen.Click += new RoutedEventHandler(ItemOpen);
            var menuItemDelete = new MenuItem { Header = "Delete" };
            menuItemDelete.Click += new RoutedEventHandler(ItemDelete);
            item.ContextMenu.Items.Add(menuItemOpen);
            item.ContextMenu.Items.Add(menuItemDelete);

            item.Selected += new RoutedEventHandler(StatusBarUpdate);
            return item;
        }

        private void ItemOpen(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TreeViewItem folder = null;
            if (menuItem != null)
            {
                folder = ((ContextMenu)menuItem.Parent).PlacementTarget as TreeViewItem;
            }
            textBlock.Text = File.ReadAllText((String)folder.Tag);
        }

        private void ItemCreate(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TreeViewItem folder = null;
            if (menuItem != null)
            {
                folder = ((ContextMenu)menuItem.Parent).PlacementTarget as TreeViewItem;
            }
            if (folder != null)
            {
                string path = (string)folder.Tag;
                CreateDialog dialog = new CreateDialog(path);
                var result = dialog.ShowDialog();
                if (result.GetValueOrDefault())
                {
                    if (File.Exists(dialog.path))
                    {
                        FileInfo file = new FileInfo(dialog.path);
                        folder.Items.Add(CreateTreeFile(file));
                    }
                    else if (Directory.Exists(dialog.path))
                    {
                        DirectoryInfo dir = new DirectoryInfo(dialog.path);
                        folder.Items.Add(CreateTreeDirectory(dir));
                    }
                }
            }
        }

        private void ItemDelete(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            TreeViewItem item = null;
            if (menuItem != null)
            {
                item = ((ContextMenu)menuItem.Parent).PlacementTarget as TreeViewItem;
            }
            if (item != null)
            {
                string path = (string)item.Tag;
                FileAttributes attributes = File.GetAttributes(path);
                File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    deleteDirectory(path);
                }
                else
                {
                    File.Delete(path);
                }
                if ((TreeViewItem)treeView.Items[0] != item)
                {
                    TreeViewItem parent = (TreeViewItem)item.Parent;
                    parent.Items.Remove(item);
                }
                else
                {
                    treeView.Items.Clear();
                }
            }
        }

        private void deleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach (var subdir in dir.GetDirectories())
            {
                deleteDirectory(subdir.FullName);
            }
            foreach (var file in dir.GetFiles())
            {
                File.Delete(file.FullName);
            }
            Directory.Delete(path);
        }

        private void StatusBarUpdate(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
            FileAttributes attributes = File.GetAttributes((string)item.Tag);
            statusBarDOSAttr.Text = "";
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                statusBarDOSAttr.Text += 'r';
            }
            else
            {
                statusBarDOSAttr.Text += '-';
            }
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                statusBarDOSAttr.Text += 'a';
            }
            else
            {
                statusBarDOSAttr.Text += '-';
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                statusBarDOSAttr.Text += 'h';
            }
            else
            {
                statusBarDOSAttr.Text += '-';
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                statusBarDOSAttr.Text += 's';
            }
            else
            {
                statusBarDOSAttr.Text += '-';
            }
        }
    }
}
