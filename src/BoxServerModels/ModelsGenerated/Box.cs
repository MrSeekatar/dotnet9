#pragma warning disable CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
#pragma warning disable CA1307 // Specify StringComparison
// ReSharper disable RedundantUsingDirective
// ReSharper disable CheckNamespace
#nullable disable
/*
 * BoxServer
 *
 * API and objects for BoxServer
 *
 * OpenAPI spec version: 1.0.0
 *
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BoxServer.Models
{
    /// <summary>
    /// Box object for adding and updating a box
    /// </summary>
    [DataContract]
    public partial class Box : IEquatable<Box>
    {
        /// <summary>
        /// Name of box
        /// </summary>
        /// <value>Name of box</value>
        [Required]

        [DataMember(Name="name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Set by the system, the id of this box.
        /// </summary>
        /// <value>Set by the system, the id of this box.</value>

        [DataMember(Name="BoxId")]
        [JsonPropertyName("boxId")]
        [Key]
        public int? BoxId { get; set; }

        /// <summary>
        /// English internal description
        /// </summary>
        /// <value>English internal description</value>

        [DataMember(Name="description")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Set by the system when the box is created
        /// </summary>
        /// <value>Set by the system when the box is created</value>

        [DataMember(Name="createdOn")]
        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// If not active, the Box can not run.
        /// </summary>
        /// <value>If not active, the Box can not run.</value>
        [Required]

        [DataMember(Name="active")]
        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
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
            return obj.GetType() == GetType() && Equals((Box)obj);
        }

        /// <summary>
        /// Returns true if Box instances are equal
        /// </summary>
        /// <param name="other">Instance of Box to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Box other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) &&
                (
                    BoxId == other.BoxId ||
                    BoxId != null &&
                    BoxId.Equals(other.BoxId)
                ) &&
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) &&
                (
                    CreatedOn == other.CreatedOn ||
                    CreatedOn.Equals(other.CreatedOn)
                ) &&
                (
                    Active == other.Active ||
                   Active.Equals(other.Active)
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
                    if (Name != null)
                    hashCode = hashCode * 59 + Name.GetHashCode();

                    hashCode = hashCode * 59 + BoxId.GetHashCode();
                    if (Description != null)
                    hashCode = hashCode * 59 + Description.GetHashCode();
                    hashCode = hashCode * 59 + CreatedOn.GetHashCode();

                    hashCode = hashCode * 59 + Active.GetHashCode();
                return hashCode;
            }
        }

        #region Operators
        #pragma warning disable 1591

        public static bool operator ==(Box left, Box right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Box left, Box right)
        {
            return !Equals(left, right);
        }

        #pragma warning restore 1591
        #endregion Operators
    }
}

#pragma warning restore CA1834 // Consider using 'StringBuilder.Append(char)' when applicable
#pragma warning disable CA1307 // Specify StringComparison
