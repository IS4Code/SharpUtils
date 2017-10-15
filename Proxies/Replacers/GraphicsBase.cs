/* Date: 16.10.2017, Time: 0:40 */
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace IllidanS4.SharpUtils.Proxies.Replacers
{
	using DrawImageAbort = Graphics.DrawImageAbort;
	using EnumerateMetafileProc = Graphics.EnumerateMetafileProc;
	
	public abstract class GraphicsBase : ProxyImplementation<Graphics, IGraphics>, IGraphics
	{
		Type IProxyReplacer<Graphics, IGraphics>.GetBoundType()
		{
			return typeof(Graphics);
		}
		
	    public abstract void AddMetafileComment(byte[] data);
	    public abstract GraphicsContainer BeginContainer();
	    public abstract GraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, GraphicsUnit unit);
	    public abstract GraphicsContainer BeginContainer(RectangleF dstrect, RectangleF srcrect, GraphicsUnit unit);
	    public abstract void Clear(Color color);
	    public abstract void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize);
	    public abstract void CopyFromScreen(Point upperLeftSource, Point upperLeftDestination, Size blockRegionSize, CopyPixelOperation copyPixelOperation);
	    public abstract void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize);
	    public abstract void CopyFromScreen(int sourceX, int sourceY, int destinationX, int destinationY, Size blockRegionSize, CopyPixelOperation copyPixelOperation);
	    public abstract void Dispose();
	    public abstract void DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle);
	    public abstract void DrawArc(Pen pen, RectangleF rect, float startAngle, float sweepAngle);
	    public abstract void DrawArc(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);
	    public abstract void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);
	    public abstract void DrawBezier(Pen pen, Point pt1, Point pt2, Point pt3, Point pt4);
	    public abstract void DrawBezier(Pen pen, PointF pt1, PointF pt2, PointF pt3, PointF pt4);
	    public abstract void DrawBezier(Pen pen, float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4);
	    public abstract void DrawBeziers(Pen pen, Point[] points);
	    public abstract void DrawBeziers(Pen pen, PointF[] points);
	    public abstract void DrawClosedCurve(Pen pen, Point[] points);
	    public abstract void DrawClosedCurve(Pen pen, PointF[] points);
	    public abstract void DrawClosedCurve(Pen pen, Point[] points, float tension, FillMode fillmode);
	    public abstract void DrawClosedCurve(Pen pen, PointF[] points, float tension, FillMode fillmode);
	    public abstract void DrawCurve(Pen pen, Point[] points);
	    public abstract void DrawCurve(Pen pen, PointF[] points);
	    public abstract void DrawCurve(Pen pen, Point[] points, float tension);
	    public abstract void DrawCurve(Pen pen, PointF[] points, float tension);
	    public abstract void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments);
	    public abstract void DrawCurve(Pen pen, Point[] points, int offset, int numberOfSegments, float tension);
	    public abstract void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);
	    public abstract void DrawEllipse(Pen pen, Rectangle rect);
	    public abstract void DrawEllipse(Pen pen, RectangleF rect);
	    public abstract void DrawEllipse(Pen pen, int x, int y, int width, int height);
	    public abstract void DrawEllipse(Pen pen, float x, float y, float width, float height);
	    public abstract void DrawIcon(Icon icon, Rectangle targetRect);
	    public abstract void DrawIcon(Icon icon, int x, int y);
	    public abstract void DrawIconUnstretched(Icon icon, Rectangle targetRect);
	    public abstract void DrawImage(Image image, Point point);
	    public abstract void DrawImage(Image image, PointF point);
	    public abstract void DrawImage(Image image, Point[] destPoints);
	    public abstract void DrawImage(Image image, RectangleF rect);
	    public abstract void DrawImage(Image image, PointF[] destPoints);
	    public abstract void DrawImage(Image image, Rectangle rect);
	    public abstract void DrawImage(Image image, int x, int y);
	    public abstract void DrawImage(Image image, float x, float y);
	    public abstract void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr);
	    public abstract void DrawImage(Image image, int x, int y, int width, int height);
	    public abstract void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, float x, float y, float width, float height);
	    public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr);
	    public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
	    public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
	    public abstract void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);
	    public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);
	    public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit);
	    public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr);
	    public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);
	    public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
	    public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback);
	    public abstract void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);
	    public abstract void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);
	    public abstract void DrawImageUnscaled(Image image, Point point);
	    public abstract void DrawImageUnscaled(Image image, Rectangle rect);
	    public abstract void DrawImageUnscaled(Image image, int x, int y);
	    public abstract void DrawImageUnscaled(Image image, int x, int y, int width, int height);
	    public abstract void DrawImageUnscaledAndClipped(Image image, Rectangle rect);
	    public abstract void DrawLine(Pen pen, Point pt1, Point pt2);
	    public abstract void DrawLine(Pen pen, PointF pt1, PointF pt2);
	    public abstract void DrawLine(Pen pen, int x1, int y1, int x2, int y2);
	    public abstract void DrawLine(Pen pen, float x1, float y1, float x2, float y2);
	    public abstract void DrawLines(Pen pen, Point[] points);
	    public abstract void DrawLines(Pen pen, PointF[] points);
	    public abstract void DrawPath(Pen pen, GraphicsPath path);
	    public abstract void DrawPie(Pen pen, Rectangle rect, float startAngle, float sweepAngle);
	    public abstract void DrawPie(Pen pen, RectangleF rect, float startAngle, float sweepAngle);
	    public abstract void DrawPie(Pen pen, int x, int y, int width, int height, int startAngle, int sweepAngle);
	    public abstract void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);
	    public abstract void DrawPolygon(Pen pen, Point[] points);
	    public abstract void DrawPolygon(Pen pen, PointF[] points);
	    public abstract void DrawRectangle(Pen pen, Rectangle rect);
	    public abstract void DrawRectangle(Pen pen, int x, int y, int width, int height);
	    public abstract void DrawRectangle(Pen pen, float x, float y, float width, float height);
	    public abstract void DrawRectangles(Pen pen, Rectangle[] rects);
	    public abstract void DrawRectangles(Pen pen, RectangleF[] rects);
	    public abstract void DrawString(string s, Font font, Brush brush, PointF point);
	    public abstract void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle);
	    public abstract void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format);
	    public abstract void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format);
	    public abstract void DrawString(string s, Font font, Brush brush, float x, float y);
	    public abstract void DrawString(string s, Font font, Brush brush, float x, float y, StringFormat format);
	    public abstract void EndContainer(GraphicsContainer container);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit, EnumerateMetafileProc callback, IntPtr callbackData);
	    public abstract void EnumerateMetafile(Metafile metafile, Point destPoint, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF destPoint, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, Point[] destPoints, Rectangle srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, PointF[] destPoints, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void EnumerateMetafile(Metafile metafile, RectangleF destRect, RectangleF srcRect, GraphicsUnit unit, EnumerateMetafileProc callback, IntPtr callbackData, ImageAttributes imageAttr);
	    public abstract void ExcludeClip(Rectangle rect);
	    public abstract void ExcludeClip(Region region);
	    public abstract void FillClosedCurve(Brush brush, Point[] points);
	    public abstract void FillClosedCurve(Brush brush, PointF[] points);
	    public abstract void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode);
	    public abstract void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode);
	    public abstract void FillClosedCurve(Brush brush, Point[] points, FillMode fillmode, float tension);
	    public abstract void FillClosedCurve(Brush brush, PointF[] points, FillMode fillmode, float tension);
	    public abstract void FillEllipse(Brush brush, Rectangle rect);
	    public abstract void FillEllipse(Brush brush, RectangleF rect);
	    public abstract void FillEllipse(Brush brush, int x, int y, int width, int height);
	    public abstract void FillEllipse(Brush brush, float x, float y, float width, float height);
	    public abstract void FillPath(Brush brush, GraphicsPath path);
	    public abstract void FillPie(Brush brush, Rectangle rect, float startAngle, float sweepAngle);
	    public abstract void FillPie(Brush brush, int x, int y, int width, int height, int startAngle, int sweepAngle);
	    public abstract void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);
	    public abstract void FillPolygon(Brush brush, Point[] points);
	    public abstract void FillPolygon(Brush brush, PointF[] points);
	    public abstract void FillPolygon(Brush brush, Point[] points, FillMode fillMode);
	    public abstract void FillPolygon(Brush brush, PointF[] points, FillMode fillMode);
	    public abstract void FillRectangle(Brush brush, Rectangle rect);
	    public abstract void FillRectangle(Brush brush, RectangleF rect);
	    public abstract void FillRectangle(Brush brush, int x, int y, int width, int height);
	    public abstract void FillRectangle(Brush brush, float x, float y, float width, float height);
	    public abstract void FillRectangles(Brush brush, Rectangle[] rects);
	    public abstract void FillRectangles(Brush brush, RectangleF[] rects);
	    public abstract void FillRegion(Brush brush, Region region);
	    public abstract void Flush();
	    public abstract void Flush(FlushIntention intention);
	    public abstract object GetContextInfo();
	    public abstract IntPtr GetHdc();
	    public abstract Color GetNearestColor(Color color);
	    public abstract void IntersectClip(Rectangle rect);
	    public abstract void IntersectClip(RectangleF rect);
	    public abstract void IntersectClip(Region region);
	    public abstract bool IsVisible(Point point);
	    public abstract bool IsVisible(PointF point);
	    public abstract bool IsVisible(Rectangle rect);
	    public abstract bool IsVisible(RectangleF rect);
	    public abstract bool IsVisible(int x, int y);
	    public abstract bool IsVisible(float x, float y);
	    public abstract bool IsVisible(int x, int y, int width, int height);
	    public abstract bool IsVisible(float x, float y, float width, float height);
	    public abstract Region[] MeasureCharacterRanges(string text, Font font, RectangleF layoutRect, StringFormat stringFormat);
	    public abstract SizeF MeasureString(string text, Font font);
	    public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea);
	    public abstract SizeF MeasureString(string text, Font font, int width);
	    public abstract SizeF MeasureString(string text, Font font, PointF origin, StringFormat stringFormat);
	    public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);
	    public abstract SizeF MeasureString(string text, Font font, int width, StringFormat format);
	    public abstract SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled);
	    public abstract void MultiplyTransform(Matrix matrix);
	    public abstract void MultiplyTransform(Matrix matrix, MatrixOrder order);
	    public abstract void ReleaseHdc();
	    public abstract void ReleaseHdc(IntPtr hdc);
	    public abstract void ReleaseHdcInternal(IntPtr hdc);
	    public abstract void ResetClip();
	    public abstract void ResetTransform();
	    public abstract void Restore(GraphicsState gstate);
	    public abstract void RotateTransform(float angle);
	    public abstract void RotateTransform(float angle, MatrixOrder order);
	    public abstract GraphicsState Save();
	    public abstract void ScaleTransform(float sx, float sy);
	    public abstract void ScaleTransform(float sx, float sy, MatrixOrder order);
	    public abstract void SetClip(GraphicsPath path);
	    public abstract void SetClip(Graphics g);
	    public abstract void SetClip(Rectangle rect);
	    public abstract void SetClip(RectangleF rect);
	    public abstract void SetClip(GraphicsPath path, CombineMode combineMode);
	    public abstract void SetClip(Graphics g, CombineMode combineMode);
	    public abstract void SetClip(Rectangle rect, CombineMode combineMode);
	    public abstract void SetClip(RectangleF rect, CombineMode combineMode);
	    public abstract void SetClip(Region region, CombineMode combineMode);
	    public abstract void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] pts);
	    public abstract void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, PointF[] pts);
	    public abstract void TranslateClip(int dx, int dy);
	    public abstract void TranslateClip(float dx, float dy);
	    public abstract void TranslateTransform(float dx, float dy);
	    public abstract void TranslateTransform(float dx, float dy, MatrixOrder order);
	
	    // Properties
	    public abstract Region Clip { get; set; }
	    public abstract RectangleF ClipBounds { get; }
	    public abstract CompositingMode CompositingMode { get; set; }
	    public abstract CompositingQuality CompositingQuality { get; set; }
	    public abstract float DpiX { get; }
	    public abstract float DpiY { get; }
	    public abstract InterpolationMode InterpolationMode { get; set; }
	    public abstract bool IsClipEmpty { get; }
	    public abstract bool IsVisibleClipEmpty { get; }
	    protected abstract IntPtr NativeGraphicsInternal{ get; }
	    IntPtr IGraphics.NativeGraphics { get{return NativeGraphicsInternal;} }
	    public abstract float PageScale { get; set; }
	    public abstract GraphicsUnit PageUnit { get; set; }
	    public abstract PixelOffsetMode PixelOffsetMode { get; set; }
	    protected abstract object PrintingHelperInternal { get; set; }
	    object IGraphics.PrintingHelper { get{return PrintingHelperInternal;} set{PrintingHelperInternal = value;} }
	    public abstract Point RenderingOrigin { get; set; }
	    public abstract SmoothingMode SmoothingMode { get; set; }
	    public abstract int TextContrast { get; set; }
	    public abstract TextRenderingHint TextRenderingHint { get; set; }
	    public abstract Matrix Transform { get; set; }
	    public abstract RectangleF VisibleClipBounds { get; }
	}
}
