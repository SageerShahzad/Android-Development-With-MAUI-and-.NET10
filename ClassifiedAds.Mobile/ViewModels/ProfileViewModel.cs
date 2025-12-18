using System.Collections.ObjectModel;

namespace ClassifiedAds.Mobile.ViewModels;

public class MenuItem
{
    public string Title { get; set; }
    public string Icon { get; set; }
}

public class ProfileViewModel : BaseViewModel
{
    public ObservableCollection<MenuItem> MenuItems { get; set; }

    public ProfileViewModel()
    {
        MenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Title = "My orders", Icon = "dotnet_bot.png" },
            new MenuItem { Title = "My deposit account", Icon = "dotnet_bot.png" },
            new MenuItem { Title = "My bonuses", Icon = "dotnet_bot.png" },
            new MenuItem { Title = "My wish list", Icon = "dotnet_bot.png" },
            new MenuItem { Title = "Settings", Icon = "dotnet_bot.png" },
            new MenuItem { Title = "Help & Support", Icon = "dotnet_bot.png" }
        };
    }
}