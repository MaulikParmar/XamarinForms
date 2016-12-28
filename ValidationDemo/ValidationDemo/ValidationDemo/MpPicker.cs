using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ValidationDemo
{
    public class MpPicker : Picker
    {
        #region Properties
        Boolean _disableNestedCalls;

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(MpPicker),
                null, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create("SelectedItem", typeof(Object), typeof(MpPicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty SelectedValueProperty =
            BindableProperty.Create("SelectedValue", typeof(Object), typeof(MpPicker),
                null, BindingMode.TwoWay, propertyChanged: OnSelectedValueChanged);

        public String DisplayMemberPath { get; set; }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                if (this.SelectedItem != value)
                {
                    SetValue(SelectedItemProperty, value);
                    InternalSelectedItemChanged();
                }
            }
        }

        public Object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
                InternalSelectedValueChanged();
            }
        }

        public String SelectedValuePath { get; set; }
        #endregion

        #region Constructor
        public MpPicker()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public event EventHandler<SelectedItemChangedEventArgs> ItemSelected;
        #endregion

        #region Methods / Actions
        private void InstanceOnItemsSourceChanged(Object oldValue, Object newValue)
        {
            _disableNestedCalls = true;
            this.Items.Clear();

            var oldCollectionINotifyCollectionChanged = oldValue as INotifyCollectionChanged;
            if (oldCollectionINotifyCollectionChanged != null)
            {
                oldCollectionINotifyCollectionChanged.CollectionChanged -= ItemsSource_CollectionChanged;
            }

            var newCollectionINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            if (newCollectionINotifyCollectionChanged != null)
            {
                newCollectionINotifyCollectionChanged.CollectionChanged += ItemsSource_CollectionChanged;
            }

            if (!Equals(newValue, null))
            {
                var hasDisplayMemberPath = !String.IsNullOrWhiteSpace(this.DisplayMemberPath);

                foreach (var item in (IEnumerable)newValue)
                {
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.DisplayMemberPath);
                        this.Items.Add(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        this.Items.Add(item.ToString());
                    }
                }

                this.SelectedIndex = -1;
                this._disableNestedCalls = false;

                if (this.SelectedItem != null)
                {
                    this.InternalSelectedItemChanged();
                }
                else if (hasDisplayMemberPath && this.SelectedValue != null)
                {
                    this.InternalSelectedValueChanged();
                }
            }
            else
            {
                _disableNestedCalls = true;
                this.SelectedIndex = -1;
                this.SelectedItem = null;
                this.SelectedValue = null;
                _disableNestedCalls = false;
            }
        }

        private void InternalSelectedItemChanged()
        {
            if (_disableNestedCalls)
            {
                return;
            }

            var selectedIndex = -1;
            Object selectedValue = null;
            if (this.ItemsSource != null)
            {
                var index = 0;
                var hasSelectedValuePath = !String.IsNullOrWhiteSpace(this.SelectedValuePath);
                foreach (var item in this.ItemsSource)
                {
                    if (item != null && item.Equals(this.SelectedItem))
                    {
                        selectedIndex = index;
                        if (hasSelectedValuePath)
                        {
                            var type = item.GetType();
                            var prop = type.GetRuntimeProperty(this.SelectedValuePath);
                            selectedValue = prop.GetValue(item);
                        }
                        break;
                    }
                    index++;
                }
            }
            _disableNestedCalls = true;
            this.SelectedValue = selectedValue;
            this.SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        private void InternalSelectedValueChanged()
        {
            if (_disableNestedCalls)
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(this.SelectedValuePath))
            {
                return;
            }
            var selectedIndex = -1;
            Object selectedItem = null;
            var hasSelectedValuePath = !String.IsNullOrWhiteSpace(this.SelectedValuePath);
            if (this.ItemsSource != null && hasSelectedValuePath)
            {
                var index = 0;
                foreach (var item in this.ItemsSource)
                {
                    if (item != null)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.SelectedValuePath);
                        if (Object.Equals(prop.GetValue(item), this.SelectedValue))
                        {
                            selectedIndex = index;
                            selectedItem = item;
                            break;
                        }
                    }

                    index++;
                }
            }
            _disableNestedCalls = true;
            this.SelectedItem = selectedItem;
            this.SelectedIndex = selectedIndex;
            _disableNestedCalls = false;
        }

        private void ItemsSource_CollectionChanged(Object sender, NotifyCollectionChangedEventArgs e)
        {
            var hasDisplayMemberPath = !String.IsNullOrWhiteSpace(this.DisplayMemberPath);
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.DisplayMemberPath);
                        this.Items.Add(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        this.Items.Add(item.ToString());
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.NewItems)
                {
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.DisplayMemberPath);
                        this.Items.Remove(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        this.Items.Remove(item.ToString());
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (var item in e.NewItems)
                {
                    if (hasDisplayMemberPath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.DisplayMemberPath);
                        this.Items.Remove(prop.GetValue(item).ToString());
                    }
                    else
                    {
                        var index = this.Items.IndexOf(item.ToString());
                        if (index > -1)
                        {
                            this.Items[index] = item.ToString();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Items.Clear();
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (hasDisplayMemberPath)
                        {
                            var type = item.GetType();
                            var prop = type.GetRuntimeProperty(this.DisplayMemberPath);
                            this.Items.Remove(prop.GetValue(item).ToString());
                        }
                        else
                        {
                            var index = this.Items.IndexOf(item.ToString());
                            if (index > -1)
                            {
                                this.Items[index] = item.ToString();
                            }
                        }
                    }
                }
                else
                {
                    _disableNestedCalls = true;
                    this.SelectedItem = null;
                    this.SelectedIndex = -1;
                    this.SelectedValue = null;
                    _disableNestedCalls = false;
                }
            }
        }
        #endregion

        #region Static Method
        static void OnItemsSourceChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            if (Equals(newValue, null) && Equals(oldValue, null))
            {
                return;
            }

            var picker = (MpPicker)bindable;
            picker.InstanceOnItemsSourceChanged(oldValue, newValue);
        }

        void OnSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (_disableNestedCalls)
            {
                return;
            }

            if (this.SelectedIndex < 0 || this.ItemsSource == null || !this.ItemsSource.GetEnumerator().MoveNext())
            {
                _disableNestedCalls = true;
                if (this.SelectedIndex != -1)
                {
                    this.SelectedIndex = -1;
                }
                this.SelectedItem = null;
                this.SelectedValue = null;
                _disableNestedCalls = false;
                return;
            }

            _disableNestedCalls = true;

            var index = 0;
            var hasSelectedValuePath = !String.IsNullOrWhiteSpace(this.SelectedValuePath);
            foreach (var item in this.ItemsSource)
            {
                if (index == this.SelectedIndex)
                {
                    this.SelectedItem = item;
                    if (hasSelectedValuePath)
                    {
                        var type = item.GetType();
                        var prop = type.GetRuntimeProperty(this.SelectedValuePath);
                        this.SelectedValue = prop.GetValue(item);
                    }

                    break;
                }
                index++;
            }

            _disableNestedCalls = false;
        }

        static void OnSelectedItemChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var boundPicker = (MpPicker)bindable;
            boundPicker.ItemSelected?.Invoke(boundPicker, new SelectedItemChangedEventArgs(newValue));
            boundPicker.InternalSelectedItemChanged();
        }

        static void OnSelectedValueChanged(BindableObject bindable, Object oldValue, Object newValue)
        {
            var boundPicker = (MpPicker)bindable;
            boundPicker.InternalSelectedValueChanged();
        }
        #endregion

        #region Validations Properties

        private INotifyDataErrorInfo _NotifyErrors;
        private string BindingPath = "";

        #region Has Error
        public static readonly BindableProperty HasErrorProperty =
            BindableProperty.Create("HasError", typeof(bool), typeof(MpPicker), false, defaultBindingMode: BindingMode.TwoWay);

        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }
            private set { SetValue(HasErrorProperty, value); }
        }
        #endregion

        #region ErrorMessage

        public static readonly BindableProperty ErrorMessageProperty =
           BindableProperty.Create("ErrorMessage", typeof(string), typeof(MpPicker), string.Empty);

        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }
        #endregion

        #region ShowErrorMessage

        public static readonly BindableProperty ShowErrorMessageProperty =
           BindableProperty.Create("ShowErrorMessage", typeof(bool), typeof(MpPicker), false, propertyChanged: OnShowErrorMessageChanged, defaultBindingMode: BindingMode.TwoWay);

        private static void OnShowErrorMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // execute on bindable context changed method
            (bindable as MpPicker)?.CheckValidation();
        }

        public bool ShowErrorMessage
        {
            get { return (bool)GetValue(ShowErrorMessageProperty); }
            set { SetValue(ShowErrorMessageProperty, value); }
        }
        #endregion

        #endregion

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

            // Do nothing if show error message property value is false
            if (!this.ShowErrorMessage)
                return;

            if (this.BindingContext != null && this.BindingContext is INotifyDataErrorInfo)
            {
                // Get 
                _NotifyErrors = this.BindingContext as INotifyDataErrorInfo;
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

    }
}
