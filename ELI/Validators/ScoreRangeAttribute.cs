using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ELI.Validators
{
    /*** Not currently used. Leaving as example ***/
    public class ScoreRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] allowedValues = { "5-", "5", "5+", "4-", "4", "4+", "3-", "3", "3+", "2-", "2", "2+", "1-", "1", "1+" };
            if (Array.IndexOf(allowedValues, value.ToString().Trim()) > -1)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("This score value must be in the range of 1- to 5+.");
        }
    }
}
