namespace poojaPathBooking.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreatePujaBookingDto
{
    [Required(ErrorMessage = "PujaTypeId is required")]
    public int PujaTypeId { get; set; }

    [Required(ErrorMessage = "CustomerId is required")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "BookingMode is required")]
    [RegularExpression("^(Online|Offline|Phone)$", ErrorMessage = "BookingMode must be Online, Offline, or Phone")]
    public string BookingMode { get; set; } = string.Empty;

    [Required(ErrorMessage = "PujaDate is required")]
    [DataType(DataType.Date)]
    public DateOnly PujaDate { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? PujaTime { get; set; }

    [Required(ErrorMessage = "PeopleCount is required")]
    [Range(1, 1000, ErrorMessage = "PeopleCount must be between 1 and 1000")]
    public int PeopleCount { get; set; }

    [StringLength(int.MaxValue)]
    public string? Note { get; set; }

    [Required(ErrorMessage = "TotalAmount is required")]
    [Range(0, 9999999999.99, ErrorMessage = "TotalAmount must be a valid amount")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(10)]
    [RegularExpression("^(INR|USD|EUR|GBP)$", ErrorMessage = "Currency must be INR, USD, EUR, or GBP")]
    public string Currency { get; set; } = "INR";
}

public class UpdatePujaBookingDto
{
    [Required(ErrorMessage = "PujaTypeId is required")]
    public int PujaTypeId { get; set; }

    [Required(ErrorMessage = "BookingMode is required")]
    [RegularExpression("^(Online|Offline|Phone)$", ErrorMessage = "BookingMode must be Online, Offline, or Phone")]
    public string BookingMode { get; set; } = string.Empty;

    [Required(ErrorMessage = "PujaDate is required")]
    [DataType(DataType.Date)]
    public DateOnly PujaDate { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly? PujaTime { get; set; }

    [Required(ErrorMessage = "PeopleCount is required")]
    [Range(1, 1000, ErrorMessage = "PeopleCount must be between 1 and 1000")]
    public int PeopleCount { get; set; }

    [StringLength(int.MaxValue)]
    public string? Note { get; set; }

    [Required(ErrorMessage = "BookingStatus is required")]
    [RegularExpression("^(Pending|Confirmed|Completed|Cancelled)$", ErrorMessage = "BookingStatus must be Pending, Confirmed, Completed, or Cancelled")]
    public string BookingStatus { get; set; } = "Pending";

    [Required(ErrorMessage = "TotalAmount is required")]
    [Range(0, 9999999999.99, ErrorMessage = "TotalAmount must be a valid amount")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(10)]
    [RegularExpression("^(INR|USD|EUR|GBP)$", ErrorMessage = "Currency must be INR, USD, EUR, or GBP")]
    public string Currency { get; set; } = "INR";

    [Required(ErrorMessage = "IsPaid is required")]
    public bool IsPaid { get; set; }
}

public class UpdateBookingStatusDto
{
    [Required(ErrorMessage = "BookingStatus is required")]
    [RegularExpression("^(Pending|Confirmed|Completed|Cancelled)$", ErrorMessage = "BookingStatus must be Pending, Confirmed, Completed, or Cancelled")]
    public string BookingStatus { get; set; } = string.Empty;
}

public class UpdatePaymentStatusDto
{
    [Required(ErrorMessage = "IsPaid is required")]
    public bool IsPaid { get; set; }
}

public class PujaBookingResponseDto
{
    public int BookingId { get; set; }
    public int PujaTypeId { get; set; }
    public string? PujaTypeName { get; set; }
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerContact { get; set; }
    public string BookingMode { get; set; } = string.Empty;
    public DateOnly PujaDate { get; set; }
    public TimeOnly? PujaTime { get; set; }
    public int PeopleCount { get; set; }
    public string? Note { get; set; }
    public string BookingStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
