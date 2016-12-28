namespace ValidationDemo
{
    public interface IControlValidation
    {
        bool HasError { get;  }
        string ErrorMessage { get; }
        bool ShowErrorMessage { get; set; }
    }
}
