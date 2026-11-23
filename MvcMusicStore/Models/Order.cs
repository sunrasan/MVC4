using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MvcMusicStore.Models
{
    /// <summary>
    /// Order model - updated to use Microsoft.AspNetCore.Mvc [Bind] attribute
    /// instead of the legacy System.Web.Mvc [Bind(Include = "...")] syntax.
    /// </summary>
    [Bind("FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email")]
    public class Order
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }

        [ScaffoldColumn(false)]
        public System.DateTime OrderDate { get; set; }

        [ScaffoldColumn(false)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [System.ComponentModel.DisplayName("First Name")]
        [StringLength(160)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [System.ComponentModel.DisplayName("Last Name")]
        [StringLength(160)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(70, MinimumLength = 3)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string State { get; set; } = string.Empty;

        [Required]
        [System.ComponentModel.DisplayName("Postal Code")]
        [StringLength(10, MinimumLength = 5)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [System.ComponentModel.DisplayName("Email Address")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessage = "Email is not valid.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
