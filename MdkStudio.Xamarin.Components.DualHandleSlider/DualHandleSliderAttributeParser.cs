using System;
using Android.Content;
using Android.Util;

namespace MdkStudio.Xamarin.Components.DualHandleSlider.Droid
{
  public class DualHandleSliderAttributeParser
  {
    public readonly float MinValue;
    public readonly float MaxValue;
    public readonly float Step;
    public readonly float LeftValue;
    public readonly float RightValue;
    public readonly string Title;
    public readonly bool HideValues;
    public readonly int HandleDrawable;

    public readonly int TitleMarginLeft;
    public readonly int TitleMarginBottom;
    public readonly int LeftValueMarginLeft;
    public readonly int LeftValueMarginTop;
    public readonly int RightValueMarginRight;
    public readonly int RightValueMarginTop;
    public readonly int ValuesTopMargin;

    private Context _cachedContext;

    public DualHandleSliderAttributeParser(IAttributeSet attributes, Context context)
    {
      _cachedContext = context;

      MinValue = ParseFloat(attributes, "min_value", 0f);
      MaxValue = ParseFloat(attributes, "max_value", 1f);
      Step = ParseFloat(attributes, "step", 0f);
      LeftValue = ParseFloat(attributes, "left_value", 0f);
      RightValue = ParseFloat(attributes, "right_value", 1f);
      Title = GetString(attributes, "title");
      HideValues = GetBool(attributes, "hide_values");
      HandleDrawable = GetDrawableId(attributes, "handle_drawable");

      TitleMarginLeft = GetPixelValue(attributes, "title_marginLeft");
      TitleMarginBottom = GetPixelValue(attributes, "title_marginBottom");
      LeftValueMarginLeft = GetPixelValue(attributes, "leftValue_marginLeft");
      RightValueMarginRight = GetPixelValue(attributes, "rightValue_marginRight");
      ValuesTopMargin = GetPixelValue(attributes, "values_marginTop");
    }

    private float ParseFloat(IAttributeSet attributes, string attributeName, float defaultNumber)
    {
      string attributeValue = attributes.GetAttributeValue(null, attributeName);
      if (string.IsNullOrEmpty(attributeValue))
      {
        return defaultNumber;
      }
      float result;
      if (float.TryParse(attributeValue, out result))
      {
        return result;
      }
      throw new ArgumentException(string.Format("Wrong {0} value '{1}'. Must be a number", attributeName, attributeValue));
    }

    private string GetString(IAttributeSet attributes, string attributeName)
    {
      string attributeValue = attributes.GetAttributeValue(null, attributeName);

      if (string.IsNullOrEmpty(attributeValue))
      {
        return string.Empty;
      }

      return attributeValue;
    }

    private bool GetBool(IAttributeSet attributes, string attributeName)
    {
      string attributeValue = attributes.GetAttributeValue(null, attributeName);

      if (string.IsNullOrEmpty(attributeValue))
      {
        return false;
      }

      bool result;
      if (bool.TryParse(attributeValue, out result))
      {
        return result;
      }

      throw new ArgumentException(string.Format("Wrong {0} value '{1}'. Must be a true or false", attributeName, attributeValue));
    }

    private int GetPixelValue(IAttributeSet attributes, string attributeName)
    {
      string attributeValue = attributes.GetAttributeValue(null, attributeName);

      if (string.IsNullOrEmpty(attributeValue))
      {
        return 0;
      }

      if (attributeValue.Contains("dp"))
      {
        string[] delimiters = { "dp" };
        var splitValues = attributeValue.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        if (splitValues.Length != 1)
        {
          throw new ArgumentException(string.Format("Wrong parameter for {0}, value must have number followed by dp", attributeName));
        }

        attributeValue = splitValues[0];
      }

      var pixelValue = DpToPixel(int.Parse(attributeValue));
      return pixelValue;
    }

    private int DpToPixel(int dp)
    {
      DisplayMetrics displayMetrics = _cachedContext.Resources.DisplayMetrics;
     
      return (int)Math.Round(dp * (displayMetrics.Xdpi / (float)DisplayMetrics.DensityDefault));
    }

    private int GetDrawableId(IAttributeSet attributes, string attributeName)
    {
      string attributeValue = attributes.GetAttributeValue(null, attributeName);

      if (string.IsNullOrEmpty(attributeValue))
      {
        return -1;
      }

      var stringHasDrawablePrefix = attributeValue.Contains("@drawable/");
      if (stringHasDrawablePrefix)
      {
        attributeValue = attributeValue.Replace("@drawable/", "");
      }

      var resourceId = GetResourceId(attributeValue, "drawable", _cachedContext.PackageName);
      return resourceId;

    }

    private int GetResourceId(String pVariableName, String pResourcename, String pPackageName)
    {
      try
      {
        return _cachedContext.Resources.GetIdentifier(pVariableName, pResourcename, pPackageName);
      }
      catch (Exception e)
      {
        throw new ArgumentException(string.Format("Could not find drawable bitmap {0}. Check your format is correct. e.g. @drawable/handle_bitmap", pVariableName));
      }
    }
  }
}