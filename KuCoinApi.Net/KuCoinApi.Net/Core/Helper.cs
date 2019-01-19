using KuCoinApi.Net.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KuCoinApi.Net.Core
{
    public class Helper
    {
        /// <summary>
        /// Round a number down N decimal places
        /// </summary>
        /// <param name="number">Number to round down</param>
        /// <param name="decimalPlaces">N decimal places</param>
        /// <returns>Rounded down number</returns>
        public decimal RoundDown(decimal number, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));

            return Math.Floor(number * power) / power;
        }

        /// <summary>
        /// Calculate buy percent difference
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <param name="lastSell">Last sell price</param>
        /// <returns>Double of percent difference</returns>
        public double GetBuyPercent(decimal currentPrice, decimal lastSell)
        {
            return GetPercent(lastSell, currentPrice);
        }

        /// <summary>
        /// Calculate sell percent difference
        /// </summary>
        /// <param name="currentPrice">Current price</param>
        /// <param name="lastBuy">Last buy price</param>
        /// <returns>Double of percent difference</returns>
        public double GetSellPercent(decimal currentPrice, decimal lastBuy)
        {
            return GetPercent(currentPrice, lastBuy);
        }

        /// <summary>
        /// Calculate percent difference from current price and last buy
        /// </summary>
        /// <param name="priceA">Price A</param>
        /// <param name="priceB">Price B</param>
        /// <returns>double of percent difference</returns>
        public double GetPercent(decimal priceA, decimal priceB)
        {
            var A = priceA;
            var B = priceB;
            var C = B == 0 ? 0 : (double)(A / B) - 1;

            return C;
        }

        /// <summary>
        /// Get zero'd satoshi count
        /// </summary>
        /// <returns>decimal of zero sats</returns>
        public decimal ZeroSats()
        {
            return 0.00000000M;
        }

        /// <summary>
        /// Get a collection from an enum
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <returns>Collection of enum values</returns>
        public IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Creates dashed pair (ie BTC-ETH)
        /// </summary>
        /// <param name="pair">String of pair</param>
        /// <param name="markets">Array of available markets</param>
        /// <returns>String of pair</returns>
        public string CreateDashedPair(string pair, string[] markets)
        {
            if (pair.IndexOf("-") < 0)
            {
                for(var i = 0;i<markets.Length;i++)
                {
                    var marketLen = markets[i].Length;
                    if(pair.Substring(pair.Length - marketLen).Equals(markets[i]))
                    {
                        pair = pair.Replace(markets[i], $"-{markets[i]}");
                    }
                }
            }

            return pair;
        }

        /// <summary>
        /// Set first character uppercase for a string
        /// </summary>
        /// <param name="toUpperCase">String to uppercase</param>
        /// <returns>String with first character uppercased</returns>
        public string UpperCaseFirst(string toUpperCase)
        {
            if (string.IsNullOrEmpty(toUpperCase))
            {
                return string.Empty;
            }
            char[] a = toUpperCase.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        /// <summary>
        /// Convert an array of strings to a string
        /// </summary>
        /// <param name="myArray">Array of strings</param>
        /// <returns>String of array values</returns>
        public string ArrayToString(string[] myArray)
        {
            var qsValues = string.Empty;

            for (int i = 0; i < myArray.Length; i++)
            {
                qsValues += qsValues != string.Empty ? "&" : "";
                qsValues += myArray[i];
            }

            return qsValues;
        }

        /// <summary>
        /// Convert a list of strings to a string
        /// </summary>
        /// <param name="myList">List of strings</param>
        /// <returns>String of list values</returns>
        public string ListToString(List<string> myList)
        {
            var qsValues = string.Empty;

            for (int i = 0; i < myList.Count; i++)
            {
                qsValues += qsValues != string.Empty ? "&" : "";
                qsValues += myList[i];
            }

            return qsValues;
        }

        /// <summary>
        /// Convert an object to a string of property names and values
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="myObject">Object to convert</param>
        /// <returns>String of properties and values</returns>
        public string ObjectToString<T>(T myObject)
        {
            var qsValues = string.Empty;

            foreach (PropertyInfo p in myObject.GetType().GetProperties())
            {
                qsValues += qsValues != string.Empty ? "&" : "";
                qsValues += $"{p.Name}={p.GetValue(myObject, null)}";
            }

            return qsValues;
        }

        /// <summary>
        /// Create new decmial to the Nth power
        /// </summary>
        /// <param name="precision">precision of decimal</param>
        /// <param name="value">Value to set, default = 1</param>
        /// <returns>New decimal</returns>
        public decimal DecimalValueAtPrecision(int precision, int value = 1)
        {
            var pow = Math.Pow(10, precision);
            decimal newValue = value / (decimal)pow;

            return newValue;
        }
        
        /// <summary>
        /// Convert interval to kucoin interval
        /// </summary>
        /// <param name="interval">Interval value</param>
        /// <returns>Int of kucoin interval</returns>
        public int IntervalToKuCoinInterval(Interval interval)
        {
            switch (interval)
            {
                case Interval.OneM:
                    return 0;
                case Interval.FiveM:
                    return 1;
                case Interval.FifteenM:
                    return 2;
                case Interval.ThirtyM:
                    return 3;
                case Interval.OneH:
                    return 4;
                case Interval.FourH:
                    return 5;
                case Interval.OneD:
                    return 6;
                case Interval.OneW:
                    return 7;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Convert interval to kucoin string interval
        /// </summary>
        /// <param name="interval">Interval value</param>
        /// <returns>String of kucoin interval</returns>
        public string IntervalToKuCoinStringInterval(Interval interval)
        {
            switch (interval)
            {
                case Interval.OneM:
                    return "1";
                case Interval.FiveM:
                    return "5";
                case Interval.FifteenM:
                    return "15";
                case Interval.ThirtyM:
                    return "30";
                case Interval.OneH:
                    return "60";
                case Interval.FourH:
                    return "480";
                case Interval.OneD:
                    return "D";
                case Interval.OneW:
                    return "W";
                default:
                    return "1";
            }
        }

        /// <summary>
        /// Convert interval to total seconds
        /// </summary>
        /// <param name="interval">Interval value</param>
        /// <returns>Long of seconds</returns>
        public long IntervalToSeconds(Interval interval)
        {
            switch (interval)
            {
                case Interval.OneM:
                    return (60 * 1);
                case Interval.FiveM:
                    return (60 * 5);
                case Interval.FifteenM:
                    return (60 * 15);
                case Interval.ThirtyM:
                    return (60 * 30);
                case Interval.OneH:
                    return (60 * 60);
                case Interval.FourH:
                    return (60 * 60 * 5);
                case Interval.OneD:
                    return (60 * 60 * 24);
                case Interval.OneW:
                    return (60 * 60 * 24 * 7);
                default:
                    return (60 * 1);
            }
        }

        /// <summary>
        /// Get unix time off-set from current unix time
        /// </summary>
        /// <param name="ending">Ending time</param>
        /// <param name="interval">Stick interval</param>
        /// <param name="stickNumber">Number of sticks</param>
        /// <returns>Long of off-set time</returns>
        public long GetFromUnixTime(long ending, Interval interval, int stickNumber)
        {
            var seconds = IntervalToSeconds(interval);
            var totalSeconds = seconds * stickNumber;

            return ending - totalSeconds;
        }

        /// <summary>
        /// Cultural nutral decimal to string
        /// </summary>
        /// <param name="value">Decimal value</param>
        /// <returns>String of decimal value</returns>
        public string DecimalToString(decimal value)
        {
            var decimalString = value.ToString();

            return decimalString.Replace(",", ".");
        }
    }
}
