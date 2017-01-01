using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ValidationDemo.Layouts
{
    public partial class LayoutDemo : ContentPage
    {
        public LayoutDemo()
        {
            InitializeComponent();

            for (int i = 0; i < 50; i++)
            {
                Image img = new Image() { WidthRequest = 100, HeightRequest = 100 };
                img.Source = new UriImageSource()
                {
                    Uri = new Uri("http://findicons.com/files/icons/5/animals/128/elephant.png")
                };
                wrapPanel.Children.Add(img);
            }

        }
    }
}
