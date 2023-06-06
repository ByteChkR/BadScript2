using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
/// Implements the "Math" API
/// </summary>
public class BadMathApi : BadInteropApi
{
	/// <summary>
	/// Random Number Generator
	/// </summary>
	private static readonly Random s_Random = new Random();

	/// <summary>
	/// Constructs a new Math API Instance
	/// </summary>
	public BadMathApi() : base("Math") { }

	protected override void LoadApi(BadTable target)
	{
		target.SetProperty("PI", (decimal)Math.PI);
		target.SetProperty("E", (decimal)Math.E);
		target.SetProperty("Tau", (decimal)Math.PI * 2);
		target.SetFunction<decimal>("Abs", x => (decimal)Math.Abs((double)x));
		target.SetFunction<decimal>("Asin", x => (decimal)Math.Asin((double)x));
		target.SetFunction<decimal>("Acos", x => (decimal)Math.Acos((double)x));
		target.SetFunction<decimal>("Atan", x => (decimal)Math.Atan((double)x));
		target.SetFunction<decimal, decimal>("Atan2", (x, y) => (decimal)Math.Atan2((double)x, (double)y));
		target.SetFunction<decimal>("Ceiling", x => (decimal)Math.Ceiling((double)x));
		target.SetFunction<decimal>("Cos", x => (decimal)Math.Cos((double)x));
		target.SetFunction<decimal>("Cosh", x => (decimal)Math.Cosh((double)x));
		target.SetFunction<decimal>("Exp", x => (decimal)Math.Exp((double)x));
		target.SetFunction<decimal>("Floor", x => (decimal)Math.Floor((double)x));
		target.SetFunction<decimal>("Log", x => (decimal)Math.Log((double)x));
		target.SetFunction<decimal>("Log10", x => (decimal)Math.Log10((double)x));
		target.SetFunction<decimal, decimal>("Max", (x, y) => (decimal)Math.Max((double)x, (double)y));
		target.SetFunction<decimal, decimal>("Min", (x, y) => (decimal)Math.Min((double)x, (double)y));
		target.SetFunction<decimal, decimal>("Pow", (x, y) => (decimal)Math.Pow((double)x, (double)y));
		target.SetFunction<decimal, decimal>("Round", (x, y) => (decimal)Math.Round((double)x, (int)y));
		target.SetFunction<decimal>("Sign", x => (decimal)Math.Sign((double)x));
		target.SetFunction<decimal>("Sin", x => (decimal)Math.Sin((double)x));
		target.SetFunction<decimal>("Sinh", x => (decimal)Math.Sinh((double)x));
		target.SetFunction<decimal>("Sqrt", x => (decimal)Math.Sqrt((double)x));
		target.SetFunction<decimal>("Tan", x => (decimal)Math.Tan((double)x));
		target.SetFunction<decimal>("Tanh", x => (decimal)Math.Tanh((double)x));
		target.SetFunction<decimal>("Truncate", x => (decimal)Math.Truncate((double)x));
		target.SetFunction("NextRandom", () => (decimal)s_Random.NextDouble());
	}
}
