using System.Windows;
using EnglishLearningApp.ViewModels;

namespace EnglishLearningApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is MainViewModel vm)
                vm.OnWindowClosing();
        }
    }
}
