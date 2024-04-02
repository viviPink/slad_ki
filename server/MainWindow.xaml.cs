using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = System.Windows.Application;
using System.ComponentModel;
using Image = System.Windows.Controls.Image;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using Control = System.Windows.Controls.Control;
using Brushes = System.Windows.Media.Brushes;

namespace TCPserver
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class Server : Window
    {

        SimpleTcpServer server;
        // SimpleTcpServer server = new SimpleTcpServer("127.0.0.1", 9000);
        ObservableCollection<string> addresses = new ObservableCollection<string>();
        List<Image> clientImages = new List<Image>();
        Bitmap Capture;
        private string clientName;
        public Server()
        {
            InitializeComponent();
            lstClientIP.ItemsSource = addresses;
            this.DataContext = this;
            server = new SimpleTcpServer(txtIP.Text); // Создаем экземпляр SimpleTcpServer
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;
        }
        // SimpleTcpServer server;
        //Bitmap Capture; //картинка для передачи 




        //// Метод для создания кнопки клиента и добавления в WrapPanel
        private void AddClientImage(string clientName, string clientIp)
        {
            Image clientImage = new Image();

            bool fullscreen = false;

            clientImage.Name = clientName; // Присваиваем имя клиента как имя элемента Image

            // Создаем объект BitmapImage и устанавливаем его в качестве источника для элемента Image
            BitmapImage bitmapImage = new BitmapImage(new Uri(@"E:\поректттттттттттттттттттттттттт\TCPClient2\6VSQgorLYmU.jpg", UriKind.Absolute));
            clientImage.Source = bitmapImage;

            // Установка желаемых размеров для элемента Image
            clientImage.Width = 120;
            clientImage.Height = 100;



            // Добавляем стиль для кнопки клиента
            Style clientImageStyle = new Style(typeof(Image));
            clientImageStyle.Setters.Add(new Setter(Control.MarginProperty, new Thickness(5))); // Устанавливаем отступы
            clientImageStyle.Setters.Add(new Setter(Control.BorderBrushProperty, Brushes.Gray)); // Устанавливаем серую рамку
            clientImageStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(2))); // Устанавливаем толщину рамки
           // clientImageStyle.Setters.Add(new Setter(Control.CornerRadiusProperty, new CornerRadius(10))); // Устанавливаем закругленные углы
            clientImage.Style = clientImageStyle;


            clientImage.MouseDown += (sender, e) =>
            {
                if (e.ClickCount == 2 && fullscreen == false)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    fullscreen = true;
                }
                else if (e.ClickCount == 2 && fullscreen == true)
                {

                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                    fullscreen = false;
                }
            };

            clientImages.Add(clientImage);
            clientsWrapPanel.Children.Add(clientImage);

            addresses.Add(clientIp); // Добавляем айпи-адрес клиента в коллекцию addresses
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { 
            server.Start();
            txtInfo.Text += $"Starting..{Environment.NewLine}";
            btnStart.IsEnabled = false;
            btnSend.IsEnabled = true;

        }
            catch (Exception ex) {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }


       

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
            
                string receivedData = Encoding.UTF8.GetString(e.Data.ToArray());

                // Проверяем, является ли полученные данные именем клиента
                if (receivedData.StartsWith("CLIENT_NAME:"))
                {
                     clientName = receivedData.Replace("CLIENT_NAME:", "");

                    
                        // Обновляем текстовое поле txtInfo с IP-портом клиента и его именем
                        txtInfo.Text += $"{e.IpPort}: {clientName}{Environment.NewLine}";
                    
                }
                else
                {
                    
                        // Обновляем текстовое поле txtInfo с IP-портом клиента и полученными данными
                        txtInfo.Text += $"{e.IpPort}: {receivedData}{Environment.NewLine}";
                    
                }



            });

        }


        public static Bitmap ByteArrayToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return new Bitmap(ms);
            }
        }

        private void Invoke(MethodInvoker methodInvoker)
        {
            throw new NotImplementedException();
        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                //удаление 
                lstClientIP.Items.Remove(e.IpPort);
            });

        }

        //ошибка при соеденении клиента __ решение: involke метод
        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
           // string clientName = e.textBoxClientName; // Получение переданного из клиента имени

            this.Dispatcher.Invoke(() =>
            {
                txtInfo.Text += $"{clientName} connected.{Environment.NewLine}";
                AddClientImage(clientName, e.IpPort); // Использование имени клиента из клиента
                if (!addresses.Contains(e.IpPort))
                {
                    addresses.Add(e.IpPort);
                }
            });
        }



        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (server.IsListening)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text) && lstClientIP.SelectedItem != null)//proverka soob
                {
                    if (lstClientIP.SelectedItem is string selectedClient) // Преобразуем выбранный элемент в тип string
                    {
                        server.Send(selectedClient, txtMessage.Text);
                        txtInfo.Text += $"Server: {txtMessage.Text}{Environment.NewLine}";
                        txtMessage.Text = string.Empty;

                    }
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnSend.IsEnabled = false;
            //server = new SimpleTcpServer(txtIP.Text);
            //server.Events.ClientConnected += Events_ClientConnected;
            //server.Events.ClientDisconnected += Events_ClientDisconnected;
            //server.Events.DataReceived += Events_DataReceived;

        }
    }
}

