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

        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(@"assest\img\icons8-off-48.ico"); // مسیر آیکون برنامه‌ی‌تان را اینجا قرار دهید
            notifyIcon.Visible = true;
            notifyIcon.Text = "AC";
            notifyIcon.Click += NotifyIcon_Click;
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // نمایش پنجره اصلی در صورت مخفی بودن
            this.Show();
            this.WindowState = WindowState.Normal;
        }
        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            // مخفی کردن پنجره اصلی
            this.Hide();
            // نمایش آیکون در منوی hidden icons
            notifyIcon.Visible = true;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // مخفی کردن پنجره اصلی به جای بسته شدن
            e.Cancel = true;
            this.Hide();
            // نمایش آیکون در منوی hidden icons
            notifyIcon.Visible = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string username = Settings.Default.Username;
            txtUsername.Text = username;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Reset2();
        }

        private async  void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (saveinfo .IsChecked == true)
            {
                Settings.Default.Username = txtUsername.Text;
                Settings.Default.Save();
            }
            check = true;

            try
            {
                lblStatus.Content = "Status : Cheking Status";
                var url = "http://autocontrol.freehost.io/getStatus.php"; // آدرس کامل کد PHP خود را اینجا قرار دهید
                while (true)
                {
                    if (check == false)
                    {
                        lblStatus.Content = "Status : Stop Check";
                        return;
                    }
                    // ایجاد یک instance از HttpClient
                    using (var httpClient = new HttpClient())
                    {
                        // ایجاد داده‌های فرم برای ارسال
                        var formData = new FormUrlEncodedContent(new[]
                        {
                    new KeyValuePair<string, string>("username", txtUsername.Text), // جایگزین your_username با نام کاربری موردنظر خود کنید
                    new KeyValuePair<string, string>("password", psbPassword.Password)  // جایگزین your_password با رمز عبور موردنظر خود کنید
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
                        else if(responseContent == "User not found or incorrect password.")
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
                MessageBox.Show($"Problem sending request: {ex.Message}");
            }
        }
        private async void Reset()
        {
            try
            {
                lblStatus.Content = "Status : Reset";

                var url = "http://autocontrol.freehost.io/reset.php"; // آدرس کامل کد PHP خود را اینجا قرار دهید

                // ایجاد یک instance از HttpClient
                using (var httpClient = new HttpClient())
                {
                    // ایجاد داده‌های فرم برای ارسال
                    var formData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("email", txtUsername.Text), // جایگزین your_username با نام کاربری موردنظر خود کنید
                    new KeyValuePair<string, string>("password", psbPassword.Password)  // جایگزین your_password با رمز عبور موردنظر خود کنید
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
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Trouble resetting: {ex.Message}");
            }
        }
        private async void Reset2()
        {
            try
            {
                lblStatus.Content = "Status : Reset";

                var url = "http://autocontrol.freehost.io/reset.php"; // آدرس کامل کد PHP خود را اینجا قرار دهید

                // ایجاد یک instance از HttpClient
                using (var httpClient = new HttpClient())
                {
                    // ایجاد داده‌های فرم برای ارسال
                    var formData = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("email", txtUsername.Text), // جایگزین your_username با نام کاربری موردنظر خود کنید
                    new KeyValuePair<string, string>("password", psbPassword.Password)  // جایگزین your_password با رمز عبور موردنظر خود کنید
                });

                    // ارسال درخواست POST و دریافت پاسخ
                    var response = await httpClient.PostAsync(url, formData);

                    // خواندن و چاپ پاسخ
                    var responseContent = await response.Content.ReadAsStringAsync();
                   
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Trouble resetting: {ex.Message}");
            }
        }

        private void ShutDown()
        {
            try
            {
                Process[] processes = Process.GetProcesses();


                foreach (Process process in processes)
                {
                    // چک کردن این که آیا برنامه دارای پنجره‌ای قابل دیدن است
                    if (!String.IsNullOrEmpty(process.MainWindowTitle))
                    {
                        process.Kill();
                    }
                }

                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t 10");
                psi.CreateNoWindow = true; // بدون نمایش پنجره خط فرمان
                psi.UseShellExecute = false; // استفاده از محیط غیرهمزمان
                Process.Start(psi); // اجرای دستورات خاموش کردن سیستم
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Trouble shutting down: {ex.Message}");
            }
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            check = false;
        }
    }
}
