namespace Mindbox.I18n.Tests;

[TestClass]
public class LocalizableStringTests
{
	public const string DefaultKey = "Prefix:key";

	[TestMethod]
	public void ForKey_NullValue_ThrowException()
	{
		Assert.ThrowsException<ArgumentNullException>(() => LocalizableString.ForKey(null!));
	}

	[TestMethod]
	public void ForKey_KeyAreEqual()
	{
		var localizableString = LocalizableString.ForKey(DefaultKey);

		Assert.IsNotNull(localizableString);
		Assert.AreEqual(DefaultKey, localizableString.Key);
	}

	[TestMethod]
	public void ForKey_WithParameters_ThrowException_WhenNull()
	{
		Assert.ThrowsException<ArgumentNullException>(() =>
			LocalizableString.ForKey(DefaultKey).WithParameters(null!));
	}

	[TestMethod]
	public void ForKey_WithParameters()
	{
		var firstParameter = 123;
		var secondParameter = DateTime.UtcNow;

		var localizableString = LocalizableString
			.ForKey(DefaultKey)
			.WithParameters(parameters => parameters
				.WithField("FirstValue", firstParameter)
				.WithField("SecondValue", secondParameter));

		Assert.IsNotNull(localizableString.LocalizationParameters);
		Assert.AreEqual(firstParameter, localizableString.LocalizationParameters.Fields["FirstValue"]);
		Assert.AreEqual(secondParameter, localizableString.LocalizationParameters.Fields["SecondValue"]);
	}
}