using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KimporterX
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                switch (btn.Text)
                {
                    case "Open":
                        try
                        {
                            var fileData = await CrossFilePicker.Current.PickFile();
                            if (fileData == null) return;
                            
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                        break;
                    case "Manage":
                        break;
                    default:
                        break;
                }
            }
        }
        
    }
}
