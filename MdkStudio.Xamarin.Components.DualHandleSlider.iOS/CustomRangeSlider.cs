using System;
using System.Drawing;
using CoreGraphics;
using UIKit;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.iOS
{
	public class CustomRangeSlider : UIView
	{
		public delegate void ValueChangedDelegate(float value);

		private enum SliderType
		{
			LeftSlider,
			RightSlider
		}

		public static float BarHeight = 25f;
		public static float SliderSize = 35f;
		public static int StretchImageWidthCap = 16;

		private UIImageView backBarImageView;

		private ActiveSliderView activeSliderView;

		private SliderHandleControl leftSliderControl;

		private SliderHandleControl rightSliderControl;

		private float minValue;

		private float maxValue;

		private float step;

		private float leftValue = -1f;

		private float rightValue = -1f;

		private float normalizedStep;

		private bool initialized = false;

		public event ValueChangedDelegate LeftValueChanged;

		public event ValueChangedDelegate RightValueChanged;

		public override CGRect Frame
		{
			get { return base.Frame; }
			set
			{
				if (base.Frame != value)
				{
					base.Frame = value;
					OnFrameChanged();
				}
			}
		}

		public float MinValue
		{
			get
			{
				return minValue;
			}
			set
			{
				minValue = value;
				if (minValue > maxValue)
				{
					maxValue = minValue;
				}
				RecalculateNormalizedStep();
			}
		}

		public float MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{
				maxValue = value;
				if (maxValue < minValue)
				{
					minValue = maxValue;
				}
				RecalculateNormalizedStep();
			}
		}

		public float Step
		{
			get
			{
				return step;
			}
			set
			{
				step = value;
				RecalculateNormalizedStep();
			}
		}

		public float LeftValue
		{
			get
			{
				return GetRealValue(leftValue);
			}
			set
			{
				if (value > maxValue)
				{
					value = maxValue;
				}
				if (value < minValue)
				{
					value = minValue;
				}
				UpdatePositionFromValue(SliderType.LeftSlider, GetNormalizedValue(value, 0f));
			}
		}

		public float RightValue
		{
			get
			{
				return GetRealValue(rightValue);
			}
			set
			{
				if (value > maxValue)
				{
					value = maxValue;
				}
				if (value < minValue)
				{
					value = minValue;
				}
				UpdatePositionFromValue(SliderType.RightSlider, GetNormalizedValue(value, 1f));
			}
		}

		public CustomRangeSlider(RectangleF frame)
		{
			InitViews();
			Frame = frame;
		}

		public CustomRangeSlider()
		{
			Bounds = new RectangleF();
			InitViews();
		}

		private void InitViews()
		{
			InitSubviews();
			InitDefaultValues();
			initialized = true;
		}

		private void InitDefaultValues()
		{
			MinValue = 0f;
			MaxValue = 1f;
			Step = 0f;
			LeftValue = 0f;
			RightValue = 1f;
		}

		private void InitSubviews()
		{
			var sliderYPosition = (BarHeight / 2) - SliderSize / 2;
			var leftSliderFrame = new RectangleF(new PointF(0, sliderYPosition), new SizeF(SliderSize, SliderSize));
			var rightSliderFrame = new RectangleF(new PointF(0, sliderYPosition), new SizeF(SliderSize, SliderSize));
			leftSliderControl = new SliderHandleControl(leftSliderFrame);
			leftSliderControl.Moved += OnLeftHandleMoved;
			rightSliderControl = new SliderHandleControl(rightSliderFrame);
			rightSliderControl.Moved += OnRightHandleMoved;
			activeSliderView = new ActiveSliderView();
			backBarImageView = new UIImageView(UIImage.FromBundle("Images/CustomRangeSlider/inactive_progress_slider.png").StretchableImage(StretchImageWidthCap, StretchImageWidthCap));
			AddSubview(backBarImageView);
			AddSubview(activeSliderView);
			AddSubview(leftSliderControl);
			AddSubview(rightSliderControl);

		}

		private void UpdatePositionFromValue(SliderType slider, float value)
		{
			if (value < 0f)
			{
				value = 0f;
			}
			else if (value > 1f)
			{
				value = 1f;
			}
			value = LimitValueToStep(value);
			if (slider == SliderType.RightSlider && value < leftValue)
			{
				value = leftValue;
			}
			else if (slider == SliderType.LeftSlider && value > rightValue)
			{
				value = rightValue;
			}
			if (slider != SliderType.RightSlider)
			{
				if (slider == SliderType.LeftSlider)
				{
					if (leftValue != value)
					{
						leftValue = value;
						SetLeftValue(leftValue);
						NotifyPositionChange(LeftValueChanged, value);
					}
				}
			}
			else if (rightValue != value)
			{
				rightValue = value;
				SetRightValue(value);
				NotifyPositionChange(RightValueChanged, value);
			}
		}

		private float GetRealValue(float value)
		{
			return (maxValue - minValue) * value + minValue;
		}

		private float GetNormalizedValue(float realValue, float defaultValue)
		{
			if (MaxValue != MinValue)
			{
				return (realValue - MinValue) / (MaxValue - MinValue);
			}
			return defaultValue;
		}

		private void RecalculateNormalizedStep()
		{
			if (maxValue != minValue)
			{
				normalizedStep = step / (maxValue - minValue);
			}
			else
			{
				normalizedStep = 0f;
			}
		}

		private float LimitValueToStep(float value)
		{
			if (normalizedStep == 0f)
			{
				return value;
			}
			return normalizedStep * (float)Math.Round((double)(value / normalizedStep));
		}

		private void NotifyPositionChange(ValueChangedDelegate changedDelegate, float newValue)
		{
			if (changedDelegate != null)
			{
				changedDelegate(GetRealValue(newValue));
			}
		}

		private void OnRightHandleMoved(float rightHandlePosition)
		{
			UpdatePositionFromValue(SliderType.RightSlider, LimitValueToStep((float)(rightHandlePosition / Bounds.Width)));
		}

		private void OnLeftHandleMoved(float leftHandlePosition)
		{
			UpdatePositionFromValue(SliderType.LeftSlider, LimitValueToStep((float)(leftHandlePosition / Bounds.Width)));
		}

		private void SetLeftValue(float value)
		{
			float num = value * (float)Bounds.Width;
			num = LimitXPosition(num);
			activeSliderView.LimitLeft(num);
			leftSliderControl.Center = new PointF(num, (float)leftSliderControl.Center.Y);
			if (leftSliderControl.Frame.Right > rightSliderControl.Frame.Left)
			{
				BringSubviewToFront(leftSliderControl);
			}
		}

		private void SetRightValue(float value)
		{
			float num = value * (float)Bounds.Width;
			num = LimitXPosition(num);
			activeSliderView.LimitRight(num);
			rightSliderControl.Center = (new PointF(num, (float)rightSliderControl.Center.Y));
			if (rightSliderControl.Frame.Left < leftSliderControl.Frame.Right)
			{
				BringSubviewToFront(rightSliderControl);
			}
		}

		private float LimitXPosition(float newX)
		{
			float halfHandleWidth = (float)leftSliderControl.Bounds.Width / 2;
			float num = (float)activeSliderView.Bounds.Size.Width - halfHandleWidth;
			if (newX < halfHandleWidth)
			{
				newX = halfHandleWidth;
			}
			if (newX > num)
			{
				newX = num;
			}
			return newX;
		}

		private void OnFrameChanged()
		{
			if (!initialized)
			{
				InitViews();
			}

			float backBarSidePadding = 10f;
			backBarImageView.Frame = (new RectangleF(backBarSidePadding / 2, 0, (float)Bounds.Width - backBarSidePadding, CustomRangeSlider.BarHeight));
			activeSliderView.Frame = (new RectangleF(new PointF(0f, 0), new SizeF((float)Bounds.Size.Width, (float)Bounds.Size.Height)));
			SetLeftValue(GetNormalizedValue(LeftValue, 0f));
			SetRightValue(GetNormalizedValue(RightValue, 1f));
		}
	}
}
