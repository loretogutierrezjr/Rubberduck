using System;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections.QuickFixes
{
    public sealed class RemoveRedundantOptionQuickFix : IQuickFix
    {
        private readonly RubberduckParserState _state;
        private static readonly HashSet<Type> _supportedInspections = new HashSet<Type>
        {
            typeof(RedundantOptionInspection)
        };

        public RemoveRedundantOptionQuickFix(RubberduckParserState state)
        {
            _state = state;
        }

        public IReadOnlyCollection<Type> SupportedInspections => _supportedInspections.ToList();

        public void Fix(IInspectionResult result)
        {
            var rewriter = _state.GetRewriter(result.QualifiedSelection.QualifiedName);
            rewriter.Remove(result.Context);
        }

        public string Description(IInspectionResult result)
        {
            return string.Format(InspectionsUI.RedundantOptionQuickFix, result.Context.Start.Text + " " + result.Context.Stop.Text);
        }

        public bool CanFixInProcedure => false;
        public bool CanFixInModule => true;
        public bool CanFixInProject => true;
    }
}