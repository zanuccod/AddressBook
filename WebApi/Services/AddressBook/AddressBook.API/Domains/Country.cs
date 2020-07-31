using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBook.API.Domains
{
    [Table("Countries")]
    public class Country : IEquatable<Country>
    {
        // autoincrement
        [Key]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string ISOCode { get; set; }

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj == null || !ReferenceEquals(obj, this))
            {
                return false;
            }

            return Equals(obj as Country);
        }

        public bool Equals(Country item)
        {
            if (item == null)
            {
                return false;
            }

            bool result = true;
            result &= (Id == item.Id);
            result &= string.Compare(Name, item.Name, StringComparison.Ordinal) == 0;
            result &= string.Compare(ISOCode, item.ISOCode, StringComparison.Ordinal) == 0;

            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, ISOCode);
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, ISOCode: {ISOCode}";
        }

        #endregion
    }
}
