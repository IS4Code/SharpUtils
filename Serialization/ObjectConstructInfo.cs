/* Date: 19.4.2015, Time: 15:54 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace IllidanS4.SharpUtils.Serialization
{
	[Serializable]
	public class ObjectConstructInfo
	{
		private static readonly ObjectIDGenerator idGen = new ObjectIDGenerator();
		
		public Type Type{get; private set;}
		public ReadOnlyCollection<FieldInfo> Fields{get; private set;}
		public ReadOnlyCollection<object> Data{get; private set;}
		public long Id{get; private set;}
		
		private Dictionary<long,ObjectConstructInfo> objCache = new Dictionary<long,ObjectConstructInfo>();
		
		public ObjectConstructInfo(object obj)
		{
			Init(obj);
		}
		
		private ObjectConstructInfo(object obj, Dictionary<long,ObjectConstructInfo> cache)
		{
			objCache = cache;
			Init(obj);
		}
		
		private void Init(object obj)
		{
			bool _;
			Id = idGen.GetId(obj, out _);
			objCache[Id] = this;
			Type = obj.GetType();
			var fields = Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			Fields = Array.AsReadOnly(fields);
			object[] data = new object[fields.Length];
			for(int i = 0; i < fields.Length; i++)
			{
				var fi = fields[i];
				object val = fi.GetValue(obj);
				Type t = fi.FieldType;
				if(val == null || t.IsPrimitive || (val is MemberInfo && t.IsSerializable))
				{
					data[i] = val;
				}else if(t.IsArray && !t.GetElementType().IsPrimitive)
				{
					Array orig = (Array)val;
					ObjectConstructInfo[] arr = new ObjectConstructInfo[orig.Length];
					for(int j = 0; j < arr.Length; j++)
					{
						object aval = orig.GetValue(j);
						if(aval != null)
							arr[j] = new ObjectConstructInfo(aval, objCache);
					}
					data[i] = arr;
				}else{
					ObjectConstructInfo oci;
					if(objCache.TryGetValue(idGen.GetId(val, out _), out oci))
					{
						data[i] = oci;
					}else{
						data[i] = new ObjectConstructInfo(val, objCache);
					}
				}
			}
			Data = Array.AsReadOnly(data);
		}
		
		public ObjectConstructInfo(Type type, FieldInfo[] fields, object[] data)
		{
			Type = type;
			Fields = Array.AsReadOnly(fields);
			Data = Array.AsReadOnly(data);
		}
		
		public object Construct()
		{
			Console.WriteLine(Type);
			object inst = FormatterServices.GetUninitializedObject(Type);
			for(int i = 0; i < Fields.Count; i++)
			{
				var fi = Fields[i];
				object val = Data[i];
				var oci = val as ObjectConstructInfo;
				if(oci != null) val = oci.Construct();
				var arr = val as ObjectConstructInfo[];
				if(arr != null)
				{
					Type t = fi.FieldType.GetElementType();
					Array aval = Array.CreateInstance(t, arr.Length);
					for(int j = 0; j < arr.Length; j++)
					{
						var aoci = arr[j];
						if(aoci != null)
							aval.SetValue(aoci.Construct(), j);
					}
				}
				fi.SetValue(inst, val);
			}
			return inst;
		}
	}
}
