using System.ComponentModel.DataAnnotations;

namespace ValidationDemo
{
    public class Customer : ModelBase
    {
        private string _firstName;
        [Display(Name = "First Name")]
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
    }
}
