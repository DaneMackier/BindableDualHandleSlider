using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using UIKit;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.iOS
{
  internal sealed class ActiveSliderView : UIView
  {
    private UIImageView activeImageView;

    private static float DefaultLeftPoint;

    private static float DefaultRightPoint = 300f;

    private float rightLimitPoint;

    private float leftLimitPoint;

    public ActiveSliderView()
    {
      this.leftLimitPoint = ActiveSliderView.DefaultLeftPoint;
      this.rightLimitPoint = ActiveSliderView.DefaultRightPoint;
      this.Bounds = new RectangleF(0f, 0f, ActiveSliderView.DefaultRightPoint, CustomRangeSlider.BarHeight);
      this.InitSubviews();
    }

    private void InitSubviews()
    {
      this.activeImageView = new UIImageView(UIImage.FromBundle("Images/CustomRangeSlider/active_progress_slider.png").StretchableImage(CustomRangeSlider.StretchImageWidthCap, 0));
      this.AddSubview(this.activeImageView);
    }

    public void LimitRight(float rightLimitPoint)
    {
      this.rightLimitPoint = rightLimitPoint;
      this.SetNeedsLayout();
    }

    public void LimitLeft(float leftLimitPoint)
    {
      this.leftLimitPoint = leftLimitPoint;
      this.SetNeedsLayout();
    }

    public override void LayoutSubviews()
    {
      this.activeImageView.Frame = new RectangleF(this.leftLimitPoint, 0f, this.rightLimitPoint - this.leftLimitPoint, CustomRangeSlider.BarHeight);
    }
  }
}
