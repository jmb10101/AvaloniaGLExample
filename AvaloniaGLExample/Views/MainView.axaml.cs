//

using System;
using System.Reactive.Linq;
using System.Reflection.PortableExecutable;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using ReactiveUI;
using AvaloniaGLExample.Graphics;
using AvaloniaGLExample.ViewModels;

namespace AvaloniaGLExample.Views;

public partial class MainView : ReactiveWindow<MainViewModel>
{
    public MainView()
    {
        this.InitializeComponent();
        AvaloniaXamlLoader.Load(this);

        this.WhenActivated(disposables =>
        {
            this
                .OneWayBind(
                    this.ViewModel,
                    vm => vm.AvaloniaGLExampleViewModel,
                    v => v.AvaloniaGLExampleModule.ViewModel)
                .DisposeWith(disposables);
        });
    }

    public ViewModelViewHost AvaloniaGLExampleModule => this.FindControl<ViewModelViewHost>("Module0");
}