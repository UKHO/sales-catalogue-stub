using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class ProductCounts : IEquatable<ProductCounts>
    {
        /// <summary>
        /// number of products explicitly requested.
        /// </summary>
        /// <value>number of products explicitly requested.</value>
        [DataMember(Name = "requestedProductCount")]
        public int? RequestedProductCount { get; set; }

        /// <summary>
        /// number of products that have data included in the produced exchange set.
        /// </summary>
        /// <value>number of products that have data included in the produced exchange set.</value>
        [DataMember(Name = "returnedProductCount")]
        public int? ReturnedProductCount { get; set; }

        /// <summary>
        /// Gets or Sets RequestedProductsAlreadyUpToDateCount
        /// </summary>
        [DataMember(Name = "requestedProductsAlreadyUpToDateCount")]
        public int? RequestedProductsAlreadyUpToDateCount { get; set; }

        /// <summary>
        /// Where a requested product is not included in the returned Exchange Set, the product will be listed in the requestedProductNotInExchangeSet portion of the response along with a reason. The reason will be one of:    *  productWithdrawn (the product has been withdrawn from the AVCS service)    *  invalidProduct (the product is not part of the AVCS Service, i.e. is an invalid or unknown ENC)    *  noDataAvailableForCancelledProduct (the product has been cancelled, and is beyond the retention period. Cancelled cells within the retention period will be returned with the cancellation data in the exchange set) 
        /// </summary>
        /// <value>Where a requested product is not included in the returned Exchange Set, the product will be listed in the requestedProductNotInExchangeSet portion of the response along with a reason. The reason will be one of:    *  productWithdrawn (the product has been withdrawn from the AVCS service)    *  invalidProduct (the product is not part of the AVCS Service, i.e. is an invalid or unknown ENC)    *  noDataAvailableForCancelledProduct (the product has been cancelled, and is beyond the retention period. Cancelled cells within the retention period will be returned with the cancellation data in the exchange set) </value>
        [DataMember(Name = "requestedProductsNotInExchangeSet")]
        public List<RequestedProductsNotInExchangeSet> RequestedProductsNotInExchangeSet { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductCounts {\n");
            sb.Append("  RequestedProductCount: ").Append(RequestedProductCount).Append("\n");
            sb.Append("  ReturnedProductCount: ").Append(ReturnedProductCount).Append("\n");
            sb.Append("  RequestedProductsAlreadyUpToDateCount: ").Append(RequestedProductsAlreadyUpToDateCount).Append("\n");
            sb.Append("  RequestedProductsNotInExchangeSet: ").Append(RequestedProductsNotInExchangeSet).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ProductCounts)obj);
        }

        /// <summary>
        /// Returns true if ProductCounts instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductCounts to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductCounts other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    RequestedProductCount == other.RequestedProductCount ||
                    RequestedProductCount != null &&
                    RequestedProductCount.Equals(other.RequestedProductCount)
                ) &&
                (
                    ReturnedProductCount == other.ReturnedProductCount ||
                    ReturnedProductCount != null &&
                    ReturnedProductCount.Equals(other.ReturnedProductCount)
                ) &&
                (
                    RequestedProductsAlreadyUpToDateCount == other.RequestedProductsAlreadyUpToDateCount ||
                    RequestedProductsAlreadyUpToDateCount != null &&
                    RequestedProductsAlreadyUpToDateCount.Equals(other.RequestedProductsAlreadyUpToDateCount)
                ) &&
                (
                    RequestedProductsNotInExchangeSet == other.RequestedProductsNotInExchangeSet ||
                    RequestedProductsNotInExchangeSet != null &&
                    RequestedProductsNotInExchangeSet.SequenceEqual(other.RequestedProductsNotInExchangeSet)
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
                if (RequestedProductCount != null)
                    hashCode = hashCode * 59 + RequestedProductCount.GetHashCode();
                if (ReturnedProductCount != null)
                    hashCode = hashCode * 59 + ReturnedProductCount.GetHashCode();
                if (RequestedProductsAlreadyUpToDateCount != null)
                    hashCode = hashCode * 59 + RequestedProductsAlreadyUpToDateCount.GetHashCode();
                if (RequestedProductsNotInExchangeSet != null)
                    hashCode = hashCode * 59 + RequestedProductsNotInExchangeSet.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(ProductCounts left, ProductCounts right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProductCounts left, ProductCounts right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
