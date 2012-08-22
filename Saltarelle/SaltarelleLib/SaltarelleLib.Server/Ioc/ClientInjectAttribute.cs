using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saltarelle.Ioc {
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ClientInjectAttribute : Attribute {
    }
}
