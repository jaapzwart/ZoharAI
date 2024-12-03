using CommunityToolkit.Maui;

namespace ZoharBible;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        // Maak een instantie van je audio service class.
        IAudioService audioService = new AudioService();
        
        MainPage = new NavigationPage(new Intro());
    }
}