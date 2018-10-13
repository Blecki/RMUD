using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AndroidTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            Game.Game.Begin(s => Output.Text += s);
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            Output.Text = "";
            Game.Game.Input(InputBox.Text);
            InputBox.Text = "";
        }
    }
}
