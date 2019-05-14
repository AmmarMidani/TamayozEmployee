using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace TamayozEmploteeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                TamayozService.MySetting.CheckLocalFilesSystems();
                TamayozService.MySetting.GetIconList();
                TamayozService.MySetting.GetConstNotifications();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MainWindow: MainWindow: " + ex.Message);
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Login();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: LoginButton_Click: " + ex.Message);
            }
        }

        private async Task Login()
        {
            try
            {
                string mobile = this.txtMobile.Text.Trim(), password = this.txtPassword.Password;
                if (mobile.Length == 0 || password.Length == 0)
                {
                    MyNotifyIcon2.ShowBalloonTip("تحذير", "الرجاء ملأ كافة الحقول", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning);
                    return;
                }
                this.LoginButton.Content = "الرجاء الانتظار";
                this.LoginButton.IsEnabled = false;
                var val = await Task.Run(() => TamayozService.MySetting.PostLoginAPI(mobile, password));
                switch (val["Status"])
                {
                    case "0":
                        MyNotifyIcon2.ShowBalloonTip("خطأ تسجيل الدخول", val["Message"], Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                        this.LoginButton.Content = "تسجيل دخول";
                        this.LoginButton.IsEnabled = true;
                        break;
                    case "1":
                        MyNotifyIcon2.ShowBalloonTip("مرحبا بك", val["Message"], MyNotifyIcon2.Icon, true);
                        PleaseWait pw = new PleaseWait();
                        pw.OnPopup += Pw_OnPopup;
                        pw.Show();
                        HideMe();
                        break;
                    case "2":
                        MyNotifyIcon2.ShowBalloonTip("خطأ تسجيل الدخول", val["Message"], Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                        this.LoginButton.Content = "تسجيل دخول";
                        this.LoginButton.IsEnabled = true;
                        break;
                    default:
                        MyNotifyIcon2.ShowBalloonTip("خطأ اتصال", "حصل خطأ أثناء محاولة الاتصال الرجاء مراجعة المشرف ", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                        this.LoginButton.Content = "تسجيل دخول";
                        this.LoginButton.IsEnabled = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: Login: " + ex.Message);
            }
        }

        private void Pw_OnPopup(object sender, PleaseWait.PopupEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Popup pu = new Popup(
                        e.Result["title"].ToString(),
                        e.Result["content"].ToString(),
                        e.Result["url"].ToString(),
                        e.Result["icon_url"].ToString(),
                        e.Result["notif_id"].ToString()
                    );
                    pu.Show();
                });
                MyNotifyIcon2.ShowBalloonTip(e.Result["title"].ToString(), e.Result["content"].ToString(), MyNotifyIcon2.Icon, true);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: Pw_OnPopup: " + ex.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: MenuItem_Click: " + ex.Message);
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: Rectangle_MouseLeftButtonDown: " + ex.Message);
            }
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                closeMe();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: button_Click_1: " + ex.Message);
            }
        }

        private void closeMe()
        {
            try
            {
                var result = MessageBox.Show("هل تود فعلاً الخروج من التطبيق؟\nلن تصلك الاشعارات بعد الآن", "تحذير", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.OK)
                {
                    var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                    var goout = new DoubleAnimation(-350, (Duration)TimeSpan.FromSeconds(1));
                    var ce = new CircleEase();
                    ce.EasingMode = EasingMode.EaseIn;
                    goout.EasingFunction = ce;
                    anim.Completed += (s, _) => Environment.Exit(0);
                    this.BeginAnimation(UIElement.OpacityProperty, anim);
                    this.BeginAnimation(TopProperty, goout);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: closeMe: " + ex.Message);
            }
        }

        private void HideMe()
        {
            try
            {
                var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                var goout = new DoubleAnimation(-350, (Duration)TimeSpan.FromSeconds(1));
                var ce = new CircleEase();
                ce.EasingMode = EasingMode.EaseIn;
                goout.EasingFunction = ce;
                goout.Completed += (s, _) => this.Hide();
                this.BeginAnimation(UIElement.OpacityProperty, anim);
                this.BeginAnimation(TopProperty, goout);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: HideMe: " + ex.Message);
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtPassword.Password.Trim().Length > 0)
                    this.PPTB.Visibility = Visibility.Hidden;
                else this.PPTB.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: txtPassword_PasswordChanged: " + ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TamayozService.MySetting.GetUserID() != 0)
                {
                    PleaseWait pw = new PleaseWait();
                    pw.OnPopup += Pw_OnPopup;
                    pw.Show();
                    HideMe();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: LoginButton_Click: " + ex.Message);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                TamayozService.MySetting.RemoveAllFile();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MainWindow: closeMe: " + ex.Message);
            }
        }
    }

    public class PasswordBoxMonitor : DependencyObject
    {
        public static bool GetIsMonitoring(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMonitoringProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static readonly DependencyProperty IsMonitoringProperty =
            DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));


        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        public static readonly DependencyProperty PasswordLengthProperty = DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pb = d as System.Windows.Controls.PasswordBox;
            if (pb == null)
            {
                return;
            }
            if ((bool)e.NewValue)
            {
                pb.PasswordChanged += PasswordChanged;
            }
            else
            {
                pb.PasswordChanged -= PasswordChanged;
            }
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var pb = sender as System.Windows.Controls.PasswordBox;
            if (pb == null)
            {
                return;
            }
            SetPasswordLength(pb, pb.Password.Length);
        }

    }
}