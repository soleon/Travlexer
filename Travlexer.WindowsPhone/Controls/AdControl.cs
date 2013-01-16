using System.ComponentModel;
using System.Windows.Controls;
using SOMAWP7;

namespace Travlexer.WindowsPhone.Controls
{
    public class AdControl : ContentPresenter
    {
        public AdControl()
        {
            var ad = new SomaAdViewer
            {
#if DEBUG
                ShowErrors = true,
#endif
                Pub = 923838645,
                Adspace = 65773647,
                PopupAd = true,
                LocationUseOK = true
            };
            Content = ad;

#if DEBUG
            if (DesignerProperties.IsInDesignTool) return;
#endif
            ad.StartAds();
        }
    }
}