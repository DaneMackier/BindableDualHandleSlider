using Foundation;
using System;
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.iOS
{
  public partial class BindableRangeSlider : UIView
  {
    private CustomRangeSlider _slider;

    public UILabel TitleLabel;
    public UILabel LeftLabel;
    public UILabel RightLabel;

    public event EventHandler LeftValueChanged;
    public event EventHandler RightValueChanged;
    public event EventHandler MinValueChanged;
    public event EventHandler MaxValueChanged;
    public event EventHandler TitleValueChanged;

    private int _leftValue;
    private int _rightValue;
    private int _minValue;
    private int _maxValue;
    private string _title;
    private string _leadingFormatSymbol;
    private string _trailingFormatSymbol;

    public string LeadingFormatSymbol
    {
      get { return _leadingFormatSymbol; }
      set { _leadingFormatSymbol = value; }
    }

    public int LeftValue
    {
      get { return _leftValue; }
      set
      {
        _leftValue = value;
        LeftLabel.Text = $"{LeadingFormatSymbol} {_leftValue} {TrailingFormatSymbol}";

        if (_leftValue == MinValue)
        {
          LeftLabel.Text = "Any";
        }

        if (LeftValueChanged != null)
        {
          LeftValueChanged(this, EventArgs.Empty);
        }
      }
    }

    public int RightValue
    {
      get { return _rightValue; }
      set
      {
        _rightValue = value;
        RightLabel.Text = $"{LeadingFormatSymbol} {_rightValue} {TrailingFormatSymbol}";

        if (_rightValue == MaxValue)
        {
          RightLabel.Text = "Any";
        }

        if (RightValueChanged != null)
        {
          RightValueChanged(this, EventArgs.Empty);
        }
      }
    }

    public string TrailingFormatSymbol
    {
      get { return _trailingFormatSymbol; }
      set { _trailingFormatSymbol = value; }
    }

    public int MinValue
    {
      get { return _minValue; }
      set
      {
        _minValue = value;

        if (_slider != null)
        {
          _slider.MinValue = _minValue;
        }

        LeftValue = _minValue;

        if (MinValueChanged != null)
        {
          MinValueChanged(this, EventArgs.Empty);
        }
      }
    }
    public int MaxValue
    {
      get { return _maxValue; }
      set
      {
        _maxValue = value;

        if (_slider != null)
        {
          _slider.MaxValue = _maxValue;
        }

        RightValue = _maxValue;

        if (MaxValueChanged != null)
        {
          MaxValueChanged(this, EventArgs.Empty);
        }
      }
    }

    public string Title
    {
      get { return _title; }
      set
      {
        _title = value;
        TitleLabel.Text = _title;
      }
    }

    public BindableRangeSlider (IntPtr handle) : base (handle)
    {
      TitleLabel = new UILabel();
			TitleLabel.TextColor = UIColor.White;
      AddSubview(TitleLabel);

      LeftLabel = new UILabel();
      LeftLabel.TextColor = UIColor.White;
      AddSubview(LeftLabel);

      RightLabel = new UILabel();
      RightLabel.TextColor = UIColor.White;
      AddSubview(RightLabel);
    }

    public override void LayoutSubviews ()
    {
      base.LayoutSubviews ();

      InitializeSlider();
      InitializeTitleLabel();
      InitializeValueLabels();
    }

    private void InitializeTitleLabel()
    {
      var titleFont = UIFont.FromName ("Helvetica-Light", 15f);
      var titleLabelHeight = 25;
      var sliderYPosition = _slider.Frame.Y;
      var titleLabelTopPadding = 15;
      var titleLabelYPosition = sliderYPosition - titleLabelHeight - titleLabelTopPadding;


      TitleLabel.Font = titleFont;
      TitleLabel.Frame = new CGRect(25, titleLabelYPosition,100, titleLabelHeight);
    }

    private void InitializeValueLabels()
    {
      var labelPadding = 25;
      var valueLabelHeight = 20;
      var valueLabelTopPadding = 15;
      var sliderBottomYPosition = _slider.Frame.GetMaxY ();
      var valueLabelYPosition = sliderBottomYPosition + valueLabelTopPadding;
      var valueLabelWidth = RightLabel.Text.GetLabelWidth ();

      var valueLabelFont = UIFont.FromName("Helvetica-Bold", 11f);
      LeftLabel.Frame = new CGRect(labelPadding, 
                                   valueLabelYPosition, 
                                   valueLabelWidth, 
                                   valueLabelHeight);
      LeftLabel.Font = valueLabelFont;

      RightLabel.Frame = new CGRect(Frame.Width - valueLabelWidth - labelPadding, 
                                    valueLabelYPosition, 
                                    valueLabelWidth, 
                                    valueLabelHeight);
      RightLabel.Font = valueLabelFont;
      RightLabel.TextAlignment = UITextAlignment.Right;
    }

    private void InitializeSlider()
    {
      if (_slider == null)
      {
        float totalMarginWidth = 50;
        var sliderWidth = (float) (Frame.Width - totalMarginWidth);

        float sliderHeight = CustomRangeSlider.BarHeight;
        _slider = new CustomRangeSlider
        {
          Frame =
            new RectangleF(totalMarginWidth/2, 
                           (float) (Frame.Height/2) - sliderHeight/2, 
                           sliderWidth, 
                           sliderHeight),
        };
        AddSubview(_slider);

        _slider.RightValueChanged += OnRightValueChanged;
        _slider.LeftValueChanged += OnLeftValueChanged;
      }
    }

    private void OnRightValueChanged(float value)
    {
      RightValue = (int)value;
    }

    private void OnLeftValueChanged(float value)
    {
      LeftValue = (int)value;
    }
  }
}