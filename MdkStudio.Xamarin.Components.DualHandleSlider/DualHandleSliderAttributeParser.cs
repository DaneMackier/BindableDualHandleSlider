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

    public DualHandleSliderAttributeParser(IAttributeSet attributes)
    {
      MinValue = ParseFloat(attributes, "min_value", 0f);
      MaxValue = ParseFloat(attributes, "max_value", 1f);
      Step = ParseFloat(attributes, "step", 0f);
      LeftValue = ParseFloat(attributes, "left_value", 0f);
      RightValue = ParseFloat(attributes, "right_value", 1f);
      Title = GetString(attributes, "title");
      HideValues = GetBool(attributes, "hide_values");
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

    //public int DpToPixel(int dp)
    //{
    //  DisplayMetrics displayMetrics = _context.Resources.DisplayMetrics;
    //  return (int)Math.Ceiling(dp *  displayMetrics.Density);
    //}
  }
}