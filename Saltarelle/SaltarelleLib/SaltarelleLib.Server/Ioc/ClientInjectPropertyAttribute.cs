using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle.Ioc {
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ClientInjectPropertyAttribute : Attribute {
        public Type PropertyType { get; set; }
        public string PropertyName { get; set; }

        public ClientInjectPropertyAttribute(Type propertyType, string propertyName) {
            this.PropertyType = propertyType;
            this.PropertyName = propertyName;
        }
    }
}
