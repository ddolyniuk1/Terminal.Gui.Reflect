using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Terminal.Gui.Reflect
{
    public record ValidationResult(bool IsValid, string? ErrorMessage);

    public static class ValidationMapper
    {
        /// <summary>
        /// Validates a property value, updates the model's <see cref="INotifyDataErrorInfo"/>
        /// error state if supported, and returns the result for the UI to display.
        /// 
        /// This is the main entry point editors should call — it handles both UI feedback
        /// and model notification in one shot.
        /// </summary>
        public static ValidationResult ValidateAndNotify(object model, PropertyInfo property, object? value)
        {
            var result = Validate(property, value);

            if (model is INotifyDataErrorInfo errorInfo)
                SetModelErrors(errorInfo, property.Name, result);

            return result;
        }

        /// <summary>
        /// Runs all DataAnnotations validators on a property value and returns the first failure, or success.
        /// </summary>
        public static ValidationResult Validate(PropertyInfo property, object? value)
        {
            var attributes = property.GetCustomAttributes<ValidationAttribute>();
            var context    = new ValidationContext(value ?? new object()) { MemberName = property.Name };

            foreach (var attr in attributes)
            {
                var result = attr.GetValidationResult(value, context);
                if (result == System.ComponentModel.DataAnnotations.ValidationResult.Success) continue;

                // Prefer a custom ErrorMessage set by the developer; otherwise generate a
                // compact symbol-based message so the validation label stays narrow.
                var message = HasCustomMessage(attr)
                    ? result!.ErrorMessage
                    : GetCompactMessage(attr);

                return new ValidationResult(false, message ?? "Invalid");
            }

            return new ValidationResult(true, null);
        }

        /// <summary>
        /// Returns true if the developer explicitly set ErrorMessage / ErrorMessageResourceName
        /// on the attribute rather than relying on the default localised template.
        /// </summary>
        private static bool HasCustomMessage(ValidationAttribute attr) =>
            !string.IsNullOrEmpty(attr.ErrorMessage)
            || attr.ErrorMessageResourceName != null;

        /// <summary>
        /// Produces a short, symbol-oriented message for built-in attributes.
        /// Examples:  "Required"  "≥ 5 chars"  "≤ 100"  "5–100 chars"  "Invalid email"
        /// </summary>
        private static string GetCompactMessage(ValidationAttribute attr) =>
            attr switch
            {
                RequiredAttribute                              => "This field is required",
                MinLengthAttribute min                         => $"Must be at least {min.Length} characters in length",
                MaxLengthAttribute max                         => $"Must be less than {max.Length} characters in length",
                StringLengthAttribute { MinimumLength: > 0 } sl
                                                               => $"{sl.MinimumLength}–{sl.MaximumLength} characters",
                StringLengthAttribute sl                       => $"Must be less than {sl.MaximumLength} characters",
                RangeAttribute r                               => $"{r.Minimum}–{r.Maximum}",
                EmailAddressAttribute                          => "Invalid email",
                PhoneAttribute                                 => "Invalid phone",
                UrlAttribute                                   => "Invalid URL",
                CreditCardAttribute                            => "Invalid card",
                RegularExpressionAttribute                     => "Invalid format",
                CompareAttribute c                             => $"Must match {c.OtherProperty}",
                _                                              => "Invalid",
            };

        /// <summary>
        /// Returns true if the property has [Required].
        /// </summary>
        public static bool IsRequired(PropertyInfo property) =>
            property.GetCustomAttribute<RequiredAttribute>() != null;

        /// <summary>
        /// Returns the [Display] name, then [DisplayName], then the property name.
        /// </summary>
        public static string GetLabel(PropertyInfo property) =>
            property.GetCustomAttribute<DisplayAttribute>()?.GetName()
            ?? property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
            ?? property.Name;

        /// <summary>
        /// Returns the [Display] description / tooltip.
        /// </summary>
        public static string? GetDescription(PropertyInfo property) =>
            property.GetCustomAttribute<DisplayAttribute>()?.GetDescription();

        /// <summary>
        /// Returns the [Display] placeholder prompt.
        /// </summary>
        public static string? GetPrompt(PropertyInfo property) =>
            property.GetCustomAttribute<DisplayAttribute>()?.GetPrompt();

        /// <summary>
        /// Returns the [Display] order, defaulting to int.MaxValue so un-ordered properties sort last.
        /// </summary>
        public static int GetOrder(PropertyInfo property) =>
            property.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? int.MaxValue;

        /// <summary>
        /// Returns the explicit [DataType], or infers one from the CLR type.
        /// </summary>
        public static DataType GetDataType(PropertyInfo property)
        {
            var dataType = property.GetCustomAttribute<DataTypeAttribute>()?.DataType;
            if (dataType.HasValue) return dataType.Value;

            var t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (t == typeof(DateTime) || t == typeof(DateTimeOffset)) return DataType.DateTime;
            if (t == typeof(DateOnly))                                  return DataType.Date;
            if (t == typeof(TimeOnly) || t == typeof(TimeSpan))        return DataType.Time;

            return DataType.Text;
        }

        /// <summary>
        /// Pushes or clears errors on a model that implements <see cref="INotifyDataErrorInfo"/>.
        /// Uses reflection to call SetErrors/ClearErrors if the model exposes them,
        /// otherwise falls back to a convention-based internal errors dictionary.
        /// </summary>
        private static void SetModelErrors(INotifyDataErrorInfo model, string propertyName, ValidationResult result)
        {
            var setErrors = model.GetType().GetMethod(
                "SetErrors",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                [typeof(string), typeof(IEnumerable<string>)]);

            if (setErrors != null)
            {
                var messages = result.IsValid
                    ? (IEnumerable<string>)[]
                    : [result.ErrorMessage ?? "Invalid"];

                setErrors.Invoke(model, [propertyName, messages]);
                return;
            }

            // Fallback: try to write directly into a backing dictionary named _errors.
            var errorsField = model.GetType().GetField(
                "_errors",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (errorsField?.GetValue(model) is IDictionary<string, List<string>> dict)
            {
                if (result.IsValid)
                    dict.Remove(propertyName);
                else
                    dict[propertyName] = [result.ErrorMessage ?? "Invalid"];
            }
        }
    }
}