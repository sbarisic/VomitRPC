using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace VomitRPC {
	static class Utils {
		static JsonSerializerOptions Opts;

		static Utils() {
			Opts = new JsonSerializerOptions();
			Opts.WriteIndented = true;
		}

		public static string Serialize(object Val) {
			return JsonSerializer.Serialize(Val, options: Opts);
		}

		public static T Deserialize<T>(string Str) {
			return (T)JsonSerializer.Deserialize(Str, typeof(T), options: Opts);
		}

		public static object ChangeType(JsonElement Element, Type T) {
			if (T == typeof(int))
				return Element.GetInt32();
			else if (T == typeof(float))
				return Element.GetSingle();
			else if (T == typeof(string))
				return Element.GetString();
			else if (T == typeof(string[]))
				return Element.EnumerateArray().Select(E => (string)ChangeType(E, typeof(string))).ToArray();

			throw new NotImplementedException();
		}

		public static void ChangeTypes(ref object[] Arr, Type[] Types) {
			for (int i = 0; i < Arr.Length; i++) {
				Arr[i] = ChangeType((JsonElement)Arr[i], Types[i]);
			}
		}
	}
}
