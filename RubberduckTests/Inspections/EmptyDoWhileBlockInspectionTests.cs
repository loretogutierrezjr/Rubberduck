﻿using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using RubberduckTests.Mocks;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Inspections.Resources;

namespace RubberduckTests.Inspections
{
    [TestClass]
    public class EmptyDoWhileBlockInspectionTests
    {
        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyDoWhileBlock_InspectionType()
        {
            var inspection = new EmptyDoWhileBlockInspection(null);
            var expectedInspection = CodeInspectionType.CodeQualityIssues;

            Assert.AreEqual(expectedInspection, inspection.InspectionType);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyDoWhileBlock_InspectionName()
        {
            const string expectedName = nameof(EmptyDoWhileBlockInspection);
            var inspection = new EmptyDoWhileBlockInspection(null);

            Assert.AreEqual(expectedName, inspection.Name);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyDoWhileBlock_DoesNotFiresOnImplementedLoopBlocks()
        {
            const string inputCode =
@"Sub Foo(results As Collection)
    Dim i As Integer
    i = 1

    Do While i < 6
        Cells( i, 1 ).Value = 20
        i = i + 1
    Loop
End Sub";
            CheckActualEmptyBlockCountEqualsExpected(inputCode, 0);
        }

        [TestMethod]
        [TestCategory("Inspections")]
        public void EmptyDoWhileBlock_FiresOnEmptyLoopBlocks()
        {
            const string inputCode =
@"Sub Foo(results As Collection)
    Dim i As Integer
    i = 1

    Do While i < 6
        'Cells( i, 1 ).Value = 20
        'i = i + 1
    Loop
End Sub";
            CheckActualEmptyBlockCountEqualsExpected(inputCode, 1);
        }

        private void CheckActualEmptyBlockCountEqualsExpected(string inputCode, int expectedCount)
        {
            IVBComponent component;
            var vbe = MockVbeBuilder.BuildFromSingleStandardModule(inputCode, out component);
            var state = MockParser.CreateAndParse(vbe.Object);

            var inspection = new EmptyDoWhileBlockInspection(state);
            var inspector = InspectionsHelper.GetInspector(inspection);
            var actualResults = inspector.FindIssuesAsync(state, CancellationToken.None).Result;

            Assert.AreEqual(expectedCount, actualResults.Count());
        }
    }
}
