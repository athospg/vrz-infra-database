using System;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using NpgsqlTypes;
using VRZ.Infra.Database.Abstractions.Context;

namespace VRZ.Infra.Database.Postgresql
{
    public static class NpgsqlExtensions
    {
        public static void AddNpgsqlServiceCollection(this IServiceCollection services, string connectionString,
            string defaultSchema = null)
        {
            services.AddSingleton<IDatabaseContext>(new NpgsqlContext(connectionString, defaultSchema));
        }


        public static string ToNpgsqlString<TSource>(this TSource n)
        {
            return n switch
            {
                null => "NULL",
                string s => s.GetNpgsqlString(),
                Guid g => $"'{g}'",
                DateTime dateTime => $"'{dateTime.GetNpgsqlDateTime()}'",
                DateTimeOffset dateTimeOffset => $"'{dateTimeOffset.GetNpgsqlDateTime()}'",
                float v => v.ToString(NumberFormatInfo.InvariantInfo),
                double v => v.ToString(NumberFormatInfo.InvariantInfo),
                decimal v => v.ToString(NumberFormatInfo.InvariantInfo),
                Enum e => Convert.ChangeType(e, e.GetTypeCode()).ToString(),
                _ => n.ToString(),
            };
        }

        public static string GetNpgsqlString(this string s)
        {
            return string.IsNullOrWhiteSpace(s)
                ? "NULL"
                : $"'{s.Replace("\\", "\\\\").Replace("'", "\\'")}'";
        }

        /// <summary>
        ///     Converts a DateTimeOffset to a NpgsqlDateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string GetNpgsqlDateTime(this DateTimeOffset timestamp)
        {
            return new NpgsqlDateTime(timestamp.UtcDateTime).ToString();
        }

        /// <summary>
        ///     Converts a DateTime to a NpgsqlDateTime.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static string GetNpgsqlDateTime(this DateTime timestamp)
        {
            return new NpgsqlDateTime(timestamp.ToUniversalTime()).ToString();
        }
    }
}
