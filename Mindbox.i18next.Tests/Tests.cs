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
				new InitOptions
				{
					Locale = "ru",
					LocalePath = "ru"
				});

			var result = i18next.Translate("test:testKey1", new Options());

			Assert.AreEqual("Это первый тест", result);
		}

		[TestMethod]
		public void HappyPathEn()
		{
			var i18next = new I18Next();
			i18next.Init(
				new InitOptions
				{
					Locale = "en",
					LocalePath = "en"
				});

			var result = i18next.Translate("test:testKey1", new Options());

			Assert.AreEqual("This is the first test", result);
		}
	}
}