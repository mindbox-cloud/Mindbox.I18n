using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test
{
	[TestClass]
    public class TranslationDiagnosticTests : MindboxI18nAnalyzerTests
	{
		private TestTranslationSource translationSource;

		[TestInitialize]
	    public void TestInitialize()
	    {
			translationSource = new TestTranslationSource();

		    translationSource.AddTranslation(
			    "Marvin:HumanHate",
			    "Marvin was humming ironically because he hated humans so much.");
	    }

		[TestMethod]
		public void TranslationAnalysis_TranslationExists_Info()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			public LocalizableString Name => ""Marvin:HumanHate"";
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.TranslationHint.Id,
				Message = "\"Marvin was humming ironically because he hated humans so much.\"",
				Severity = DiagnosticSeverity.Info,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 8, 37)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void TranslationAnalysis_TranslationNotFound_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			public LocalizableString Name => ""Marvin:HumanLove"";
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.TranslationMustExistForLocalizationKey.Id,
				Message = "Translation not found for key \"Marvin:HumanLove\"",
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 8, 37)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		protected override MindboxI18nAnalyzer CreateAnalyzer()
	    {
		    return new MindboxI18nAnalyzer(translationSource);
	    }
    }
}
