using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiParadigmGrapher.GraphFunctions;
using MultiParadigmGrapherTest.ContentHandling;

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
    }
}