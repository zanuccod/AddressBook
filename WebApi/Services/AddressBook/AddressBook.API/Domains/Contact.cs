using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressBook.API.Domains
{
    [Table("Contacts")]
    public class Contact : IEquatable<Contact>
    {
        // autoincrement
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }

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
            result &= (PhoneNumber == item.PhoneNumber);

            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Surname, Nickname, PhoneNumber);
        }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Surname: {Surname}, Nickname: {Nickname}, PhoneNumber: {PhoneNumber}";
        }

        #endregion
    }
}
