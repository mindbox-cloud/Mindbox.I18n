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

namespace Mindbox.I18n.Tests;

public class TemplateStub : ITemplate
{
	private readonly ICompositeModelDefinition _compositeModelDefinition;

	public TemplateStub(ICompositeModelDefinition compositeModelDefinition)
	{
		_compositeModelDefinition = compositeModelDefinition ?? throw new ArgumentNullException(nameof(compositeModelDefinition));
	}

	public ICompositeModelDefinition GetModelDefinition() => _compositeModelDefinition;

	public virtual string Render(
		ICompositeModelValue model,
		ICallContextContainer? callContextContainer = null)
	{
		throw new NotImplementedException();
	}

	public virtual void Render(
		TextWriter textWriter,
		ICompositeModelValue model,
		ICallContextContainer? callContextContainer = null)
	{
		throw new NotImplementedException();
	}

	public bool IsConstant => false;
	public IList<ITemplateError>? Errors => null!;
}