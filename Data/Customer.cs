using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class Customer
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Address { get; set; }

    public string? Cell { get; set; }

    public string Email { get; set; } = null!;

    public string? Img { get; set; }

    public string Role { get; set; } = null!;

    public string State { get; set; } = null!;

    public string? RandomKey { get; set; }

    public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
