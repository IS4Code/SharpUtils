/* Date: 12.12.2014, Time: 15:32 */
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace IllidanS4.SharpUtils.Reflection.Emit
{
	public class MethodSignature : IEquatable<MethodSignature>, ISignatureElement
	{
		public CallingConvention UnmanagedCallingConvention{get; private set;}
		public CallingConventions CallingConvention{get; private set;}
		public MdSigCallingConvention MethodCallingConvention{
			get{
				MdSigCallingConvention callConv = 0;
				if(UnmanagedCallingConvention != 0) callConv = (MdSigCallingConvention)((byte)UnmanagedCallingConvention-1);
				switch(CallingConvention)
				{
					case CallingConventions.HasThis:
						callConv |= MdSigCallingConvention.HasThis;
						break;
					case CallingConventions.ExplicitThis:
						callConv |= MdSigCallingConvention.ExplicitThis;
						break;
					case CallingConventions.VarArgs:
						callConv |= MdSigCallingConvention.Vararg;
						break;
				}
				return callConv;
			}
		}
		public Type ReturnType{get; private set;}
		private Type[] paramTypes;
		private Type[] optionalParamTypes;
		public Type[] ParameterTypes{
			get{
				return (Type[])paramTypes.Clone();
			}
			private set{
				paramTypes = value==null?Type.EmptyTypes:(Type[])value.Clone();
			}
		}
		
		public Type[] OptionalParameterTypes{
			get{
				return (Type[])optionalParamTypes.Clone();
			}
			private set{
				optionalParamTypes = value==null?Type.EmptyTypes:(Type[])value.Clone();
			}
		}
		
		public bool IsUnmanaged{
			get{
				return UnmanagedCallingConvention != 0;
			}
		}
		
		public bool HasThis{
			get{
				return (CallingConvention&CallingConventions.HasThis) != 0;
			}
		}
		
		public bool ExplicitThis{
			get{
				return (CallingConvention&CallingConventions.ExplicitThis) != 0;
			}
		}
		
		public bool VarArgs{
			get{
				return (CallingConvention&CallingConventions.VarArgs) != 0;
			}
		}
		
		/// <summary>
		/// Creates an empty method signature.
		/// </summary>
		public MethodSignature() : this(CallingConventions.Standard, typeof(void), Type.EmptyTypes, Type.EmptyTypes)
		{
			
		}
		
		/// <summary>
		/// Copies a method signature.
		/// </summary>
		/// <param name="original">The signature to be copied.</param>
		public MethodSignature(MethodSignature original) : this(original.CallingConvention, original.UnmanagedCallingConvention, original.ReturnType, original.paramTypes, original.optionalParamTypes)
		{
			
		}
		
		private MethodSignature(CallingConventions callingConvention, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			CallingConvention = callingConvention;
			UnmanagedCallingConvention = unmanagedCallConv;
			OptionalParameterTypes = optionalParameterTypes;
			ReturnType = returnType;
			ParameterTypes = parameterTypes;
		}
		
		/// <summary>
		/// Creates a new managed method signature.
		/// </summary>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The parameter types of the method.</param>
		public MethodSignature(Type returnType, params Type[] parameterTypes) : this(CallingConventions.Standard, returnType, parameterTypes)
		{
			
		}
		
		/// <summary>
		/// Creates a new method signature using managed calling convention.
		/// </summary>
		/// <param name="callingConvention">The managed calling convetion. Cannot be Any.</param>
		/// <param name="returnType">The return type.</param>
		/// <param name="parameterTypes">The parameter types.</param>
		public MethodSignature(CallingConventions callingConvention, Type returnType, Type[] parameterTypes) : this(callingConvention, returnType, parameterTypes, null)
		{
			
		}
		
		/// <summary>
		/// Creates a new method signature using managed calling convention.
		/// </summary>
		/// <param name="callingConvention">The managed calling convetion. Cannot be Any.</param>
		/// <param name="returnType">The return type.</param>
		/// <param name="parameterTypes">The parameter types.</param>
		/// <param name="optionalParameterTypes">Optional parameter types for varargs calling convention.</param>
		public MethodSignature(CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			CallingConvention = callingConvention;
			OptionalParameterTypes = optionalParameterTypes;
			ReturnType = returnType;
			ParameterTypes = parameterTypes;
		}
		
		/// <summary>
		/// Creates a new method signature using unmanaged calling convention.
		/// </summary>
		/// <param name="unmanagedCallConv">The unmanaged calling convention. Cannot be Winapi.</param>
		/// <param name="returnType">The return type.</param>
		/// <param name="parameterTypes">The parameter types.</param>
		public MethodSignature(CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			if(unmanagedCallConv == System.Runtime.InteropServices.CallingConvention.Winapi) throw new ArgumentException("Unmanaged calling convention cannot be Winapi.", "unmanagedCallConv");
			UnmanagedCallingConvention = unmanagedCallConv;
			ReturnType = returnType;
			ParameterTypes = parameterTypes;
		}
		
		/// <summary>
		/// Gets a signature of a method.
		/// </summary>
		/// <param name="method">The method to be taken its signature from.</param>
		/// <returns>The signature of the method.</returns>
		public static MethodSignature FromMethodInfo(MethodInfo method)
		{
			return FromMethodInfo(method, null);
		}
		
		/// <summary>
		/// Gets a signature of a method.
		/// </summary>
		/// <param name="method">The method to be taken its signature from.</param>
		/// <param name="optionalParameterTypes">Optional parameter types for varargs method.</param>
		/// <returns>The signature of the method.</returns>
		public static MethodSignature FromMethodInfo(MethodInfo method, params Type[] optionalParameterTypes)
		{
			var callconv = method.CallingConvention;
			if((callconv&CallingConventions.VarArgs)==0 && optionalParameterTypes != null) throw new ArgumentException("Method must ve varargs to specify optional parameter types.");
			return new MethodSignature(callconv, method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray(), optionalParameterTypes);
		}
		
		public byte[] GetSignature(ModuleBuilder modBuilder)
		{
			SignatureHelper sig = SignatureTools.GetSigHelper(modBuilder);
			sig.AddElement(this);
			return sig.GetSignature();
		}

		void ISignatureElement.AddSignature(SignatureHelper signature)
		{
			signature.AddData((byte)MethodCallingConvention);
			signature.AddData(paramTypes.Length+optionalParamTypes.Length);
			signature.AddArgumentSignature(ReturnType);
			foreach(var type in paramTypes)
			{
				signature.AddArgumentSignature(type);
			}
			if(VarArgs)
			{
				signature.AddSentinel();
				foreach(var type in optionalParamTypes)
				{
					signature.AddArgumentSignature(type);
				}
			}
		}
		
		public override string ToString()
		{
			StringBuilder name = new StringBuilder();
			name.Append(ReturnType.ToString());
			name.Append(" (");
			name.Append(String.Join(", ", paramTypes.Select(t => t.ToString())));
			if(VarArgs)
			{
				if(paramTypes.Length > 0) name.Append(", ");
				name.Append("...");
				if(optionalParamTypes.Length > 0)
				{
					name.Append(", ");
					name.Append(String.Join(", ", optionalParamTypes.Select(t => t.ToString())));
				}
			}
			name.Append(")");
			return name.ToString();
		}
		
		public Type[] GetReturnTypeRequiredCustomModifiers()
		{
			ModifiedType mt = ReturnType as ModifiedType;
			if(mt == null) return null;
			return mt.Modifiers.EnumerateRequiredCustomModifiers().ToArray();
		}
		
		public Type[] GetReturnTypeOptionalCustomModifiers()
		{
			ModifiedType mt = ReturnType as ModifiedType;
			if(mt == null) return null;
			return mt.Modifiers.EnumerateOptionalCustomModifiers().ToArray();
		}
		
		public Type[][] GetParameterTypeRequiredCustomModifiers()
		{
			return ParameterTypes.OfType<ModifiedType>().Select(t => t.Modifiers.EnumerateRequiredCustomModifiers().ToArray()).ToArray();
		}
		
		public Type[][] GetParameterTypeOptionalCustomModifiers()
		{
			return ParameterTypes.OfType<ModifiedType>().Select(t => t.Modifiers.EnumerateOptionalCustomModifiers().ToArray()).ToArray();
		}
		
		#region Equals and GetHashCode implementation
		public bool Equals(MethodSignature other)
		{
			return
				this.CallingConvention == other.CallingConvention &&
				this.UnmanagedCallingConvention == other.UnmanagedCallingConvention &&
				this.ReturnType == other.ReturnType &&
				this.paramTypes.SequenceEqual(other.paramTypes) &&
				this.optionalParamTypes.SequenceEqual(other.optionalParamTypes);
		}
		
		public override bool Equals(object obj)
		{
			MethodSignature other = obj as MethodSignature;
			if (other == null)
				return false;
			return Equals(other);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (paramTypes != null)
					hashCode += 1000000007 * paramTypes.GetHashCode();
				if (optionalParamTypes != null)
					hashCode += 1000000009 * optionalParamTypes.GetHashCode();
				hashCode += 1000000021 * UnmanagedCallingConvention.GetHashCode();
				hashCode += 1000000033 * CallingConvention.GetHashCode();
				if (ReturnType != null)
					hashCode += 1000000087 * ReturnType.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(MethodSignature lhs, MethodSignature rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			if (ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(MethodSignature lhs, MethodSignature rhs)
		{
			return !(lhs == rhs);
		}
		#endregion
	}
}
