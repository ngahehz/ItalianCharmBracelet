using System;
using System.Collections.Generic;

namespace ItalianCharmBracelet.Data;

public partial class SalesInvoice
{
    public string Id { get; set; } = null!;

    public DateOnly? Date { get; set; }

    public double? TotalPayment { get; set; }

    public string? VoucherId { get; set; }

    public string? Note { get; set; }

    public string? State { get; set; }

    public virtual ICollection<SalesInvoiceDetail> SalesInvoiceDetails { get; set; } = new List<SalesInvoiceDetail>();

    public virtual Voucher? Voucher { get; set; }
}
