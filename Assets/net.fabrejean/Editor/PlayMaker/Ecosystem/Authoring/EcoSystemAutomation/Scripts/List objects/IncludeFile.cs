
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class IncludeFile
    {

        public string includeFileString;

        public IncludeFile(string folderToAdd)
        {
            includeFileString = folderToAdd;
        }
    }
}
