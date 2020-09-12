using System;
using System.ComponentModel.DataAnnotations;

namespace IPChecker
{
	public class AddressInfo
	{		
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public DateTime Time { get; set; }

		public string Address6 { get; set; }

		public string Address4 { get; set; }
		public string Version { get; set; }
	}
}
