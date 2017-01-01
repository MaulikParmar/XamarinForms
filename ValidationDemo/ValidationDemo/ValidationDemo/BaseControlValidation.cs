using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ValidationDemo
{
   public class BaseControlValidation<T>
                where T : BindableObject, IControlValidation
    {
        #region Property
        private INotifyDataErrorInfo _NotifyErrors;
        private string BindingPath = "";
        private T _control;
        private string _toValidatePropertyName = "";
        private Action<bool, string> _SetPrivatePropertiesAction;
        #endregion

        #region Constructor
        /// <summary>
        /// Base class to set validation using data annotations
        /// </summary>
        /// <param name="Control">Control want to validate</param>
        /// <param name="ToValidatePropertyName">Property Name, Which you want to validate</param>
        /// <param name="SetPrivateProperties">Action to set 'HasError' and 'ErrorMessage' properties value</param>
        public BaseControlValidation(T Control, string ToValidatePropertyName, Action<bool, string> SetPrivateProperties)
        {
            this._control = Control;
            this._toValidatePropertyName = ToValidatePropertyName;
            this._SetPrivatePropertiesAction = SetPrivateProperties;
        }
        #endregion
        /// <summary>
        /// Set private property values of control
        /// 'HasError'
        /// 'ErrorMessage'
        /// </summary>
        /// <param name="HasError"></param>
        /// <param name="ErrorMessage"></param>
        private void InvokeSetPrivatePropertyAction(bool HasError, string ErrorMessage)
        {
            // Set 'HasError'
            // Set 'ErrorMessage';
            _SetPrivatePropertiesAction?.Invoke(HasError, ErrorMessage);
        }

        /// <summary>
        /// Method will subscibe and unsubsribe Error changed event
        /// Get bindable property path of text property
        /// </summary>
        public void CheckValidation()
        {
            // Reset variables values
            // Set 'HasError' = false
            // Set 'ErrorMessage' = "";
            InvokeSetPrivatePropertyAction(false, "");

            BindingPath = "";
            //this.Placeholder = "";

            if (_NotifyErrors != null)
            {
                // Unsubscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;
                _NotifyErrors = null; // Set null value on binding context change          
            }

            // Do nothing if show error message property value is false
            if (!this._control.ShowErrorMessage)
                return;

            if (this._control.BindingContext != null && this._control.BindingContext is INotifyDataErrorInfo)
            {
                // Get 
                _NotifyErrors = this._control.BindingContext as INotifyDataErrorInfo;

                // Return do nothing for your object
                if (_NotifyErrors == null)
                    return;

                // Subscribe event
                _NotifyErrors.ErrorsChanged += _NotifyErrors_ErrorsChanged;

                // get property name for windows and other operating system
                // for windows 10 property name will be : properties
                // And other operation system its value : _properties
                string condition = "properties";

                // Get bindable properties
                var _propertiesFieldInfo = typeof(BindableObject)
                           .GetRuntimeFields()
                           .Where(x => x.IsPrivate == true && x.Name.Contains(condition))
                           .FirstOrDefault();
               
                // Get Control
                var bindable = (BindableObject)_control;

                // Get value
                var _properties = _propertiesFieldInfo
                                 .GetValue(bindable) as IList;

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
                    if (binding != null && property != null && property.PropertyName.Equals(this._toValidatePropertyName))
                    {
                        // set binding path
                        BindingPath = binding.Path;
                        SetPlaceHolder();
                    }
                }
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
                    // HasError = true; //set has error value to true
                    // ErrorMessage = errors; // assign error
                    this.InvokeSetPrivatePropertyAction(true, errors);
                }
                else
                {
                    // reset error message and flag
                    // HasError = false;
                    //  ErrorMessage = "";
                    this.InvokeSetPrivatePropertyAction(false, "");
                }
            }
        }

        private void SetPlaceHolder()
        {
            if (!string.IsNullOrEmpty(BindingPath) && this._control.BindingContext != null)
            {
                // Get display attributes
                var _attributes = this._control.BindingContext.GetType()
                    .GetRuntimeProperty(BindingPath)
                    .GetCustomAttribute<DisplayAttribute>();

                // Set place holder
                if (_attributes != null)
                {
                   // this.Placeholder = _attributes.Name; // assign placeholder property
                }
            }
        }

    }
}
