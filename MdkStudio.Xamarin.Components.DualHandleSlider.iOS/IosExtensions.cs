using System;
namespace MdkStudio.Xamarin.Components.DualHandleSlider.iOS
{
  public static class IosExtensions
  {
    public static float GetLabelWidth (this string text, float charWidth = 9.5f)
    {
      var textLength = text.Length;
      return textLength * charWidth;
    }
  }
}

