using System.Collections.Generic;

namespace Mindbox.I18n;

public class InitializationOptions
{
	public IReadOnlyList<string> SupportedLanguages { get; set; } = new List<string>();
	public ILogger Logger { get; set; } = null;
	public string LocalizationDirectoryPath { get; set; }
	public ITranslationSource TranslationSource { get; set; }
}