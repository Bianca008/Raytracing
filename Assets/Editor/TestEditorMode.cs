﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Tests
{
    public class TestEditorMode
    {

        [Test]
        public void OneRayTwoPointsIntensities_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            AcousticRay ray = new AcousticRay(origin);
            ray.ColissionPoints.Add(new Vector3(1, 1, 1));

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(ray);

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            double epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(rays[0].Intensities[0] - 0.07957) < epsilon);
            Assert.IsTrue(Math.Abs(rays[0].Intensities[1] - 0.0106613589) < epsilon);
        }

        [Test]
        public void OneRayMultiplePointsIntensities_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            AcousticRay ray = new AcousticRay(origin);
            ray.ColissionPoints.Add(new Vector3(1, 1, 1));
            ray.ColissionPoints.Add(new Vector3(2, 1, 1));
            ray.ColissionPoints.Add(new Vector3(1, 5, 1));

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(ray);

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            double epsilon = 1e-5;
            double[] expectedValues = { 0.07957747, 0.0106613589, 0.00571340513, 0.001289675 };

            for (int index = 0; index < expectedValues.Length; ++index)
                Assert.IsTrue(Math.Abs(rays[0].Intensities[index] - expectedValues[index]) < epsilon);
        }

        [Test]
        public void TwoRaysMultiplePointsIntensities_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            AcousticRay first = new AcousticRay(origin);
            first.ColissionPoints.Add(new Vector3(1, 1, 1));
            first.ColissionPoints.Add(new Vector3(2, 1, 1));
            first.ColissionPoints.Add(new Vector3(1, 5, 1));

            AcousticRay second = new AcousticRay(origin);
            second.ColissionPoints.Add(new Vector3(1, 1, 1));
            second.ColissionPoints.Add(new Vector3(2, 1, 1));
            second.ColissionPoints.Add(new Vector3(1, 5, 1));
            second.ColissionPoints.Add(new Vector3(1, 8, 2));
            second.ColissionPoints.Add(new Vector3(3, 5, 3));

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(first);
            rays.Add(second);

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            double epsilon = 1e-5;
            double[] expectedValuesFirstRay = { 0.07957747, 0.0106613589, 0.00571340513, 0.001289675 };
            double[] expectedValuesSecondRay = { 0.07957747, 0.0106613589,
                                                 0.00571340513, 0.001289675,
                                                 0.0006555853067, 0.00036531784 };

            for (int index = 0; index < expectedValuesFirstRay.Length; ++index)
                Assert.IsTrue(Math.Abs(rays[0].Intensities[index] - expectedValuesFirstRay[index]) < epsilon);

            for (int index = 0; index < expectedValuesSecondRay.Length; ++index)
                Assert.IsTrue(Math.Abs(rays[1].Intensities[index] - expectedValuesSecondRay[index]) < epsilon);
        }

        [Test]
        public void OneRayComputeDistances_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            AcousticRay ray = new AcousticRay(origin);
            ray.ColissionPoints.Add(new Vector3(1, 1, 1));
            ray.ColissionPoints.Add(new Vector3(2, 1, 1));
            ray.ColissionPoints.Add(new Vector3(1, 5, 1));

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(ray);

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            double epsilon = 1e-2;
            double[] expectedValues = { 0, 1.73, 2.73, 6.85};

            for (int index = 0; index < expectedValues.Length; ++index)
                Assert.IsTrue(Math.Abs(rays[0].Distances[index] - expectedValues[index]) < epsilon);
        }

        [Test]
        public void OneRayComputeTime_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            AcousticRay ray = new AcousticRay(origin);
            ray.ColissionPoints.Add(new Vector3(1, 1, 1));
            ray.ColissionPoints.Add(new Vector3(2, 1, 1));
            ray.ColissionPoints.Add(new Vector3(1, 5, 1));

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(ray);

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            List<List<double>> rayTime = TimeCalculator.GetTime(rays);

            double epsilon = 1e-5;
            double[] expectedValues = { 0, 0.0050466, 0.007960288, 0.01997364 };

            for (int index = 0; index < expectedValues.Length; ++index)
                Assert.IsTrue(Math.Abs(rayTime[0][index] - expectedValues[index]) < epsilon);
        }

        //[Test]
        //public void GetPolarCoordinates_Test()
        //{
        //    Vector3 point = new Vector3(3, 4, 5);

        //    GameObject gameObject = new GameObject("RayGenerator");
        //    RayGenerator firstRay = gameObject.AddComponent<RayGenerator>();

        //    Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);

        //    double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
        //    double epsilon = 1e-8;
        //    Assert.IsTrue(Math.Abs(expectedValues[0] - polarCoordinates.Item1) < epsilon);
        //    Assert.IsTrue(Math.Abs(expectedValues[1] - polarCoordinates.Item2) < epsilon);
        //    Assert.IsTrue(Math.Abs(expectedValues[2] - polarCoordinates.Item3) < epsilon);
        //}

        //[Test]
        //public void GetCartesianCoordinates_Test()
        //{
        //    Vector3 point = new Vector3(3, 4, 5);

        //    GameObject gameObject = new GameObject("RayGenerator");
        //    RayGenerator firstRay = gameObject.AddComponent<RayGenerator>();

        //    Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);
        //    Vector3 recalculatedPoint = firstRay.GetCartesianCoordinates(polarCoordinates.Item1,
        //        polarCoordinates.Item2,
        //        polarCoordinates.Item3);

        //    double epsilon = 1e-8;
        //    Assert.IsTrue(Math.Abs(recalculatedPoint.x - point.x) < epsilon);
        //    Assert.IsTrue(Math.Abs(recalculatedPoint.y - point.y) < epsilon);
        //    Assert.IsTrue(Math.Abs(recalculatedPoint.z - point.z) < epsilon);
        //}

        //[Test]
        //public void LineIntersectionWithSphere_Test()
        //{

        //}

        //[Test]
        //public void RemoveDuplicateForTwoRays_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay first = new AcousticRay(origin);
        //    first.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    first.ColissionPoints.Add(new Vector3(1, 2, 1));
        //    first.ColissionPoints.Add(new Vector3(1, 3, 1));

        //    AcousticRay second = new AcousticRay(origin);
        //    second.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    second.ColissionPoints.Add(new Vector3(1, 2.002f, 1));
        //    second.ColissionPoints.Add(new Vector3(1, 3, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();

        //    rays.Add(first);
        //    rays.Add(second);

        //    List<AcousticRay> newRays = RayGenerator.RemoveDuplicates(rays);

        //    Assert.IsTrue(newRays.Count == 1);
        //}

        //[Test]
        //public void RemoveDuplicateForMultipleRays_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay first = new AcousticRay(origin);
        //    first.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    first.ColissionPoints.Add(new Vector3(1, 2, 1));
        //    first.ColissionPoints.Add(new Vector3(1, 3, 1));

        //    AcousticRay second = new AcousticRay(origin);
        //    second.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    second.ColissionPoints.Add(new Vector3(1, 2.002f, 1));
        //    second.ColissionPoints.Add(new Vector3(1, 3, 1));

        //    AcousticRay third = new AcousticRay(origin);
        //    third.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    third.ColissionPoints.Add(new Vector3(1, 2.002f, 1));
        //    third.ColissionPoints.Add(new Vector3(1, 3, 1));
        //    third.ColissionPoints.Add(new Vector3(1, 4, 1));

        //    AcousticRay fourth = new AcousticRay(origin);
        //    fourth.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    fourth.ColissionPoints.Add(new Vector3(1, 2.002f, 1));
        //    fourth.ColissionPoints.Add(new Vector3(1, 3.002f, 1));
        //    fourth.ColissionPoints.Add(new Vector3(1, 4.0005f, 1));

        //    AcousticRay fifth = new AcousticRay(origin);
        //    fifth.ColissionPoints.Add(new Vector3(1, 1, 1));
        //    fifth.ColissionPoints.Add(new Vector3(1, 2.002f, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();

        //    rays.Add(first);
        //    rays.Add(second);
        //    rays.Add(third);
        //    rays.Add(fourth);
        //    rays.Add(fifth);

        //    List<AcousticRay> newRays = RayGenerator.RemoveDuplicates(rays);

        //    Assert.IsTrue(newRays.Count == 3);
        //}
    }
}
