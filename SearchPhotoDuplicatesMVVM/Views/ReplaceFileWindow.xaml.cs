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
using System.Windows.Shapes;
using SearchPhotoDuplicatesMVVM.Models;
using SearchPhotoDuplicatesMVVM.ViewModels;

namespace SearchPhotoDuplicatesMVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для ReplaceFileWindow.xaml
    /// </summary>
    public partial class ReplaceFileWindow : Window
    {
        public ReplaceFileWindow()
        {
            InitializeComponent();
            DataContext = new ReplaceFileViewModel();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; this.Close();
        }
    }
}
