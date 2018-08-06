using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mindbox.i18next.Tests
{
	[TestClass]
	public class Tests
	{
		[TestMethod]
		public void HappyPathRu()
		{
			var i18next = new I18Next();
			i18next.Init(
				new Options
				{
					Language = "ru",
					LanguageDirectoryPath = "ru"
				});

			var result = i18next.Translate("test:testKey1", new TranslationOptions());

			Assert.AreEqual("Это первый тест", result);
		}

		[TestMethod]
		public void HappyPathEn()
		{
			var i18next = new I18Next();
			i18next.Init(
				new Options
				{
					Language = "en",
					LanguageDirectoryPath = "en"
				});

			var result = i18next.Translate("test:testKey1", new TranslationOptions());

			Assert.AreEqual("This is the first test", result);
		}
	}
}