using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace IllidanS4.SharpUtils
{
	public static class EmptyManager
	{
		public static object GetEmpty(Type t)
		{
			if(t.IsArray)
			{
				return Array.CreateInstance(t.GetElementType(), 0);
			}else if(t == TypeOf<Missing>.TypeID)
			{
				return Missing.Value;
			}else if(t == TypeOf<EventArgs>.TypeID)
			{
				return EventArgs.Empty;
			}else if(t == TypeOf<Version>.TypeID)
			{
				return new Version();
			}else if(t == TypeOf<string>.TypeID)
			{
				return String.Empty;
			}else if(t == TypeOf<IEnumerable>.TypeID)
			{
				return new EmptyEnumerable();
			}else if(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				Type arg = t.GetGenericArguments()[0];
				return Activator.CreateInstance(typeof(EmptyEnumerable<>).MakeGenericType(arg));
			}else if(t == TypeOf<Type>.TypeID)
			{
				return typeof(void);
			}else if(t == TypeOf<Enum>.TypeID)
			{
				return (EmptyEnum)0;
			}else if(t == TypeOf<ValueType>.TypeID)
			{
				return default(EmptyStruct);
			}else if(t == TypeOf<Stream>.TypeID)
			{
				return Stream.Null;
			}else if(t == TypeOf<TextWriter>.TypeID)
			{
				return TextWriter.Null;
			}else if(t == TypeOf<TextReader>.TypeID)
			{
				return TextReader.Null;
			}else if(t == TypeOf<DBNull>.TypeID)
			{
				return DBNull.Value;
			}else if(t == TypeOf<Action>.TypeID)
			{
				return (Action)(()=>{});
			}else if(t == TypeOf<IPAddress>.TypeID)
			{
				return IPAddress.None;
			}
			return null;
		}
		
		private class EmptyEnumerable : IEnumerable
		{
			public IEnumerator GetEnumerator()
			{
				yield break;
			}
		}
		
		private class EmptyEnumerable<T> : IEnumerable<T>
		{
			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			
			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}
		}
		
		private enum EmptyEnum{}
		
		private struct EmptyStruct{}
	}
}