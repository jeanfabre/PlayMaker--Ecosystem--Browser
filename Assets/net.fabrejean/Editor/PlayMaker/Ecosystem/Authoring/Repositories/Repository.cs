using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace HutongGames.PlayMaker.Ecosystem.Publishing
{
	public class Repository {



		private Authoring.Repositories _target;
		/// <summary>
		/// Gets the repository target.
		/// </summary>
		/// <value>The target.</value>
		public Authoring.Repositories Target
		{
			get{
				return this._target;
			}

		}

		public string _localPath;
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="HutongGames.PlayMaker.Ecosystem.Publishing.Repository"/>
		/// local path.
		/// </summary>
		/// <value><c>true</c> if local path; otherwise, <c>false</c>.</value>
		public string LocalPath
		{
			get{
				return this._localPath;
			}

			set{
				this._localPath = value;
				Authoring.SetRepositoryLocalPath(this.Target,this._localPath);
			}
		}


		bool _isPathValid;
		/// <summary>
		/// Gets a value indicating whether this instance is path valid.
		/// </summary>
		/// <value><c>true</c> if this instance is path valid; otherwise, <c>false</c>.</value>
		public bool IsPathValid
		{
			get{
				return this._isPathValid;
			}

		}

		bool _isValidRepositoryPath;
		/// <summary>
		/// Gets a value indicating whether this instance is valid repository path.
		/// </summary>
		/// <value><c>true</c> if this instance is valid repository path; otherwise, <c>false</c>.</value>
		public bool IsValidRepositoryPath
		{
			get{
				return this._isValidRepositoryPath;
			}

		}

		private string _repositoryRootFile;
		public string RepositoryRootFile
		{
			get{
				return _repositoryRootFile;
			}
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="HutongGames.PlayMaker.Ecosystem.Publishing.Repository"/> class.
		/// </summary>
		public Repository(Authoring.Repositories target)
		{
			this._target = target;
			this._repositoryRootFile = "PlayMakerEcosystem." + this._target + ".repository.txt";
		}

		public bool CheckRepository()
		{
			this._isPathValid = false;
			this._isValidRepositoryPath = false;

			if(!string.IsNullOrEmpty(this._localPath))
			{
				this._isPathValid = Directory.Exists(this._localPath);


			}

			if (this._isPathValid)
			{
				string _filePath = this._localPath + Path.DirectorySeparatorChar + this._repositoryRootFile ;
				Debug.Log (_filePath);
				this._isValidRepositoryPath = File.Exists(_filePath);
			}

				
			return this._isValidRepositoryPath;
		}


		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="HutongGames.PlayMaker.Ecosystem.Publishing.Repository"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="HutongGames.PlayMaker.Ecosystem.Publishing.Repository"/>.</returns>
		public override string ToString()
		{
			return "Repository "+this._target+" isPathValid:"+this._isPathValid+" isPathRepValid"+this.IsValidRepositoryPath;
		}


	}
}