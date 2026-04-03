using System;
using System.Linq.Expressions;
using System.Reflection;
using Unity.Netcode;

public static class BufferSerializerFactory
{
	private static readonly Lazy<Func<FastBufferReader, object>> _creator =
		new Lazy<Func<FastBufferReader, object>>(BuildDelegate);

	private static Func<FastBufferReader, object> BuildDelegate()
	{
		// Get the assembly type (adjust namespace if needed)
		var readerType = Type.GetType("Unity.Netcode.BufferSerializerReader, Unity.Netcode")
						 ?? throw new InvalidOperationException("BufferSerializerReader type not found.");

		var serializerType = Type.GetType("Unity.Netcode.BufferSerializer`1, Unity.Netcode")
						 ?.MakeGenericType(readerType)
						 ?? throw new InvalidOperationException("BufferSerializer<> type not found.");

		// Get internal BufferSerializer<T> constructor
		var ctor = serializerType.GetConstructor(
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new Type[] { readerType },
			null
		) ?? throw new InvalidOperationException("BufferSerializer constructor not found.");

		// Get BufferSerializerReader constructor
		var readerCtor = readerType.GetConstructor(new Type[] { typeof(FastBufferReader) })
						 ?? throw new InvalidOperationException("BufferSerializerReader constructor not found.");

		// Lambda parameter
		var readerParam = Expression.Parameter(typeof(FastBufferReader), "reader");

		// new BufferSerializerReader(reader)
		var readerExpr = Expression.New(readerCtor, readerParam);

		// new BufferSerializer<BufferSerializerReader>(readerExpr)
		var serializerExpr = Expression.New(ctor, readerExpr);

		// Compile to Func<FastBufferReader, object>
		return Expression.Lambda<Func<FastBufferReader, object>>(serializerExpr, readerParam).Compile();
	}

	public static object Create(FastBufferReader reader) => _creator.Value(reader);
}