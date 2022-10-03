/* Project: Dice Rolling App

 Purpose Details: To learn how to use MAUI
 Course: IST/CMPSC Capstone
 Author: David Martinez
 Date Developed: 9/28/2022
 Last Date Changed: 10/2/2022
*/

using Plugin.Maui.Audio;
namespace DiceGame;

/// <summary>
/// Main Page of the Dice Game, shows image of rolled die and roll counter
/// </summary>
public partial class MainPage : ContentPage
{
	int rollCount = 0;
    static int flashlightBlinkDelay = 1000;
    IAudioPlayer playSound;
    bool isShaked = false;
    private readonly static object syncObj = new object();

    // Maui has no buiilt-in audio playback framework,
    // need to install packages and set up manually
    // ref https://www.youtube.com/watch?v=oIYnEuZ9oew
    private readonly IAudioManager audioManager;

    public MainPage(IAudioManager audioManager)
	{
		InitializeComponent();
        this.audioManager = audioManager;
        ConfigureSound();

        // Exception thrown if used on device without accelerometer
        if (Accelerometer.Default.IsSupported)
        {
            // adds 
            Accelerometer.Default.ShakeDetected += RollDice;
            Accelerometer.Default.ShakeDetected += CheckShaking;

            Accelerometer.Default.Start(SensorSpeed.Game);
        }

    }

    /// <summary>
    /// Sets up plugin to support audio play back
    /// </summary>
    private async void ConfigureSound()
    {
        playSound = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("dice.mp3"));
    }

    /// <summary>
    /// Called whenever roll button clicked or device shaked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RollDice(object sender, EventArgs e)
    { 
	
        rollCount++;
        string text;

        while (isShaked)
        {
            playSound.Play();
            Vibration.Default.Vibrate();
            rollCounter.Text = "Rolling die";
            isShaked = false;
        }

        if (rollCount == 1)
        {
            text = $"Rolled die {rollCount} time";
        }

        else
        {
            text = $"Rolled die {rollCount} times";
        }

        rollCounter.Text = text;
        TextToSpeech.Default.SpeakAsync(text);

        Random r = new Random();
        int rollResult = r.Next(1, 7);

        diceResultImg.Source = ImageSource.FromFile($"d{rollResult}im.png");
        SemanticProperties.SetDescription(diceResultImg, $"Rolled a {rollResult}");
        SemanticScreenReader.Announce(rollCounter.Text);

        BlinkDieCountFlashlight();
    }

    /// <summary>
    /// Event handler used to make sure device is still being shaked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckShaking(object sender, EventArgs e)
    {
        isShaked = true;
    }

    /// <summary>
    /// Blinks flashlight based on number of total dice rolls
    /// </summary>
    private async void BlinkDieCountFlashlight() 
    {
        try
        {
            for (int i = 0; i < rollCount; i++)
            {
                await Flashlight.Default.TurnOnAsync();
                await Task.Delay(flashlightBlinkDelay);
                await Flashlight.Default.TurnOffAsync();

            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Error using Flashlight", ex.Message);
        }

    }
}

