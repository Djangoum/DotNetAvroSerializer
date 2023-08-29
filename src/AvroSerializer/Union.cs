using System;

namespace DotNetAvroSerializer
{
    public struct Union<T1, T2>
    {
        private readonly T1? value1;
        private readonly T2? value2;

        public Union(T1 val1)
        {
            ArgumentNullException.ThrowIfNull(val1, nameof(val1));

            value1 = val1;
            value2 = default;
        }

        public Union(T2 val2)
        {
            ArgumentNullException.ThrowIfNull(val2, nameof(val2));

            value2 = val2;
            value1 = default;
        }

        public T1? Value1 => value1;

        public T2? Value2 => value2;

        public static implicit operator T1?(Union<T1, T2> union)
        {
            return union.value1;
        }

        public static implicit operator T2?(Union<T1, T2> union)
        {
            return union.value2;
        }

        public static Union<T1, T2> From(T1 val1)
        { 
            return new Union<T1, T2>(val1);
        }

        public static Union<T1, T2> From(T2 val2)
        {
            return new Union<T1,T2>(val2);
        }
    }

    public record struct Union<T1, T2, T3>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
    }

    public record struct Union<T1, T2, T3, T4>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;
    }

    public record struct Union<T1, T2, T3, T4, T5>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;
        public T5 Value5;
    }

    public record struct Union<T1, T2, T3, T4, T5, T6>
    {
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;
        public T5 Value5;
        public T6 Value6;
    }
}
