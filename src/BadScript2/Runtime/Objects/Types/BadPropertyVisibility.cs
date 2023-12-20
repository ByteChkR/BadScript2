namespace BadScript2.Runtime.Objects.Types;

[Flags]
public enum BadPropertyVisibility
{
	Public = 1,
	Protected = 2,
	Private = 4,
	All = Public | Protected | Private,
}
