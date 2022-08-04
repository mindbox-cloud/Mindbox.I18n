using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

#nullable enable

/// <summary>
/// Контекст локализации в рамках запроса.
/// </summary>
public class LocalizationContext : ILocalizationContext
{
	/// <summary>
	/// Язык пользователя.
	/// </summary>
	public ILocale? UserLocale { get; set; }
}