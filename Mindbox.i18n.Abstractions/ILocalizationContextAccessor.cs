namespace Mindbox.i18n.Abstractions;

#nullable enable

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