/* Date: 24.4.2015, Time: 15:32 */
using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using Microsoft.CSharp.RuntimeBinder;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// A dynamic proxy that connects to a <see cref="StaticAdapter"/> and passes dynamic calls to it in form of method invokes.
	/// </summary>
	public class StaticProxy : DynamicObject, IAdapterProxy
	{
		public StaticAdapter Adapter{get; private set;}
		public Type ProxyType{get{return Adapter.ProxyType;}}
		public string ProxyTypeString{get{return Adapter.ProxyTypeString;}}
		
		public StaticProxy(AppDomain targetDomain, ObjectHandle objHandle) : this(CreateAdapter(targetDomain, objHandle))
		{
			
		}
		
		public StaticProxy(StaticAdapter adapter)
		{
			Adapter = adapter;
		}
		
		private static StaticAdapter CreateAdapter(AppDomain targetDomain, ObjectHandle objHandle)
		{
			Type tAdapter = TypeOf<StaticAdapter>.TypeID;
			return (StaticAdapter)targetDomain.CreateInstanceAndUnwrap(tAdapter.Assembly.FullName, tAdapter.FullName, false, 0, null, new[]{objHandle}, null, null);
		}
		
		private bool Try(CallSiteBinder binder, string member, object[] args, out object result)
		{
			ObjectTypeHandle[] argHandles = AdapterTools.Marshal(args);
			BindingInfo binding = new BindingInfo(binder);
			const CSharpArgumentInfoFlags flagsRefOrOut = CSharpArgumentInfoFlags.IsRef | CSharpArgumentInfoFlags.IsOut;
			ObjectTypeHandle resHandle;
			try{
				resHandle = Adapter.InvokeMember(member, ref argHandles, binding.ArgumentFlags.Skip(1).Select(f => (f&flagsRefOrOut)!=0).ToArray());
			}catch(MissingMethodException)
			{
				result = null;
				return false;
			}
			AdapterTools.Unmarshal(argHandles).CopyTo(args, 0);
			result = AdapterTools.Unmarshal(resHandle);
			return true;
		}
		
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			return Try(binder, binder.Name, args, out result);
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return Try(binder, "get_"+binder.Name, new object[0], out result);
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			object _;
			return Try(binder, "set_"+binder.Name, new object[]{value}, out _);
		}
		
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
		{
			return Try(binder, "get_Item", indexes, out result);
		}
		
		public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
		{
			object _;
			return Try(binder, "set_Item", indexes.Concat(new[]{value}).ToArray(), out _);
		}
		
		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			if(binder.Type == Adapter.ProxyType)
			{
				result = Adapter.Unwrap();
				return true;
			}else{
				return Try(binder, binder.Explicit?"op_Explicit":"op_Implicit", new object[0], out result);
			}
		}
		
		/*protected class MetaObject : DynamicMetaObject
		{
			public StaticProxy Proxy{get; private set;}
			
			public MetaObject(StaticProxy proxy, Expression parameter) : base(parameter, BindingRestrictions.Empty, proxy)
			{
				Proxy = proxy;
			}
			
			public virtual DynamicMetaObject Bind(CallSiteBinder binder, string name, params DynamicMetaObject[] args)
			{
				var del = (Delegate)Proxy.Adapter.BindInvokeMember(AdapterProvider.Instance, name, args.Select(a => a.LimitType).ToArray()).Unwrap();
				return new DynamicMetaObject(
					Expression.Invoke(Expression.Constant(del), args.Select(a => a.Expression)),
					BindingRestrictions.GetTypeRestriction(Expression, LimitType)
				);
			}
			
			public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
			{
				return Bind(binder, "get_Item", indexes);
			}
			
			public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
			{
				return Bind(binder, "get_Item", indexes.Concat(new[]{value}).ToArray());
			}
			
			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				return Bind(binder, "get_"+binder.Name);
			}
			
			public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
			{
				return Bind(binder, "set_"+binder.Name, value);
			}
			
			public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
			{
				return Bind(binder, "Invoke", args);
			}
			
			public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
			{
				return Bind(binder, binder.Name, args);
			}
		}
		
		public virtual DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return new MetaObject(this, parameter);
		}*/
	}
}
