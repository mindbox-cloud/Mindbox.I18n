using System.Text.Json.Serialization;

namespace Mindbox.I18n.Analyzers;
#nullable disable
public class TranslationSourceConfigurationSection
{
	[JsonPropertyName("solutionFilePath")]
	public string SolutionFilePath { get; set; }

	[JsonPropertyName("locale")]
	public string Locale { get; set; }
}