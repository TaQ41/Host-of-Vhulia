using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CoreUI
{
    /// <summary>
    /// An instance that holds the address used to obtain a GameObject that is also stored in this instance.
    /// The header GameObject (LoadedUIWidget) should contain the interaction functions and the canvas.
    /// In most cases, let this class release the asset when it is destroyed, only when you want to retain this UIWidget,
    /// but unload the GameObject should the Release call be manual.
    /// </summary>
    public class UIWidget
    {
        /// <summary>
        /// The header GameObject of the loaded UIWidget.
        /// </summary>
        public GameObject LoadedUIWidget { get; private set; }

        /// <summary>
        /// The address used to obtain the header GameObject.
        /// </summary>
        public string Address { get; private set; }

        public UIWidget(string address = "", GameObject parent = null, System.Action<GameObject> callBack = null)
        {
            if (address != string.Empty)
                _ = LoadUIWidget(address, parent, callBack);
        }

        ~UIWidget()
        {
            ReleaseAddressableInstance();
        }

        /// <summary>
        /// Manual call of the releaseInstance; don't use in any case where the UIWidget will be deconstructed / deleted.
        /// </summary>
        public void ReleaseAddressableInstance()
        {
            if (LoadedUIWidget != null)
                Addressables.ReleaseInstance(LoadedUIWidget);
        }

        /// <summary>
        /// Loads the UIWidget based on the provided address and stores the result in the containing UIWidget class.
        /// Optionally provide a callback that is called on the operation being done (no matter its status.)
        /// </summary>
        /// <param name="address">The address used to load the instance.</param>
        /// <param name="parent">The GameObject this instance will be placed under.</param>
        /// <param name="callBack">Send a callback on the process failing or succeeding.</param>
        /// <returns></returns>
        public async Task LoadUIWidget(string address, GameObject parent, System.Action<GameObject> callBack = null)
        {
            Address = address;
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(address, parent: parent.transform);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"Failed to load the UIWidget Asset at address: [{address}]!");
                callBack?.Invoke(default);
                return;
            }

            LoadedUIWidget = handle.Result;
            callBack?.Invoke(LoadedUIWidget);
        }
    }
}
