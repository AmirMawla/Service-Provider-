using Microsoft.AspNetCore.Http;
using SeeviceProvider_BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Errors;
public class BannerErrors
{
    public static readonly Error NoOffersForThisVendor = new("vendor.NoOffers", "No Offers  belong to your vendor account", StatusCodes.Status404NotFound);
    public static readonly Error DontoWnThisProduct = new("vendor.DontoWnThisProduct", "You Dont Own This Product", StatusCodes.Status400BadRequest);
}
