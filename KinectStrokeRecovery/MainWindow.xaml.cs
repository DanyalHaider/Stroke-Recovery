using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Diagnostics;

namespace KinectStrokeRecovery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
  public partial class MainWindow : Window
    {
        private KinectSensorChooser sensorChooser;
        public MainWindow()
        {

            InitializeComponent();
            Loaded += OnLoaded;
        }
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {

            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

        }

        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    //     Gets the depth stream for the sensor.
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                    error = true;
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                }
                catch (InvalidOperationException)
                {
                    error = true;
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (!error)
                kinectRegion.KinectSensor = args.NewSensor;
        }

        private void HomeButtonOnClick(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }

        private void SessionButtonOnClick(object sender, RoutedEventArgs e)
        {
            sensorChooser.Stop();
            Login log = new Login();
            this.Close();
            log.Show();
           // Process.Start("C:\\Windows\\System32\\OSK.EXE");
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            sensorChooser.Stop();
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            Help hlp = new Help();
            //hlp.Background = new SolidColorBrush(Colors.LightGray) { Opacity = 0.5 };
            //hlp.WindowStyle = System.Windows.WindowStyle.None;
            //hlp.AllowsTransparency = true;
            //hlp.Opacity = 0.1;
            sensorChooser.Stop();
            hlp.Show();
            this.Close();
        }

        private void KinectTileButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            sensorChooser.Stop();
            mw.Show();
            this.Close();
        }

        private void KinectTileButton_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void KinectTileButton_Click_3(object sender, RoutedEventArgs e)
        {
            AboutUs abt = new AboutUs();
            sensorChooser.Stop();
            abt.Show();
            this.Close();
        }


    }
    }

