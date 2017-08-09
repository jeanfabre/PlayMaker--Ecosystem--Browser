using System.Collections.Generic;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [CreateAssetMenu(fileName = "EcoSytemBundle", menuName = "Automation", order = 1)]
    [System.Serializable]
    // Single Files List

    // Package List
    public class PackageList : ScriptableObject
    {
        [SerializeField]
        public List<IncludeFolder> includeFolders = new List<IncludeFolder>();
        [SerializeField]
        public List<ExcludeFolder> excludeFolders = new List<ExcludeFolder>();
        [SerializeField]
        public List<IncludeFile> includeFiles = new List<IncludeFile>();
        [SerializeField]
        public List<ExcludeFile> excludeFiles = new List<ExcludeFile>();
        [SerializeField]
        public List<YouTubeLink> youTubeLists = new List<YouTubeLink>();
        [SerializeField]
        public string type;
        [SerializeField]
        public string modules;
        [SerializeField]
        public string version;
        [SerializeField]
        public string uMinVersion;
        [SerializeField]
        public string pmMinVersion;
        [SerializeField]
        public string youTubeVidLink;
        [SerializeField]
        public string webLink;
        [SerializeField]
        public string keyWords;
        [SerializeField]
        public Object assetPath;
        [SerializeField]
        public string pingMenu;
        [SerializeField]
        public string packageName;
        [SerializeField]
        public string targetDirectory;
        [SerializeField]
        public string ecoFilter;
        [SerializeField]
        public string targetPackageTextFile;
    }
    public class IncludeFolder
    {
        [SerializeField]
        public string includeFolderString = "";

        public IncludeFolder(string folderToAdd)
        {
            includeFolderString = folderToAdd;
        }
    }
    // Exclude Folder
    public class ExcludeFolder
    {
        [SerializeField]
        public string excludeFolderString = "";

        public ExcludeFolder(string folderToAdd)
        {
            excludeFolderString = folderToAdd;
        }
    }

    // Include File

    public class IncludeFile
    {
        [SerializeField]
        public string includeFileString;

        public IncludeFile(string folderToAdd)
        {
            includeFileString = folderToAdd;
        }
    }
    // Exclude File
    public class ExcludeFile
    {
        [SerializeField]
        public string excludeFileString;

        public ExcludeFile(string folderToAdd)
        {
            excludeFileString = folderToAdd;
        }
    }

    public class YouTubeLink
    {
        [SerializeField]
        public string youTubeLinkString = string.Empty;
    }
}