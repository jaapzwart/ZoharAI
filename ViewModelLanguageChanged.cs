using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZoharBible;

public class ViewModelLanguageChanged : INotifyPropertyChanged
{
    private string _selectedLanguage;

    public event PropertyChangedEventHandler PropertyChanged;

    public string SelectedLanguage
    {
        get => _selectedLanguage;
        set
        {
            if (_selectedLanguage != value)
            {
                _selectedLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}