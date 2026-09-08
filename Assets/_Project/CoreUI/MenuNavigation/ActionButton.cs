using UnityEngine;
using UnityEngine.EventSystems;

namespace CoreUI.MenuNavigation.Buttons
{
    public class ActionButton : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            // Click no run?
        }
        
        public virtual void Run(params object[] runInfo)
        {
            
        }
    }
}