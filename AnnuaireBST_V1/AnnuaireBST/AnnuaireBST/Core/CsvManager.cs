using AnnuaireBST.Models;
using System.Text.RegularExpressions;

namespace AnnuaireBST.Core;

public static class CsvManager
{
    private static readonly string filePath = "contacts.csv";

    // Lecture CSV → Liste de contacts
    // Complexité : O(n)
    public static List<Contact> ReadContacts()
    {
        List<Contact> contacts = new List<Contact>();

        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            Console.WriteLine("  [!] Fichier CSV introuvable — fichier vide créé.");
            return contacts;
        }

        string[] lignes = File.ReadAllLines(filePath);

        if (lignes.Length == 0)
        {
            Console.WriteLine("  [!] Fichier CSV vide — aucun contact chargé.");
            return contacts;
        }

        int ligneNum = 0;
        foreach (string ligne in lignes)
        {
            ligneNum++;

            if (string.IsNullOrWhiteSpace(ligne))
            {
                Console.WriteLine($"  [!] Ligne {ligneNum} ignorée — ligne vide.");
                continue;
            }

            string[] parts = ligne.Split(',');

            if (parts.Length != 4)
            {
                Console.WriteLine($"  [!] Ligne {ligneNum} ignorée — format invalide : '{ligne}'");
                continue;
            }

            string nom = parts[0].Trim();
            string tel = parts[1].Trim();
            string email = parts[2].Trim();
            string ville = parts[3].Trim();

            // Nom obligatoire
            if (string.IsNullOrWhiteSpace(nom))
            {
                Console.WriteLine($"  [!] Ligne {ligneNum} ignorée — nom manquant.");
                continue;
            }

            // Avertissements pour champs optionnels
            if (string.IsNullOrWhiteSpace(tel))
                Console.WriteLine($"  [!] Avertissement ligne {ligneNum} — téléphone manquant pour '{nom}'");
            else if (!Regex.IsMatch(tel, @"^\d{3}-\d{3}-\d{4}$"))
                Console.WriteLine($"  [!] Avertissement ligne {ligneNum} — format téléphone invalide pour '{nom}' : '{tel}'");

            if (string.IsNullOrWhiteSpace(email))
                Console.WriteLine($"  [!] Avertissement ligne {ligneNum} — email manquant pour '{nom}'");

            if (string.IsNullOrWhiteSpace(ville))
                Console.WriteLine($"  [!] Avertissement ligne {ligneNum} — ville manquante pour '{nom}'");

            contacts.Add(new Contact(nom, tel, email, ville));
        }

        return contacts;
    }

    // Sauvegarde Liste de contacts → CSV
    // Complexité : O(n)
    public static void SaveContacts(List<string> lignes)
    {
        File.WriteAllLines(filePath, lignes);
        Console.WriteLine("  Contacts sauvegardés avec succès !");
    }
}