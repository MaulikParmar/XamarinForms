using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ValidationDemo
{
    public static class Utility
    {
        public static T GetParentControl<T>(this Element control) where T : class
        {
            // Parent is null return null
            if (control.ParentView == null)
                return null;

            // Parent is desired control
            // Than return parent
            if (control.ParentView is T)
                return control.ParentView as T;

            // search for control
            return GetParentControl<T>(control.ParentView); 

        }
    }
}
