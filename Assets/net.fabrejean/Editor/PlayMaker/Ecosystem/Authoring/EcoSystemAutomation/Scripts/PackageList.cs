using System.Collections.Generic;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [CreateAssetMenu(fileName = "EcoSytemBundle", menuName = "Automation", order = 1)]
    [System.Serializable]
    public class PackageList : ScriptableObject
    {
        [SerializeField]
        public List<IncludeFolder> IncludeFolders = new List<IncludeFolder>();
        [SerializeField]
        public List<ExcludeFolder> ExcludeFolders = new List<ExcludeFolder>();
        [SerializeField]
        public List<IncludeFile> IncludeFiles = new List<IncludeFile>();
        [SerializeField]
        public List<ExcludeFile> ExcludeFiles = new List<ExcludeFile>();
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
}