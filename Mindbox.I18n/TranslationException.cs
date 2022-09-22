// Copyright 2022 Mindbox Ltd
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Runtime.Serialization;

namespace Mindbox.I18n;

[Serializable]
public class TranslationException : Exception
{
	public static TranslationException MissingKey(string localeName, string @namespace, string key) =>
		new($"Key \"{key}\" was not found in namespace \"{@namespace}\" for locale \"{localeName}\".");

	public static TranslationException MissingNamespace(string localeName, string @namespace, string key) =>
		new($"Namespace \"{@namespace}\" was not found for key \"{key}\" for locale \"{localeName}\".");

	public TranslationException() { }
	public TranslationException(string message) : base(message) { }
	public TranslationException(string message, Exception inner) : base(message, inner) { }

	protected TranslationException(
		SerializationInfo info,
		StreamingContext context) : base(info, context)
	{
	}
}