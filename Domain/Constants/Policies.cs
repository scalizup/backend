﻿namespace Domain.Constants;

public abstract class Policies
{
    public const string CanPurge = nameof(CanPurge);
    
    public const string NeedsTenantAccess = nameof(NeedsTenantAccess);
}