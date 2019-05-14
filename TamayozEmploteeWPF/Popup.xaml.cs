using System;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace TamayozEmploteeWPF
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private string _url;
        private string _title;
        private string _content;
        private string _icon;
        private string _notifID;

        public Popup(string Title, string Content, string URL, string Icon, string NotificationID)
        {
            InitializeComponent();
            try
            {
                this._title = Title;
                this._content = Content;
                this._url = URL;
                this._icon = Icon;
                this._notifID = NotificationID;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Popup: Popup: " + ex.Message);
            }
        }

        private void ShowUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(_url);
                closeMe();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Popup: ShowUrl_Click: " + ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = _title;
                this.txtContent.Text = this._content;
                this.txtTitle.Text = _title;
                this.PictureBox.Source = new BitmapImage(new Uri(TamayozService.MySetting.GetIconFromURL(_icon)));
                this.textBox.Text = _notifID;
            }
            catch (Exception)
            {
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
                //MessageBox.Show("Popup: Rectangle_MouseLeftButtonDown: " + ex.Message);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                closeMe();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Popup: button_Click: " + ex.Message);
            }
        }

        private void closeMe()
        {
            try
            {
                var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                var goout = new DoubleAnimation(-226, (Duration)TimeSpan.FromSeconds(1));
                var ce = new CircleEase();
                ce.EasingMode = EasingMode.EaseIn;
                goout.EasingFunction = ce;
                anim.Completed += (s, _) => this.Close();
                this.BeginAnimation(UIElement.OpacityProperty, anim);
                this.BeginAnimation(TopProperty, goout);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Popup: closeMe: " + ex.Message);
            }
        }
    }
}