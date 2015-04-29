/* Date: 14.2.2015, Time: 12:35 */
using System;
using System.Security.AccessControl;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace IllidanS4.SharpUtils.Proxies.Replacers
{
	public abstract class RegistryKeyBase : ProxyImplementation<RegistryKey, IRegistryKey>, IRegistryKey
	{
		protected RegistryKeyPermissionCheck CheckMode{get; private set;}
		
		protected RegistryKeyBase() : this(RegistryKeyPermissionCheck.Default)
		{
			
		}
		
		protected RegistryKeyBase(RegistryKeyPermissionCheck permissionCheck)
		{
			CheckMode = permissionCheck;
		}
		
		public virtual SafeRegistryHandle Handle
		{
			get{
				throw new NotSupportedException();
			}
		}
		protected virtual void Dispose(bool disposing)
		{
			
		}
		public virtual void Close()
		{
			this.Dispose(true);
		}
		public virtual void Dispose()
		{
			this.Dispose(true);
		}
		public virtual RegistryKey CreateSubKey(string subkey)
		{
			return this.CreateSubKey(subkey, this.CheckMode);
		}
		public virtual RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.CreateSubKey(subkey, permissionCheck, RegistryOptions.None, null);
		}
		public virtual RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions options)
		{
			return this.CreateSubKey(subkey, permissionCheck, options, null);
		}
		public virtual RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			return this.CreateSubKey(subkey, permissionCheck, RegistryOptions.None, registrySecurity);
		}
		public abstract RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity);
		public virtual void DeleteSubKey(string subkey)
		{
			this.DeleteSubKey(subkey, true);
		}
		public abstract void DeleteSubKey(string subkey, bool throwOnMissingSubKey);
		
		public virtual void DeleteSubKeyTree(string subkey)
		{
			this.DeleteSubKeyTree(subkey, true);
		}
		public abstract void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey);
		
		public virtual void DeleteValue(string name)
		{
			this.DeleteValue(name, true);
		}
		public abstract void DeleteValue(string name, bool throwOnMissingValue);
		
		public virtual RegistryKey OpenSubKey(string name)
		{
			return this.OpenSubKey(name, false);
		}
		public virtual RegistryKey OpenSubKey(string name, bool writable)
		{
			return this.OpenSubKey(name, RegistryKeyPermissionCheck.Default, GetRegistryKeyAccess(writable));
		}
		
		public virtual RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			return this.OpenSubKey(name, permissionCheck, GetRegistryKeyAccess(permissionCheck));
		}
		public abstract RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights);
		
		protected static RegistryRights GetRegistryKeyAccess(bool isWritable)
		{
			if(!isWritable)
			{
				return RegistryRights.ReadKey;
			}else{
				return RegistryRights.ReadKey | RegistryRights.WriteKey;
			}
		}
		protected static RegistryRights GetRegistryKeyAccess(RegistryKeyPermissionCheck mode)
		{
			switch (mode)
			{
				case RegistryKeyPermissionCheck.Default:
				case RegistryKeyPermissionCheck.ReadSubTree:
					return RegistryRights.ReadKey;
				case RegistryKeyPermissionCheck.ReadWriteSubTree:
					return RegistryRights.ReadKey | RegistryRights.WriteKey;
				default:
					return default(RegistryRights);
			}
		}
		
		public virtual object GetValue(string name)
		{
			return this.GetValue(name, null);
		}
		public virtual object GetValue(string name, object defaultValue)
		{
			return this.GetValue(name, defaultValue, RegistryValueOptions.None);
		}
		public abstract object GetValue(string name, object defaultValue, RegistryValueOptions options);
		
		public abstract RegistryValueKind GetValueKind(string name);
		
		public virtual void SetValue(string name, object value)
		{
			this.SetValue(name, value, RegistryValueKind.Unknown);
		}
		public abstract void SetValue(string name, object value, RegistryValueKind valueKind);
		
		public override string ToString()
		{
			return Name;
		}
		
		public virtual RegistrySecurity GetAccessControl()
		{
			return GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}
		public abstract RegistrySecurity GetAccessControl(AccessControlSections includeSections);
		
		public abstract void SetAccessControl(RegistrySecurity registrySecurity);
		
		public abstract int SubKeyCount{get;}
		
		public abstract RegistryView View{get;}
		
		public abstract int ValueCount{get;}
		
		public abstract string Name{get;}
		
		public abstract void Flush();
		
		public abstract string[] GetSubKeyNames();
		
		public abstract string[] GetValueNames();
		
		RegistryKey IRegistryKey.InternalOpenSubKey(string name, bool writable)
		{
			return OpenSubKey(name, writable);
		}
		
		int IRegistryKey.InternalSubKeyCount()
		{
			return SubKeyCount;
		}
		
		string[] IRegistryKey.InternalGetSubKeyNames()
		{
			return GetSubKeyNames();
		}
		
		int IRegistryKey.InternalValueCount()
		{
			return ValueCount;
		}
		
		object IRegistryKey.InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity)
		{
			return GetValue(name, defaultValue, doNotExpand ? RegistryValueOptions.DoNotExpandEnvironmentNames : 0);
		}
		
		void IRegistryKey.Win32Error(int errorCode, string str)
		{
			throw new NotImplementedException();
		}
	}
}
