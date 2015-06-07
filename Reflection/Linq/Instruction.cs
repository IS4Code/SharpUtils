/* Date: 3.4.2015, Time: 18:18 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using IllidanS4.SharpUtils.Accessing;

namespace IllidanS4.SharpUtils.Reflection.Linq
{
	/// <summary>
	/// Description of Instruction.
	/// </summary>
	public struct Instruction : IEquatable<Instruction>
	{
		public OpCode? OpCode{get; private set;}
		private readonly bool hasArg;
		public byte? ByteArgument{get; private set;}
		[CLSCompliant(false)]
		public sbyte? SByteArgument{get; private set;}
		public short? Int16Argument{get; private set;}
		public int? Int32Argument{get; private set;}
		public long? Int64Argument{get; private set;}
		public float? SingleArgument{get; private set;}
		public double? DoubleArgument{get; private set;}
		private readonly object argument;
		public object Argument{
			get{
				if(!hasArg) return null;
				if(argument != null)
				{
					var read = argument as IReadAccessor;
					if(read != null) return read.Item;
					else return argument;
				}
				if(ByteArgument != null) return ByteArgument;
				if(SByteArgument != null) return SByteArgument;
				if(Int16Argument != null) return Int16Argument;
				if(Int32Argument != null) return Int32Argument;
				if(Int64Argument != null) return Int64Argument;
				if(SingleArgument != null) return SingleArgument;
				if(DoubleArgument != null) return DoubleArgument;
				return null;
			}
		}
		
		private readonly Action<ILGenerator> emit;
		
		public Instruction(OpCode opcode) : this(opcode, false)
		{
			
		}
		
		private Instruction(OpCode opcode, bool hasArg) : this()
		{
			OpCode = opcode;
			this.hasArg = hasArg;
			if(!hasArg) emit = il => il.Emit(opcode);
		}
		
		private Instruction(object arg, Action<ILGenerator> emit) : this()
		{
			OpCode = null;
			hasArg = true;
			argument = arg;
			this.emit = emit;
		}
		
		public Instruction(OpCode opcode, byte arg) : this(opcode, true)
		{
			ByteArgument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		[CLSCompliant(false)]
		public Instruction(OpCode opcode, sbyte arg) : this(opcode, true)
		{
			SByteArgument= arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, short arg) : this(opcode, true)
		{
			Int16Argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, int arg) : this(opcode, true)
		{
			Int32Argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, long arg) : this(opcode, true)
		{
			Int64Argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, float arg) : this(opcode, true)
		{
			SingleArgument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, double arg) : this(opcode, true)
		{
			DoubleArgument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, IReadAccessor<Label> arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg.Item);
		}
		
		public Instruction(OpCode opcode, Type arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, MethodInfo arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, FieldInfo arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, SignatureHelper arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, string arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, IReadAccessor<LocalBuilder> arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => {il.Emit(opcode, arg.Item);};
		}
		
		public Instruction(OpCode opcode, IReadAccessor<Label>[] arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => {il.Emit(opcode, arg.Select(a => a.Item).ToArray());};
		}
		
		public Instruction(OpCode opcode, ConstructorInfo arg) : this(opcode, true)
		{
			argument = arg;
			emit = il => il.Emit(opcode, arg);
		}
		
		public Instruction(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes) : this(opcode, true)
		{
			argument = new object[]{methodInfo, optionalParameterTypes};
			emit = il => il.EmitCall(opcode, methodInfo, optionalParameterTypes);
		}
		
		public Instruction(OpCode opcode, CallingConventions callingConvetions, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes) : this(opcode, true)
		{
			argument = new object[]{callingConvetions, returnType, parameterTypes, optionalParameterTypes};
			emit = il => il.EmitCalli(opcode, callingConvetions, returnType, parameterTypes, optionalParameterTypes);
		}
		
		public Instruction(OpCode opcode, CallingConvention callingConvention, Type returnType, Type[] parameterTypes) : this(opcode, true)
		{
			argument = new object[]{callingConvention, returnType, parameterTypes};
			emit = il => il.EmitCalli(opcode, callingConvention, returnType, parameterTypes);
		}
		
		public void Emit(ILGenerator il)
		{
			emit.Invoke(il);
		}
		
		public static Instruction LexicalScope(params Instruction[] instructions)
		{
			return LexicalScope((IEnumerable<Instruction>)instructions);
		}
		
		public static Instruction LexicalScope(IEnumerable<Instruction> instructions)
		{
			return new Instruction(
				instructions,
				il => {
					il.BeginScope();
					foreach(var ins in instructions)
					{
						ins.Emit(il);
					}
					il.EndScope();
				}
			);
		}
		
		public static Instruction DefineLabel(IWriteAccessor<Label> output)
		{
			return new Instruction(
				output,
				il => {
					output.Item = il.DefineLabel();
				}
			);
		}
		
		public static Instruction MarkLabel(IReadAccessor<Label> input)
		{
			return new Instruction(
				input,
				il => {
					il.MarkLabel(input.Item);
				}
			);
		}
		
		public static Instruction DeclareLocal(IWriteAccessor<LocalBuilder> output, Type localType)
		{
			return DeclareLocal(output, localType, false);
		}
		
		public static Instruction DeclareLocal(IWriteAccessor<LocalBuilder> output, Type localType, bool pinned)
		{
			return new Instruction(
				output,
				il => {
					output.Item = il.DeclareLocal(localType, pinned);
				}
			);
		}
		
		public static Instruction DeclareLocal(Type localType)
		{
			return DeclareLocal(localType, false);
		}
		
		public static Instruction DeclareLocal(Type localType, bool pinned)
		{
			return new Instruction(
				null,
				il => {
					il.DeclareLocal(localType, pinned);
				}
			);
		}
		
		public static implicit operator Instruction(OpCode opcode)
		{
			return new Instruction(opcode);
		}
		
		public static explicit operator Action<ILGenerator>(Instruction instruction)
		{
			return instruction.emit;
		}
		
		#region Equals and GetHashCode implementation
		
		public override bool Equals(object obj)
		{
			return (obj is Instruction) && Equals((Instruction)obj);
		}
		
		public bool Equals(Instruction other)
		{
			return Object.Equals(Argument, other.Argument);
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * OpCode.GetHashCode();
				hashCode += 1000000009 * ByteArgument.GetHashCode();
				hashCode += 1000000021 * SByteArgument.GetHashCode();
				hashCode += 1000000033 * Int16Argument.GetHashCode();
				hashCode += 1000000087 * Int32Argument.GetHashCode();
				hashCode += 1000000093 * Int64Argument.GetHashCode();
				hashCode += 1000000097 * SingleArgument.GetHashCode();
				hashCode += 1000000103 * DoubleArgument.GetHashCode();
				if (argument != null)
					hashCode += 1000000123 * argument.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(Instruction left, Instruction right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(Instruction left, Instruction right)
		{
			return !left.Equals(right);
		}
		
		public override string ToString()
		{
			object arg = Argument;
			if(arg == null)
			{
				return OpCode.ToString();
			}else{
				object[] arr = arg as object[];
				if(arr != null)
				{
					return OpCode.ToString()+" "+String.Join(" ", arr.Select(o => (o??"").ToString()));
				}else{
					return OpCode.ToString()+" "+arg;
				}
			}
			
		}
		#endregion
	}
}
