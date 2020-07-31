using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper;
using Dapper.ColumnMapper;

namespace AddressBook.API.Domains
{
    [Table("Contacts")]
    public class Contact : IEquatable<Contact>
    {
        // autoincrement
        [Key]
        [Range(1, int.MaxValue)]
        [ColumnMapping("ContactId")]
        public int Id { get; set; }

        [Required]
        [ColumnMapping("ContactName")]
        public string Name { get; set; }
        [Required]
        [ColumnMapping("ContactSurname")]
        public string Surname { get; set; }
        [ColumnMapping("ContactNickname")]
        public string Nickname { get; set; }

        [Required]
        [RegularExpression("^(\\+)?[0-9]{0,15}")]
        [ColumnMapping("ContactPhoneNumber")]
        public string PhoneNumber { get; set; }

        // here entire entity but at the table Contacts we write only CountyId value
        public Country Country { get; set; }

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj == null || !ReferenceEquals(obj, this))
            {
                return false;
            }

            return Equals(obj as Contact);
        }

        public bool Equals(Contact item)
        {
            if (item == null)
            {
                return false;
            }

            bool result = true;
            result &= (Id == item.Id);
            result &= string.Compare(Name, item.Name, StringComparison.Ordinal) == 0;
            result &= string.Compare(Surname, item.Surname, StringComparison.Ordinal) == 0;
            result &= string.Compare(Nickname, item.Nickname, StringComparison.Ordinal) == 0;
            result &= string.Compare(PhoneNumber, item.PhoneNumber, StringComparison.Ordinal) == 0;
            if (item.Country != null)
            {
                result &= Country.Equals(item.Country);
            }

            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Nickname, PhoneNumber, Country?.Id);
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Surname: {Surname}, Nickname: {Nickname}, PhoneNumber: {PhoneNumber}, CountryId: {Country?.Id}";
        }

        #endregion
    }
}
