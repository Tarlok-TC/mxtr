using System;

namespace mxtrAutomation.Common.Utils
{
    public class Trend
    {
        public double Value { get; set; }
        public double PreviousValue { get; set; }
        public double Change { get; set; }

        public Polarity Polarity { get; set; }

        public Trend(double value, double previousValue)
            : this(value, previousValue, Polarity.Positive) { }

        public Trend(double value, double previousValue, Polarity polarity)
        {
            Value = value;
            PreviousValue = previousValue;
            Polarity = polarity;

            Change = (Value / PreviousValue) - 1;
        }

        /// <summary>
        /// Determines if the change value is not infinity/nan
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !Double.IsInfinity(Change) && !Double.IsNaN(Change);
        }

        /// <summary>
        /// Gets a value indicating if the change was positive based on the polarity.
        /// </summary>
        /// <returns></returns>
        public bool IsPositive()
        {
            if (Polarity == Polarity.Positive)
            {
                if (Value > PreviousValue)
                {
                    return true;
                }
            }
            else
            {
                if (Value < PreviousValue)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating if the value increased over the previous value.
        /// </summary>
        /// <returns></returns>
        public bool IsIncreasing()
        {
            return Value >= PreviousValue;
        }

        public Trend Join(Trend trend)
        {
            return Join(trend, Polarity);
        }
        public Trend Join(Trend trend, Polarity polarity)
        {
            return new Trend(Value + trend.Value, PreviousValue + trend.PreviousValue, polarity);
        }

        public override string ToString()
        {
            return Change.ToString("#,0%");
        }
        public string ToString(int decimalPlaces)
        {
            if (Double.IsInfinity(Change))
            {
                return "Infinite";
            }
            else if (Double.IsNaN(Change))
            {
                return "NaN";
            }

            return (Change * 100).ToString("N" + decimalPlaces) + "%";
        }
        public string ToString(string format)
        {
            return Change.ToString(format);
        }
    }
}
