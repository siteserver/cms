using System;

namespace SS.CMS.Abstractions.Enums
{
    public sealed class TaxisType
    {
        public static readonly TaxisType OrderById = new TaxisType("OrderById");
        public static readonly TaxisType OrderByIdDesc = new TaxisType("OrderByIdDesc");
        public static readonly TaxisType OrderByChannelId = new TaxisType("OrderByChannelId");
        public static readonly TaxisType OrderByChannelIdDesc = new TaxisType("OrderByChannelIdDesc");
        public static readonly TaxisType OrderByAddDate = new TaxisType("OrderByAddDate");
        public static readonly TaxisType OrderByAddDateDesc = new TaxisType("OrderByAddDateDesc");
        public static readonly TaxisType OrderByLastEditDate = new TaxisType("OrderByLastEditDate");
        public static readonly TaxisType OrderByLastEditDateDesc = new TaxisType("OrderByLastEditDateDesc");
        public static readonly TaxisType OrderByTaxis = new TaxisType("OrderByTaxis");
        public static readonly TaxisType OrderByTaxisDesc = new TaxisType("OrderByTaxisDesc");
        public static readonly TaxisType OrderByHits = new TaxisType("OrderByHits");
        public static readonly TaxisType OrderByHitsByDay = new TaxisType("OrderByHitsByDay");
        public static readonly TaxisType OrderByHitsByWeek = new TaxisType("OrderByHitsByWeek");
        public static readonly TaxisType OrderByHitsByMonth = new TaxisType("OrderByHitsByMonth");
        public static readonly TaxisType OrderByRandom = new TaxisType("OrderByRandom");

        private TaxisType(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static TaxisType Parse(string val)
        {
            if (string.Equals(OrderById.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderById;
            }
            if (string.Equals(OrderByIdDesc.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByIdDesc;
            }
            if (string.Equals(OrderByChannelId.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByChannelId;
            }
            if (string.Equals(OrderByChannelIdDesc.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByChannelIdDesc;
            }
            if (string.Equals(OrderByAddDate.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByAddDate;
            }
            if (string.Equals(OrderByAddDateDesc.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByAddDateDesc;
            }
            if (string.Equals(OrderByLastEditDate.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByLastEditDate;
            }
            if (string.Equals(OrderByLastEditDateDesc.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByLastEditDateDesc;
            }
            if (string.Equals(OrderByTaxis.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByTaxis;
            }
            if (string.Equals(OrderByTaxisDesc.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByTaxisDesc;
            }
            if (string.Equals(OrderByHits.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByHits;
            }
            if (string.Equals(OrderByHitsByDay.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByHitsByDay;
            }
            if (string.Equals(OrderByHitsByWeek.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByHitsByWeek;
            }
            if (string.Equals(OrderByHitsByMonth.Value, val, StringComparison.OrdinalIgnoreCase))
            {
                return OrderByHitsByMonth;
            }

            return OrderByTaxisDesc;
        }
    }
}
