using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Codify.Converters;

namespace Travlexer.WindowsPhone.Controls
{
    public class AdControl : ContentPresenter
    {
        public AdControl()
        {
            #region Smaato

//            {
//                var ad = new SOMAWP7.SomaAdViewer
//                {
//#if DEBUG
//                    ShowErrors = true,
//#endif
//                    Pub = 923838645,
//                    Adspace = 65773647,
//                    PopupAd = true,
//                    LocationUseOK = true
//                };
//                Content = ad;

//#if DEBUG
//                if (System.ComponentModel.DesignerProperties.IsInDesignTool) return;
//#endif
//                ad.StartAds();
//            }

            #endregion

            #region AdMob

            {
                var ad = new Google.AdMob.Ads.WindowsPhone7.WPF.BannerAd
                {
                    AdUnitID = "a150e41c7f6d5e1"
                };
                SetBinding(VisibilityProperty, new Binding("AdAvailable")
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                    Converter = (VisibilityConverter)Application.Current.Resources["VisibilityConverter"]
                });
                Content = ad;
            }

            #endregion
        }
    }
}