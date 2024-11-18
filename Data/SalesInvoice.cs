using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class SalesInvoice
{
    public string Id { get; set; } = null!;

    public string? CustomerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Cell { get; set; }

    public DateOnly? Date { get; set; }

    public double? TotalPayment { get; set; }

    public string? VoucherId { get; set; }

    public string? Note { get; set; }

    public string? PaymentMethod { get; set; }

    public string? StateId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();

    public virtual State? State { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
