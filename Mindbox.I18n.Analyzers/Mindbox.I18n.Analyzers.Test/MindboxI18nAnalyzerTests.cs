using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;

namespace Mindbox.I18n.Analyzers.Test
{
    public abstract class MindboxI18nAnalyzerTests : DiagnosticVerifier
    {
	    protected override IEnumerable<Assembly> GetAdditionalAssemblies()
	    {
		    yield return typeof(LocalizableString).Assembly;
	    }

	    protected sealed override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
	    {
		    return CreateAnalyzer();
	    }

	    protected abstract MindboxI18nAnalyzer CreateAnalyzer();
    }
}
