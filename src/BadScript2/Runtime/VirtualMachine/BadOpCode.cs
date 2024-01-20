namespace BadScript2.Runtime.VirtualMachine;

/// <summary>
///     Defines the Operations that the BadVirtualMachine can execute.
/// </summary>
public enum BadOpCode
{
    Nop = 0,
    DefVar = 0x0001,
    DefVarTyped = 0x0002,
    LoadVar = 0x0003,
    LoadMember = 0x0004,
    LoadMemberNullChecked = 0x0005,
    Assign = 0x0006,
    Push = 0x0007,
    FormatString = 0x0008,
    ClearStack = 0x0009,
    LoadArrayAccess = 0x000A,
    LoadArrayAccessNullChecked = 0x000B,
    LoadArrayAccessReverse = 0x000C,
    LoadArrayAccessReverseNullChecked = 0x000D,
    Dup = 0x000E,
    ArrayInit = 0x000F,
    TableInit = 0x0010,
    HasProperty = 0x0011,
    Swap = 0x0012,
    Pop = 0x0013,
    TypeOf = 0x0014,
    Delete = 0x0015,
    InstanceOf = 0x0016,
    Equals = 0x1000,
    NotEquals = 0x1001,
    Greater = 0x1002,
    GreaterEquals = 0x1003,
    Less = 0x1004,
    LessEquals = 0x1005,
    And = 0x2000,

    //Or = 0x2001, //Short circuiting with JumpRelativeIfTrue
    Not = 0x2002,
    XOr = 0x2003,
    AndAssign = 0x2004,

    //OrAssign = 0x2005, //Short circuiting with JumpRelativeIfTrue
    XOrAssign = 0x2006,
    Add = 0x3000,
    Sub = 0x3001,
    Mul = 0x3002,
    Div = 0x3003,
    Mod = 0x3004,
    AddAssign = 0x3005,
    SubAssign = 0x3006,
    MulAssign = 0x3007,
    DivAssign = 0x3008,
    ModAssign = 0x3009,
    PostInc = 0x3010,
    PostDec = 0x3011,
    PreInc = 0x3012,
    PreDec = 0x3013,
    Exp = 0x3014,
    Neg = 0x3015,
    ExpAssign = 0x3016,
    JumpRelative = 0x4000,
    JumpRelativeIfFalse = 0x4001,
    JumpRelativeIfTrue = 0x4002,
    JumpRelativeIfNotNull = 0x4003,
    JumpRelativeIfNull = 0x4004,
    Invoke = 0x4005,
    New = 0x4006,
    Range = 0x4007,
    CreateScope = 0x5000,
    DestroyScope = 0x5001,
    Return = 0x6000,
    Break = 0x6001,
    Continue = 0x6002,
    Throw = 0x6003,
    SetBreakPointer = 0x6004,
    SetContinuePointer = 0x6005,
    SetThrowPointer = 0x6006,
    AquireLock = 0x7000,
    ReleaseLock = 0x7001,

    Eval = 0x8000,

    BinaryUnpack = 0x9000,
    UnaryUnpack = 0x9001,
}