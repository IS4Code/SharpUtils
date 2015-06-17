/* Date: ‎20.12.‎2012, Time: ‏‎17:02 */
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Interop;
using IllidanS4.SharpUtils.Metadata;

namespace IllidanS4.SharpUtils.Accessing
{
	public sealed class FieldAccessor<T> : ReadFieldAccessor<T>, IReadWriteAccessor<T>, IRefReference<T>, ITypedReference
	{
		readonly Action<T> setter;
		
		public FieldAccessor(FieldInfo field, object target) : base(field, target)
		{
			if(field.IsInitOnly) throw new ArgumentException("This field is read-only.", "field");
			setter = AccessorGenerator.GenerateSetter<T>(field, target);
		}
		
		public new T Item{
			set{
				setter(value);
			}
			get{
				return base.Item;
			}
		}
		
		object IWriteAccessor.Item{
			set{
				Item = (T)value;
			}
		}
		object IReadWriteAccessor.Item{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
		
		public TRet GetReference<TRet>(Reference.RefFunc<T, TRet> func)
		{
			return GetReference<TRet>(tr => tr.AsRef(func));
		}
		
		public TRet GetReference<TRet>(Reference.OutFunc<T, TRet> func)
		{
			return GetReference<TRet>(tr => tr.AsRef(Reference.OutToRefFunc(func)));
		}
		
		[CLSCompliant(false)]
		public TRet GetReference<TRet>(TypedReferenceTools.TypedRefFunc<TRet> func)
		{
			return TypedReferenceTools.MakeTypedReference(Target, new[]{Field}, func);
		}
		
		object IStrongBox.Value{
			get{
				return Item;
			}
			set{
				Item = (T)value;
			}
		}
	}
	
	public class ReadFieldAccessor<T> : BasicReadAccessor<T>, IFieldAccessor
	{
		readonly Func<T> getter;
		
		public FieldInfo Field{get; private set;}
		public object Target{get; private set;}
		
		public ReadFieldAccessor(FieldInfo field, object target)
		{
			if(field.FieldType != TypeOf<T>.TypeID)throw new ArgumentException("Field is not of type "+TypeOf<T>.TypeID.ToString()+".", "field");
			if(target != null && field.DeclaringType != target.GetType())throw new TypeLoadException("Target does not contain this field.");
			getter = AccessorGenerator.GenerateGetter<T>(field, target);
			
			Field = field;
			Target = target;
		}
		
		public override T Item
		{
			get{
				return getter();
			}
		}
	}
	
	public interface IFieldAccessor : IReadAccessor
	{
		FieldInfo Field{get;}
		object Target{get;}
	}
}