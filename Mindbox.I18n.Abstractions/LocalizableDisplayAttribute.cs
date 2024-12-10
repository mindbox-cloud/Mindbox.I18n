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

namespace Mindbox.I18n.Abstractions;

[AttributeUsage(AttributeTargets.Field)]
[Obsolete("Use Mindbox.I18n.Abstractions.LocalizableEnumMember instead")]
public sealed class LocalizableDisplayAttribute : Attribute
{
#pragma warning disable CA1019
	public LocalizableDisplayAttribute([LocalizationKey] string name)
	{
		LocalizableName = LocalizableString.ForKey(name);
	}

	public LocalizableDisplayAttribute([LocalizationKey] string name, [LocalizationKey] string description)
	{
		LocalizableName = LocalizableString.ForKey(name);
		LocalizableDescription = LocalizableString.ForKey(description);
	}
#pragma warning restore CA1019
	public LocalizableString LocalizableName { get; }
	public LocalizableString? LocalizableDescription { get; }
}