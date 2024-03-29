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

using System.Threading;
using Mindbox.I18n.Abstractions;

namespace Mindbox.I18n;

/// <inheritdoc />
public class LocalizationContextAccessor : ILocalizationContextAccessor
{
	private static readonly AsyncLocal<ContextHolder> _sessionContextCurrent = new();

	public ILocalizationContext? Context
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
		public ILocalizationContext? Context { get; set; }
	}
}