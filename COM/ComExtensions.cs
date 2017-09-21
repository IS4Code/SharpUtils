/* Date: 5.9.2017, Time: 14:39 */
using System;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;

namespace IllidanS4.SharpUtils.Com
{
	[CLSCompliant(false)]
	public static class ComExtensions
	{
		[DebuggerStepThrough]
		public static T BindToHandler<T>(this IShellItem psi, IBindCtx pbc, Guid bhid) where T : class
		{
			return (T)psi.BindToHandler(pbc, bhid, typeof(T).GUID);
		}
		
		[DebuggerStepThrough]
		public static T BindToHandler<T>(this IShellItem2 psi, IBindCtx pbc, Guid bhid) where T : class
		{
			return (T)psi.BindToHandler(pbc, bhid, typeof(T).GUID);
		}
		
		[DebuggerStepThrough]
        public static T GetPropertyStore<T>(this IShellItem2 psi, GETPROPERTYSTOREFLAGS flags) where T : class
        {
        	return (T)psi.GetPropertyStore(flags, typeof(T).GUID);
        }
        
		[DebuggerStepThrough]
        public static T GetPropertyStoreWithCreateObject<T>(this IShellItem2 psi, GETPROPERTYSTOREFLAGS flags, object punkCreateObject) where T : class
        {
        	return (T)psi.GetPropertyStoreWithCreateObject(flags, punkCreateObject, typeof(T).GUID);
        }
        
		[DebuggerStepThrough]
        public static T GetPropertyStoreForKeys<T>(this IShellItem2 psi, PROPERTYKEY[] rgKeys, GETPROPERTYSTOREFLAGS flags) where T : class
        {
        	return (T)psi.GetPropertyStoreForKeys(rgKeys, unchecked((uint)rgKeys.Length), flags, typeof(T).GUID);
        }
        
		[DebuggerStepThrough]
        public static T GetPropertyDescriptionList<T>(this IShellItem2 psi, PROPERTYKEY keyType) where T : class
        {
        	return (T)psi.GetPropertyDescriptionList(ref keyType, typeof(T).GUID);
        }
        
		[DebuggerStepThrough]
		public static T BindToObject<T>(this IShellFolder psf, IntPtr pidl, IBindCtx pbc) where T : class
		{
			return (T)psf.BindToObject(pidl, pbc, typeof(T).GUID);
		}
		
		[DebuggerStepThrough]
		public static T BindToStorage<T>(this IShellFolder psf, IntPtr pidl, IBindCtx pbc) where T : class
		{
			return (T)psf.BindToStorage(pidl, pbc, typeof(T).GUID);
		}
		
		[DebuggerStepThrough]
		public static T CreateViewObject<T>(this IShellFolder psf, IntPtr hwndOwner) where T : class
		{
			return (T)psf.CreateViewObject(hwndOwner, typeof(T).GUID);
		}
		
		[DebuggerStepThrough]
		public static T GetUIObjectOf<T>(this IShellFolder psf, IntPtr hwndOwner, IntPtr[] apidl, uint rgfReserved=0)
		{
			return (T)psf.GetUIObjectOf(hwndOwner, unchecked((uint)apidl.Length), apidl, typeof(T).GUID, ref rgfReserved);
		}
		
		[DebuggerStepThrough]
		public static T GetAt<T>(this IPropertyChangeArray pproparray, int iIndex) where T : class
		{
			return (T)pproparray.GetAt(iIndex, typeof(T).GUID);
		}
	}
}
