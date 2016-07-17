﻿using System;
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
using Coding4Fun.Kinect.Wpf;

namespace KinectStrokeRecovery
{
    /// <summary>
    /// Interaction logic for SessionMenu.xaml
    /// </summary>
    public partial class ExerciseMenu : Window
    {
        public bool StatusKinect;
        public string gender;

        public  KinectSensorChooser sensorChooser;
        public ExerciseMenu(string gen)
        {
            gender = gen;
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


        
        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            //Game exe = new Game(sensorChooser);
           // exe.Show();
            //this.Close();
        }

        private void KinectTileButton_Click_1(object sender, RoutedEventArgs e)
        {
            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            Exercise exe = new Exercise(sensorChooser,BtnName, gender);
            exe.Show();
            this.Close();
        }

        private void KinectTileButton_Click_2(object sender, RoutedEventArgs e)
        {
            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            Exercise exe = new Exercise(sensorChooser,BtnName,gender);
            exe.Show();
            this.Close();
        }

        private void KinectTileButton_Click_3(object sender, RoutedEventArgs e)
        {
            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            Exercise exe = new Exercise(sensorChooser,BtnName, gender);
            exe.Show();
            this.Close();
        }

        private void KinectTileButton_Click_4(object sender, RoutedEventArgs e)
        {
            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            Exercise exe = new Exercise(sensorChooser,BtnName, gender);
            exe.Show();
            this.Close();
        }

        private void KinectTileButton_Click_5(object sender, RoutedEventArgs e)
        {

            string BtnName;
            BtnName = (sender as Microsoft.Kinect.Toolkit.Controls.KinectTileButton).Content.ToString();
            Exercise exe = new Exercise(sensorChooser,BtnName, gender);
            exe.Show();
            this.Close();
        }

        private void HelpKinectTileButton_Click(object sender, RoutedEventArgs e)
        {
            Help hlp = new Help();
            hlp.Show();
            this.Close();
        }

        private void KinectTileButton_Click_6(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            sensorChooser.Stop();
            mw.Show();
            this.Close();
        }

        private void KinectTileButton_Click_7(object sender, RoutedEventArgs e)
        {
            sensorChooser.Stop();
            CharacterSelection cs = new CharacterSelection();
            cs.Show();
            this.Close();
        }

        private void KinectTileButton_Click_8(object sender, RoutedEventArgs e)
        {
            AboutUs abt = new AboutUs();
            sensorChooser.Stop();
            abt.Show();
            this.Close();
        }


    }
}
