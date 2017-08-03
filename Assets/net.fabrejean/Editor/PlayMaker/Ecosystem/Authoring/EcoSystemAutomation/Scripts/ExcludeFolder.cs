using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class ExcludeFolder
    {
        [SerializeField]
        public string excludeFolderString = "";

        public ExcludeFolder(string folderToAdd)
        {
            excludeFolderString = folderToAdd;
        }
    }
}