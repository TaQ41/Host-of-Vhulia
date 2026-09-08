using System.Collections.Generic;
using UnityEngine;

namespace CoreUI.MenuNavigation
{
    /// <summary>
    /// Essentially a node used across a navigation system where each node has its own open and close operations and can contain child subwindows.
    /// </summary>
    public class UIWindow : MonoBehaviour
    {
        [SerializeField] private Canvas m_windowCanvas;
        [SerializeField] private List<UIWindow> m_subWindows;

        [SerializeField]
        private ICustomWindowOperations m_customWindowOperations;

        public bool IsCanvasEnabled { get { return m_windowCanvas.enabled;} }

        /// <summary>
        /// Disables this UIWindow and hides it, if a customWindowOperations is provided, then that implementation will be used,
        /// otherwise this force closes the window.
        /// </summary>
        public void CloseWindow()
        {
            if (m_customWindowOperations == null)
            {
                ForceCloseWindow();
                return;
            }

            m_customWindowOperations.Close(m_windowCanvas, m_subWindows);
        }

        /// <summary>
        /// A force close ignores the provided customWindowOperations and disables the canvas set in the UIWindow and then
        /// calls the same force close on all subwindows.
        /// </summary>
        public void ForceCloseWindow()
        {
            m_windowCanvas.enabled = false;
            foreach (var subWindow in m_subWindows)
                subWindow.ForceCloseWindow();

            m_subWindows.Clear();
        }

        /// <summary>
        /// Opens the window with the provided customWindowOperations implementation, otherwise, this only enables the canvas.
        /// </summary>
        public void OpenWindow()
        {
            if (m_customWindowOperations != null)
            {
                m_customWindowOperations.Open(m_windowCanvas);
                return;
            }

            m_windowCanvas.enabled = true;
        }

        /// <summary>
        /// Opens a unique subwindow on this UIWindow. The subwindow will be added to a list on this UIWindow.
        /// A subwindow cannot be opened more than once, however, if a functionality requires this, it should
        /// instead use a different method.
        /// </summary>
        /// <param name="subWindow">The subwindow to open as a child of the current UIWindow.</param>
        public void OpenSubWindow(UIWindow subWindow)
        {
            // Prevent the same subWindow from opening.
            foreach (UIWindow currSubWindow in m_subWindows)
                if (currSubWindow == subWindow)
                    return;

            subWindow.OpenWindow();
            if (m_subWindows == null || m_subWindows.Count == 0)
            {
                m_subWindows = new() {subWindow};
                return;
            }

            m_subWindows.Add(subWindow);
        }

        /// <summary>
        /// Closes a subwindow on this UIWindow. Doesn't throw on the subwindow not being found.
        /// </summary>
        /// <param name="subWindow">The subwindow to close as a child of the current UIWindow.</param>
        public void CloseSubWindow(UIWindow subWindow)
        {
            foreach (UIWindow currSubWindow in m_subWindows)
                if (currSubWindow == subWindow)
                {
                    subWindow.CloseWindow();
                    m_subWindows.Remove(currSubWindow);
                    return;
                }
        }
    }
}