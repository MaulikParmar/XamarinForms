using System.ComponentModel;
using Xamarin.Forms;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ValidationDemo
{
    // Under testing
    public class MpEntry : Entry, IControlValidation
    {
        #region Property
        private INotifyDataErrorInfo _NotifyErrors;
        private string BindingPath = "";
        private INotifyScrollToProperty _NotifyScroll;
        #endregion

        #region Has Error
        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create("HasError", typeof(bool), typeof(MpEntry), false, defaultBindingMode:BindingMode.TwoWay);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
           BindableProperty.Create("ErrorMessage", typeof(string), typeof(MpEntry), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
           BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(MpEntry), false, propertyChanged:OnShowErrorMessageChanged, defaultBindingMode: BindingMode.TwoWay);

        private static void OnShowErrorMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // execute on bindable context changed method
            MpEntry control = bindable as MpEntry;
            if (control != null && control.BindingContext != null)
            {
                control.CheckValidation();
            }
        }

        public bool ShowErrorMessage
        {
            get { return (bool)GetValue(ShowErrorMessageProperty); }
            set { SetValue(ShowErrorMessageProperty, value); }
        }
        #endregion

        #region Override Binding context change property
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            CheckValidation();
        }

        /// <summary>
        /// Method will subscibe and unsubsribe Error changed event
        /// Get bindable property path of text property
        /// </summary>
        public void CheckValidation()
        {
            // Reset variables values
            ErrorMessage = "";
            HasError = false;
            BindingPath = "";           

            if (_NotifyErrors != null)
            {
                // Unsubscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;
                _NotifyErrors = null; // Set null value on binding context change          
            }

            // Remove notify scroll to property
            if (_NotifyScroll != null)
            {
                _NotifyScroll.ScrollToProperty -= NotifyScroll_ScrollToProperty;
                _NotifyScroll = null;
            }

            // Do nothing if show error message property value is false
            if (!this.ShowErrorMessage)
                return;

            this.Placeholder = ""; // Change place holder value

            if (this.BindingContext != null && this.BindingContext is INotifyDataErrorInfo)
            {
                // Get 
                _NotifyErrors = this.BindingContext as INotifyDataErrorInfo;

                if (_NotifyErrors == null)
                    return;

                // Subscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;

                // Remove notify scroll to property
                if (this.BindingContext is INotifyScrollToProperty)
                {
                    _NotifyScroll = this.BindingContext as INotifyScrollToProperty;
                    _NotifyScroll.ScrollToProperty += NotifyScroll_ScrollToProperty;
                }

                // get property name for windows and other operating system
                // for windows 10 property name will be : properties
                // And other operation system its value : _properties
                string condition = "properties";
                
                // Get bindable properties
                var _propertiesFieldInfo = typeof(BindableObject)
                           .GetRuntimeFields()
                           .Where(x => x.IsPrivate == true && x.Name.Contains(condition))
                           .FirstOrDefault();

                // Get value
                var _properties = _propertiesFieldInfo
                                 .GetValue(this) as IList;

                if (_properties == null)
                {
                    return;
                }

                // Get first object
                var fields = _properties[0]
                    .GetType()
                    .GetRuntimeFields();

                // Get binding field info
                FieldInfo bindingFieldInfo = fields.FirstOrDefault(x => x.Name.Equals("Binding"));
                // Get property field info
                FieldInfo propertyFieldInfo = fields.FirstOrDefault(x => x.Name.Equals("Property"));

                foreach (var item in _properties)
                {
                    // Now get binding and property value
                    Binding binding = bindingFieldInfo.GetValue(item) as Binding;
                    BindableProperty property = propertyFieldInfo.GetValue(item) as BindableProperty;
                    if (binding != null && property != null && property.PropertyName.Equals("Text"))
                    {
                        // set binding path
                        BindingPath = binding.Path;
                        SetPlaceHolder();
                    }
                }
            }
        }

        // Scroll to control when request for scroll to property
        private void NotifyScroll_ScrollToProperty(string PropertyName)
        {
            // If property is requested property
            if (this.BindingPath.Equals(PropertyName))
            {
                // Get scroll 
                ScrollView _scroll = Utility.GetParentControl<ScrollView>(this);
                _scroll?.ScrollToAsync(this, ScrollToPosition.Center, true);
            }
        }

        /// <summary>
        /// Method will fire on property changed
        /// Check validation of text property
        /// Set validation if any validation message on property changed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _NotifyErrors_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            // Error changed
            if (e.PropertyName.Equals(this.BindingPath))
            {
                // Get errors
                string errors = _NotifyErrors
                            .GetErrors(e.PropertyName)
                            ?.Cast<string>()
                            .FirstOrDefault();

                // If has error
                // assign validation values
                if (!string.IsNullOrEmpty(errors))
                {
                    HasError = true; //set has error value to true
                    ErrorMessage = errors; // assign error
                }
                else
                {
                    // reset error message and flag
                    HasError = false;
                    ErrorMessage = "";
                }
            }
        }

        private void SetPlaceHolder()
        {
            if (!string.IsNullOrEmpty(BindingPath) && this.BindingContext != null)
            {
                // Get display attributes
                var _attributes = this.BindingContext.GetType()
                    .GetRuntimeProperty(BindingPath)
                    .GetCustomAttribute<DisplayAttribute>();

                // Set place holder
                if (_attributes != null)
                {
                    this.Placeholder = _attributes.Name; // assign placeholder property
                }
            }
        }
        #endregion
    }
}

