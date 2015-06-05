﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Net.FabreJean.UnityEditor
{
	public class EcosystemUtils {

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

	}
}
