using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mindbox.I18n
{
	public class NullI18NextLogger : ILogger
	{
		public void LogMissingKey(string localeName, string @namespace, string key)
		{
			// empty
		}

		public void LogInvalidKey(string key)
		{
			// empty
		}
	}
}
