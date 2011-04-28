using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Concerts
{
    public class SettingsHelper
    {
        public Dictionary<String, object> settings;

        public SettingsHelper()
        {
            this.settings = new Dictionary<String, object>();
        }

        public void booleanSetting(String name, Boolean value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }else{
                settings.Add(name, value);
            }
        }

        public void stringSetting(String name, String value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }
            else
            {
                settings.Add(name, value);
            }
        }

        public void intSetting(String name, int value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }
            else
            {
                settings.Add(name, value);
            }
        }

        public void floatSetting(String name, float value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }
            else
            {
                settings.Add(name, value);
            }
        }

        public void doubleSetting(String name, double value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }
            else
            {
                settings.Add(name, value);
            }
        }

        public void objectSetting(String name, object value)
        {
            if (settings.ContainsKey(name))
            {
                settings[name] = value;
            }
            else
            {
                settings.Add(name, value);
            }
        }

        public Boolean booleanSetting(String name, out Boolean value)
        {
            value = new Boolean();
            if (settings.ContainsKey(name))
            {
                value = (Boolean)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean stringSetting(String name, out String value)
        {
            value = String.Empty;
            if (settings.ContainsKey(name))
            {
                value = (String)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean intSetting(String name, out int value)
        {
            value = 0;
            if (settings.ContainsKey(name))
            {
                value = (int)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean floatSetting(String name, out float value)
        {
            value = 0f;
            if (settings.ContainsKey(name))
            {
                value = (float)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean doubleSetting(String name, out double value)
        {
            value = 0;
            if (settings.ContainsKey(name))
            {
                value = (double)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean objectSetting(String name, out object value)
        {
            value = new object();
            if (settings.ContainsKey(name))
            {
                value = (object)settings[name];
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
