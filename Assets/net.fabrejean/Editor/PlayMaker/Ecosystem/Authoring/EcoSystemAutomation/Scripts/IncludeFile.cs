using System.IO;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class IncludeFile
    {
        [SerializeField]
        public string includeFileString;

        public IncludeFile(string folderToAdd)
        {
            includeFileString = folderToAdd;
        }
    }
}