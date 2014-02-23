using WinStoreFinalProject.Common;
using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace WinStoreFinalProject
{
    public sealed partial class montecarlo : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private Ellipse newEllipse;
        DispatcherTimer monteCarloTimer;
        private Random random;
        private double x, y, pi;
        private int dotX, dotY, throws, success, speed;

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public montecarlo()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            //creates the quarter circle on the page
            createBigCirle();

            //initializes values
            throws = 0;
            success = 0;
            random = new Random();
            speed = 300;

            //add event listener to speed selector
            speedSlider.ValueChanged += speedSlider_ValueChanged;

            //draws dots on a timer
            //adds event listener to the timer
            //sets the speed of the timer based on the speed selector
            monteCarloTimer = new DispatcherTimer();
            monteCarloTimer.Tick += monteCarloTimer_Tick;
            monteCarloTimer.Interval = new TimeSpan(0, 0, 0, 0, speed);
            monteCarloTimer.Start();
        }

        void speedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            speed = 301 - (int) speedSlider.Value * 3;
            monteCarloTimer.Interval = new TimeSpan(0, 0, 0, 0, speed);
        }

        private void createBigCirle()
        {
            newEllipse = new Ellipse();
            newEllipse.Width = dotCanvas.Width * 2;
            newEllipse.Height = dotCanvas.Height * 2;
            newEllipse.StrokeThickness = 2;
            newEllipse.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            newEllipse.Fill = new SolidColorBrush(Windows.UI.Colors.Black);

            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(0, 0, 600, 600);
            dotCanvas.Clip = rectangle;

            dotCanvas.Children.Add(newEllipse);
        }

        async void monteCarloTimer_Tick(object sender, object e)
        {
            //runs in a separate thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //x and y are set to a random number from 0.000 to 1.000
                x = random.Next(0, 1001) / (double) 1000;
                y = random.Next(0, 1001) / (double) 1000;

                //dot coordinates for drawing on the canvas are calculated
                dotX = (int) (x * dotCanvas.Width);
                dotY = (int) (y * dotCanvas.Width);

                //addDot is already set to run in a separate thread, no await is needed
                addDot(dotX, dotY);

                throws++;
                //if dot is inside the circle, increment success
                if (((x * x) + (y * y)) <= 1)
                {
                    success++;
                }
                //pi calculation
                pi = ((double) success / (double) throws) * (double) 4;
                tbPiDisplay.Text = "Current calculation of pi: " + pi.ToString("N5");
                
            });
        }

        async private void addDot(int x, int y)
        {
            //runs in a separate thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                //creates a new dot
                //backgroundThreadEllipse must be re-initialized on each call or we will end up moving the dot instead of drawing a new one
                Ellipse backgroundThreadEllipse = new Ellipse();
                backgroundThreadEllipse.Width = 8;
                backgroundThreadEllipse.Height = 8;
                backgroundThreadEllipse.StrokeThickness = 0;
                backgroundThreadEllipse.Stroke = new SolidColorBrush(Windows.UI.Colors.Blue);
                backgroundThreadEllipse.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
                backgroundThreadEllipse.Margin = new Thickness(x, y, 0, 0);
                dotCanvas.Children.Add(backgroundThreadEllipse);
            });
        }

        async private void btnRunTenThousand_Click(object sender, RoutedEventArgs e)
        {
            //runs in a new thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                for (int i = 0; i <= 10000; i++) 
                {
                    //copy of monteCarloTimer_Tick
                    x = random.Next(0, 1001) / (double) 1000;
                    y = random.Next(0, 1001) / (double) 1000;
                    dotX = (int) (x * dotCanvas.Width);
                    dotY = (int) (y * dotCanvas.Width);
                    throws++;
                    if (((x * x) + (y * y)) <= 1)
                    {
                        success++;
                    }
                    pi = ((double) success / (double) throws) * (double) 4;
                }
            });
            //updates the display after the loop, not before
            tbPiDisplay.Text = "Current calculation of pi: " + pi.ToString("N5");
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
