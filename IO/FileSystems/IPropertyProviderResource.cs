/* Date: 22.9.2017, Time: 12:55 */
using System;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public interface IPropertyProviderResource<TProperty>
	{
		T GetProperty<T>(TProperty property);
	}
}
