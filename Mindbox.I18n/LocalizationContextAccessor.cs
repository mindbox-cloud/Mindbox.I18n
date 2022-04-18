using System.Threading;

namespace Mindbox.I18n.AspNetCore;

#nullable enable

/// <inheritdoc />
public class LocalizationContextAccessor : ILocalizationContextAccessor
{
	private static readonly AsyncLocal<ContextHolder> _sessionContextCurrent = new();

	public LocalizationContext? Context
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
		public LocalizationContext? Context { get; set; }
	}
}