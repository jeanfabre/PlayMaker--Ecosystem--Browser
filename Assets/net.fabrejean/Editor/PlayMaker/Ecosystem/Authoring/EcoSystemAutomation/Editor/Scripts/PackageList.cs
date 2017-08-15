using System.Collections.Generic;
using UnityEngine;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [CreateAssetMenu(fileName = "EcoAutoPacker", menuName = "NewEcoAutoPacker", order = 1)]
    [System.Serializable]
    // Package List
    public class PackageList : ScriptableObject
    {
        #region Lists
        [SerializeField]
        public List<string> includeFolders = new List<string>();
        [SerializeField]
        public List<string> excludeFolders = new List<string>();
        [SerializeField]
        public List<string> includeFiles = new List<string>();
        [SerializeField]
        public List<string> excludeFiles = new List<string>();
        [SerializeField]
        public List<string> youTubeLists = new List<string>();
        [SerializeField]
        public List<string> webLinkList = new List<string>();
        [SerializeField]
        public List<string> modulesList = new List<string>();
        [SerializeField]
        public List<string> ecoFilterList = new List<string>();
        #endregion

        #region Strings
        [SerializeField]
        public string type;
        [SerializeField]
        public string version;
        [SerializeField]
        public string keyWords;
        [SerializeField]
        public string packageName;
        [SerializeField]
        public string targetDirectory;

        [SerializeField]
        public string targetPackageTextFile;
        #endregion

        #region Selection lists

        [SerializeField]
        public string[] packagetypeselection = new[] { "ActionPackage", "SamplePackage", "TemplatePackage" };
        [SerializeField]
        public int packagetypeselected = 0;
        [SerializeField]
        public string[] UminVselection = new[] { "ActionPackage", "SamplePackage", "TemplatePackage" };
        [SerializeField]
        public int UminVselected = 0;
        [SerializeField]
        public string[] PMminVselection = new[] { "ActionPackage", "SamplePackage", "TemplatePackage" };
        [SerializeField]
        public int PMminVselected = 0;
        [SerializeField]
        public string[] Pingtype = new[] { "None", "Asset Path", "Menu" };
        [SerializeField]
        public int Pingtypeselected = 0;
        [SerializeField]
        public string[] uMinVersion = new[] { "4.7", "5.0", "5.1", "5.2", "5.3", "5.4", "5.5", "5.6", "2017.1" };
        [SerializeField]
        public int uMinVersionSelected = 0;
        public string[] pmMinVersion = new[] { "1.8.0", "1.8.1", "1.8.2", "1.8.3", "1.8.4", "1.8.5"};
        [SerializeField]
        public int pmMinVersionSelected = 0;
        #endregion

        #region Others
        [SerializeField]
        public Object assetPath;
        [SerializeField]
        public Object pingMenu;
        [SerializeField]
        public bool fileExistsCheck = true;
        #endregion
    }
}