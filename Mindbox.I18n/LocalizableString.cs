using System;

namespace Mindbox.I18n
{
	public abstract class LocalizableString
	{
		protected static ILogger Logger { get; private set; } = new NullI18NextLogger();

		public static void InitializeLogger(ILogger logger)
		{
			Logger = logger ?? new NullI18NextLogger();
		}

		public static LocalizableString ForKey([LocalizationKey]string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}
			return new LocaleDependentString(key);
		}

		public static LocalizableString LocaleIndependent(string localeIndependentString)
		{
			return new LocaleIndependentString(localeIndependentString);
		}

		public static implicit operator LocalizableString(string key)
		{
			// Strictly speaking, this is illegal and will result in ArgumentNullException later.
			if (key == null)
			{
				Logger.LogInvalidOperation($"Attempting to implicitly cast nulll to LocalizableString");
				return null;
			}

			return new LocaleDependentString(key);
		}

		public abstract string Key { get; }

		public override string ToString()
		{
			Logger.LogInvalidOperation($"ToString() called on LocalizableString with key {Key}");
			return Key;
		}

		public abstract string Render(LocalizationProvider localizationProvider, Locale locale);

		public LocalizableString WithContext<TContext>(TContext context) where TContext : class
		{
			if (this.context != null)
				throw new InvalidOperationException($"Context has already been set");

			this.context = context ?? throw new ArgumentNullException(nameof(context));

			return this;
		}

		public TContext GetContext<TContext>() where TContext : class
		{
			if (context == null)
				return null;

			if (!(context is TContext result))
				throw new InvalidOperationException(
					$"Context is not empty, but can't cast it's value of type {context.GetType()} to {typeof(TContext)}");

			return (TContext)context;
		}

		private object context;
	}
}
