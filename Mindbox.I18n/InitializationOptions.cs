using System.Collections.Generic;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

public class InitializationOptions
{
	public IReadOnlyList<string> SupportedLanguages { get; set; } = new List<string>();
	public ILogger Logger { get; set; } = null!;
	public string LocalizationDirectoryPath { get; set; } = null!;
	public ITranslationSource TranslationSource { get; set; } = null!;
}