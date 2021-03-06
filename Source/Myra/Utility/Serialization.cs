﻿using Newtonsoft.Json.Linq;

namespace Myra.Utility
{
	public static class Serialization
	{
		public static bool GetStyle(this JObject styles, string name, out string result)
		{
			result = null;

			JToken obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = obj.ToString();

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out JObject result)
		{
			result = null;

			JToken obj;
			if (!styles.TryGetValue(name, out obj))
			{
				return false;
			}

			result = obj.ToObject<JObject>();

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out int result)
		{
			result = 0;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			int i;
			if (!int.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}
		
		public static bool GetStyle(this JObject styles, string name, out bool result)
		{
			result = false;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			bool i;
			if (!bool.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}
				
		public static bool GetStyle(this JObject styles, string name, out char result)
		{
			result = '\0';

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			char i;
			if (!char.TryParse(s, out i))
			{
				return false;
			}

			result = i;

			return true;
		}

		public static bool GetStyle(this JObject styles, string name, out float result)
		{
			result = 0.0f;

			string s;
			if (!GetStyle(styles, name, out s))
			{
				return false;
			}

			float f;
			if (!float.TryParse(s, out f))
			{
				return false;
			}

			result = f;

			return true;
		}
	}
}