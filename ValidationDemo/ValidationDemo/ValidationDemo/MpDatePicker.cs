using System;
using Xamarin.Forms;

namespace ValidationDemo
{
    public class MpDatePicker : DatePicker, IControlValidation
    {
        #region Properties
        public BaseControlValidation<MpDatePicker> _Validate;
        #endregion

        #region Constructor
        public MpDatePicker()
        {
            _Validate = new BaseControlValidation<MpDatePicker>(
                this,
                DatePicker.DateProperty.PropertyName,
                SetPrivateFilelds);
        }

        private void SetPrivateFilelds(bool _hasError, string _errorMessage)
        {
            // Set private fields
            this.HasError = _hasError;
            this.ErrorMessage = _errorMessage;
        }
        #endregion

        #region HasError

        public static readonly BindableProperty HasErrorProperty =
           BindableProperty.Create("HasError", typeof(bool), typeof(MpDatePicker), false);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
           BindableProperty.Create("ErrorMessage", typeof(string), typeof(MpDatePicker), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            private set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
           BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(MpDatePicker), false, propertyChanged: OnPropertyChanged);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            MpDatePicker control = bindable as MpDatePicker;
            if (control != null && control.BindingContext != null)
            {
                control._Validate.CheckValidation();
            }
        }

        public bool ShowErrorMessage
        {
            get { return (bool)GetValue(ShowErrorMessageProperty); }
            set { SetValue(ShowErrorMessageProperty, value); }
        }
        #endregion

        #region On Binding context chagned
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this._Validate.CheckValidation(); // Check for validation
        }
        #endregion
    }
}
