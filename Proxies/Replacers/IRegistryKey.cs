/* Date: 14.2.2015, Time: 12:12 */
using System;
using System.Security.AccessControl;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace IllidanS4.SharpUtils.Proxies.Replacers
{
	public interface IRegistryKey : IProxyReplacer<RegistryKey, IRegistryKey>, IDisposable
	{
		int SubKeyCount{get;}
		RegistryView View{get;}
		SafeRegistryHandle Handle{get;}
		int ValueCount{get;}
		string Name{get;}
		void Close();
		void Flush();
		RegistryKey CreateSubKey(string subkey);
		RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck);
		RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions options);
		RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity);
		RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity);
		void DeleteSubKey(string subkey);
		void DeleteSubKey(string subkey, bool throwOnMissingSubKey);
		void DeleteSubKeyTree(string subkey);
		void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey);
		void DeleteValue(string name);
		void DeleteValue(string name, bool throwOnMissingValue);
		RegistryKey OpenSubKey(string name);
		RegistryKey OpenSubKey(string name, bool writable);
		RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck);
		RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights);
		string[] GetSubKeyNames();
		string[] GetValueNames();
		object GetValue(string name);
		object GetValue(string name, object defaultValue);
		object GetValue(string name, object defaultValue, RegistryValueOptions options);
		RegistryValueKind GetValueKind(string name);
		void SetValue(string name, object value);
		void SetValue(string name, object value, RegistryValueKind valueKind);
		string ToString();
		RegistrySecurity GetAccessControl();
		RegistrySecurity GetAccessControl(AccessControlSections includeSections);
		void SetAccessControl(RegistrySecurity registrySecurity);
		
		//Internal
		
		RegistryKey InternalOpenSubKey(string name, bool writable);
		int InternalSubKeyCount();
		string[] InternalGetSubKeyNames();
		int InternalValueCount();
		object InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity);
		void Win32Error(int errorCode, string str);
	}
}
