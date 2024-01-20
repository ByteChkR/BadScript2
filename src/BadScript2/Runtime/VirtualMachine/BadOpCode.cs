namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Defines the Operations that the BadVirtualMachine can execute.
/// </summary>
public enum BadOpCode
{
    /// <summary>
    /// No Operation.
    /// </summary>
    Nop = 0,
    /// <summary>
    /// Define a Variable.
    /// </summary>
    DefVar = 0x0001,
    /// <summary>
    /// Define a Typed Variable.
    /// </summary>
    DefVarTyped = 0x0002,
    /// <summary>
    /// Load a Variable.
    /// </summary>
    LoadVar = 0x0003,
    /// <summary>
    /// Load a Member of an Object.
    /// </summary>
    LoadMember = 0x0004,
    /// <summary>
    /// Load a Member of an Object and check if it is null.
    /// </summary>
    LoadMemberNullChecked = 0x0005,
    /// <summary>
    /// Assign to a Reference.
    /// </summary>
    Assign = 0x0006,
    /// <summary>
    /// Push a Value onto the Stack.
    /// </summary>
    Push = 0x0007,
    /// <summary>
    /// Format a String.
    /// </summary>
    FormatString = 0x0008,
    /// <summary>
    /// Clear the Stack.
    /// </summary>
    ClearStack = 0x0009,
    /// <summary>
    /// Load an Array Access.
    /// </summary>
    LoadArrayAccess = 0x000A,
    /// <summary>
    /// Load an Array Access and check if it is null.
    /// </summary>
    LoadArrayAccessNullChecked = 0x000B,
    /// <summary>
    /// Load an Array Access in reverse order.
    /// </summary>
    LoadArrayAccessReverse = 0x000C,
    /// <summary>
    /// Load an Array Access in reverse order and check if it is null.
    /// </summary>
    LoadArrayAccessReverseNullChecked = 0x000D,
    /// <summary>
    /// Duplicate the top of the Stack.
    /// </summary>
    Dup = 0x000E,
    /// <summary>
    /// Initialize an Array.
    /// </summary>
    ArrayInit = 0x000F,
    /// <summary>
    /// Initialize a Table.
    /// </summary>
    TableInit = 0x0010,
    /// <summary>
    /// Check if an Object has a Property.
    /// </summary>
    HasProperty = 0x0011,
    /// <summary>
    /// Swap the top two Stack Values.
    /// </summary>
    Swap = 0x0012,
    /// <summary>
    /// Pop the top of the Stack.
    /// </summary>
    Pop = 0x0013,
    /// <summary>
    /// Get the Type of an Object.
    /// </summary>
    TypeOf = 0x0014,
    /// <summary>
    /// Delete a Property.
    /// </summary>
    Delete = 0x0015,
    /// <summary>
    /// Get the Type of an Object and check if it is an Instance of another Type.
    /// </summary>
    InstanceOf = 0x0016,
    /// <summary>
    /// Compare two Objects.
    /// </summary>
    Equals = 0x1000,
    /// <summary>
    /// Compare two Objects for Inequality.
    /// </summary>
    NotEquals = 0x1001,
    /// <summary>
    /// Compare two Objects for Greater Than.
    /// </summary>
    Greater = 0x1002,
    /// <summary>
    /// Compare two Objects for Greater Than or Equal.
    /// </summary>
    GreaterEquals = 0x1003,
    /// <summary>
    /// Compare two Objects for Less Than.
    /// </summary>
    Less = 0x1004,
    /// <summary>
    /// Compare two Objects for Less Than or Equal.
    /// </summary>
    LessEquals = 0x1005,
    /// <summary>
    /// Logical And two Booleans.
    /// </summary>
    And = 0x2000,

    //Or = 0x2001, //Short circuiting with JumpRelativeIfTrue
    /// <summary>
    /// Logical Not a Boolean.
    /// </summary>
    Not = 0x2002,
    /// <summary>
    /// Logical XOr two Booleans.
    /// </summary>
    XOr = 0x2003,
    /// <summary>
    /// Logical And two Booleans and assign the result to the first.
    /// </summary>
    AndAssign = 0x2004,

