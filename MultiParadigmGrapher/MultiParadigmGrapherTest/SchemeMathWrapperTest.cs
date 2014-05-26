using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiParadigmGrapher.GraphFunctions;
using MultiParadigmGrapherTest.ContentHandling;
using System.Collections.Generic;
using System.Linq;

namespace MultiParadigmGrapherTest
{
    [TestClass]
    public class SchemeMathWrapperTest
    {
        [TestInitialize]
        public void init()
        {
            SchemeMathWrapper.LoadSchemeFunctions(new ContentProvider());
        }

        [TestMethod]
        public void verifyIsProcedure_noexception()
        {            
            SchemeMathWrapper.VerifyIsProcedure("sin");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void verifyIsProcedure_argumentException()
        {
            SchemeMathWrapper.VerifyIsProcedure("(+ 2 2)");
        }


        #region 1-to-1 with scheme tests
        string f = "(lambda (x) (* 2 x))";

        //'((0 . 0) (1/2 . 1) (1 . 2) (3/2 . 3) (2 . 4)))
        List<Tuple<double, double>> expectedPlotData = new List<Tuple<double, double>> 
        {
            new Tuple<double,double>(0, 0),
            new Tuple<double,double>(0.5, 1),
            new Tuple<double,double>(1, 2),
            new Tuple<double,double>(1.5, 3),
            new Tuple<double,double>(2, 4)
        };

        //'((0 . 1/2) (1/2 . 3/2) (1 . 5/2) (3/2 . 7/2))
        List<Tuple<double, double>> expectedMidpointIntegralData = new List<Tuple<double, double>> 
        {
            new Tuple<double,double>(0, 0.5),
            new Tuple<double,double>(0.5, 1.5),
            new Tuple<double,double>(1, 2.5),
            new Tuple<double,double>(1.5, 3.5)
        };

        [TestMethod]
        public void stepToSamples()
        {
            Assert.AreEqual(21, SchemeMathWrapper.StepToSamples(0, 10, 0.5));
        }

        [TestMethod]
        public void samplesToStep()
        {
            Assert.AreEqual(0.5, SchemeMathWrapper.SamplesToStep(0, 10, 21));
        }

        [TestMethod]
        public void calcPlotData()
        {
            var actual = SchemeMathWrapper.CalcPlotData(f, 0, 2, 0.5).ToList();

            CollectionAssert.AreEqual(expectedPlotData, actual);
        }

        [TestMethod]
        public void calcMidpointIntegralCoords()
        {
            var actual = SchemeMathWrapper.CalcMidpointIntegralCoords(f, 0, 2, 4).ToList();

            CollectionAssert.AreEqual(expectedMidpointIntegralData, actual);
        }

        [TestMethod]
        public void calcDefiniteIntegral()
        {
            var actual = SchemeMathWrapper.CalcDefiniteIntegral(0, 2, 4, expectedMidpointIntegralData);

            Assert.AreEqual(4, actual);
        }
        #endregion


    }
}