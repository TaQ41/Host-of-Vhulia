using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using LocaleSetupProcesses;

namespace LocalesCore
{
    /// <summary>
    /// Attached to a GameObject in every locale scene that allows locales to be loaded to and from.
    /// </summary>
    public class LocaleLoader : MonoBehaviour
    {
        /// <summary>
        /// Each LocaleLoader has a tag to allow the newly loaded scene to connect with its LocaleLoader more efficiently.
        /// </summary>
        public const string TagInLocaleScene = "LocaleLoader";

        /// <summary>
        /// Will show every loaded process part and those that are not included are in the process of loading.
        /// </summary>
        private LocaleSetupProcessPart.LoadProcessParts m_loadedProcessParts;

        /// <summary>
        /// The list of GameObjects inheriting from ILocaleSetupProcess that load a fundamental part of the loading scene.
        /// </summary>
        [SerializeField] private List<GameObject> m_localeSetupGOParts = new();

        /// <summary>
        /// A final script that enables scene-specific objects in order to begin.
        /// </summary>
        [SerializeField] private LocaleSceneEnabler m_LocaleSceneEnabler;

        /// <summary>
        /// A flag given by the settings when loading a locale as to whether the SceneEnabler should be called immediately upon all process parts finishing.
        /// </summary>
        private bool m_enableLocaleFlag;

        /// <summary>
        /// When a new localeLoader is found, the loaded transition scene name will be sent to it so that it can call the unload call properly.
        /// </summary>
        private string m_transitionSceneName;

        /// <summary>
        /// Load to a locale given a certain set of settings.
        /// If the operation is successful, this will unload the current locale scene.
        /// </summary>
        /// <param name="settings">Settings that config properties on loading the new scene.</param>
        [Sirenix.OdinInspector.Button]
        public void LoadLocale(LocaleLoadingSettings settings)
        {
            if (settings.TransitionSceneName.Equals(string.Empty))
                settings.TransitionSceneName = LocaleTransitionManager.DefaultTransitionSceneName;

            try
            {
                AsyncOperation opLoadTransition = SceneManager.LoadSceneAsync(settings.TransitionSceneName, LoadSceneMode.Additive);
                StartCoroutine(
                    StartTransition(opLoadTransition, settings)
                );
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Wait for the transition scene to be loaded and then call the LoadTransition method on it.
        /// Although I don't really have to wait for the transition scene, it will load in less than a second.
        /// </summary>
        /// <param name="opLoadTransition">The async operation of the transition scene loading.</param>
        /// <param name="settings">The settings used to customize how a locale loads, passed to the LoadTransition call.</param>
        private IEnumerator StartTransition(AsyncOperation opLoadTransition, LocaleLoadingSettings settings)
        {
            while (!opLoadTransition.isDone)
                yield return null;

            _ = LocaleTransitionManager.LoadTransition(gameObject.scene, settings);
        }

        /// <summary>
        /// Used on the "from" part of the LocaleLoader, called on the newly loaded locale and each GameObject containing a load process
        /// will have its load called, the PartLoadedEvent method is the callback used.
        /// </summary>
        /// <param name="enableLocaleAfterLoad">Should the locale be enabled immediately after all processes have loaded?</param>
        internal void SetupLocale(bool enableLocaleAfterLoad, string transitionSceneName)
        {
            bool validPartFound = false;
            m_loadedProcessParts = LocaleSetupProcessPart.LoadProcessParts.All;
            m_enableLocaleFlag = enableLocaleAfterLoad;
            m_transitionSceneName = transitionSceneName;

            foreach (var part in m_localeSetupGOParts)
            {
                if (part == null)
                {
                    Debug.LogWarning("GameObject listed as a part of the locale setup process was null!");
                    continue;
                }
                
                if (!part.TryGetComponent(out LocaleSetupProcessPart process))
                {
                    Debug.LogWarning($"GameObject listed as a part of the locale setup process was missing a component inheriting from '{nameof(LocaleSetupProcessPart)}'!");
                    continue;
                }

                validPartFound = true;
                m_loadedProcessParts &= ~process.GetLoadProcessParts();
                _ = process.BeginSetup(() => PartLoadedEvent(process.GetLoadProcessParts()));
            }

            if (!validPartFound)
                FinalizeLoadingProcess();
        }

        /// <summary>
        /// A callback that each loading process calls when it has finished.
        /// If all processes are loaded, the scene is enabled depending on the flag set from the settings to load the current locale.
        /// </summary>
        /// <param name="parts">The processes that the LocaleSetupProcess covered in loading.</param>
        private void PartLoadedEvent(LocaleSetupProcessPart.LoadProcessParts parts)
        {
            m_loadedProcessParts |= parts;
            if (m_loadedProcessParts == LocaleSetupProcessPart.LoadProcessParts.All)
                FinalizeLoadingProcess();
        }

        /// <summary>
        /// Unloads the transition scene, begins cutscenes, and checks flag toggles.
        /// Created that in the case of a scene having no process parts, the new scene can still correctly load.
        /// </summary>
        internal void FinalizeLoadingProcess()
        {
            if (m_enableLocaleFlag)
            {
                // Load cutscenes
                LocaleTransitionManager.UnloadTransition(m_transitionSceneName);
                m_LocaleSceneEnabler.EnableScene();
            }
        }
    }
}