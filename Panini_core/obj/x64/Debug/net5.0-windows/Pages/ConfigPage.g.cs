﻿#pragma checksum "..\..\..\..\..\Pages\ConfigPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "D8C7B03AE28F82E37C2B08835869914153FB064F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Panini.Pages;
using Panini.ViewModel;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Panini.Pages {
    
    
    /// <summary>
    /// ConfigPage
    /// </summary>
    public partial class ConfigPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 97 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ignoreStartWith;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ignoreEndWith;
        
        #line default
        #line hidden
        
        
        #line 139 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ignoreContains;
        
        #line default
        #line hidden
        
        
        #line 158 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox hyperlinkCheck;
        
        #line default
        #line hidden
        
        
        #line 170 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox inlineLinkCheck;
        
        #line default
        #line hidden
        
        
        #line 188 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox relatedLinkCheck;
        
        #line default
        #line hidden
        
        
        #line 235 "..\..\..\..\..\Pages\ConfigPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox getParentTagCheckbox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Panini;component/pages/configpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Pages\ConfigPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.ignoreStartWith = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.ignoreEndWith = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.ignoreContains = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.hyperlinkCheck = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 5:
            this.inlineLinkCheck = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.relatedLinkCheck = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 7:
            this.getParentTagCheckbox = ((System.Windows.Controls.CheckBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

