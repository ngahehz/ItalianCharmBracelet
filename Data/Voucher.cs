using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class Voucher
{
    public string Id { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Discription { get; set; }

    public double? DiscountAmount { get; set; }

    public double? MinInvoiceValue { get; set; }

    public double? PercentDiscount { get; set; }

    public double? MaxDiscount { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Note { get; set; }

    public int? State { get; set; }

    public virtual ICollection<SalesInvoice> SalesInvoices { get; set; } = new List<SalesInvoice>();
}
