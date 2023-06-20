using System.Collections;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

using HtmlAgilityPack;

namespace BadHtml.Transformer;

/// <summary>
/// Implements the 'bs:each' node transformer
/// </summary>
public class BadForEachNodeTransformer : BadHtmlNodeTransformer
{
	public override bool CanTransform(BadHtmlContext context)
	{
		return context.InputNode.Name == "bs:each";
	}

	/// <summary>
	/// Runs an iteration of the 'bs:each' node
	/// </summary>
	/// <param name="context">The Html Context</param>
	/// <param name="name">The Variable name of the foreach block</param>
	/// <param name="obj">The Value of the current iteration</param>
	private void RunIteration(BadHtmlContext context, string name, BadObject obj)
	{
		BadExecutionContext loopContext = new BadExecutionContext(context.ExecutionContext.Scope.CreateChild("bs:while",
			context.ExecutionContext.Scope,
			null,
			BadScopeFlags.Breakable | BadScopeFlags.Continuable));
		loopContext.Scope.DefineVariable(name, obj, context.ExecutionContext.Scope);

		foreach (HtmlNode? child in context.InputNode.ChildNodes)
		{
			BadHtmlContext childContext = context.CreateChild(child, context.OutputNode, loopContext);

			Transform(childContext);
		}
	}

	public override void TransformNode(BadHtmlContext context)
	{
		HtmlAttribute? enumerationAttribute = context.InputNode.Attributes["on"];
		HtmlAttribute? itemAttribute = context.InputNode.Attributes["as"];

		if (enumerationAttribute == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Missing 'on' attribute in 'bs:each' node",
				context.CreateOuterPosition());
		}

		if (itemAttribute == null)
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Missing 'as' attribute in 'bs:each' node",
				context.CreateOuterPosition());
		}

		if (string.IsNullOrEmpty(enumerationAttribute.Value))
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Empty 'on' attribute in 'bs:each' node",
				context.CreateAttributePosition(enumerationAttribute));
		}

		if (string.IsNullOrEmpty(itemAttribute.Value))
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Empty 'as' attribute in 'bs:each' node",
				context.CreateAttributePosition(itemAttribute));
		}

		BadObject enumeration = context.ParseAndExecute(enumerationAttribute.Value,
			context.CreateAttributePosition(enumerationAttribute));

		if (enumeration is IBadEnumerable badEnumerable)
		{
			foreach (BadObject o in badEnumerable)
			{
				RunIteration(context, itemAttribute.Value, o);
			}
		}
		else if (enumeration is IBadEnumerator badEnumerator)
		{
			while (badEnumerator.MoveNext())
			{
				RunIteration(context,
					itemAttribute.Value,
					badEnumerator.Current ??
					throw BadRuntimeException.Create(context.ExecutionContext.Scope,
						"Enumerator returned null",
						context.CreateAttributePosition(enumerationAttribute)));
			}
		}
		else if (enumeration is IEnumerable enumerable)
		{
			foreach (object? o in enumerable)
			{
				RunIteration(context, itemAttribute.Value, BadObject.Wrap(o));
			}
		}
		else if (enumeration is IEnumerator enumerator)
		{
			while (enumerator.MoveNext())
			{
				RunIteration(context,
					itemAttribute.Value,
					BadObject.Wrap(enumerator.Current));
			}
		}
		else
		{
			throw BadRuntimeException.Create(context.ExecutionContext.Scope,
				"Result of 'on' attribute in 'bs:each' node is not enumerable",
				context.CreateAttributePosition(enumerationAttribute));
		}
	}
}
