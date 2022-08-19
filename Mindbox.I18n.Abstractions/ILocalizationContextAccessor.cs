namespace Mindbox.I18n.Abstractions;

/// <summary>
/// Предоставляет доступ к текущему контексту локализации запроса.
/// </summary>
public interface ILocalizationContextAccessor
{
	/// <summary>
	/// Текущий контекст.
	/// </summary>
	ILocalizationContext? Context { get; set; }
}