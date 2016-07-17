using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace BLL
{

    public class GestureRecognition
    {
        KinectSensor sensor = KinectSensor.KinectSensors[0];

        Skeleton[] skeletons;


        public GestureRecognition()
        {


            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.3f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 1.0f,
                MaxDeviationRadius = 0.04f
            };
            sensor.SkeletonStream.Enable();
            sensor.Start();
            //substribe to the SkeletonFrameReady event , This event gets fired when a First skeleton is detected by the Kinect Sensor
            sensor.SkeletonFrameReady += TrackSkeleton;

        }

        public Joint SetPosition(Joint joint)
        {
            Microsoft.Kinect.SkeletonPoint vector = new Microsoft.Kinect.SkeletonPoint();
            vector.X = ScaleVector(640, joint.Position.X);
            vector.Y = ScaleVector(480, -joint.Position.Y);


            Joint updatedJoint = new Joint();
            updatedJoint = joint;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;
            return updatedJoint;

            //Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            //Canvas.SetTop(ellipse, updatedJoint.Position.Y);
        }
        // Match screen resolution with kinect resolution
        public float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2); // Formula to set resolution
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }
        public void TrackSkeleton(object sender, SkeletonFrameReadyEventArgs e)
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

                    Joint leftjoint = SetPosition(currentSkeleton.Joints[JointType.HandLeft]);
                    Joint rightjoint = SetPosition(currentSkeleton.Joints[JointType.HandRight]);


                    //return rightxy;

                }
            }
        }
    }
}
