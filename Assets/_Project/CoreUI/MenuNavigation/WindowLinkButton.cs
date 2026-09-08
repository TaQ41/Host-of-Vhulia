using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreUI.MenuNavigation.Buttons
{
    /// <summary>
    /// A button to change the current UIWindow. Unlike subwindows, only one UIWindow may be open at a time.
    /// Supports sequential navigation between windows and linking windows/paths.
    /// </summary>
    public class WindowLinkButton : MonoBehaviour, IPointerClickHandler
    {
        private enum LinkType
        {
                Next = 0,
            Previous = 1,
                Link = 2,
        };

        [SerializeField] private UIWindowNavigator m_windowNavigator;
        [SerializeField] private LinkType m_linkType;

        [BoxGroup("LinkSettings"), HideIf("m_linkType", Value = LinkType.Previous)] public UIWindow Target;

        [Tooltip("When specifying a path, do not include the root, the linking implementation automatically uses the root as the first UIWindow.")]
        [BoxGroup("LinkSettings"), ShowIf("m_linkType", Value = LinkType.Link)] public UIWindow[] WindowDirectory;
        [BoxGroup("LinkSettings"), ShowIf("m_linkType", Value = LinkType.Link)] public bool AppendDirectoryToCurrent;

        /// <summary>
        /// Open the next UIWindow set as the target in the inspector or navigate to the last opened window if applicable.
        /// </summary>
        /// <param name="isPrevious">Should this navigate to the last opened window?</param>
        public void SequenceToWindow(bool isPrevious)
        {
            if (isPrevious)
            {
                m_windowNavigator.OpenPreviousUIWindow();
                return;
            }

            m_windowNavigator.OpenNextUIWindow(Target);
        }

        /// <summary>
        /// Link to a new window with the directory properties specified in the inspector.
        /// </summary>
        public void LinkToWindow()
        {
            m_windowNavigator.OpenTargetLink(Target, WindowDirectory, AppendDirectoryToCurrent);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_linkType == LinkType.Link)
            {
                LinkToWindow();
                return;
            }

            SequenceToWindow(isPrevious: m_linkType == LinkType.Previous);
        }
    }
}