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

namespace Galihanova_Autoservice
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();

            //Загрузить список из бд
            var currentServices = ГалихановаАвтосервисEntities.GetContext().Service.ToList(); //Связать с нашим листвью
            ServiceListView.ItemsSource = currentServices; //добавили строки

            ComboType.SelectedIndex = 0;

            UpdateServicies();
        }

        private void TBoxSearch_TextChanged (object sender, TextChangedEventArgs e)
        {
            UpdateServicies();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServicies();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateServicies();
        }

        private void RButtonDown_Cheked(object sender, RoutedEventArgs e)
        {
            UpdateServicies();
        }
        private void UpdateServicies ()
        {
            var currentServicies = ГалихановаАвтосервисEntities.GetContext().Service.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentServicies = currentServicies.Where(p=> (Convert.ToInt32(p.DiscountInt) >= 0 && Convert.ToInt32(p.DiscountInt) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServicies = currentServicies.Where(p => (Convert.ToInt32(p.DiscountInt) >= 0 && Convert.ToInt32(p.DiscountInt) < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServicies = currentServicies.Where(p => (Convert.ToInt32(p.DiscountInt) >= 5 && Convert.ToInt32(p.DiscountInt) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServicies = currentServicies.Where(p => (Convert.ToInt32(p.DiscountInt) >= 15 && Convert.ToInt32(p.DiscountInt) < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentServicies = currentServicies.Where(p => (Convert.ToInt32(p.DiscountInt) >= 30 && Convert.ToInt32(p.DiscountInt) < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentServicies = currentServicies.Where(p => (Convert.ToInt32(p.DiscountInt) >= 70 && Convert.ToInt32(p.DiscountInt) <= 100)).ToList();
            }

            //Реализуем поиск данный в листвью
            currentServicies = currentServicies.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            //Отображение итогов фильтра и поиска
            ServiceListView.ItemsSource = currentServicies.ToList();

            if (RbuttonDown.IsChecked.Value)
            {
                ServiceListView.ItemsSource = currentServicies.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                ServiceListView.ItemsSource= currentServicies.OrderBy(p => p.Cost).ToList();
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Manager.MainFrame.Navigate(new AddEditPage());
        //}
    }
}
