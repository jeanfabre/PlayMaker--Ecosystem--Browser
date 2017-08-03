using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class IncludeFolder
    {
        [SerializeField]
        public string includeFolderString = "";

        public IncludeFolder(string folderToAdd)
        {
            includeFolderString = folderToAdd;
        }
    }
}
