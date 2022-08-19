using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Mindbox.I18n.Analyzers;
#nullable disable
[DataContract]
public class AnalysisSettingsConfiguration
{
	[JsonPropertyName("translationSource")]
	public TranslationSourceConfigurationSection TranslationSource { get; set; }
}