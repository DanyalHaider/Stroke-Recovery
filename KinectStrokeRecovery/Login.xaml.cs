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
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using System.Data;
using BLL;
using System.Diagnostics;

namespace KinectStrokeRecovery
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private KinectSensorChooser sensorChooser;
        public Login()
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

            //fill scroll content
           /* for (int i = 1; i < 20; i++)
            {
                var button = new KinectCircleButton
                {
                    Content = i,
                    Height = 200
                };

                int i1 = i;
                button.Click +=
                    (o, args) => MessageBox.Show("You clicked button #" + i1);

                scrollContent.Children.Add(button);
            }*/
        }

        
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
           
            bool error = false;
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
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

                  /*  try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                        args.NewSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                        error = true;
                    }*/
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


        private void LoginButtonOnClick(object sender, RoutedEventArgs e)
        {
           
            if (textBoxEmail.Text.Length == 0)
            {
                errormessage.Text = "Enter an email.";
                textBoxEmail.Focus();
            }

            else
            {
                DataTable dt = new DataTable();
                    sensorChooser.Stop();
                    Person per = new Person();
                    per.UName = textBoxEmail.Text;
                    per.PPassword = passwordBox1.Password;

                    dt = per.Login(per);
                    if (dt.Rows.Count > 0)
                    {
            

                        CharacterSelection cs = new CharacterSelection();
                        cs.Show();
                        this.Close();
                        //Process[] proc = Process.GetProcessesByName("osk");
                        // proc[0].Kill();
                    }
                    else {
                        errormessage.Text = "Invalid Email && Password ";
                    }
                
            }
        }

        private void RegistButtonOnClick(object sender, RoutedEventArgs e)
        {
            sensorChooser.Stop();
            Signup sign = new Signup();
            this.Close();
            sign.Show();
            //Process.Start("C:\\Windows\\System32\\OSK.EXE");
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            sensorChooser.Stop();
            //Process[] proc = Process.GetProcessesByName("osk");
            //proc[0].Kill();
            mw.Show();
            this.Close();
        }

        private void KinectTileButton_Click_1(object sender, RoutedEventArgs e)
        {
            Help hlp = new Help();
            sensorChooser.Stop();
            //Process[] proc = Process.GetProcessesByName("osk");
            //proc[0].Kill();
            hlp.Show();
            this.Close();
        }

        private void KinectTileButton_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            //Process[] proc = Process.GetProcessesByName("osk");
            sensorChooser.Stop();
            //proc[0].Kill();
            mw.Show();
            this.Close();
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
