using System.ComponentModel.DataAnnotations;

namespace ValidationDemo
{
    public class Customer : ModelBase
    {
        private string _firstName;
        [Display(Name = "First name")]
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

        private string _lastName;
        [Display(Name = "Last Name")]
        [Required]
        [StringLength(20)]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                ValidateProperty(value);
                base.NotifyPropertyChanged("LastName");
            }
        }


        private string _email;
        [Display(Name = "Email")]
        [Required]
        [Email]
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                ValidateProperty(value);
                base.NotifyPropertyChanged("Email");
            }
        }

    }
}
