using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Net.FabreJean.PlayMaker.Ecosystem
{
    [CustomEditor(typeof(PackageList))]
    public class EcosystemPackager : Editor
    {
        PackageList pl;
        private string folderToAdd;
        private string fileToAdd;
        public OPTIONS op;
        private List<string> includeFileList = new List<string>();
        List<string> PackageTextArray = new List<string>();
        private string packageType;
        private string get_targetDirectory;
        private string unityPackage;



        public enum OPTIONS
        {
            New_Action_Package = 0,
            New_Sample_Package = 1,
            New_Template_Package = 2
        }

        private void OnEnable()
        {
            pl = (PackageList)target;
        }
        // Inspector
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Package"))
            {
                CreatePackage();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            pl.packageName = (string)EditorGUILayout.TextField("Set Package Name", pl.packageName);
            if (EditorGUI.EndChangeCheck())
            {
                pl.targetDirectory = get_targetDirectory + "/" + pl.packageName + ".unitypackage";
                pl.targetPackageTextFile = get_targetDirectory + "/" + pl.packageName + ".package.txt";
                int index = pl.targetDirectory.IndexOf("PlayMaker/Ecosystem");
                if (index != -1) unityPackage = pl.targetDirectory.Substring(index);
            }
            GUILayout.EndVertical();

            // AssetPath Button
            GUILayout.BeginHorizontal("Button");
            if (GUILayout.Button("Target Directory"))
                OnSetTargetDirectory();
            pl.targetDirectory = (string)EditorGUILayout.TextField(pl.targetDirectory);
            GUILayout.EndHorizontal();



            op = (OPTIONS)EditorGUILayout.EnumPopup("Set Package Type", op);
            switch (op)
            {
                case OPTIONS.New_Action_Package:
                    EditorGUILayout.Space();
                    packageType = ("__PACKAGE__");

                    pl.ecoFilter = (string)EditorGUILayout.TextField(new GUIContent("EcoFilter", "TOOLTIP TEST"), pl.ecoFilter);
                    pl.type = (string)EditorGUILayout.TextField("Type", pl.type);
                    pl.modules = (string)EditorGUILayout.TextField("Modules", pl.modules);
                    pl.version = (string)EditorGUILayout.TextField("Version", pl.version);
                    pl.uMinVersion = (string)EditorGUILayout.TextField("Unity Minimum Version", pl.uMinVersion);
                    pl.pmMinVersion = (string)EditorGUILayout.TextField("PlayMaker Minimum Version", pl.pmMinVersion);
                    EditorGUILayout.BeginHorizontal();

                    pl.assetPath = (Object)EditorGUILayout.ObjectField("Ping Asset Path", pl.assetPath, typeof (Object), true) ;
                    EditorGUILayout.EndHorizontal();
                    
                    pl.youTubeVidLink = (string)EditorGUILayout.TextField("YouTube Video Link", pl.youTubeVidLink);
                    pl.webLink = (string)EditorGUILayout.TextField("Web Link", pl.webLink);
                    pl.keyWords = (string)EditorGUILayout.TextField("Keywords", pl.keyWords);

                    

                    break;

                case OPTIONS.New_Sample_Package:
                    EditorGUILayout.Space();
                    packageType = ("__SAMPLE__");
                    pl.ecoFilter = (string)EditorGUILayout.TextField("EcoFilter", pl.ecoFilter);
                    pl.modules = (string)EditorGUILayout.TextField("Modules", pl.modules);
                    pl.version = (string)EditorGUILayout.TextField("Version", pl.version);
                    pl.uMinVersion = (string)EditorGUILayout.TextField("Unity Minimum Version", pl.uMinVersion);
                    pl.pmMinVersion = (string)EditorGUILayout.TextField("PlayMaker Minimum Version", pl.pmMinVersion);
                    pl.youTubeVidLink = (string)EditorGUILayout.TextField("YouTube Video Link", pl.youTubeVidLink);
                    pl.webLink = (string)EditorGUILayout.TextField("Web Link", pl.webLink);
                    pl.keyWords = (string)EditorGUILayout.TextField("Keywords", pl.keyWords);

                    break;

                case OPTIONS.New_Template_Package:
                    EditorGUILayout.Space();
                    packageType = ("__TEMPLATE__");
                    pl.ecoFilter = (string)EditorGUILayout.TextField("EcoFilter", pl.ecoFilter);
                    pl.modules = (string)EditorGUILayout.TextField("Modules", pl.modules);
                    pl.uMinVersion = (string)EditorGUILayout.TextField("Unity Minimum Version", pl.uMinVersion);
                    pl.pmMinVersion = (string)EditorGUILayout.TextField("PlayMaker Minimum Version", pl.pmMinVersion);
                    pl.youTubeVidLink = (string)EditorGUILayout.TextField("YouTube Video Link", pl.youTubeVidLink);
                    pl.webLink = (string)EditorGUILayout.TextField("Web Link", pl.webLink);
                    pl.keyWords = (string)EditorGUILayout.TextField("Keywords", pl.keyWords);
                    pl.pingMenu = (string)EditorGUILayout.TextField("Menu Target", pl.pingMenu);

                    break;

            } //Switch end
            EditorGUILayout.Space();
            // Add Buttons
            GUILayout.Label("Set Folders And Files");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Include Folder", GUILayout.Width(150)))
            {
                IncludeFolder();
            }
            if (GUILayout.Button("Exclude Folder", GUILayout.Width(150)))
            {
                ExcludeFolder();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Include File", GUILayout.Width(150)))
            {
                IncludeFile();
            }
            if (GUILayout.Button("Exclude File", GUILayout.Width(150)))
            {
                ExcludeFile();
            }
            GUILayout.EndHorizontal();
            // End Add Buttons

            // Include folder List
            GUILayout.BeginVertical();
            GUILayout.Label("Included Folder List");
            GUILayout.EndVertical();

            for (int CountA = 0; CountA < pl.IncludeFolders.Count; CountA++)
            {
                GUILayout.BeginHorizontal();
                pl.IncludeFolders[CountA].includeFolderString = GUILayout.TextField(pl.IncludeFolders[CountA].includeFolderString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveIncludeFolder(CountA);
                    return;
                }
                GUILayout.EndHorizontal();
            }

            // Exclude folder List
            GUILayout.BeginVertical();
            GUILayout.Label("Excluded Folder List");
            GUILayout.EndVertical();

            for (int CountB = 0; CountB < pl.ExcludeFolders.Count; CountB++)
            {
                GUILayout.BeginHorizontal();
                pl.ExcludeFolders[CountB].excludeFolderString = GUILayout.TextField(pl.ExcludeFolders[CountB].excludeFolderString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveExcludeFolder(CountB);
                    return;
                }
                GUILayout.EndHorizontal();
            }

            // Include File List
            GUILayout.BeginVertical();
            GUILayout.Label("Included File List");
            GUILayout.EndVertical();

            for (int CountC = 0; CountC < pl.IncludeFiles.Count; CountC++)
            {
                GUILayout.BeginHorizontal();
                pl.IncludeFiles[CountC].includeFileString = GUILayout.TextField(pl.IncludeFiles[CountC].includeFileString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveIncludeFile(CountC);
                    return;
                }
                GUILayout.EndHorizontal();
            }

            // Exclude File List
            GUILayout.BeginVertical();
            GUILayout.Label("Excluded File List");
            GUILayout.EndVertical();

            for (int CountD = 0; CountD < pl.ExcludeFiles.Count; CountD++)
            {
                GUILayout.BeginHorizontal();
                pl.ExcludeFiles[CountD].excludeFileString = GUILayout.TextField(pl.ExcludeFiles[CountD].excludeFileString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveExcludeFile(CountD);
                    return;
                }
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                // save on change but HOW????
                // Tried repaint
                // setdirty
                // AssetDatabase.SaveAssets()
                //Debug.Log("changed");
                EditorUtility.SetDirty(this);
            }
        }
        // End Inspector

        // Add Include Folder
        private void IncludeFolder()
        {
            string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Include", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            pl.IncludeFolders.Add(new IncludeFolder(folderToAdd));

        }
        // Add Exclude Folder
        private void ExcludeFolder()
        {
            string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Exclude", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            pl.ExcludeFolders.Add(new ExcludeFolder(folderToAdd));
        }
        // Add Include File
        private void IncludeFile()
        {
            string get_Folder = EditorUtility.OpenFilePanel("Select File To Include", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            pl.IncludeFiles.Add(new IncludeFile(fileToAdd));
        }
        // Add Exclude File
        private void ExcludeFile()
        {
            string get_Folder = EditorUtility.OpenFilePanel("Select File To Exclude", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            pl.ExcludeFiles.Add(new ExcludeFile(fileToAdd));
        }
        // Remove Include Folder
        private void RemoveIncludeFolder(int index)
        {
            pl.IncludeFolders.RemoveAt(index);
        }
        // Remove Exclude Folder
        private void RemoveExcludeFolder(int index)
        {
            pl.ExcludeFolders.RemoveAt(index);
        }
        // Remove Include File
        private void RemoveIncludeFile(int index)
        {
            pl.IncludeFiles.RemoveAt(index);
        }
        // Remove Exclude File
        private void RemoveExcludeFile(int index)
        {
            pl.ExcludeFiles.RemoveAt(index);
        }

        // Set Target Directory
        private void OnSetTargetDirectory()
        {
            get_targetDirectory = EditorUtility.OpenFolderPanel(Application.dataPath, "", "*.*");
            pl.targetDirectory = get_targetDirectory + "/" + pl.packageName + ".unitypackage";
            pl.targetPackageTextFile = get_targetDirectory + "/" + pl.packageName + ".package.txt";
            int index = pl.targetDirectory.IndexOf("PlayMaker/Ecosystem");
            if (index != -1)
            {
                unityPackage = pl.targetDirectory.Substring(index);
            }
                
        }

        // Create the package
        private void CreatePackage()
        {
            includeFileList.Clear();
            //Include folders
            for (int count = 0; count < pl.IncludeFolders.Count; count++)
            {
                string directoryPath = pl.IncludeFolders[count].includeFolderString;
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                FileInfo[] info = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo f in info)
                {
                    string get_File = f.FullName;
                    if (get_File.Contains("PlayMaker.dll"))
                    {
                        Debug.LogError("Your Package containt a PlayMaker.dll file Which is not allow to be shared. Please remove from your list");
                        return;
                    }
                    int index = get_File.IndexOf("Assets");
                    fileToAdd = get_File.Substring(index);
                    fileToAdd = fileToAdd.Replace('\\', '/');

                    includeFileList.Add(fileToAdd);

                }
            }
            //Include files
            for (int count = 0; count < pl.IncludeFiles.Count; count++)
            {
                string filePathString = pl.IncludeFiles[count].includeFileString;
                if (!includeFileList.Contains(filePathString))
                {
                    includeFileList.Add(filePathString);
                }
            }
            // Exclude folders
            for (int count = 0; count < pl.ExcludeFolders.Count; count++)
            {
                string directoryPath = pl.ExcludeFolders[count].excludeFolderString;
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                FileInfo[] info = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo f in info)
                {
                    string get_Folder = f.FullName;
                    int indexfolder = get_Folder.IndexOf("Assets");
                    fileToAdd = get_Folder.Substring(indexfolder);
                    fileToAdd = fileToAdd.Replace('\\', '/');
                    if (includeFileList.Contains(fileToAdd))
                    {

                        int index = includeFileList.IndexOf(fileToAdd);
                        if (index != -1)
                        {
                            includeFileList.RemoveAt(index);
                        }

                    }

                }
            }
            //Exclude files
            for (int count = 0; count < pl.ExcludeFiles.Count; count++)
            {

                string filePathString = pl.ExcludeFiles[count].excludeFileString;
                if (includeFileList.Contains(filePathString))
                {
                    int index = includeFileList.IndexOf(filePathString);
                    if (index != -1)
                    {
                        includeFileList.RemoveAt(index);
                    }
                }
            }
            CreateTextFile();
        }
        // Create the Text file
        private void CreateTextFile()
        {
            PackageTextArray.Clear();

            PackageTextArray.Add("{");
            if (packageType != "") PackageTextArray.Add("\"" + "__ECO__" + "\"" + ":" + "\"" + packageType + "\"" + ",");
            if (packageType == "__PACKAGE__")
            {
                if (pl.type != "") PackageTextArray.Add("\"" + "Type" + "\"" + ":" + "\"" + pl.type + "\"" + ",");
            }
            if (pl.ecoFilter != "") PackageTextArray.Add("\"" + "EcoFilter" + "\"" + ":" + "\"" + pl.ecoFilter + "\"" + ",");
            if (pl.modules != "") PackageTextArray.Add("\"" + "UnityModules" + "\"" + ":[" + "\"" + pl.modules + "\"" + "],");
            if (pl.version != "") PackageTextArray.Add("\"" + "Version" + "\"" + ":" + "\"" + pl.version + "\"" + ",");
            if (pl.uMinVersion != "") PackageTextArray.Add("\"" + "UnityMinimumVersion" + "\"" + ":" + "\"" + pl.uMinVersion + "\"" + ",");
            if (pl.pmMinVersion != "") PackageTextArray.Add("\"" + "PlayMakerMinimumVersion" + "\"" + ":" + "\"" + pl.pmMinVersion + "\"" + ",");
            if (unityPackage != "") PackageTextArray.Add("\"" + "unitypackage" + "\"" + ":" + "\"" + unityPackage + "\"" + ",");
            if (pl.pingMenu != "") PackageTextArray.Add("\"" + "pingMenu" + "\"" + ":" + "\"" + pl.pingMenu + "\"" + ",");
            if (pl.assetPath != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(pl.assetPath);
                PackageTextArray.Add("\"" + "pingAssetPath" + "\"" + ":" + "\"" + assetPath + "\"" + ",");
            }
                if (pl.youTubeVidLink != "") PackageTextArray.Add("\"" + "YoutubeVideos" + "\"" + ":[" + "\"" + pl.youTubeVidLink + "\"" + "],");
            if (pl.webLink != "") PackageTextArray.Add("\"" + "WebLink" + "\"" + ":" + "\"" + pl.webLink + "\"" + ",");
            PackageTextArray.Add("\"" + "keywords" + "\"" + ":" + "\"" + pl.keyWords + "\"" + "");
            PackageTextArray.Add("}");
            
            BuildPackage();
        }
        // Create Package
        private void BuildPackage()
        {
            if (File.Exists(pl.targetDirectory))
            {
              //  MAKE POPUP IF EXISTS and ADD DON'T ASK AGAIN CHECKBOX
            }
            AssetDatabase.ExportPackage(includeFileList.ToArray(), pl.targetDirectory, ExportPackageOptions.Default);

            // TEST DELETE FILE BEFORE WRITING FILE
            if (File.Exists(pl.targetPackageTextFile))
            {
                File.Delete(pl.targetPackageTextFile);
            }

            StreamWriter packagetext = new StreamWriter(pl.targetPackageTextFile);

            foreach (string p in PackageTextArray)
            {
                packagetext.WriteLine(p);
            }
            packagetext.Close();

            EditorUtility.RevealInFinder(pl.targetDirectory);
        }

    }
}
