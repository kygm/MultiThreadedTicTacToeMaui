using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadedTicTacToeGui.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isBusy = false;
        private string _title = string.Empty;

        public virtual bool IsBusy
        {
            get => _isBusy;
            set
            {
                if(_isBusy != value)
                {
                    SetProperty(ref _isBusy, value);
                }
            }
        }

        public virtual string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    SetProperty(ref _title, value);
                }
            }
        }

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
        {
            if(EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            ValidateField(propertyName);
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        //original return type was IEnumerable but it was causing a build error here
        public object GetErrors(string propertyName)
        {
            return Errors.ContainsKey(propertyName) ? Errors[propertyName] : null;
        }

        public bool HasErrors { get { return Errors.Any(propErrors => propErrors.Value != null && !string.IsNullOrEmpty(propErrors.Value)); } }
        public bool IsValid { get { return HasErrors; } }

        protected void addError(string propertyName, string error)
        {
            if (!Errors.ContainsKey(propertyName))
                Errors[propertyName] = "";
            if (Errors[propertyName] != error)
            {
                Errors[propertyName] = error;
                OnErrorsChanged(propertyName);
            }

            OnPropertyChanged(nameof(Errors));
        }

        protected void clearErrors(string propertyName)
        {
            if(Errors.ContainsKey(propertyName))
            {
                Errors[propertyName] = "";
                OnErrorsChanged(propertyName);
                OnPropertyChanged(nameof(Errors));
            }
        }

        protected void ValidateField([CallerMemberName] string propertyName = "")
        {
            clearErrors(propertyName);

            try
            {
                object value = GetType().GetProperty(propertyName).GetValue(this);
                ValidationContext context = new ValidationContext(this) { MemberName = propertyName };
                List<ValidationResult> validationResults = new List<ValidationResult>();

                if (!Validator.TryValidateProperty(value, context, validationResults))
                {
                    addError(propertyName, validationResults.First()?.ErrorMessage);
                }
            }
            catch (ArgumentNullException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public virtual bool Validate()
        {
            try
            {
                Errors.Keys.ToList().ForEach(key =>
                {
                    Errors[key] = string.Empty;
                });

                ValidationContext context = new ValidationContext(this);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(this, context, validationResults, true))
                {
                    foreach(ValidationResult error in validationResults.Where(v => v != ValidationResult.Success))
                    {
                        addError(error.MemberNames?.FirstOrDefault(), error.ErrorMessage);
                    }
                }

                OnPropertyChanged(nameof(Errors));
            }
            catch(ArgumentNullException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (NullReferenceException ex) 
            {
                Debug.WriteLine(ex);
            }

            return IsValid;
        }

    }
}
