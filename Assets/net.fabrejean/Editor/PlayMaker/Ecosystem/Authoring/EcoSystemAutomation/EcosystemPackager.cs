using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;

using HutongGames.PlayMaker.Ecosystem.Publishing;

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
        private string selectedRepResult;
        private string displayExcludeFileList;
        private string displayIncludeFileList;
        private string displayIncludeFolderList;
        private string displayExcludeFolderList;
        private int widthQM = 18; // width question marks
        private int heightQM = 15; // height question marks

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
            if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                       if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
            if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");

            }

            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            GUILayout.EndVertical();


			// Target Repository
			GUILayout.BeginVertical("box");
           	GUILayout.Space(1);
			GUILayout.BeginHorizontal();
           	GUILayout.Label("Target Repository", GUILayout.Width(150));
           
			pl.targetRepository = (Authoring.Repositories)EditorGUILayout.EnumPopup(pl.targetRepository);

		
			GUILayout.EndHorizontal();

          	GUILayout.Space(1);
			if (!Authoring.UserHasRepository(pl.targetRepository))
			{


				GUILayout.BeginHorizontal();
					GUILayout.Label(" ", GUILayout.Width(150));
					GUI.color = Color.red;
					GUILayout.Label("missing Repository","Box");
					GUI.color = Color.white;
					if (GUILayout.Button("Fix", GUILayout.Width(30), GUILayout.Height(15)))
					{
						EditorApplication.ExecuteMenuItem("PlayMaker/Addons/Ecosystem/Authoring/Repositories browser");
					}

					if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
					{
						Application.OpenURL("http://www.jinxtergames.com/");
					}

				GUILayout.EndHorizontal();

			

				GUILayout.Space(1);
			}

			GUILayout.EndVertical();
          

            //Category
            EditorGUI.BeginDisabledGroup(pl.categoryString != "" && pl.categoryString != null);
            GUILayout.BeginVertical("box");
            GUILayout.Space(1);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Category", GUILayout.Width(150));
            GUILayout.BeginVertical();
            pl.categorySelected = EditorGUILayout.Popup(pl.categorySelected, pl.categoryList);
            GUILayout.EndVertical();
            if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(1);

            GUILayout.Space(1);
            EditorGUI.EndDisabledGroup();
            // Custom Category
            GUILayout.BeginHorizontal();
            GUILayout.Label("Custom Category", GUILayout.Width(150));
            pl.categoryString = EditorGUILayout.TextField(pl.categoryString, GUILayout.MinWidth(5));
            if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
            {
                Application.OpenURL("http://www.jinxtergames.com/");
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            #endregion

            #region Text Edit Input Box
            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            pl.foldout = EditorGUILayout.Foldout(pl.foldout, "Info setup");
            GUILayout.EndHorizontal();
            if (pl.foldout)
            {
                
            
                // Author
                GUILayout.BeginHorizontal();
                GUILayout.Label("Author", GUILayout.Width(150));
                pl.author = EditorGUILayout.TextField(pl.author, GUILayout.MinWidth(5));
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();

                // Type
                GUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.Width(150));
                pl.type = EditorGUILayout.TextField(pl.type, GUILayout.MinWidth(5));
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();


                // Version
                GUILayout.BeginHorizontal();
                GUILayout.Label("Version", GUILayout.Width(150));
                pl.version = EditorGUILayout.TextField(pl.version, GUILayout.MinWidth(5));
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();

                // Keywords
                GUILayout.BeginHorizontal();
                GUILayout.Label("Keywords", GUILayout.Width(150));
                pl.keyWords = EditorGUILayout.TextField(pl.keyWords);
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();

                // PlayMaker Minimum Version
                GUILayout.BeginHorizontal();
                GUILayout.Label("PlayMaker Min. Version", GUILayout.Width(150));

                pl.pmMinVersionSelected = EditorGUILayout.Popup(pl.pmMinVersionSelected, pl.pmMinVersion);
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();

                // Ping

                GUILayout.Space(8);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Ping Type", GUILayout.Width(150));
                pl.Pingtypeselected = EditorGUILayout.Popup(pl.Pingtypeselected, pl.Pingtype);
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                        if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                        if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                            GUI.FocusControl(null);
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
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
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
                            GUI.FocusControl(null);
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
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
                // Show Each YouTube Link
                if (pl.youTubeLists.Count > 0)
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
                            GUI.FocusControl(null);
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
                if (GUILayout.Button("?", GUILayout.Width(widthQM), GUILayout.Height(heightQM)))
                {
                    Application.OpenURL("http://www.jinxtergames.com/");
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
                // Show Each WebLink
                if (pl.webLinkList.Count > 0)
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
                            GUI.FocusControl(null);
                        }
                        GUILayout.Label("", GUILayout.Width(5));
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
                GUILayout.Space(5);
               
            }
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

            GUILayout.BeginHorizontal();
            // include dropbox
            GUILayout.BeginVertical("box");
            GUI.color = Color.green;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Included Files And Folders", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            InCludeDropAreaGUI();
            GUI.color = new Color(1, 1, 1, 1);
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);

            // exclude dropbox
            GUILayout.BeginVertical("box");
            GUI.color = Color.red;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Excluded Files And Folders", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            ExCludeDropAreaGUI();
            GUI.color = new Color(1, 1, 1, 1);
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            // End Add Buttons
            GUILayout.EndHorizontal();
            #endregion

            #region fileList
            GUILayout.BeginVertical("box");

            // Include Folder List
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Included Folder List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(displayIncludeFolderList, GUILayout.Width(50)))
            {
                pl.showIncludeFolderList = !pl.showIncludeFolderList;
            }
            GUILayout.EndHorizontal();

            if (pl.showIncludeFolderList)
            {
                displayIncludeFolderList = "Hide";

                if (pl.includeFolders.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUI.color = Color.green;
                    for (int CountA = 0; CountA < pl.includeFolders.Count; CountA++)
                    {
                        GUILayout.BeginHorizontal("box");
                        string assetPath = AssetDatabase.GUIDToAssetPath(pl.includeFolders[CountA]);
                        if (GUILayout.Button(assetPath, "label", GUILayout.MaxWidth(200), GUILayout.ExpandWidth(true)))
                        {

                            HighlightIncludeFolder(CountA);
                            return;
                        }

                        GUI.color = new Color(1, 1, 1, 1);
                        if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                        {
                            RemoveIncludeFolder(CountA);
                            return;
                        }
                        GUI.color = Color.green;
                        GUILayout.EndHorizontal();
                    }
                    GUI.color = new Color(1, 1, 1, 1);
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No Folders included");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                displayIncludeFolderList = "Show";
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();

            // Include File List
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Included File List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(displayIncludeFileList, GUILayout.Width(50)))
            {
                pl.showIncludeFileList = !pl.showIncludeFileList;
            }
            GUILayout.EndHorizontal();
            if (pl.showIncludeFileList)
            {
                displayIncludeFileList = "Hide";

                if (pl.includeFiles.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUI.color = Color.green;
                    for (int CountA = 0; CountA < pl.includeFiles.Count; CountA++)
                    {
                        GUILayout.BeginHorizontal("box");
                        string assetPath = AssetDatabase.GUIDToAssetPath(pl.includeFiles[CountA]);
                        if (GUILayout.Button(assetPath, "label", GUILayout.MinWidth(200), GUILayout.ExpandWidth(true)))
                        {
                            HighlightIncludeFile(CountA);
                            return;
                        }
                        GUI.color = new Color(1, 1, 1, 1);

                        if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                        {
                            RemoveIncludeFile(CountA);
                            return;
                        }
                        GUI.color = Color.green;
                        GUILayout.EndHorizontal();
                    }
                    GUI.color = new Color(1, 1, 1, 1);
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No Files included");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                displayIncludeFileList = "Show";
            }
            GUILayout.Space(5);

            GUILayout.EndVertical();
            // Exclude folder List
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Excluded Folder List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(displayExcludeFolderList, GUILayout.Width(50)))
            {
                pl.showExcludeFolderList = !pl.showExcludeFolderList;
            }
            GUILayout.EndHorizontal();
            if (pl.showExcludeFolderList)
            {
                displayExcludeFolderList = "Hide";
                if (pl.excludeFolders.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUI.color = Color.red;
                    for (int CountA = 0; CountA < pl.excludeFolders.Count; CountA++)
                    {
                        GUILayout.BeginHorizontal("box");
                        string assetPath = AssetDatabase.GUIDToAssetPath(pl.excludeFolders[CountA]);
                        if (GUILayout.Button(assetPath, "label", GUILayout.MinWidth(200), GUILayout.ExpandWidth(true)))
                        {
                            HighlightExcludeFolder(CountA);
                            return;
                        }
                        GUI.color = new Color(1, 1, 1, 1);
                        if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                        {
                            RemoveExcludeFolder(CountA);
                            return;
                        }
                        GUI.color = Color.red;
                        GUILayout.EndHorizontal();
                    }
                    GUI.color = new Color(1, 1, 1, 1);
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No Folders included");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                displayExcludeFolderList = "Show";
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
            // Exclude File List
            GUILayout.BeginVertical("box");
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Excluded File List", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(displayExcludeFileList, GUILayout.Width(50)))
            {
                pl.showExcludeFileList = !pl.showExcludeFileList;
            }
            GUILayout.EndHorizontal();
            if (pl.showExcludeFileList)
            {
                displayExcludeFileList = "Hide";

                if (pl.excludeFiles.Count > 0)
                {
                    GUILayout.BeginVertical("box");
                    GUILayout.Space(5);
                    GUI.color = Color.red;
                    for (int CountA = 0; CountA < pl.excludeFiles.Count; CountA++)
                    {
                        GUILayout.BeginHorizontal("box");
                        string assetPath = AssetDatabase.GUIDToAssetPath(pl.excludeFiles[CountA]);
                        if (GUILayout.Button(assetPath, "label", GUILayout.MinWidth(200), GUILayout.ExpandWidth(true)))
                        {
                            HighlightExcludeFile(CountA);
                            return;
                        }
                        GUI.color = new Color(1, 1, 1, 1);
                        if (GUILayout.Button("X", GUILayout.Width(16), GUILayout.Height(15)))
                        {
                            RemoveExcludeFile(CountA);
                            return;
                        }
                        GUI.color = Color.red;
                        GUILayout.EndHorizontal();
                    }
                    GUI.color = new Color(1, 1, 1, 1);
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal("box");
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No Files included");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                displayExcludeFileList = "Show";
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
            #endregion

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(pl);
                Repaint();

                #region Quotation Checker
                string charsToCheck = "\"";

                if (pl.categoryString != null)
                {
                    if (pl.categoryString.Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.categoryString = pl.categoryString.Remove(pl.categoryString.Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                if (pl.author != null)
                {
                    if (pl.author.Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.author = pl.author.Remove(pl.author.Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                if (pl.type != null)
                {
                    if (pl.type.Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.type = pl.type.Remove(pl.type.Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                if(pl.version != null)
                {
                    if (pl.version.Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.version = pl.version.Remove(pl.version.Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                if(pl.keyWords != null)
                {
                    if (pl.keyWords.Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.keyWords = pl.keyWords.Remove(pl.keyWords.Length - 1);
                        GUI.FocusControl(null);
                    }
                }
                
                for (int i = 0; i < pl.ecoFilterList.Count; i++)
                {
                    if (pl.ecoFilterList[i].Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.ecoFilterList[i] = pl.ecoFilterList[i].Remove(pl.ecoFilterList[i].Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                for (int i = 0; i < pl.modulesList.Count; i++)
                {
                    if (pl.modulesList[i].Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.modulesList[i] = pl.modulesList[i].Remove(pl.modulesList[i].Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                for (int i = 0; i < pl.youTubeLists.Count; i++)
                {
                    if (pl.youTubeLists[i].Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.youTubeLists[i] = pl.youTubeLists[i].Remove(pl.youTubeLists[i].Length - 1);
                        GUI.FocusControl(null);
                    }
                }

                for (int i = 0; i < pl.youTubeLists.Count; i++)
                {
                    if (pl.webLinkList[i].Contains(charsToCheck))
                    {
                        EditorUtility.DisplayDialog("Quotation marks are not allowed", "Quotation marks will break the Json file", "Ok", "");
                        pl.webLinkList[i] = pl.webLinkList[i].Remove(pl.webLinkList[i].Length - 1);
                        GUI.FocusControl(null);
                    }
                }
                #endregion

            }
            
        }

        #region drobox
        // TEST DROPBOX
        public void InCludeDropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "\nDrop you files\n and folders in here");
            //  
            //  GUILayout.Box("\nDrop you files\n and folders in here", GUILayout.Height(60), GUILayout.ExpandWidth(true));
            //   GUILayout.EndHorizontal();

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {
                            if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(dragged_object)))
                            {
                                addItemPath = AssetDatabase.GetAssetPath(dragged_object);
                                string get_Folder = addItemPath;
                                pl.foldersToInclude.Add(get_Folder);
                            }
                            else
                            {
                                addItemPath = AssetDatabase.GetAssetPath(dragged_object);
                                string get_Folder = addItemPath;
                                pl.filesToInclude.Add(get_Folder);
                            }
                        }
                    }
                    break;
            }
            if (pl.foldersToInclude.Count > 0 && Event.current.type == EventType.Layout)
            {
                IncludeFolder();
            }
            if (pl.filesToInclude.Count > 0 && Event.current.type == EventType.Layout)
            {
                IncludeFile();
            }
        }

        public void ExCludeDropAreaGUI()
        {
            Event evt = Event.current;
            Rect drop_area2 = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area2, "\nDrop you files\n and folders in here");
            // GUILayout.BeginHorizontal();
            //  GUILayout.Box("\nDrop you files\n and folders in here", GUILayout.Height(60), GUILayout.ExpandWidth(true));
            //  GUILayout.EndHorizontal();

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area2.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (Object dragged_object in DragAndDrop.objectReferences)
                        {

                            if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(dragged_object)))
                            {
                                addItemPath = AssetDatabase.GetAssetPath(dragged_object);
                                string get_Folder = addItemPath;
                                pl.foldersToExclude.Add(get_Folder);
                            }
                            else
                            {
                                addItemPath = AssetDatabase.GetAssetPath(dragged_object);
                                string get_Folder = addItemPath;
                                pl.filesToExclude.Add(get_Folder);

                            }
                        }
                    }
                    break;
            }
            if (pl.foldersToExclude.Count > 0 && Event.current.type == EventType.Layout)
            {
                ExcludeFolder();
            }
            if (pl.filesToExclude.Count > 0 && Event.current.type == EventType.Layout)
            {
                ExcludeFile();
            }
        }
        // TEST END DROPBOX
        #endregion

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
            for (int i = 0; i < pl.foldersToInclude.Count; i++)
            {
                folderToAdd = AssetDatabase.AssetPathToGUID(pl.foldersToInclude[i]);

                if (pl.includeFolders.Contains(folderToAdd) || pl.excludeFolders.Contains(folderToAdd))
                {
                    if (pl.includeFolders.Contains(folderToAdd))
                    {
                        EditorUtility.DisplayDialog("Folder Exists", pl.foldersToInclude[i] + "\n\n already included in the Include Folder", "Ok", "");
                    }
                    if (pl.excludeFolders.Contains(folderToAdd))
                    {
                        bool option = EditorUtility.DisplayDialog("Folder Exists", pl.foldersToInclude[i] + "\n\n already included in the Exclude Folders\n\nDo you want to remove from the Exclude Folders and add here?", "Yes", "No");
                        if (option)
                        {
                            pl.excludeFolders.Remove(folderToAdd);
                            pl.includeFolders.Add(folderToAdd);
                        }
                    }
                }
                else
                {
                    pl.includeFolders.Add(folderToAdd);
                }

            }
            EditorUtility.SetDirty(pl);
            Repaint();
            pl.foldersToInclude.Clear();
        }
        // Add Include File
        private void IncludeFile()
        {
            for (int i = 0; i < pl.filesToInclude.Count; i++)
            {
                fileToAdd = AssetDatabase.AssetPathToGUID(pl.filesToInclude[i]);


                if (pl.includeFiles.Contains(fileToAdd))
                {
                    EditorUtility.DisplayDialog("File Exists", pl.filesToInclude[i] + "\n\n already included in the Include Files", "Ok", "");
                }
                else if (pl.excludeFiles.Contains(fileToAdd))
                {
                    bool option = EditorUtility.DisplayDialog("File Exists", pl.filesToInclude[i] + "\n\n already included in the Exclude files\n\nDo you want to remove from the exclude files and add here?", "Yes", "No");
                    if (option)
                    {
                        pl.excludeFiles.Remove(fileToAdd);
                        pl.includeFiles.Add(fileToAdd);
                    }
                }
                else
                {
                    pl.includeFiles.Add(fileToAdd);
                }
            }
            EditorUtility.SetDirty(pl);
            Repaint();
            pl.filesToInclude.Clear();
        }
        // Add Exclude Folder
        private void ExcludeFolder()
        {
            for (int i = 0; i < pl.foldersToExclude.Count; i++)
            {
                folderToAdd = AssetDatabase.AssetPathToGUID(pl.foldersToExclude[i]);

                if (pl.excludeFolders.Contains(folderToAdd))
                {
                    EditorUtility.DisplayDialog("Folder Exists", pl.foldersToExclude[i] + "\n\n already included in the Include Folders", "Ok", "");
                }
                else if (pl.includeFolders.Contains(folderToAdd))
                {
                    bool option = EditorUtility.DisplayDialog("Folder Exists", pl.foldersToExclude[i] + "\n\n already included in the Include Folders\n\nDo you want to remove from the Include Folder and add here?", "Yes", "No");
                    if (option)
                    {
                        pl.includeFolders.Remove(folderToAdd);
                        pl.excludeFolders.Add(folderToAdd);
                    }
                }
                else
                {
                    pl.excludeFolders.Add(folderToAdd);
                }

            }
            EditorUtility.SetDirty(pl);
            Repaint();
            pl.foldersToExclude.Clear();
        }
        // Add Exclude File
        private void ExcludeFile()
        {
            for (int i = 0; i < pl.filesToExclude.Count; i++)
            {
                fileToAdd = AssetDatabase.AssetPathToGUID(pl.filesToExclude[i]);

                if (pl.excludeFiles.Contains(fileToAdd))
                {
                    EditorUtility.DisplayDialog("File Exists", pl.filesToExclude[i] + "\n\n already included in the Exclude files", "Ok", "");
                }
                else if (pl.includeFiles.Contains(fileToAdd))
                {
                    bool option = EditorUtility.DisplayDialog("File Exists", pl.filesToExclude[i] + "\n\n already included in the Include files\n\nDo you want to remove from the Include files and add here?", "Yes", "No");
                    if (option)
                    {
                        pl.includeFiles.Remove(fileToAdd);
                        pl.excludeFiles.Add(fileToAdd);
                    }
                }
                else
                {
                    pl.excludeFiles.Add(fileToAdd);
                }

            }
            EditorUtility.SetDirty(pl);
            Repaint();
            pl.filesToExclude.Clear();
        }
        #endregion

        #region Highlight Folders

        // Remove Include Folder
        private void HighlightIncludeFolder(int index)
        {
            string path = AssetDatabase.GUIDToAssetPath(pl.includeFolders[index]);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        // Remove Exclude Folder
        private void HighlightExcludeFolder(int index)
        {
            string path = AssetDatabase.GUIDToAssetPath(pl.excludeFolders[index]);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        // Remove Include File
        private void HighlightIncludeFile(int index)
        {
            string path = AssetDatabase.GUIDToAssetPath(pl.includeFiles[index]);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }
        // Remove Exclude File
        private void HighlightExcludeFile(int index)
        {
            string path = AssetDatabase.GUIDToAssetPath(pl.excludeFiles[index]);
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
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
        }



        private void CreatePackage()
        {
            // Check For Package Name
            if (pl.packageName == "" || pl.packageName == null)
            {
                EditorUtility.DisplayDialog("No Package Name", "There is no package Name defined\n\nPlease provide a package name", "Ok", "");
                return;
            }


            // Target Repository
            string selectedRepEnum = pl.targetRepository.ToString();
            string selectedRepString = "HutongGames.PlayMaker.Ecosystem.Publishing." + selectedRepEnum;

            selectedRepResult = EditorPrefs.GetString(selectedRepString);
            if(selectedRepResult == "")
            {
             EditorUtility.DisplayDialog("\nWrong Target Repository", "Create Package aborted\n\nis the target repository valid (not missing)", "Ok");
                return;

            }


            pl.targetDirectory = selectedRepResult + "/PlayMaker/Ecosystem/" + packageTypeDirectory + "/" + pl.categoryList[pl.categorySelected];
            unityPackage = "/PlayMaker/Ecosystem/" + packageTypeDirectory + "/" + pl.categoryList[pl.categorySelected];
            Debug.Log(selectedRepResult);

            includeFileList.Clear();
            //Include folders and check for playmaker.dll
            for (int count = 0; count < pl.includeFolders.Count; count++)
            {
                string directoryPath = AssetDatabase.GUIDToAssetPath(pl.includeFolders[count]);
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                FileInfo[] info = dir.GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo f in info)
                {
                    string get_File = f.FullName;

                    int index = get_File.IndexOf("Assets");
                    fileToAdd = get_File.Substring(index);
                    fileToAdd = fileToAdd.Replace('\\', '/');
                    includeFileList.Add(fileToAdd);

                }
            }

            //Include files
            for (int count = 0; count < pl.includeFiles.Count; count++)
            {
                string filePathString = AssetDatabase.GUIDToAssetPath(pl.includeFiles[count]);
                if (!includeFileList.Contains(filePathString))
                {
                    includeFileList.Add(filePathString);
                }
            }
            // Exclude folders
            for (int count = 0; count < pl.excludeFolders.Count; count++)
            {
                string directoryPath = AssetDatabase.GUIDToAssetPath(pl.excludeFolders[count]);
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

                string filePathString = AssetDatabase.GUIDToAssetPath(pl.excludeFiles[count]);
                if (includeFileList.Contains(filePathString))
                {
                    int index = includeFileList.IndexOf(filePathString);
                    if (index != -1)
                    {
                        includeFileList.RemoveAt(index);
                    }
                }
            }

            for (int i = 0; i < includeFileList.Count; i++)
            {
                if (includeFileList[i].Contains("PlayMaker.dll"))
                {
                    Debug.Log("the folder" + includeFileList[i]);
                    Debug.LogError("Your Package containt a PlayMaker.dll file Which is not allow to be shared. Please remove from your list");
                    Object obj = AssetDatabase.LoadAssetAtPath(includeFileList[i], typeof(Object));
                    Debug.Log(obj);
                    EditorGUIUtility.PingObject(obj);
                    return;
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
            if (pl.author != "") PackageTextArray.Add("\"" + "Type" + "\"" + ":" + "\"" + pl.author + "\"" + ",");
            if (pl.type != "") PackageTextArray.Add("\"" + "Type" + "\"" + ":" + "\"" + pl.type + "\"" + ",");
            if (pl.version != "") PackageTextArray.Add("\"" + "Version" + "\"" + ":" + "\"" + pl.version + "\"" + ",");
            if (pl.uMinVersion[pl.uMinVersionSelected] != "") PackageTextArray.Add("\"" + "UnityMinimumVersion" + "\"" + ":" + "\"" + pl.uMinVersion[pl.uMinVersionSelected] + "\"" + ",");
            if (pl.pmMinVersion[pl.pmMinVersionSelected] != "") PackageTextArray.Add("\"" + "PlayMakerMinimumVersion" + "\"" + ":" + "\"" + pl.pmMinVersion[pl.pmMinVersionSelected] + "\"" + ",");
			if (unityPackage != "") PackageTextArray.Add("\"" + "unitypackage" + "\"" + ":" + "\"" + unityPackage + "/" + pl.packageName + ".unitypackage" + "\"" +  ",");

            switch (pl.Pingtypeselected)
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
                if (ytLinkString != "")
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
				packagetextString = null;
				int i = 1;
				foreach (string p in PackageTextArray)
				{
					packagetextString += p;
					if (i < PackageTextArray.Count) {
						packagetextString += System.Environment.NewLine;
					}
					i++;
				}

                bool jsonOk = false;
                JSON.JsonDecode(packagetextString, ref jsonOk);
                if (jsonOk)
                {
                    Directory.CreateDirectory(pl.targetDirectory);

					File.WriteAllText(pl.targetPackageTextFile, packagetextString);

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
