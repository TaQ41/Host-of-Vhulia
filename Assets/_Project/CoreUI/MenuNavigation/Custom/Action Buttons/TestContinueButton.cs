using UnityEngine;
using LocalesCore;

namespace CoreUI.MenuNavigation.Buttons.Custom
{
    /// <summary>
    /// 
    /// </summary>
    public class TestContinueButton : ActionButton
    {
        [SerializeField]
        private LocaleLoader m_localeLoader;

        public LocaleLoadingSettings settings;
        
        [Sirenix.OdinInspector.Button]
        public override void Run(params object[] runInfo)
        {
            m_localeLoader.LoadLocale(settings);
        }
    }
}