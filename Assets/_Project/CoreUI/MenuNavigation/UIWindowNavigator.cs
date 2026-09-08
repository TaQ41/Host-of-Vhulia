using System.Collections.Generic;
using UnityEngine;

namespace CoreUI.MenuNavigation
{
    /// <summary>
    /// Navigates through a collection of UIWindow nodes and calls the outermost open and close operations on the menu itself and the active UIWindow.
    /// When closing UIWindows from here, any and all subwindows of that UIWindow will subsequently be closed.
    /// </summary>
    public class UIWindowNavigator : MonoBehaviour
    {
        [SerializeField, Tooltip("Specify one UIWindow to have open for initial loading, or place the WindowNavigator on the same object with the root UIWindow.")]
        private List<UIWindow> m_windowDirectory;

        [SerializeField]
        private bool m_loadOnStart = true;

        /// <summary> (Testing method)
        /// Returns all UIWindows in the windowDirectory as the gameObjects they are attached to.
        /// The order is the same as in the windowDirectory (root to currently active.)
        /// </summary>
        /// <returns>An array of the UIWindows as GameObjects in the windowDirectory.</returns>
        [Sirenix.OdinInspector.Button]
        public IEnumerable<GameObject> WindowDirectoryAsGameObjects()
        {
            GameObject[] windowNames = new GameObject[m_windowDirectory.Count];
            for (int i = 0; i < windowNames.Length; i++)
                yield return windowNames[i] = m_windowDirectory[i].gameObject;
        }

        void Start()
        {
            if (m_loadOnStart)
                OpenMenu();
        }

        /// <summary>
        /// Opens the menu without resetting the windowDirectory. If the windowDirectory is empty and no UIWindow is located with the UIWindowNavigator,
        /// this will do nothing. An optional directory can be provided to be combined with the current, either way the "leftmost" window will be opened.
        /// </summary>
        /// <param name="initialDirectory">A directory that can be provided to be appended to the current directory.</param>
        public void OpenMenu(IEnumerable<UIWindow> initialDirectory = null)
        {
            if (m_windowDirectory == null || m_windowDirectory.Count == 0)
            {
                try
                {
                    m_windowDirectory = new() {gameObject.GetComponent<UIWindow>()};
                }
                catch
                {
                    Debug.LogError("No root UIWindow was specified or found in the WindowNavigator.");
                }

                return;
            }

            if (initialDirectory != null)
                m_windowDirectory.AddRange(initialDirectory);

            m_windowDirectory[^1].OpenWindow();
        }

        /// <summary>
        /// Closes the currently active UIWindow without resetting the current directory. Optionally don't force close and use the normal
        /// close method found on the active UIWindow.
        /// </summary>
        /// <param name="useCustom">Use the normal close method on the UIWindow that may use the provided customWindowOperations.</param>
        public void CloseMenu(bool useCustom = false)
        {
            if (m_windowDirectory == null || m_windowDirectory.Count == 0)
                return;
            
            if (useCustom)
            {
                m_windowDirectory[^1].CloseWindow();
                return;
            }

            m_windowDirectory[^1].ForceCloseWindow();
        }

        /// <summary>
        /// Resets the windowDirectory to contain only the root window.
        /// </summary>
        public void ClearDirectory()
        {
            m_windowDirectory = new() {m_windowDirectory[0]};
        }

        /// <summary>
        /// Opens a UIWIndow to be the next active in the directory, this will close the last window and add the current to the directory.
        /// </summary>
        /// <param name="window">The UIWindow to be opened.</param>
        public void OpenNextUIWindow(UIWindow window)
        {
            if (window == null)
                return;

            m_windowDirectory[^1].CloseWindow();
            m_windowDirectory.Add(window);
            window.OpenWindow();
        }

        /// <summary>
        /// Tries to go back a step and close the active UIWindow then open the previous UIWindow.
        /// Doesn't do anything on the windowDirectory containing only the root or less.
        /// </summary>
        public void OpenPreviousUIWindow()
        {
            if (m_windowDirectory.Count <= 1)
                return;

            m_windowDirectory[^1].CloseWindow();
            m_windowDirectory.RemoveAt(m_windowDirectory.Count - 1);
            
            m_windowDirectory[^1].OpenWindow();
        }

        /// <summary>
        /// Unlike the open and previous UIWindow methods, this activates any UIWindow that can be appended to the path
        /// and contain its own path that will be used on previous UIWindow calls.
        /// </summary>
        /// <param name="window">The window to be opened.</param>
        /// <param name="path">The path behind the window that will be used when previous calls are made.</param>
        /// <param name="appendPath">Should the path and the window be appended to the current path or be set as the path.</param>
        public void OpenTargetLink(UIWindow window, IEnumerable<UIWindow> path = null, bool appendPath = false)
        {
            if (window == null)
                return;

            if (path != null)
            {
                if (appendPath)
                {
                    m_windowDirectory.AddRange(path);
                }
                else
                {
                    m_windowDirectory = new() {m_windowDirectory[0]};
                    m_windowDirectory.AddRange(path);
                }

                OpenNextUIWindow(window);
                return;
            }

            if (appendPath)
                OpenNextUIWindow(window);
            else
            {
                m_windowDirectory[^1].CloseWindow();
                m_windowDirectory = new() {m_windowDirectory[0], window};
            }
        }
    }
}