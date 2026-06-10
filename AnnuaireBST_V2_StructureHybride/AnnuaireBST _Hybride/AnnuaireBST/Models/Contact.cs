using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnuaireBST.Models;

public class Contact : IComparable<Contact>
{
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string City { get; set; }

    public Contact(string name, string phone, string email, string city)
    {
        Name = name;
        Phone = phone;
        Email = email;
        City = city;

    }

    public int CompareTo(Contact other)
    {
        int cmpNom = string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase);

        // Si les noms sont différents, on compare par nom
        if (cmpNom != 0) return cmpNom;

        // Si les noms sont identiques, on compare par téléphone
        return string.Compare(this.Phone, other.Phone, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return $"{Name},{Phone},{Email},{City}";
    }
    public string AfficherContact()
    {
        return $"{Name} | {Phone} | {Email} | {City}";
    }
}
