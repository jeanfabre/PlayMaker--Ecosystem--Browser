using System.IO;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [System.Serializable]
    public class ExcludeFile
    {
        [SerializeField]
        public string excludeFileString;

        public ExcludeFile(string folderToAdd)
        {
            excludeFileString = folderToAdd;
        }
    }
}