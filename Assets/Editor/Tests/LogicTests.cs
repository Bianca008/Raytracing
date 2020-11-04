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
            ray.collisionPoints.Add(new Vector3(1, 5, 3));
            ray.collisionPoints.Add(new Vector3(1, 2, 6));

            Assert.IsTrue(Math.Abs(ray.GetDistance() - 13.32) < 1e-2);
        }

        [Test]
        public void ZeroIntersectedRays_Test()
        {
            var microphone = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f)
            };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                3,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var newRays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            newRays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            Assert.IsTrue(newRays.Count == 0);
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

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[microphone[0].id][0] - 0.02038056768) < 1e-3);
        }

        [Test]
        public void ComputeIntensityForRay_Test()
        {
            var microphone = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(new System.Numerics.Vector3(0, 0, 1.976f), 0.1f)
            };
            var rayGeometryGenerator = new RayGeometry(new Vector3(0, 0, 0),
                microphone,
                500,
                3,
                200);
            rayGeometryGenerator.GenerateRays();

            var rays = rayGeometryGenerator.GetIntersectedRays(microphone[0]);

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            var epsilon = 1e-5;
            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[microphone[0].id][9] - 0.0005087481) < epsilon);
        }

        [Test]
        public void GetPolarCoordinates_Test()
        {
            var point = new Vector3(3, 4, 5);

            var polarCoordinates = DegreesRadiansConverter.TransformToPolarCoordinates(point);

            double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
            var epsilon = 1e-5;
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

            var epsilon = 1e-8;
            Assert.IsTrue(Math.Abs(recalculatedPoint.X - point.X) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Y - point.Y) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.Z - point.Z) < epsilon);
        }

        [Test]
        public void TruncateRayZeroCollisionPoints_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            var newRay = ray.TruncateRay(0, new Vector3(1, 5, 7));

            Assert.IsTrue(newRay.source == new Vector3(0, 0, 0) && newRay.microphonePosition == new Vector3(1, 5f, 7));
            Assert.IsTrue(newRay.collisionPoints.Count == 0 && newRay.acousticMaterials.Count == 0);
        }

        [Test]
        public void TruncateRayForInvalidPosition_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            var newRay = ray.TruncateRay(5, new Vector3(1, 5, 7));

            Assert.IsTrue(newRay.source == new Vector3(0, 0, 0) && newRay.microphonePosition == new Vector3(1, 5f, 7));
            Assert.IsTrue(newRay.collisionPoints.Count == 0 && newRay.acousticMaterials.Count == 0);
        }

        [Test]
        public void TruncateRayUsualCase_Test()
        {
            var ray = new AcousticRay(new Vector3(0, 0, 0), new Vector3(1, 5, 7));
            ray.collisionPoints.Add(new Vector3(1, 5, 3));
            ray.collisionPoints.Add(new Vector3(1, 2, 6));
            ray.collisionPoints.Add(new Vector3(1, 5, 16));
            ray.collisionPoints.Add(new Vector3(1, 2, 20));

            var gameObject = new GameObject();
            gameObject.AddComponent<AcousticMaterial>();
            ray.acousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.acousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.acousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());
            ray.acousticMaterials.Add(gameObject.GetComponent<AcousticMaterial>());

            Vector3 microphonePos = new Vector3(1, 5.02f, 16);
            var newRay = ray.TruncateRay(3, microphonePos);

            Assert.IsTrue(newRay.source == new Vector3(0, 0, 0) && newRay.microphonePosition == new Vector3(1, 5.02f, 16));
            Assert.IsTrue(newRay.collisionPoints.Count == 3 && newRay.acousticMaterials.Count == 3);
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

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.collisionPoints.Add(firstRayVectors[index]);

            for (int index = 0; index < secondRayVectors.Count; ++index)
                secondRay.collisionPoints.Add(secondRayVectors[index]);

            for (int index = 0; index < thirdRayVectors.Count; ++index)
                thirdRay.collisionPoints.Add(thirdRayVectors[index]);

            Assert.IsTrue(firstRay.collisionPoints.Count == 4 && secondRay.collisionPoints.Count == 3 && thirdRay.collisionPoints.Count == 5);
        }

        [Test]
        public void DistanceForZeroCollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphonePosition = new Vector3(1, 5, 3);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(microphonePosition, 0.5f)
            };


            var firstRay = new AcousticRay(origin, microphonePosition);

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
            var microphonePosition = new Vector3(1, 5, 3);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(microphonePosition, 0.5f)
            };

            var firstRay = new AcousticRay(origin, microphonePosition);

            List<Vector3> firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.collisionPoints.Add(firstRayVectors[index]);

            var rays = new List<AcousticRay>
            {
                firstRay
            };

            var raysDictionary = new Dictionary<int, List<AcousticRay>>();
            raysDictionary[microphones[0].id] = rays;

            var distanceCalculator = new DistanceCalculator(raysDictionary, microphones);
            distanceCalculator.ComputeDistances();

            List<float> distancesResults = new List<float>() { 3.74f, 4.74f, 6.15f, 8.17f, 8.19f };
            Assert.IsTrue(Math.Abs(distancesResults[4] - raysDictionary[microphones[0].id][0].GetDistance()) < 1e-2);
        }

        [Test]
        public void TimeForMultipleCollisionPoints_Test()
        {
            var origin = new Vector3(0, 0, 0);
            var microphonePosition = new Vector3(1, 5, 3);
            var microphones = new List<MicrophoneSphere>()
            {
                new MicrophoneSphere(microphonePosition, 0.5f)
            };

            var firstRay = new AcousticRay(origin, microphonePosition);

            var firstRayVectors = new List<Vector3>()
            {
                new Vector3(1, 2, 3),
                new Vector3(1, 2, 4),
                new Vector3(1, 3, 3),
                new Vector3(1, 5.02f, 3)
            };

            for (int index = 0; index < firstRayVectors.Count; ++index)
                firstRay.collisionPoints.Add(firstRayVectors[index]);

            var rays = new List<AcousticRay>
            {
                firstRay
            };

            var raysDictionary = new Dictionary<int, List<AcousticRay>>();
            raysDictionary[microphones[0].id] = rays;

            var distanceCalculator = new DistanceCalculator(raysDictionary, microphones);
            distanceCalculator.ComputeDistances();

            var times = TimeCalculator.GetTime(raysDictionary, microphones);

            Assert.IsTrue(Math.Abs(times[microphones[0].id][0] - 0.0238) < 1e-3);
        }

        [Test]
        public void Rays11_Test()
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

            rays.Sort(delegate (AcousticRay first, AcousticRay second)
            {
                return first.GetDistance().CompareTo(second.GetDistance());
            });

            var myRays = new Dictionary<int, List<AcousticRay>>();
            myRays.Add(microphone[0].id, rays);

            var intensityCalculator = new IntensityCalculator(myRays, microphone, 1);
            intensityCalculator.ComputePower();

            var distanceCalculator = new DistanceCalculator(myRays, microphone);
            distanceCalculator.ComputeDistances();

            double epsilon = 1e-4;
            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[microphone[0].id][0] - 0.0098243) < epsilon);
            Assert.IsTrue(Math.Abs(intensityCalculator.intensities[microphone[0].id][1] - 0.0049536) < epsilon);
        }

        [Test]
        public void Rays11Phase_Test()
        {
            List<MicrophoneSphere> microphone = new List<MicrophoneSphere>() {
                new MicrophoneSphere(new System.Numerics.Vector3(2, 1.6f, 1.7f), 0.1f) };
            RayGeometry rayGeometryGenerator = new RayGeometry(new Vector3(0, 0.5f, 0),
                microphone,
                1000,
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

            PhaseCalculator phaseCalculator = new PhaseCalculator(myRays, microphone, intensityCalculator.intensities);
            phaseCalculator.ComputePhase(1000);

            double epsilon = 1e-4;
            Assert.IsTrue(Math.Abs(phaseCalculator.echogramMagnitudePhase[microphone[0].id][0].Real + 0.002589105375) < epsilon);
        }
    }
}
