using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

namespace Tests
{
    public class TestEditorMode
    {
        [Test]
        public void RayDistanceZeroCollisionPoints_Test()
        {
            AcousticRay ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));

            Assert.IsTrue(Math.Abs(ray.Distance - 8.66) < 1e-2);
        }

        [Test]
        public void RayDistance_Test()
        {
            AcousticRay ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            ray.CollisionPoints.Add(new Vector3(1, 5, 3));
            ray.CollisionPoints.Add(new Vector3(1, 2, 6));

            Assert.IsTrue(Math.Abs(ray.Distance - 13.32) < 1e-2);
        }

        [Test]
        public void ZeroIntersectedRays_Test()
        {
            MicrophoneSphere microphone = new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f);
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone.Center,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);

            });

            Assert.IsTrue(newRays.Count == 0);
        }

        [Test]
        public void IntersectedRays_Test()
        {
            MicrophoneSphere microphone = new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f);
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone.Center,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);
            });

            Assert.IsTrue(newRays.Count == 1);
            Assert.IsTrue(newRays[0].CollisionPoints.Count == 0);
        }

        [Test]
        public void ComputeIntensityForDirectRay_Test()
        {
            MicrophoneSphere microphone = new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f);
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone.Center,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);
            });

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            var value = PressureConverter.ConvertIntensityToPressure(rays[0].Intensities[0]);
            Assert.IsTrue(Math.Abs(rays[0].Intensities[0] - 0.02038) < 1e-3);
        }

        [Test]
        public void ComputeIntensityForRay_Test()
        {
            MicrophoneSphere microphone = new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f);
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone.Center,
                500,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);
            });

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            double epsilon = 1e-7;
            Assert.IsTrue(Math.Abs(rays[10].Intensities[0] - 0.0032034272) < epsilon);
            Assert.IsTrue(Math.Abs(rays[10].Intensities[1] - 0.0020316458) < epsilon);
            Assert.IsTrue(Math.Abs(rays[10].Intensities[2] - 0.0005087481) < epsilon);
        }

        [Test]
        public void CollisionPoints_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 microphone = new Vector3(1, 5, 3);

            AcousticRay firstRay = new AcousticRay(origin, microphone);
            AcousticRay secondRay = new AcousticRay(origin, microphone);
            AcousticRay thirdRay = new AcousticRay(origin, microphone);

            List<Vector3> firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            List<Vector3> secondRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 2, 5)
            };

            List<Vector3> thirdRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 5),
                new Vector3(1, 3, 5),
                new Vector3(1, 4, 5),
                new Vector3(1, 5, 5),
                new Vector3(1, 6, 3.002f)
            };

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.CollisionPoints.Add(firstRayVectors[index]);

            for (int index = 0; index < secondRayVectors.Count; ++index)
                secondRay.CollisionPoints.Add(secondRayVectors[index]);

            for (int index = 0; index < thirdRayVectors.Count; ++index)
                thirdRay.CollisionPoints.Add(thirdRayVectors[index]);

            Assert.IsTrue(firstRay.CollisionPoints.Count == 4);
            Assert.IsTrue(secondRay.CollisionPoints.Count == 3);
            Assert.IsTrue(thirdRay.CollisionPoints.Count == 5);
        }

        [Test]
        public void DistanceForZeroCollisionPoints_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 microphone = new Vector3(1, 5, 3);

            AcousticRay firstRay = new AcousticRay(origin, microphone);

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(firstRay);

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            Assert.IsTrue(Math.Abs(rays[0].Distances[0] - 5.92) < 1e-2);
        }

        [Test]
        public void DistanceForMultipleCollisionPoints_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 microphone = new Vector3(1, 5, 3);

            AcousticRay firstRay = new AcousticRay(origin, microphone);

            List<Vector3> firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.CollisionPoints.Add(firstRayVectors[index]);

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(firstRay);

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            List<float> distancesResults = new List<float>() { 3.74f, 4.74f, 6.15f, 8.17f };
            for (int index = 0; index < distancesResults.Count; ++index)
                Assert.IsTrue(Math.Abs(distancesResults[index] - rays[0].Distances[index]) < 1e-2);
        }

        [Test]
        public void TimeForMultipleCollisionPoints_Test()
        {
            Vector3 origin = new Vector3(0, 0, 0);
            Vector3 microphone = new Vector3(1, 5, 3);

            AcousticRay firstRay = new AcousticRay(origin, microphone);

            List<Vector3> firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.CollisionPoints.Add(firstRayVectors[index]);

            List<AcousticRay> rays = new List<AcousticRay>();
            rays.Add(firstRay);

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            List<List<double>> times = TimeCalculator.GetTime(rays);

           List<float> timeResults = new List<float>() { 3.74f/ 343.21f,
               4.74f / 343.21f,
               6.15f / 343.21f,
               8.17f / 343.21f };

            for (int index = 0; index < timeResults.Count; ++index)
                Assert.IsTrue(Math.Abs(timeResults[index] - times[0][index]) < 1e-2);
        }

        [Test]
        public void Rays11_Test()
        {
            MicrophoneSphere microphone = new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f);
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
                microphone.Center,
                1000,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.Distance.CompareTo(second.Distance);
            });

            IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
            intensityCalculator.ComputePower();

            DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
            distanceCalculator.ComputeDistances();

            List<List<double>> times = TimeCalculator.GetTime(rays);

            double epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(rays[0].Intensities[0] - 0.0098243202) < epsilon);
            Assert.IsTrue(Math.Abs(rays[1].Intensities[0] - 0.0063655) < epsilon);
            Assert.IsTrue(Math.Abs(rays[1].Intensities[1] - 0.0049525) < epsilon);
        }

        //[Test]
        //public void OneRayTwoPointsIntensities_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay ray = new AcousticRay(origin);
        //    ray.CollisionPoints.Add(new Vector3(1, 1, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(ray);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
        //    intensityCalculator.ComputePower();

        //    double epsilon = 1e-5;
        //    Assert.IsTrue(Math.Abs(rays[0].Intensities[0] - 0.07957) < epsilon);
        //    Assert.IsTrue(Math.Abs(rays[0].Intensities[1] - 0.0106613589) < epsilon);
        //}

        //[Test]
        //public void OneRayMultiplePointsIntensities_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay ray = new AcousticRay(origin);
        //    ray.CollisionPoints.Add(new Vector3(1, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(2, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(1, 5, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(ray);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
        //    intensityCalculator.ComputePower();

        //    double epsilon = 1e-5;
        //    double[] expectedValues = { 0.07957747, 0.0106613589, 0.00571340513, 0.001289675 };

        //    for (int index = 0; index < expectedValues.Length; ++index)
        //        Assert.IsTrue(Math.Abs(rays[0].Intensities[index] - expectedValues[index]) < epsilon);
        //}

        //[Test]
        //public void TwoRaysMultiplePointsIntensities_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay first = new AcousticRay(origin);
        //    first.CollisionPoints.Add(new Vector3(1, 1, 1));
        //    first.CollisionPoints.Add(new Vector3(2, 1, 1));
        //    first.CollisionPoints.Add(new Vector3(1, 5, 1));

        //    AcousticRay second = new AcousticRay(origin);
        //    second.CollisionPoints.Add(new Vector3(1, 1, 1));
        //    second.CollisionPoints.Add(new Vector3(2, 1, 1));
        //    second.CollisionPoints.Add(new Vector3(1, 5, 1));
        //    second.CollisionPoints.Add(new Vector3(1, 8, 2));
        //    second.CollisionPoints.Add(new Vector3(3, 5, 3));

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(first);
        //    rays.Add(second);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(rays, 1);
        //    intensityCalculator.ComputePower();

        //    double epsilon = 1e-5;
        //    double[] expectedValuesFirstRay = { 0.07957747, 0.0106613589, 0.00571340513, 0.001289675 };
        //    double[] expectedValuesSecondRay = { 0.07957747, 0.0106613589,
        //                                         0.00571340513, 0.001289675,
        //                                         0.0006555853067, 0.00036531784 };

        //    for (int index = 0; index < expectedValuesFirstRay.Length; ++index)
        //        Assert.IsTrue(Math.Abs(rays[0].Intensities[index] - expectedValuesFirstRay[index]) < epsilon);

        //    for (int index = 0; index < expectedValuesSecondRay.Length; ++index)
        //        Assert.IsTrue(Math.Abs(rays[1].Intensities[index] - expectedValuesSecondRay[index]) < epsilon);
        //}

        //[Test]
        //public void OneRayComputeDistances_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay ray = new AcousticRay(origin);
        //    ray.CollisionPoints.Add(new Vector3(1, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(2, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(1, 5, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(ray);

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        //    distanceCalculator.ComputeDistances();

        //    double epsilon = 1e-2;
        //    double[] expectedValues = { 0, 1.73, 2.73, 6.85};

        //    for (int index = 0; index < expectedValues.Length; ++index)
        //        Assert.IsTrue(Math.Abs(rays[0].Distances[index] - expectedValues[index]) < epsilon);
        //}

        //[Test]
        //public void OneRayComputeTime_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    AcousticRay ray = new AcousticRay(origin);
        //    ray.CollisionPoints.Add(new Vector3(1, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(2, 1, 1));
        //    ray.CollisionPoints.Add(new Vector3(1, 5, 1));

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(ray);

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        //    distanceCalculator.ComputeDistances();

        //    List<List<double>> rayTime = TimeCalculator.GetTime(rays);

        //    double epsilon = 1e-5;
        //    double[] expectedValues = { 0, 0.0050466, 0.007960288, 0.01997364 };

        //    for (int index = 0; index < expectedValues.Length; ++index)
        //        Assert.IsTrue(Math.Abs(rayTime[0][index] - expectedValues[index]) < epsilon);
        //}

        ////[Test]
        ////public void GetPolarCoordinates_Test()
        ////{
        ////    Vector3 point = new Vector3(3, 4, 5);

        ////    GameObject gameObject = new GameObject("RayGenerator");
        ////    RayGenerator firstRay = gameObject.AddComponent<RayGenerator>();

        ////    Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);

        ////    double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
        ////    double epsilon = 1e-8;
        ////    Assert.IsTrue(Math.Abs(expectedValues[0] - polarCoordinates.Item1) < epsilon);
        ////    Assert.IsTrue(Math.Abs(expectedValues[1] - polarCoordinates.Item2) < epsilon);
        ////    Assert.IsTrue(Math.Abs(expectedValues[2] - polarCoordinates.Item3) < epsilon);
        ////}

        ////[Test]
        ////public void GetCartesianCoordinates_Test()
        ////{
        ////    Vector3 point = new Vector3(3, 4, 5);

        ////    GameObject gameObject = new GameObject("RayGenerator");
        ////    RayGenerator firstRay = gameObject.AddComponent<RayGenerator>();

        ////    Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);
        ////    Vector3 recalculatedPoint = firstRay.GetCartesianCoordinates(polarCoordinates.Item1,
        ////        polarCoordinates.Item2,
        ////        polarCoordinates.Item3);

        ////    double epsilon = 1e-8;
        ////    Assert.IsTrue(Math.Abs(recalculatedPoint.x - point.x) < epsilon);
        ////    Assert.IsTrue(Math.Abs(recalculatedPoint.y - point.y) < epsilon);
        ////    Assert.IsTrue(Math.Abs(recalculatedPoint.z - point.z) < epsilon);
        ////}

        ////[Test]
        ////public void LineIntersectionWithSphere_Test()
        ////{

        ////}

        ////[Test]
        ////public void RemoveDuplicateForTwoRays_Test()
        ////{
        ////    Vector3 origin = new Vector3(0, 0, 0);
        ////    AcousticRay first = new AcousticRay(origin);
        ////    first.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    first.CollisionPoints.Add(new Vector3(1, 2, 1));
        ////    first.CollisionPoints.Add(new Vector3(1, 3, 1));

        ////    AcousticRay second = new AcousticRay(origin);
        ////    second.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    second.CollisionPoints.Add(new Vector3(1, 2.002f, 1));
        ////    second.CollisionPoints.Add(new Vector3(1, 3, 1));

        ////    List<AcousticRay> rays = new List<AcousticRay>();

        ////    rays.Add(first);
        ////    rays.Add(second);

        ////    List<AcousticRay> newRays = RayGenerator.RemoveDuplicates(rays);

        ////    Assert.IsTrue(newRays.Count == 1);
        ////}

        ////[Test]
        ////public void RemoveDuplicateForMultipleRays_Test()
        ////{
        ////    Vector3 origin = new Vector3(0, 0, 0);
        ////    AcousticRay first = new AcousticRay(origin);
        ////    first.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    first.CollisionPoints.Add(new Vector3(1, 2, 1));
        ////    first.CollisionPoints.Add(new Vector3(1, 3, 1));

        ////    AcousticRay second = new AcousticRay(origin);
        ////    second.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    second.CollisionPoints.Add(new Vector3(1, 2.002f, 1));
        ////    second.CollisionPoints.Add(new Vector3(1, 3, 1));

        ////    AcousticRay third = new AcousticRay(origin);
        ////    third.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    third.CollisionPoints.Add(new Vector3(1, 2.002f, 1));
        ////    third.CollisionPoints.Add(new Vector3(1, 3, 1));
        ////    third.CollisionPoints.Add(new Vector3(1, 4, 1));

        ////    AcousticRay fourth = new AcousticRay(origin);
        ////    fourth.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    fourth.CollisionPoints.Add(new Vector3(1, 2.002f, 1));
        ////    fourth.CollisionPoints.Add(new Vector3(1, 3.002f, 1));
        ////    fourth.CollisionPoints.Add(new Vector3(1, 4.0005f, 1));

        ////    AcousticRay fifth = new AcousticRay(origin);
        ////    fifth.CollisionPoints.Add(new Vector3(1, 1, 1));
        ////    fifth.CollisionPoints.Add(new Vector3(1, 2.002f, 1));

        ////    List<AcousticRay> rays = new List<AcousticRay>();

        ////    rays.Add(first);
        ////    rays.Add(second);
        ////    rays.Add(third);
        ////    rays.Add(fourth);
        ////    rays.Add(fifth);

        ////    List<AcousticRay> newRays = RayGenerator.RemoveDuplicates(rays);

        ////    Assert.IsTrue(newRays.Count == 3);
        ////}
    }
}
