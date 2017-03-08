/* Date: 5.3.2017, Time: 16:04 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace IllidanS4.SharpUtils.Proxies.Replacers
{
	/// <summary>
	/// Contains the necessary methods for implementing the <see cref="System.Windows.Forms.Control"/> class.
	/// </summary>
	public interface IControl : IProxyReplacer<Control, IControl>
	{
		string ToString();
		
		//CheckBox, RadioButton public
		bool Checked{ get; set; }
		//CheckBox public
		CheckState CheckState{ get; set; }
		
		//Control public
		IAsyncResult BeginInvoke(Delegate method);
		IAsyncResult BeginInvoke(Delegate method, params object[] args);
		void BringToFront();
		bool Contains(Control ctl);
		void CreateControl();
		Graphics CreateGraphics();
		DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects);
		void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds);
		object EndInvoke(IAsyncResult asyncResult);
		Form FindForm();
		bool Focus();
		IContainerControl GetContainerControl();
		Control GetChildAtPoint(Point pt);
		Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue);
		Control GetNextControl(Control ctl, bool forward);
		Size GetPreferredSize(Size proposedSize);
		void Hide();
		void Invalidate();
		void Invalidate(bool invalidateChildren);
		void Invalidate(Rectangle rc);
		void Invalidate(Region region);
		void Invalidate(Rectangle rc, bool invalidateChildren);
		void Invalidate(Region region, bool invalidateChildren);
		object Invoke(Delegate method);
		object Invoke(Delegate method, params object[] args);
		void PerformLayout();
		void PerformLayout(Control affectedControl, string affectedProperty);
		Point PointToClient(Point p);
		Point PointToScreen(Point p);
		PreProcessControlState PreProcessControlMessage(ref Message msg);
		bool PreProcessMessage(ref Message msg);
		Rectangle RectangleToClient(Rectangle r);
		Rectangle RectangleToScreen(Rectangle r);
		void Refresh();
		void ResetBackColor();
		void ResetBindings();
		void ResetCursor();
		void ResetFont();
		void ResetForeColor();
		void ResetImeMode();
		void ResetRightToLeft();
		void ResetText();
		void ResumeLayout();
		void ResumeLayout(bool performLayout);
		void Scale(SizeF factor);
		void Scale(float ratio);
		void Scale(float dx, float dy);
		void Select();
		bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);
		void SendToBack();
		void SetBounds(int x, int y, int width, int height);
		void SetBounds(int x, int y, int width, int height, BoundsSpecified specified);
		void Show();
		void SuspendLayout();
		void Update();
		
		//Internal
		
		/*
		protected internal void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID);
		protected void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID);
		private IntPtr ActiveXMergeRegion(IntPtr region);
		private void ActiveXOnFocus(bool focus);
		private void ActiveXUpdateBounds(ref int x, ref int y, ref int width, ref int height, int flags);
		private void ActiveXViewChanged();
		internal void AddReflectChild();
		internal Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight);
		internal Size ApplySizeConstraints(Size proposedSize);
		internal Size ApplySizeConstraints(int width, int height);
		internal void AssignParent(Control value);
		internal void BeginUpdateInternal();
		internal bool CanProcessMnemonic();
		internal bool CanSelectCore();
		protected AccessibleObject CreateAccessibilityInstance();
		internal void CreateControl(bool fIgnoreVisible);
		protected ControlCollection CreateControlsInstance();
		internal Graphics CreateGraphicsInternal();
		protected void CreateHandle();
		protected void DefWndProc(ref Message m);
		protected void DestroyHandle();
		private void DetachContextMenu(object sender, EventArgs e);
		private void DetachContextMenuStrip(object sender, EventArgs e);
		protected void Dispose(bool disposing);
		internal void DisposeAxControls();
		private void DisposeFontHandle();
		internal bool EndUpdateInternal();
		internal bool EndUpdateInternal(bool invalidate);
		internal Form FindFormInternal();
		private Control FindMarshalingControl();
		internal bool FocusInternal();
		internal static Control FromHandleInternal(IntPtr handle);
		internal static Control FromChildHandleInternal(IntPtr handle);
		private AccessibleObject GetAccessibilityObject(int accObjId);
		protected AccessibleObject GetAccessibilityObjectById(int objectId);
		internal bool GetAnyDisposingInHierarchy();
		protected AutoSizeMode GetAutoSizeMode();
		internal static AutoValidate GetAutoValidateForControl(Control control);
		internal IContainerControl GetContainerControlInternal();
		private static FontHandleWrapper GetDefaultFontHandleWrapper();
		internal Control GetFirstChildControlInTabOrder(bool forward);
		internal IntPtr GetHRgn(Region region);
		internal Control[] GetChildControlsInTabOrder(bool handleCreatedOnly);
		private ArrayList GetChildControlsTabOrderList(bool handleCreatedOnly);
		private static ArrayList GetChildWindows(IntPtr hWndParent);
		private int[] GetChildWindowsInTabOrder();
		private ArrayList GetChildWindowsTabOrderList();
		private MenuItem GetMenuItemFromHandleId(IntPtr hmenu, int item);
		private Font GetParentFont();
		internal Size GetPreferredSizeCore(Size proposedSize);
		internal static IntPtr GetSafeHandle(IWin32Window window);
		protected Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified);
		internal bool GetState(int flag);
		private bool GetState2(int flag);
		protected bool GetStyle(ControlStyles flag);
		protected bool GetTopLevel();
		internal bool GetVisibleCore();
		private MouseButtons GetXButton(int wparam);
		private void HookMouseEvent();
		internal static void CheckParentingCycle(Control bottom, Control toFind);
		private void ChildGotFocus(Control child);
		internal IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg);
		protected void InitLayout();
		private void InitMouseWheelSupport();
		private void InitScaling(BoundsSpecified specified);
		protected void InvokeGotFocus(Control toInvoke, EventArgs e);
		protected void InvokeLostFocus(Control toInvoke, EventArgs e);
		private void InvokeMarshaledCallback(ThreadMethodEntry tme);
		private static void InvokeMarshaledCallbackDo(ThreadMethodEntry tme);
		private static void InvokeMarshaledCallbackHelper(object obj);
		private void InvokeMarshaledCallbacks();
		protected void InvokeOnClick(Control toInvoke, EventArgs e);
		protected void InvokePaint(Control c, PaintEventArgs e);
		protected void InvokePaintBackground(Control c, PaintEventArgs e);
		internal bool IsDescendant(Control descendant);
		private static bool IsFocusManagingContainerControl(Control ctl);
		internal bool IsFontSet();
		protected bool IsInputChar(char charCode);
		protected bool IsInputKey(Keys keyData);
		internal bool IsUpdating();
		private bool IsValidBackColor(Color c);
		private void ListenToUserPreferenceChanged(bool listen);
		private object MarshaledInvoke(Control caller, Delegate method, object[] args, bool synchronous);
		private void MarshalStringToMessage(string value, ref Message m);
		internal void NotifyEnter();
		protected void NotifyInvalidate(Rectangle invalidatedArea);
		internal void NotifyLeave();
		private void NotifyValidated();
		private bool NotifyValidating();
		internal void NotifyValidationResult(object sender, CancelEventArgs ev);
		protected void OnAutoSizeChanged(EventArgs e);
		protected void OnBackColorChanged(EventArgs e);
		protected void OnBackgroundImageChanged(EventArgs e);
		protected void OnBackgroundImageLayoutChanged(EventArgs e);
		protected void OnBindingContextChanged(EventArgs e);
		internal void OnBoundsUpdate(int x, int y, int width, int height);
		protected void OnCausesValidationChanged(EventArgs e);
		protected void OnClick(EventArgs e);
		protected void OnClientSizeChanged(EventArgs e);
		protected void OnContextMenuChanged(EventArgs e);
		protected void OnContextMenuStripChanged(EventArgs e);
		protected void OnControlAdded(ControlEventArgs e);
		protected void OnControlRemoved(ControlEventArgs e);
		protected void OnCreateControl();
		protected void OnCursorChanged(EventArgs e);
		protected void OnDockChanged(EventArgs e);
		protected void OnDoubleClick(EventArgs e);
		protected void OnDragDrop(DragEventArgs drgevent);
		protected void OnDragEnter(DragEventArgs drgevent);
		protected void OnDragLeave(EventArgs e);
		protected void OnDragOver(DragEventArgs drgevent);
		protected void OnEnabledChanged(EventArgs e);
		protected void OnEnter(EventArgs e);
		protected void OnFontChanged(EventArgs e);
		protected void OnForeColorChanged(EventArgs e);
		internal void OnFrameWindowActivate(bool fActivate);
		protected void OnGiveFeedback(GiveFeedbackEventArgs gfbevent);
		protected void OnGotFocus(EventArgs e);
		protected void OnHandleCreated(EventArgs e);
		protected void OnHandleDestroyed(EventArgs e);
		protected void OnHelpRequested(HelpEventArgs hevent);
		protected void OnChangeUICues(UICuesEventArgs e);
		internal void OnChildLayoutResuming(Control child, bool performLayout);
		internal void OnImeContextStatusChanged(IntPtr handle);
		protected void OnImeModeChanged(EventArgs e);
		protected void OnInvalidated(InvalidateEventArgs e);
		internal void OnInvokedSetScrollPosition(object sender, EventArgs e);
		protected void OnKeyDown(KeyEventArgs e);
		protected void OnKeyPress(KeyPressEventArgs e);
		protected void OnKeyUp(KeyEventArgs e);
		protected void OnLayout(LayoutEventArgs levent);
		internal void OnLayoutResuming(bool performLayout);
		internal void OnLayoutSuspended();
		protected void OnLeave(EventArgs e);
		protected void OnLocationChanged(EventArgs e);
		protected void OnLostFocus(EventArgs e);
		protected void OnMarginChanged(EventArgs e);
		protected void OnMouseCaptureChanged(EventArgs e);
		protected void OnMouseClick(MouseEventArgs e);
		protected void OnMouseDoubleClick(MouseEventArgs e);
		protected void OnMouseDown(MouseEventArgs e);
		protected void OnMouseEnter(EventArgs e);
		protected void OnMouseHover(EventArgs e);
		protected void OnMouseLeave(EventArgs e);
		protected void OnMouseMove(MouseEventArgs e);
		protected void OnMouseUp(MouseEventArgs e);
		protected void OnMouseWheel(MouseEventArgs e);
		protected void OnMove(EventArgs e);
		protected void OnNotifyMessage(Message m);
		protected void OnPaddingChanged(EventArgs e);
		protected void OnPaint(PaintEventArgs e);
		protected void OnPaintBackground(PaintEventArgs pevent);
		protected void OnParentBackColorChanged(EventArgs e);
		protected void OnParentBackgroundImageChanged(EventArgs e);
		internal void OnParentBecameInvisible();
		protected void OnParentBindingContextChanged(EventArgs e);
		protected void OnParentCursorChanged(EventArgs e);
		protected void OnParentEnabledChanged(EventArgs e);
		protected void OnParentFontChanged(EventArgs e);
		protected void OnParentForeColorChanged(EventArgs e);
		internal void OnParentHandleRecreated();
		internal void OnParentHandleRecreating();
		protected void OnParentChanged(EventArgs e);
		private void OnParentInvalidated(InvalidateEventArgs e);
		protected void OnParentRightToLeftChanged(EventArgs e);
		protected void OnParentVisibleChanged(EventArgs e);
		protected void OnPreviewKeyDown(PreviewKeyDownEventArgs e);
		protected void OnPrint(PaintEventArgs e);
		protected void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent);
		protected void OnRegionChanged(EventArgs e);
		protected void OnResize(EventArgs e);
		protected void OnRightToLeftChanged(EventArgs e);
		private void OnSetScrollPosition(object sender, EventArgs e);
		protected void OnSizeChanged(EventArgs e);
		protected void OnStyleChanged(EventArgs e);
		protected void OnSystemColorsChanged(EventArgs e);
		protected void OnTabIndexChanged(EventArgs e);
		protected void OnTabStopChanged(EventArgs e);
		protected void OnTextChanged(EventArgs e);
		internal void OnTopMostActiveXParentChanged(EventArgs e);
		protected void OnValidated(EventArgs e);
		protected void OnValidating(CancelEventArgs e);
		protected void OnVisibleChanged(EventArgs e);
		private static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor);
		internal void PaintBackground(PaintEventArgs e, Rectangle rectangle);
		internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor);
		internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor, Point scrollOffset);
		private void PaintException(PaintEventArgs e);
		internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle);
		internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region transparentRegion);
		private void PaintWithErrorHandling(PaintEventArgs e, short layer);
		internal bool PerformContainerValidation(ValidationConstraints validationConstraints);
		internal bool PerformControlValidation(bool bulkValidation);
		internal void PerformLayout(LayoutEventArgs args);
		internal Point PointToClientInternal(Point p);
		internal static PreProcessControlState PreProcessControlMessageInternal(Control target, ref Message msg);
		private void PrintToMetaFile(HandleRef hDC, IntPtr lParam);
		private void PrintToMetaFile_SendPrintMessage(HandleRef hDC, IntPtr lParam);
		internal void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds);
		protected bool ProcessCmdKey(ref Message msg, Keys keyData);
		protected bool ProcessDialogChar(char charCode);
		protected bool ProcessDialogKey(Keys keyData);
		protected bool ProcessKeyEventArgs(ref Message m);
		protected internal bool ProcessKeyMessage(ref Message m);
		protected bool ProcessKeyPreview(ref Message m);
		protected internal bool ProcessMnemonic(char charCode);
		internal void ProcessUICues(ref Message msg);
		internal void RaiseCreateHandleEvent(EventArgs e);
		protected void RaiseDragEvent(object key, DragEventArgs e);
		protected void RaiseKeyEvent(object key, KeyEventArgs e);
		protected void RaiseMouseEvent(object key, MouseEventArgs e);
		protected void RaisePaintEvent(object key, PaintEventArgs e);
		protected void RecreateHandle();
		internal void RecreateHandleCore();
		protected static bool ReflectMessage(IntPtr hWnd, ref Message m);
		internal static bool ReflectMessageInternal(IntPtr hWnd, ref Message m);
		private void RemovePendingMessages(int msgMin, int msgMax);
		internal void RemoveReflectChild();
		private bool RenderColorTransparent(Color c);
		private void ResetEnabled();
		private void ResetLocation();
		private void ResetMargin();
		private void ResetMinimumSize();
		protected void ResetMouseEventArgs();
		private void ResetPadding();
		private void ResetSize();
		private void ResetVisible();
		protected ContentAlignment RtlTranslateAlignment(ContentAlignment align);
		protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align);
		protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align);
		protected internal ContentAlignment RtlTranslateContent(ContentAlignment align);
		protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align);
		protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align);
		internal void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl);
		protected void ScaleControl(SizeF factor, BoundsSpecified specified);
		internal void ScaleControl(SizeF includedFactor, SizeF excludedFactor, Control requestingControl);
		protected void ScaleCore(float dx, float dy);
		internal void ScaleChildControls(SizeF includedFactor, SizeF excludedFactor, Control requestingControl);
		internal Size ScaleSize(Size startSize, float x, float y);
		protected void Select(bool directed, bool forward);
		internal bool SelectNextControlInternal(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap);
		private void SelectNextIfFocused();
		internal IntPtr SendMessage(int msg, bool wparam, int lparam);
		internal IntPtr SendMessage(int msg, int wparam, ref NativeMethods.RECT lparam);
		internal IntPtr SendMessage(int msg, ref int wparam, ref int lparam);
		internal IntPtr SendMessage(int msg, int wparam, int lparam);
		internal IntPtr SendMessage(int msg, int wparam, IntPtr lparam);
		internal IntPtr SendMessage(int msg, int wparam, string lparam);
		internal IntPtr SendMessage(int msg, IntPtr wparam, int lparam);
		internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam);
		internal void SetAcceptDrops(bool accept);
		protected void SetAutoSizeMode(AutoSizeMode mode);
		protected void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified);
		protected void SetClientSizeCore(int x, int y);
		private void SetHandle(IntPtr value);
		private void SetParentHandle(IntPtr value);
		internal void SetState(int flag, bool value);
		internal void SetState2(int flag, bool value);
		protected void SetStyle(ControlStyles flag, bool value);
		protected void SetTopLevel(bool value);
		internal void SetTopLevelInternal(bool value);
		internal static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette);
		protected void SetVisibleCore(bool value);
		private void SetWindowFont();
		private void SetWindowStyle(int flag, bool value);
		internal bool ShouldPerformContainerValidation();
		private bool ShouldSerializeAccessibleName();
		internal bool ShouldSerializeBackColor();
		internal bool ShouldSerializeCursor();
		private bool ShouldSerializeEnabled();
		internal bool ShouldSerializeFont();
		internal bool ShouldSerializeForeColor();
		internal bool ShouldSerializeImeMode();
		internal bool ShouldSerializeMargin();
		internal bool ShouldSerializeMaximumSize();
		internal bool ShouldSerializeMinimumSize();
		internal bool ShouldSerializePadding();
		internal bool ShouldSerializeRightToLeft();
		internal bool ShouldSerializeSize();
		internal bool ShouldSerializeText();
		private bool ShouldSerializeVisible();
		protected Size SizeFromClientSize(Size clientSize);
		internal Size SizeFromClientSize(int width, int height);
		void IDropTarget.OnDragDrop(DragEventArgs drgEvent);
		void IDropTarget.OnDragEnter(DragEventArgs drgEvent);
		void IDropTarget.OnDragLeave(EventArgs e);
		void IDropTarget.OnDragOver(DragEventArgs drgEvent);
		void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs);
		void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs);
		void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty);
		void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified);
		int UnsafeNativeMethods.IOleControl.FreezeEvents(int bFreeze);
		int UnsafeNativeMethods.IOleControl.GetControlInfo(NativeMethods.tagCONTROLINFO pCI);
		int UnsafeNativeMethods.IOleControl.OnAmbientPropertyChange(int dispID);
		int UnsafeNativeMethods.IOleControl.OnMnemonic(ref NativeMethods.MSG pMsg);
		void UnsafeNativeMethods.IOleInPlaceActiveObject.ContextSensitiveHelp(int fEnterMode);
		void UnsafeNativeMethods.IOleInPlaceActiveObject.EnableModeless(int fEnable);
		int UnsafeNativeMethods.IOleInPlaceActiveObject.GetWindow(out IntPtr hwnd);
		void UnsafeNativeMethods.IOleInPlaceActiveObject.OnDocWindowActivate(int fActivate);
		void UnsafeNativeMethods.IOleInPlaceActiveObject.OnFrameWindowActivate(bool fActivate);
		void UnsafeNativeMethods.IOleInPlaceActiveObject.ResizeBorder(NativeMethods.COMRECT prcBorder, UnsafeNativeMethods.IOleInPlaceUIWindow pUIWindow, bool fFrameWindow);
		int UnsafeNativeMethods.IOleInPlaceActiveObject.TranslateAccelerator(ref NativeMethods.MSG lpmsg);
		void UnsafeNativeMethods.IOleInPlaceObject.ContextSensitiveHelp(int fEnterMode);
		int UnsafeNativeMethods.IOleInPlaceObject.GetWindow(out IntPtr hwnd);
		void UnsafeNativeMethods.IOleInPlaceObject.InPlaceDeactivate();
		void UnsafeNativeMethods.IOleInPlaceObject.ReactivateAndUndo();
		void UnsafeNativeMethods.IOleInPlaceObject.SetObjectRects(NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect);
		int UnsafeNativeMethods.IOleInPlaceObject.UIDeactivate();
		int UnsafeNativeMethods.IOleObject.Advise(IAdviseSink pAdvSink, out int cookie);
		int UnsafeNativeMethods.IOleObject.Close(int dwSaveOption);
		int UnsafeNativeMethods.IOleObject.DoVerb(int iVerb, IntPtr lpmsg, UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect);
		int UnsafeNativeMethods.IOleObject.EnumAdvise(out IEnumSTATDATA e);
		int UnsafeNativeMethods.IOleObject.EnumVerbs(out UnsafeNativeMethods.IEnumOLEVERB e);
		UnsafeNativeMethods.IOleClientSite UnsafeNativeMethods.IOleObject.GetClientSite();
		int UnsafeNativeMethods.IOleObject.GetClipboardData(int dwReserved, out IDataObject data);
		int UnsafeNativeMethods.IOleObject.GetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel);
		int UnsafeNativeMethods.IOleObject.GetMiscStatus(int dwAspect, out int cookie);
		int UnsafeNativeMethods.IOleObject.GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker);
		int UnsafeNativeMethods.IOleObject.GetUserClassID(ref Guid pClsid);
		int UnsafeNativeMethods.IOleObject.GetUserType(int dwFormOfType, out string userType);
		int UnsafeNativeMethods.IOleObject.InitFromData(IDataObject pDataObject, int fCreation, int dwReserved);
		int UnsafeNativeMethods.IOleObject.IsUpToDate();
		int UnsafeNativeMethods.IOleObject.OleUpdate();
		int UnsafeNativeMethods.IOleObject.SetClientSite(UnsafeNativeMethods.IOleClientSite pClientSite);
		int UnsafeNativeMethods.IOleObject.SetColorScheme(NativeMethods.tagLOGPALETTE pLogpal);
		int UnsafeNativeMethods.IOleObject.SetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel);
		int UnsafeNativeMethods.IOleObject.SetHostNames(string szContainerApp, string szContainerObj);
		int UnsafeNativeMethods.IOleObject.SetMoniker(int dwWhichMoniker, object pmk);
		int UnsafeNativeMethods.IOleObject.Unadvise(int dwConnection);
		void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode);
		int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd);
		void UnsafeNativeMethods.IPersist.GetClassID(out Guid pClassID);
		void UnsafeNativeMethods.IPersistPropertyBag.GetClassID(out Guid pClassID);
		void UnsafeNativeMethods.IPersistPropertyBag.InitNew();
		void UnsafeNativeMethods.IPersistPropertyBag.Load(UnsafeNativeMethods.IPropertyBag pPropBag, UnsafeNativeMethods.IErrorLog pErrorLog);
		void UnsafeNativeMethods.IPersistPropertyBag.Save(UnsafeNativeMethods.IPropertyBag pPropBag, bool fClearDirty, bool fSaveAllProperties);
		void UnsafeNativeMethods.IPersistStorage.GetClassID(out Guid pClassID);
		void UnsafeNativeMethods.IPersistStorage.HandsOffStorage();
		void UnsafeNativeMethods.IPersistStorage.InitNew(UnsafeNativeMethods.IStorage pstg);
		int UnsafeNativeMethods.IPersistStorage.IsDirty();
		int UnsafeNativeMethods.IPersistStorage.Load(UnsafeNativeMethods.IStorage pstg);
		void UnsafeNativeMethods.IPersistStorage.Save(UnsafeNativeMethods.IStorage pstg, bool fSameAsLoad);
		void UnsafeNativeMethods.IPersistStorage.SaveCompleted(UnsafeNativeMethods.IStorage pStgNew);
		void UnsafeNativeMethods.IPersistStreamInit.GetClassID(out Guid pClassID);
		void UnsafeNativeMethods.IPersistStreamInit.GetSizeMax(long pcbSize);
		void UnsafeNativeMethods.IPersistStreamInit.InitNew();
		int UnsafeNativeMethods.IPersistStreamInit.IsDirty();
		void UnsafeNativeMethods.IPersistStreamInit.Load(UnsafeNativeMethods.IStream pstm);
		void UnsafeNativeMethods.IPersistStreamInit.Save(UnsafeNativeMethods.IStream pstm, bool fClearDirty);
		void UnsafeNativeMethods.IQuickActivate.GetContentExtent(NativeMethods.tagSIZEL pSizel);
		void UnsafeNativeMethods.IQuickActivate.QuickActivate(UnsafeNativeMethods.tagQACONTAINER pQaContainer, UnsafeNativeMethods.tagQACONTROL pQaControl);
		void UnsafeNativeMethods.IQuickActivate.SetContentExtent(NativeMethods.tagSIZEL pSizel);
		int UnsafeNativeMethods.IViewObject.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds, IntPtr pfnContinue, int dwContinue);
		int UnsafeNativeMethods.IViewObject.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze);
		void UnsafeNativeMethods.IViewObject.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink);
		int UnsafeNativeMethods.IViewObject.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet);
		void UnsafeNativeMethods.IViewObject.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink);
		int UnsafeNativeMethods.IViewObject.Unfreeze(int dwFreeze);
		void UnsafeNativeMethods.IViewObject2.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds, IntPtr pfnContinue, int dwContinue);
		int UnsafeNativeMethods.IViewObject2.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze);
		void UnsafeNativeMethods.IViewObject2.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink);
		int UnsafeNativeMethods.IViewObject2.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd, IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet);
		void UnsafeNativeMethods.IViewObject2.GetExtent(int dwDrawAspect, int lindex, NativeMethods.tagDVTARGETDEVICE ptd, NativeMethods.tagSIZEL lpsizel);
		void UnsafeNativeMethods.IViewObject2.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink);
		int UnsafeNativeMethods.IViewObject2.Unfreeze(int dwFreeze);
		private void UnhookMouseEvent();
		private void UpdateBindings();
		protected internal void UpdateBounds();
		protected void UpdateBounds(int x, int y, int width, int height);
		protected void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight);
		private void UpdateChildControlIndex(Control ctl);
		private void UpdateChildZOrder(Control ctl);
		internal void UpdateImeContextMode();
		private void UpdateReflectParent(bool findNewParent);
		private void UpdateRoot();
		protected void UpdateStyles();
		internal void UpdateStylesCore();
		protected void UpdateZOrder();
		private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref);
		internal bool ValidateActiveControl(out bool validatedControlAllowsFocusChange);
		private void VerifyImeModeChanged(ImeMode oldMode, ImeMode newMode);
		internal void VerifyImeRestrictedModeChanged();
		private void WaitForWaitHandle(WaitHandle waitHandle);
		internal void WindowAssignHandle(IntPtr handle, bool value);
		internal void WindowReleaseHandle();
		private void WmCaptureChanged(ref Message m);
		private void WmClose(ref Message m);
		private void WmCommand(ref Message m);
		internal void WmContextMenu(ref Message m);
		internal void WmContextMenu(ref Message m, Control sourceControl);
		private void WmCreate(ref Message m);
		private void WmCtlColorControl(ref Message m);
		private void WmDestroy(ref Message m);
		private void WmDisplayChange(ref Message m);
		private void WmDrawItem(ref Message m);
		private void WmDrawItemMenuItem(ref Message m);
		private void WmEraseBkgnd(ref Message m);
		private void WmExitMenuLoop(ref Message m);
		private void WmGetControlName(ref Message m);
		private void WmGetControlType(ref Message m);
		private void WmGetObject(ref Message m);
		private void WmHelp(ref Message m);
		private void WmImeEndComposition(ref Message m);
		private void WmImeChar(ref Message m);
		private void WmImeKillFocus();
		private void WmImeNotify(ref Message m);
		internal void WmImeSetFocus();
		private void WmImeStartComposition(ref Message m);
		private void WmInitMenuPopup(ref Message m);
		private void WmInputLangChange(ref Message m);
		private void WmInputLangChangeRequest(ref Message m);
		private void WmKeyChar(ref Message m);
		private void WmKillFocus(ref Message m);
		private void WmMeasureItem(ref Message m);
		private void WmMenuChar(ref Message m);
		private void WmMenuSelect(ref Message m);
		private void WmMouseDown(ref Message m, MouseButtons button, int clicks);
		private void WmMouseEnter(ref Message m);
		private void WmMouseHover(ref Message m);
		private void WmMouseLeave(ref Message m);
		private void WmMouseMove(ref Message m);
		private void WmMouseUp(ref Message m, MouseButtons button, int clicks);
		private void WmMouseWheel(ref Message m);
		private void WmMove(ref Message m);
		private void WmNotify(ref Message m);
		private void WmNotifyFormat(ref Message m);
		private void WmOwnerDraw(ref Message m);
		private void WmPaint(ref Message m);
		private void WmParentNotify(ref Message m);
		private void WmPrintClient(ref Message m);
		private void WmQueryNewPalette(ref Message m);
		private void WmSetCursor(ref Message m);
		private void WmSetFocus(ref Message m);
		private void WmShowWindow(ref Message m);
		private void WmUpdateUIState(ref Message m);
		private void WmWindowPosChanged(ref Message m);
		private void WmWindowPosChanging(ref Message m);
		protected void WndProc(ref Message m);
		private void WndProcException(Exception e);
		*/
		
		// Properties
		
		AccessibleObject AccessibilityObject { get; }
		string AccessibleDefaultActionDescription { get; set; }
		string AccessibleDescription { get; set; }
		string AccessibleName { get; set; }
		AccessibleRole AccessibleRole { get; set; }
		bool AllowDrop { get; set; }
		AnchorStyles Anchor { get; set; }
		Point AutoScrollOffset { get; set; }
		bool AutoSize { get; set; }
		Color BackColor { get; set; }
		Image BackgroundImage { get; set; }
		ImageLayout BackgroundImageLayout { get; set; }
		BindingContext BindingContext { get; set; }
		int Bottom { get; }
		Rectangle Bounds { get; set; }
		bool CanFocus { get; }
		bool CanSelect { get; }
		bool Capture { get; set; }
		bool CausesValidation { get; set; }
		Rectangle ClientRectangle { get; }
		Size ClientSize { get; set; }
		string CompanyName { get; }
		bool ContainsFocus { get; }
		ContextMenu ContextMenu { get; set; }
		ContextMenuStrip ContextMenuStrip { get; set; }
		Control.ControlCollection Controls { get; }
		bool Created { get; }
		Cursor Cursor { get; set; }
		ControlBindingsCollection DataBindings { get; }
		Rectangle DisplayRectangle { get; }
		bool Disposing { get; }
		DockStyle Dock { get; set; }
		bool Enabled { get; set; }
		bool Focused { get; }
		Font Font { get; set; }
		Color ForeColor { get; set; }
		IntPtr Handle { get; }
		bool HasChildren { get; }
		int Height { get; set; }
		ImeMode ImeMode { get; set; }
		bool InvokeRequired { get; }
		bool IsAccessible { get; set; }
		bool IsDisposed { get; }
		bool IsHandleCreated { get; }
		bool IsMirrored { get; }
		LayoutEngine LayoutEngine { get; }
		int Left { get; set; }
		Point Location { get; set; }
		Padding Margin { get; set; }
		Size MaximumSize { get; set; }
		Size MinimumSize { get; set; }
		string Name { get; set; }
		Padding Padding { get; set; }
		Control Parent { get; set; }
		Size PreferredSize { get; }
		string ProductName { get; }
		string ProductVersion { get; }
		bool RecreatingHandle { get; }
		Region Region { get; set; }
		int Right { get; }
		RightToLeft RightToLeft { get; set; }
		ISite Site { get; set; }
		Size Size { get; set; }
		int TabIndex { get; set; }
		bool TabStop { get; set; }
		object Tag { get; set; }
		string Text { get; set; }
		int Top { get; set; }
		Control TopLevelControl { get; }
		bool UseWaitCursor { get; set; }
		bool Visible { get; set; }
		int Width { get; set; }
		IWindowTarget WindowTarget { get; set; }
		
		/*
		private Color ActiveXAmbientBackColor { get; }
		private Font ActiveXAmbientFont { get; }
		private Color ActiveXAmbientForeColor { get; }
		private bool ActiveXEventsFrozen { get; }
		private IntPtr ActiveXHWNDParent { get; }
		private ActiveXImpl ActiveXInstance { get; }
		private AmbientProperties AmbientPropertiesService { get; }
		internal IntPtr BackColorBrush { get; }
		internal bool BecomingActiveControl { get; set; }
		internal BindingContext BindingContextInternal { get; set; }
		private BufferedGraphicsContext BufferContext { get; }
		internal ImeMode CachedImeMode { get; set; }
		internal bool CacheTextInternal { get; set; }
		internal bool CanAccessProperties { get; }
		protected bool CanEnableIme { get; }
		protected bool CanRaiseEvents { get; }
		internal bool CaptureInternal { get; set; }
		protected CreateParams CreateParams { [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)] get; }
		internal int CreateThreadId { get; }
		internal ImeMode CurrentImeContextMode { get; }
		protected Cursor DefaultCursor { get; }
		protected ImeMode DefaultImeMode { get; }
		protected Padding DefaultMargin { get; }
		protected Size DefaultMaximumSize { get; }
		protected Size DefaultMinimumSize { get; }
		protected Padding DefaultPadding { get; }
		private RightToLeft DefaultRightToLeft { get; }
		protected Size DefaultSize { get; }
		internal Color DisabledColor { get; }
		internal int DisableImeModeChangedCount { get; set; }
		protected bool DoubleBuffered { get; set; }
		private bool DoubleBufferingEnabled { get; }
		internal IntPtr FontHandle { get; }
		protected int FontHeight { get; set; }
		internal IntPtr HandleInternal { get; }
		internal bool HasMenu { get; }
		internal bool HostedInWin32DialogManager { get; }
		private static bool IgnoreWmImeNotify { get; set; }
		protected ImeMode ImeModeBase { get; set; }
		private bool ImeSupported { get; }
		internal int ImeWmCharsToIgnore { get; set; }
		internal IntPtr InternalHandle { get; }
		internal bool IsActiveX { get; }
		internal bool IsContainerControl { get; }
		internal bool IsIEParent { get; }
		internal bool IsLayoutSuspended { get; }
		internal bool IsMnemonicsListenerAxSourced { get; }
		internal bool IsTopMdiWindowClosing { get; set; }
		internal bool IsWindowObscured { get; }
		private bool LastCanEnableIme { get; set; }
		private AccessibleObject NcAccessibilityObject { get; }
		internal ContainerControl ParentContainerControl { get; }
		internal Control ParentInternal { get; set; }
		protected static ImeMode PropagatingImeMode { get; private set; }
		internal PropertyStore Properties { get; }
		internal Color RawBackColor { get; }
		private Control ReflectParent { get; set; }
		protected internal bool RenderRightToLeft { get; }
		internal bool RenderTransparencyWithVisualStyles { get; }
		internal bool RenderTransparent { get; }
		internal BoundsSpecified RequiredScaling { get; set; }
		internal bool RequiredScalingEnabled { get; set; }
		protected bool ResizeRedraw { get; set; }
		protected bool ScaleChildren { get; }
		internal bool ShouldAutoValidate { get; }
		protected internal bool ShowFocusCues { get; }
		protected internal bool ShowKeyboardCues { get; }
		internal int ShowParams { get; }
		internal bool SupportsUseCompatibleTextRendering { get; }
		IArrangedElement IArrangedElement.Container { get; }
		ArrangedElementCollection IArrangedElement.Children { get; }
		bool IArrangedElement.ParticipatesInLayout { get; }
		PropertyStore IArrangedElement.Properties { get; }
		internal bool TabStopInternal { get; set; }
		internal Control TopLevelControlInternal { get; }
		internal Control TopMostParent { get; }
		internal bool UseCompatibleTextRenderingInt { get; set; }
		internal bool ValidationCancelled { get; set; }
		private ControlVersionInfo VersionInfo { get; }
		private int WindowExStyle { get; set; }
		internal int WindowStyle { get; set; }
		internal string WindowText { get; set; }
		*/
	}
}