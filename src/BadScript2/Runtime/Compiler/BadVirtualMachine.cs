using BadScript2.Debugging;
using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Types;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Compiler;

public class BadVirtualMachine
{
    private readonly BadCompilerResult m_Result;
    private readonly Stack<BadObject> m_Stack = new Stack<BadObject>();
    private int m_ProgramCounter;


    public BadVirtualMachine(BadCompilerResult result, int entry = 0)
    {
        m_Result = result;
        m_ProgramCounter = entry;
    }

    private void PushScope(BadExecutionContext ctx)
    {
        m_Stack.Push(ctx.Scope);
    }

    private void Push(BadInstruction instr)
    {
        foreach (BadObject o in instr.Arguments)
        {
            m_Stack.Push(o);
        }
    }

    private void Pop()
    {
        m_Stack.Pop();
    }

    private void Load(BadInstruction instr)
    {
        BadObject target = m_Stack.Pop().Dereference();
        m_Stack.Push(target.GetProperty(instr.Arguments[0]));
    }

    private void Add(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadAddExpression.Add(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void Sub(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadSubtractExpression.Sub(left, right, instr.Position);


        m_Stack.Push(obj.Dereference());
    }

    private void Div(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadDivideExpression.Div(left, right, instr.Position);


        m_Stack.Push(obj.Dereference());
    }

    private void Mul(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadMultiplyExpression.Mul(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void Mod(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadModulusExpression.Mod(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void Not(BadInstruction instr)
    {
        m_Stack.Push(BadLogicNotExpression.Not(m_Stack.Pop().Dereference(), instr.Position).Dereference());
    }

    private void XOr(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadLogicXOrExpression.XOr(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void Equal(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadEqualityExpression.Equal(left, right);

        m_Stack.Push(obj.Dereference());
    }

    private void NotEqual(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadInequalityExpression.NotEqual(left, right);

        m_Stack.Push(obj.Dereference());
    }

    private void GreaterThan(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadGreaterThanExpression.GreaterThan(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void GreaterOrEqual(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadGreaterOrEqualExpression.GreaterOrEqual(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void LessThan(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadLessThanExpression.LessThan(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void LessOrEqual(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject left = m_Stack.Pop().Dereference();
        BadObject obj = BadLessOrEqualExpression.LessOrEqual(left, right, instr.Position);

        m_Stack.Push(obj.Dereference());
    }

    private void Return(BadExecutionContext ctx)
    {
        ctx.Scope.SetReturnValue(m_Stack.Pop());
    }

    private void Jump(BadInstruction instr)
    {
        if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
        {
            throw new BadRuntimeException("Jump instruction must have a number argument", instr.Position);
        }

        m_ProgramCounter = (int)num.Value;
    }

    private void JumpIfFalse(BadInstruction instr)
    {
        BadObject left = m_Stack.Pop().Dereference();
        if (left is not IBadBoolean lBool)
        {
            throw new BadRuntimeException("Expected boolean value", instr.Position);
        }

        if (!lBool.Value)
        {
            if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
            {
                throw new BadRuntimeException("Jump instruction must have a number argument", instr.Position);
            }

            m_ProgramCounter = (int)num.Value;
        }
    }


    private void Throw(BadExecutionContext ctx)
    {
        ctx.Scope.SetError(m_Stack.Pop(), null);
    }

    private void JumpIfNull(BadInstruction instr)
    {
        BadObject left = m_Stack.Pop().Dereference();
        if (left == BadObject.Null)
        {
            if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
            {
                throw new BadRuntimeException("Jump instruction must have a number argument", instr.Position);
            }

            m_ProgramCounter = (int)num.Value;
        }
    }

    private void JumpIfNotNull(BadInstruction instr)
    {
        BadObject left = m_Stack.Pop().Dereference();
        if (left != BadObject.Null)
        {
            if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
            {
                throw new BadRuntimeException("Jump instruction must have a number argument", instr.Position);
            }

            m_ProgramCounter = (int)num.Value;
        }
    }

    private void JumpIfTrue(BadInstruction instr)
    {
        BadObject left = m_Stack.Pop().Dereference();
        if (left is not IBadBoolean lBool)
        {
            throw new BadRuntimeException("Expected boolean value", instr.Position);
        }

        if (lBool.Value)
        {
            if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
            {
                throw new BadRuntimeException("Jump instruction must have a number argument", instr.Position);
            }

            m_ProgramCounter = (int)num.Value;
        }
    }

    private void Assign(BadInstruction instr)
    {
        BadObject right = m_Stack.Pop().Dereference();
        BadObject leftO = m_Stack.Pop();

        if (leftO is not BadObjectReference left)
        {
            throw new BadRuntimeException("Expected object reference", instr.Position);
        }

        left.Set(right);
    }

    private IEnumerable<BadObject> Call(BadExecutionContext ctx, BadObject obj, BadInstruction instr)
    {
        if (obj is not BadFunction func)
        {
            throw new BadRuntimeException("Expected function", instr.Position);
        }

        if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
        {
            throw new BadRuntimeException("Call instruction must have a number argument", instr.Position);
        }

        BadObject[] args = new BadObject[(int)num.Value];

        for (int i = args.Length - 1; i >= 0; i--)
        {
            args[i] = m_Stack.Pop().Dereference();
        }

        return func.Invoke(args, ctx);
    }

    private void CreateArray(BadInstruction instr)
    {
        if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
        {
            throw new BadRuntimeException("CreateArray instruction must have a number argument", instr.Position);
        }

        int size = (int)num.Value;
        List<BadObject> arr = new List<BadObject>(size);
        for (int i = size - 1; i >= 0; i--)
        {
            arr[i] = m_Stack.Pop().Dereference();
        }

        m_Stack.Push(new BadArray(arr));
    }

    private void CreateTable(BadInstruction instr)
    {
        if (instr.Arguments.Length == 0 || instr.Arguments[0] is not IBadNumber num)
        {
            throw new BadRuntimeException("CreateTable instruction must have a number argument", instr.Position);
        }

        Dictionary<BadObject, BadObject> table = new Dictionary<BadObject, BadObject>();
        int size = (int)num.Value;
        for (int i = size - 1; i >= 0; i--)
        {
            BadObject value = m_Stack.Pop().Dereference();
            BadObject key = m_Stack.Pop().Dereference();
            table.Add(key, value);
        }

        m_Stack.Push(new BadTable(table));
    }

    public IEnumerable<BadObject> Execute(BadExecutionContext ctx)
    {
        BadInstruction[] instrs = m_Result.GetInstructions();
        Stack<BadExecutionContext> ctxStack = new Stack<BadExecutionContext>();
        Stack<int> pcStack = new Stack<int>();

        BadExecutionContext GetContext()
        {
            return ctxStack.Count == 0 ? ctx : ctxStack.Peek();
        }

        while (m_ProgramCounter < instrs.Length && !ctx.Scope.IsError && ctx.Scope.ReturnValue == null)
        {
            BadInstruction i = instrs[m_ProgramCounter];
            
            if (BadDebugger.IsAttached)
            {
                BadDebugger.Step(new BadDebuggerStep(GetContext(), i.Position, i));
            }
            m_ProgramCounter++;
            switch (i.OpCode)
            {
                case BadOpCode.Nop:
                    break;
                case BadOpCode.Push:
                    Push(i);

                    break;
                case BadOpCode.PushScope:
                    PushScope(GetContext());

                    break;
                case BadOpCode.Load:
                    Load(i);

                    break;
                case BadOpCode.Pop:
                    Pop();

                    break;
                case BadOpCode.Add:
                    Add(i);

                    break;
                case BadOpCode.Return:

                    Return(GetContext());
                    if (pcStack.Count != 0)
                    {
                        BadObject? ret = GetContext().Scope.ReturnValue;
                        if (ret == null)
                        {
                            throw new BadRuntimeException("Internal Failure: Return value is null", i.Position);
                        }

                        m_Stack.Push(ret);
                        ctxStack.Pop();
                        m_ProgramCounter = pcStack.Pop();
                    }

                    break;
                case BadOpCode.Jump:
                    Jump(i);

                    break;
                case BadOpCode.Call:
                {
                    BadObject obj = BadObject.Null;
                    BadObject func = m_Stack.Pop().Dereference();
                    if (func is BadCompiledFunction cFunc &&
                        cFunc.CompilerResult == m_Result)
                    {
                        BadObject[] args = new BadObject[cFunc.Parameters.Length];
                        for (int argIndex = args.Length - 1; argIndex >= 0; argIndex--)
                        {
                            args[argIndex] = m_Stack.Pop().Dereference();
                        }

                        BadExecutionContext funcCtx = new BadExecutionContext(
                            cFunc.ParentScope.CreateChild(
                                cFunc.ToString(),
                                GetContext().Scope,
                                BadScopeFlags.Returnable | BadScopeFlags.AllowThrow | BadScopeFlags.CaptureThrow
                            )
                        );
                        ctxStack.Push(funcCtx);
                        cFunc.ApplyParameters(funcCtx, args, i.Position);
                        pcStack.Push(m_ProgramCounter);
                        m_ProgramCounter = cFunc.Entry;
                    }
                    else
                    {
                        foreach (BadObject o in Call(GetContext(), func, i))
                        {
                            obj = o;

                            yield return o;
                        }

                        m_Stack.Push(obj);
                    }

                    break;
                }
                case BadOpCode.JumpIfFalse:
                    JumpIfFalse(i);

                    break;
                case BadOpCode.JumpIfTrue:
                    JumpIfTrue(i);

                    break;

                case BadOpCode.JumpIfNull:
                    JumpIfNull(i);

                    break;
                case BadOpCode.JumpIfNotNull:
                    JumpIfNotNull(i);

                    break;
                case BadOpCode.Assign:
                    Assign(i);

                    break;
                case BadOpCode.DefineVar:
                {
                    GetContext().Scope.DefineVariable(i.Arguments[0], BadObject.Null);

                    break;
                }
                case BadOpCode.LessThan:
                    LessThan(i);

                    break;
                case BadOpCode.Sub:
                    Sub(i);

                    break;
                case BadOpCode.Mul:
                    Mul(i);

                    break;
                case BadOpCode.Div:
                    Div(i);

                    break;
                case BadOpCode.Mod:
                    Mod(i);

                    break;
                case BadOpCode.LessThanOrEqual:
                    LessOrEqual(i);

                    break;
                case BadOpCode.GreaterThan:
                    GreaterThan(i);

                    break;
                case BadOpCode.GreaterThanOrEqual:
                    GreaterOrEqual(i);

                    break;
                case BadOpCode.Equal:
                    Equal(i);

                    break;
                case BadOpCode.NotEqual:
                    NotEqual(i);

                    break;
                case BadOpCode.Not:
                    Not(i);

                    break;
                case BadOpCode.XOr:
                    XOr(i);

                    break;
                case BadOpCode.Throw:
                    Throw(GetContext());

                    break;
                case BadOpCode.ClearStack:
                    m_Stack.Clear();

                    break;

                case BadOpCode.CreateScope:
                {
                    if (i.Arguments.Length != 2)
                    {
                        throw new BadRuntimeException("CreateScope instruction must have two arguments", i.Position);
                    }

                    if (i.Arguments[0] is not IBadString name)
                    {
                        throw new BadRuntimeException(
                            "CreateScope instruction must have a string argument",
                            i.Position
                        );
                    }

                    if (i.Arguments[1] is not IBadNumber num)
                    {
                        throw new BadRuntimeException(
                            "CreateScope instruction must have a number argument",
                            i.Position
                        );
                    }

                    BadScopeFlags flags = (BadScopeFlags)num.Value;
                    ctxStack.Push(
                        new BadExecutionContext(GetContext().Scope.CreateChild(name.Value, GetContext().Scope, flags))
                    );

                    break;
                }
                case BadOpCode.DestroyScope:
                    ctxStack.Pop();

                    break;
                case BadOpCode.Dup:
                    m_Stack.Push(m_Stack.Peek());

                    break;
                case BadOpCode.Swap:
                    BadObject a = m_Stack.Pop();
                    BadObject b = m_Stack.Pop();
                    m_Stack.Push(a);
                    m_Stack.Push(b);

                    break;

                case BadOpCode.Dereference:
                    m_Stack.Push(m_Stack.Pop().Dereference());

                    break;

                case BadOpCode.CreateArray:
                    CreateArray(i);

                    break;
                case BadOpCode.CreateTable:
                    CreateTable(i);

                    break;

                case BadOpCode.NewObj:
                {
                    BadObject obj = m_Stack.Pop().Dereference();
                    if (obj is not BadClassPrototype proto)
                    {
                        throw new BadRuntimeException("Expected class prototype", i.Position);
                    }

                    if (i.Arguments.Length == 0 || i.Arguments[0] is not IBadNumber num)
                    {
                        throw new BadRuntimeException("NewObj instruction must have a number argument", i.Position);
                    }

                    BadObject[] args = new BadObject[(int)num.Value];
                    for (int argI = args.Length - 1; argI >= 0; argI--)
                    {
                        args[argI] = m_Stack.Pop().Dereference();
                    }

                    BadObject newObj = BadObject.Null;
                    foreach (BadObject o in BadNewExpression.CreateObject(proto, GetContext(), args, i.Position))
                    {
                        newObj = o;

                        yield return o;
                    }

                    m_Stack.Push(newObj);

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        yield return ctx.Scope.ReturnValue ?? BadObject.Null;
    }
}