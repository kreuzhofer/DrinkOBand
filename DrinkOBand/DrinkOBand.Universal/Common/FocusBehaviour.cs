using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace DrinkOBand.Universal.Common
{
    public class FocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.GotFocus += (sender, args) => IsFocused = true;
            AssociatedObject.LostFocus += (sender, a) => IsFocused = false;
            base.OnAttached();
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(FocusBehavior),
                new PropertyMetadata(false));

        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set
            {
                SetValue(IsFocusedProperty, value);
                if (value)
                {
                    AssociatedObject.Focus(FocusState.Programmatic);
                }
            }
        }
    }
}