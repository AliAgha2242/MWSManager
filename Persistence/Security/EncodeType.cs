namespace MWSMan.Core.Persistence.Security;


[Flags]
public enum EncodeType : byte
{
    Min = 0,
    KeyAtStart = 1,
    RotateRight = 2,
    FirstRotate = 4,
    Max = 8,
}
