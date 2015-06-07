/* Date: 29.4.2015, Time: 15:01 */
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// A real proxy connected to a <see cref="StaticAdapter"/> that allows passing messages and marshalling arguments from the transparent proxy.
	/// </summary>
	public class MarshallerProxy : RealProxy, IRemotingTypeInfo, IAdapterProxy
	{
		public StaticAdapter Adapter{get; private set;}
		public Type ProxyType{get{return Adapter.ProxyType;}}
		public string ProxyTypeString{get{return Adapter.ProxyTypeString;}}
		
		public MarshallerProxy(AppDomain targetDomain, ObjectHandle objHandle) : this(CreateAdapter(targetDomain, objHandle))
		{
			
		}
		
		public MarshallerProxy(StaticAdapter adapter) : base(GetMarshalByRefType(adapter.ProxyType))
		{
			Adapter = adapter;
		}
		
		private static Type GetMarshalByRefType(Type baseType)
		{
			if(TypeOf<MarshalByRefObject>.TypeID.IsAssignableFrom(baseType))
			{
				return baseType;
			}else{
				return TypeOf<MarshalByRefObject>.TypeID;
			}
		}
		
		private static StaticAdapter CreateAdapter(AppDomain targetDomain, ObjectHandle objHandle)
		{
			Type tAdapter = TypeOf<StaticAdapter>.TypeID;
			return (StaticAdapter)targetDomain.CreateInstanceAndUnwrap(tAdapter.Assembly.FullName, tAdapter.FullName, false, 0, null, new[]{objHandle}, null, null);
		}
		
		public override IMessage Invoke(IMessage msg)
		{
			IMethodCallMessage msgCall = (IMethodCallMessage)msg;
			var oArgs = msgCall.Args;
			var args = AdapterTools.Marshal(oArgs);
			try{
				var ret = Adapter.InvokeMember(msgCall.MethodBase, ref args);
				oArgs = AdapterTools.Unmarshal(args);
				return new ReturnMessage(AdapterTools.Unmarshal(ret), oArgs, oArgs.Length, msgCall.LogicalCallContext, msgCall);
			}catch(TargetInvocationException e)
			{
				return new ReturnMessage(e.InnerException, msgCall);
			}catch(Exception e)
			{
				return new ReturnMessage(e, msgCall);
			}
		}
		
		public string TypeName{
			get; set;
		}
		
		public bool CanCastTo(Type fromType, object o)
		{
			return fromType.IsAssignableFrom(ProxyType);
		}
	}
}
