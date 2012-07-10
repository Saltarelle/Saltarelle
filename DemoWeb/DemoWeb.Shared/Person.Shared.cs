using System;
using System.Runtime.CompilerServices;

namespace DemoWeb {
	[Record]
	public sealed class Person {
		[PreserveCase]
		public string FirstName;

		[PreserveCase]
		public string LastName;
		
		public Person(string firstName, string lastName) {
			this.FirstName = firstName;
			this.LastName  = lastName;
		}
	}
}