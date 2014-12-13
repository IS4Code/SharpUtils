using System;
using System.Collections;

namespace IllidanS4.SharpUtils.Templates
{
	public interface IStaticEqualityComparer
	{
		bool Equals(object a, object b);
	}
}
