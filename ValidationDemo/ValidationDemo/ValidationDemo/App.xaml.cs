using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ValidationDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ValidationDemo.MainPage();
            //MainPage = new ValidationDemo.Layouts.LayoutDemo();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
