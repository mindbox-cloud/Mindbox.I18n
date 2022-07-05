namespace Mindbox.I18n;

#nullable enable

/// <summary>
/// Контекст локализации в рамках запроса.
/// </summary>
public class LocalizationContext
{
	/// <summary>
	/// Язык пользователя.
	/// </summary>
	public Locale? UserLocale { get; set; }
}