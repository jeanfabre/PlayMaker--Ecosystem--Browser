using UnityEditor;
using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Ecosystem.Publishing
{
	public class Authoring {

		private static string __namespace__ = "HutongGames.PlayMaker.Ecosystem.Publishing";


		public enum Repositories {
			Misc,
			Beta,
			Unity_4_Actions,
			Unity_5_Actions,
			Unity_5_Packages,
			Unity_2017_Actions,
			Unity_UI
		}

		public static string GetGithubUrl(Repositories rep)
		{
		
			return "";
		}


		#region repositories management
		public static  Repository[] repositories;
		#endregion

		static Repositories[] repositoriesLut;

		public static void GetAllRepositories()
		{

			if (Authoring.repositories != null)
			{
				return;
			}

			Debug.Log("GetAllRepositories");
			List<Repository> reps = new List<Repository>();
			List<Repositories> repsLut = new List<Repositories> ();
			foreach (int id in Enum.GetValues(typeof(Authoring.Repositories)) )
			{
				Repository _rep = new Repository ((Authoring.Repositories)id);
				reps.Add (_rep);
				repsLut.Add ((Authoring.Repositories)id);

				_rep.LocalPath = Authoring.GetRepositoryLocalPath (_rep.Target);
				_rep.CheckRepository ();
			}

			//  DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
			//  FileInfo[] goFileInfo = directory.GetFiles("*.unity", SearchOption.AllDirectories);


			repositories = reps.ToArray ();
			repositoriesLut = repsLut.ToArray ();
		}

		public static string GetRepositoryLocalPath(Repositories rep)
		{
			string prefKey = __namespace__ +"."+ rep.ToString ();

			Debug.Log (prefKey);

			if (EditorPrefs.HasKey(prefKey))
			{
					return EditorPrefs.GetString(prefKey);
			}

			return null;
		}

		public	static bool SetRepositoryLocalPath(Repositories rep,string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}

			string prefKey =  __namespace__ +"."+ rep.ToString ();

			Debug.Log ("SetRepositoryLocalPath for "+prefKey+" to <"+path+">");

			EditorPrefs.SetString(prefKey,path);

			return true;
		}


		public static bool UserHasRepository(Repositories rep)
		{

			Repository _rep = GetRepository (rep);
			

			return _rep.IsValidRepositoryPath;
		}

		/// <summary>
		/// Gets the repository.
		/// </summary>
		/// <returns>The repository.</returns>
		/// <param name="rep">Rep.</param>
		public static Repository GetRepository(Repositories rep)
		{
			if (repositories == null || repositories.Length == 0)
			{
				GetAllRepositories ();
			}

			int index = ArrayUtility.IndexOf<Repositories>(repositoriesLut, rep);

			return repositories[index];
		}

	}
}
