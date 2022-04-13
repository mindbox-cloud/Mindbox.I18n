namespace Mindbox.I18n.AspNetCore;

/// <inheritdoc />
public class RequestLocalizationContextAccessor : IRequestLocalizationContextAccessor
{
	private static readonly AsyncLocal<ContextHolder> _sessionContextCurrent = new();

	public RequestLocalizationContext? Context
	{
		get
		{
			return _sessionContextCurrent.Value?.Context;
		}
		set
		{
			var holder = _sessionContextCurrent.Value;
			if (holder != null)
			{
				// Clear current context trapped in the AsyncLocals, as its done.
				holder.Context = null;
			}

			if (value != null)
			{
				// Use an object indirection to hold the SessionContext in the AsyncLocal,
				// so it can be cleared in all ExecutionContexts when its cleared.
				_sessionContextCurrent.Value = new ContextHolder { Context = value };
			}
		}
	}

	private class ContextHolder
	{
		public RequestLocalizationContext? Context { get; set; }
	}
}