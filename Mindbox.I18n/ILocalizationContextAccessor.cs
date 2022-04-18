namespace Mindbox.I18n;

#nullable enable

/// <summary>
/// Предоставляет доступ к текущему контексту локализации запроса.
/// </summary>
public interface ILocalizationContextAccessor
{
	/// <summary>
	/// Текущий контекст. 
	/// </summary>
	LocalizationContext? Context { get; set; }
}