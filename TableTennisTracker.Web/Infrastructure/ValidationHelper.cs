using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace TableTennisTracker.Web.Infrastructure;

public static class ValidationHelper
{
    public static void EnsureNotEmptyGuid(ModelStateDictionary modelState, string fieldName, Guid value, string message)
    {
        if (value == Guid.Empty)
        {
            modelState.AddModelError(fieldName, message);
        }
    }
}
