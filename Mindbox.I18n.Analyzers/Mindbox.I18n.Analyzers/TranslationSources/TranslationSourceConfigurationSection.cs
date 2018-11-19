using System.Runtime.Serialization;

namespace Mindbox.I18n.Analyzers
{
	[DataContract]
	public class TranslationSourceConfigurationSection
	{
		[DataMember(Name = "solutionFilePath")]
		public string SolutionFilePath { get; set; }

		[DataMember(Name = "locale")]
		public string Locale { get; set; }
	}
}