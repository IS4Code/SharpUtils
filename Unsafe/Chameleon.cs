/* Date: 12.11.2014, Time: 15:53 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Unsafe
{
	/// <summary>
	/// A mutable object that is able to change its type.
	/// </summary>
	public unsafe class Chameleon
	{
		/// <summary>
		/// The type accessor of the chameleon object.
		/// </summary>
		public Type Type{
			get{
				return this.GetType();
			}
			set{
				((IntPtr*)UnsafeTools.GetAddress(this))[0] = value.TypeHandle.Value;
			}
		}
		
		/// <summary>
		/// Returns the pointer to the start of the object's data.
		/// </summary>
		public IntPtr Data{
			get{
				return UnsafeTools.GetAddress(this)+IntPtr.Size;
			}
		}
		
		/// <summary>
		/// Returns the pointer to the start of the object's data.
		/// </summary>
		[CLSCompliant(false)]
		public byte* ByteData{
			get{
				return (byte*)Data;
			}
		}
		
		public Chameleon()
		{
			
		}
		
		private static readonly AssemblyBuilder chameleonAssembly;
		private static readonly ModuleBuilder chameleonModule;
		private static readonly Dictionary<int,Type> chameleons = new Dictionary<int,Type>();
		private static readonly Type chameleonType = typeof(Chameleon);
		static Chameleon()
		{
			chameleonAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ChameleonAssembly"), AssemblyBuilderAccess.Run);
			chameleonModule = chameleonAssembly.DefineDynamicModule("ChameleonAssembly.dll");
		}
		
		/// <summary>
		/// Creates a new chameleon objects by specifying its size.
		/// </summary>
		/// <param name="size">The size of the new object.</param>
		/// <returns>The new chameleon object.</returns>
		public static Chameleon Create(int size)
		{
			if(size == 0) return new Chameleon();
			Type type;
			if(!chameleons.TryGetValue(size, out type))
			{
				TypeBuilder tb = chameleonModule.DefineType("Chameleon"+size, TypeAttributes.Public | TypeAttributes.Sealed, chameleonType, size);
				type = chameleons[size] = tb.CreateType();
			}
			return ((Chameleon)Activator.CreateInstance(type)).Mutate<Chameleon>();
		}
		
		/// <summary>
		/// Changes the object's type and cast-returns it.
		/// </summary>
		/// <returns></returns>
		public T Mutate<T>()
		{
			Type = TypeOf<T>.TypeID;
			return (T)(object)this;
		}
	}
}
