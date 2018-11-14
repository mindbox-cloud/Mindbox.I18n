using System.Runtime.Serialization;

namespace Mindbox.I18n.Analyzers
{
	[DataContract]
	public class TranslationSourceConfigurationSection
	{
		[DataMember(Name = "baseDirectory")]
		public string BaseDirectory { get; set; }

		[DataMember(Name = "locale")]
		public string Locale { get; set; }

		[DataMember(Name = "ignorePaths")]
		public string[] IgnorePaths { get; set; }
	}
}