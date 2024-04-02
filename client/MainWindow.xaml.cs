using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
//using MessageBox = System.Windows.MessageBox;


namespace TCPClient2
{
    /// <summary>
    /// Interaction logic for Client.xaml
    /// </summary>
    /// 

    public partial class Client : Window
    {
        //SimpleTcpClient client = new SimpleTcpClient("127.0.0.1", 9000
        //
        Bitmap Capture; //картинка для передачи 
        public Client()
        {
            InitializeComponent();
            Loaded += Client_Loaded; // Подписка на событие Loaded
        }
        SimpleTcpClient client;
        //Bitmap Capture;

        public object StartTransmissionButton { get; private set; }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            try
            { // Установка соединения с сервером
                client.Connect();
                btnSend.IsEnabled = true;
                btnConnect.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обработчик события загрузки формы
        private void Client_Loaded(object sender, EventArgs e)
        {
            // Инициализация клиента, подписка на события Connected, DataReceived, Disconnected
            client = new SimpleTcpClient(txtIP.Text);
            client.Events.Connected += Events_Connected;
            client.Events.DataReceived += Events_DataReceived;
            client.Events.Disconnected += Events_Disconnected;
            btnSend.IsEnabled = false;

        }
        //this.Invoke -- это означает: "При каждом подключении к серверу,
        //пожалуйста, добавь строку "Server connected."
        //в текстовое поле." Таким образом, мы можем увидеть,
        //когда сервер подключается и был ли успешен этот процесс. 



        // Обработчик события "Disconnected"
        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtInfo.Text += $"Server disconnected.{Environment.NewLine}";
            });
        }


        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtInfo.Text += "Send Photo";

                // Вывод полученных данных от сервера
                //                Capture = (Bitmap)ByteArrayToImage(e.Data.ToArray());
                //                txtInfo.Text += Capture.Height.ToString() + " px";
                //                txtInfo.Text += Capture.Width.ToString() + " px";
                //                pictureBox2.Image = Capture;
                //удаление изображения для освобождения памяти ------> ошибка 
                //Capture.Dispose();


            });
        }

        private void Invoke(MethodInvoker methodInvoker)
        {
            throw new NotImplementedException();
        }


        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            string clientName = textBoxClientName.Text; // Получение имени клиента из TextBox
            client.Send(clientName);
            this.Dispatcher.Invoke(() =>
            {
                txtInfo.Text += $"Server connected.{Environment.NewLine}";
            });
            // Запуск клиента
            client.Connect();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client.IsConnected)
            //для проверки, является ли текст введенным пользователем непустым
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {  // Отправка сообщения серверу
                    client.Send(txtMessage.Text);
                    // Отображение отправленного сообщения
                    txtInfo.Text += $"Me:{txtMessage.Text}{Environment.NewLine}";
                    txtMessage.Text = string.Empty;
                }
            }
        }


        public static byte[] ResizeAndConvertImageToByte(System.Drawing.Image img)
        {
            System.Drawing.Image resizedImg = new System.Drawing.Bitmap(img, new System.Drawing.Size(img.Width / 2, img.Height / 2));
            return ImageToByte2(resizedImg); // Преобразуем уменьшенное изображение в массив байтов
        }

        //преобразование в массив байт
        public static byte[] ImageToByte2(System.Drawing.Image img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
        private void Capture2(Bitmap bm)
        {
            while (true)
            {
                bm = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                Graphics g = Graphics.FromImage(bm);
                g.CopyFromScreen(0, 0, 0, 0, bm.Size);

                byte[] captureBytes2 = ResizeAndConvertImageToByte(bm);
                client.Send(captureBytes2);


                Thread.Sleep(1000);
            }
        }

        private void send_video_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void send_video_Click(object sender, RoutedEventArgs e)
        //{

        //    Bitmap capture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        //    Capture2(capture);
        //    byte[] captureBytes = ResizeAndConvertImageToByte(capture);
        //    client.Send(captureBytes);


        //}

    }


}


