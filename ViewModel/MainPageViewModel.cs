namespace Siklab.ViewModel
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        bool isRefreshing;

        IConnectivity connectivity;
        IGeolocation geolocation;
        MonkeyService monkeyService;

        public ObservableCollection<Monkey> Monkeys { get; } = new();

        public MainPageViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation)
        {
            Title = "Monkey Finder";
            this.connectivity = connectivity;
            this.geolocation = geolocation;
            this.monkeyService = monkeyService;
        }

        [RelayCommand]
        async Task GetMonkeysAsync()
        {
            if (IsBusy)
                return;

            try
            {
                if (connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("No internet detected", $"Check your internet and try again", "OK");
                    return;
                }

                IsBusy = true;
                var monkeys = await monkeyService.GetMonkeys();

                if (Monkeys.Count != 0)
                    Monkeys.Clear();

                foreach (var monkey in monkeys)
                    Monkeys.Add(monkey);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error", $"Unable to get monkeys: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }

        }

        [RelayCommand]
        async Task GoToDetailsPageAsync(Monkey monkey)
        {
            if (monkey is null) 
                return;

            await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true, 
                new Dictionary<string, Object> 
                {
                    {"Monkey", monkey}  
                });
            // to pass data use  await.Shell.Current.GoToAsync($"{nameof(DetailsPage)}?id={monkey.Name}");
        }

        [RelayCommand]
        async Task GetClosestMonkeyAsync() 
        {
            if (IsBusy || Monkeys == null || Monkeys.Count == 0)
                return;

            try
            {
                // Check if the location is null and get the location if it is
                var location = await geolocation.GetLastKnownLocationAsync();
                if (location is null)
                {
                    location = await geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30),
                        });
                }

                // If still no location
                if (location is null)
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to get location", "OK");
                    return;
                }

                var first = Monkeys.OrderBy(m => location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Miles)).FirstOrDefault();

                // If no monkeys
                if (first is null)
                {
                    await Shell.Current.DisplayAlert("Error", "No monkeys found", "OK");
                    return;
                }

                // Display 
                await Shell.Current.DisplayAlert("Closest Monkey", $"{first.Name} in {first.Location}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error", $"Unable to get closest monkey: {ex.Message}", "OK");
            }
        }
    }
}