using Firebase.Database;
using Mopups.Interfaces;
using Mopups.Services;

namespace ZoomInside;

public partial class ManualSearch : ContentPage
{
    FirebaseClient firebaseClient =
            new FirebaseClient("https://zoominside-2ccf3-default-rtdb.europe-west1.firebasedatabase.app/");

    private async Task LoadDataAsync()
    {
        // Simulate loading data asynchronously
        await Task.Delay(3000); // Placeholder for actual data loading code
    }

    IPopupNavigation popupNavigation;
    public ManualSearch(IPopupNavigation popupNavigation)
	{
		InitializeComponent();

        this.popupNavigation = popupNavigation;
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (searchBox.Text == null)
        {
            await DisplayAlert("Грешка!", "Моля попълнете нужните полета!", "Добре!");
            return;
        }

        try
        {
            // Showing the activity indicator
            activityIndicator.IsRunning = true;
            activityIndicator.IsVisible = true;
            await LoadDataAsync();

            var substance = searchBox.Text.ToLower();

            char[] splitCharacters = { '{', '}', ' ', ',', '[', ']' };
            List<string> dataToCheck = substance
                .Split(splitCharacters, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            for (var i = 0; i < dataToCheck.Count; i++)
            {
                dataToCheck[i] = dataToCheck[i].ToLower();

                //if (dataToCheck[i][0] == 'e')
                //    dataToCheck[i] = dataToCheck[i].Replace('e', 'е');
            }

            var firebaseObject = await firebaseClient.Child("Es").OnceAsync<FirebaseData>();
            List<FirebaseData> dataFromFirebase = firebaseObject.Select(x => x.Object).ToList();


            // Hide the activity indicator
            activityIndicator.IsRunning = false;
            activityIndicator.IsVisible = false;

            await popupNavigation.PushAsync(new MyMopup(dataToCheck, dataFromFirebase));
        }
        catch (Exception)
        {
            await DisplayAlert("Грешка!", "Нещо се обърка.", "Добре!");
        }        
    }

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
        infoBox.IsVisible = !infoBox.IsVisible;
    }
}