using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Terminal.Gui.Reflect.Attributes;

namespace Terminal.Gui.Reflect.TestApp
{
    [DisplayName("Some View")]
    [CategoryLayout("Bools", 1, 3, -1)]
    [CategoryLayout("Text", 1, 1, -1)]
    [Layout(2, 3)]
    public class BasicViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        [Category("Bools")]
        public bool SomeBool
        {
            get;
            set => SetField(ref field, value);
        }

        [Category("Text")]
        [DisplayName("Some Text")]
        [Display(Prompt = "This is some text")]
        [MinLength(5)]
        [MaxLength(50)]
        [Description("This is some text")]
        public string? SomeText
        {
            get;
            set => SetField(ref field, value);
        }

        [Category("Numbers")]
        public int SomeInteger
        {
            get;
            set => SetField(ref field, value);
        }

        [Category("Numbers")]
        public double SomeDouble
        {
            get;
            set => SetField(ref field, value);
        }

        [Browsable(false)]
        [Category("Bools")]
        public bool HiddenMember
        {
            get;
            set => SetField(ref field, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        
        private readonly Dictionary<string, List<string>> _errors = new();

        [Browsable(false)]
        public bool HasErrors => _errors.Count > 0;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
 
        public IEnumerable GetErrors(string? propertyName) =>
            string.IsNullOrEmpty(propertyName)
                ? _errors.Values.SelectMany(err => err)
                : _errors.TryGetValue(propertyName, out var e) ? e : [];

        // Called by ValidationService via reflection
        [UsedImplicitly]
        protected void SetErrors(string propertyName, IEnumerable<string> errors)
        {
            var list = errors.ToList();
            if (list.Count == 0) _errors.Remove(propertyName);
            else                 _errors[propertyName] = list;

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors));
        }

    }
}
