namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Контекст локализации в рамках запроса.
/// </summary>
public class RequestLocalizationContext
{
	/// <summary>
	/// Язык пользователя.
	/// </summary>
	public Locale? UserLocale { get; set; }
}