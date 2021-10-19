using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace It_Doesnt_Matter.Models
{
    public class MeasurementUnit : INotifyPropertyChanged
    {
        private List<MetricUnits> metricUnits;
        private string measure;
        public MeasurementProperty Property { get; set; }
        public List<MetricUnits> MetricUnits
        {
            get => metricUnits;
            set
            {
                metricUnits = value;
                OnPropertyChanged("MetricUnits");
            }
        }
        // Validating textbox for mass value - it should be present and it should a valid double
        [Required(ErrorMessage = "Input cannot be empty!")]
        [RegularExpression(@"^(?!0+$)[0-9]*?(\.[0-9][0-9]*)?$", ErrorMessage = "Please provide a valid input!")]
        public string Measure
        {
            get => measure;
            set
            {
                measure = value;
                ValidateProperty(value, "Measure");
                OnPropertyChanged("Measure");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private void ValidateProperty<T>(T value, string property)
        {
            Validator.ValidateProperty(value, new ValidationContext(this, null, null)
            {
                MemberName = property
            });
        }
    }
    public class MetricUnits
    {
        public string Unit { get; set; }
        public double ConversionFactor { get; set; }
        public string Symbol { get; set; }
    }

    public enum MeasurementProperty
    {
        Mass,
        GravitationalAcceleration,
        Weight
    }

}
