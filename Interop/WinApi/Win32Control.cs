/* Date: 5.3.2017, Time: 16:26 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IllidanS4.SharpUtils.Proxies.Replacers.Forms;

namespace IllidanS4.SharpUtils.Interop.WinApi
{
	public partial class Win32Control : ControlBase, IEquatable<Win32Control>
	{
		private readonly IntPtr _hwnd;
		
		public override IntPtr Handle{
			get{
				return _hwnd;
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
			string cls = Interop.GetClassName(Handle, true);
			int style = Interop.GetWindowStyle(Handle);
			switch(cls)
			{
				case "Button":
					if((style & Interop.BS_CHECKBOX) != 0)
						return typeof(CheckBox);
					if((style & Interop.BS_RADIOBUTTON) != 0)
						return typeof(RadioButton);
					return typeof(Button);
				case "ListBox":
					return typeof(ListBox); //or CheckedListBox
				case "ComboBox":
					return typeof(ComboBox); //or DataGridViewComboBoxEditingControl
				case "Edit":
					return typeof(TextBoxBase); //TextBoxBase, MaskedTextBox, DataGridTextBox, DataGridViewTextBoxEditingControl
				case "Static":
					return typeof(Label); //or LinkLabel
				case "MDIClient":
					return typeof(MdiClient);
				case "ScrollBar":
					return typeof(ScrollBar); //VScrollBar, HScrollBar
					
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
			return Interop.GetClassName(Handle, false);
		}
		
		public override string Text{
			get{
				IntPtr len = Interop.SendMessage(Handle, Interop.WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
				len += (int)len+2;
				
				IntPtr buffer = Marshal.AllocHGlobal(len);
				try{
					Interop.SendMessage(Handle, Interop.WM_GETTEXT, len, buffer);
					return Marshal.PtrToStringAuto(buffer);
				}finally{
					Marshal.FreeHGlobal(buffer);
				}
			}
			set{
				IntPtr str = Marshal.StringToHGlobalAuto(value);
				try{
					var ret = Interop.SendMessage(Handle, Interop.WM_SETTEXT, IntPtr.Zero, str);
					if(ret != (IntPtr)1) throw new Win32Exception();
				}finally{
					Marshal.FreeHGlobal(str);
				}
			}
		}
		
		public override Control Parent{
			get{
				IntPtr parent = Interop.GetAncestor(Handle, Interop.GA_PARENT);
				//if(parent == Interop.GetDesktopWindow()) return null;
				return Win32Control.FromHandle(parent);
			}
			set{
				Interop.SetParent(Handle, value.Handle);
			}
		}
		
		private UserData GetUserData()
		{
			IntPtr userdata = Interop.GetWindowLongPtr(Handle, Interop.GWL_USERDATA);
			return UserData.Load(userdata);
		}
		
		private UserData CreateUserData()
		{
			var data = GetUserData();
			if(data == null)
			{
				data = new UserData();
				Interop.SetWindowLongPtr(Handle, Interop.GWL_USERDATA, data.ToIntPtr());
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
				var rect = Interop.GetWindowRect(Handle);
				return rect.right-rect.left;
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value, Height, Interop.SWP_NOACTIVATE | Interop.SWP_NOMOVE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Height{
			get{
				var rect = Interop.GetWindowRect(Handle);
				return rect.bottom-rect.top;
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, 0, 0, Width, value, Interop.SWP_NOACTIVATE | Interop.SWP_NOMOVE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override Size Size{
			get{
				var rect = Interop.GetWindowRect(Handle);
				return new Size(rect.right-rect.left, rect.bottom-rect.top);
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, 0, 0, value.Width, value.Height, Interop.SWP_NOACTIVATE | Interop.SWP_NOMOVE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Left{
			get{
				var rect = Interop.GetWindowRect(Handle);
				return rect.left;
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, value, Top, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOSIZE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override int Top{
			get{
				var rect = Interop.GetWindowRect(Handle);
				return rect.top;
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, Left, value, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOSIZE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override Point Location{
			get{
				var rect = Interop.GetWindowRect(Handle);
				return new Point(rect.left, rect.top);
			}
			set{
				var result = Interop.SetWindowPos(Handle, IntPtr.Zero, value.X, value.Y, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOSIZE | Interop.SWP_NOZORDER);
				if(!result) throw new Win32Exception();
			}
		}
		
		public override void BringToFront()
		{
			var result = Interop.SetWindowPos(Handle, (IntPtr)0, 0, 0, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOSIZE | Interop.SWP_NOMOVE);
			if(!result) throw new Win32Exception();
		}
		
		public override void SendToBack()
		{
			var result = Interop.SetWindowPos(Handle, (IntPtr)1, 0, 0, 0, 0, Interop.SWP_NOACTIVATE | Interop.SWP_NOSIZE | Interop.SWP_NOMOVE);
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
				Interop.EnumChildWindows(
					control.Handle,
					(hWnd, lParam) => {list.Add(hWnd); return true;},
					IntPtr.Zero
				);
				return list;
			}
			
			public override void Add(Control value)
			{
				value.Parent = null;
				Interop.SetParent(value.Handle, control.Handle);
			}
			
			public override void Clear()
			{
				foreach(var hwnd in GetWindowCollection())
				{
					Interop.SetParent(hwnd, IntPtr.Zero);
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
			return Graphics.FromHdc(Interop.GetWindowDC(Handle));
		}
		
		public override bool Enabled{
			get{
				int style = Interop.GetWindowStyle(Handle);
				return (style & Interop.WS_DISABLED) == 0;
			}
			set{
				Interop.EnableWindow(Handle, value);
			}
		}
		
		public override bool Visible{
			get{
				int style = Interop.GetWindowStyle(Handle);
				return (style & Interop.WS_VISIBLE) != 0;
			}
			set{
				Interop.ShowWindow(Handle, value ? Interop.SW_SHOW : Interop.SW_HIDE);
			}
		}
		
		public override bool Checked{
			get{
				return IntPtr.Zero != Interop.SendMessage(Handle, Interop.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
			}
			set{
				
			}
		}
		
		public override CheckState CheckState{
			get{
				return (CheckState)Interop.SendMessage(Handle, Interop.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
			}
			set{
				Interop.SendMessage(Handle, Interop.BM_SETCHECK, (IntPtr)value, IntPtr.Zero);
			}
		}
		
		public override System.Windows.Forms.IWindowTarget WindowTarget {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
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
		
		public override void Update()
		{
			throw new NotImplementedException();
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
		
		public override System.ComponentModel.ISite Site {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override void Show()
		{
			throw new NotImplementedException();
		}
		
		public override void SetBounds(int x, int y, int width, int height, System.Windows.Forms.BoundsSpecified specified)
		{
			throw new NotImplementedException();
		}
		
		public override void SetBounds(int x, int y, int width, int height)
		{
			throw new NotImplementedException();
		}
		
		public override bool SelectNextControl(System.Windows.Forms.Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			throw new NotImplementedException();
		}
		
		public override void Select()
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
		
		public override int Right {
			get {
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
		
		public override System.Drawing.Region Region {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override void Refresh()
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Rectangle RectangleToScreen(System.Drawing.Rectangle r)
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Rectangle RectangleToClient(System.Drawing.Rectangle r)
		{
			throw new NotImplementedException();
		}
		
		public override bool RecreatingHandle {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override string ProductVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override string ProductName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override bool PreProcessMessage(ref System.Windows.Forms.Message msg)
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
		
		public override System.Drawing.Point PointToScreen(System.Drawing.Point p)
		{
			throw new NotImplementedException();
		}
		
		public override System.Drawing.Point PointToClient(System.Drawing.Point p)
		{
			throw new NotImplementedException();
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
		
		public override void Invalidate(System.Drawing.Region region, bool invalidateChildren)
		{
			throw new NotImplementedException();
		}
		
		public override void Invalidate(System.Drawing.Rectangle rc, bool invalidateChildren)
		{
			throw new NotImplementedException();
		}
		
		public override void Invalidate(System.Drawing.Region region)
		{
			throw new NotImplementedException();
		}
		
		public override void Invalidate(System.Drawing.Rectangle rc)
		{
			throw new NotImplementedException();
		}
		
		public override void Invalidate(bool invalidateChildren)
		{
			throw new NotImplementedException();
		}
		
		public override void Invalidate()
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
		
		public override void Hide()
		{
			throw new NotImplementedException();
		}
		
		public override bool HasChildren {
			get {
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
		
		public override System.Drawing.Font Font {
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
		
		public override bool Focus()
		{
			throw new NotImplementedException();
		}
		
		public override System.Windows.Forms.Form FindForm()
		{
			throw new NotImplementedException();
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
		
		public override string CompanyName {
			get {
				throw new NotImplementedException();
			}
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
		
		public override System.Drawing.Rectangle Bounds {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public override int Bottom {
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
