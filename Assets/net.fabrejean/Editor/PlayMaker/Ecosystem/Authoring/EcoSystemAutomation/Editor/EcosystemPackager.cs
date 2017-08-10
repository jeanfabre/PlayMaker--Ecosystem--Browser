﻿using UnityEngine;
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
        private Texture folderImage;



        public enum OPTIONS
        {
            ActionPackage = 0,
            SamplePackage = 1,
            TemplatePackage = 2
        }

        private void OnEnable()
        {
            pl = (PackageList)target;
            folderImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EcoSystemAutomation/Images/folderIcon.png", typeof(Texture));
        }
        // Inspector
        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Changes"))
            {
                CreatePackage();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Create Package"))
            {
                CreatePackage();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();

            // Set Package Name
            GUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Package Name", GUILayout.Width(150));
            pl.packageName = EditorGUILayout.TextField(pl.packageName, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            // Target Directory
            GUILayout.BeginHorizontal("box");
            GUILayout.Space(1);
            GUILayout.Label("Target Directory", GUILayout.Width(150));
            pl.targetDirectory = EditorGUILayout.TextField(pl.targetDirectory, GUILayout.MinWidth(1));
            if (GUILayout.Button(folderImage, GUILayout.Height(16), GUILayout.Width(24)))
            {
                OnSetTargetDirectory();
            }
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.Space(1);
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box");
            GUILayout.Space(1);

            // Set Package type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Package Type", GUILayout.Width(150));
            op = (OPTIONS)EditorGUILayout.EnumPopup("", op, GUILayout.MinWidth(5)); // change layout
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");

            }
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical("box");
            switch (op)
            {
                case OPTIONS.ActionPackage:
                    EditorGUILayout.Space();
                    packageType = ("__PACKAGE__");
                    break;

                case OPTIONS.SamplePackage:
                    EditorGUILayout.Space();
                    packageType = ("__SAMPLE__");
                    break;

                case OPTIONS.TemplatePackage:
                    EditorGUILayout.Space();
                    packageType = ("__TEMPLATE__");
                    break;

            } //Switch end

            // EcoFilter
            GUILayout.BeginHorizontal();
            GUILayout.Label("EcoFilter", GUILayout.Width(150));
            pl.ecoFilter = EditorGUILayout.TextField(pl.ecoFilter, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");

            }
            GUILayout.EndHorizontal();

            // Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width(150));
            pl.type = EditorGUILayout.TextField(pl.type, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Modules
            GUILayout.BeginHorizontal();
            GUILayout.Label("Modules", GUILayout.Width(150));
            pl.modules = EditorGUILayout.TextField(pl.modules, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Version
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version", GUILayout.Width(150));
            pl.version = EditorGUILayout.TextField(pl.version, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Unity Minimum Version
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unity Minimum Version", GUILayout.Width(150));
            pl.uMinVersion = EditorGUILayout.TextField(pl.uMinVersion, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // PlayMaker Minimum Version
            GUILayout.BeginHorizontal();
            GUILayout.Label("PlayMaker Min. Version", GUILayout.Width(150));
            pl.pmMinVersion = EditorGUILayout.TextField(pl.pmMinVersion, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Ping Asset Path
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ping Asset Path", GUILayout.Width(150));
            pl.assetPath = (Object)EditorGUILayout.ObjectField(pl.assetPath, typeof(Object), true);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            EditorGUILayout.EndHorizontal();

            // Keywords
            GUILayout.BeginHorizontal();
            GUILayout.Label("Keywords", GUILayout.Width(150));
            pl.keyWords = EditorGUILayout.TextField(pl.keyWords);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            
            // YouTube Link's
            GUILayout.BeginHorizontal();
            GUILayout.Label("YouTube Video Links", GUILayout.Width(150));
            if (GUILayout.Button("Add", GUILayout.Height(16), GUILayout.MinWidth(5)))
            {
                AddYoutubeString();
            }
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            // Show Each YouTube Link
            if(pl.youTubeLists.Count > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(5);
                for (int CountA = 0; CountA < pl.youTubeLists.Count; CountA++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(5));

                    pl.youTubeLists[CountA].youTubeLinkString = EditorGUILayout.TextField(pl.youTubeLists[CountA].youTubeLinkString);
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        RemoveYouTubeString(CountA);
                    }
                    GUILayout.Label("", GUILayout.Width(5));
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }


            
            // Web Link's
            GUILayout.BeginHorizontal();
            GUILayout.Label("Web Links", GUILayout.Width(150));
            if (GUILayout.Button("Add", GUILayout.Height(16), GUILayout.MinWidth(5)))
            {
                AddWebLinkString();
            }
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            // Show Each WebLink
            if(pl.webLinkList.Count > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(5);
                for (int CountA = 0; CountA < pl.webLinkList.Count; CountA++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(5));
                    pl.webLinkList[CountA].webLinkString = EditorGUILayout.TextField(pl.webLinkList[CountA].webLinkString);
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        RemoveWebLinkString(CountA);
                    }
                    GUILayout.Label("", GUILayout.Width(5));
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }
            


            GUILayout.Space(5);

            GUILayout.EndVertical();
            GUILayout.Space(10);
            // Add Buttons
            GUILayout.Label("Set Folders And Files");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Include Folder"))
            {
                IncludeFolder();
            }
            if (GUILayout.Button("Exclude Folder"))
            {
                ExcludeFolder();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Include File"))
            {
                IncludeFile();
            }
            if (GUILayout.Button("Exclude File"))
            {
                ExcludeFile();
            }
            GUILayout.EndHorizontal();
            // End Add Buttons

            // Include folder List
            GUILayout.BeginVertical();
            GUILayout.Label("Included Folder List");
            GUILayout.EndVertical();

            for (int CountA = 0; CountA < pl.includeFolders.Count; CountA++)
            {
                GUILayout.BeginHorizontal();
                pl.includeFolders[CountA].includeFolderString = EditorGUILayout.TextField(pl.includeFolders[CountA].includeFolderString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
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

            for (int CountB = 0; CountB < pl.excludeFolders.Count; CountB++)
            {
                GUILayout.BeginHorizontal();
                pl.excludeFolders[CountB].excludeFolderString = EditorGUILayout.TextField(pl.excludeFolders[CountB].excludeFolderString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
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

            for (int CountC = 0; CountC < pl.includeFiles.Count; CountC++)
            {
                GUILayout.BeginHorizontal();
                pl.includeFiles[CountC].includeFileString = EditorGUILayout.TextField(pl.includeFiles[CountC].includeFileString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
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

            for (int CountD = 0; CountD < pl.excludeFiles.Count; CountD++)
            {
                GUILayout.BeginHorizontal();
                pl.excludeFiles[CountD].excludeFileString = EditorGUILayout.TextField(pl.excludeFiles[CountD].excludeFileString, GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
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
                Repaint();
            }
        }
        // End Inspector


        private void AddYoutubeString()
        {
            string stringToAdd = "place link";
            pl.youTubeLists.Add(new YouTubeLink(stringToAdd));
        }

        private void AddWebLinkString()
        {
            string stringToAdd = "place link";
            pl.webLinkList.Add(new WebLink(stringToAdd));

        }

        // Add Include Folder
        private void IncludeFolder()
        {
            string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Include", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            pl.includeFolders.Add(new IncludeFolder(folderToAdd));

        }
        // Add Exclude Folder
        private void ExcludeFolder()
        {
            string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Exclude", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            pl.excludeFolders.Add(new ExcludeFolder(folderToAdd));
        }
        // Add Include File
        private void IncludeFile()
        {
            string get_Folder = EditorUtility.OpenFilePanel("Select File To Include", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            pl.includeFiles.Add(new IncludeFile(fileToAdd));
        }
        // Add Exclude File
        private void ExcludeFile()
        {
            string get_Folder = EditorUtility.OpenFilePanel("Select File To Exclude", Application.dataPath, "*.*");
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            pl.excludeFiles.Add(new ExcludeFile(fileToAdd));
        }
        // Remove YouTube String
        private void RemoveYouTubeString(int index)
        {
            pl.youTubeLists.RemoveAt(index);
        }
        // Remove WebLink String
        private void RemoveWebLinkString(int index)
        {
            pl.webLinkList.RemoveAt(index);
        }
        // Remove Include Folder
        private void RemoveIncludeFolder(int index)
        {
            pl.includeFolders.RemoveAt(index);
        }
        // Remove Exclude Folder
        private void RemoveExcludeFolder(int index)
        {
            pl.excludeFolders.RemoveAt(index);
        }
        // Remove Include File
        private void RemoveIncludeFile(int index)
        {
            pl.includeFiles.RemoveAt(index);
        }
        // Remove Exclude File
        private void RemoveExcludeFile(int index)
        {
            pl.excludeFiles.RemoveAt(index);
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
            for (int count = 0; count < pl.includeFolders.Count; count++)
            {
                string directoryPath = pl.includeFolders[count].includeFolderString;
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
            for (int count = 0; count < pl.includeFiles.Count; count++)
            {
                string filePathString = pl.includeFiles[count].includeFileString;
                if (!includeFileList.Contains(filePathString))
                {
                    includeFileList.Add(filePathString);
                }
            }
            // Exclude folders
            for (int count = 0; count < pl.excludeFolders.Count; count++)
            {
                string directoryPath = pl.excludeFolders[count].excludeFolderString;
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
            for (int count = 0; count < pl.excludeFiles.Count; count++)
            {

                string filePathString = pl.excludeFiles[count].excludeFileString;
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

            Debug.Log(pl.youTubeLists.Count);
            for (int count = 0; count < pl.youTubeLists.Count; count++)
            {
                string ytLink = pl.youTubeLists[count].youTubeLinkString; 
                PackageTextArray.Add("\"" + "YoutubeVideos" + "\"" + ":[" + "\"" + ytLink + "\"" + "],");
            }





            if (pl.webLink != "") PackageTextArray.Add("\"" + "WebLink" + "\"" + ":" + "\"" + pl.webLink + "\"" + ",");


            PackageTextArray.Add("\"" + "keywords" + "\"" + ":" + "\"" + pl.keyWords + "\"" + "");
            PackageTextArray.Add("}");

            BuildPackage();
        }
        // Create Package
        private void BuildPackage()
        {
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

        private void SaveChanges()
        {
            //SETUP SAVE CHANGES
        }
    }
}
