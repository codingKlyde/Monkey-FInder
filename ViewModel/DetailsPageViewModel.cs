namespace Siklab.ViewModel
{
    [QueryProperty("Monkey", "Monkey")]
    public partial class DetailsPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        Monkey monkey;

        IMap map;

        public DetailsPageViewModel(IMap map) 
        { 
          this.map = map;
        }

        [RelayCommand]
        async Task GoBackAsync()
        {
            // To bring back data, use:
            // await Shell.Current.GoToAsync("..?id=1");

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task OpenMapAsync()
        {
            try
            {
                await map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
                    new MapLaunchOptions
                    {
                        Name = Monkey.Name,
                        NavigationMode = NavigationMode.None,
                    });
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error", $"Unable to open map: {ex.Message}", "OK");
            }
        }
    }
}