using Camera.MAUI;
using RestSharp;
using System.Reflection.Emit;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Firebase.Database;
using Mopups.Services;
using Mopups.Interfaces;
using Tesseract;
using System.Runtime.CompilerServices;

namespace ZoomInside;

public partial class CameraAccessment : ContentPage
{
    //private static string fullPath;

    private static string FirebaseUrl = "https://zoominside-2ccf3-default-rtdb.europe-west1.firebasedatabase.app/";
    private static string Secret = "0QrfB79PDE9o2OZ2OJsxZPzKcESXPdZ5nqUUMmbJ";

    FirebaseClient firebaseClient =
            new FirebaseClient(FirebaseUrl);

    private async Task LoadDataAsync()
    {
        // Simulate loading data asynchronously
        await Task.Delay(3000); // Placeholder for actual data loading code
    }


    IPopupNavigation popupNavigation;
    public CameraAccessment(IPopupNavigation popupNavigation)
    {
        InitializeComponent();

        // Set the binding context to the current instance of the page (this)
        this.BindingContext = this;

        this.popupNavigation = popupNavigation;

        //fullPath = CreateDirectory();
    }

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        try
        {
            // Use MediaPicker to interact with the device camera
            var photo = CapturePhotoAsync();


            if (photo != null)
            {
                //var filePath = CreateDirectory();
                //SaveImage(photo, filePath);


                var extractedText = ProcessImageWithTesseract();
                var dataToCheck = extractedText
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .ToList();


                // Hiding the camera button and the manual search button
                cameraButton.IsVisible = false;
                searchButtoon.IsVisible = false;


                var firebaseObject = await firebaseClient.Child("Es").OnceAsync<FirebaseData>();
                List<FirebaseData> dataFromFirebase = firebaseObject.Select(x => x.Object).ToList();

                await popupNavigation.PushAsync(new MyMopup(dataToCheck, dataFromFirebase));
            }
            else
            {
                return;
            }
        }
        catch (FeatureNotSupportedException fnsEx)
        {
            // Feature not supported on the device
            await DisplayAlert("Error", fnsEx.Message, "OK");
            return;
        }
        catch (PermissionException pEx)
        {
            // Permissions not granted
            await DisplayAlert("Error", pEx.Message, "OK");
            return;
        }
        catch (Exception ex)
        {
            // Other errors
            await DisplayAlert("Error", ex.Message, "OK");
            return;
        }

    }

    private string ProcessImageWithTesseract(string fullPath)
    {
        //var tessDataPath = @"C:\Users\yanis\Downloads\tesseract-master\tesseract-master\src\Tesseract.Tests\tessdata";

        //try
        //{
        //    using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
        //    {
        //        using (var img = Pix.LoadFromFile(fullPath))
        //        {
        //            using (var page = engine.Process(img))
        //            {
        //                string text = page.GetText();

        //                string unwanted = ",;().!-_";
        //                string cleanedText = new string(text.Where(c => !unwanted.Contains(c)).ToArray());
        //                return cleanedText;
        //            }
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    throw new Exception($"Error extracting the text {ex.Message}");
        //}

        string text = string.Empty;
        try
        {
            // Initialize the Tesseract Engine
            using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(fullPath);

            // Perform OCR
            using var page = engine.Process(img);
            text = page.GetText();
            Console.WriteLine($"Extracted Text: {text}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting text: {ex.Message}");
        }
        return text;
    }

    private string CreateDirectory()
    {
        try
        {
            var customPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Snapshots");
            if (!Directory.Exists(customPath))
            {
                Directory.CreateDirectory(customPath);
            }

            var fileName = "snap.jpeg";
            var filePath = Path.Combine(customPath, fileName);

            return filePath;

        }
        catch (Exception ex)
        {
            throw new Exception($"Възникна грешка {ex.Message}");
        }
    }

    private async void SaveImage(FileResult photo, string filePath)
    {
        //byte[] imageBytes = null;

        //var stream = await photo.OpenReadAsync();
        //if (stream != null)
        //{
        //    using (MemoryStream toBytes = new MemoryStream())
        //    {
        //        await stream.CopyToAsync(toBytes);
        //        imageBytes = toBytes.ToArray();
        //    }
        //}
        //await File.WriteAllBytesAsync(fullPath, imageBytes);

        try
        {
            var customPath = Path.Combine(FileSystem.Current.AppDataDirectory, "Snapshots");
            if (!Directory.Exists(customPath))
            {
                Directory.CreateDirectory(customPath);
            }

            using (var stream = await photo.OpenReadAsync())
            using (var fileStream = File.Create(filePath))
            {
                await stream.CopyToAsync(fileStream);
            }

        }
        catch (Exception ex)
        {
            throw new Exception($"Възникна грешка {ex.Message}");
        }
    }

    private async Task<FileResult> CapturePhotoAsync()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    // Save the photo to local storage and return its path
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(filePath);
                    await stream.CopyToAsync(newStream);
                    return photo;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"Error capturing photo: {ex.Message}");
            }
        }
        return null;
    }




    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AdminAuthentication());
        //Navigation.PushAsync(new Authentication());
    }

    private void searchButtoon_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ManualSearch(popupNavigation));
    }

    private void adminButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AdminAuthentication());
    }
}