using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.Droid
{
  public class BindableDualHandleSlider : RelativeLayout
  {
    private DualHandleSliderAttributeParser _attributeParser;

    private TextView _titleText;
    private TextView _leftTextView;
    private TextView _rightTextView;

    private DualHandleSlider _dualRangeSlider;

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
        _leftTextView.Text = $"{LeadingFormatSymbol} {_leftValue} {TrailingFormatSymbol}";

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
        _rightTextView.Text = $"{LeadingFormatSymbol} {_rightValue} {TrailingFormatSymbol}";

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
        _dualRangeSlider.MinValue = _minValue;

        if (LeftValue == 0)
        {
          LeftValue = _minValue;
        }

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
        _dualRangeSlider.MaxValue = _maxValue;

        if (RightValue == 0)
        {
          RightValue = _maxValue;
        }

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
        _titleText.Text = _title;

        if (TitleValueChanged != null)
        {
          TitleValueChanged(this, EventArgs.Empty);
        }
      }
    }

    public BindableDualHandleSlider(Context context, IAttributeSet attrs) :
      base(context, attrs)
    {
      _attributeParser = new DualHandleSliderAttributeParser(attrs, context);

      Initialize(context);
    }

    private void Initialize(Context context)
    {
      InitializeTitleView(context);
      InitializeDualHandleSlider(context);
      InitializeValuesViews(context);
      InitializeDefaultValues();
    }

    private void InitializeTitleView(Context context)
    {
      var titleColor = Color.AliceBlue;
      _titleText = new TextView(context);
      _titleText.SetTextColor(titleColor);
      _titleText.SetTextSize(ComplexUnitType.Sp, 18);
      _titleText.SetTypeface(_titleText.Typeface, Typeface.Default.Style);
      _titleText.Id = GenerateViewId();

      var titleLayoutParams = new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

      titleLayoutParams.LeftMargin = _attributeParser.TitleMarginLeft;
      titleLayoutParams.BottomMargin = _attributeParser.TitleMarginBottom;

      AddView(_titleText, titleLayoutParams);
    }

    private void InitializeDualHandleSlider(Context context)
    {
      var rangeLayoutParams = new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
      rangeLayoutParams.Width = ViewGroup.LayoutParams.MatchParent;
      rangeLayoutParams.AddRule(LayoutRules.Below, _titleText.Id);

      _dualRangeSlider = new DualHandleSlider(context,
                                              _attributeParser.MaxValue,
                                              _attributeParser.MinValue,
                                              _attributeParser.Step,
                                              _attributeParser.LeftValue,
                                              _attributeParser.RightValue,
                                              _attributeParser.Title);
      
      _dualRangeSlider.HandleDrawableId = _attributeParser.HandleDrawable;
      _dualRangeSlider.Id = GenerateViewId();
      _dualRangeSlider.LeftValueChanged += OnLeftValueChanged;
      _dualRangeSlider.RightValueChanged += OnRightValueChanged;

      AddView(_dualRangeSlider, rangeLayoutParams);
    }

    private void InitializeValuesViews(Context context)
    {
      var labelColor = Color.White;
      _leftTextView = new TextView(context);
      _leftTextView.SetTypeface(_leftTextView.Typeface, Typeface.DefaultBold.Style);
      _leftTextView.SetTextColor(labelColor);
      _leftTextView.Text = $"{LeadingFormatSymbol} {_leftValue} {TrailingFormatSymbol}";

      _rightTextView = new TextView(context);
      _rightTextView.SetTypeface(_rightTextView.Typeface, Typeface.DefaultBold.Style);
      _rightTextView.SetTextColor(labelColor);

      var leftLayoutParams = new LayoutParams(
        ViewGroup.LayoutParams.WrapContent, 
        ViewGroup.LayoutParams.WrapContent);
      leftLayoutParams.AddRule(LayoutRules.AlignParentLeft);
      leftLayoutParams.AddRule(LayoutRules.Below, _dualRangeSlider.Id);
      leftLayoutParams.LeftMargin = _attributeParser.LeftValueMarginLeft;
      leftLayoutParams.TopMargin = _attributeParser.ValuesTopMargin;

      var rightLayoutParams = new LayoutParams(
        ViewGroup.LayoutParams.WrapContent,
        ViewGroup.LayoutParams.WrapContent);
      rightLayoutParams.AddRule(LayoutRules.AlignParentRight);
      rightLayoutParams.AddRule(LayoutRules.Below, _dualRangeSlider.Id);
      rightLayoutParams.RightMargin = _attributeParser.RightValueMarginRight;
      rightLayoutParams.TopMargin = _attributeParser.ValuesTopMargin;

      AddView(_leftTextView, leftLayoutParams);
      AddView(_rightTextView, rightLayoutParams);
    }

    private void InitializeDefaultValues()
    {
      Title = _attributeParser.Title;

      MaxValue = (int)_attributeParser.MaxValue;
      MinValue = (int)_attributeParser.MinValue;

      if (_attributeParser.HideValues)
      {
        _leftTextView.Visibility = ViewStates.Invisible;
        _rightTextView.Visibility = ViewStates.Invisible;
      }
    }

    private void OnRightValueChanged(float value)
    {
      RightValue = (int)Math.Ceiling(value);
    }

    private void OnLeftValueChanged(float value)
    {
      LeftValue = (int)Math.Ceiling(value);
    }
  }
}