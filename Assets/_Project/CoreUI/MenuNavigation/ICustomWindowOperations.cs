using System.Collections.Generic;
using UnityEngine;

namespace CoreUI.MenuNavigation
{
    /// <summary>
    /// Interface used in classes to allow UIWindows to use a custom script's implementation for opening and closing windows.
    /// Classes with this should default to a force close/disabling the canvas.
    /// </summary>
    public interface ICustomWindowOperations
    {
        abstract void Close(Canvas canvas, IEnumerable<UIWindow> subWindows);
        abstract void Open(Canvas canvas);
    }
}