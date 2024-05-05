using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Runtime.InteropServices;
using AutoControl.Properties;
using NotifyIcon = System.Windows.Forms.NotifyIcon;
using MessageBox = System.Windows.MessageBox;
namespace AutoControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool check ;
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeNotifyIcon();
        }

        private void text_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String txtname;
                TextBlock x = (TextBlock)sender;
                txtname = x.Name;
                TextBox txt = (TextBox)FindName(txtname.Replace("text", "txt"));
                txt.Focus();
            }
            catch (Exception ex)
            {
            }
        }
        private void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                String textname;
                TextBox txt = (TextBox)sender;
                textname = txt.Name;
                TextBlock text = (TextBlock)FindName(textname.Replace("txt", "text"));

                if (!string.IsNullOrEmpty(txt.Text) && txt.Text.Length > 0)
                {
                    text.Visibility = Visibility.Hidden;
                }
                else
                {
                    text.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(@"assest\img\icons8-off-48.ico"); 
            notifyIcon.Visible = true;
            notifyIcon.Text = "AC";
            notifyIcon.Click += NotifyIcon_Click;
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }
        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            notifyIcon.Visible = true;
        }
       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string username = Settings.Default.Username;
            txtUsername.Text = username;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {
            }   
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (rbtnHibernet.IsChecked == false && rbtnShutdown.IsChecked == false)
            {
                MessageBox.Show("لطفا نوع عملیات را انتخاب کنید");
                return;
            }
            check = true;
            ChechStatus();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            check = false;
        }

        private async void ChechStatus()
        {
         
            try
            {
                lblStatus.Content = "Status : Cheking Status";
                var url = "https://autocontrol.freehost.io/getStatus.php";
                while (true)
                {
                    if (check == false)
                    {
                        lblStatus.Content = "Status : Stop Check";
                        return;
                    }
                    using (var httpClient = new HttpClient())
                    {
                        var formData = new FormUrlEncodedContent(new[]
                        {
                    new KeyValuePair<string, string>("username", txtUsername.Text),
                    new KeyValuePair<string, string>("password", txtPassword.Text)
                });

                        // ارسال درخواست POST و دریافت پاسخ
                        var response = await httpClient.PostAsync(url, formData);

                        // خواندن و چاپ پاسخ
                        var responseContent = await response.Content.ReadAsStringAsync();
                        if (responseContent == "1")
                        {
                            check = false;
                            Reset();
                        }
                        else if (responseContent == "User not found or incorrect password.")
                        {
                            check = false;
                            MessageBox.Show(responseContent);
                        }
                    }
                }
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                MessageBoxResult x =  MessageBox.Show($"Problem sending request: {ex.Message} , مشکلی در اینترنت وجود دارد. برای بررسی مجدد کلید اوکی و برای لغو کلید کنسل را بزنید","error",MessageBoxButton.OKCancel,MessageBoxImage.Error);
                if (x == MessageBoxResult.OK)
                {
                    ChechStatus();
                }
                else
                {
                    check = false;
                    return;
                }
            }
        }
        private async void Reset()
        {
           
            try
            {
                lblStatus.Content = "Status : Reset";

                var url = "https://autocontrol.freehost.io/reset.php"; 

                using (var httpClient = new HttpClient())
                {
                    var formData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("email", txtUsername.Text), 
                    new KeyValuePair<string, string>("password", txtPassword.Text)  
                });

                    // ارسال درخواست POST و دریافت پاسخ
                    var response = await httpClient.PostAsync(url, formData);

                    // خواندن و چاپ پاسخ
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent == "Reset")
                    {
                        await Task.Delay(4000);
                        ShutDown();
                    }
                    else
                    {
                        MessageBox.Show("Not Reset");
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult x = MessageBox.Show($"Problem sending request: {ex.Message} , مشکلی در اینترنت وجود دارد. برای بررسی مجدد کلید اوکی و برای لغو کلید کنسل را بزنید", "error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (x == MessageBoxResult.OK)
                {
                    Reset();
                }
                else
                {
                    return;
                }
            }
        }
        // وارد کردن اطلاعات مربوط به API ویندوز
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // ثابت‌های مورد نیاز برای ارسال دستورات به کیبورد
        const int VK_LWIN = 0x5B; // کد کلید ویندوز سمت چپ
        const int VK_X = 0x58; // کد کلید X
        const int VK_U = 0x55; // کد کلید U
        const int VK_H = 0x48; // کد کلید H
        const int KEYEVENTF_KEYUP = 0x0002; // کلید را آزاد کنید بعد از فشرده شدن آن

        private void ShutDown()
        {
            if (checkbox_SaveInfo.IsChecked == true)
            {
                Settings.Default.Username = txtUsername.Text;
                Settings.Default.Save();
            }
            try
            {
                if (rbtnHibernet.IsChecked == true)
                {
                    // فشردن کلید ویندوز
                    keybd_event((byte)VK_LWIN, 0, 0, UIntPtr.Zero);
                    // فشردن کلید X
                    keybd_event((byte)VK_X, 0, 0, UIntPtr.Zero);

                    keybd_event((byte)VK_X, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                    System.Threading.Thread.Sleep(200);

                    keybd_event((byte)VK_U, 0, 0, UIntPtr.Zero);
                    keybd_event((byte)VK_U, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                    System.Threading.Thread.Sleep(200);

                    keybd_event((byte)VK_H, 0, 0, UIntPtr.Zero);
                    keybd_event((byte)VK_H, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
                else
                {
                    // فشردن کلید ویندوز
                    keybd_event((byte)VK_LWIN, 0, 0, UIntPtr.Zero);
                    // فشردن کلید X
                    keybd_event((byte)VK_X, 0, 0, UIntPtr.Zero);

                    keybd_event((byte)VK_X, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                    keybd_event((byte)VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                    System.Threading.Thread.Sleep(200);

                    keybd_event((byte)VK_U, 0, 0, UIntPtr.Zero);
                    keybd_event((byte)VK_U, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                    System.Threading.Thread.Sleep(200);

                    keybd_event((byte)VK_H, 0, 0, UIntPtr.Zero);
                    keybd_event((byte)VK_H, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 0 /f");
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                Process.Start(psi);
            }
        }
        private async void Reset2()
        {
           
            try
            {
                lblStatus.Content = "Status : Reset";

                var url = "https://autocontrol.freehost.io/reset.php"; // آدرس کامل کد PHP خود را اینجا قرار دهید

                // ایجاد یک instance از HttpClient
                using (var httpClient = new HttpClient())
                {
                    // ایجاد داده‌های فرم برای ارسال
                    var formData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("email", txtUsername.Text), // جایگزین your_username با نام کاربری موردنظر خود کنید
                    new KeyValuePair<string, string>("password", txtPassword.Text)  // جایگزین your_password با رمز عبور موردنظر خود کنید
                });

                    // ارسال درخواست POST و دریافت پاسخ
                    var response = await httpClient.PostAsync(url, formData);

                    // خواندن و چاپ پاسخ
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (responseContent == "Reset")
                    {
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Not Reset");
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBoxResult x = MessageBox.Show($"Problem sending request: {ex.Message} , مشکلی در اینترنت وجود دارد. برای بررسی مجدد کلید اوکی و برای لغو کلید کنسل را بزنید", "error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (x == MessageBoxResult.OK)
                {
                    Reset2();
                }
                else
                {
                    return;
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Text.Trim() == "")
            {
                MessageBoxResult Result;
                Result =  MessageBox.Show("insert Password","Warning",MessageBoxButton.OKCancel,MessageBoxImage.Stop);
                if (Result== MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    this.Close();
                }
            }
            if (txtUsername.Text.Trim() == "")
            {
                MessageBoxResult Result;
                Result = MessageBox.Show("insert Username", "Warning", MessageBoxButton.OKCancel, MessageBoxImage.Stop);
                if (Result == MessageBoxResult.OK)
                {
                    return;
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                Reset2();
            }
        }

        private void btnMore_Click(object sender, RoutedEventArgs e)
        {
            if (More.Visibility ==Visibility.Visible)
            {
                More.Visibility = Visibility.Hidden;
            }
            else
            {
                More.Visibility = Visibility.Visible;
            }
        }
    }
}
