using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Net.FabreJean.UnityEditor
{
	/// <summary>
	/// Ecosystem utils. Set of common methods and tools.
	/// </summary>
	public class EcosystemUtils {

		/// <summary>
		/// Parses an url query string like ?variable=value&anotherVariable
		/// </summary>
		/// <returns>The query string.</returns>
		/// <param name="query">Query.</param>
		public static Dictionary<string, string> ParseQueryString(String query)
		{
			Dictionary<String, String> queryDict = new Dictionary<string, string>();
			foreach (String token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 2)
					queryDict[parts[0].Trim()] = WWW.UnEscapeURL(parts[1]).Trim();
				else
					queryDict[parts[0].Trim()] = "";
			}
			return queryDict;
		}

		/// <summary>
		/// Extracts the meta data from text. expect a json content encapsulated between EcoMetaStart and EcoMetaEnd
		/// </summary>
		/// <returns>The meta data from text.</returns>
		/// <param name="text">Text.</param>
		public static Hashtable ExtractEcoMetaDataFromText(string text)
		{
			
			// check for Meta data
			Match match = Regex.Match(text,@"(?<=EcoMetaStart)[^>]*(?=EcoMetaEnd)",RegexOptions.IgnoreCase);
			
			// Here we check the Match instance.
			if (match.Success)
			{
				//	Debug.Log("we have meta data :" + match.Value);
				return  (Hashtable)JSON.JsonDecode(match.Value);
			}

			return new Hashtable();
		}

		public string InsertEcoMetaDataToText(string originalText,Hashtable json)
		{
			//string jsonString = JSON.JsonEncode(json);

			//string ecoMetaContent = "EcoMetaStart\n"+jsonString+"\nEcoMetaEnd";

			string modifiedText = "";

			return modifiedText;
		}


	}
}
