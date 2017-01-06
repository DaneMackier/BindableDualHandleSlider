using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;

namespace mdkstudio.components.dualhandleslider
{
	public class DualHandleSlider : View
	{
		private enum SliderType
		{
			LeftSlider,
			RightSlider
		}

		public delegate void ValueChangedDelegate(float value);

		private float sliderOuterRadius;
		private float sliderInnerRadius;
		private float progressLineHeight;
		private float selectedStrokeWidth;
		private float padding;
		private float grayLineHeight;
		private float sliderRight;
		private float sliderGrayTop;
		private float sliderGrayBottom;
		private float sliderBlueTop;
		private float sliderBlueBottom;
		private float sliderWidthWithoutPadding;
		private float sliderMiddle;

		private RectF rect;

		private Paint paint;

		private Color sliderSelectedColor = Color.Argb(255, 57, 112, 230);
		private Color sliderOuterColor = Color.Argb(255, 57, 112, 230);
		private Color activeProgressColor = Color.Argb(255, 89, 162, 236);
		private Color emptyProgressColor = Color.Argb(255, 216, 216, 216);

		private SliderType? draggingSlider;

		private float _leftValue;
		private float _rightValue = 1f;
		private float _normalizedStep;
		private float _leftPointX;
		private float _rightPointX;
		private float _minValue;
		private float _maxValue;
		private float _step;

		private SliderType lastSlider;

		public event ValueChangedDelegate LeftValueChanged;
		public event ValueChangedDelegate RightValueChanged;

		public float MinValue
		{
			get
			{
				return _minValue;
			}
			set
			{
				_minValue = value;
				if (_minValue > _maxValue)
				{
					_maxValue = _minValue;
				}
				RecalculateNormalizedStep();
			}
		}

		public float MaxValue
		{
			get
			{
				return _maxValue;
			}
			set
			{
				_maxValue = value;
				if (_maxValue < _minValue)
				{
					_minValue = _maxValue;
				}
				RecalculateNormalizedStep();
			}
		}

		public float Step
		{
			get
			{
				return _step;
			}
			set
			{
				_step = value;
				RecalculateNormalizedStep();
			}
		}

		public float LeftValue
		{
			get
			{
				return GetRealValue(_leftValue);
			}
			set
			{
				if (value > RightValue)
				{
					RightValue = value;
				}
				if (UpdatePositionFromValue(DualHandleSlider.SliderType.LeftSlider, GetNormalizedValue(value, 0f), false))
				{
					Invalidate();
				}
			}
		}

		public float RightValue
		{
			get
			{
				return GetRealValue(_rightValue);
			}
			set
			{
				if (value < LeftValue)
				{
					LeftValue = value;
				}
				if (UpdatePositionFromValue(DualHandleSlider.SliderType.RightSlider, GetNormalizedValue(value, 1f), false))
				{
					Invalidate();
				}
			}
		}

		public DualHandleSlider(Context context, IAttributeSet attributes) : base(context, attributes)
		{
			DualHandleSliderAttributeParser customRangeSliderAttributesParser = new DualHandleSliderAttributeParser(attributes);

			Initialize(
        customRangeSliderAttributesParser.MinValue, 
        customRangeSliderAttributesParser.MaxValue, 
        customRangeSliderAttributesParser.Step, 
        customRangeSliderAttributesParser.LeftValue,
        customRangeSliderAttributesParser.RightValue,
        customRangeSliderAttributesParser.Title);
}

		public DualHandleSlider(Context context, 
                            float minValue = 0f, 
                            float maxValue = 1f, 
                            float step = 0f,
                            float leftValue = 0f,
                            float rightValue = 1f,
                            string title = "") : base(context)
		{
      Initialize(minValue, maxValue, step, leftValue, rightValue, title);
		}

    private void Initialize(
      float minValue, 
      float maxValue, 
      float step, 
      float leftValue, 
      float rightValue,
      string title)
		{
			InitDpiDependentParams();

			rect = new RectF();
			paint = new Paint(Android.Graphics.PaintFlags.AntiAlias);
			paint.StrokeWidth = (selectedStrokeWidth);
			paint.SetStyle(Paint.Style.Fill);
			MinValue = minValue;
			MaxValue = maxValue;
			Step = step;
			LeftValue = leftValue;
			RightValue = rightValue;
		}

