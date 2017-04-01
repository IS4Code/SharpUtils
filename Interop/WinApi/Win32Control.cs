/* Date: 5.3.2017, Time: 16:26 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IllidanS4.SharpUtils.Proxies.Replacers;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	public partial class Win32Control : ControlBase, IEquatable<Win32Control>, IWindowTarget
	{
		private IntPtr _hwnd;
		
		public override IntPtr Handle{
			get{
				return _hwnd;
			}
		}
		
		public static Control Desktop{
			get{
				return FromHandle(User32.GetDesktopWindow());
			}
		}
		
		protected Win32Control(IntPtr hwnd)
		{
			_hwnd = hwnd;
		}
		
		public static Control FromHandle(IntPtr handle)
		{
			if(handle == IntPtr.Zero) return null;
			return Control.FromHandle(handle) ?? new Win32Control(handle).GetProxy();
		}
		
		protected override Type GetControlType()
		{
			string cls = User32.GetClassName(Handle, true);
			int style = User32.GetWindowStyle(Handle);
			switch(cls)
			{
				case "Button":
					if((style & User32.BS_CHECKBOX) != 0)
						return typeof(CheckBox);
					if((style & User32.BS_RADIOBUTTON) != 0)
						return typeof(RadioButton);
					return typeof(Button);
				case "ListBox":
					return typeof(ListBox); //or CheckedListBox
				case "ComboBox":
					return typeof(ComboBox); //or DataGridViewComboBoxEditingControl
				case "Edit":
					return typeof(TextBoxBase); //TextBox, MaskedTextBox, DataGridTextBox, DataGridViewTextBoxEditingControl
				case "Static":
					return typeof(Label); //or LinkLabel
				case "MDIClient":
					return typeof(MdiClient);
				case "ScrollBar":
					if((style & User32.SBS_VERT) != 0)
						return typeof(VScrollBar);
					return typeof(HScrollBar);
					
				/*case "ComboLBox":
					return typeof(ListBox);
				case "DDEMLEvent":
					return typeof(Form);
				case "Message":
					return typeof(Form);
				case "#32768": //menu
					return typeof(Form);
				case "#32769": //desktop
					return typeof(Form);
				case "#32770": //dialog box
					return typeof(Form);
				case "#32771": //task switch window
					return typeof(Form);
				case "#32772": //icon title
					return typeof(Form);*/
				case "ConsoleWindowClass":
					return typeof(Form);
				default:
					return base.GetControlType();
			}
		}
		
		public override bool Equals(object obj)
		{
			Win32Control other = obj as Win32Control;
			if(other != null)
			{
				return this.Equals(other);
			}
			return base.Equals(obj);
		}
		
		public bool Equals(Win32Control other)
		{
			return this._hwnd == other._hwnd;
		}
		
		public override bool Equals(Control other)
		{
			var impl = GetImplementation(other) as Win32Control;
			if(impl != null)
			{
				return Equals(impl);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return Handle.GetHashCode();
		}
		
		public override string ToString()
		{
			string typename = User32.GetClassName(Handle, true);
			string classname = User32.GetClassName(Handle, false);
			if(typename == classname) return typename;
			else return typename+":"+classname;
		}
		
		public override string Text{
			get{
				int len = (int)User32.SendMessage(Handle, User32.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
				len += 2; //the null character at the end
				
				IntPtr buffer = Marshal.AllocHGlobal(len*len); //extra space
				try{
					User32.SendMessage(Handle, User32.WM_GETTEXT, (IntPtr)len, buffer);
					return Marshal.PtrToStringAuto(buffer);
				}finally{
					Marshal.FreeHGlobal(buffer);
				}
			}
			set{
				IntPtr str = Marshal.StringToHGlobalAuto(value);
				try{
					var ret = User32.SendMessage(Handle, User32.WM_SETTEXT, IntPtr.Zero, str);
					if(ret != (IntPtr)1) throw new Win32Exception();
				}finally{
					Marshal.FreeHGlobal(str);
				}
			}
		}
		
		public override Control Parent{
			get{
				IntPtr parent = User32.GetAncestor(Handle, User32.GA_PARENT);
				//if(parent == User32.GetDesktopWindow()) return null;
				return Win32Control.FromHandle(parent);
			}
			set{
				User32.SetParent(Handle, value.Handle);
			}
		}
		
		private UserData GetUserData()
		{
			IntPtr userdata = User32.GetWindowLongPtr(Handle, User32.GWL_USERDATA);
			return UserData.Load(userdata);
		}
		
		private UserData CreateUserData()
		{
			var data = GetUserData();
			if(data == null)
			{
				data = new UserData();
				User32.SetWindowLongPtr(Handle, User32.GWL_USERDATA, data.ToIntPtr());
			}
			return data;
		}
		
		public override object Tag{
			get{
				var data = GetUserData();
				if(data != null) return data.Tag;
				return null;
			}
			set{
				var data = CreateUserData();
				data.Tag = value;
			}
		}
		
		public override string Name{
			get{
				var data = GetUserData();
				if(data != null) return data.Name;
				return null;
			}
			set{
				var data = CreateUserData();
				data.Name = value;
			}
		}
		
		private sealed class UserData : IDisposable
		{
			public object Tag;
			public string Name;
			
			private readonly GCHandle Handle;
			
			public UserData()
			{
				Handle = GCHandle.Alloc(this);
			}
			
			public static UserData Load(IntPtr value)
			{
				if(value == IntPtr.Zero) return null;
				var handle = GCHandle.FromIntPtr(value);
				return (UserData)handle.Target;
			}
			
			public IntPtr ToIntPtr()
			{
				return GCHandle.ToIntPtr(Handle);
			}
			
			public void Dispose()
			{
				Handle.Free();
			}
		}
		
		public override int Width{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.right-rect.left;
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value, Height, User32.SWP_NOACTIVATE | User32.SWP_NOMOVE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Height{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.bottom-rect.top;
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, Width, value, User32.SWP_NOACTIVATE | User32.SWP_NOMOVE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override Size Size{
			get{
				var rect = User32.GetWindowRect(Handle);
				return new Size(rect.right-rect.left, rect.bottom-rect.top);
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value.Width, value.Height, User32.SWP_NOACTIVATE | User32.SWP_NOMOVE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Left{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.left;
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, value, Top, 0, 0, User32.SWP_NOACTIVATE | User32.SWP_NOSIZE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Top{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.top;
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, Left, value, 0, 0, User32.SWP_NOACTIVATE | User32.SWP_NOSIZE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Right{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.right;
			}
		}
		
		public override int Bottom{
			get{
				var rect = User32.GetWindowRect(Handle);
				return rect.bottom;
			}
		}
		
		public override Point Location{
			get{
				var rect = User32.GetWindowRect(Handle);
				return new Point(rect.left, rect.top);
			}
			set{
				var result = User32.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, 0, 0, User32.SWP_NOACTIVATE | User32.SWP_NOSIZE | User32.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override void BringToFront()
		{
			var result = User32.SetWindowPos(Handle, (IntPtr)0, 0, 0, 0, 0, User32.SWP_NOACTIVATE | User32.SWP_NOSIZE | User32.SWP_NOMOVE);
			if(!result) throw new Win32Exception();
		}
		
		public override void SendToBack()
		{
			var result = User32.SetWindowPos(Handle, (IntPtr)1, 0, 0, 0, 0, User32.SWP_NOACTIVATE | User32.SWP_NOSIZE | User32.SWP_NOMOVE);
			if(!result) throw new Win32Exception();
		}
		
		public override Control.ControlCollection Controls{
			get{
				return new ControlCollection(this);
			}
		}
		
		private class ControlCollection : Control.ControlCollection
		{
			readonly Win32Control control;
			
			public ControlCollection(Win32Control control) : base(control.GetProxy())
			{
				this.control = control;
			}
			
			private List<IntPtr> GetWindowCollection()
			{
				var list = new List<IntPtr>();
				User32.EnumChildWindows(
					control.Handle,
					(hWnd, lParam) => {list.Add(hWnd); return true;},
					IntPtr.Zero
				);
				return list;
			}
			
			public override void Add(Control value)
			{
				value.Parent = null;
				User32.SetParent(value.Handle, control.Handle);
			}
			
			public override void Clear()
			{
				foreach(var hwnd in GetWindowCollection())
				{
					User32.SetParent(hwnd, IntPtr.Zero);
				}
			}
			
			public override bool ContainsKey(string key)
			{
				return this.Cast<Control>().Any(c => c.Name == key);
			}
			
			public override int Count{
				get{
					return GetWindowCollection().Count;
				}
			}
			
			public override IEnumerator GetEnumerator()
			{
				foreach(IntPtr hwnd in GetWindowCollection())
				{
					yield return Win32Control.FromHandle(hwnd);
				}
			}
			
			public override int GetChildIndex(Control child, bool throwException)
			{
				return base.GetChildIndex(child, throwException);
			}
			
			public override int IndexOfKey(string key)
			{
				return base.IndexOfKey(key);
			}
			
			public override void Remove(Control value)
			{
				if(value == null) return;
				if(control.Equals(value.Parent)) value.Parent = null;
			}
			
			public override void RemoveByKey(string key)
			{
				base.RemoveByKey(key);
			}
			
			public override void SetChildIndex(Control child, int newIndex)
			{
				base.SetChildIndex(child, newIndex);
			}
			
			public override Control this[int index]
			{
				get{
					return base[index];
				}
			}
			
			public override Control this[string key]
			{
				get{
					return base[key];
				}
			}
		}
		
		public override Graphics CreateGraphics()
		{
			return Graphics.FromHdc(User32.GetWindowDC(Handle));
		}
		
		public override bool Enabled{
			get{
				int style = User32.GetWindowStyle(Handle);
				return (style & User32.WS_DISABLED) == 0;
			}
			set{
				User32.EnableWindow(Handle, value);
			}
		}
		
		public override bool Visible{
			get{
				int style = User32.GetWindowStyle(Handle);
				return (style & User32.WS_VISIBLE) != 0;
			}
			set{
				User32.ShowWindow(Handle, value ? User32.SW_SHOW : User32.SW_HIDE);
			}
		}
		
		public override bool Checked{
			get{
				return IntPtr.Zero != User32.SendMessage(Handle, User32.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
			}
			set{
				
			}
		}
		
		public override CheckState CheckState{
			get{
				return (CheckState)User32.SendMessage(Handle, User32.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
			}
			set{
				User32.SendMessage(Handle, User32.BM_SETCHECK, (IntPtr)value, IntPtr.Zero);
			}
		}
		
		public override Font Font{
			get{
				IntPtr hfont = User32.SendMessage(Handle, User32.WM_GETFONT, IntPtr.Zero, IntPtr.Zero);
				if(hfont == IntPtr.Zero) return SystemFonts.DefaultFont;
				return Font.FromHfont(hfont);
			}
			set{
				IntPtr hfont = value != null ? value.ToHfont() : IntPtr.Zero;
				User32.SendMessage(Handle, User32.WM_SETFONT, hfont, IntPtr.Zero);
			}
		}
		
		public override void Update()
		{
			bool result = User32.UpdateWindow(Handle);
			if(!result) throw new Win32Exception();
		}
		
		public override void Invalidate(Region region, bool invalidateChildren)
		{
			if(region == null) Invalidate(invalidateChildren);
			using(var graphics = this.CreateGraphics())
			{
				IntPtr hrgn = region.GetHrgn(graphics);
				try{
					bool result;
					if(invalidateChildren)
					{
						result = User32.RedrawWindow(Handle, IntPtr.Zero, hrgn, User32.RDW_INVALIDATE | User32.RDW_ERASE | User32.RDW_ALLCHILDREN);
					}else{
						result = User32.InvalidateRgn(Handle, hrgn, true);
					}
					if(!result) throw new Win32Exception();
				}finally{
					User32.DeleteObject(hrgn);
				}
			}
		}
		
		public override void Invalidate(Rectangle rc, bool invalidateChildren)
		{
			if(rc.IsEmpty) Invalidate(invalidateChildren);
			bool result;
			var rect = new User32.RECT();
			rect.left = rc.Left;
			rect.top = rc.Top;
			rect.right = rc.Right;
			rect.bottom = rc.Bottom;
			if(invalidateChildren)
			{
				result = User32.RedrawWindow(Handle, ref rect, IntPtr.Zero, User32.RDW_INVALIDATE | User32.RDW_ERASE | User32.RDW_ALLCHILDREN);
			}else{
				result = User32.InvalidateRect(Handle, ref rect, true);
			}
			if(!result) throw new Win32Exception();
		}
		
		public override void Invalidate(Region region)
		{
			Invalidate(region, false);
		}
		
		public override void Invalidate(Rectangle rc)
		{
			Invalidate(rc, false);
		}
		
		public override void Invalidate(bool invalidateChildren)
		{
			bool result;
			if(invalidateChildren)
			{
				result = User32.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, User32.RDW_INVALIDATE | User32.RDW_ERASE | User32.RDW_ALLCHILDREN);
			}else{
				result = User32.InvalidateRect(Handle, IntPtr.Zero, true);
			}
			if(!result) throw new Win32Exception();
		}
		
		public override void Invalidate()
		{
			Invalidate(false);
		}
		
		public override void Refresh()
		{
			Invalidate(true);
			Update();
		}
		
		public override void Show()
		{
			Visible = true;
		}
		
		public override void Hide()
		{
			Visible = false;
		}
		
		public override bool HasChildren{
			get{
				return Controls.Count > 0;
			}
		}
		
		public override void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
		{
			uint flags = User32.SWP_NOACTIVATE | User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOZORDER;
			if((specified & BoundsSpecified.X) != 0)
			{
				if((specified & BoundsSpecified.Y) != 0)
				{
					flags &= ~User32.SWP_NOMOVE;
				}else{
					Location = new Point(x, Top);
				}
			}else if((specified & BoundsSpecified.Y) != 0)
			{
				Location = new Point(Left, y);
			}
			
			if((specified & BoundsSpecified.Width) != 0)
			{
				if((specified & BoundsSpecified.Height) != 0)
				{
					flags &= ~User32.SWP_NOSIZE;
				}else{
					Width = width;
				}
			}else if((specified & BoundsSpecified.Height) != 0)
			{
				Height = height;
			}
			User32.SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, flags);
		}
		
		public override void SetBounds(int x, int y, int width, int height)
		{
			SetBounds(x, y, width, height, BoundsSpecified.All);
		}
		
		public override void Select()
		{
			User32.SetActiveWindow(Handle);
		}
		
		public override bool Focus()
		{
			return User32.SetFocus(Handle) != IntPtr.Zero;
		}
		
		public override bool RecreatingHandle{
			get{
				return false;
			}
		}
		
		public FileVersionInfo FileVersion{
			get{
				IntPtr hInst = User32.GetWindowInstance(Handle);
				int proc;
				User32.GetWindowThreadProcessId(Handle, out proc);
				IntPtr hproc = Process.GetProcessById(proc).Handle;
				string modname = PsApi.GetModuleFileNameEx(hproc, hInst);
				return FileVersionInfo.GetVersionInfo(modname);
			}
		}
		
		public string GetVersionString(string name)
		{
			IntPtr hInst = User32.GetWindowInstance(Handle);
			IntPtr hResInfo = Kernel32.FindResource(hInst, (IntPtr)1, Kernel32.RT_VERSION);
			if(hResInfo != IntPtr.Zero)
			{
				IntPtr hResData = Kernel32.LoadResource(hInst, hResInfo);
				if(hResData == IntPtr.Zero) return null;
				IntPtr res = Kernel32.LockResource(hResData);
				int size = Kernel32.SizeofResource(hInst, hResInfo);
				
				byte[] buffer = new byte[size];
				Marshal.Copy(res, buffer, 0, size);
				
				IntPtr ptr;
				int len;
				if(!Version.VerQueryValue(buffer, @"\VarFileInfo\Translation", out ptr, out len))
					return null;
				int lang = Marshal.ReadInt16(ptr);
				int cp = Marshal.ReadInt16(ptr, 2);
				
				if(!Version.VerQueryValue(buffer, String.Format(@"\StringFileInfo\{0:X4}{1:X4}\{2}", lang, cp, name), out ptr, out len))
					return null;
				
				string str = Marshal.PtrToStringAuto(ptr, len);
				if(str == null) return null;
				return str.TrimEnd('\0');
			}
			return null;
		}
		
		public override string ProductVersion{
			get{
				return GetVersionString("ProductVersion") ?? FileVersion.ProductVersion;
			}
		}
		
		public override string ProductName{
			get{
				return GetVersionString("ProductName") ?? FileVersion.ProductName;
			}
		}
		
		public override string CompanyName{
			get{
				return GetVersionString("CompanyName") ?? FileVersion.CompanyName;
			}
		}
		
		public override IWindowTarget WindowTarget{
			get{
				return this;
			}
			set{
				throw new NotImplementedException();
			}
		}
		
		void IWindowTarget.OnHandleChange(IntPtr newHandle)
		{
			_hwnd = newHandle;
		}
		
		void IWindowTarget.OnMessage(ref Message m)
		{
			m.Result = User32.SendMessage(Handle, unchecked((uint)m.Msg), m.WParam, m.LParam);
		}
		
		public override Region Region{
			get{
				IntPtr hRgn = Gdi32.CreateRectRgn(0, 0, 0, 0);
				try{
					int result = User32.GetWindowRgn(Handle, hRgn);
					if(result == 0) return null;
					return Region.FromHrgn(hRgn);
				}finally{
					User32.DeleteObject(hRgn);
				}
			}
			set{
				using(var graphics = this.CreateGraphics())
				{
					IntPtr hRgn = value.GetHrgn(graphics);
					try{
						int result = User32.SetWindowRgn(Handle, hRgn, this.Visible);
						if(result == 0) throw new Win32Exception();
					}finally{
						User32.DeleteObject(hRgn);
					}
				}
			}
		}
		
		public override Point PointToScreen(Point p)
		{
			var pt = new User32.POINT();
			pt.x = p.X;
			pt.y = p.Y;
			
			bool result = User32.ClientToScreen(Handle, ref pt);
			if(!result) throw new Win32Exception();
			
			return new Point(pt.x, pt.y);
		}
		
		public override Point PointToClient(Point p)
		{
			var pt = new User32.POINT();
			pt.x = p.X;
			pt.y = p.Y;
			
			bool result = User32.ScreenToClient(Handle, ref pt);
			if(!result) throw new Win32Exception();
			
			return new Point(pt.x, pt.y);
		}
		
		public override Rectangle RectangleToScreen(Rectangle r)
		{
			var pts = new User32.POINT[2];
			pts[0].x = r.Left;
			pts[0].y = r.Top;
			pts[1].x = r.Right;
			pts[1].y = r.Bottom;
			User32.MapWindowPoints(Handle, IntPtr.Zero, pts, pts.Length);
			return Rectangle.FromLTRB(pts[0].x, pts[0].y, pts[1].x, pts[1].y);
		}
		
		public override Rectangle RectangleToClient(Rectangle r)
		{
			var pts = new User32.POINT[2];
			pts[0].x = r.Left;
			pts[0].y = r.Top;
			pts[1].x = r.Right;
			pts[1].y = r.Bottom;
			User32.MapWindowPoints(IntPtr.Zero, Handle, pts, pts.Length);
			return Rectangle.FromLTRB(pts[0].x, pts[0].y, pts[1].x, pts[1].y);
		}
		
		public override bool PreProcessMessage(ref Message msg)
		{
			return false;
		}
		
		public override Form FindForm()
		{
			if(typeof(Form).IsAssignableFrom(GetControlType()))
			{
				return (Form)this.GetProxy();
			}else{
				var parent = this.Parent;
				if(parent == null) return null;
				return parent.FindForm();
			}
		}
		
		public override Rectangle Bounds{
			get{
				var rect = User32.GetWindowRect(Handle);
				return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
			}
			set{
				SetBounds(value.X, value.Y, value.Width, value.Height);
			}
		}
		
		
		
		public override bool UseWaitCursor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override Control TopLevelControl {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool TabStop {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override int TabIndex {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override void SuspendLayout()
		{
			throw new NotImplementedException();
		}
		
		public override ISite Site {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			throw new NotImplementedException();
		}
		
		public override void Scale(float dx, float dy)
		{
			throw new NotImplementedException();
		}
		
		public override void Scale(float ratio)
		{
			throw new NotImplementedException();
		}
		
		public override void Scale(System.Drawing.SizeF factor)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.RightToLeft RightToLeft {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override void ResumeLayout(bool performLayout)
		{
			throw new NotImplementedException();
		}
		
		public override void ResumeLayout()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetText()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetRightToLeft()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetImeMode()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetForeColor()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetFont()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetCursor()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetBindings()
		{
			throw new NotImplementedException();
		}
		
		public override void ResetBackColor()
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.PreProcessControlState PreProcessControlMessage(ref System.Windows.Forms.Message msg)
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Size PreferredSize {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void PerformLayout(System.Windows.Forms.Control affectedControl, string affectedProperty)
		{
			throw new NotImplementedException();
		}
		
		public override void PerformLayout()
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.Padding Padding {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Size MinimumSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Size MaximumSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.Padding Margin {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsMirrored {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsHandleCreated {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsDisposed {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool IsAccessible {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool InvokeRequired {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override object Invoke(Delegate method, params object[] args)
		{
			throw new NotImplementedException();
		}
		
		public override object Invoke(Delegate method)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.ImeMode ImeMode {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Size GetPreferredSize(System.Drawing.Size proposedSize)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.Control GetNextControl(System.Windows.Forms.Control ctl, bool forward)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.Control GetChildAtPoint(System.Drawing.Point pt, System.Windows.Forms.GetChildAtPointSkip skipValue)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.Control GetChildAtPoint(System.Drawing.Point pt)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.IContainerControl GetContainerControl()
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Color ForeColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool Focused {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override object EndInvoke(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}
		
		public override void DrawToBitmap(System.Drawing.Bitmap bitmap, System.Drawing.Rectangle targetBounds)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.DragDropEffects DoDragDrop(object data, System.Windows.Forms.DragDropEffects allowedEffects)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.DockStyle Dock {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool Disposing {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Rectangle DisplayRectangle {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.ControlBindingsCollection DataBindings {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.Cursor Cursor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool Created {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override void CreateControl()
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.ContextMenuStrip ContextMenuStrip {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.ContextMenu ContextMenu {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool ContainsFocus {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool Contains(System.Windows.Forms.Control ctl)
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Size ClientSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Rectangle ClientRectangle {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool CausesValidation {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool Capture {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool CanSelect {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool CanFocus {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.BindingContext BindingContext {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override IAsyncResult BeginInvoke(Delegate method, params object[] args)
		{
			throw new NotImplementedException();
		}
		
		public override IAsyncResult BeginInvoke(Delegate method)
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.ImageLayout BackgroundImageLayout {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Image BackgroundImage {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Color BackColor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool AutoSize {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Drawing.Point AutoScrollOffset {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.AnchorStyles Anchor {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override bool AllowDrop {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.AccessibleRole AccessibleRole {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override string AccessibleName {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override string AccessibleDescription {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override string AccessibleDefaultActionDescription {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override System.Windows.Forms.AccessibleObject AccessibilityObject {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
