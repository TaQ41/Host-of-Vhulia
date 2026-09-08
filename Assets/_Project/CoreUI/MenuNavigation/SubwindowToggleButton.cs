using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreUI.MenuNavigation.Buttons
{
    /// <summary>
    /// A toggle button to open/close subwindows on a UIWindow.
    /// </summary>
    public class SubwindowToggleButton : MonoBehaviour, IPointerClickHandler
    {
        private enum ToggleMode
        {
            None = 0,

            /// <summary> Only allow opening the subWindow from this button, usually paired with a button on the subwindow to close it. </summary>
            Open = 1,

            /// <summary> Only allow closing the subWindow from this button, usually paired with a button on the parent window to open it. </summary>
            Close = 2,

            /// <summary> Allow both opening and closing of a subwindow from a parent window. </summary>
            Toggle = 3
        }

        [SerializeField] private UIWindow m_parentWindow;
        [SerializeField] private UIWindow m_targetWindow;
        [SerializeField] private ToggleMode m_toggleMode;

        /// <summary>
        /// Open or Close the subwindow based on the toggleMode set in the inspector.
        /// </summary>
        public void Toggle()
        {
            switch (m_toggleMode)
            {
                case ToggleMode.Open:
                    m_parentWindow.OpenSubWindow(m_targetWindow);
                    break;

                case ToggleMode.Close:
                    m_parentWindow.CloseSubWindow(m_targetWindow);
                    break;

                case ToggleMode.Toggle:
                    if (m_targetWindow.IsCanvasEnabled)
                        m_parentWindow.CloseSubWindow(m_targetWindow);
                    else
                        m_parentWindow.OpenSubWindow(m_targetWindow);
                    break;

                default:
                    Debug.Log("Using a toggle button that hasn't been given a specified toggle mode.");
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Toggle();
        }
    }
}