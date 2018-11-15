using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test
{
	[TestClass]
    public class OnlyStringLiteralsCanBeUsedAsKeysTests : MindboxI18nAnalyzerTests
	{
		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_CorrectKey_NoDiagnostics()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""Namespace:Key_Key"";
			}
		}
    }";

			VerifyCSharpDiagnostic(test);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_StringInterpolation_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = $""{DateTime.Now}"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_StringFormat_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = string.Format(""{0}"", ""text"");
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_LocalStringVariable_Error()
		{
			var test = @"
	using Mindbox.I18n;
    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				string key = ""text"";
				LocalizableString s = key;
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_MethodArgumentIsLocalizableString_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod2(LocalizableString s) 
			{
				
			}

			void TestMethod() 
			{
				string key = ""text"";
				TestMethod2(key);
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 16, 17)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_StringMemberAccess_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = TestingClass.Key;
			}

			private const string Key = ""text"";
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 40)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_StringConditionalMemberAccess_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				(string, string)? tuple = (""a"", ""b"");
				LocalizableString s = tuple?.Item1;
			}

			private object object;
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 11, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_StringConcatenation_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""pupa"" + ""lupa"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.Id,
				Message = Diagnostics.OnlyStringLiteralsCanBeUsedAsKeys.MessageFormat.ToString(),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void OnlyStringLiteralsCanBeUsed_LocalizableStringAssignedToLocalizableString_NoDiagnostics()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{				
				LocalizableString s = ""Namespace:Key_Key"";
				LocalizableString s2 = s;
			}
		}
    }";

			VerifyCSharpDiagnostic(test);
		}

		protected override MindboxI18nAnalyzer CreateAnalyzer()
		{
			return new MindboxI18nAnalyzer();
		}
	}
}
