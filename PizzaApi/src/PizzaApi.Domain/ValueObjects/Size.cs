namespace PizzaApi.Domain.ValueObjects
{
    public class Size
    {
        public string Value { get; private set; }

        private Size() { } // EF Core requires a parameterless constructor

        private Size(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Size cannot be empty.", nameof(value));

            Value = value;
        }

        public static Size Small => new Size("Small");
        public static Size Medium => new Size("Medium");
        public static Size Large => new Size("Large");

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (obj is Size size)
                return Value == size.Value;

            return false;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}