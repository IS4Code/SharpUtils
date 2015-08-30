/* Date: ‎20.12.‎2012, Time: ‏‎17:07 */
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IllidanS4.SharpUtils.Accessing
{
	/// <summary>
	/// Methods in this class can generate dynamic methods that can return and set a field.
	/// </summary>
	public static class AccessorGenerator
	{
		/// <summary>
		/// Generates a method that can set a field's value.
		/// </summary>
		/// <param name="fi">The field accessed by the method.</param>
		/// <param name="target">The object that contains the field, if is it not static.</param>
		/// <returns>The accessor procedure.</returns>
		public static Action<T> GenerateSetter<T>(FieldInfo fi, object target)
		{
			if(!fi.IsStatic && target == null) throw new ArgumentNullException("target", "Target can be NULL only if field is static.");
			if(fi.IsStatic && target != null) throw new ArgumentException("Target must be NULL if field is static.", "target");
			
			Type t = TypeOf<T>.TypeID;
			if(!fi.FieldType.IsAssignableFrom(t)) throw new ArgumentException("Field type does not match type argument.", "T");
			
			DynamicMethod setter = new DynamicMethod(
				"set_"+fi.Name,
				typeof(void),
				fi.IsStatic?new[]{t}:new[]{target.GetType(), t},
				fi.DeclaringType
			);
			ILGenerator il = setter.GetILGenerator();
			if(fi.IsStatic)
			{
				il.Emit(OpCodes.Ldarg_0);
				if(t != fi.FieldType && t.IsValueType)il.Emit(OpCodes.Box, t);
				il.Emit(OpCodes.Stsfld, fi);
			}else{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldarg_1);
				if(t != fi.FieldType && t.IsValueType)il.Emit(OpCodes.Box, t);
				il.Emit(OpCodes.Stfld, fi);
			}
			il.Emit(OpCodes.Ret);
			return (Action<T>)setter.CreateDelegate(TypeOf<Action<T>>.TypeID, target);
		}
		
		/// <summary>
		/// Generates a method that can get a field's value.
		/// </summary>
		/// <param name="fi">The field accessed by the method.</param>
		/// <param name="target">The object that contains the field, if is it not static.</param>
		/// <returns>The accessor function.</returns>
		public static Func<T> GenerateGetter<T>(FieldInfo fi, object target)
		{
			if(!fi.IsStatic && target == null) throw new ArgumentNullException("target", "Target can be NULL only if field is static.");
			if(fi.IsStatic && target != null) throw new ArgumentException("Target must be NULL if field is static.", "target");
			
			Type t = TypeOf<T>.TypeID;
			if(!t.IsAssignableFrom(fi.FieldType)) throw new ArgumentException("Field type does not match type argument.", "T");
			
			DynamicMethod getter = new DynamicMethod(
				"get_"+fi.Name,
				t,
				fi.IsStatic?Type.EmptyTypes:new[]{target.GetType()},
				fi.DeclaringType
			);
			ILGenerator il = getter.GetILGenerator();
			if(fi.IsStatic)
			{
				il.Emit(OpCodes.Ldsfld, fi);
			}else{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldfld, fi);
			}
			if(fi.FieldType != t && fi.FieldType.IsValueType)il.Emit(OpCodes.Box, fi.FieldType);
			il.Emit(OpCodes.Ret);
			return (Func<T>)getter.CreateDelegate(TypeOf<Func<T>>.TypeID, target);
		}
	}
}