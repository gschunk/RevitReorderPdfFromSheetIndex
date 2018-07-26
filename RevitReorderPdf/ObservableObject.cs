/*
   Copyright (C) 2018 Pheinex LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace RevitReorderPdf
{
    [DataContract(IsReference = true)]
    abstract public class ObservableObject : INotifyPropertyChanged
    {
        private ISet<string> propertyNames;

        protected bool ThrowOnInvalidPropertyName { get { return true; } }

        protected ISet<string> PropertyNames
        {
            get
            {
                if (propertyNames == null)
                {
                    propertyNames = new HashSet<string>();
                    foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        propertyNames.Add(property.Name);
                    }

                }
                return propertyNames;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            if (propertyName == "") return;

            if (!PropertyNames.Contains(propertyName))
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool SetValue<T>(ref T field, T newValue, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(propertyName);

                Debug.Print("Property {0} changed to {1} on {2}", propertyName, newValue, this.ToString());

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}