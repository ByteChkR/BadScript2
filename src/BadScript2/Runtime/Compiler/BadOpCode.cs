namespace BadScript2.Runtime.Compiler;

public enum BadOpCode
{
    Nop = 0x0000,
    Push = 0x0001,
    Pop = 0x0002,
    Load = 0x0003,
    PushScope = 0x0004,
    ClearStack = 0x0005,
    DefineVar = 0x0006,
    CreateScope = 0x0007,
    DestroyScope = 0x0008,
    Dup = 0x0009,
    Swap = 0x000A,
    Dereference = 0x000B,
    CreateTable = 0x000C,
    CreateArray = 0x000D,
    CreateClass = 0x000E,
    NewObj = 0x000F,

    Add = 0x0100,
    Sub = 0x0101,
    Mul = 0x0102,
    Div = 0x0103,
    Mod = 0x0104,

    Return = 0x0200,
    Throw = 0x0201,

    Jump = 0x0300,
    JumpIfFalse = 0x0301,
    JumpIfTrue = 0x0302,
    Call = 0x0303,
    JumpIfNull = 0x0304,
    JumpIfNotNull = 0x0305,

    Assign = 0x0400,

    LessThan = 0x0500,
    LessThanOrEqual = 0x0501,
    GreaterThan = 0x0502,
    GreaterThanOrEqual = 0x0503,
    Equal = 0x0504,
    NotEqual = 0x0505,

    Not = 0x0602,
    XOr = 0x0603,
}