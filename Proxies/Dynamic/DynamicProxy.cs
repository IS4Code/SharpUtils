/* Date: 24.4.2015, Time: 15:32 */
using System;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// A proxy that connects to the <see cref="DynamicAdapter"/>, passing dynamic calls.
	/// </summary>
	public class DynamicProxy : StaticProxy
	{
		public new DynamicAdapter Adapter{get; private set;}
		
		public DynamicProxy(AppDomain targetDomain, ObjectHandle objHandle) : this(CreateAdapter(targetDomain, objHandle))
		{
			
		}
		
		public DynamicProxy(DynamicAdapter adapter) : base(adapter)
		{
			Adapter = adapter;
		}
		
		private static DynamicAdapter CreateAdapter(AppDomain targetDomain, ObjectHandle objHandle)
		{
			Type tAdapter = TypeOf<DynamicAdapter>.TypeID;
			return (DynamicAdapter)targetDomain.CreateInstanceAndUnwrap(tAdapter.Assembly.FullName, tAdapter.FullName, false, 0, null, new[]{objHandle}, null, null);
		}
		
		private bool Try(CallSiteBinder binder, object[] args, out object result)
		{
			ObjectHandle[] argHandles = AdapterTools.Marshal(args);
			var binding = new BindingInfo(binder);
			var resHandle = Adapter.InvokeMember(binding, ref argHandles);
			AdapterTools.Unmarshal(argHandles).CopyTo(args, 0);
			result = AdapterTools.Unmarshal(resHandle);
			return true;
		}
		
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			return Try(binder, args, out result);
		}
		
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return Try(binder, new object[0], out result);
		}
		
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			object _;
			return Try(binder, new[]{value}, out _);
		}
	}
}
