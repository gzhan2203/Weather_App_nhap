using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;

namespace Weather
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> ChatMessages { get; set; }
        private const string APIKey = "c9fbe5aa93b282c4776082f04710695a";

        public MainWindow()
        {
            InitializeComponent();
            ChatMessages = new ObservableCollection<string>();
        }

        private void MessageInput_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(MessageInput.Text))
            {

            }
        }
        private void MessageInput_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void MessageInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            // Check if city name is provided
            if (!string.IsNullOrWhiteSpace(txt_City.Text))
            {
                getWeather(); // Call the method to fetch and display weather data
            }
            else
            {
                MessageBox.Show("Please enter a city name.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {

                ChatMessagesDisplay.AppendText($"{MessageInput.Text}\n");

                MessageInput.Clear();

                ChatMessagesDisplay.ScrollToEnd();
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void textSearch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txt_Search.Focus();
        }

        private void txtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_Search.Text) && txt_Search.Text.Length > 0)
                txt_Search.Visibility = Visibility.Collapsed;
            else
                txt_Search.Visibility = Visibility.Visible;
        }

        private void txt_Search_GotFocus(object sender, RoutedEventArgs e)
        {
            // Clear the placeholder text when the TextBox gains focus
            if (txt_Search.Text == "Search city...")
            {
                txt_Search.Text = "";
                txt_Search.Foreground = Brushes.White; // Change color to actual input color
            }
        }

        private void txt_Search_LostFocus(object sender, RoutedEventArgs e)
        {
            // Restore the placeholder text if the TextBox is empty when it loses focus
            if (string.IsNullOrWhiteSpace(txt_Search.Text))
            {
                txt_Search.Text = "Search city...";
                txt_Search.Foreground = Brushes.Gray; // Set placeholder color
            }
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Sign in logic here
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Button click logic here
        }

        private void MusicButton_Click(object sender, RoutedEventArgs e)
        {
            // Music button logic here
        }

        private static readonly HttpClient client = new HttpClient(); // Đặt HttpClient làm biến tĩnh
        private async void ChuongButton_Click(object sender, RoutedEventArgs e)
        {
            string location = txt_Search.Text?.Trim();
            if (string.IsNullOrEmpty(location))
            {
                new CustomMessageBox("Please enter a city name.").ShowDialog();
                return;
            }

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={APIKey}&units=metric";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    new CustomMessageBox($"Error: {response.StatusCode} - {response.ReasonPhrase}").ShowDialog();
                    return;
                }

                var weatherData = await response.Content.ReadFromJsonAsync<WeatherInfo.Root>();
                if (weatherData != null)
                {
                    string detailedMessage = "";

                    // Thêm kiểm tra nhiệt độ thấp
                    if (weatherData.main.temp < 15)
                    {
                        detailedMessage += "Cold weather detected. Please wear warm clothes.\n";
                    }

                    if (weatherData.main.temp > 30)
                    {
                        detailedMessage += "High UV index. Please wear sunscreen.\n";
                    }

                    if (weatherData.wind.speed > 15)
                    {
                        detailedMessage += "Strong winds. Please be cautious when outdoors.\n";
                    }

                    if (string.IsNullOrEmpty(detailedMessage))
                    {
                        detailedMessage = "No severe weather warnings. It's safe.";
                    }

                    new CustomMessageBox(detailedMessage).ShowDialog();
                }
                else
                {
                    new CustomMessageBox("No data found for the specified city.").ShowDialog();
                }
            }
            catch (Exception ex)
            {
                new CustomMessageBox($"Unable to retrieve weather information. Error: {ex.Message}").ShowDialog();
            }
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Tab selection logic here
        }

        private void getWeather()
        {
            try
            {
                using (var web = new WebClient())
                {
                    FetchAndDisplayCurrentWeather(web);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void FetchAndDisplayCurrentWeather(WebClient web)
        {
            string urlCurrent = $"https://api.openweathermap.org/data/2.5/weather?q={txt_Search.Text}&appid={APIKey}";
            string jsonCurrent = web.DownloadString(urlCurrent);
            var currentWeather = JsonConvert.DeserializeObject<WeatherInfo.Root>(jsonCurrent);

            //pictureBox1.ImageLocation = $"https://openweathermap.org/img/wn/{currentWeather.weather[0].icon}@2x.png";
            //lblCondition.Text = currentWeather.weather[0].main;
            txt_Detail.Text = $"Hello! How are you doing? The weather today has {currentWeather.weather[0].description}";
            //txt_Humidity
            double tempCelsius = currentWeather.main.temp - 273.15;
            txt_Sunrise.Text = UnixTimeToDateTime(currentWeather.sys.sunset).ToString("hh:mm tt", CultureInfo.InvariantCulture);
            txt_Sunset.Text = UnixTimeToDateTime(currentWeather.sys.sunrise).ToString("hh:mm tt", CultureInfo.InvariantCulture);
            txt_Pressure.Text = $"{currentWeather.main.pressure}";
            txt_icon_WindSpeed.Text = $"{$"{currentWeather.wind.speed} m/s"}";
            txt_WindSpeed.Text = $"{currentWeather.wind.speed}";
            txt_Temp.Text = $"{tempCelsius:F1}°C";
            txt_main_temp.Text = $"{tempCelsius:F1}°C";
            txt_icon_Humidity.Text = $"{currentWeather.main.humidity} %";
            txt_icon_Cloud.Text = $"{currentWeather.clouds.all} %";
            txt_Humidity.Text = $"{currentWeather.main.humidity}";
        }

        private static DateTime UnixTimeToDateTime(long unixTime) =>
            DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;

        internal class WeatherInfo
        {
            public class Coord
            {
                public double lon { get; set; }
                public double lat { get; set; }
            }

            public class Weather
            {
                public string main { get; set; }
                public string description { get; set; }
                public string icon { get; set; }
            }

            public class Main
            {
                public double temp { get; set; }
                public double pressure { get; set; }
                public double humidity { get; set; }
            }

            public class Wind
            {
                public double speed { get; set; }
            }

            public class Sys
            {
                public long sunrise { get; set; }
                public long sunset { get; set; }
            }

            public class Root
            {
                public Coord coord { get; set; }
                public List<Weather> weather { get; set; }
                public Main main { get; set; }
                public Wind wind { get; set; }
                public Sys sys { get; set; }

                public Clouds clouds { get; set; }
            }
        }

        internal class ForecastRoot
        {
            public List<ForecastList> list { get; set; }
        }

        internal class ForecastList
        {
            public long dt { get; set; }
            public Main main { get; set; }
            public Clouds clouds { get; set; }
            public Wind wind { get; set; }
            public Sys sys { get; set; }
            public string dt_txt { get; set; }
        }

        internal class Main
        {
            public double temp { get; set; }
            public double humidity { get; set; }
        }

        internal class Clouds
        {
            public int all { get; set; }
        }

        internal class Wind
        {
            public double speed { get; set; }
        }

        internal class Sys
        {
            public string pod { get; set; }
        }
    }
}