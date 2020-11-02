using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Vector3 = System.Numerics.Vector3;

namespace Tests
{
    public class LogicTests
    {
        [Test]
        public void RayDistanceZeroCollisionPoints_Test()
        {
            AcousticRay ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));

            Assert.IsTrue(Math.Abs(ray.GetDistance() - 8.66) < 1e-2);
        }

        [Test]
        public void RayDistance_Test()
        {
            AcousticRay ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            ray.collisionPoints.Add(new Vector3(1, 5, 3));
            ray.collisionPoints.Add(new Vector3(1, 2, 6));

            Assert.IsTrue(Math.Abs(ray.GetDistance() - 13.32) < 1e-2);
        }

        [Test]
        public void ZeroIntersectedRays_Test()
        {
            List<MicrophoneSphere> microphone = new List<MicrophoneSphere>(){
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());

            });

            Assert.IsTrue(newRays.Count == 0);
        }

        [Test]
        public void IntersectedRays_Test()
        {
            List<MicrophoneSphere> microphone = new List<MicrophoneSphere>(){
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f) };
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> newRays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            Assert.IsTrue(newRays.Count == 1);
            Assert.IsTrue(newRays[0].collisionPoints.Count == 0);
        }

        [Test]
        public void ComputeIntensityForDirectRay_Test()
        {
            List<MicrophoneSphere> microphone = new List<MicrophoneSphere>(){
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f) };
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            Dictionary<int, List<AcousticRay>> myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].id, rays);

            IntensityCalculator intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[0][0] - 0.02038056768) < 1e-3);
        }

        [Test]
        public void ComputeIntensityForRay_Test()
        {
            List<MicrophoneSphere> microphone = new List<MicrophoneSphere>() {
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f) };
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                500,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            Dictionary<int, List<AcousticRay>> myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].id, rays);

            IntensityCalculator intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            double epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[0][9] - 0.0005087481) < epsilon);
        }

        [Test]
        public void GetPolarCoordinates_Test()
        {
            Vector3 point = new Vector3(3, 4, 5);

            Vector3 polarCoordinates = DegreesRadiansConverter.TransformToPolarCoordinates(point);

            double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
            double epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(expectedValues[0] - polarCoordinates.X) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[1] - polarCoordinates.Y) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[2] - polarCoordinates.Z) < epsilon);
        }

        [Test]
        public void GetCartesianCoordinates_Test()
        {
            Vector3 point = new Vector3(3, 4, 5);

            Vector3 polarCoordinates = DegreesRadiansConverter.TransformToPolarCoordinates(point);
            Vector3 recalculatedPoint = DegreesRadiansConverter.TransformToCartesianCoordinates(polarCoordinates.X,
                polarCoordinates.Y,
                polarCoordinates.Z);

            double epsilon = 1e-8;
            Assert.IsTrue(Math.Abs(recalculatedPoint.X - point.X) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Y - point.Y) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Z - point.Z) < epsilon);
        }

        //[Test]
        //public void CollisionPoints_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    Vector3 microphone = new Vector3(1, 5, 3);

        //    AcousticRay firstRay = new AcousticRay(origin, microphone);
        //    AcousticRay secondRay = new AcousticRay(origin, microphone);
        //    AcousticRay thirdRay = new AcousticRay(origin, microphone);

        //    List<Vector3> firstRayVectors = new List<Vector3>()
        //    {
        //        new Vector3(1, 2, 3),
        //        new Vector3(1, 2, 4),
        //        new Vector3(1, 3, 3),
        //        new Vector3(1, 5.02f, 3)
        //    };

        //    List<Vector3> secondRayVectors = new List<Vector3>()
        //    {
        //        new Vector3(1, 2, 3),
        //        new Vector3(1, 2, 4),
        //        new Vector3(1, 2, 5)
        //    };

        //    List<Vector3> thirdRayVectors = new List<Vector3>()
        //    {
        //        new Vector3(1, 2, 5),
        //        new Vector3(1, 3, 5),
        //        new Vector3(1, 4, 5),
        //        new Vector3(1, 5, 5),
        //        new Vector3(1, 6, 3.002f)
        //    };

        //    for (int index = 0; index < firstRayVectors.Count; ++index)
        //        firstRay.CollisionPoints.Add(firstRayVectors[index]);

        //    for (int index = 0; index < secondRayVectors.Count; ++index)
        //        secondRay.CollisionPoints.Add(secondRayVectors[index]);

        //    for (int index = 0; index < thirdRayVectors.Count; ++index)
        //        thirdRay.CollisionPoints.Add(thirdRayVectors[index]);

        //    Assert.IsTrue(firstRay.CollisionPoints.Count == 4);
        //    Assert.IsTrue(secondRay.CollisionPoints.Count == 3);
        //    Assert.IsTrue(thirdRay.CollisionPoints.Count == 5);
        //}

        //[Test]
        //public void DistanceForZeroCollisionPoints_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    Vector3 microphone = new Vector3(1, 5, 3);

        //    AcousticRay firstRay = new AcousticRay(origin, microphone);

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(firstRay);

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        //    distanceCalculator.ComputeDistances();

        //    Assert.IsTrue(Math.Abs(rays[0].Distances[0] - 5.92) < 1e-2);
        //}

        //[Test]
        //public void DistanceForMultipleCollisionPoints_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    Vector3 microphone = new Vector3(1, 5, 3);

        //    AcousticRay firstRay = new AcousticRay(origin, microphone);

        //    List<Vector3> firstRayVectors = new List<Vector3>()
        //    {
        //        new Vector3(1, 2, 3),
        //        new Vector3(1, 2, 4),
        //        new Vector3(1, 3, 3),
        //        new Vector3(1, 5.02f, 3)
        //    };

        //    for (int index = 0; index < firstRayVectors.Count; ++index)
        //        firstRay.CollisionPoints.Add(firstRayVectors[index]);

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(firstRay);

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        //    distanceCalculator.ComputeDistances();

        //    List<float> distancesResults = new List<float>() { 3.74f, 4.74f, 6.15f, 8.17f };
        //    for (int index = 0; index < distancesResults.Count; ++index)
        //        Assert.IsTrue(Math.Abs(distancesResults[index] - rays[0].Distances[index]) < 1e-2);
        //}

        //[Test]
        //public void TimeForMultipleCollisionPoints_Test()
        //{
        //    Vector3 origin = new Vector3(0, 0, 0);
        //    Vector3 microphone = new Vector3(1, 5, 3);

        //    AcousticRay firstRay = new AcousticRay(origin, microphone);

        //    List<Vector3> firstRayVectors = new List<Vector3>()
        //    {
        //        new Vector3(1, 2, 3),
        //        new Vector3(1, 2, 4),
        //        new Vector3(1, 3, 3),
        //        new Vector3(1, 5.02f, 3)
        //    };

        //    for (int index = 0; index < firstRayVectors.Count; ++index)
        //        firstRay.CollisionPoints.Add(firstRayVectors[index]);

        //    List<AcousticRay> rays = new List<AcousticRay>();
        //    rays.Add(firstRay);

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(rays);
        //    distanceCalculator.ComputeDistances();

        //    List<List<double>> times = TimeCalculator.GetTime(rays);

        //    List<float> timeResults = new List<float>() { 3.74f/ 343.21f,
        //       4.74f / 343.21f,
        //       6.15f / 343.21f,
        //       8.17f / 343.21f };

        //    for (int index = 0; index < timeResults.Count; ++index)
        //        Assert.IsTrue(Math.Abs(timeResults[index] - times[0][index]) < 1e-2);
        //}

        //[Test]
        //public void Rays11_Test()
        //{
        //    List<MicrophoneSphere> microphone = new List<MicrophoneSphere>() {
        //        new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
        //    RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
        //        microphone,
        //        1000,
        //        3,
        //        200);
        //    rayGeometryGenerator.GenerateRays();

        //    List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

        //    rays.Sort(delegate (AcousticRay first, AcousticRay second)
        //    {
        //        return first.Distance.CompareTo(second.Distance);
        //    });

        //    Dictionary<int, List<AcousticRay>> myRays = new Dictionary<int, List<AcousticRay>>();
        //    myRays.Add(microphone[0].Id, rays);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
        //    intensityCalculator.ComputePower();

        //    DistanceCalculator distanceCalculator = new DistanceCalculator(myRays, microphone);
        //    distanceCalculator.ComputeDistances();

        //    //List<List<double>> times = TimeCalculator.GetTime(myRays);

        //    double epsilon = 1e-4;
        //    Assert.IsTrue(Math.Abs(intensityCalculator.Intensities[0][0].Real - 0.0098243) < epsilon);
        //    Assert.IsTrue(Math.Abs(intensityCalculator.Intensities[0][1].Real - 0.0049536) < epsilon);
        //}

        //[Test]
        //public void Rays11Phase_Test()
        //{
        //    List<MicrophoneSphere> microphone = new List<MicrophoneSphere>() {
        //        new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
        //    RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
        //        microphone,
        //        1000,
        //        3,
        //        200);
        //    rayGeometryGenerator.GenerateRays();

        //    List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

        //    rays.Sort(delegate (AcousticRay first, AcousticRay second)
        //    {
        //        return first.Distance.CompareTo(second.Distance);
        //    });

        //    Dictionary<int, List<AcousticRay>> myRays = new Dictionary<int, List<AcousticRay>>();
        //    myRays.Add(microphone[0].Id, rays);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
        //    intensityCalculator.ComputePower();

        //    PhaseCalculator phaseCalculator = new PhaseCalculator(myRays, microphone, intensityCalculator.Intensities);
        //    phaseCalculator.ComputePhase(1000);

        //    double epsilon = 1e-4;
        //    Assert.IsTrue(Math.Abs(phaseCalculator.EchogramMagnitudePhase[0][0].Real + 0.002589105375) < epsilon);
        //}

        //[Test]
        //public void RaysPhase_Test()
        //{
        //    List<MicrophoneSphere> microphone = new List<MicrophoneSphere>() {
        //        new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
        //    RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
        //        microphone,
        //        63000,
        //        3,
        //        200);
        //    rayGeometryGenerator.GenerateRays();

        //    List<AcousticRay> rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

        //    rays.Sort(delegate (AcousticRay first, AcousticRay second)
        //    {
        //        return first.Distance.CompareTo(second.Distance);
        //    });

        //    Dictionary<int, List<AcousticRay>> myRays = new Dictionary<int, List<AcousticRay>>();
        //    myRays.Add(microphone[0].Id, rays);

        //    IntensityCalculator intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
        //    intensityCalculator.ComputePower();
        //    intensityCalculator.TransformIntensitiesToPressure();

        //    PhaseCalculator phaseCalculator = new PhaseCalculator(myRays, microphone, intensityCalculator.Intensities);
        //    phaseCalculator.ComputePhase(1000);

        //    double epsilon = 1e-4;
        //    Assert.IsTrue(Math.Abs(phaseCalculator.EchogramMagnitudePhase[0][0] + 0.002589105375) < epsilon);
        //}
    }
}
