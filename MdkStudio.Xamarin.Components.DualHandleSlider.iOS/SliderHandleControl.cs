using System;
using System.Drawing;
using UIKit;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.iOS
{
	internal class SliderHandleControl : UIControl
	{
		private UIImageView handleImageView;

		public event Action<float> Moved;

		public SliderHandleControl(RectangleF frame)
		{
			Frame = frame;
			InitSubviews();
			UILongPressGestureRecognizer uILongPressGestureRecognizer = new UILongPressGestureRecognizer(delegate (UILongPressGestureRecognizer gesture)
			{
				if (gesture.State == UIGestureRecognizerState.Began)
				{
					this.handleImageView.Highlighted = (true);
				}
				if (this.Moved != null)
				{
					float x = (float)gesture.LocationInView(this.Superview).X;
					this.Moved.Invoke(x);
				}
				if (gesture.State == UIGestureRecognizerState.Ended || gesture.State == UIGestureRecognizerState.Cancelled)
				{
					this.handleImageView.Highlighted = false;
				}
			});
			uILongPressGestureRecognizer.MinimumPressDuration = 0.0;
			AddGestureRecognizer(uILongPressGestureRecognizer);
		}

		private void InitSubviews()
		{
			handleImageView = new UIImageView(UIImage.FromBundle("SliderHandle"));
			handleImageView.HighlightedImage = (UIImage.FromBundle("SliderHandle"));
			handleImageView.Frame = Bounds;
			AddSubview(this.handleImageView);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			//      this.handleImageView.Center = (new PointF((float)RectangleFExtensions.GetMidX(this.Frame), (float)RectangleFExtensions.GetMidY(this.Frame)));
		}
	}
}
