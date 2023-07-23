using BadScript2.Debugging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.VirtualMachine;

public class BadRuntimeVirtualMachine
{
	private readonly Stack<BadObject> m_ArgumentStack = new Stack<BadObject>();
	private readonly Stack<BadRuntimeVirtualStackFrame> m_ContextStack = new Stack<BadRuntimeVirtualStackFrame>();

	private readonly BadInstruction[] m_Instructions;
	private readonly bool m_UseOverrides;
	private int m_InstructionPointer;

	public BadRuntimeVirtualMachine(BadInstruction[] instructions, bool useOverrides = true)
	{
		m_Instructions = instructions;
		m_UseOverrides = useOverrides;
	}

	private IEnumerable<BadObject> Execute()
	{
		while (m_InstructionPointer < m_Instructions.Length)
		{
			BadInstruction instr = m_Instructions[m_InstructionPointer];
			BadExecutionContext ctx = m_ContextStack.Peek().Context;

			if (BadDebugger.IsAttached)
			{
				BadDebugger.Step(new BadDebuggerStep(ctx, instr.Position, instr));
			}

			if (ctx.Scope.ReturnValue != null)
			{
				//Pop scopes until we find a scope that captures return
				while ((ctx.Scope.Flags & BadScopeFlags.CaptureReturn) == 0)
				{
					m_ContextStack.Pop();

					if (m_ContextStack.Count == 0)
					{
						yield break; //We exited the virtual machine. Quit the Execute method.
					}

					BadRuntimeVirtualStackFrame sf = m_ContextStack.Peek();
					ctx = sf.Context;

					//Set Return Pointer to the next instruction
					m_InstructionPointer = sf.ReturnPointer;
				}

				m_ContextStack.Pop();

				if (m_ContextStack.Count == 0)
				{
					yield break; //We exited the virtual machine. Quit the Execute method.
				}

				continue;
			}

			if (ctx.Scope.IsError)
			{
				m_ArgumentStack.Push(ctx.Scope.Error!);

				//Pop scopes until we find a scope that captures throw
				while ((ctx.Scope.Flags & BadScopeFlags.CaptureThrow) == 0)
				{
					m_ContextStack.Pop();

					if (m_ContextStack.Count == 0)
					{
						yield break; //We exited the virtual machine. Quit the Execute method.
					}
				}

				BadRuntimeVirtualStackFrame sframe = m_ContextStack.Peek();
				m_InstructionPointer = sframe.CreatePointer + sframe.ThrowPointer;
				m_ContextStack.Pop();

				continue;
			}

			if (ctx.Scope.IsBreak)
			{
				//Pop scopes until we find a scope that captures break
				while ((ctx.Scope.Flags & BadScopeFlags.CaptureBreak) == 0)
				{
					m_ContextStack.Pop();

					if (m_ContextStack.Count == 0)
					{
						throw BadRuntimeException.Create(ctx.Scope, "VIRTUAL MACHINE BREAK ERROR");
					}

					BadRuntimeVirtualStackFrame sf = m_ContextStack.Peek();
					ctx = sf.Context;

					//Set Return Pointer to the next instruction
					m_InstructionPointer = sf.CreatePointer + sf.BreakPointer;
				}

				m_ContextStack.Pop();

				continue;
			}

			if (ctx.Scope.IsContinue)
			{
				//Pop scopes until we find a scope that captures continue
				while ((ctx.Scope.Flags & BadScopeFlags.CaptureContinue) == 0)
				{
					m_ContextStack.Pop();

					if (m_ContextStack.Count == 0)
					{
						throw BadRuntimeException.Create(ctx.Scope, "VIRTUAL MACHINE CONTINUE ERROR");
					}

					BadRuntimeVirtualStackFrame sf = m_ContextStack.Peek();
					ctx = sf.Context;

					//Set Return Pointer to the next instruction
					m_InstructionPointer = sf.CreatePointer + sf.ContinuePointer;
				}

				m_ContextStack.Pop();

				continue;
			}

			//Console.WriteLine($"{m_InstructionPointer}\t: {instr}");
			m_InstructionPointer++;

			switch (instr.OpCode)
			{
				case BadOpCode.Nop:

					break;
				case BadOpCode.Dup:
					m_ArgumentStack.Push(m_ArgumentStack.Peek());

					break;
				case BadOpCode.Pop:
					m_ArgumentStack.Pop();

					break;
				case BadOpCode.AquireLock:
				{
					BadObject lockObj = m_ArgumentStack.Pop().Dereference();

					if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
					{
						throw new BadRuntimeException("Lock object must be of type Array, Object or Class",
							instr.Position);
					}

					while (!BadLockList.Instance.TryAquire(lockObj))
					{
						yield return BadObject.Null;
					}

					break;
				}
				case BadOpCode.ArrayInit:
				{
					int length = (int)instr.Arguments[0];
					List<BadObject> arr = new List<BadObject>();

					for (int i = 0; i < length; i++)
					{
						arr.Insert(0, m_ArgumentStack.Pop().Dereference());
					}

					m_ArgumentStack.Push(new BadArray(arr));

					break;
				}
				case BadOpCode.TableInit:
				{
					int length = (int)instr.Arguments[0];
					Dictionary<BadObject, BadObject> arr = new Dictionary<BadObject, BadObject>();

					for (int i = 0; i < length; i++)
					{
						BadObject val = m_ArgumentStack.Pop().Dereference();
						BadObject key = m_ArgumentStack.Pop().Dereference();
						arr.Add(key, val);
					}

					m_ArgumentStack.Push(new BadTable(arr));

					break;
				}
				case BadOpCode.HasProperty:
				{
					BadObject key = m_ArgumentStack.Pop().Dereference();
					BadObject obj = m_ArgumentStack.Pop().Dereference();
					BadObject? result = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadInExpression.InWithOverride(ctx, key, obj, instr.Position))
						{
							result = o;
						}
					}
					else
					{
						result = BadInExpression.In(key, obj);
					}

					m_ArgumentStack.Push(result);

					break;
				}
				case BadOpCode.Invoke:
				{
					BadObject func = m_ArgumentStack.Pop().Dereference();
					int argCount = (int)instr.Arguments[0];
					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop().Dereference();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadInvocationExpression.Invoke(func, args, instr.Position, ctx))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.New:
				{
					BadObject func = m_ArgumentStack.Pop().Dereference();

					if (func is not BadClassPrototype ptype)
					{
						throw new BadRuntimeException("Cannot create object from non-class type", instr.Position);
					}

					int argCount = (int)instr.Arguments[0];
					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop().Dereference();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadNewExpression.CreateObject(ptype, ctx, args, instr.Position))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.Range:
				{
					BadObject start = m_ArgumentStack.Pop().Dereference();
					BadObject end = m_ArgumentStack.Pop().Dereference();

					if (start is not IBadNumber sn || end is not IBadNumber en)
					{
						throw new BadRuntimeException("Range start and end must be numbers", instr.Position);
					}

					m_ArgumentStack.Push(
						new BadInteropEnumerator(BadRangeExpression.Range(sn.Value, en.Value).GetEnumerator()));

					break;
				}
				case BadOpCode.ReleaseLock:
				{
					BadObject lockObj = m_ArgumentStack.Pop().Dereference();

					if (lockObj is not BadArray && lockObj is not BadTable && lockObj is BadClass)
					{
						throw new BadRuntimeException("Lock object must be of type Array, Object or Class",
							instr.Position);
					}


					BadLockList.Instance.Release(lockObj);

					break;
				}
				case BadOpCode.DefVar:
				{
					BadObject name = (BadObject)instr.Arguments[0];
					bool isReadOnly = (bool)instr.Arguments[1];
					ctx.Scope.DefineVariable(name, BadObject.Null, ctx.Scope, new BadPropertyInfo(null, isReadOnly));
					m_ArgumentStack.Push(ctx.Scope.GetVariable(name));

					break;
				}
				case BadOpCode.DefVarTyped:

				{
					BadObject name = (BadObject)instr.Arguments[0];
					bool isReadOnly = (bool)instr.Arguments[1];
					ctx.Scope.DefineVariable(name,
						BadObject.Null,
						ctx.Scope,
						new BadPropertyInfo((BadClassPrototype)m_ArgumentStack.Pop().Dereference(), isReadOnly));
					m_ArgumentStack.Push(ctx.Scope.GetVariable(name));

					break;
				}
				case BadOpCode.LoadVar:
					m_ArgumentStack.Push(ctx.Scope.GetVariable((BadObject)instr.Arguments[0]));

					break;
				case BadOpCode.LoadMember:
					m_ArgumentStack.Push(m_ArgumentStack.Pop()
						.Dereference()
						.GetProperty((string)instr.Arguments[0], ctx.Scope));

					break;
				case BadOpCode.LoadMemberNullChecked:
				{
					BadObject obj = m_ArgumentStack.Pop().Dereference();
					BadObject name = (string)instr.Arguments[0];

					if (!obj.HasProperty(name))
					{
						m_ArgumentStack.Push(BadObject.Null);
					}
					else
					{
						m_ArgumentStack.Push(obj.GetProperty(name, ctx.Scope));
					}

					break;
				}
				case BadOpCode.LoadArrayAccess:
				{
					BadObject obj = m_ArgumentStack.Pop().Dereference();
					int argCount = (int)instr.Arguments[0];

					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadArrayAccessExpression.Access(ctx, obj, args, instr.Position))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.LoadArrayAccessNullChecked:
				{
					BadObject obj = m_ArgumentStack.Pop().Dereference();
					int argCount = (int)instr.Arguments[0];

					if (obj == BadObject.Null)
					{
						for (int i = 0; i < argCount; i++)
						{
							m_ArgumentStack.Pop();
						}

						m_ArgumentStack.Push(BadObject.Null);

						break;
					}


					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadArrayAccessExpression.Access(ctx, obj, args, instr.Position))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.LoadArrayAccessReverse:
				{
					BadObject obj = m_ArgumentStack.Pop().Dereference();
					int argCount = (int)instr.Arguments[0];

					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadArrayAccessReverseExpression.Access(ctx, obj, args, instr.Position))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.LoadArrayAccessReverseNullChecked:
				{
					BadObject obj = m_ArgumentStack.Pop().Dereference();

					if (obj == BadObject.Null)
					{
						m_ArgumentStack.Push(BadObject.Null);

						break;
					}

					int argCount = (int)instr.Arguments[0];

					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop();
					}

					BadObject r = BadObject.Null;

					foreach (BadObject o in BadArrayAccessReverseExpression.Access(ctx, obj, args, instr.Position))
					{
						r = o;

						yield return o;
					}

					m_ArgumentStack.Push(r);

					break;
				}
				case BadOpCode.Swap:
				{
					BadObject a = m_ArgumentStack.Pop();
					BadObject b = m_ArgumentStack.Pop();

					m_ArgumentStack.Push(a);
					m_ArgumentStack.Push(b);

					break;
				}
				case BadOpCode.Assign:
				{
					BadObject val = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					left.Set(val);

					break;
				}
				case BadOpCode.Push:
					m_ArgumentStack.Push((BadObject)instr.Arguments[0]);

