
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class ExcludeFile
    {
        public string excludeFileString;

        public ExcludeFile(string folderToAdd)
        {
            excludeFileString = folderToAdd;
        }
    }
}
