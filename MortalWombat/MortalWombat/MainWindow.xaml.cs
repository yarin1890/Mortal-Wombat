using System.Windows;


namespace MortalWombat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Game();
            newWindow.Show();
            this.Hide();
        }

        private void instructions_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Instructions();
            newWindow.Show();
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
