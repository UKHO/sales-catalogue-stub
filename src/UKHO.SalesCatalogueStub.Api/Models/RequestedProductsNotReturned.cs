using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class RequestedProductsNotReturned : IEquatable<RequestedProductsNotReturned>
    {
        /// <summary>
        /// Gets or Sets ProductName
        /// </summary>
        [Required]
        [DataMember(Name = "productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or Sets Reason
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public enum ReasonEnum
        {
            /// <summary>
            /// Enum ProductWithdrawnEnum for productWithdrawn
            /// </summary>
            [EnumMember(Value = "productWithdrawn")]
            ProductWithdrawnEnum = 0,
            /// <summary>
            /// Enum InvalidProductEnum for invalidProduct
            /// </summary>
            [EnumMember(Value = "invalidProduct")]
            InvalidProductEnum = 1,
            /// <summary>
            /// Enum NoDataAvailableForCancelledProductEnum for noDataAvailableForCancelledProduct
            /// </summary>
            [EnumMember(Value = "noDataAvailableForCancelledProduct")]
            NoDataAvailableForCancelledProductEnum = 2,
            /// <summary>
            /// Enum DuplicateProductEnum for duplicateProduct
            /// </summary>
            [EnumMember(Value = "duplicateProduct")]
            DuplicateProductEnum = 3

        }

        /// <summary>
        /// Gets or Sets Reason
        /// </summary>
        [Required]
        [DataMember(Name = "reason")]
        public ReasonEnum? Reason { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RequestedProductsNotReturned {\n");
            sb.Append("  ProductName: ").Append(ProductName).Append("\n");
            sb.Append("  Reason: ").Append(Reason).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((RequestedProductsNotReturned)obj);
        }

        /// <summary>
        /// Returns true if RequestedProductsNotReturned instances are equal
        /// </summary>
        /// <param name="other">Instance of RequestedProductsNotReturned to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RequestedProductsNotReturned other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    ProductName == other.ProductName ||
                    ProductName != null &&
                    ProductName.Equals(other.ProductName)
                ) &&
                (
                    Reason == other.Reason ||
                    Reason != null &&
                    Reason.Equals(other.Reason)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;
                // Suitable nullity checks etc, of course :)
                if (ProductName != null)
                    hashCode = hashCode * 59 + ProductName.GetHashCode();
                if (Reason != null)
                    hashCode = hashCode * 59 + Reason.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(RequestedProductsNotReturned left, RequestedProductsNotReturned right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RequestedProductsNotReturned left, RequestedProductsNotReturned right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
