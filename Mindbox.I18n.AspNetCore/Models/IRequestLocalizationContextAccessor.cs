namespace Mindbox.I18n.AspNetCore;

/// <summary>
/// Предоставляет доступ к текущему контексту локализации запроса.
/// </summary>
public interface IRequestLocalizationContextAccessor
{
	/// <summary>
	/// Текущий контекст. 
	/// </summary>
	RequestLocalizationContext? Context { get; set; }
}