using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class ProductResponse : IEquatable<ProductResponse>
    {
        /// <summary>
        /// Gets or Sets Products
        /// </summary>
        [DataMember(Name = "products")]
        public Products Products { get; set; }

        /// <summary>
        /// Gets or Sets ProductCounts
        /// </summary>
        [DataMember(Name = "productCounts")]
        public ProductCounts ProductCounts { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductResponse {\n");
            sb.Append("  Products: ").Append(Products).Append("\n");
            sb.Append("  ProductCounts: ").Append(ProductCounts).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ProductResponse)obj);
        }

        /// <summary>
        /// Returns true if ProductResponse instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductResponse other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Products == other.Products ||
                    Products != null &&
                    Products.Equals(other.Products)
                ) &&
                (
                    ProductCounts == other.ProductCounts ||
                    ProductCounts != null &&
                    ProductCounts.Equals(other.ProductCounts)
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
                if (Products != null)
                    hashCode = hashCode * 59 + Products.GetHashCode();
                if (ProductCounts != null)
                    hashCode = hashCode * 59 + ProductCounts.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(ProductResponse left, ProductResponse right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProductResponse left, ProductResponse right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
