using System;

namespace Mindbox.I18n
{
	public abstract class LocalizableString
	{
		public abstract string Key { get; }

		public static LocalizableString LocaleIndependent(string localeIndependentString)
		{
			return new LocaleIndependentString(localeIndependentString);
		}

		public override string ToString()
		{
			return ToStringCore();
		}

		public abstract string Render(LocalizationProvider localizationProvider, Locale locale);

		protected abstract string ToStringCore();

		public static implicit operator LocalizableString(string key)
		{
			// Strictly speaking, this is illegal and will result in ArgumentNullException later.
			if (key == null)
				return null;

			return new LocaleDependentString(key);
		}

		public static LocalizableString ForKey([LocalizationKey]string key)
		{
			return new LocaleDependentString(key);
		}

		private object context;

		public LocalizableString WithContext<TContext>(TContext context) where TContext : class
		{
			if (this.context != null)
				throw new InvalidOperationException($"Context is already set");

			this.context = context ?? throw new ArgumentNullException(nameof(context));

			return this;
		}

		public LocalizableString WithContext<TContext>(Func<TContext> contextGetter) where TContext : class
		{
			if (contextGetter == null)
				throw new ArgumentNullException(nameof(contextGetter));
			
			return WithContext(contextGetter());
		}
		
		public TContext GetContext<TContext>() where TContext : class
		{
			if (context == null)
				return null;

			var result = context as TContext;
			if (result == null)
				throw new InvalidOperationException(
					$"Context is not empty, but can't cast it's value of type {context.GetType()} to {typeof(TContext)}");

			return context as TContext;
		}
	}
}
