//

using System;
using ReactiveUI;
using OpenTK.Mathematics;
using AvaloniaGLExample.Graphics;
using AvaloniaGLExample.Utilities;
using AvaloniaGLExample.Views;

namespace AvaloniaGLExample.ViewModels;

public class MainViewModel : ReactiveObject
{
    private ReactiveObject avaloniaGLExampleModule;

    public MainViewModel()
    {
        this.AvaloniaGLExampleViewModel = new AvaloniaGLExampleViewModel();
    }
    
    public ReactiveObject AvaloniaGLExampleViewModel
    {
        get => this.avaloniaGLExampleModule;
        private init => this.RaiseAndSetIfChanged(ref this.avaloniaGLExampleModule, value);
    }
}