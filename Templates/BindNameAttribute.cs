﻿using System;

namespace IllidanS4.SharpUtils.Templates
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class BindNameAttribute : Attribute
	{
		public string Name{get; private set;}
		
		public BindNameAttribute(string name)
		{
			Name = name;
		}
	}
}
