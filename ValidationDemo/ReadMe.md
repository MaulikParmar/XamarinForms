Custom Entry and Editor(MpEntry and MpEdior) that allow developer to validate control using data annotation method like in wpf.

Support
  It is not worked for windows 8.1 phone because windows 8.1 phone does not support System.ComponentModel.DataAnnotations and for IOS version must be greater than 7.2
  
Video URL

Part 1 : https://www.youtube.com/watch?v=eEi-Oky4U08

Part 2 : https://www.youtube.com/watch?v=c7AQh1RBH1o



If you are wpf developer and you have worked on validation using 'INotifyDataErrorInfo', easy stuff for you.

Step 1 : Implement 'INotifyDataErrorInfo' interface for your model.(Check "ValidationBase.cs" file)
Step 2 : Add DataAnnotations validation for our model.

I.E. 
   
    [Required]
    [StringLength(20)]
    public string FirstName
    {
        get { return _firstName; }
        set
        {
            _firstName = value;
            ValidateProperty(value);
            base.NotifyPropertyChanged("FirstName");
        }
    }


Step 4 : Set "ShowErrorMessage" property true for your MpEntry or MpEditor Control
Step 6 : Bind "ErrorMessage" property to label to show error message.

I.E.

 <local:MpEntry
      x:Name="firstName"
      HorizontalOptions="Fill"
      ShowErrorMessage="True"
      Text="{Binding FirstName}"
      VerticalOptions="Center" />

  <Label
      BindingContext="{x:Reference firstName}"
      IsVisible="{Binding HasError}"
      Text="{Binding ErrorMessage}"
      TextColor="Red" />

 Now thats it done!

Stpe 7(Optional) :  You can change background color of control when HasError property vaue true for control.

Common style for MpEntry control

  <Application.Resources>

        <!--  Application resource dictionary  -->
        <ResourceDictionary>
            <Style TargetType="local:MpEntry">
                <Style.Triggers>
                    <Trigger TargetType="local:MpEntry" Property="HasError" Value="True">
                        <Setter Property="BackgroundColor" Value="Red" />
                    </Trigger>
                </Style.Triggers>
            </Style>
        </ResourceDictionary>

    </Application.Resources>

