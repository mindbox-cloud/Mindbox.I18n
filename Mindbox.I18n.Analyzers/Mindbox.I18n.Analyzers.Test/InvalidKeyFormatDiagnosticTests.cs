using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test
{
	[TestClass]
    public class InvalidKeyFormatDiagnosticTests : MindboxI18nAnalyzerTests
	{
		[TestMethod]
		public void KeyMustHaveCorrectFormat_VariableAssignment_CorrectKeyFormat_NoDiagnostics()
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
		public void KeyMustHaveCorrectFormat_VariableAssignment_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s;
				s = ""Кириллическая строка"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 11, 9)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_VariableInitializer_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""Кириллическая строка"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_VariableAssignment_NoNamespace_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""KeyWithoutNamespace"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("KeyWithoutNamespace"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_VariableAssignment_UnwantedSpaces_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void TestMethod() 
			{
				LocalizableString s = ""Namespace : Key"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Namespace : Key"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 27)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_MethodArgument_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			void SetName(LocalizableString localizableString)
			{
			}

			void TestMethod() 
			{
				SetName(""Кириллическая строка"");
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 14, 13)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_FieldInitializer_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			private LocalizableString Name = ""Кириллическая строка"";
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 8, 37)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_PropertyInitializer_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			private LocalizableString Name { get; set; } = ""Кириллическая строка"";
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 8, 51)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_PropertyAssignment_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			private LocalizableString Name { get; set; }

			void TestMethod() 
			{
				Name = ""Кириллическая строка"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 12, 12)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_Constructor_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			public TestingClass(LocalizableString s)
			{
			}

			void TestMethod() 
			{
				return new TestingClass(""Кириллическая строка"");
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 14, 29)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[TestMethod]
		public void KeyMustHaveCorrectFormat_ReturnValue_CyryllicValue_Error()
		{
			var test = @"
	using Mindbox.I18n;

    namespace ConsoleApplication1
    {
		class TestingClass 
		{
			LocalizableString TestMethod() 
			{
				return ""Кириллическая строка"";
			}
		}
    }";
			var expected = new DiagnosticResult
			{
				Id = Diagnostics.KeyMustHaveCorrectFormat.Id,
				Message = BuildExpectedMessage("Кириллическая строка"),
				Severity = DiagnosticSeverity.Error,
				Locations =
					new[] {
						new DiagnosticResultLocation("Test0.cs", 10, 12)
					}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		protected override MindboxI18nAnalyzer CreateAnalyzer()
		{
			return new MindboxI18nAnalyzer();
		}

		private string BuildExpectedMessage(string key)
		{
			return string.Format(Diagnostics.KeyMustHaveCorrectFormat.MessageFormat.ToString(), key);
		}
	}
}
