using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

namespace ProjectFile
{

    /// <summary>
    /// The collection of methods that will allow for ProjectFile I/O operations. Contains all information neccessary to
    /// act on saved files and future save files. Use only for normal game file saves and retrieval.
    /// </summary>
    public class ProjectFileUtilities
    {
        private const string SavedFilesDir = @"Assets\_Project\ProjectFile\Saved Files";
        private const string QuickPullDataPath = SavedFilesDir + @"QuickPullData.json";

        #region QuickPull System

        /// <summary>
        /// A wrapper class that holds the List of QuickPullObjects. This is neccessary to allow I/O operations on the QuickPullData file.
        /// </summary>
        [Serializable]
        private class QuickPullWrapper
        {
            public List<QuickPullObject> Pool = new();
        }

        /// <summary>
        /// Attempts to find the QuickPullWrapper in the directory.
        /// </summary>
        /// <param name="quickPullWrapper"></param>
        /// <returns>A boolean that is false if the QuickPullDataPath couldn't pick anything up.</returns>
        private static bool TryCreateQuickPullWrapper(out QuickPullWrapper quickPullWrapper)
        {
            quickPullWrapper = null;

            try 
            {
                quickPullWrapper = JsonUtility.FromJson<QuickPullWrapper>(File.ReadAllText(QuickPullDataPath));
            }
            catch
            {
                Debug.LogError($"Unable to access the ${QuickPullDataPath} file! Ensure it exists at {SavedFilesDir + QuickPullDataPath}.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds a QuickPullObject and matches it as the index in the QuickPullWrapper pool.
        /// Defaults to the pool's count.
        /// </summary>
        /// <param name="quickPullWrapper">The pool that the obj may be in.</param>
        /// <param name="obj">The QuickPullObject being checked for.</param>
        /// <returns>The index in the QuickPullWrapper pool of the obj, returns the pool's count if nothing was matched.</returns>
        private static int GetQuickPullIndex(QuickPullWrapper quickPullWrapper, QuickPullObject obj)
        {
            for (int i = 0; i < quickPullWrapper.Pool.Count; i++)
            {
                if (quickPullWrapper.Pool[i].Guid.Equals(obj.Guid))
                    return i;
            }

            return quickPullWrapper.Pool.Count;
        }

        /// <summary>
        /// Attempts to modify the QuickPullData by updating the value of obj in the Pool or having the
        /// deletion mode toggled, this will delete the value in the Pool.
        /// </summary>
        /// <param name="obj">The object that will be used to find the current object and then update it.</param>
        /// <param name="isDeletion">A toggle for deleting the element, if false, this will try to add or change a current value.</param>
        private static void UpdateQuickPullData(QuickPullObject obj, bool isDeletion)
        {
            if (TryCreateQuickPullWrapper(out QuickPullWrapper quickPullWrapper) == false)
                return;

            int quickPullIndex = GetQuickPullIndex(quickPullWrapper, obj);

            if (isDeletion)
            {
                if (quickPullIndex == quickPullWrapper.Pool.Count)
                    return;

                quickPullWrapper.Pool.RemoveAt(quickPullIndex);
            }
            else
            {
                if (quickPullIndex == quickPullWrapper.Pool.Count)
                    quickPullWrapper.Pool.Add(obj);
                else
                    quickPullWrapper.Pool[quickPullIndex] = obj;
            }

            File.WriteAllText(QuickPullDataPath, JsonUtility.ToJson(quickPullWrapper));
        }

        /// <summary>
        /// Resets the QuickPullData with actual information that could be discovered under the Saved Files directory.
        /// This will recreate a wrapper class with QuickPullObjects created from each save file.
        /// </summary>
        public static void RegenerateQuickPullData()
        {
            QuickPullWrapper quickPullData = new();
            ProjectFileHeader projFile = ScriptableObject.CreateInstance<ProjectFileHeader>();
            foreach (string file in Directory.EnumerateFiles(SavedFilesDir, searchPattern: "*.json"))
            {
                if (file.Equals(QuickPullDataPath))
                    continue;

                try {
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(file), projFile);
                } catch {
                    continue;
                }

                quickPullData.Pool.Add(new QuickPullObject(projFile.IdentificationData));
            }

            string quickPullDataJson = JsonUtility.ToJson(quickPullData);
            File.WriteAllText(QuickPullDataPath, quickPullDataJson);
        }

        /// <summary>
        /// Get all the QuickPullObjects found in the wrapper in the QuickPullData at the time of calling.
        /// </summary>
        /// <returns>The Pool of the wrapper, but converted to an array.</returns>
        public static QuickPullObject[] QuickPullAll()
        {
            string quickPullDataJson = File.ReadAllText(QuickPullDataPath);
            QuickPullWrapper wrapper = JsonUtility.FromJson<QuickPullWrapper>(quickPullDataJson);
            return wrapper.Pool.ToArray();
        }

        #endregion

        /// <summary>
        /// Serialize a projectFile into a json file at its specified location (guid).
        /// Awaits the writing call. Affects the QuickPullData.
        /// </summary>
        /// <param name="projectFile">The projectFile to save.</param>
        /// <returns>A bool determining if this succeeded or not.</returns>
        public static async Task<bool> SerializeFile(ProjectFileHeader projectFile)
        {
            try
            {
                if (!Directory.Exists(SavedFilesDir))
                    Directory.CreateDirectory(SavedFilesDir);

                await File.WriteAllTextAsync(SavedFilesDir + projectFile.IdentificationData.Guid + ".json", JsonUtility.ToJson(projectFile));
            }
            catch
            {
                return false;
            }

            UpdateQuickPullData(new QuickPullObject(projectFile.IdentificationData), isDeletion: false);
            return true;
        }

        /// <summary>
        /// Deserialize a projectFile from its guid and sets it to the provided projectFileHeader instance.
        /// </summary>
        /// <param name="guid">The guid in the indentification section of a projectFile.</param>
        public static void DeserializeFile(string guid, ProjectFileHeader projFile)
        {
            string filePath = SavedFilesDir + guid + ".json";

            if (File.Exists(filePath))
                JsonUtility.FromJsonOverwrite(File.ReadAllText(filePath), projFile);
        }

        /// <summary>
        /// Delete a file given its guid. This will handle both the .json and .meta files.
        /// Affects the QuickPullData.
        /// </summary>
        /// <param name="guid"></param>
        public static void DeleteFile(string guid)
        {
            string expectedFilePath = SavedFilesDir + guid + ".json";
            File.Delete(expectedFilePath);
            File.Delete(expectedFilePath + ".meta");

            UpdateQuickPullData(new QuickPullObject() {Guid = guid}, isDeletion: true);
        }
    }
}