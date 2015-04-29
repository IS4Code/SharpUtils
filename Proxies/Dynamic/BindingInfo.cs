/* Date: 24.4.2015, Time: 15:30 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using IllidanS4.SharpUtils.Reflection;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// Serializable equivalent of <see cref="System.Runtime.CompilerServices.CallSiteBinder"/>.
	/// </summary>
	[Serializable]
	public class BindingInfo
	{
		public BinderTypeEnum BinderType{get; private set;}
		
		public Type ReturnType{get; private set;}
		public CSharpBinderFlags Flags{get; private set;}
		public Type Context{get; private set;}
		public ExpressionType Operation{get; private set;}
		public Type Type{get; private set;}
		public bool Explicit{get; private set;}
		public bool IgnoreCase{get; private set;}
		public string Name{get; private set;}
		public Type[] TypeArguments{get; private set;}
		public CSharpArgumentInfoFlags[] ArgumentFlags{get; private set;}
		public string[] ArgumentNames{get; private set;}
		
		public enum BinderTypeEnum
		{
			BinaryOperation,
			Convert,
			GetIndex,
			GetMember,
			Invoke,
			InvokeMember,
			SetIndex,
			SetMember
		}
		
		public BindingInfo(CallSiteBinder binder)
		{
			Context = binder.GetCallingContext();
			var argInfo = binder.GetArgumentInfo();
			if(argInfo != null)
			{
				ArgumentFlags = argInfo.Select(a => a.GetFlags()).ToArray();
				ArgumentNames = argInfo.Select(a => a.GetName()).ToArray();
			}else{
				ArgumentFlags = new CSharpArgumentInfoFlags[0];
				ArgumentNames = new string[0];
			}
			Flags = binder.GetBinderFlags();
			
			var bo = binder as BinaryOperationBinder;
			if(bo != null)
			{
				BinderType = BinderTypeEnum.BinaryOperation;
				ReturnType = bo.ReturnType;
				Operation = bo.Operation;
				return;
			}
			var c = binder as ConvertBinder;
			if(c != null)
			{
				BinderType = BinderTypeEnum.Convert;
				ReturnType = c.ReturnType;
				Type = c.Type;
				Explicit = c.Explicit;
				return;
			}
			var gi = binder as GetIndexBinder;
			if(gi != null)
			{
				BinderType = BinderTypeEnum.GetIndex;
				//CallInfo = gi.CallInfo;
				ReturnType = gi.ReturnType;
				return;
			}
			var gm = binder as GetMemberBinder;
			if(gm != null)
			{
				BinderType = BinderTypeEnum.GetMember;
				IgnoreCase = gm.IgnoreCase;
				Name = gm.Name;
				ReturnType = gm.ReturnType;
				return;
			}
			var i = binder as InvokeBinder;
			if(i != null)
			{
				BinderType = BinderTypeEnum.Invoke;
				//CallInfo = i.CallInfo;
				return;
			}
			var im = binder as InvokeMemberBinder;
			if(im != null)
			{
				BinderType = BinderTypeEnum.InvokeMember;
				//CallInfo = im.CallInfo;
				IgnoreCase = im.IgnoreCase;
				Name = im.Name;
				ReturnType = im.ReturnType;
				TypeArguments = im.GetTypeArguments().ToArray();
				return;
			}
			var si = binder as SetIndexBinder;
			if(si != null)
			{
				BinderType = BinderTypeEnum.SetIndex;
				//CallInfo = si.CallInfo;
				ReturnType = si.ReturnType;
				return;
			}
			var sm = binder as SetMemberBinder;
			if(sm != null)
			{
				BinderType = BinderTypeEnum.SetMember;
				IgnoreCase = sm.IgnoreCase;
				Name = sm.Name;
				ReturnType = sm.ReturnType;
				return;
			}
		}
		
		public CallSiteBinder CreateBinder()
		{
			IEnumerable<CSharpArgumentInfo> args = ArgumentFlags.Zip(ArgumentNames, (f,n)=>CSharpArgumentInfo.Create(f,n));
			switch(BinderType)
			{
				case BinderTypeEnum.BinaryOperation:
					return Binder.BinaryOperation(Flags, Operation, Context, args);
				case BinderTypeEnum.Convert:
					return Binder.Convert(Flags, Type, Context);
				case BinderTypeEnum.GetIndex:
					return Binder.GetIndex(Flags, Context, args);
				case BinderTypeEnum.GetMember:
					return Binder.GetMember(Flags, Name, Context, args);
				case BinderTypeEnum.Invoke:
					return Binder.Invoke(Flags, Context, args);
				case BinderTypeEnum.InvokeMember:
					return Binder.InvokeMember(Flags, Name, TypeArguments, Context, args);
				case BinderTypeEnum.SetIndex:
					return Binder.SetIndex(Flags, Context, args);
				case BinderTypeEnum.SetMember:
					return Binder.SetMember(Flags, Name, Context, args);
				default:
					return null;
			}
		}
	}
}
