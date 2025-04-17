using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace YourAppNamespace;

public partial class ImageSizeSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private int _imageWidth;

    [ObservableProperty]
    private int _imageHeight;

    public ImageSizeSettingsViewModel(int currentWidth, int currentHeight)
    {
        _imageWidth = currentWidth;
        _imageHeight = currentHeight;
    }
}