    //OrAssign = 0x2005, //Short circuiting with JumpRelativeIfTrue
    /// <summary>
    /// Logical XOr two Booleans and assign the result to the first.
    /// </summary>
    XOrAssign = 0x2006,
    /// <summary>
    /// Add two Objects.
    /// </summary>
    Add = 0x3000,
    /// <summary>
    /// Subtract two Objects.
    /// </summary>
    Sub = 0x3001,
    /// <summary>
    /// Multiply two Objects.
    /// </summary>
    Mul = 0x3002,
    /// <summary>
    /// Divide two Objects.
    /// </summary>
    Div = 0x3003,
    /// <summary>
    /// Modulo two Objects.
    /// </summary>
    Mod = 0x3004,
    /// <summary>
    /// Assign the result of an Addition to the first Object.
    /// </summary>
    AddAssign = 0x3005,
    /// <summary>
    /// Assign the result of a Subtraction to the first Object.
    /// </summary>
    SubAssign = 0x3006,
    /// <summary>
    /// Assign the result of a Multiplication to the first Object.
    /// </summary>
    MulAssign = 0x3007,
    /// <summary>
    /// Assign the result of a Division to the first Object.
    /// </summary>
    DivAssign = 0x3008,
    /// <summary>
    /// Assign the result of a Modulo to the first Object.
    /// </summary>
    ModAssign = 0x3009,
    /// <summary>
    /// Postfix Increment.
    /// </summary>
    PostInc = 0x3010,
    /// <summary>
    /// Postfix Decrement.
    /// </summary>
    PostDec = 0x3011,
    /// <summary>
    /// Prefix Increment.
    /// </summary>
    PreInc = 0x3012,
    /// <summary>
    /// Prefix Decrement.
    /// </summary>
    PreDec = 0x3013,
    /// <summary>
    /// Exponentiate two Objects.
    /// </summary>
    Exp = 0x3014,
    /// <summary>
    /// Negate an Object.
    /// </summary>
    Neg = 0x3015,
    /// <summary>
    /// Assign the result of an Exponentiation to the first Object.
    /// </summary>
    ExpAssign = 0x3016,
    /// <summary>
    /// Jump to a specific Instruction addressed by a relative offset.
    /// </summary>
    JumpRelative = 0x4000,
    /// <summary>
    /// Jump to a specific Instruction addressed by a relative offset if the top of the Stack is false.
    /// </summary>
    JumpRelativeIfFalse = 0x4001,
    /// <summary>
    /// Jump to a specific Instruction addressed by a relative offset if the top of the Stack is true.
    /// </summary>
    JumpRelativeIfTrue = 0x4002,
    /// <summary>
    /// Jump to a specific Instruction addressed by a relative offset if the top of the Stack is not null.
    /// </summary>
    JumpRelativeIfNotNull = 0x4003,
    /// <summary>
    /// Jump to a specific Instruction addressed by a relative offset if the top of the Stack is null.
    /// </summary>
    JumpRelativeIfNull = 0x4004,
    /// <summary>
    /// Invoke a Function.
    /// </summary>
    Invoke = 0x4005,
    /// <summary>
    /// Create a new Object.
    /// </summary>
    New = 0x4006,
    /// <summary>
    /// Create a new Range Enumerable
    /// </summary>
    Range = 0x4007,
    /// <summary>
    /// Create a new Scope.
    /// </summary>
    CreateScope = 0x5000,
    /// <summary>
    /// Destroy the current scope.
    /// </summary>
    DestroyScope = 0x5001,
    /// <summary>
    /// Return from a Function.
    /// </summary>
    Return = 0x6000,
    /// <summary>
    /// Break from a Loop.
    /// </summary>
    Break = 0x6001,
    /// <summary>
    /// Continue a Loop.
    /// </summary>
    Continue = 0x6002,
    /// <summary>
    /// Throw an Exception.
    /// </summary>
    Throw = 0x6003,
    /// <summary>
    /// Set the Break instruction pointer.
    /// </summary>
    SetBreakPointer = 0x6004,
    /// <summary>
    /// Set the Continue instruction pointer.
    /// </summary>
    SetContinuePointer = 0x6005,
    /// <summary>
    /// Set the Throw instruction pointer.
    /// </summary>
    SetThrowPointer = 0x6006,
    /// <summary>
    /// Aquire a Lock.
    /// </summary>
    AquireLock = 0x7000,
    /// <summary>
    /// Release a Lock.
    /// </summary>
    ReleaseLock = 0x7001,

    /// <summary>
    /// Evaluate an Expression.(used if there is no Expression Compiler for the Expression)
    /// </summary>
    Eval = 0x8000,

    /// <summary>
    /// Unpack the right side of a Binary Operation into the left side.
    /// </summary>
    BinaryUnpack = 0x9000,
    /// <summary>
    /// Unpack the right side of a Unary Operation into the current scope
    /// </summary>
    UnaryUnpack = 0x9001,
}