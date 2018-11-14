using System.Runtime.Serialization;

namespace Mindbox.I18n.Analyzers
{
	[DataContract]
	public class AnalysisSettingsConfiguration
	{
		[DataMember(Name = "translationSource")]
		public TranslationSourceConfigurationSection TranslationSource { get;set; }
	}
}