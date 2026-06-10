using AnnuaireBST.DataStructures;
using AnnuaireBST.Models;

namespace AnnuaireBST.Core;

public class Annuaire
{
    // 3 arbres BST — structure hybride
    // Arbre principal trié par nom — tiebreaker par téléphone pour gérer les homonymes
    private BST<Contact> _arbreNom = new BST<Contact>((a, b) =>
    {
        int cmp = string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        if (cmp != 0) return cmp;
        return string.Compare(a.Phone, b.Phone, StringComparison.OrdinalIgnoreCase);
    });
    // Arbre secondaire trié par ville — tiebreaker par nom pour gérer plusieurs contacts dans la même ville
    private BST<Contact> _arbreVille = new BST<Contact>((a, b) =>
    {
        int cmp = string.Compare(a.City, b.City, StringComparison.OrdinalIgnoreCase);
        if (cmp != 0) return cmp;
        int cmpNom = string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        if (cmpNom != 0) return cmpNom;
        return string.Compare(a.Phone, b.Phone, StringComparison.OrdinalIgnoreCase);
    });

    private BST<Contact> _arbreTel = new BST<Contact>((a, b) =>
        string.Compare(a.Phone, b.Phone, StringComparison.OrdinalIgnoreCase));

    // Table de hachage pour recherche O(1) par téléphone
    private HashTable<string, Contact> _hashTel = new HashTable<string, Contact>();

    // BST avec liste - gestion des doublons par liste dans chaque noeud
    private BSTListe<string, Contact> _arbreNomListe = new BSTListe<string, Contact>();

    // Constructeur — charge les contacts au démarrage
    public Annuaire()
    {
        ChargerCSV();   // insère dans _arbreNom, _arbreVille et _arbreTel
    }

    // ===== CSV =====
    private void ChargerCSV()
    {
        List<Contact> contacts = CsvManager.ReadContacts();
        int doublons = 0;

        foreach (Contact c in contacts)
        {
            if (!_arbreNom.Insert(c))
                doublons++;
            else
            {
                // Insérer aussi dans les 2 autres arbres et la table de hachage
                _arbreVille.Insert(c);
                _arbreTel.Insert(c);
                _hashTel.Add(c.Phone, c);
                _arbreNomListe.Insert(c.Name, c);
            }
        }

        Console.WriteLine($"{_arbreNom.Count()} contacts chargés !");
        if (doublons > 0)
            Console.WriteLine($"  [!] {doublons} doublon(s) ignoré(s) dans le CSV !");
    }

