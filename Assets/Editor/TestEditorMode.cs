using System;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TestEditorMode
    {
        [Test]
        public void GetPolarCoordinates_Test()
        {
            Vector3 point = new Vector3(3, 4, 5);

            GameObject gameObject = new GameObject("FirstRay");
            FirstRay firstRay = gameObject.AddComponent<FirstRay>();

            Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);

            double[] expectedValues = { 7.0710678118655, 0.92729521800161, 0.78539816339745 };
            double epsilon = 1e-8;
            Assert.IsTrue(Math.Abs(expectedValues[0] - polarCoordinates.Item1) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[1] - polarCoordinates.Item2) < epsilon);
            Assert.IsTrue(Math.Abs(expectedValues[2] - polarCoordinates.Item3) < epsilon);
        }

        [Test]
        public void GetCartesianCoordinates_Test()
        {
            Vector3 point = new Vector3(3, 4, 5);

            GameObject gameObject = new GameObject("FirstRay");
            FirstRay firstRay = gameObject.AddComponent<FirstRay>();

            Tuple<double, double, double> polarCoordinates = firstRay.GetPolarCoordinates(point);
            Vector3 recalculatedPoint = firstRay.GetCartesianCoordinates(polarCoordinates.Item1,
                polarCoordinates.Item2,
                polarCoordinates.Item3);

            double epsilon = 1e-8;
            Assert.IsTrue(Math.Abs(recalculatedPoint.x - point.x) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.y - point.y) < epsilon);
            Assert.IsTrue(Math.Abs(recalculatedPoint.z - point.z) < epsilon);
        }
    }
}
