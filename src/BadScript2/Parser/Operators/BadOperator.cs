namespace BadScript2.Parser.Operators
{
    public abstract class BadOperator
    {
        protected BadOperator(int precedence, string symbol)
        {
            Precedence = precedence;
            Symbol = symbol;
        }

        public int Precedence { get; }
        public string Symbol { get; }
    }
}