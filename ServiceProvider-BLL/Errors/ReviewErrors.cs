using Microsoft.AspNetCore.Http;
using SeeviceProvider_BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Errors
{
    public static class ReviewErrors
    {
        public static readonly Error HasNotOrdered = new("Bad Request", "You can only review services you have purchased and completed", StatusCodes.Status400BadRequest);
        public static readonly Error ShipmentNotDelivered = new Error("Review.ShipmentNotDelivered", "Item not delivered yet. You can only review delivered items.", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicatedReview = new("Duplicated Review", "You have already submitted a review for this service", StatusCodes.Status409Conflict);
        public static readonly Error ReviewsNotFound = new("Not Found", "No reviews on this product yet", StatusCodes.Status404NotFound);
        public static readonly Error VendorReviewsNotFound = new("Not Found", "No reviews for this vendor", StatusCodes.Status404NotFound);
        public static readonly Error ReviewNotFound = new("Not Found", "This does't exisit ", StatusCodes.Status404NotFound);
        public static readonly Error Forbid = new("Forbid", "User does NOT own this review", StatusCodes.Status403Forbidden);

    }
}
