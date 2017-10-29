/* Date: 1.8.2015, Time: 18:40 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IllidanS4.SharpUtils.Collections.Sequences;

namespace IllidanS4.SharpUtils.Reflection
{
	/// <summary>
	/// Represents a portion of the types in the .NET type system.
	/// </summary>
	public class TypeSystem
	{
		public BindingFlags VisibilityFlags{get; private set;}
		public bool Arrays{get; private set;}
		public bool MDArrays{get; private set;}
		public bool Pointers{get; private set;}
		public bool ByRefs{get; private set;}
		public bool ReferenceTypePointers{get; private set;}
		public bool Generic{get; private set;}
		public bool GenericParameters{get; private set;}
		public bool DerivedFromStatic{get; private set;}
		public bool DerivedFromOpenGeneric{get; private set;}
		
		public static readonly TypeSystem Public = new TypeSystem(BindingFlags.Public);
		public static readonly TypeSystem Internal = new TypeSystem(BindingFlags.NonPublic | BindingFlags.Public);
		public static readonly TypeSystem Everything = new TypeSystem(BindingFlags.NonPublic | BindingFlags.Public, true, true, true, true, true, true, true, true, true);
		
		
		public TypeSystem(BindingFlags visibility) : this(visibility, true, true, true, true, true, true, true, false, false)
		{
			
		}
		
		public TypeSystem(BindingFlags visibility, bool arrays = true, bool mdarrays = true, bool byrefs = true, bool pointers = true, bool refpointers = true, bool generic = true, bool genparams = true, bool staticderived = false, bool opengenderived = false)
		{
			VisibilityFlags = visibility;
			Arrays = arrays;
			MDArrays = mdarrays;
			Pointers = pointers;
			ByRefs = byrefs;
			ReferenceTypePointers = refpointers;
			Generic = generic;
			GenericParameters = genparams;
			DerivedFromStatic = staticderived;
			DerivedFromOpenGeneric = opengenderived;
		}
		
		/// <summary>
		/// Enumerates all types defined in CommonLanguageRuntimeLibrary, that is mscorlib.dll.
		/// </summary>
		public IEnumerable<Type> CoreTypes{
			get{
				return ModuleTypes(Types.CommonLanguageRuntimeLibrary);
			}
		}
		
		/// <summary>
		/// Enumerates all types which have a type code assigned to them.
		/// </summary>
		public IEnumerable<Type> BasicTypes{
			get{
				var core = Types.Core;
				foreach(string val in Enum.GetNames(typeof(TypeCode)).Skip(1))
				{
					yield return core.GetType("System."+val);
				}
			}
		}
		
		/// <summary>
		/// Enumerates all primitive types.
		/// </summary>
		public IEnumerable<Type> PrimitiveTypes{
			get{
				return CoreTypes.Where(t => t.IsPrimitive);
			}
		}
		
		/// <summary>
		/// Enumerates all defined types in the current application domain.
		/// </summary>
		public IEnumerable<Type> DefinedTypes{
			get{
				return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => AssemblyTypes(a));
			}
		}
		
		/// <summary>
		/// Enumerates all defined types and the fnptr type with its arrays.
		/// </summary>
		public IEnumerable<Type> ExtendedTypes{
			get{
				foreach(Type t in DefinedTypes)
				{
					yield return t;
				}
				if(ByRefs)
					yield return Types.Generated.FnPtrByRef;
				if(Pointers)
					yield return Types.Generated.FnPtrPointer;
				foreach(Type t in AllArrays(new FunctionPointerType()))
				{
					yield return t.UnderlyingSystemType;
				}
			}
		}
		
		/// <summary>
		/// Enumerates all extended types and types constructed from them.
		/// </summary>
		public IEnumerable<Type> PossibleTypes{
			get{
				return AllTypes(ExtendedTypes);
			}
		}
		
		/// <summary>
		/// Enumerates all types in an assembly.
		/// </summary>
		/// <param name="asm">The assembly containing the enumerated types.</param>
		public IEnumerable<Type> AssemblyTypes(Assembly asm)
		{
			return WithNested(asm.GetTypes());
		}
		
		/// <summary>
		/// Enumerates all types in a module.
		/// </summary>
		/// <param name="mod">The module containing the enumerated types.</param>
		public IEnumerable<Type> ModuleTypes(Module mod)
		{
			return WithNested(mod.GetTypes());
		}
		
		/// <summary>
		/// Enumerates all types constructed from the input.
		/// </summary>
		/// <param name="source">The input types.</param>
		public IEnumerable<Type> AllTypes(IEnumerable<Type> source)
		{
			return SequenceTools.SelectManyInfinite(AllGenericTypes(source).Select(t => AllDerivedRecursive(t)));
		}
		
		private IEnumerable<Type> AllGenericTypes(IEnumerable<Type> source)
		{
			if(Generic) return SequenceTools.SelectManyInfinite(source.Select(t => AllGenericItself(t, AllTypes(source))));
			else return source;
		}
		
		/// <summary>
		/// Enumerates source types and all types nested in them.
		/// </summary>
		/// <param name="types">The source types.</param>
		public IEnumerable<Type> WithNested(IEnumerable<Type> types)
		{
			foreach(Type t in types)
			{
				yield return t;
				foreach(Type t2 in AllNested(t))
				{
					yield return t2;
				}
			}
		}
		
		/// <summary>
		/// Enumerates all types nested in a type.
		/// </summary>
		/// <param name="type">The type containing nested types.</param>
		public IEnumerable<Type> AllNested(Type type)
		{
			foreach(Type t in type.GetNestedTypes(VisibilityFlags))
			{
				yield return t;
				foreach(Type t2 in AllNested(t))
				{
					yield return t2;
				}
			}
		}
		
		/// <summary>
		/// Enumerates all constructed types from a type.
		/// </summary>
		/// <param name="type">The source type.</param>
		public IEnumerable<Type> AllDerivedRecursive(Type type)
		{
			yield return type;
			foreach(Type t in AllDerivedBase(type))
			{
				yield return t;
			}
		}
		
		private IEnumerable<Type> AllDerivedBase(Type type)
		{
			if(
				!type.IsByRef && (!type.IsGenericTypeDefinition || DerivedFromOpenGeneric) &&
				(DerivedFromStatic || (!type.IsAbstract || !type.IsSealed))
			)
			{
				IEnumerable<Type> byrefs = Type.EmptyTypes;
				IEnumerable<Type> pointers = Type.EmptyTypes;
				if(Pointers)
				{
					if(ReferenceTypePointers || (type.IsValueType || type.IsPointer))
					{
						try{
							pointers = AllDerivedRecursive(type.MakePointerType());
						}catch(TypeLoadException)
						{
							
						}
					}
				}
				if(ByRefs)
				{
					try{
						byrefs = AllDerivedRecursive(type.MakeByRefType());
					}catch(TypeLoadException)
					{
						
					}
				}
				
				var arrays = AllArrays(type).Select(t => AllDerivedRecursive(t));
				
				return SequenceTools.SelectManyOuter(new[]{byrefs, pointers}.Concat(arrays));
			}else{
				return Type.EmptyTypes;
			}
		}
		
		private IEnumerable<Type> AllArrays(Type type)
		{
			if(Arrays)
			{
				Type arrt;
				try{
					arrt = type.MakeArrayType();
				}catch(TypeLoadException)
				{
					yield break;
				}
				yield return arrt;
				if(MDArrays) foreach(var arr in AllMDArrays(type)) yield return arr;
			}
		}
		
		/// <summary>
		/// Returns all MD array types created from a type.
		/// </summary>
		/// <param name="type">The element type of the arrays.</param>
		/// <returns>
		/// <paramref name="type"/>[*], <paramref name="type"/>[,], <paramref name="type"/>[,,] etc.
		/// Returns only arrays up to rank 32.
		/// </returns>
		public static IEnumerable<Type> AllMDArrays(Type type)
		{
			for(int i = 1; i <= 32; i++)
			{
				Type arr;
				try{
					arr = type.MakeArrayType(i);
				}catch(TypeLoadException)
				{
					yield break;
				}
				yield return arr;
			}
		}
		
		/// <summary>
		/// Returns all constructed generic types from a type. Also yields the definition.
		/// </summary>
		/// <param name="def">The type definition.</param>
		/// <param name="args">The possible type arguments.</param>
		public IEnumerable<Type> AllGenericItself(Type def, IEnumerable<Type> args)
		{
			yield return def;
			if(!def.IsGenericTypeDefinition) yield break;
			if(GenericParameters)
			{
				var tparams = def.GetGenericArguments();
				foreach(Type t in tparams)
				{
					yield return t;
				}
				args = SequenceTools.SelectManyOuter(args, tparams);
			}
			foreach(Type t in AllGeneric(def, args))
			{
				yield return t;
			}
		}
		
		/// <summary>
		/// Returns all constructed generic types from a type.
		/// </summary>
		/// <param name="def">The type definition.</param>
		/// <param name="args">The possible type arguments.</param>
		public IEnumerable<Type> AllGeneric(Type def, IEnumerable<Type> args)
		{
			if(!def.IsGenericTypeDefinition) return Type.EmptyTypes;
			Type[] tparams = def.GetGenericArguments();
			Type[] targs = new Type[tparams.Length];
			return SequenceTools.SelectManyInfinite(AllGenericBase(def, args, tparams, targs, 0));
		}
		
		private IEnumerable<IEnumerable<Type>> AllGenericBase(Type def, IEnumerable<Type> args, Type[] tparams, Type[] targs, int pos)
		{
			if(pos >= targs.Length)
			{
				Type con;
				try{
					con = def.MakeGenericType(targs);
				}catch(ArgumentException)
				{
					yield break;
				}
				yield return new Type[]{con};
			}else foreach(Type t in args)
			{
				if(!ConstraintsOkay(tparams[pos], t)) continue;
				targs[pos] = t;
				yield return SequenceTools.SelectManyInfinite(AllGenericBase(def, args, tparams, (Type[])targs.Clone(), pos+1));
			}
		}
		
		private static readonly Type nullableType = typeof(Nullable<>);
		
		private bool ConstraintsOkay(Type param, Type arg)
		{
			if(arg.IsPointer || arg.IsByRef) return false;
			if(!DerivedFromStatic && (arg.IsAbstract && arg.IsSealed)) return false;
			if(!DerivedFromOpenGeneric && arg.IsGenericTypeDefinition) return false;
			if((param.GenericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) != 0)
			{
				if(arg.IsValueType) return false;
			}
			if((param.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0)
			{
				if(!arg.IsValueType) return false;
				if(arg.GetGenericTypeDefinition() == nullableType) return false;
			}
			if((param.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) != 0)
			{
				if(arg.GetConstructor(Type.EmptyTypes) == null) return false;
			}
			var constr = param.GetGenericParameterConstraints();
			foreach(var c in constr)
			{
				if(!c.IsAssignableFrom(arg)) return false;
			}
			return true;
		}
	}
}
