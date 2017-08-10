using System.Collections.Generic;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [CreateAssetMenu(fileName = "EcoAutoPacker", menuName = "NewEcoAutoPacker", order = 1)]
    [System.Serializable]
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
        public List<WebLink> webLinkList = new List<WebLink>();
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
        public string youTubeVidLink; //need to change to list
        [SerializeField]
        public string webLink; //need to change to list
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
}