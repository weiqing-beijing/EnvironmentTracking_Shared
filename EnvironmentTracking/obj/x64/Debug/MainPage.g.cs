﻿#pragma checksum "C:\Source\Workspaces\Wei Qing TFS\EnvironmentTracking\EnvironmentTracking\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0907F161E7657BD3998B643BA20F6222"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnvironmentTracking
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.SensorID = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 2:
                {
                    this.textBlock1 = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3:
                {
                    this.Hum = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 4:
                {
                    this.Temp = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 5:
                {
                    this.Time = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

