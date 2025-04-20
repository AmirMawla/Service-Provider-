using Microsoft.AspNetCore.Http;
using SeeviceProvider_BLL.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Errors
{
    public static class VendorErrors
    {
        public static readonly Error NotFound = new("Not Found", "no Vendors found", StatusCodes.Status404NotFound);
        public static readonly Error InvalidCredentials = new("Vendor.InvalidCredentials", "Invalid Email/Password", StatusCodes.Status400BadRequest);
        public static readonly Error DuplicatedEmail = new("Vendor.DuplicatedEmail", "Email already exisit", StatusCodes.Status409Conflict);
        public static readonly Error NotApproved = new("Vendor.NotApproved", "Your account is pending admin approval.", StatusCodes.Status401Unauthorized);
        public static readonly Error EmailNotConfirmed = new("Vendor.EmailNotConfirmed", "Your account is pending admin to confirm your email before logging in.", StatusCodes.Status401Unauthorized);
        public static readonly Error NoPendingVendors = new("Vendor.NoPendingVendors", "No pending vendors exisit", StatusCodes.Status404NotFound);
        public static readonly Error ApprovedVendor = new("Vendor.ApprovedVendor", "Vendor already approved.", StatusCodes.Status409Conflict);
        public static readonly Error VendorNotRegisterdInSubCategory = new("Vendor.VendorNotRegisterdInSubCategory", "Vendor not registerd in subCategory.", StatusCodes.Status400BadRequest);
        public static readonly Error InvalidRefreshToken = new("Vendor.InvalidRefreshToken", "Invalid Refresh Token", StatusCodes.Status400BadRequest);

    }
}
