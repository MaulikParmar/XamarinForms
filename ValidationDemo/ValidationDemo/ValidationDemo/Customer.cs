using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValidationDemo
{
    public class Customer : ModelBase
    {
        public Customer()
        {
            Locations = new List<Location>();
            Locations.Add(new Location() { Id = 1, Name = "Delhi" });
            Locations.Add(new Location() { Id = 1, Name = "Mumbai" });
            Locations.Add(new Location() { Id = 1, Name = "Ahmedabad" });
        }

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


        private List<Location> _locations;
        public List<Location> Locations
        {
            get { return _locations; }
            set
            {
                if (_locations != value)
                {
                    _locations = value;
                    base.NotifyPropertyChanged("Locations");
                }
            }
        }


        private Location _selectedLocation;
        [Required]
        public Location SelectedLocation
        {
            get { return _selectedLocation; }
            set
            {
                if (_selectedLocation != value)
                {
                    _selectedLocation = value;
                    ValidateProperty(value);                    
                    base.NotifyPropertyChanged("SelectedLocation");
                }
            }
        }


        private DateTime _JoinDate;
        [Display(Name ="Join Date")]
        [Required]
        [Range(typeof(DateTime), "1/1/2017", "1/2/2017")]
        public DateTime JoinDate
        {
            get { return _JoinDate; }
            set
            {
                if (_JoinDate != value)
                {
                    _JoinDate = value;
                    ValidateProperty(value);
                    base.NotifyPropertyChanged("JoinDate");
                }
            }
        }

    }


    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