		private void InitDpiDependentParams()
		{
			float lineHeight = 22f;

			float density = Resources.DisplayMetrics.Density;
			sliderOuterRadius = ((lineHeight / 2) + 5f) * density;
			sliderInnerRadius = 5f * density;
			progressLineHeight = lineHeight * density;
			selectedStrokeWidth = 2f * density;
			grayLineHeight = lineHeight * density;
			padding = sliderOuterRadius + selectedStrokeWidth;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			switch (e.ActionIndex)
			{
				case 0:
					draggingSlider = new DualHandleSlider.SliderType?(DetectClosestSlider(e.GetX()));
					UpdatePositionFromCoordinate(draggingSlider.Value, e.GetX());
					Invalidate();
					break;
				case 1:
				case 3:
					draggingSlider = default(DualHandleSlider.SliderType?);
					Invalidate();
					break;
				case 2:
					if (draggingSlider.HasValue && UpdatePositionFromCoordinate(draggingSlider.Value, e.GetX()))
					{
						Invalidate();
					}
					break;
			}
			return true;
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			sliderMiddle = h / 2;

			sliderWidthWithoutPadding = w - 2f * padding;
			sliderRight = w - padding;

			sliderGrayTop = ((int)Math.Round((sliderMiddle - grayLineHeight / 2f)));
			sliderGrayBottom = sliderMiddle + grayLineHeight / 2f;

			sliderBlueTop = ((int)Math.Round((sliderMiddle - progressLineHeight / 2f)));
			sliderBlueBottom = sliderMiddle + progressLineHeight / 2f;


			Console.WriteLine($"sliderBlueTop: {sliderBlueTop}, sliderBlueBottom:{sliderBlueBottom}, sliderMiddler: {sliderMiddle}");

			UpdatePositionFromValue(DualHandleSlider.SliderType.LeftSlider, _leftValue, true);
			UpdatePositionFromValue(DualHandleSlider.SliderType.RightSlider, _rightValue, true);
		}

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			int num = (int)FloatMath.Floor(4f * sliderOuterRadius);
			int num2 = View.ResolveSize(num, widthMeasureSpec);
			int num3 = (int)FloatMath.Floor(2f * padding);
			int num4 = View.ResolveSize(num3, heightMeasureSpec);
			base.SetMeasuredDimension(num2, num4);
		}

		private DualHandleSlider.SliderType DetectClosestSlider(float x)
		{
			if (_rightPointX - _leftPointX < 1f)
			{
				return lastSlider;
			}
			return (x <= (_leftPointX + _rightPointX) / 2f) ? DualHandleSlider.SliderType.LeftSlider : DualHandleSlider.SliderType.RightSlider;
		}

		private bool UpdatePositionFromCoordinate(DualHandleSlider.SliderType slider, float x)
		{
			lastSlider = slider;
			return UpdatePositionFromValue(slider, (x - padding) / sliderWidthWithoutPadding, false);
		}

		private bool UpdatePositionFromValue(DualHandleSlider.SliderType slider, float value, bool forced = false)
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
			if (slider == DualHandleSlider.SliderType.RightSlider && value < _leftValue)
			{
				value = _leftValue;
			}
			else if (slider == DualHandleSlider.SliderType.LeftSlider && value > _rightValue)
			{
				value = _rightValue;
			}
			float num = value * sliderWidthWithoutPadding + padding;
			if (slider != DualHandleSlider.SliderType.RightSlider)
			{
				if (slider == DualHandleSlider.SliderType.LeftSlider)
				{
					if (_leftValue != value || forced)
					{
						_leftValue = value;
						_leftPointX = num;
						if (!forced)
						{
							NotifyPositionChange(LeftValueChanged, value);
						}
						return true;
					}
				}
			}
			else if (_rightValue != value || forced)
			{
				_rightValue = value;
				_rightPointX = num;
				if (!forced)
				{
					NotifyPositionChange(RightValueChanged, value);
				}
				return true;
			}
			return false;
		}

		private void NotifyPositionChange(DualHandleSlider.ValueChangedDelegate changedDelegate, float newValue)
		{
			if (changedDelegate != null)
			{
				changedDelegate(GetRealValue(newValue));
			}
		}

		private float GetRealValue(float value)
		{
			return (_maxValue - _minValue) * value + _minValue;
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
			if (_maxValue != _minValue)
			{
				_normalizedStep = _step / (_maxValue - _minValue);
			}
			else
			{
				_normalizedStep = 0f;
			}
		}

		private float LimitValueToStep(float value)
		{
			if (_normalizedStep == 0f)
			{
				return value;
			}
			return _normalizedStep * (float)Math.Round((double)(value / _normalizedStep));
		}

		#region Custom Draw

		protected override void OnDraw(Canvas canvas)
		{
			// draw empty bar
			rect.Set(padding, sliderGrayTop, sliderRight, sliderGrayBottom);
			paint.Color = (emptyProgressColor);
			canvas.DrawRect(rect, paint);

			// draw progress bar
			rect.Set(_leftPointX, sliderBlueTop, _rightPointX, sliderBlueBottom);
			paint.Color = (activeProgressColor);
			canvas.DrawRect(rect, paint);

			// draw circles
			DrawSlider(canvas, _leftPointX, sliderMiddle,
			  draggingSlider.HasValue && draggingSlider.Value == SliderType.LeftSlider);
			DrawSlider(canvas, _rightPointX, sliderMiddle,
			  draggingSlider.HasValue && draggingSlider.Value == SliderType.RightSlider);
		}

		private void DrawSlider(Canvas canvas, float x, float y, bool pressed)
		{
			//      if (pressed)
			//      {
			//        paint.Color = (sliderSelectedColor);
			//        canvas.DrawCircle(x, y, sliderOuterRadius, paint);
			//        paint.SetStyle(Paint.Style.Stroke);
			//        paint.Color = (sliderInnerColor);
			//        canvas.DrawCircle(x, y, sliderOuterRadius, paint);
			//        paint.SetStyle(Paint.Style.Fill);
			//      }
			//      else
			//      {
			//        paint.Color = (sliderOuterColor);
			//        canvas.DrawCircle(x, y, sliderOuterRadius, paint);
			//      }
			//      paint.Color = (sliderInnerColor);
			//      canvas.DrawCircle(x, y, sliderInnerRadius, paint);

			paint.Color = (sliderOuterColor);
			canvas.DrawCircle(x, y, sliderOuterRadius, paint);
		}

		#endregion


	}
}