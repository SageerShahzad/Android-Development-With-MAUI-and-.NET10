using ClassifiedAds.Mobile.ViewModels;
using System.Globalization;

namespace GloboTicket.Admin.Mobile.Converters;

public class StatusEnumToTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not AdStatusEnum status)
        {
            return string.Empty;
        }

        return status switch
        {
            AdStatusEnum.OnSale => "On sale",
            AdStatusEnum.AlmostSoldOut => "Almost sold out",
            AdStatusEnum.SalesClosed => "Ticket sale is closed",
            AdStatusEnum.Cancelled => "Cancelled",
            _ => string.Empty
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
