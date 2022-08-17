using System.Text.Json.Serialization;

namespace Mindbox.I18n.Analyzers;

public class TranslationSourceConfigurationSection
{
	[JsonPropertyName("solutionFilePath")]
	public string SolutionFilePath { get; set; }

	[JsonPropertyName("locale")]
	public string Locale { get; set; }
}