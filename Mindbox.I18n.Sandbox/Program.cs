namespace Mindbox.I18n.Sandbox;

internal class Program
{
	private static void Main(string[] _1)
	{
		// Incorrect key
		_ = "Abacaba";

		// Correct key, should be found
		_ = "Cars:Bntl";

		// Correct key, should be found
		_ = "Bands:DeathCab";

		// Correct key but doesn't have a translation
		_ = "Bands:SomeUnknownGuys";

		// Correct key but doesn't have a translation
		_ = "Bands:UnknownAtTheBeginning";

		// Should be an error because the translation file is ignored by the configuration
		_ = "Drinks:Coke";
		_ = $"Try {"that"}";
	}
}