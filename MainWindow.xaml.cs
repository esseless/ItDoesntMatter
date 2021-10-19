//Author:           Sharanya Sargur, Bhupinder Singh, Keval Patel
//Created On:       07/06/2021
//Last Modified On: 11/06/2021
//Copy Rights:      Conestoga College
//Description:      Includes methods that handle the manipulation of UI data and other computations 
using It_Doesnt_Matter.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace It_Doesnt_Matter
{
    public partial class MainWindow : Window
    {
        private static readonly string unitsConfigPath = @".\Configs\MeasurementUnitsConfig.json";
        private readonly string measurementConfig;
        private readonly List<MeasurementUnit> measurementUnits = new();
        private static readonly string labelsConfigPath = @".\Configs\AppLabelsConfig.json";
        private readonly string labelsConfig;
        private readonly List<AppLabels> appLabels = new();
        private double sourceConversionFactor = 1;
        private double targetConversionFactor = 1;
        private readonly double gravitationalAcceleration = 9.8;
        private double targetMeasure = 0;
        private static readonly string tooLightNotificationText = "This doesn't matter, it is too light!!";
        private static readonly string tooHeavyNotificationText = "This must matter a lot, sooo heavy!!";

        public MainWindow()
        {
            InitializeComponent();
            // Read the config
            measurementConfig = File.ReadAllText(unitsConfigPath);
            labelsConfig = File.ReadAllText(labelsConfigPath);
            // Deserializing
            measurementUnits = JsonConvert.DeserializeObject<List<MeasurementUnit>>(measurementConfig);
            appLabels = JsonConvert.DeserializeObject<List<AppLabels>>(labelsConfig);
            // Binding the sources and context
            SourceMetrics.ItemsSource = measurementUnits[0].MetricUnits;
            TargetMetrics.ItemsSource = measurementUnits[1].MetricUnits;
            this.DataContext = measurementUnits;
        }

        private void SourceMetrics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourceMetrics.SelectedIndex != -1)
            {
                var selectedUnit = e.AddedItems[0] as MetricUnits;
                sourceConversionFactor = selectedUnit.ConversionFactor;
                if (!SourceMeasure.IsEnabled)
                {
                    SourceMeasure.IsEnabled = true;
                }
                SourceMeasure.DataContext = measurementUnits[0];
                // Clear previously calculated target value
                if (TargetMeasure.Text != null)
                {
                    TargetMeasure.Clear();
                    AbnormalityIndicator.Text = null;
                }
            }
        }

        private void TargetMetrics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TargetMetrics.SelectedIndex != -1)
            {
                var selectedUnit = e.AddedItems[0] as MetricUnits;
                targetConversionFactor = selectedUnit.ConversionFactor;
                TargetMeasure.DataContext = measurementUnits[1];
                // Format the current weight to match the chosen metric if the value exists
                if (!string.IsNullOrEmpty(TargetMeasure.Text))
                {
                    Set_TargetMeasure();
                }
            }
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            Set_TargetMeasure();
        }

        private void Set_TargetMeasure()
        {
            try
            {
                double source = double.Parse(SourceMeasure.Text);
                double sourceInStdMetric = source * sourceConversionFactor;
                if ((bool)SwitchUnits.IsChecked)
                {
                    targetMeasure = sourceInStdMetric * gravitationalAcceleration;
                    // If weight is not between 10N and 1000N
                    AbnormalityIndicator.Text = targetMeasure < 10 ? tooLightNotificationText
                                                                   : targetMeasure > 1000 ? tooHeavyNotificationText : null;
                }
                else
                {
                    targetMeasure = sourceInStdMetric / gravitationalAcceleration;
                    // If weight is not between 10N and 1000N
                    AbnormalityIndicator.Text = sourceInStdMetric < 10 ? tooLightNotificationText
                                                                       : sourceInStdMetric > 1000 ? tooHeavyNotificationText : null;
                }
                // Format to show in the chosen metric 
                double targetInSelectedMetric = targetMeasure / targetConversionFactor;
                TargetMeasure.Text = $"{targetInSelectedMetric:N4}";
            }
            catch (Exception ex)
            {
                AbnormalityIndicator.Text = "Something is broken!!!!";
            }
        }

        private void SourceMeasure_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Clear previous calculations/indications
            TargetMeasure.Clear();
            AbnormalityIndicator.Text = null;
        }

        private void SwitchUnits_Click(object sender, RoutedEventArgs e)
        {
            AppLabels labels;
            if ((bool)SwitchUnits.IsChecked)
            {
                labels = appLabels[0];
                SourceMass.FontWeight = FontWeights.Normal;
                SourceWeight.FontWeight = FontWeights.Bold;
            }
            else
            {
                labels = appLabels[1];
                SourceWeight.FontWeight = FontWeights.Normal;
                SourceMass.FontWeight = FontWeights.Bold;
            }
            // Swapping the sources
            (SourceMetrics.ItemsSource, TargetMetrics.ItemsSource) = (TargetMetrics.ItemsSource, SourceMetrics.ItemsSource);
            // Updating styles for the selected source
            TargetMeasure.Text = null;
            SourceMeasure.Text = null;
            MaterialDesignThemes.Wpf.HintAssist.SetHint(SourceMetrics, labels.SourceMetricHint);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TargetMetrics, labels.TargetMetricHint);
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(SourceMeasure, labels.SourceHelperText);
            MaterialDesignThemes.Wpf.HintAssist.SetHelperText(TargetMeasure, labels.TargetHelperText);
        }
    }

}
