﻿using System;
using System.Drawing;
using System.Globalization;
using Rubberduck.Parsing.VBA;
using Rubberduck.VBEditor.SafeComWrappers.VB.Enums;

namespace Rubberduck.UI.Command.MenuItems
{
    public abstract class CommandMenuItemBase : ICommandMenuItem
    {
        protected CommandMenuItemBase(CommandBase command)
        {
            _command = command;
        }

        private readonly CommandBase _command;
        public CommandBase Command => _command;

        public abstract string Key { get; }

        public virtual Func<string> Caption
        {
            get
            {
                return () => string.IsNullOrEmpty(Key)
                    ? string.Empty
                    : RubberduckUI.ResourceManager.GetString(Key, CultureInfo.CurrentUICulture);
            }
        }

        public virtual string ToolTipKey { get; set; }
        public virtual Func<string> ToolTipText
        {
            get
            {
                return () => string.IsNullOrEmpty(ToolTipKey)
                    ? string.Empty
                    : RubberduckUI.ResourceManager.GetString(ToolTipKey, CultureInfo.CurrentUICulture);
            }
        }

        /// <summary>
        /// Evaluates current parser state to determine whether command can be executed.
        /// </summary>
        /// <param name="state">The current parser state.</param>
        /// <returns>Returns <c>true</c> if command can be executed.</returns>
        /// <remarks>Returns <c>true</c> if not overridden.</remarks>
        public virtual bool EvaluateCanExecute(RubberduckParserState state)
        {
            return state != null && _command.CanExecute(state);
        }

        public virtual ButtonStyle ButtonStyle => ButtonStyle.IconAndCaption;
        public virtual bool HiddenWhenDisabled => false;
        public virtual bool IsVisible => true;
        public virtual bool BeginGroup => false;
        public virtual int DisplayOrder => default(int);
        public virtual Image Image => null;
        public virtual Image Mask => null;
    }
}
