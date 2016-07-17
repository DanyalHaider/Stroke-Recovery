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
using Coding4Fun.Kinect.Wpf;
using System.Windows.Threading;
using Microsoft.Win32.SafeHandles;
using System.Threading.Tasks;
using System.Threading;
namespace KinectStrokeRecovery
{
    /// <summary>
    /// Interaction logic for Exercise.xaml
    /// </summary>
    public partial class Exercise : Window
    {
     // public KinectSensor sensor = KinectSensor.KinectSensors[0];
        public Type LastWindow;
        string CopyBtnName;
        public bool checkvideo;
       public Skeleton[] skeletons;
       short[] pixelData;
       KinectSensorChooser sensor;
       string Gender;
        public Exercise(KinectSensorChooser sensorChooser, string BtnName, string gend)
        {
            Gender = gend;
            CopyBtnName = BtnName;
            InitializeComponent();
            sensor= sensorChooser;
            //SessionMenu sm = new SessionMenu();
            checkvideo = false;
            sensorChooser.Kinect.SkeletonStream.Enable();
            sensorChooser.Kinect.DepthStream.Enable();
            
            sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            sensorChooserUi.KinectSensorChooser = sensorChooser;
            sensorChooser.Kinect.SkeletonFrameReady += TrackSkeleton;
            sensorChooser.Kinect.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
            sensorChooser.Kinect.Start();
            
        }
//******************************************************************************************************************************
//Hand Cursor animation in depth  
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            bool error = false;
            if (args.OldSensor != null)
            {
                //try
                //{
                //    args.OldSensor.DepthStream.Range = DepthRange.Default;
                //    //     Gets the depth stream for the sensor.
                //    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                //    args.OldSensor.DepthStream.Disable();
                //    args.OldSensor.SkeletonStream.Disable();
                //}
                //catch (InvalidOperationException)
                //{
                //    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                //    // E.g.: sensor might be abruptly unplugged.
                //    error = true;
                //}
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
//*************************************************************************************************************************
// Depth video of patient
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            bool receivedData = false;
            
            using (DepthImageFrame DFrame = e.OpenDepthImageFrame())
            {
                if (DFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    pixelData = new short[DFrame.PixelDataLength];
                    DFrame.CopyPixelDataTo(pixelData);
                    receivedData = true;
                }
            }

            if (receivedData)
            {
                BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                        PixelFormats.Gray16, null, pixelData, 640 * 2);

                depthImage.Source = source;
            }
        }
//************************************************************************************************************************
// Set position and Template matching
        public void SetPosition(Joint LHJoint, Joint RHJoint, Joint LEJoint, Joint REJoint, Joint Head, Joint BodyOrign)
        {

            var scaledJointLH = LHJoint.ScaleTo(1920, 1080);
            var scaledJointRH = RHJoint.ScaleTo(1920, 1080);
            var scaledJointLE = LEJoint.ScaleTo(1920, 1080);
            var scaledJointRE = REJoint.ScaleTo(1920, 1080);
            var scaledJointHead = Head.ScaleTo(1920, 1080);
            var scaledJointOrign = BodyOrign.ScaleTo(1920, 1080);
            if (Gender == "Male")
            {
                switch (CopyBtnName)
                {
                    case "Right Arm":

                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\RightArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();
                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\RightArm.avi", UriKind.Absolute);

                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                            //DispatcherTimer timer = new DispatcherTimer();
                            //timer.Interval = TimeSpan.FromSeconds(60);
                            //timer.Tick +=new EventHandler(timer_Tick);



                        }

                        var disX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var disY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        if ((disX > 265 && disX < 465) && (disY > -435 && disY < -290))
                        {
                            Template.Background = System.Windows.Media.Brushes.Gray;
                            //slider1.Value = 100;
                            labelMatch.Content = "Great Job!";
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                     DispatcherTimer timer3 = new DispatcherTimer();
                                     timer3.Interval = TimeSpan.FromSeconds(1);
                                     timer3.Tick += (ssss, eeee) => { 
                                    RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                    RecYG.Visibility = System.Windows.Visibility.Visible;
                                    timer3.Stop();

                                    DispatcherTimer timer4 = new DispatcherTimer();
                                    timer4.Interval = TimeSpan.FromSeconds(0.5);
                                    timer4.Tick += (sssss, eeeee) =>
                                    {
                                        RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                        RecLast.Visibility = System.Windows.Visibility.Visible;
                                        timer4.Stop();


                                    };
                                    timer4.Start();
                                     };
                                      timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            /*RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                            RectMid.Background = System.Windows.Media.Brushes.Yellow;
                            RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;*/
                             
                             

                        }
                        else if ((disX > 465 && disX < 670) && (disY > -290 && disY < -145))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                           
                           // var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(2);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Visibility = System.Windows.Visibility.Collapsed;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Visibility = System.Windows.Visibility.Collapsed;
                            // slider1.Value = 50;
                            /*RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                            RectMid.Background = System.Windows.Media.Brushes.Yellow;
                             */
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            labelMatch.Content = "Try! You Can Do";
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RecRY.Visibility = System.Windows.Visibility.Collapsed;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Visibility = System.Windows.Visibility.Collapsed;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Visibility = System.Windows.Visibility.Collapsed;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        break;
                    case "Both Arm":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\BothArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\TWOARMS.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }
                        var RdisX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var RdisY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        var LdisX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var LdisY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;

                        if ((RdisX > 210 && RdisX < 450) && (RdisY > -460 && RdisY < -312) && (LdisX > -467 && LdisX < -250) && (LdisY > -470 && LdisY < -295))
                        {


                            Template.Background = System.Windows.Media.Brushes.Gray;
                            //slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            labelMatch.Content = "Great Job!";
                        }
                        else if ((RdisX > 450 && RdisX < 685) && (RdisY > -312 && RdisY < -165) && (LdisX > -685 && LdisX < -467) && (LdisY > -295 && LdisY < -120))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            labelMatch.Content = "Try! You Can Do";
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        break;
                    case "Left Arm":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\LeftArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();
                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\LeftArm.avi", UriKind.Absolute);

                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var distX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var distY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;
                        if ((distX > -680 && distX < -655) && (distY > -155 && distY < -95))
                        {
                            labelMatch.Content = "Great Job!";
                            Template.Background = System.Windows.Media.Brushes.Gray;
                            //slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                        }
                        else if ((distX > -655 && distX < -630) && (distY > -95 && distY < -40))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            labelMatch.Content = "Try! You Can Do";
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        break;
                    case "Left Elbow":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\BothArm.JPG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\TWOARMS.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var LEdisX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var LEdisY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;
                        var LEdisZ = scaledJointLH.Position.Z - scaledJointOrign.Position.Z;
                        if ((LEdisX > -467 && LEdisX < -250) && (LEdisY > -470 && LEdisY < -295) && (LEdisZ > -470 && LEdisZ < -295))
                        {
                            labelMatch.Content = "Great Job!";
                           // slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            Template.Background = System.Windows.Media.Brushes.Gray;
                        }
                        else if ((LEdisX > -467 && LEdisX < -250) && (LEdisY > -470 && LEdisY < -295) && (LEdisZ > -470 && LEdisZ < -295))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            labelMatch.Content = "Try! You Can Do";
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        break;
                    case "Right Elbow":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\RightElbow.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\RightElbow.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var REdisX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var REdisY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        /*var REdisZ = scaledJointRH.Position.Z - scaledJointOrign.Position.Z;
                        && (REdisZ > -470 && REdisZ < -295) */
                        if ((REdisX > 125 && REdisX < 250) && (REdisY > -200 && REdisY < -190))
                        {

                            labelMatch.Content = "Great Job!";
                            Template.Background = System.Windows.Media.Brushes.Gray;
                           // slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                        }
                        else if ((REdisX > 250 && REdisX < 375) && (REdisY > -210 && REdisY < -200))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                }
                // Canvas.SetLeft(ellipse, scaledJointLH.Position.X);
                //Canvas.SetTop(ellipse, scaledJointLH.Position.Y);
            }
            else 
            {
                switch (CopyBtnName)
                {
                    case "Right Arm":

                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\GRightArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();
                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\GRightArm.avi", UriKind.Absolute);

                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                            //DispatcherTimer timer = new DispatcherTimer();
                            //timer.Interval = TimeSpan.FromSeconds(60);
                            //timer.Tick +=new EventHandler(timer_Tick);



                        }

                        var disX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var disY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        if ((disX > 265 && disX < 465) && (disY > -435 && disY < -290))
                        {
                            Template.Background = System.Windows.Media.Brushes.Gray;
                           // slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            labelMatch.Content = "Great Job!";

                        }
                        else if ((disX > 465 && disX < 670) && (disY > -290 && disY < -145))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                    case "Both Arm":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\BothArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\TWOARMS.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }
                        var RdisX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var RdisY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        var LdisX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var LdisY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;

                        if ((RdisX > 210 && RdisX < 450) && (RdisY > -460 && RdisY < -312) && (LdisX > -467 && LdisX < -250) && (LdisY > -470 && LdisY < -295))
                        {


                            Template.Background = System.Windows.Media.Brushes.Gray;
                           // slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            labelMatch.Content = "Great Job!";
                        }
                        else if ((RdisX > 450 && RdisX < 685) && (RdisY > -312 && RdisY < -165) && (LdisX > -685 && LdisX < -467) && (LdisY > -295 && LdisY < -120))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                    case "Left Arm":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\GLeftArm.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();
                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\GLeftArm.avi", UriKind.Absolute);

                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var distX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var distY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;
                        if ((distX > -680 && distX < -655) && (distY > -155 && distY < -95))
                        {
                            labelMatch.Content = "Great Job!";
                            Template.Background = System.Windows.Media.Brushes.Gray;
                            //slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();

                        }
                        else if ((distX > -655 && distX < -630) && (distY > -95 && distY < -40))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                    case "Left Elbow":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\BothArm.JPG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\TWOARMS.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var LEdisX = scaledJointLH.Position.X - scaledJointOrign.Position.X;
                        var LEdisY = scaledJointLH.Position.Y - scaledJointOrign.Position.Y;
                        var LEdisZ = scaledJointLH.Position.Z - scaledJointOrign.Position.Z;
                        if ((LEdisX > -467 && LEdisX < -250) && (LEdisY > -470 && LEdisY < -295) && (LEdisZ > -470 && LEdisZ < -295))
                        {
                            labelMatch.Content = "Great Job!";
                            //slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            Template.Background = System.Windows.Media.Brushes.Gray;
                        }
                        else if ((LEdisX > -467 && LEdisX < -250) && (LEdisY > -470 && LEdisY < -295) && (LEdisZ > -470 && LEdisZ < -295))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            //slider1.Value = 50;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                    case "Right Elbow":
                        TemplateImage.BeginInit();
                        TemplateImage.Source = new BitmapImage(new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Images\GRightElbow.PNG"));
                        //TemplateImage.CacheOption = BitmapCacheOption.OnLoad;
                        TemplateImage.EndInit();

                        if (checkvideo == false)
                        {
                            Video.Source = new Uri(@"C:\Users\Danyal Haider\Documents\Visual Studio 2010\Projects\KinectStrokeRecovery\KinectStrokeRecovery\Video\GRightElbow.avi", UriKind.Absolute);
                            Video.Play();
                            checkvideo = true;
                            Video.MediaEnded += new RoutedEventHandler(KinectTileButton_Click);
                        }

                        var REdisX = scaledJointRH.Position.X - scaledJointOrign.Position.X;
                        var REdisY = scaledJointRH.Position.Y - scaledJointOrign.Position.Y;
                        /*var REdisZ = scaledJointRH.Position.Z - scaledJointOrign.Position.Z;
                        && (REdisZ > -470 && REdisZ < -295) */
                        if ((REdisX > 125 && REdisX < 250) && (REdisY > -200 && REdisY < -190))
                        {

                            labelMatch.Content = "Great Job!";
                            Template.Background = System.Windows.Media.Brushes.Gray;
                           // slider1.Value = 100;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(1.7);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(1.5);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();

                                    DispatcherTimer timer3 = new DispatcherTimer();
                                    timer3.Interval = TimeSpan.FromSeconds(1);
                                    timer3.Tick += (ssss, eeee) =>
                                    {
                                        RecYG.Background = System.Windows.Media.Brushes.YellowGreen;
                                        RecYG.Visibility = System.Windows.Visibility.Visible;
                                        timer3.Stop();

                                        DispatcherTimer timer4 = new DispatcherTimer();
                                        timer4.Interval = TimeSpan.FromSeconds(0.5);
                                        timer4.Tick += (sssss, eeeee) =>
                                        {
                                            RecLast.Background = System.Windows.Media.Brushes.LawnGreen;
                                            RecLast.Visibility = System.Windows.Visibility.Visible;
                                            timer4.Stop();


                                        };
                                        timer4.Start();
                                    };
                                    timer3.Start();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                        }
                        else if ((REdisX > 250 && REdisX < 375) && (REdisY > -210 && REdisY < -200))
                        {
                            labelMatch.Content = "Some more Effort Require!";
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 50;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromSeconds(3);
                            timer.Tick += (ss, ee) =>
                            {
                                RecRY.Background = System.Windows.Media.Brushes.OrangeRed;
                                RecRY.Visibility = System.Windows.Visibility.Visible;
                                timer.Stop();

                                DispatcherTimer timer2 = new DispatcherTimer();
                                timer2.Interval = TimeSpan.FromSeconds(3);
                                timer2.Tick += (sss, eee) =>
                                {
                                    RectMid.Background = System.Windows.Media.Brushes.Yellow;
                                    RectMid.Visibility = System.Windows.Visibility.Visible;
                                    timer2.Stop();
                                };
                                timer2.Start();
                            };
                            timer.Start();
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                        }
                        else
                        {
                            Template.Background = System.Windows.Media.Brushes.Transparent;
                           // slider1.Value = 0;
                            RectIni.Background = System.Windows.Media.Brushes.Red;
                            RecRY.Background = System.Windows.Media.Brushes.Transparent;
                            RectMid.Background = System.Windows.Media.Brushes.Transparent;
                            RecYG.Background = System.Windows.Media.Brushes.Transparent;
                            RecLast.Background = System.Windows.Media.Brushes.Transparent;
                            labelMatch.Content = "Try! You Can Do";
                        }
                        break;
                }
            }
        }
//********************************************************************************************************
// Joints Tracking 
        void TrackSkeleton(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;

            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {

                }
                else
                {

                    skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {
                // sub query expression .
                Skeleton currentSkeleton = (from skl in skeletons
                                            where skl.TrackingState == SkeletonTrackingState.Tracked
                                            select skl).FirstOrDefault();

                if (currentSkeleton != null)
                {
                    
                    SetPosition(currentSkeleton.Joints[JointType.HandLeft], currentSkeleton.Joints[JointType.HandRight], currentSkeleton.Joints[JointType.ElbowLeft], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.Head], currentSkeleton.Joints[JointType.HipCenter]);
                    //SetPosition(rightHand, currentSkeleton.Joints[JointType.HandRight]);



                }
            }
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
           // ExerciseMenu sm = new ExerciseMenu();
            //sm.sensorChooser.Kinect.Stop();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            
            
        }

        private void KinectTileButton_Click(object sender, RoutedEventArgs e)
        {
         
            ExerciseMenu exm = new ExerciseMenu(Gender);            
            sensor.Kinect.Stop();
            //Video.Stop();
            checkvideo = false;
            exm.Show();
            this.Close();
           
           
        }

    }
}
