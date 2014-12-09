using System;
// Totally inspired by the great work of Patrick Hogan with InControl
// https://github.com/pbhogan/InControl
// Only that I don't rely on the number of time you play the project, but on the number of compilation, which I think reflects more true internal changes of the code

using System.Text.RegularExpressions;
using UnityEngine;


namespace Net.FabreJean.UnityEditor
{

	public struct VersionInfo : IComparable<VersionInfo>
	{
		public enum VersionType {Alpha,Beta,ReleaseCandidate,Final};

		public int Major;
		public int Minor;
		public int Patch;
		public VersionType Type;
		public int Build;
		

		public VersionInfo( int major, int minor = 0, int patch = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = VersionType.Final;
			Build = 0;
		}

		public VersionInfo( int major, int minor = 0, int patch = 0, int build = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = VersionType.Final;
			Build = build;
		}

		public VersionInfo( int major, int minor = 0, int patch = 0, VersionType type = VersionType.Final , int build = 0 )
		{
			Major = major;
			Minor = minor;
			Patch = patch;
			Type  = type;
			Build = build;
		}

		public VersionInfo( string version )
		{
			if (string.IsNullOrEmpty(version))
			{
				Major = 0;
				Minor = 0;
				Patch=0;
				Type = VersionType.Final;
				Build=0;

			}else{
				var match = Regex.Match(version, @"^(\d+)\.(\d+)\.(\d+)" );	

				Major = Convert.ToInt32( match.Groups[1].Value );
				Minor = Convert.ToInt32( match.Groups[2].Value );
				Patch = Convert.ToInt32( match.Groups[3].Value );
				Type = VersionType.Final;
				Build = 0;
			}
		}

		public static VersionInfo UnityVersion()
		{
			var match = Regex.Match( Application.unityVersion, @"^(\d+)\.(\d+)\.(\d+)" );
			var build = 0;
			return new VersionInfo() {
				Major = Convert.ToInt32( match.Groups[1].Value ),
				Minor = Convert.ToInt32( match.Groups[2].Value ),
				Patch = Convert.ToInt32( match.Groups[3].Value ),
				Build = build
			};
		}
		
		/// <summary>
		/// I haven't digged into this but if I assign a struct from a variable to another does that create a shallow copy or it remains the same in memory?
		/// like myversion == someother version ? what happens there? 
		/// </summary>
		public VersionInfo Clone() { 
			return new VersionInfo(Major, Minor, Patch,Type,Build); 
		}

		public int CompareTo( VersionInfo other )
		{
			if (Major < other.Major) return -1;
			if (Major > other.Major) return +1;
			if (Minor < other.Minor) return -1;
			if (Minor > other.Minor) return +1;
			if (Patch < other.Patch) return -1;
			if (Patch > other.Patch) return +1;
			if (Build < other.Build) return -1;
			if (Build > other.Build) return +1;
			return 0;
		}
		
		
		public static bool operator ==( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) == 0;
		}
		
		
		public static bool operator !=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) != 0;
		}
		
		
		public static bool operator <=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) <= 0;
		}
		
		
		public static bool operator >=( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) >= 0;
		}
		
		
		public static bool operator <( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) < 0;
		}
		
		
		public static bool operator >( VersionInfo a, VersionInfo b )
		{
			return a.CompareTo( b ) > 0;
		}

		public static VersionType GetVersionTypeFromString(string type)
		{
			if (string.IsNullOrEmpty(type))
			{
				return VersionType.Final;
			}

			switch (type.ToLower())
			{
			case "a": case "alpha":
				return VersionType.Alpha;
			case "b": case"beta":
				return VersionType.Beta;
			case "rc": case"releasecandidate":
				return VersionType.ReleaseCandidate;
			case "f": case"final":
				return VersionType.Final;
			}

			return VersionType.Final;
		}

		public static string GetVersionTypeAsString(VersionType type)
		{
			if (type== VersionType.Alpha)
			{
				return "a";
			}
			if (type== VersionType.Beta)
			{
				return "b";
			}
			if (type== VersionType.ReleaseCandidate)
			{
				return "rc";
			}

			return "f";
		}

		public static string GetVersionTypeAsLongString(VersionType type)
		{
			if (type== VersionType.Alpha)
			{
				return "Alpha";
			}
			if (type== VersionType.Beta)
			{
				return "Beta";
			}
			if (type== VersionType.ReleaseCandidate)
			{
				return "Release Candidate";
			}

			return "Final";
		}

		
		public override string ToString()
		{
			if (Build == 0)
			{
				return string.Format( "{0}.{1}.{2}", Major, Minor, Patch );
			}
			return string.Format( "{0}.{1}.{2} {3} {4}", Major, Minor, Patch, GetVersionTypeAsString(Type), Build );
		}
		
		
		public string ToShortString()
		{
			if (Build == 0)
			{
				return string.Format( "{0}.{1}.{2}", Major, Minor, Patch );
			}
			return string.Format( "{0}.{1}.{2}{3}{4}", Major, Minor, Patch, GetVersionTypeAsString(Type), Build );
		}
		
		
		public override bool Equals( object other )
		{
			if (other is VersionInfo)
			{
				return this == ((VersionInfo) other);
			}
			return false;
		}
		
		
		public override int GetHashCode()
		{
			return Major.GetHashCode() ^ Minor.GetHashCode() ^ Patch.GetHashCode() ^ Build.GetHashCode();
		}



	}
}