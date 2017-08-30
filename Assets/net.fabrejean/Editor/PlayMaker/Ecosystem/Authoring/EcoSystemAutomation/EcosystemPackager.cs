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
        public string op;
        private List<string> includeFileList = new List<string>();
        List<string> PackageTextArray = new List<string>();
        private string packageType;
        private string get_targetDirectory;
        private string unityPackage;
        private Texture folderImage;
        private bool overwriteWaring;
        private Object additem;
        private string addItemPath;
        private bool Canceled;
        private string packageTypeDirectory;
        private string packageTypeExtention;
        private string packagetextString;

        private void OnEnable()
        {
            pl = (PackageList)target;
            folderImage = (Texture)AssetDatabase.LoadAssetAtPath("Assets/EcoSystemAutomation/Editor/Images/folderIcon.png", typeof(Texture));
        }
        // Inspector
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            #region Top Buttons
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Check if file exist", GUILayout.Width(100));
            pl.fileExistsCheck = EditorGUILayout.Toggle(pl.fileExistsCheck, GUILayout.Width(15));
            if (GUILayout.Button("Create Package", GUILayout.Height(20)))
            {
                OnSetTargetDirectory();
                CreatePackage();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
            #endregion

            #region Text Edit Target and Package Type
            // Set Package Name
            GUILayout.BeginVertical("box");
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
            /*         GUILayout.BeginHorizontal("box");
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
           */
            GUILayout.BeginVertical("box");
            GUILayout.Space(1);

            // Set Package type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Package Type", GUILayout.Width(150));
           pl.packagetypeselected = EditorGUILayout.Popup(pl.packagetypeselected, pl.packagetypeselection);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");

            }
            
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            GUILayout.EndVertical();

            //Category
            EditorGUI.BeginDisabledGroup(pl.categoryString != "");
                GUILayout.BeginVertical("box");
                GUILayout.Space(1);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Select Category", GUILayout.Width(150));
                GUILayout.BeginVertical();
                pl.categorySelected = EditorGUILayout.Popup(pl.categorySelected, pl.categoryList);
                GUILayout.EndVertical();
                if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(1);

                GUILayout.Space(1);
            EditorGUI.EndDisabledGroup();
            // Category
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom Category", GUILayout.Width(150));
            pl.categoryString = EditorGUILayout.TextField(pl.categoryString, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            #endregion

            #region Text Edit Input Box
            // Type
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width(150));
            pl.type = EditorGUILayout.TextField(pl.type, GUILayout.MinWidth(5));
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

            // Keywords
            GUILayout.BeginHorizontal();
            GUILayout.Label("Keywords", GUILayout.Width(150));
            pl.keyWords = EditorGUILayout.TextField(pl.keyWords);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Unity Minimum Version
            GUILayout.BeginHorizontal();
            GUILayout.Label("Unity Minimum Version", GUILayout.Width(150));
            GUILayout.BeginVertical();
            pl.uMinVersionSelected = EditorGUILayout.Popup(pl.uMinVersionSelected, pl.uMinVersion);
            GUILayout.EndVertical();
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // PlayMaker Minimum Version
            GUILayout.BeginHorizontal();
            GUILayout.Label("PlayMaker Min. Version", GUILayout.Width(150));

            pl.pmMinVersionSelected = EditorGUILayout.Popup(pl.pmMinVersionSelected, pl.pmMinVersion);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();

            // Ping
            
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ping Type", GUILayout.Width(150));
            pl.Pingtypeselected = EditorGUILayout.Popup(pl.Pingtypeselected, pl.Pingtype);
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");

            }
            GUILayout.EndHorizontal();
            switch (pl.Pingtypeselected)
            {


                case 1:

                    // Ping Asset Path
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Select Asset Path", GUILayout.Width(150));
                    pl.assetPath = (Object)EditorGUILayout.ObjectField(pl.assetPath, typeof(Object), false);
                    if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        Application.OpenURL("http://www.jinxtergames.com/");
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    GUILayout.EndVertical();
                    break;

                case 2:
                    // Ping Menu
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(3);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Select Menu", GUILayout.Width(150));
                    pl.pingMenu = (Object)EditorGUILayout.ObjectField(pl.pingMenu, typeof(Object), false);
                    if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        Application.OpenURL("http://www.jinxtergames.com/");
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    GUILayout.EndVertical();
                    break;
            }



            // EcoFilter Link's
            GUILayout.BeginHorizontal();
            GUILayout.Label("EcoFilter", GUILayout.Width(150));
            if (GUILayout.Button("Add", GUILayout.Height(16), GUILayout.MinWidth(5)))
            {
                AddEcoFilterString();
            }
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            // Show Each EcoFilter Link
            if (pl.ecoFilterList.Count > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(5);
                for (int CountA = 0; CountA < pl.ecoFilterList.Count; CountA++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(5));

                    pl.ecoFilterList[CountA] = EditorGUILayout.TextField(pl.ecoFilterList[CountA]);
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        RemoveEcoFilterString(CountA);
                    }
                    GUILayout.Label("", GUILayout.Width(5));
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            // Modules Link's
            GUILayout.BeginHorizontal();
            GUILayout.Label("Modules", GUILayout.Width(150));
            if (GUILayout.Button("Add", GUILayout.Height(16), GUILayout.MinWidth(5)))
            {
                AddModuleString();
            }
            if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(15)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);
            // Show Each Module Link
            if (pl.modulesList.Count > 0)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(5);
                for (int CountA = 0; CountA < pl.modulesList.Count; CountA++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(5));

                    pl.modulesList[CountA] = EditorGUILayout.TextField(pl.modulesList[CountA]);
                    if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                    {
                        RemoveModuleString(CountA);
                    }
                    GUILayout.Label("", GUILayout.Width(5));
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }



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

                    pl.youTubeLists[CountA] = EditorGUILayout.TextField(pl.youTubeLists[CountA]);
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
                    pl.webLinkList[CountA] = EditorGUILayout.TextField(pl.webLinkList[CountA]);
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
            #endregion

            #region Set Folders And Files
            GUILayout.Space(10);
            // Add Buttons

            GUILayout.BeginVertical("box");
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Set Folders And Files", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // include dropbox
            GUILayout.BeginVertical("box");
            GUI.color = Color.green;
            EditorGUILayout.LabelField("Drop files or folder to include");
            additem = (Object)EditorGUILayout.ObjectField(additem, typeof(Object), false);
            GUI.color = new Color(1, 1, 1, 1);
            addItemPath = AssetDatabase.GetAssetPath(additem);
            if (additem != null)
            {
                if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(additem)))
                {
                    IncludeFolder();
                }
                else
                {
                    IncludeFile();
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(5);

            // exclude dropbox
            GUILayout.BeginVertical("box");
            GUI.color = Color.red;
            EditorGUILayout.LabelField("Drop files or folder to exclude");
            additem = (Object)EditorGUILayout.ObjectField(additem, typeof(Object), false);
            GUI.color = new Color(1, 1, 1, 1);
            addItemPath = AssetDatabase.GetAssetPath(additem);
            if (additem != null)
            {
                if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(additem)))
                {
                    ExcludeFolder();
                }
                else
                {
                    ExcludeFile();
                }
            }
            GUILayout.EndVertical();
            // End Add Buttons

            // Include folder List
            GUILayout.BeginVertical();
            GUILayout.Label("Included Folder List");
            GUILayout.EndVertical();

            for (int CountA = 0; CountA < pl.includeFolders.Count; CountA++)
            {
                GUILayout.BeginHorizontal();
                //   pl.includeFolders[CountA].includeFolderString = EditorGUILayout.TextField(pl.includeFolders[CountA].includeFolderString, GUILayout.MinWidth(200));
                // test
                pl.includeFolders[CountA] = EditorGUILayout.TextField(pl.includeFolders[CountA], GUILayout.MinWidth(200));
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
                pl.excludeFolders[CountB] = EditorGUILayout.TextField(pl.excludeFolders[CountB], GUILayout.MinWidth(200));
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
                pl.includeFiles[CountC] = EditorGUILayout.TextField(pl.includeFiles[CountC], GUILayout.MinWidth(200));
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
                pl.excludeFiles[CountD] = EditorGUILayout.TextField(pl.excludeFiles[CountD], GUILayout.MinWidth(200));
                if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                {
                    RemoveExcludeFile(CountD);
                    return;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(pl);
                Repaint();
            }
            #endregion
        }
        // End Inspector
        #region Add Strings, Folders, Files
        // Add EcoFilter String
        private void AddEcoFilterString()
        {
            string EcostringToAdd = "";
            pl.ecoFilterList.Add(EcostringToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Add Module String
        private void AddModuleString()
        {
            string modstringToAdd = "";
            pl.modulesList.Add(modstringToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Add youtube String
        private void AddYoutubeString()
        {
            string stringToAdd = "";
            pl.youTubeLists.Add(stringToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Add Weblink String
        private void AddWebLinkString()
        {
            string stringToAdd = "";
            pl.webLinkList.Add(stringToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();

        }

        // Add Include Folder
        private void IncludeFolder()
        {
            // string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Include", Application.dataPath, "*.*");
            string get_Folder = addItemPath;
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            additem = null;
            pl.includeFolders.Add(folderToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();

        }
        // Add Include File
        private void IncludeFile()
        {
            //   string get_Folder = EditorUtility.OpenFilePanel("Select File To Include", Application.dataPath, "*.*");
            string get_Folder = addItemPath;
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            additem = null;
            pl.includeFiles.Add(fileToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Add Exclude Folder
        private void ExcludeFolder()
        {
            //string get_Folder = EditorUtility.OpenFolderPanel("Select Folder To Exclude", Application.dataPath, "*.*");
            string get_Folder = addItemPath;
            int index = get_Folder.IndexOf("Assets");
            folderToAdd = get_Folder.Substring(index);
            additem = null;
            pl.excludeFolders.Add(folderToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Add Exclude File
        private void ExcludeFile()
        {
            // string get_Folder = EditorUtility.OpenFilePanel("Select File To Exclude", Application.dataPath, "*.*");
            string get_Folder = addItemPath;
            int index = get_Folder.IndexOf("Assets");
            fileToAdd = get_Folder.Substring(index);
            additem = null;
            pl.excludeFiles.Add(fileToAdd);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        #endregion

        #region Remove Strings, Folders, Files
        // Remove EcoFilter String
        private void RemoveEcoFilterString(int index)
        {
            pl.ecoFilterList.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove Module String
        private void RemoveModuleString(int index)
        {
            pl.modulesList.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove YouTube String
        private void RemoveYouTubeString(int index)
        {
            pl.youTubeLists.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove WebLink String
        private void RemoveWebLinkString(int index)
        {
            pl.webLinkList.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove Include Folder
        private void RemoveIncludeFolder(int index)
        {
            pl.includeFolders.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove Exclude Folder
        private void RemoveExcludeFolder(int index)
        {
            pl.excludeFolders.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove Include File
        private void RemoveIncludeFile(int index)
        {
            pl.includeFiles.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        // Remove Exclude File
        private void RemoveExcludeFile(int index)
        {
            pl.excludeFiles.RemoveAt(index);
            EditorUtility.SetDirty(pl);
            Repaint();
        }
        #endregion

        // Set Target Directory
        /*        private void OnSetTargetDirectory()
                {
                    get_targetDirectory = EditorUtility.OpenFolderPanel(Application.dataPath, "", "*.*");
                    pl.targetDirectory = get_targetDirectory;
                    int index = pl.targetDirectory.IndexOf("PlayMaker/Ecosystem");
                    if (index != -1)
                    {
                        unityPackage = pl.targetDirectory.Substring(index);
                        EditorUtility.SetDirty(pl);
                        Repaint();
                    }

                }
        */

        // Create the package
        private void OnSetTargetDirectory()
        {
            switch (pl.packagetypeselected)
            {
                case 0:
                    EditorGUILayout.Space();
                    packageType = ("__PACKAGE__");
                    packageTypeDirectory = "Custom Packages";
                    packageTypeExtention = ".package.txt";
                    break;

                case 1:
                    EditorGUILayout.Space();
                    packageType = ("__SAMPLE__");
                    packageTypeDirectory = "Custom Samples";
                    packageTypeExtention = ".sample.txt";
                    break;

                case 2:
                    EditorGUILayout.Space();
                    packageType = ("__TEMPLATE__");
                    packageTypeDirectory = "Custom Templates";
                    packageTypeExtention = ".template.txt";
                    break;

            } //Switch end
            string exportdirectory = Directory.GetCurrentDirectory();
            pl.targetDirectory = exportdirectory + "/PlayMaker/Ecosystem/" + packageTypeDirectory + "/" + pl.categoryList[pl.categorySelected];
            unityPackage = "/PlayMaker/Ecosystem/" + packageTypeDirectory + "/" + pl.categoryList[pl.categorySelected];
            Debug.Log(exportdirectory);
            return;
        }



        private void CreatePackage()
        {
            includeFileList.Clear();
            //Include folders and check for playmaker.dll
            for (int count = 0; count < pl.includeFolders.Count; count++)
            {
                string directoryPath = pl.includeFolders[count];
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
                string filePathString = pl.includeFiles[count];
                if (!includeFileList.Contains(filePathString))
                {
                    includeFileList.Add(filePathString);
                }
            }
            // Exclude folders
            for (int count = 0; count < pl.excludeFolders.Count; count++)
            {
                string directoryPath = pl.excludeFolders[count];
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

                string filePathString = pl.excludeFiles[count];
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
            pl.targetPackageTextFile = pl.targetDirectory + "/" + pl.packageName + packageTypeExtention;
            PackageTextArray.Clear();

            PackageTextArray.Add("{");
            if (packageType != "") PackageTextArray.Add("\"" + "__ECO__" + "\"" + ":" + "\"" + packageType + "\"" + ",");
            if (pl.type != "") PackageTextArray.Add("\"" + "Type" + "\"" + ":" + "\"" + pl.type + "\"" + ",");
            if (pl.version != "") PackageTextArray.Add("\"" + "Version" + "\"" + ":" + "\"" + pl.version + "\"" + ",");
            if (pl.uMinVersion[pl.uMinVersionSelected] != "") PackageTextArray.Add("\"" + "UnityMinimumVersion" + "\"" + ":" + "\"" + pl.uMinVersion[pl.uMinVersionSelected] + "\"" + ",");
            if (pl.pmMinVersion[pl.pmMinVersionSelected] != "") PackageTextArray.Add("\"" + "PlayMakerMinimumVersion" + "\"" + ":" + "\"" + pl.pmMinVersion[pl.pmMinVersionSelected] + "\"" + ",");
            if (unityPackage != "") PackageTextArray.Add("\"" + "unitypackage" + "\"" + ":" + "\"" + unityPackage + "\"" + ",");

           switch(pl.Pingtypeselected)
            {
                case 1:
                    if (pl.assetPath != null)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(pl.assetPath);
                        PackageTextArray.Add("\"" + "pingAssetPath" + "\"" + ":" + "\"" + assetPath + "\"" + ",");
                    }
                    break;


                case 2:
                    if (pl.pingMenu != null)
                    {
                        string pingMenu = AssetDatabase.GetAssetPath(pl.pingMenu);
                        PackageTextArray.Add("\"" + "pingMenu" + "\"" + ":" + "\"" + pingMenu + "\"" + ",");
                    }
                    break;
            }
            

            // For each EcoFilter
            for (int count = 0; count < pl.ecoFilterList.Count; count++)
            {
                string ecoFilterString = pl.ecoFilterList[count];
                if (ecoFilterString != "")
                {
                    PackageTextArray.Add("\"" + "EcoFilter" + "\"" + ":" + "\"" + ecoFilterString + "\"" + ",");
                }
            }
            // for each Module
            for (int count = 0; count < pl.modulesList.Count; count++)
            {
                string moduleString = pl.modulesList[count];
                if (moduleString != "")
                {
                    PackageTextArray.Add("\"" + "UnityModules" + "\"" + ":[" + "\"" + moduleString + "\"" + "],");
                }
            }
            // for each youtube string
            for (int count = 0; count < pl.youTubeLists.Count; count++)
            {
                string ytLinkString = pl.youTubeLists[count];
                if(ytLinkString != "")
                {
                    PackageTextArray.Add("\"" + "YoutubeVideos" + "\"" + ":[" + "\"" + ytLinkString + "\"" + "],");
                }
            }
            // for each weblink string
            for (int count = 0; count < pl.webLinkList.Count; count++)
            {
                string wbLinkString = pl.webLinkList[count];
                if (wbLinkString != "")
                {
                    PackageTextArray.Add("\"" + "WebLink" + "\"" + ":" + "\"" + wbLinkString + "\"" + ",");
                }
            }

            PackageTextArray.Add("\"" + "keywords" + "\"" + ":" + "\"" + pl.keyWords + "\"" + "");
            PackageTextArray.Add("}");

            BuildPackage();
        }

        #region BuildPackage
        private void BuildPackage()
        {
            Canceled = false;
            string exportdirectory = pl.targetDirectory + "/" + pl.packageName + ".unitypackage";
            if (Directory.Exists(pl.targetDirectory))
            {
                
                if (File.Exists(exportdirectory) && pl.fileExistsCheck == true)
                {

                    int option = EditorUtility.DisplayDialogComplex("This Package Exists already.", "Overwrite package?", "yes", "yes, Don't ask again", "No");
                    switch (option)
                    {
                        case 0:
                            File.Delete(exportdirectory);
                            AssetDatabase.ExportPackage(includeFileList.ToArray(), exportdirectory, ExportPackageOptions.Default);
                            break;
                        case 1:
                            pl.fileExistsCheck = false;
                            File.Delete(exportdirectory);
                            AssetDatabase.ExportPackage(includeFileList.ToArray(), exportdirectory, ExportPackageOptions.Default);
                            EditorUtility.SetDirty(pl);
                            Repaint();
                            break;
                        case 2:
                            EditorUtility.DisplayDialog("Build Canceled", "I saved your life!", "Thank You", "");
                            Canceled = true;
                            break;
                    }
                }
            }
            if (!Canceled)
            {
                

                // Complete Json Txt in packagetextString below

                bool jsonOk = false;
                JSON.JsonDecode(packagetextString, ref jsonOk);
                if(jsonOk)
                {
                    Directory.CreateDirectory(pl.targetDirectory);
                    StreamWriter packagetext = new StreamWriter(pl.targetPackageTextFile);
                    packagetextString = null;
                    foreach (string p in PackageTextArray)
                    {
                        packagetextString += p;
                        packagetextString += System.Environment.NewLine;
                    }
                    Debug.Log("isOk " + jsonOk);
                    packagetext.Write(packagetextString);
                    packagetext.Close();

                    Debug.Log(pl.targetDirectory);
                    
                    AssetDatabase.ExportPackage(includeFileList.ToArray(), exportdirectory, ExportPackageOptions.Default);

                    EditorUtility.RevealInFinder(exportdirectory);
                }
                else
                {
                    EditorUtility.DisplayDialog("Build Canceled Text File Error", "Did you use any { or } or : or " + '"' + "\ninside the info section?", "Ok", "");
                }

            }
        }
        #endregion
    }
}
