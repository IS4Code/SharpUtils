/* Date: 29.3.2015, Time: 11:48 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace IllidanS4.SharpUtils.Reflection.TypeSupport
{
	/// <summary>
	/// Represents an instance of a <see cref="TypeConstruct"/>.
	/// </summary>
	public class VirtualObjectProxy : RealProxy, ICloneable
	{
		//I have no idea what should this be used for.
		
		public TypeConstruct VirtualType{get; private set;}
		public FieldsData Fields{get; private set;}
		
		public VirtualObjectProxy(TypeConstruct type) : base(ProxyType(type))
		{
			VirtualType = type;
			Fields = new FieldsData(this);
		}
		
		protected VirtualObjectProxy(VirtualObjectProxy proxy) : base(ProxyType(proxy.VirtualType))
		{
			
		}
		
		private static Type ProxyType(TypeConstruct type)
		{
			Type under = type.UnderlyingSystemType;
			if(TypeOf<MarshalByRefObject>.TypeID.IsAssignableFrom(under))
			{
				return under;
			}else{
				return TypeOf<MarshalByRefObject>.TypeID;
			}
		}
		
		public override IMessage Invoke(IMessage msg)
		{
			IMethodCallMessage msgCall = msg as IMethodCallMessage;
			if(msgCall != null)
			{
				try{
					object[] args = msgCall.Args;
					object ret = VirtualType.InvokeMethod(msgCall.MethodBase, this.GetTransparentProxy(), args);
					return new ReturnMessage(ret, args, args.Length, msgCall.LogicalCallContext, msgCall);
				}catch(TargetInvocationException e)
				{
					return new ReturnMessage(e.InnerException, msgCall);
				}catch(Exception e)
				{
					return new ReturnMessage(e, msgCall);
				}
			}
			return null;
		}
		
		public object Clone()
		{
			return new VirtualObjectProxy(this);
		}
		
		public class FieldsData : ICloneable
		{
			private readonly Dictionary<string, object> dict = new Dictionary<string, object>();
			private readonly VirtualObjectProxy proxy;
			
			internal bool IsInit{get; private set;}
			
			public FieldsData(VirtualObjectProxy proxy)
			{
				IsInit = true;
				this.proxy = proxy;
			}
			
			protected FieldsData(FieldsData fields) : this(fields.proxy)
			{
				dict = new Dictionary<string, object>(fields.dict);
				IsInit = false;
			}
			
			internal void DoneInit()
			{
				IsInit = false;
			}
			
			private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			
			public FieldInfo this[string field]{
				get{
					return proxy.VirtualType.GetField(field, flags);
				}
			}
			
			public object this[FieldInfo field]{
				get{
					return dict[field.Name];
				}
				set{
					if(field.IsInitOnly && !IsInit)
					{
						throw new FieldAccessException(Extensions.GetResourceString("FieldAccess_InitOnly"));
					}
					dict[field.Name] = value;
				}
			}
		
			public object Clone()
			{
				return new FieldsData(this);
			}
		}
	}
}
