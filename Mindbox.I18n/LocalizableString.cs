using System;

namespace Mindbox.I18n
{
	public abstract class LocalizableString
	{
		public static LocalizableString ForKey([LocalizationKey]string key)
		{
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
				return null;

			return new LocaleDependentString(key);
		}

		public abstract string Key { get; }

		public override string ToString()
		{
			return ToStringCore();
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

			var result = context as TContext;
			if (result == null)
				throw new InvalidOperationException(
					$"Context is not empty, but can't cast it's value of type {context.GetType()} to {typeof(TContext)}");

			return (TContext)context;
		}

		private object context;

		protected abstract string ToStringCore();
	}
}
