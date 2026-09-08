using UnityEngine;

namespace ProjectFile
{
    [CreateAssetMenu(fileName = "ProjectFileHeader", menuName = "Scriptable Objects/ProjectFileHeader")]
    public class ProjectFileHeader : ScriptableObject
    {
        public Identification IdentificationData;
    }

    [System.Serializable]
    public struct Identification
    {
        public string Guid;
        public string ProjectName;
    }
}