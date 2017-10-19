namespace dona.Forms.Model
{
    public class Credit
    {
        public double Amount { get; }
        public virtual string FormattedCredit => $"Su saldo es de: ${Amount}";

        public Credit(double amount)
        {
            Amount = amount;
        }

        protected Credit() { }
    }

    public class UnlimitedCredit : Credit
    {
        public override string FormattedCredit =>
            "Dispone de saldo ilimitado. Sus donaciones se cargarán en su factura.";
    }
}