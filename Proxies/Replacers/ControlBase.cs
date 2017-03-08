/* Date: 5.3.2017, Time: 16:21 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace IllidanS4.SharpUtils.Proxies.Replacers
{
	public abstract class ControlBase : ProxyImplementation<Control, IControl>, IControl, IEquatable<Control>, IWin32Window
	{
		Type IProxyReplacer<Control, IControl>.GetBoundType()
		{
			return GetControlType();
		}
		
		protected virtual Type GetControlType()
		{
			return typeof(Control);
		}
		
		public override bool Equals(object obj)
		{
			Control other = obj as Control;
			if(other != null)
			{
				return this.Equals(other);
			}
			return base.Equals(obj);
		}
		
		public abstract bool Equals(Control other);
		
		public abstract override int GetHashCode();
		
		public override string ToString()
		{
			return GetControlType().ToString();
		}
		
		public abstract IAsyncResult BeginInvoke(Delegate method);
		public abstract IAsyncResult BeginInvoke(Delegate method, params object[] args);
		public abstract void BringToFront();
		public abstract bool Contains(Control ctl);
		public abstract void CreateControl();
		public abstract Graphics CreateGraphics();
		public abstract DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);
		public abstract void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);
		public abstract object EndInvoke(IAsyncResult asyncResult);
		public abstract Form FindForm();
		public abstract bool Focus();
		public abstract IContainerControl GetContainerControl();
		public abstract Control GetChildAtPoint(Point pt);
		public abstract Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue);
		public abstract Control GetNextControl(Control ctl, bool forward);
		public abstract Size GetPreferredSize(Size proposedSize);
		public abstract void Hide();
		public abstract void Invalidate();
		public abstract void Invalidate(bool invalidateChildren);
		public abstract void Invalidate(Rectangle rc);
		public abstract void Invalidate(Region region);
		public abstract void Invalidate(Rectangle rc, bool invalidateChildren);
		public abstract void Invalidate(Region region, bool invalidateChildren);
		public abstract object Invoke(Delegate method);
		public abstract object Invoke(Delegate method, params object[] args);
		public abstract void PerformLayout();
		public abstract void PerformLayout(Control affectedControl, string affectedProperty);
		public abstract Point PointToClient(Point p);
		public abstract Point PointToScreen(Point p);
		public abstract PreProcessControlState PreProcessControlMessage(ref Message msg);
		public abstract bool PreProcessMessage(ref Message msg);
		public abstract Rectangle RectangleToClient(Rectangle r);
		public abstract Rectangle RectangleToScreen(Rectangle r);
		public abstract void Refresh();
		public abstract void ResetBackColor();
		public abstract void ResetBindings();
		public abstract void ResetCursor();
		public abstract void ResetFont();
		public abstract void ResetForeColor();
		public abstract void ResetImeMode();
		public abstract void ResetRightToLeft();
		public abstract void ResetText();
		public abstract void ResumeLayout();
		public abstract void ResumeLayout(bool performLayout);
		public abstract void Scale(SizeF factor);
		public abstract void Scale(float ratio);
		public abstract void Scale(float dx, float dy);
		public abstract void Select();
		public abstract bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);
		public abstract void SendToBack();
		public abstract void SetBounds(int x, int y, int width, int height);
		public abstract void SetBounds(int x, int y, int width, int height, BoundsSpecified specified);
		public abstract void Show();
		public abstract void SuspendLayout();
		public abstract void Update();
		
		public abstract AccessibleObject AccessibilityObject { get; }
		public abstract string AccessibleDefaultActionDescription { get; set; }
		public abstract string AccessibleDescription { get; set; }
		public abstract string AccessibleName { get; set; }
		public abstract AccessibleRole AccessibleRole { get; set; }
		public abstract bool AllowDrop { get; set; }
		public abstract AnchorStyles Anchor { get; set; }
		public abstract Point AutoScrollOffset { get; set; }
		public abstract bool AutoSize { get; set; }
		public abstract Color BackColor { get; set; }
		public abstract Image BackgroundImage { get; set; }
		public abstract ImageLayout BackgroundImageLayout { get; set; }
		public abstract BindingContext BindingContext { get; set; }
		public abstract int Bottom { get; }
		public abstract Rectangle Bounds { get; set; }
		public abstract bool CanFocus { get; }
		public abstract bool CanSelect { get; }
		public abstract bool Capture { get; set; }
		public abstract bool CausesValidation { get; set; }
		public abstract Rectangle ClientRectangle { get; }
		public abstract Size ClientSize { get; set; }
		public abstract string CompanyName { get; }
		public abstract bool ContainsFocus { get; }
		public abstract ContextMenu ContextMenu { get; set; }
		public abstract ContextMenuStrip ContextMenuStrip { get; set; }
		public abstract Control.ControlCollection Controls { get; }
		public abstract bool Created { get; }
		public abstract Cursor Cursor { get; set; }
		public abstract ControlBindingsCollection DataBindings { get; }
		public abstract Rectangle DisplayRectangle { get; }
		public abstract bool Disposing { get; }
		public abstract DockStyle Dock { get; set; }
		public abstract bool Enabled { get; set; }
		public abstract bool Focused { get; }
		public abstract Font Font { get; set; }
		public abstract Color ForeColor { get; set; }
		public abstract IntPtr Handle { get; }
		public abstract bool HasChildren { get; }
		public abstract int Height { get; set; }
		public abstract ImeMode ImeMode { get; set; }
		public abstract bool InvokeRequired { get; }
		public abstract bool IsAccessible { get; set; }
		public abstract bool IsDisposed { get; }
		public abstract bool IsHandleCreated { get; }
		public abstract bool IsMirrored { get; }
		public abstract LayoutEngine LayoutEngine { get; }
		public abstract int Left { get; set; }
		public abstract Point Location { get; set; }
		public abstract Padding Margin { get; set; }
		public abstract Size MaximumSize { get; set; }
		public abstract Size MinimumSize { get; set; }
		public abstract string Name { get; set; }
		public abstract Padding Padding { get; set; }
		public abstract Control Parent { get; set; }
		public abstract Size PreferredSize { get; }
		public abstract string ProductName { get; }
		public abstract string ProductVersion { get; }
		public abstract bool RecreatingHandle { get; }
		public abstract Region Region { get; set; }
		public abstract int Right { get; }
		public abstract RightToLeft RightToLeft { get; set; }
		public abstract ISite Site { get; set; }
		public abstract Size Size { get; set; }
		public abstract int TabIndex { get; set; }
		public abstract bool TabStop { get; set; }
		public abstract object Tag { get; set; }
		public abstract string Text { get; set; }
		public abstract int Top { get; set; }
		public abstract Control TopLevelControl { get; }
		public abstract bool UseWaitCursor { get; set; }
		public abstract bool Visible { get; set; }
		public abstract int Width { get; set; }
		public abstract IWindowTarget WindowTarget { get; set; }
		
		//CheckBox, RadioButton
		public abstract bool Checked { get; set; }
		public abstract CheckState CheckState { get; set; }
	}
}
