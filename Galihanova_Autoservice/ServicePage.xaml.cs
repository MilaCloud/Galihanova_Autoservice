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

        int CountRecords; //Кол-во записей в таблице
        int CountPage;//Общее кол-во страниц
        int CurrentPage = 0;

        List<Service> CurrentPageList = new List<Service>(); //текущий лист, который заносится в таблицу;
        List<Service> TableList; //лист, содержащий все записи, все сортировки/фильтры/поиски применяются к данной переменной
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
                currentServicies = currentServicies.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                currentServicies = currentServicies.OrderBy(p => p.Cost).ToList();
            }
            ServiceListView.ItemsSource = currentServicies;
            TableList = currentServicies;
            ChangePage(0, 0);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //забираем сервис с нажатой кнопкой удалить
            var currentService = (sender as Button).DataContext as Service;

            //Проверка на наличие записей клиентов на услугу
            var currentClientServices = ГалихановаАвтосервисEntities.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientServices.Count != 0)
            {
                MessageBox.Show("Невозможно удалить, так как существуют записи на эту услугу");
            }
            else
            {

                if (MessageBox.Show("Вы точно хотите удалить услугу?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        ГалихановаАвтосервисEntities.GetContext().Service.Remove(currentService);
                        ГалихановаАвтосервисEntities.GetContext().SaveChanges();
                        //Выводим измененную таблицу в листвью
                        ServiceListView.ItemsSource = ГалихановаАвтосервисEntities.GetContext().Service.ToList();
                        //Применяем фильтры и поиск если они были
                        UpdateServicies();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ToString());
                    }
                }
            }
        }

        private void ChangePage (int direction, int? selectedPage)
        {
            //Параметр int direction (направление перелистывания), возможны 3 значения:
            //            0 - начальная загрузка, 
            //1 - предыдущая страница, 
            //2 - следующая страница, 

            CurrentPageList.Clear();
            CountRecords = TableList.Count; //опред колво записй во всем списке

            //колво страниц
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }

            Boolean Ifupdate = true; //Правильность

            int min;

            if (selectedPage.HasValue) //Если не null
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else //нажата стрелка
            {
                switch (direction)
                {
                    case 1: //предыдущая стр
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                    case 2: //следующая стр
                        if (CurrentPage < CountPage - 1) //Если вперед можно
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage *10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }

            if (Ifupdate) //если currentPage не вышел из диапазона
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBALLRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                //обновить отображение списка услуг
                ServiceListView.Items.Refresh();
            }    
        }


        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
            
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }
        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                ГалихановаАвтосервисEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = ГалихановаАвтосервисEntities.GetContext().Service.ToList();
                UpdateServicies();
            }
           
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new SignUpPage((sender as Button).DataContext as Service));
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Manager.MainFrame.Navigate(new AddEditPage());
        //}
    }
}
