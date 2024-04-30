using System.Collections;
using System.Collections.Generic;
using System.IO;
using QFramework;
using Schwarzer.Windows;
using Larvend.PlotEditor.Serialization;
using UnityEngine;

namespace Larvend.PlotEditor.UI
{
    public struct PlotEditorUIRefreshEvent {}
    public struct OnCommandChangedEvent {}
    public struct OnCommandRefreshEvent {}
    public struct OnResourcesChangedEvent {}
    public struct OnCurrentImageResourceChangedEvent {}
    public struct OnResourceRefreshEvent {}

    public struct OnImageResourceSelectedEvent {
        public string Guid;
    }
}