    private void SauvegarderCSV(string ordre)
    {
        List<string> lignes = new List<string>();
        if (ordre == "PreOrder") _arbreNom.SavePreOrder(lignes);
        else if (ordre == "InOrder") _arbreNom.SaveInOrder(lignes);
        else if (ordre == "PostOrder") _arbreNom.SavePostOrder(lignes);
        else if (ordre == "BFS") _arbreNom.SaveBFS(lignes);
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
                TreePrinter.AfficherArbre(_arbreNom.Root); 
                Console.ResetColor();
                Console.WriteLine("\nAppuyez sur ENTRÉE pour continuer...");
                Console.ReadLine();
            }
            else if (choix == "8") { _arbreNom = new BST<Contact>((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase)); ChargerCSV(); }
            else if (choix == "9") SauvegarderCSV("PreOrder");
            else if (choix == "10") AfficherComplexites();
            else if (choix == "11") RechercherParTelephone();
            else if (choix == "12") RechercherParNomListe();
            else if (choix == "0") continuer = Quitter();
            else if (!string.IsNullOrWhiteSpace(choix))
                Console.WriteLine("  [!] Choix invalide !");
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
        Console.WriteLine("  12 | Rechercher par nom (BST/Liste)");
        Console.WriteLine("   0 | Quitter");
        Console.WriteLine("══════════════════════════════════════");
        Console.Write("Choix : ");
    }

    // ===== Méthodes métier =====
    private void AjouterContact()
    {
        Console.WriteLine("\n══ AJOUTER UN CONTACT ══");
        string nom = LireChamp("Nom");
        if (nom == null) return;  // ← annuler
        string tel = LireChampTelephone("Téléphone");
        if (tel == null) return;  // ← annuler
        string email = LireChampEmail("Email");
        if (email == null) return;  // ← annuler
        string ville = LireChamp("Ville");
        if (ville == null) return;  // ← annuler

        Contact nouveau = new Contact(nom, tel, email, ville);

        // Vérifier si le contact existe déjà - doublons
        if (_arbreNom.Contains(nouveau))
        {
            Console.WriteLine($"  [!] Le contact '{nom}' existe déjà !");
            return;
        }

        // Insérer dans les 3 arbres
        _arbreNom.Insert(nouveau);
        _arbreVille.Insert(nouveau);
        _arbreTel.Insert(nouveau);
        // Insérer dans la table de hachage
        _hashTel.Add(nouveau.Phone, nouveau);
        // Insérer dans le BST/Liste
        _arbreNomListe.Insert(nouveau.Name, nouveau);
        Console.WriteLine($"  Contact '{nom}' ajouté !");
    }

    private void RechercherContact()
    {
        Console.WriteLine("\n══ RECHERCHER UN CONTACT ══");
        string nom = LireChamp("Nom à rechercher");

        List<Contact> resultats = _arbreNom.SearchByName(nom, c => c.Name);

        if (resultats.Count == 0)
            Console.WriteLine($"  Contact '{nom}' introuvable !");
        else
        {
            Console.WriteLine($"\n  Contact(s) trouvé(s) :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }
    private void RechercherParVille()
    {
        Console.WriteLine("\n══ RECHERCHER PAR VILLE ══");
        string ville = LireChamp("Ville");
        if (ville == null) return;

        List<Contact> resultats = _arbreVille.SearchByCity(ville, c => c.City); if (resultats.Count == 0)
            Console.WriteLine($"  Aucun contact à '{ville}'");
        else
        {
            Console.WriteLine($"\n  Contacts à {ville} :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }


    // Recherche un contact par numéro de téléphone
    private void RechercherParTelephone()
    {
        Console.WriteLine("\n══ RECHERCHER PAR TÉLÉPHONE ══");
        string tel = LireChampTelephone("Téléphone à rechercher");
        if (tel == null) return;

        // Utilise la Hash Table → O(1) !
        Contact resultat = _hashTel.Get(tel);

        if (resultat == null)
            Console.WriteLine($"  [!] Aucun contact avec le téléphone '{tel}' !");
        else
            Console.WriteLine($"  Contact trouvé : {resultat}");
    }

    // Recherche par nom dans le BST/Liste — retourne tous les contacts avec ce nom
    private void RechercherParNomListe()
    {
        Console.WriteLine("\n══ RECHERCHER PAR NOM (BST/Liste) ══");
        string nom = LireChamp("Nom à rechercher");
        if (nom == null) return;

        List<Contact> resultats = _arbreNomListe.Search(nom);

        if (resultats == null || resultats.Count == 0)
            Console.WriteLine($"  [!] Aucun contact '{nom}' trouvé !");
        else
        {
            Console.WriteLine($"\n  {resultats.Count} contact(s) trouvé(s) :");
            foreach (Contact c in resultats)
                Console.WriteLine("  " + c);
        }
    }

    private void SupprimerContact()
    {
        Console.WriteLine("\n══ SUPPRIMER UN CONTACT ══");
        string nom = LireChamp("Nom à supprimer");

        // Chercher tous les contacts avec ce nom
        List<Contact> correspondances = _arbreNom.SearchByName(nom, c => c.Name);

        if (correspondances.Count == 0)
        {
            Console.WriteLine($"  [!] Contact '{nom}' introuvable !");
            return;
        }
        else if (correspondances.Count == 1)
        {
            // Un seul résultat -> supprimer dans les 3 arbres
            _arbreNom.Delete(correspondances[0]);
            _arbreVille.Delete(correspondances[0]);
            _arbreTel.Delete(correspondances[0]);
            // Supprimer de la table de hachage
            _hashTel.Remove(correspondances[0].Phone);
            _arbreNomListe.Delete(correspondances[0].Name, correspondances[0]);
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

                if (!_arbreNom.Contains(aSupprimer))
                {
                    Console.WriteLine($"  [!] Aucun contact '{nom}' avec ce téléphone. Réessayez.");
                    continue;
                }
                // Récupérer le vrai contact avec toutes ses données
                Contact vraiContact = _arbreNom.Search(aSupprimer).Value;

                // Supprimer dans les 3 arbres avec le vrai contact
                _arbreNom.Delete(vraiContact);
                _arbreVille.Delete(vraiContact);
                _arbreTel.Delete(vraiContact);
                // Supprimer de la table de hachage
                _hashTel.Remove(vraiContact.Phone);
                _arbreNomListe.Delete(vraiContact.Name, vraiContact);
                Console.WriteLine($"  Contact '{nom}' supprimé !");
                break;
            }
        }
    }

    private void AfficherContacts()
    {
        if (_arbreNom.Count() == 0)
        {
            Console.WriteLine("  [!] Aucun contact !");
            return;
        }

        Console.WriteLine("\n══ AFFICHER LES CONTACTS ══");
        Console.WriteLine("  1 - InOrder par nom (alphabétique)");
        Console.WriteLine("  2 - InOrder par ville");
        Console.WriteLine("  3 - InOrder par téléphone");
        Console.WriteLine("  4 - PreOrder (structure)");
        Console.WriteLine("  5 - PostOrder");
        Console.WriteLine("  6 - BFS (par niveau)");
        Console.WriteLine("  0 - Retour");
        Console.Write("Choix : ");
        string choix = Console.ReadLine()?.Trim() ?? "";

        Console.WriteLine();
        if (choix == "1") _arbreNom.InOrder();
        else if (choix == "2") _arbreVille.InOrder();
        else if (choix == "3") _arbreTel.InOrder();
        else if (choix == "4") _arbreNom.PreOrder();
        else if (choix == "5") _arbreNom.PostOrder();
        else if (choix == "6") _arbreNom.BFS();
        else if (choix == "0") return;
        else Console.WriteLine("  [!] Choix invalide !");
    }

    private void AfficherHauteur()
    {
        Console.WriteLine($"\n  Hauteur de l'arbre : {_arbreNom.Height()}");
        Console.WriteLine($"  Nombre de contacts : {_arbreNom.Count()}");
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
        Benchmark.MesurerTraversees(_arbreNom);
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
    private bool EstVide() => _arbreNom.Count() == 0;

    private string LireChamp(string nomChamp)
    {
        while (true)
        {
            Console.Write($"  {nomChamp} (0 pour annuler) : ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (valeur == "0") return null; // annuler

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
            Console.Write($"  {nomChamp} (0 pour annuler): ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (valeur == "0") return null;

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
            Console.Write($"  {nomChamp} (0 pour annuler): ");
            string valeur = Console.ReadLine()?.Trim() ?? "";

            if (valeur == "0") return null;

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