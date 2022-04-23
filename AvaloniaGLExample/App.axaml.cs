//

using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Splat;
using AvaloniaGLExample.ViewModels;
using AvaloniaGLExample.Views;

namespace AvaloniaGLExample;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterViewsForViewModels(typeof(MainView).Assembly);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainView() { ViewModel = new MainViewModel() };
        }

        base.OnFrameworkInitializationCompleted();
    }
}