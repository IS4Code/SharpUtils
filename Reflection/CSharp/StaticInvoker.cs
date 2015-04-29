/* Date: 2.4.2015, Time: 19:32 */
using System;
using System.Dynamic;
using System.Reflection;

namespace IllidanS4.SharpUtils.Reflection.CSharp
{
	public class StaticInvoker : DynamicObject
	{
		readonly BindingFlags flags;
		readonly Type type;
		
		public StaticInvoker(Type staticType) : this(staticType, false)
		{
			
		}
		
		public StaticInvoker(Type staticType, bool nonPublic)
		{
			type = staticType;
			flags = BindingFlags.Static | BindingFlags.Public | (nonPublic ? BindingFlags.NonPublic : 0);
		}
		
		public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
		{
			try{
				result = Activator.CreateInstance(type, args);
				return true;
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}catch(MissingMemberException)
			{
				result = null;
				return false;
			}
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			try{
				result = type.InvokeMember(binder.Name, flags | BindingFlags.GetField | BindingFlags.GetProperty, null, null, null);
				return true;
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}catch(MissingMemberException)
			{
				result = null;
				return false;
			}
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			try{
				type.InvokeMember(binder.Name, flags | BindingFlags.SetField | BindingFlags.SetProperty, null, null, new[]{value});
				return true;
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}catch(MissingMemberException)
			{
				return false;
			}
		}
		
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			try{
				result = type.InvokeMember(binder.Name, flags | BindingFlags.InvokeMethod, null, null, args);
				return true;
			}catch(TargetInvocationException e)
			{
				throw e.InnerException;
			}catch(MissingMemberException)
			{
				result = null;
				return false;
			}
		}
	}
}
