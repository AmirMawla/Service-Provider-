﻿using Microsoft.AspNetCore.Http;
using SeeviceProvider_BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Errors
{
    public static class OrderErrors
    {
        public static readonly Error OrderNotFound = new("Not Found", "Order does not exist", StatusCodes.Status404NotFound);

        public static readonly Error OrderCreationFaild = new("Failure", "Order creation failed", StatusCodes.Status400BadRequest);
        public static readonly Error NoOrdersForThisVendor = new("NoOrdersForThisVendor", "No orders found for this vendor.", StatusCodes.Status404NotFound);
        public static readonly Error UserNotOrderOwner = new("NotAuthorized", "User is not the order owner.", StatusCodes.Status403Forbidden);
        public static readonly Error VendorNotAssociatedWithThisOrder = new("NotAuthorized", "Vendor is not associated with this order.", StatusCodes.Status403Forbidden);
        public static readonly Error PaymentProcessingFailed = new("Failure", "This payment hasn't been successeded.", StatusCodes.Status400BadRequest);
        public static readonly Error ShippingNotFound = new("Shipping Not Found", "No shippings were found for this order .", StatusCodes.Status404NotFound);
       // public static readonly Error OrderNotFound = new( "Order.NotFound","Order not found with specified ID",StatusCodes.Status404NotFound);

        public static readonly Error NoProductsForVendor = new( "Order.NoVendorProducts", "No products in this order belong to your vendor account", StatusCodes.Status404NotFound);
    }
}
