using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OMPS
{
#if false
    public partial class JobNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string strValue && !Regex.IsMatch(strValue, @"^[JS]\d{9}$"))
            {
                return new ValidationResult(
                    ErrorMessage ??
                    "Job number must start with an J or S and be followed by all 9 digits."
                );
            } else if (value is not string)
            {
                return new ValidationResult(
                    ErrorMessage ??
                    "Parameter must be a string"
                );
            }
            return ValidationResult.Success;
        }
    }

    public class PositiveNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int intValue && intValue < 0)
            {
                return new ValidationResult(ErrorMessage ?? "Value must be positive.");
            }
            else if (value is not int)
            {
                return new ValidationResult(
                    ErrorMessage ??
                    "Parameter must be an int"
                );
            }
            return ValidationResult.Success;
        }
    }

    public interface IHostObjectApi
    {
        public Task<string?> GetRecentOrders(int limit);
        public Task<string?> GetOrderColorSet(string job, int limit = 0);
        public Task<string?> GetOrderData(string job, int limit = 0);
        public Task<string?> GetJobOrderLines(string job, int limit = 0);
    }

    [ComVisible(true)]
    public class HostObjectApi: IHostObjectApi
    {

        public static readonly int MAX_LIMIT = int.MaxValue;
        public async Task<string?> GetRecentOrders(
            [PositiveNumberAttribute]
            int limit = 0) =>
            JsonSerializer.Serialize(await GlobalObjects.SqlMethods.GetRecentOrders(limit));

        public async Task<string?> GetOrderColorSet(
            [JobNumberAttribute]
            string job,
            [PositiveNumberAttribute]
            int limit = 0) =>
            JsonSerializer.Serialize(await GlobalObjects.SqlMethods.GetOrderColorSet(job, limit));

        public async Task<string?> GetOrderData(
            [JobNumberAttribute]
            string job,
            [PositiveNumberAttribute]
            int limit = 0) =>
            JsonSerializer.Serialize(await GlobalObjects.SqlMethods.GetOrderData(job, limit));

        public async Task<string?> GetJobOrderLines(
            [JobNumberAttribute]
            string job,
            [PositiveNumberAttribute]
            int limit = 0) =>
            JsonSerializer.Serialize(await GlobalObjects.SqlMethods.GetJobOrderLines(job, limit));
    }
#endif
}
