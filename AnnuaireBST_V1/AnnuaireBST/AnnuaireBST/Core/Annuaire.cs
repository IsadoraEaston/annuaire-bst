using AnnuaireBST.DataStructures;
using AnnuaireBST.Models;

namespace AnnuaireBST.Core;

public class Annuaire
{
    private BST<Contact> _arbre = new BST<Contact>();

    // Constructeur — charge les contacts au démarrage
    public Annuaire()
    {
        ChargerCSV();
    }

    // ===== CSV =====
    private void ChargerCSV()
    {
        List<Contact> contacts = CsvManager.ReadContacts();
        int doublons = 0;

        foreach (Contact c in contacts)
        {
            if (!_arbre.Insert(c))
                doublons++;
        }

        Console.WriteLine($"{_arbre.Count()} contacts chargés !");
        if (doublons > 0)
            Console.WriteLine($"  [!] {doublons} doublon(s) ignoré(s) dans le CSV !");
    }

    private void SauvegarderCSV(string ordre)
    {
        List<string> lignes = new List<string>();
        if (ordre == "PreOrder") _arbre.SavePreOrder(lignes);
        else if (ordre == "InOrder") _arbre.SaveInOrder(lignes);
        else if (ordre == "PostOrder") _arbre.SavePostOrder(lignes);
        else if (ordre == "BFS") _arbre.SaveBFS(lignes);
        CsvManager.SaveContacts(lignes);
    }

    // ===== Run — menu principal =====
    public void Run()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        bool continuer = true;

        while (continuer)
        {
            AfficherMenuPrincipal();
            string choix = Console.ReadLine()?.Trim() ?? "";

            if (choix == "1") AjouterContact();
            else if (choix == "2") RechercherContact();
            else if (choix == "3") SupprimerContact();
            else if (choix == "4") AfficherContacts();
            else if (choix == "5") AfficherHauteur();
            else if (choix == "6") RechercherParVille();
            else if (choix == "7")
            {
                Console.Clear();
                Console.WriteLine("=== AFFICHAGE GRAPHIQUE DE L'ARBRE BST ===");
                TreePrinter.AfficherArbre(_arbre.Root); 
                Console.ResetColor();
                Console.WriteLine("\nAppuyez sur ENTRÉE pour continuer...");
                Console.ReadLine();
            }
            else if (choix == "8") { _arbre = new BST<Contact>(); ChargerCSV(); }
            else if (choix == "9") SauvegarderCSV("PreOrder");
            else if (choix == "10") AfficherComplexites();
            else if (choix == "11") RechercherParTelephone();
            else if (choix == "0") continuer = Quitter();
            else Console.WriteLine("  [!] Choix invalide !");
        }

