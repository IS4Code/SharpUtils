/* Date: 15.7.2015, Time: 21:45 */
using System;
using System.Linq;
using System.Text;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Text
{
	/// <summary>
	/// Represents a mutable string.
	/// </summary>
	public abstract class StringReference
	{
		public StringReference()
		{
			
		}
		
		public abstract void Clear();
		public abstract override string ToString();
		public abstract void Append(string value);
		public abstract int Length{get;}
		public abstract void Insert(int index, string value);
		public abstract void Remove(int startIndex, int count);
		public abstract void Replace(string oldValue, string newValue);
		
		public virtual void Insert(int index, string value, int count)
		{
			Insert(index, String.Concat(Enumerable.Repeat(value, count)));
		}
		
		public virtual void AppendLine()
		{
			Append(Environment.NewLine);
		}
		
		public virtual void AppendLine(string value)
		{
			Append(value+Environment.NewLine);
		}
		
		public static implicit operator StringReference(StringBuilder builder)
		{
			return Create(builder);
		}
		
		public static implicit operator StringReference(string r)
		{
			return new StringRef(AtomicContainer.Create(r));
		}
		
		public static StringReference Create(StringBuilder builder)
		{
			return new StringBuilderRef(builder);
		}
		
		public static StringReference Create(string value)
		{
			return Create(AtomicContainer.Create(value));
		}
		
		public static StringReference Create(IReadWriteAccessor<string> acc)
		{
			return new StringRef(acc);
		}
		
		public static void Create(ref string reference, Action<StringReference> outputAct)
		{
			ReferenceAccessor.Create(ref reference, r => outputAct(Create(r)));
		}
		
		public static TRet Create<TRet>(ref string reference, Func<StringReference, TRet> outputFunc)
		{
			return ReferenceAccessor.Create(ref reference, r => outputFunc(Create(r)));
		}
		
		private sealed class StringBuilderRef : StringReference
		{
			readonly StringBuilder builder;
			
			public StringBuilderRef(StringBuilder builder)
			{
				this.builder = builder;
			}
			
			public override void Clear()
			{
				builder.Clear();
			}
			
			public override string ToString()
			{
				return builder.ToString();
			}
			
			public override void Append(string value)
			{
				builder.Append(value);
			}
		
			public override void AppendLine()
			{
				builder.AppendLine();
			}
			
			public override void AppendLine(string value)
			{
				builder.AppendLine(value);
			}
			
			public override int Length{
				get{
					return builder.Length;
				}
			}
			
			public override void Insert(int index, string value)
			{
				builder.Insert(index, value);
			}
			
			public override void Insert(int index, string value, int count)
			{
				builder.Insert(index, value, count);
			}
			
			public override void Remove(int startIndex, int count)
			{
				builder.Remove(startIndex, count);
			}
			
			public override void Replace(string oldValue, string newValue)
			{
				builder.Replace(oldValue, newValue);
			}
		}
		
		private sealed class StringRef : StringReference
		{
			readonly IReadWriteAccessor<string> acc;
			
			private string Item{
				get{
					return acc.Item;
				}
				set{
					acc.Item = value;
				}
			}
			
			public StringRef(IReadWriteAccessor<string> acc)
			{
				this.acc = acc;
			}
			
			public override string ToString()
			{
				return Item;
			}
			
			public override void Replace(string oldValue, string newValue)
			{
				Item = Item.Replace(oldValue, newValue);
			}
			
			public override void Remove(int startIndex, int count)
			{
				Item = Item.Remove(startIndex, count);
			}
			
			public override int Length{
				get{
					return Item.Length;
				}
			}
			
			public override void Insert(int index, string value)
			{
				Item = Item.Insert(index, value);
			}
			
			public override void Clear()
			{
				Item = String.Empty;
			}
			
			public override void Append(string value)
			{
				Item += value;
			}
		}
	}
}
