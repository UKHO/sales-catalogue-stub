/*
 * Sales Catalogue Service API
 *
 * This API is for Sales Catalogue Service 
 *
 * OpenAPI spec version: 1.3
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace UKHO.SalesCatalogueStub.Api.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class ProductVersions : List<ProductVersionsInner>, IEquatable<ProductVersions>
    {
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ProductVersions {\n");
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
            return obj.GetType() == GetType() && Equals((ProductVersions)obj);
        }

        /// <summary>
        /// Returns true if ProductVersions instances are equal
        /// </summary>
        /// <param name="other">Instance of ProductVersions to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(ProductVersions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return false;
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
                return hashCode;
            }
        }

        #region Operators
#pragma warning disable 1591

        public static bool operator ==(ProductVersions left, ProductVersions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProductVersions left, ProductVersions right)
        {
            return !Equals(left, right);
        }

#pragma warning restore 1591
        #endregion Operators
    }
}
