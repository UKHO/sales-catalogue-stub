using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class ProductsInner : IEquatable<ProductsInner>
    {
        /// <summary>
        /// The unique product identifiers
        /// </summary>
        /// <value>The unique product identifiers</value>
        [DataMember(Name = "productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// The edition number
        /// </summary>
        /// <value>The edition number</value>
        [DataMember(Name = "editionNumber")]
        public decimal? EditionNumber { get; set; }

        /// <summary>
        /// an array of update numbers
        /// </summary>
        /// <value>an array of update numbers</value>
        [DataMember(Name = "updateNumber")]
        public List<decimal?> UpdateNumber { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductsInner {\n");
            sb.Append("  ProductName: ").Append(ProductName).Append("\n");
            sb.Append("  EditionNumber: ").Append(EditionNumber).Append("\n");
            sb.Append("  UpdateNumber: ").Append(UpdateNumber).Append("\n");
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
            return obj.GetType() == GetType() && Equals((ProductsInner)obj);
        }

        /// <summary>
        /// Returns true if ProductsInner instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductsInner to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductsInner other)
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
                    EditionNumber == other.EditionNumber ||
                    EditionNumber != null &&
                    EditionNumber.Equals(other.EditionNumber)
                ) &&
                (
                    UpdateNumber == other.UpdateNumber ||
                    UpdateNumber != null &&
                    UpdateNumber.SequenceEqual(other.UpdateNumber)
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
                if (EditionNumber != null)
                    hashCode = hashCode * 59 + EditionNumber.GetHashCode();
                if (UpdateNumber != null)
                    hashCode = hashCode * 59 + UpdateNumber.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(ProductsInner left, ProductsInner right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProductsInner left, ProductsInner right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
