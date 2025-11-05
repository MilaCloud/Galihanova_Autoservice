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
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private Service _currentService = new Service();
        public SignUpPage(Service SelectedService)
        {
            InitializeComponent();
            if (SelectedService != null)
            {
                this._currentService= SelectedService;
            }
            DataContext = _currentService;

            var _currentClient = ГалихановаАвтосервисEntities.GetContext().Client.ToList();
            ComboClient.ItemsSource = _currentClient;
        }

        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");

            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");

            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if (TBEnd.Text == "")
                errors.AppendLine("Укажите время начала услуги в формате ЧЧ:ММ");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);

            if(_currentClientService.ID == 0)
            {
                ГалихановаАвтосервисEntities.GetContext().ClientService.Add(_currentClientService);
            }

            try
            {
                ГалихановаАвтосервисEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if (s.Length < 3 || !s.Contains(':'))
                TBEnd.Text = "";
            else
            {
                try
                {
                    string[] start = s.Split(new char[] { ':' });
                    int startHour = Convert.ToInt32(start[0].ToString());
                    int startMinute = Convert.ToInt32(start[1].ToString());

                    if (startHour < 0 || startHour > 23 || startMinute <0 || startMinute >=60)
                    {
                        TBEnd.Text = "";
                        return;
                    }

                    int sum = startHour * 60 + startMinute + _currentService.DurationInSeconds;

                    int endHour = sum / 60;
                    int endMinute = sum % 60;

                    if (endHour >= 24) endHour %= 24;

                    s = $"{endHour}:{endMinute:D2}";
                    TBEnd.Text = s;
                }
                catch
                {
                    TBEnd.Text = "";
                }
            }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text + e.Text;

            // Разрешаем только цифры и двоеточие
            if (!char.IsDigit(e.Text, 0) && e.Text != ":")
            {
                e.Handled = true;
                return;
            }

            // Проверяем формат времени
            if (e.Text == ":")
            {
                // Двоеточие можно вводить только один раз
                if (textBox.Text.Contains(':'))
                {
                    e.Handled = true;
                    return;
                }
            }

            if (newText.Length > 5)
            {
                e.Handled = true;
                return;
            }
        }
    }
}
