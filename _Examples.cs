//This code isn't meant to be compiled, it's just a list of examples that I've been lazy to put to their appropriate classes.
#if false
/* Extended reflection */
	var asb = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("TestAsm"), AssemblyBuilderAccess.Save);
	var modb = asb.DefineDynamicModule("TestAsm.dll");
	var typ = modb.DefineType("TestType");
	var sig = new MethodSignature(typeof(int).MakeModifiedType(typeof(int[])));
	var mb = typ.DefineMethod("TestMethod", MethodAttributes.Public | MethodAttributes.Static, sig);
	var il = mb.GetILGenerator();
	il.Emit(OpCodes.Ret);
	typ.DefineFieldExtended("fld", typeof(int).MakeModifiedType(new SignatureElementType(CorElementType.Sentinel)), FieldAttributes.Public);
	typ.DefineField("fld", typeof(int), null, new[]{typeof(int?)}, FieldAttributes.Public);
	typ.CreateType();
	asb.Save("TestAsm.dll");
#endif