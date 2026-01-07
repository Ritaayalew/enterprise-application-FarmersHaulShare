using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SharedKernel.Domain;

namespace IdentityAndAccessManagement.Domain.Roles
{
    public record Role
    {
        public string Value { get; }

        // Predefined valid roles
        public static readonly Role Farmer = new("farmer");
        public static readonly Role Driver = new("driver");
        public static readonly Role Coordinator = new("coordinator");
        public static readonly Role Buyer = new("buyer");

        public static readonly IReadOnlyCollection<Role> All =
            new ReadOnlyCollection<Role>(new List<Role>
            {
                Farmer,
                Driver,
                Coordinator,
                Buyer
            });

        // Constructor ensures normalization and validation
        public Role(string value)
        {
            Value = value.ToLowerInvariant();

            if (!All.Any(r => r.Value == Value))
            {
                throw new DomainException(
                    $"Invalid role: '{value}'. Valid roles are: {string.Join(", ", All.Select(r => r.Value))}");
            }
        }

        public override string ToString() => Value;

        public string DisplayName => Value switch
        {
            "farmer" => "Farmer",
            "driver" => "Driver",
            "coordinator" => "Coordinator",
            "buyer" => "Buyer",
            _ => Value
        };
    }
}