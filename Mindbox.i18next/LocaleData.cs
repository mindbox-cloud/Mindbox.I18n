using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindbox.i18next
{
	internal class LocaleData
	{
		private readonly string directoryPath;
		private readonly Dictionary<string, JObject> namespaces = new Dictionary<string, JObject>();

		public LocaleData(string directoryPath)
		{
			this.directoryPath = directoryPath;
		}

		public string GetKeyValue(string @namespace, string key)
		{
			namespaces.TryGetValue(@namespace, out var data);
			if (data == null)
			{
				data = JObject.Load(new JsonTextReader(new StreamReader(Path.Combine(directoryPath, $"{@namespace}.json"))));
				namespaces[@namespace] = data;
			}

			return data[key].Value<string>();
		}
	}
}