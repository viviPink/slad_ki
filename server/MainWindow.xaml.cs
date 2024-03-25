using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TCPserver
{
    /// <summary>
    /// Interaction logic for Server.xaml
    /// </summary>
    public partial class Server : Window
    {

       //SimpleTcpServer server = new SimpleTcpServer("127.0.0.1", 9000);
        public Server()
        {
            InitializeComponent();
        }
        SimpleTcpServer server;
        //Bitmap Capture; //картинка для передачи 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try { 
            server.Start();
            txtInfo.Text += $"Starting..{Environment.NewLine}";
            btnStart.IsEnabled = false;
            btnSend.IsEnabled = true;
        }
            catch (Exception ex)
    {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error");
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            btnSend.IsEnabled = false;
            server = new SimpleTcpServer(txtIP.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;



        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                // Обновляем текстовое поле txtInfo с IP-портом клиента и полученными данными
                txtInfo.Text += $"{e.IpPort}:{Encoding.UTF8.GetString(e.Data.ToArray())}{Environment.NewLine}";
            });
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
             Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";
                lstClientIP.Items.Add(e.IpPort);
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
    }
}
