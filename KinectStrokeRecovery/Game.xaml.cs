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
namespace KinectStrokeRecovery
{
    /// <summary>
    /// Interaction logic for Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        KinectSensorChooser sensor;
        public Game(KinectSensorChooser sensorChooser)
        {
            sensor = sensorChooser;
            InitializeComponent();
            sensorChooser.Kinect.SkeletonStream.Enable();
            sensorChooser.Kinect.DepthStream.Enable();
            sensorChooserUi.KinectSensorChooser = sensorChooser;
            sensorChooser.Kinect.Start();

        }
    }
}
