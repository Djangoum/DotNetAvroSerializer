namespace DotNetAvroSerializer;

public readonly struct Union<T1, T2>
{
    private readonly T1? _v1;
    private readonly T2? _v2;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; Index = 2; }

    public T1? Value1 => _v1;
    public T2? Value2 => _v2;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, _ => null };

    public static implicit operator Union<T1, T2>(T1 val) => new(val);
    public static implicit operator Union<T1, T2>(T2 val) => new(val);
}

public readonly struct Union<T1, T2, T3>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; Index = 3; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, _ => null };

    public static implicit operator Union<T1, T2, T3>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3>(T3 val) => new(val);
}

public readonly struct Union<T1, T2, T3, T4>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3; private readonly T4? _v4;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; _v4 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; _v4 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; _v4 = default; Index = 3; }
    public Union(T4 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = val; Index = 4; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3; public T4? Value4 => _v4;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, 4 => _v4, _ => null };

    public static implicit operator Union<T1, T2, T3, T4>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4>(T3 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4>(T4 val) => new(val);
}

public readonly struct Union<T1, T2, T3, T4, T5>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3; private readonly T4? _v4; private readonly T5? _v5;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; _v4 = default; _v5 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; _v4 = default; _v5 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; _v4 = default; _v5 = default; Index = 3; }
    public Union(T4 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = val; _v5 = default; Index = 4; }
    public Union(T5 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = val; Index = 5; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3; public T4? Value4 => _v4; public T5? Value5 => _v5;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, 4 => _v4, 5 => _v5, _ => null };

    public static implicit operator Union<T1, T2, T3, T4, T5>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5>(T3 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5>(T4 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5>(T5 val) => new(val);
}

public readonly struct Union<T1, T2, T3, T4, T5, T6>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3; private readonly T4? _v4; private readonly T5? _v5; private readonly T6? _v6;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; _v4 = default; _v5 = default; _v6 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; _v4 = default; _v5 = default; _v6 = default; Index = 3; }
    public Union(T4 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = val; _v5 = default; _v6 = default; Index = 4; }
    public Union(T5 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = val; _v6 = default; Index = 5; }
    public Union(T6 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = val; Index = 6; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3; public T4? Value4 => _v4; public T5? Value5 => _v5; public T6? Value6 => _v6;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, 4 => _v4, 5 => _v5, 6 => _v6, _ => null };

    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T3 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T4 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T5 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6>(T6 val) => new(val);
}

public readonly struct Union<T1, T2, T3, T4, T5, T6, T7>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3; private readonly T4? _v4; private readonly T5? _v5; private readonly T6? _v6; private readonly T7? _v7;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; _v4 = default; _v5 = default; _v6 = default; _v7 = default; Index = 3; }
    public Union(T4 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = val; _v5 = default; _v6 = default; _v7 = default; Index = 4; }
    public Union(T5 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = val; _v6 = default; _v7 = default; Index = 5; }
    public Union(T6 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = val; _v7 = default; Index = 6; }
    public Union(T7 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = val; Index = 7; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3; public T4? Value4 => _v4; public T5? Value5 => _v5; public T6? Value6 => _v6; public T7? Value7 => _v7;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, 4 => _v4, 5 => _v5, 6 => _v6, 7 => _v7, _ => null };

    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T3 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T4 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T5 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T6 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7>(T7 val) => new(val);
}

public readonly struct Union<T1, T2, T3, T4, T5, T6, T7, T8>
{
    private readonly T1? _v1; private readonly T2? _v2; private readonly T3? _v3; private readonly T4? _v4; private readonly T5? _v5; private readonly T6? _v6; private readonly T7? _v7; private readonly T8? _v8;
    public byte Index { get; }

    public Union(T1 val) { _v1 = val; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = default; _v8 = default; Index = 1; }
    public Union(T2 val) { _v1 = default; _v2 = val; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = default; _v8 = default; Index = 2; }
    public Union(T3 val) { _v1 = default; _v2 = default; _v3 = val; _v4 = default; _v5 = default; _v6 = default; _v7 = default; _v8 = default; Index = 3; }
    public Union(T4 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = val; _v5 = default; _v6 = default; _v7 = default; _v8 = default; Index = 4; }
    public Union(T5 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = val; _v6 = default; _v7 = default; _v8 = default; Index = 5; }
    public Union(T6 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = val; _v7 = default; _v8 = default; Index = 6; }
    public Union(T7 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = val; _v8 = default; Index = 7; }
    public Union(T8 val) { _v1 = default; _v2 = default; _v3 = default; _v4 = default; _v5 = default; _v6 = default; _v7 = default; _v8 = val; Index = 8; }

    public T1? Value1 => _v1; public T2? Value2 => _v2; public T3? Value3 => _v3; public T4? Value4 => _v4; public T5? Value5 => _v5; public T6? Value6 => _v6; public T7? Value7 => _v7; public T8? Value8 => _v8;

    public object? GetValue() => Index switch { 1 => _v1, 2 => _v2, 3 => _v3, 4 => _v4, 5 => _v5, 6 => _v6, 7 => _v7, 8 => _v8, _ => null };

    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T1 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T2 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T3 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T4 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T5 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T6 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T7 val) => new(val);
    public static implicit operator Union<T1, T2, T3, T4, T5, T6, T7, T8>(T8 val) => new(val);
}
