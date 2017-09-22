/* Date: 22.9.2017, Time: 12:27 */
using System;

namespace IllidanS4.SharpUtils.IO.FileSystems
{
	public interface IPropertyProvider<TProperty>
	{
		T GetProperty<T>(Uri uri, TProperty property);
	}
}
