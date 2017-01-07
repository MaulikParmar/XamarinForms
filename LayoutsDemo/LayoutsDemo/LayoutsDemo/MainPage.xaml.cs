using System;
using Xamarin.Forms;

namespace LayoutsDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
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
