/* Date: 10.2.2017, Time: 20:07 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace IllidanS4.SharpUtils.Interop
{
	public class MarshalInfo
	{
		private static readonly Type MarshalAsType = typeof(MarshalAsAttribute);
		private static readonly ConstructorInfo MarshalAsCtor = MarshalAsType.GetConstructor(new[]{typeof(UnmanagedType)});
		
		public VarEnum SafeArraySubType{get; private set;}
		public Type SafeArrayUserDefinedSubType{get; private set;}
		public int IidParameterIndex{get; private set;}
		public UnmanagedType ArraySubType{get; private set;}
		public short SizeParamIndex{get; private set;}
		public int SizeConst{get; private set;}
		public Type MarshalTypeRef{get; private set;}
		public string MarshalCookie{get; private set;}
		public UnmanagedType UnmanagedType{get; private set;}
		public Guid IidGuid{get; private set;}
		
		public MarshalInfo(MarshalAsAttribute marshal, Guid iid)
		{
			SafeArraySubType = marshal.SafeArraySubType;
			SafeArrayUserDefinedSubType = marshal.SafeArrayUserDefinedSubType;
			IidParameterIndex = marshal.IidParameterIndex;
			ArraySubType = marshal.ArraySubType;
			SizeParamIndex = marshal.SizeParamIndex;
			SizeConst = marshal.SizeConst;
			MarshalTypeRef = marshal.MarshalTypeRef;
			if(MarshalTypeRef == null && marshal.MarshalType != null)
			{
				MarshalTypeRef = Type.GetType(marshal.MarshalType);
			}
			MarshalCookie = marshal.MarshalCookie;
			UnmanagedType = marshal.Value;
			IidGuid = iid;
		}
		
		[Obsolete]
		public MarshalInfo(UnmanagedMarshal marshal)
		{
			UnmanagedType = marshal.GetUnmanagedType;
			ArraySubType = marshal.BaseType;
			SizeConst = marshal.ElementCount;
			IidGuid = marshal.IIDGuid;
		}
		
		public IEnumerable<CustomAttributeBuilder> CreateBuilders()
		{
			var namedProperties = new List<PropertyInfo>();
			var propertyValues = new List<object>();
			
			if(SafeArraySubType != 0)
			{
				namedProperties.Add(MarshalAsType.GetProperty("SafeArraySubType"));
				propertyValues.Add(SafeArraySubType);
			}
			if(SafeArrayUserDefinedSubType != null)
			{
				namedProperties.Add(MarshalAsType.GetProperty("SafeArrayUserDefinedSubType"));
				propertyValues.Add(SafeArrayUserDefinedSubType);
			}
			if(IidParameterIndex != 0)
			{
				namedProperties.Add(MarshalAsType.GetProperty("IidParameterIndex"));
				propertyValues.Add(IidParameterIndex);
			}
			if(ArraySubType != 0)
			{
				namedProperties.Add(MarshalAsType.GetProperty("ArraySubType"));
				propertyValues.Add(ArraySubType);
			}
			if(SizeParamIndex != 0)
			{
				namedProperties.Add(MarshalAsType.GetProperty("SizeParamIndex"));
				propertyValues.Add(SizeParamIndex);
			}
			if(SizeConst != 0)
			{
				namedProperties.Add(MarshalAsType.GetProperty("SizeConst"));
				propertyValues.Add(SizeConst);
			}
			if(MarshalTypeRef != null)
			{
				namedProperties.Add(MarshalAsType.GetProperty("MarshalTypeRef"));
				propertyValues.Add(MarshalTypeRef);
			}
			if(MarshalCookie != null)
			{
				namedProperties.Add(MarshalAsType.GetProperty("MarshalCookie"));
				propertyValues.Add(MarshalCookie);
			}
			
			var mbuilder = new CustomAttributeBuilder(MarshalAsCtor, new object[]{UnmanagedType}, namedProperties.ToArray(), propertyValues.ToArray());
			yield return mbuilder;
			
			if(IidGuid != Guid.Empty)
			{
				yield return new CustomAttributeBuilder(typeof(GuidAttribute).GetConstructors()[0], new[]{IidGuid.ToString("D")});
			}
		}
		
		public void Apply(ParameterBuilder param)
		{
			foreach(var builder in CreateBuilders())
			{
				param.SetCustomAttribute(builder);
			}
		}
		
		public void Apply(FieldBuilder field)
		{
			foreach(var builder in CreateBuilders())
			{
				field.SetCustomAttribute(builder);
			}
		}
	}
}
