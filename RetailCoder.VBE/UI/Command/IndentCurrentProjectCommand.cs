﻿using System.Runtime.InteropServices;
using NLog;
using Rubberduck.Parsing.VBA;
using Rubberduck.SmartIndenter;
using Rubberduck.VBEditor.SafeComWrappers.VB.Abstract;
using Rubberduck.VBEditor.SafeComWrappers.VB.Enums;

namespace Rubberduck.UI.Command
{
    [ComVisible(false)]
    public class IndentCurrentProjectCommand : CommandBase
    {
        private readonly IVBE _vbe;
        private readonly IIndenter _indenter;
        private readonly RubberduckParserState _state;

        public IndentCurrentProjectCommand(IVBE vbe, IIndenter indenter, RubberduckParserState state) : base(LogManager.GetCurrentClassLogger())
        {
            _vbe = vbe;
            _indenter = indenter;
            _state = state;
        }

        protected override bool EvaluateCanExecute(object parameter)
        {
            return !_vbe.ActiveVBProject.IsWrappingNullReference && _vbe.ActiveVBProject.Protection != ProjectProtection.Locked;
        }

        protected override void OnExecute(object parameter)
        {
            _indenter.IndentCurrentProject();
            if (_state.Status >= ParserState.Ready || _state.Status == ParserState.Pending)
            {
                _state.OnParseRequested(this);
            }
        }
    }
}
