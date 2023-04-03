using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Ugs
{
	public static class Config
	{
		public static bool Verbose = true;

		public static bool GamesEnabled = true;
		public static bool AppStateEnabled = true;
		
		public static bool LoadDataOnConnect = true;

		public static bool LazySignIn = true;
		public static bool LazySignInOnWrites = false;
	}
}
