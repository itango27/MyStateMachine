using HelperLib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyStateMachineLib
{
	public class SessionSnapshot
	{
		//public string UserID { get; set; } = "";
		//public string Email { get; set; } = "";
		//public string StateModelID { get; set; } = "";
		public string CurrentStateName { get; set; }
		public DateTime LastSaved { get; set; } = DateTime.UtcNow;
		//public List<string> ShoppingCart { get; set; } = new();

		public SessionSnapshot()
		{
		}

		private static string GetFilePath(string userID)
		{
			return Path.Combine(AssemblyData.DataPath, userID + ".json");
		}

		public static void Save(SessionSnapshot snapshot)
		{
			string filePath = GetFilePath("");
			string json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(filePath, json);
		}

		public static SessionSnapshot? Load(string userID)
		{
			string filePath = GetFilePath("");
			if (!File.Exists(filePath))
				return null;

			string json = File.ReadAllText(filePath);
			return JsonSerializer.Deserialize<SessionSnapshot>(json);
		}
	}
}
