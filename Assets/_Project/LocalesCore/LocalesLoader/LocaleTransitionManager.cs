using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LocalesCore
{
    /// <summary>
    /// Contains methods used for handling transitions between locale scenes.
    /// </summary>
    internal static class LocaleTransitionManager
    {
        internal static readonly string DefaultTransitionSceneName = "Basic Black Screen";

        /// <summary>
        /// Tries to load the target scene and unloads the calling scene on success. After loading the target scene, the LocaleSetup
        /// on the new localeLoader is called.
        /// </summary>
        /// <param name="callingScene"></param>
        /// <param name="settings"></param>
        internal static async Task LoadTransition(Scene callingScene, LocaleLoadingSettings settings)
        {
            int targetSceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(settings.TargetLocaleScenePath);
            if (targetSceneBuildIndex == -1)
            {
                Debug.LogError($"Target Scene could not be located! path: {settings.TargetLocaleScenePath}");
                UnloadTransition(settings.TransitionSceneName);
                return;
            }

            AsyncOperation opLoadTargetScene = SceneManager.LoadSceneAsync(targetSceneBuildIndex, LoadSceneMode.Additive);
            await SceneManager.UnloadSceneAsync(callingScene);
            await opLoadTargetScene;

            Thread.Sleep(2000); // Simulate an actual wait.
            CallLocaleSetup(settings.EnableLocaleAfterLoad, settings.TransitionSceneName);
        }

        /// <summary>
        /// Finds the new locale loader and if found, calls its SetupLocale method.
        /// </summary>
        /// <param name="enableLocaleAfterLoad">Used to pass the enableLocaleAfterLoad to the SetupLocale.</param>
        private static void CallLocaleSetup(bool enableLocaleAfterLoad, string transitionSceneName)
        {
            GameObject localeLoaderGO = GameObject.FindWithTag(LocaleLoader.TagInLocaleScene);
            if (localeLoaderGO != null)
                localeLoaderGO.GetComponent<LocaleLoader>().SetupLocale(enableLocaleAfterLoad, transitionSceneName);
        }        

        /// <summary>
        /// Unload the transition scene immediately.
        /// </summary>
        internal static void UnloadTransition(string transitionSceneName)
        {
            SceneManager.UnloadSceneAsync(transitionSceneName);
        }
    }
}
