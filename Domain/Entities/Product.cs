﻿using Domain.Entities.Common;

namespace Domain.Entities;

public class Product(
    int tenantId,
    string name)  : TenantAware(tenantId)
{
    public string Name { get; set; } = name;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public required List<Tag> Tags { get; set; }
}