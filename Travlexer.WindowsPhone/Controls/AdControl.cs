using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using SOMAWP7;

namespace Travlexer.WindowsPhone.Controls
{
    public class AdControl : ContentPresenter
    {
        #region Smaato

        public AdControl()
        {
            var ad = new SomaAdViewer
            {
#if DEBUG
                ShowErrors = true,
#endif
                Pub = 923838645,
                Adspace = 65773647,
                LocationUseOK = true
            };
            Content = ad;

#if DEBUG
            if (DesignerProperties.IsInDesignTool) return;
#else
            ad.AdError += (sender, code, description) => ad.Visibility = Visibility.Collapsed;
            ad.NewAdAvailable += (sender, args) => ad.Visibility = Visibility.Visible;
#endif
            ad.Loaded += (sender, args) => ad.StartAds();
            ad.Unloaded += (sender, args) => ad.StopAds();
        }

        #endregion


        #region AdMob

        //public AdControl()
        //{
        //    var ad = new Google.AdMob.Ads.WindowsPhone7.WPF.BannerAd
        //    {
        //        AdUnitID = "a150e41c7f6d5e1"
        //    };
        //    SetBinding(VisibilityProperty, new Binding("AdAvailable")
        //    {
        //        RelativeSource = new RelativeSource(RelativeSourceMode.Self),
        //        Converter = (VisibilityConverter)Application.Current.Resources["VisibilityConverter"]
        //    });
        //    Content = ad;
        //}

        #endregion
    }
}