﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rubberduck.UI.ReferenceBrowser
{
    /// <summary>
    /// Interaction logic for ReferenceListControl.xaml
    /// </summary>
    public partial class ReferenceListControl : UserControl
    {
        public ReferenceListControl()
        {
            InitializeComponent();
        }

        public RegisteredLibraryViewModel SelectedLibrary
        {
            get
            {
                return (RegisteredLibraryViewModel)GetValue(SelectedLibraryProperty);
            }
            set
            {
                SetValue(SelectedLibraryProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedLibraryProperty = DependencyProperty.Register(
            "SelectedLibrary",
            typeof(RegisteredLibraryViewModel),
            typeof(ReferenceListControl));
    }
}