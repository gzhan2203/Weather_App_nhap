using System.Windows;

namespace Weather
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message)
        {
            InitializeComponent();
            MessageContent.Text = message;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
