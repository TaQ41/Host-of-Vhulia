using System;

namespace LocalesCore
{
    /// <summary>
    /// A collection of settings used to specify how to load a locale.
    /// </summary>
    [Serializable]
    public struct LocaleLoadingSettings
    {
        public string TargetLocaleScenePath;
        public string TransitionSceneName;
        public bool IsMission;
        public bool EnableLocaleAfterLoad;
        
        #pragma warning disable UAC1001
        public object[] CustomSceneSetupParameters;
    }
}
