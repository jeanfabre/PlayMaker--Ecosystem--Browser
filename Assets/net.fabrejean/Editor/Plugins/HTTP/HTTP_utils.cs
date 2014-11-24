using UnityEngine;
using System.Collections;
using System.IO;
using System;
using HTTP;

namespace HTTP
{
	public static class URL
	{
		public static string Encode(string content)
		{
			return WWW.EscapeURL(content);
		}
	}
	
}