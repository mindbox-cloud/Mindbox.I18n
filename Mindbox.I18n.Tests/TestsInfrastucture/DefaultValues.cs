namespace Mindbox.I18n.Tests;

public static class DefaultValues
{
	public const string SimpleLocalizableStringKey = "Test:SimpleLocalizableStringKey";
	public const string SimpleLocalizableStringValue = "Test message";
	public const string ParameterizedLocalizableStringKey = "Test:ParameterizedLocalizableStringKey";
	public const string ParameterizedLocalizableStringValue = "Test with message: ${firstParam} and ${secondParam}";

	public static IReadOnlyDictionary<string, IModelDefinition> CompositeModelDefinitionFields =>
		new Dictionary<string, IModelDefinition>()
		{
			["firstParam"] = Mock.Of<IModelDefinition>(),
			["secondParam"] = Mock.Of<IModelDefinition>()
		};
}