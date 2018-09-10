using System;
using System.Collections.Generic;
using System.Text;

namespace Mindbox.I18n.Analyzers
{
	internal interface IAnalyzerTranslationSource
	{
		string TryGetTranslation(LocalizationKey key);
	}
}
