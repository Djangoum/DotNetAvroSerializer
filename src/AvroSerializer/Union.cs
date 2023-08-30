using System;

namespace DotNetAvroSerializer
{
    public struct Union<T1, T2>
    {
        private readonly object? value;

        public Union(object? val)
        {
            ArgumentNullException.ThrowIfNull(val, nameof(val));

            value = val;
        }

        public object? GetUnionValue()
        {
            return value;
        }

        public static implicit operator Union<T1, T2>(T1 val)
        {
            return new Union<T1, T2>(val);
        }

        public static implicit operator Union<T1, T2>(T2 val)
        {
            return new Union<T1, T2>(val);
        }

        public static implicit operator T2?(Union<T1, T2> union)
        {
            return union.value is T2 ? (T2?)union.value : default(T2);
        }

        public static implicit operator T1?(Union<T1, T2> union)
        {
            return union.value is T1 ? (T1?)union.value : default(T1);
        }
    }

    public struct Union<T1, T2, T3>
    {
        private readonly object? value;

        public Union(object? val)
        {
            ArgumentNullException.ThrowIfNull(val, nameof(val));

            value = val;
        }

        public object? GetUnionValue()
        {
            return value;
        }

        public static implicit operator Union<T1, T2, T3>(T1 val)
        {
            return new Union<T1, T2, T3>(val);
        }

        public static implicit operator Union<T1, T2, T3>(T2 val)
        {
            return new Union<T1, T2, T3>(val);
        }

        public static implicit operator Union<T1, T2, T3>(T3 val)
        {
            return new Union<T1, T2, T3>(val);
        }

        public static implicit operator T1?(Union<T1, T2, T3> union)
        {
            return union.value is T1 ? (T1?)union.value : default(T1);
        }

        public static implicit operator T2?(Union<T1, T2, T3> union)
        {
            return union.value is T2 ? (T2?)union.value : default(T2);
        }

        public static implicit operator T3?(Union<T1, T2, T3> union)
        {
            return union.value is T3 ? (T3?)union.value : default(T3);
        }
    }

    public record struct Union<T1, T2, T3, T4>
    {
        private readonly object? value;

        public Union(object? val)
        {
            ArgumentNullException.ThrowIfNull(val, nameof(val));

            value = val;
        }

        public object? GetUnionValue()
        {
            return value;
        }

        public static implicit operator Union<T1, T2, T3, T4>(T1 val)
        {
            return new Union<T1, T2, T3, T4>(val);
        }

        public static implicit operator Union<T1, T2, T3, T4>(T2 val)
        {
            return new Union<T1, T2, T3, T4>(val);
        }

        public static implicit operator Union<T1, T2, T3, T4>(T3 val)
        {
            return new Union<T1, T2, T3, T4>(val);
        }

        public static implicit operator Union<T1, T2, T3, T4>(T4 val)
        {
            return new Union<T1, T2, T3, T4>(val);
        }

        public static implicit operator T1?(Union<T1, T2, T3, T4> union)
        {
            return union.value is T1 ? (T1?)union.value : default(T1);
        }

        public static implicit operator T2?(Union<T1, T2, T3, T4> union)
        {
            return union.value is T2 ? (T2?)union.value : default(T2);
        }

        public static implicit operator T3?(Union<T1, T2, T3, T4> union)
        {
            return union.value is T3 ? (T3?)union.value : default(T3);
        }

        public static implicit operator T4?(Union<T1, T2, T3, T4> union)
        {
            return union.value is T4 ? (T4?)union.value : default(T4);
        }
    }
}
