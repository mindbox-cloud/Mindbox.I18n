# Mindbox.I18n.AspNetCore

Набор расширений, позволяющий встроить локализацию I18n в приложение ASP.NET Core.

## Как подключить

Ниже - минимальный набор изменений, нужных для встраивания в приложение ASP.NET Core.

### Startup.cs или Program.cs

Для подключения зависимостей в методе `ConfigureServices`:

```csharp
services
  .AddI18nRequestLocalization();
```

И в методе `Configure`:

```csharp
app.UseI18nRequestLocalization();
```

Место подключения важно, влияет на порядок middleware-ов.

## Использование в коде

После подключения, для доступа к локализации используйте абстракцию `ILocalizationContextAccessor`:
* `ILocalizationContextAccessor.Context.UserLocale` - язык пользователя

Для определения языка пользователя используются реализации `IRequestLocalizationProvider`.
- `AcceptLanguageHeaderLocalizationProvider` – получает язык из HTTP-заголовка Accept-Language.
- `TokenLocalizationProvider` – получает язык из токена пользователя (например, из JWT).
