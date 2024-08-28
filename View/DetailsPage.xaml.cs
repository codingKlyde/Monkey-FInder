namespace Siklab;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(DetailsPageViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}