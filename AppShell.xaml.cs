namespace DronApp1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }


        private async void OnMenuItemClicked(object sender, EventArgs e)
        {



            await DisplayAlert("Меню", "Работа с Bluetooth", "OK");
            //await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            //  await Shell.Current.GoToAsync("//LoginPage");
        }   



    }
}
