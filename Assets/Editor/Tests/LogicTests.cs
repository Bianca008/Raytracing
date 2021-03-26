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
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));

            Assert.IsTrue(Math.Abs(ray.GetDistance() - 8.66) < 1e-2);
        }

        [Test]
        public void RayDistance_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            ray.CollisionPoints.Add(new Vector3(1, 5, 3));
            ray.CollisionPoints.Add(new Vector3(1, 2, 6));

            Assert.IsTrue(Math.Abs(ray.GetDistance() - 13.32) < 1e-2);
        }

        [Test]
        public void ZeroIntersectedRaysWithCollision_Test()
        {
            var microphone = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.3f, 1.7f), 0.1f)
            };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                2,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var newRays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            newRays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));

            Assert.IsTrue(newRays.Count == 1);
        }

        [Test]
        public void IntersectedRays_Test()
        {
            var microphone = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f)
            };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var newRays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            newRays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));

            Assert.IsTrue(newRays.Count == 1);
            Assert.IsTrue(newRays[0].CollisionPoints.Count == 0);
        }

        [Test]
        public void ComputeIntensityForDirectRay_Test()
        {
            var microphone = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f)
            };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].Id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            Assert.IsTrue(Math.Abs(intensityCalculator.Intensities[microphone[0].Id][0] - 0.02038056768) < 1e-3);
        }

        [Test]
        public void GetPolarCoordinates_Test()
        {
            var point = new Vector3(3, 4, 5);

            var polarCoordinates = DegreesRadiansConverter.TransformToPolarCoordinates(point);

            double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
            double epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(expectedValues[0] - polarCoordinates.X) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[1] - polarCoordinates.Y) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[2] - polarCoordinates.Z) < epsilon);
        }

        [Test]
        public void GetCartesianCoordinates_Test()
        {
            var point = new Vector3(3, 4, 5);

            var polarCoordinates = DegreesRadiansConverter.TransformToPolarCoordinates(point);
            var recalculatedPoint = DegreesRadiansConverter.TransformToCartesianCoordinates(polarCoordinates.X,
                polarCoordinates.Y,
                polarCoordinates.Z);

            double epsilon = 1e-8;
            Assert.IsTrue(Math.Abs(recalculatedPoint.X - point.X) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Y - point.Y) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Z - point.Z) < epsilon);
        }

        [Test]
        public void TruncateRayZeroCollisionPoints_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            var newRay = ray.TruncateRay(0, new Vector3(1, 5, 7));

            Assert.IsTrue(newRay.Source == new Vector3(0, 0, 0) && newRay.MicrophonePosition == new Vector3(1, 5f, 7));
            Assert.IsTrue(newRay.CollisionPoints.Count == 0 && newRay.AcousticMaterials.Count == 0);
        }

        [Test]
        public void TruncateRayForInvalidPosition_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            var newRay = ray.TruncateRay(5, new Vector3(1, 5, 7));

            Assert.IsTrue(newRay.Source == new Vector3(0, 0, 0) && newRay.MicrophonePosition == new Vector3(1, 5f, 7));
            Assert.IsTrue(newRay.CollisionPoints.Count == 0 && newRay.AcousticMaterials.Count == 0);
        }

        [Test]
        public void TruncateRayUsualCase_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            ray.CollisionPoints.Add(new Vector3(1, 5, 3));
            ray.CollisionPoints.Add(new Vector3(1, 2, 6));
            ray.CollisionPoints.Add(new Vector3(1, 5, 16));
            ray.CollisionPoints.Add(new Vector3(1, 2, 20));

            var gameObject = new GameObject();
            gameObject.AddComponent<AcousticMaterial>();
            ray.AcousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.AcousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.AcousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.AcousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());

            var microphonePos = new Vector3(1, 5.02f, 16);
            var newRay = ray.TruncateRay(3, microphonePos);

            Assert.IsTrue(newRay.Source == new Vector3(0, 0, 0) && newRay.MicrophonePosition == new Vector3(1, 5.02f, 16));
            Assert.IsTrue(newRay.CollisionPoints.Count == 3 && newRay.AcousticMaterials.Count == 3);
        }

        [Test]
        public void CollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphone = new Vector3(1, 5, 3);

            var firstRay = new AcousticRay(origin, microphone);
            var secondRay = new AcousticRay(origin, microphone);
            var thirdRay = new AcousticRay(origin, microphone);

            var firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            var secondRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 2, 5)
            };

            var thirdRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 5),
                new Vector3(1, 3, 5),
                new Vector3(1, 4, 5),
                new Vector3(1, 5, 5),
                new Vector3(1, 6, 3.002f)
            };

            foreach (var vec in firstRayVectors)
                firstRay.CollisionPoints.Add(vec);

            foreach (var vec in secondRayVectors)
                secondRay.CollisionPoints.Add(vec);

            foreach (var vec in thirdRayVectors)
                thirdRay.CollisionPoints.Add(vec);

            Assert.IsTrue(firstRay.CollisionPoints.Count == 4 && secondRay.CollisionPoints.Count == 3 && thirdRay.CollisionPoints.Count == 5);
        }

        [Test]
        public void DistanceForZeroCollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new Vector3(1, 5, 3), 0.5f)
            };

            var firstRay = new AcousticRay(origin, microphones[0].Center);

            var rays = new List<AcousticRay>
            {
                firstRay
            };

            Assert.IsTrue(Math.Abs(rays[0].GetDistance() - 5.92) < 1e-2);
        }

        [Test]
        public void DistanceForMultipleCollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere( new Vector3(1, 5, 3), 0.5f)
            };

            var firstRay = new AcousticRay(origin, microphones[0].Center);

            var firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            foreach (var vec in firstRayVectors)
                firstRay.CollisionPoints.Add(vec);

            var rays = new List<AcousticRay>
            {
                firstRay
            };

            var raysDictionary = new Dictionary<int, List<AcousticRay>>();
            raysDictionary[microphones[0].Id] = rays;

            var distanceCalculator = new DistanceCalculator(raysDictionary, microphones);
            distanceCalculator.ComputeDistances();

            var distancesResults = new List<float>() { 3.74f, 4.74f, 6.15f, 8.17f, 8.19f };
            Assert.IsTrue(Math.Abs(distancesResults[4] - raysDictionary[microphones[0].Id][0].GetDistance()) < 1e-2);
        }

        [Test]
        public void TimeForMultipleCollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new Vector3(1, 5, 3), 0.5f)
            };

            var firstRay = new AcousticRay(origin, microphones[0].Center);

            var firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            foreach (var vec in firstRayVectors)
                firstRay.CollisionPoints.Add(vec);

            var rays = new List<AcousticRay>
            {
                firstRay
            };

            var raysDictionary = new Dictionary<int, List<AcousticRay>>();
            raysDictionary[microphones[0].Id] = rays;

            var distanceCalculator = new DistanceCalculator(raysDictionary, microphones);
            distanceCalculator.ComputeDistances();

            var times = TimeCalculator.GetTime(raysDictionary, microphones);

            Assert.IsTrue(Math.Abs(times[microphones[0].Id][0] - 0.0238) < 1e-3);
        }

        [Test]
        public void Rays_Test()
        {
            var microphone = new List<MicrophoneSphere>() {
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
                microphone,
                70000,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].Id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            var distanceCalculator = new DistanceCalculator(myRays, microphone);
            distanceCalculator.ComputeDistances();

            double epsilon = 1e-4;
            Assert.IsTrue(Math.Abs(intensityCalculator.Intensities[microphone[0].Id][0] - 0.0098243) < epsilon);
        }

        [Test]
        public void RaysPhase_Test()
        {
            var microphone = new List<MicrophoneSphere>() {
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
                microphone,
                1000,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort((first, second) => first.GetDistance().CompareTo(second.GetDistance()));

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].Id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            var phaseCalculator = new PhaseCalculator(myRays, microphone, intensityCalculator.Intensities);
            phaseCalculator.ComputePhase(1000);

            double epsilon = 1e-4;
            Assert.IsTrue(Math.Abs(phaseCalculator.EchogramMagnitudePhase[microphone[0].Id][0].Real + 0.002589105375) < epsilon);
        }
    }
}
