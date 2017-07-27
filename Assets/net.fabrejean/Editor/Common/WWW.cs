#if UNITY_2017 && UNITY_2017_1

using System;
using System.Net;
using System.Collections;
using System.Text;
using UnityEngine;
using System.ComponentModel;

namespace HutongGames.PlayMaker.Ecosystem.Utils
{

	#pragma warning disable CS0168 

	/// <summary>
	/// Alternative to Unity WWW class that doesn't work properly in 2017.0 and 2017.1, Only implemented the Interface I was using.
	/// </summary>
	public class WWW : CustomYieldInstruction, IDisposable{


		private WebClient webClient;

		private bool _isDone;

		public bool isDone 
		{
			get{
				return _isDone;
			}
		}

		private string _url;
		public string url{
			get{
				return _url;
			}
		}

		private string _error;
		public string error{
			get{
				return _error;
			}
		}

		private string _text = null;
		public string text{
			get{
				if (!_isDone)
					return string.Empty;
				
				if (_text == null)
				{
					try
					{
						_text = Encoding.UTF8.GetString (_bytes);
					} catch (Exception e)
					{
						_text = string.Empty;
					}
				}

				return _text;
			}
		}

		private byte[] _bytes;
		public byte[] bytes{
			get{
				return _bytes;
			}
		}

		public WWW (string url)
		{
			_url = url;
			this.webClient = new WebClient ();

			this.webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;
			//this.webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

			this.webClient.DownloadDataAsync (new Uri (url));
		}

		void WebClient_DownloadProgressChanged (object sender, DownloadProgressChangedEventArgs e)
		{
		//	Debug.Log ("WWW downloading " +e.ProgressPercentage + " "+ e.TotalBytesToReceive );
		}
			
		void WebClient_DownloadDataCompleted (object sender, DownloadDataCompletedEventArgs e)
		{
			this.webClient.DownloadDataCompleted -= WebClient_DownloadDataCompleted;
	
			try{
				_error = e.Error.Message;
			}catch(Exception ee)
			{
				_error = "";
			}

			try{
				_bytes = e.Result;
			}catch(Exception ee)
			{
				_bytes = null;
			}

			_isDone = true;
		}


		#region IDisposable implementation

		public void Dispose ()
		{
			this.webClient.Dispose ();
			this.webClient = null;
		}

		#endregion

		#region implemented abstract members of CustomYieldInstruction

		public override bool keepWaiting {
			get {
				return !_isDone;
			}
		}

		#endregion
	}			
}
#endif