					break;
				case BadOpCode.FormatString:
				{
					string format = (string)instr.Arguments[0];
					int argCount = (int)instr.Arguments[1];
					BadObject[] args = new BadObject[argCount];

					for (int i = argCount - 1; i >= 0; i--)
					{
						args[i] = m_ArgumentStack.Pop().Dereference();
					}

					m_ArgumentStack.Push(string.Format(format, args.Cast<object?>().ToArray()));

					break;
				}
				case BadOpCode.And:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					m_ArgumentStack.Push(BadLogicAndExpression.And(left, right, instr.Position));

					break;
				}
				case BadOpCode.Not:
				{
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject? o in BadLogicNotExpression.NotWithOverride(ctx, left, instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadLogicNotExpression.Not(left, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.XOr:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					m_ArgumentStack.Push(BadLogicXOrExpression.XOr(left, right, instr.Position));

					break;
				}
				case BadOpCode.AndAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					left.Set(BadLogicAndExpression.And(left.Dereference(), right, instr.Position));

					break;
				}
				case BadOpCode.XOrAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					left.Set(BadLogicXOrExpression.XOr(left.Dereference(), right, instr.Position));

					break;
				}
				case BadOpCode.Add:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadAddExpression.AddWithOverride(ctx, left, right, instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadAddExpression.Add(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.Sub:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadSubtractExpression.SubWithOverride(ctx, left, right, instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadSubtractExpression.Sub(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.Mul:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadMultiplyExpression.MulWithOverride(ctx, left, right, instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadMultiplyExpression.Mul(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.Exp:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadExponentiationExpression.ExpWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadExponentiationExpression.Exp(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.Div:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadDivideExpression.DivWithOverride(ctx, left, right, instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadDivideExpression.Div(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.Mod:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadModulusExpression.ModWithOverride(ctx, left, right, instr.Position))
						{
							obj = o;
						}
					}
					else
					{
						obj = BadModulusExpression.Mod(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj);

					break;
				}
				case BadOpCode.JumpRelative:
					m_InstructionPointer += (int)instr.Arguments[0];

					break;
				case BadOpCode.JumpRelativeIfFalse:
				{
					IBadBoolean val = (IBadBoolean)m_ArgumentStack.Pop().Dereference();

					if (!val.Value)
					{
						m_InstructionPointer += (int)instr.Arguments[0];
					}

					break;
				}
				case BadOpCode.JumpRelativeIfNull:
				{
					BadObject val = m_ArgumentStack.Pop().Dereference();

					if (val == BadObject.Null)
					{
						m_InstructionPointer += (int)instr.Arguments[0];
					}

					break;
				}
				case BadOpCode.JumpRelativeIfNotNull:
				{
					BadObject val = m_ArgumentStack.Pop().Dereference();

					if (val != BadObject.Null)
					{
						m_InstructionPointer += (int)instr.Arguments[0];
					}

					break;
				}
				case BadOpCode.JumpRelativeIfTrue:
				{
					IBadBoolean val = (IBadBoolean)m_ArgumentStack.Pop().Dereference();

					if (val.Value)
					{
						m_InstructionPointer += (int)instr.Arguments[0];
					}

					break;
				}
				case BadOpCode.CreateScope:
				{
					//0: name
					//1: useVisibility
					//3: flags
					//4: relative jump to break
					//5: relative jump to continue
					//6: relative jump to return
					//7: relative jump to throw
					string name = (string)instr.Arguments[0];
					bool? useVisibility = null;

					if (instr.Arguments[1] != BadObject.Null)
					{
						useVisibility = (bool)instr.Arguments[1];
					}

					BadScopeFlags flags = BadScopeFlags.AllowThrow;

					if (instr.Arguments.Length > 2)
					{
						flags = (BadScopeFlags)instr.Arguments[2];
					}

					BadRuntimeVirtualStackFrame sf = new BadRuntimeVirtualStackFrame(new BadExecutionContext(
						ctx.Scope.CreateChild(name,
							ctx.Scope,
							useVisibility,
							flags)));
					sf.CreatePointer = m_InstructionPointer;
					m_ContextStack.Push(sf);

					break;
				}
				case BadOpCode.DestroyScope:
					m_ContextStack.Pop();

					break;
				case BadOpCode.ClearStack:
					m_ArgumentStack.Clear();

					break;
				case BadOpCode.TypeOf:
				{
					m_ArgumentStack.Push(m_ArgumentStack.Pop().Dereference().GetPrototype());

					break;
				}
				case BadOpCode.InstanceOf:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();

					if (right is not BadClassPrototype type)
					{
						throw BadRuntimeException.Create(ctx.Scope,
							"Cannot check if an object is an instance of a non-class object.",
							instr.Position);
					}

					m_ArgumentStack.Push(type.IsSuperClassOf(left.GetPrototype()));

					break;
				}
				case BadOpCode.Delete:
				{
					BadObject? obj = m_ArgumentStack.Pop();

					if (obj is not BadObjectReference r)
					{
						throw BadRuntimeException.Create(ctx.Scope,
							"Cannot delete a non-reference object.",
							instr.Position);
					}

					r.Delete();

					break;
				}
				case BadOpCode.Equals:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadEqualityExpression.EqualWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadEqualityExpression.Equal(left, right);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.NotEquals:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadInequalityExpression.NotEqualWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadInequalityExpression.NotEqual(left, right);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.Greater:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadGreaterThanExpression.GreaterThanWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadGreaterThanExpression.GreaterThan(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.GreaterEquals:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadGreaterOrEqualExpression.GreaterOrEqualWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadGreaterOrEqualExpression.GreaterOrEqual(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.Less:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadLessThanExpression.LessThanWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadLessThanExpression.LessThan(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.LessEquals:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObject left = m_ArgumentStack.Pop().Dereference();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadLessOrEqualExpression.LessOrEqualWithOverride(ctx,
							         left,
							         right,
							         instr.Position))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadLessOrEqualExpression.LessOrEqual(left, right, instr.Position);
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.AddAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadAddAssignExpression.AddWithOverride(ctx,
							         left,
							         right,
							         instr.Position,
							         "+="))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadAddAssignExpression.Add(left, left.Dereference(), right, instr.Position, "+=");
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.SubAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadSubtractAssignExpression.SubtractWithOverride(ctx,
							         left,
							         right,
							         instr.Position,
							         "-="))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadSubtractAssignExpression.Subtract(left,
							left.Dereference(),
							right,
							instr.Position,
							"-=");
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.MulAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadMultiplyAssignExpression.MultiplyWithOverride(ctx,
							         left,
							         right,
							         instr.Position,
							         "*="))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadMultiplyAssignExpression.Multiply(left,
							left.Dereference(),
							right,
							instr.Position,
							"*=");
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.DivAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadDivideAssignExpression.DivideWithOverride(ctx,
							         left,
							         right,
							         instr.Position,
							         "/="))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadDivideAssignExpression.Divide(left, left.Dereference(), right, instr.Position, "/=");
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.ModAssign:
				{
					BadObject right = m_ArgumentStack.Pop().Dereference();
					BadObjectReference left = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject obj = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadModulusAssignExpression.ModulusWithOverride(ctx,
							         left,
							         right,
							         instr.Position,
							         "%="))
						{
							obj = o;

							yield return o;
						}
					}
					else
					{
						obj = BadModulusAssignExpression.Modulus(left, left.Dereference(), right, instr.Position, "%=");
					}

					m_ArgumentStack.Push(obj.Dereference());

					break;
				}
				case BadOpCode.PostInc:
				{
					BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject? result = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadPostIncrementExpression.IncrementWithOverride(ctx,
							         obj,
							         instr.Position))
						{
							result = o;
						}
					}
					else
					{
						result = BadPostIncrementExpression.Increment(obj, instr.Position);
					}

					m_ArgumentStack.Push(result);

					break;
				}
				case BadOpCode.PostDec:
				{
					BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject? result = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadPostDecrementExpression.DecrementWithOverride(ctx,
							         obj,
							         instr.Position))
						{
							result = o;
						}
					}
					else
					{
						result = BadPostDecrementExpression.Decrement(obj, instr.Position);
					}

					m_ArgumentStack.Push(result);

					break;
				}
				case BadOpCode.PreInc:
				{
					BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject? result = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadPreIncrementExpression.IncrementWithOverride(ctx,
							         obj,
							         instr.Position))
						{
							result = o;
						}
					}
					else
					{
						result = BadPreIncrementExpression.Increment(obj, instr.Position);
					}

					m_ArgumentStack.Push(result);

					break;
				}
				case BadOpCode.PreDec:
				{
					BadObjectReference obj = (BadObjectReference)m_ArgumentStack.Pop();
					BadObject? result = BadObject.Null;

					if (m_UseOverrides)
					{
						foreach (BadObject o in BadPreDecrementExpression.DecrementWithOverride(ctx,
							         obj,
							         instr.Position))
						{
							result = o;
						}
					}
					else
					{
						result = BadPreDecrementExpression.Decrement(obj, instr.Position);
					}

					m_ArgumentStack.Push(result);

					break;
				}
				case BadOpCode.Return:
				{
					BadObject ret = BadObject.Null;

					if (instr.Arguments.Length != 0)
					{
						ret = m_ArgumentStack.Pop();
						bool isRefReturn = (bool)instr.Arguments[0];

						if (!isRefReturn)
						{
							ret = ret.Dereference();
						}
					}

					ctx.Scope.SetReturnValue(ret);

					break;
				}
				case BadOpCode.Break:
					ctx.Scope.SetBreak();

					break;
				case BadOpCode.Continue:
					ctx.Scope.SetContinue();

					break;
				case BadOpCode.Throw:
					ctx.Scope.SetError(m_ArgumentStack.Pop(), null);

					break;
				case BadOpCode.SetBreakPointer:
					m_ContextStack.Peek().BreakPointer = (int)instr.Arguments[0];

					break;
				case BadOpCode.SetContinuePointer:
					m_ContextStack.Peek().ContinuePointer = (int)instr.Arguments[0];

					break;
				case BadOpCode.SetThrowPointer:
					m_ContextStack.Peek().ThrowPointer = (int)instr.Arguments[0];

					break;
				case BadOpCode.Eval:
				{
					BadExpression expr = (BadExpression)instr.Arguments[0];
					BadObject ret = BadObject.Null;

					foreach (BadObject o in ctx.Execute(expr))
					{
						ret = o;

						yield return o;
					}

					m_ArgumentStack.Push(ret);

					break;
				}
				default:
					throw new ArgumentOutOfRangeException();
			}

			//IF not control flow instruction and not an internal instruction
			//  => return argumentstack.peek()
			//yield return BadObject.Null; //make it return something
		}
	}

	public IEnumerable<BadObject> Execute(BadExecutionContext ctx)
	{
		m_ContextStack.Clear();
		m_ArgumentStack.Clear();
		m_InstructionPointer = 0;
		m_ContextStack.Push(new BadRuntimeVirtualStackFrame(ctx));

		return Execute();
	}
}
