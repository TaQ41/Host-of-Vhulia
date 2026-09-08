namespace ProjectFile
{
    /// <summary>
    /// An object used for concise data collection of each project file.
    /// All fields will be used in the "Load Game" section when displaying all files.
    /// </summary>
    [System.Serializable]
    public struct QuickPullObject
    {
        public string Guid;
        public string ProjectName;

        public QuickPullObject(Identification data)
        {
            Guid = data.Guid;
            ProjectName = data.ProjectName;
        }
    }
}