        Console.WriteLine("\nAu revoir !");
    }

    // ===== Menu principal =====
    private void AfficherMenuPrincipal()
    {
        Console.WriteLine("\n══════════════════════════════════════");
        Console.WriteLine("      ANNUAIRE DE CONTACTS (BST)      ");
        Console.WriteLine("══════════════════════════════════════");
        Console.WriteLine("   1 | Ajouter un contact");
        Console.WriteLine("   2 | Rechercher un contact");
        Console.WriteLine("   3 | Supprimer un contact");
        Console.WriteLine("   4 | Afficher les contacts (parcours)");
        Console.WriteLine("   5 | Afficher la hauteur");
        Console.WriteLine("   6 | Rechercher par ville");
        Console.WriteLine("   7 | Affichage graphique");
        Console.WriteLine("   8 | Recharger depuis le CSV");
        Console.WriteLine("   9 | Sauvegarder dans le CSV");
        Console.WriteLine("  10 | Afficher les complexités");
        Console.WriteLine("  11 | Rechercher par téléphone");
        Console.WriteLine("   0 | Quitter");
        Console.WriteLine("══════════════════════════════════════");
        Console.Write("Choix : ");
    }

    // ===== Méthodes métier =====
    private void AjouterContact()
    {
        Console.WriteLine("\n══ AJOUTER UN CONTACT ══");
        string nom = LireChamp("Nom");
        string tel = LireChampTelephone("Téléphone");
        string email = LireChampEmail("Email");
        string ville = LireChamp("Ville");

        Contact nouveau = new Contact(nom, tel, email, ville);

        // Vérifier si le contact existe déjà - doublons
        if (_arbre.Contains(nouveau))
        {
            Console.WriteLine($"  [!] Le contact '{nom}' existe déjà !");
            return;
        }

        _arbre.Insert(nouveau);
        Console.WriteLine($"  Contact '{nom}' ajouté !");
    }

    private void RechercherContact()
    {
        Console.WriteLine("\n══ RECHERCHER UN CONTACT ══");
        string nom = LireChamp("Nom à rechercher");

        List<Contact> resultats = _arbre.SearchByName(nom, c => c.Name);

        if (resultats.Count == 0)
            Console.WriteLine($"  Contact '{nom}' introuvable !");
        else
        {
            Console.WriteLine($"\n  Contact(s) trouvé(s) :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }

    // Recherche un contact par numéro de téléphone
    private void RechercherParTelephone()
    {
        Console.WriteLine("\n══ RECHERCHER PAR TÉLÉPHONE ══");
        string tel = LireChampTelephone("Téléphone à rechercher");

        List<Contact> resultats = _arbre.SearchByPhone(tel, c => c.Phone);

        if (resultats.Count == 0)
            Console.WriteLine($"  [!] Aucun contact avec le téléphone '{tel}' !");
        else
        {
            Console.WriteLine($"\n  Contact(s) trouvé(s) :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }

    private void SupprimerContact()
    {
        Console.WriteLine("\n══ SUPPRIMER UN CONTACT ══");
        string nom = LireChamp("Nom à supprimer");

        // Chercher tous les contacts avec ce nom
        List<Contact> correspondances = _arbre.SearchByName(nom, c => c.Name);

        if (correspondances.Count == 0)
        {
            Console.WriteLine($"  [!] Contact '{nom}' introuvable !");
            return;
        }
        else if (correspondances.Count == 1)
        {
            // Un seul résultat -> supprimer directement
            _arbre.Delete(correspondances[0]);
            Console.WriteLine($"  Contact '{nom}' supprimé !");
        }
        else
        {
            // Plusieurs résultats → demander le téléphone
            Console.WriteLine($"\n  Plusieurs contacts trouvés pour '{nom}' :");
            foreach (Contact c in correspondances)
                Console.WriteLine("  " + c);

            while (true)
            {
                string tel = LireChampTelephone("Téléphone pour préciser");
                Contact aSupprimer = new Contact(nom, tel, "", "");

                if (!_arbre.Contains(aSupprimer))
                {
                    Console.WriteLine($"  [!] Aucun contact '{nom}' avec ce téléphone. Réessayez.");
                    continue;
                }

                _arbre.Delete(aSupprimer);
                Console.WriteLine($"  Contact '{nom}' supprimé !");
                break;
            }
        }
    }

    private void AfficherContacts()
    {
        if (EstVide()) { Console.WriteLine("  [!] Aucun contact !"); return; }

        Console.WriteLine("\n══ AFFICHER LES CONTACTS ══");
        Console.WriteLine("  1 - InOrder (ordre alphabétique)");
        Console.WriteLine("  2 - PreOrder");
        Console.WriteLine("  3 - PostOrder");
        Console.WriteLine("  4 - BFS (par niveau)");
        Console.WriteLine("  0 - Retour");
        Console.Write("Choix : ");
        string choix = Console.ReadLine()?.Trim() ?? "";

        Console.WriteLine();
        if (choix == "1") _arbre.InOrder();
        else if (choix == "2") _arbre.PreOrder();
        else if (choix == "3") _arbre.PostOrder();
        else if (choix == "4") _arbre.BFS();
        else if (choix == "0") return;
        else Console.WriteLine("  [!] Choix invalide !");
    }

    private void AfficherHauteur()
    {
        Console.WriteLine($"\n  Hauteur de l'arbre : {_arbre.Height()}");
        Console.WriteLine($"  Nombre de contacts : {_arbre.Count()}");
    }

    private void RechercherParVille()
    {
        Console.WriteLine("\n══ RECHERCHER PAR VILLE ══");
        string ville = LireChamp("Ville");
        if (ville == null) return;

        List<Contact> resultats = _arbre.SearchByCity(ville, c => c.City); if (resultats.Count == 0)
            Console.WriteLine($"  Aucun contact à '{ville}'");
        else
        {
            Console.WriteLine($"\n  Contacts à {ville} :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }

    private void AfficherComplexites()
    {
        Console.WriteLine("\n══════════════════════════════════════════════════");
        Console.WriteLine("  COMPLEXITÉS DES OPÉRATIONS BST");
        Console.WriteLine("══════════════════════════════════════════════════");
        Console.WriteLine($"  {"Opération",-20} {"Moyenne",-15} {"Pire cas"}");
        Console.WriteLine($"  {new string('-', 45)}");
        Console.WriteLine($"  {"Insert",-20} {"O(log n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"Search",-20} {"O(log n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"Delete",-20} {"O(log n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"InOrder",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"PreOrder",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"PostOrder",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"BFS",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"Height",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine($"  {"Count",-20} {"O(n)",-15} {"O(n)"}");
        Console.WriteLine("══════════════════════════════════════════════════");
        Console.WriteLine("  n = nombre de nœuds | h = hauteur de l'arbre");

        // Mesurer les temps réels des traversées
        Benchmark.MesurerTraversees(_arbre);
    }

    private bool Quitter()
    {
        Console.Write("\n  Sauvegarder avant de quitter? (o/n) : ");
        string reponse = Console.ReadLine()?.Trim().ToLower() ?? "";

        if (reponse == "o") { SauvegarderCSV("PreOrder"); return false; }
        else if (reponse == "n") { return false; }
        else { Console.WriteLine("  [!] Entrez 'o' ou 'n'."); return true; }
    }

    // ===== Validation =====
    private bool EstVide() => _arbre.Count() == 0;

    private string LireChamp(string nomChamp)
    {
        while (true)
        {
            Console.Write($"  {nomChamp} : ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(valeur))
            {
                Console.WriteLine($"  [!] '{nomChamp}' ne peut pas être vide. Réessayez.");
                continue;
            }
            return valeur;
        }
    }

    private string LireChampEmail(string nomChamp)
    {
        while (true)
        {
            Console.Write($"  {nomChamp} : ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(valeur))
            {
                Console.WriteLine($"  [!] '{nomChamp}' ne peut pas être vide. Réessayez.");
                continue;
            }
            if (!valeur.Contains("@"))
            {
                Console.WriteLine("  [!] L'email doit contenir '@'. Réessayez.");
                continue;
            }
            return valeur;
        }
    }
    private string LireChampTelephone(string nomChamp)
    {
        while (true)
        {
            Console.Write($"  {nomChamp} : ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(valeur))
            {
                Console.WriteLine($"  [!] '{nomChamp}' ne peut pas être vide. Réessayez.");
                continue;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(valeur, @"^\d{3}-\d{3}-\d{4}$"))
            {
                Console.WriteLine("  [!] Format invalide. Utilisez xxx-xxx-xxxx. Réessayez.");
                continue;
            }
            return valeur;
        }
    }
}