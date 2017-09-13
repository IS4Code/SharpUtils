SharpUtils 2012-2017
==========

Behold, you are to visit my library full of .NET hacks, better prepare for it. You'll see lots of weird, but mostly working code. It heavily uses reflection and memory hacking to access parts of .NET that are normally inaccessible. I give you no warranty that it won't do something harmful, but I've tried to make it as much safe as possible. 
If you see some error in this code, feel free to commit a fix!  
You are free to download parts of this library and use them in your code, but please don't claim them as your own and remember who the original author was. 

## Namespaces

### Accessing
Types generalizing the concept of accessors - objects that work as generic references.
The basic interface is *IStorageAccessor*, from which all other types derive.

### Collections
Various types of collections, async collections, and immutable collection wrappers.

### COM
Some interfaces and marshallers that make working with COM easier. The types in this namespace include mainly Shell32 COM types, *IDispatch* and *IStream* wrappers.

### Coroutines
Concept of Lua-like coroutines.

### Globalization
Methods to work with application languages.

### Interop
Lots of tools to help with C interop and P/Invoke. Worth seeing. Many of the classes in this namespace work with varargs method, raw pointers and typed references.
The most interesting types in this namespace is *FarPtr* - a struct generalizing the concept of pointers, but between processes or even computers (based on remoting).
The other interesting class in this namespace is *Win32Control* - a proxy replacer (more in *Proxies*) that is used to represent *any* system window or control as an object inheriting from the *Control* class, implementing all its properties.

### IO
System-related classes working with files, pipes, and directories. An interesting concept introduced in this namespace is the universal resource system, represented by the *ResourceInfo* class, which is able to represent any resource from a drive file, special system folder, HTTP or FTP resource, or just a data URI.

### Metadata
Mostly attributes used for various purposes, like representing a method that never returns, a boxed value type, or that a method is unsafe or throws an exception.

### Numerics
Bit arithmetics on float. An interesting (but not too pratical) concept is the FloatPtr struct, with size varying between 32-bit and 64-bit based on the system native integer size.

### ObjectModel
Basic implementation of the types specified ECMA TR/89 standard, but missing from .NET.

### Patterns
A concept porting interfaces to static classes. A common pattern is the *TryParse* pattern, for example, which is also present in this namespace (as *PParseable*).

### Proxies
A proxy is an intermediate class passing invokes from one object to another.
The *Dynamic* sub-namespace contains classes related to remoting, used to pass tasks, dynamic objects, and generic object handles.
The other half of this namespace is *Replacers*. Certain types in .NET inherit from *MarshalByRefObject*, and these types can pratically all be "overridden" using a *RealProxy* to intercept calls to their methods. A replacer is an interface and its base class, representing a type to "replace" with its own implementation.

### Reflection
The most extensive and diverse part of this library. There are types to emit extended type and method signatures, using required and optional type modifiers, and a whole new type system using the *TypeSupport.TypeConstruct* base class. There are also models representing the type system in C# and C++.
There is also a class representing the *whole* .NET type system, and by that I really mean *all types* in .NET (well all types usable in the actual app-domain). The *TypeSystem* class can be used to enumerate a portion of types in the type system, including all arrays, pointers, by-refs and generic type instantiations.

### Sequences
A sequence is an enumerable specifying its finiteness. This assumes that the finiteness of a sequence is known before its creation and is deterministic and consistent. There are many LINQ-like methods, working mostly with infinite sequences.

### Serialization
Mostly memory-based serialization, using raw values.

### Text
The *StringChunk* struct is used to efficiently work with strings, and create substrings without introducing a new string instance. The *StringReference* class represents a union of a string and *StringBuilder*, an abstract mutable string.

### Threads
Tools related to threads and tasks. 

### Unsafe
The most evilest of all evils, the unsafest of all the unsafe. The wonders of the CLR and the black magic of .NET.
While there are some basic unsafe types (like the *VariadicUnion* containing all simple types in a memory union), there are also types that break all the laws of C# and .NET.
The *Chameleon* type changes its type when requested. The *ObjectHandle* class allows you to create and instantiate an object not on the managed heap, but in the unmanaged memory instead. The *UnsafeTools* class contains all tools and hacks to work with the CLR, obtaining various sizes of types, changin types of references, converting between pointers etc.
The *Experimental.PinHandle* class can be used to pin an object or a reference in the memory, so its location won't be moved. This works on all types (even when *GCHandle* doesn't work), and doesn't require lambdas. The cost is a thread for each pin handle (the thread is suspended but still preventing the reference from being moved).
The *ByValArray* struct is one of the recent additions to the namespace, and represents something utterly mind-blowing - a mutable dynamically-sized array with value type semantics. The basis for this struct is the *ReferenceStorage* class, which allows value types to store data based on their "identity" (location in memory). By allowing this, a struct can basically determine whether its location in memory was changed, and modify its data accordingly (clone the internal array for example).

From my excessive description of the last namespace, you can see which one is the most favourite of mine, but don't worry, the very dangerous things are all kept to the *Unsafe* namespace and don't appear outside of it.