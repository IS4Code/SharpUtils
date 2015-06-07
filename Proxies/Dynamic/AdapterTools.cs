/* Date: 24.4.2015, Time: 15:38 */
using System;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading.Tasks;

namespace IllidanS4.SharpUtils.Proxies.Dynamic
{
	/// <summary>
	/// Collection of marshalling tools for adapters and proxies.
	/// </summary>
	public static class AdapterTools
	{
		public static object Call(StaticAdapter adapter, MethodBase method, params object[] args)
		{
			ObjectTypeHandle[] argHandles = Marshal(args);
			ObjectTypeHandle resHandle = adapter.InvokeMember(method, ref argHandles);
			Unmarshal(argHandles).CopyTo(args, 0);
			return Unmarshal(resHandle);
		}
		
		public static ObjectTypeHandle[] Marshal(params object[] args)
		{
			ObjectTypeHandle[] argHandles = new ObjectTypeHandle[args.Length];
			for(int i = 0; i < args.Length; i++)
			{
				argHandles[i] = Marshal(args[i]);
			}
			return argHandles;
		}
		
		public static ObjectTypeHandle Marshal(object arg)
		{
			if(arg == null)
			{
				return null;
			}
			ObjectTypeHandle handle = arg as ObjectTypeHandle;
			if(handle != null)
			{
				return handle;
			}
			Type t = arg.GetType();
			if(t.IsDefined(TypeOf<MarshalByAddressAttribute>.TypeID, false))
			{
				return new UnsafeHandle(arg);
			}
			Delegate del = arg as Delegate;
			if(del != null)
			{
				return new DelegateAdapter(del);
			}
			Task task = arg as Task;
			if(task != null)
			{
				return new TaskAdapter(task);
			}
			return new ObjectTypeHandle(arg);
		}
		
		public static object[] Unmarshal(params ObjectHandle[] args)
		{
			object[] oArgs = new object[args.Length];
			for(int i = 0; i < args.Length; i++)
			{
				oArgs[i] = Unmarshal(args[i]);
			}
			return oArgs;
		}
		
		public static object Unmarshal(ObjectHandle arg)
		{
			if(arg == null)
			{
				return null;
			}else{
				DelegateAdapter delAd = arg as DelegateAdapter;
				if(delAd != null)
				{
					return DelegateAdapter.CreateDelegate(delAd);
				}
				TaskAdapter taskAd = arg as TaskAdapter;
				if(taskAd != null)
				{
					return TaskAdapter.CreateTask(taskAd);
				}
				DynamicAdapter dynAd = arg as DynamicAdapter;
				if(dynAd != null)
				{
					return DynamicAdapter.CreateProxy(dynAd);
				}
				StaticAdapter statAd = arg as StaticAdapter;
				if(statAd != null)
				{
					return StaticAdapter.CreateProxy(statAd);
				}
				UnsafeHandle uns = arg as UnsafeHandle;
				if(uns != null)
				{
					return UnsafeHandle.GetObject(uns);
				}
				return arg.Unwrap();
			}
		}
	}
}
