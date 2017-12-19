using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Icu.Util;
using Java.Util;

namespace StockApp.BarcodeReader
{
    public class DatePickerDialogFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public IOnDatePickerResponse onDatePickerResponse = null;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Java.Util.Calendar c = Java.Util.Calendar.Instance;
            int year = c.Get(Java.Util.CalendarField.Year);
            int month = c.Get(Java.Util.CalendarField.Month);
            int day = c.Get(Java.Util.CalendarField.DayOfMonth);

            return new DatePickerDialog(Activity, this, year, month, day);
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            Console.WriteLine("This is the date picked {0},{1},{2}", year, month, dayOfMonth);
            onDatePickerResponse.update(year, month, dayOfMonth);
        }
    }
}