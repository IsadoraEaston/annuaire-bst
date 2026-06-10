using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnnuaireBST.Models;

namespace AnnuaireBST.DataStructures;

public class BST<T> where T : IComparable<T>
{
    private BSTNode<T> _root;
    private Func<T, T, int> _comparateur;
    public BSTNode<T> Root => _root;

    // Constructeur par défaut — utilise CompareTo de T
    public BST()
    {
        _comparateur = (a, b) => a.CompareTo(b);
    }

    // Constructeur avec comparateur personnalisé
    public BST(Func<T, T, int> comparateur)
    {
        _comparateur = comparateur;
    }

    // Insert — insère une valeur dans l'arbre
    // Retourne true si inséré, false si doublon
    // Complexité : O(log n) en moyenne, O(n) en pire cas
    public bool Insert(T value)
    {
        if (Contains(value))
            return false;  // doublon détecté

        _root = InsertRec(_root, value);
        return true;  // insertion réussie
    }

    private BSTNode<T> InsertRec(BSTNode<T> node, T value)
    {
        if (node == null) return new BSTNode<T>(value);

        int cmp = _comparateur(value, node.Value);  // ← changement ici

        if (cmp < 0)
            node.Left = InsertRec(node.Left, value);
        else if (cmp > 0)
            node.Right = InsertRec(node.Right, value);

        return node;
    }
    // =====  AFFICHAGE CONSOLE =====
    // Affiche les contacts en ordre alphabétique (gauche, nœud, droite)
    // Complexité : O(n)
    public void InOrder()
    {
        InOrderRec(_root);
    }

    private void InOrderRec(BSTNode<T> node)
    {
        if (node == null) return;
        InOrderRec(node.Left);
        Console.WriteLine("  " + node.Value);
        InOrderRec(node.Right);
    }
    // Affiche les contacts en PreOrder (nœud, gauche, droite)
    // Complexité : O(n)
    public void PreOrder()
    {
        PreOrderRec(_root);
    }

    private void PreOrderRec(BSTNode<T> node)
    {
        if (node == null) return;
        Console.WriteLine("  " + node.Value);
        PreOrderRec(node.Left);
        PreOrderRec(node.Right);
    }
    // Affiche les contacts en PostOrder (gauche, droite, nœud)
    // Complexité : O(n)
    public void PostOrder()
    {
        PostOrderRec(_root);
    }
    private void PostOrderRec(BSTNode<T> node)
    {
        if (node == null) return;
        PostOrderRec(node.Left);
        PostOrderRec(node.Right);
        Console.WriteLine("  " + node.Value);
    }
    // Affiche les contacts niveau par niveau avec une file d'attente
    // Complexité : O(n)
    public void BFS()
    {
        if (_root == null) return;

        Queue<BSTNode<T>> file = new Queue<BSTNode<T>>();
        file.Enqueue(_root);
        int niveau = 0;

        while (file.Count > 0)
        {
            int taille = file.Count; // nb de nœuds dans ce niveau
            Console.Write($"  Niveau {niveau}: ");

            for (int i = 0; i < taille; i++)
            {
                BSTNode<T> node = file.Dequeue();
                Console.Write(node.Value + "  ");
                if (node.Left != null) file.Enqueue(node.Left);
                if (node.Right != null) file.Enqueue(node.Right);
            }

            Console.WriteLine();
            niveau++;
        }
    }    
    // Retourne la hauteur de l'arbre récursivement
    // Complexité : O(n)
    public int Height()
    {
        return HeightRec(_root);
    }

    private int HeightRec(BSTNode<T> node)
    {
        if (node == null) return 0;
        return 1 + Math.Max(HeightRec(node.Left), HeightRec(node.Right));
    }

    // Retourne le nombre de contacts dans l'arbre
    // Complexité : O(n)
    public int Count()
    {
        return CountRec(_root);
    }

    private int CountRec(BSTNode<T> node)
    {
        if (node == null) return 0;
        return 1 + CountRec(node.Left) + CountRec(node.Right);
    }

    // ===== RECHERCHE =====
    //Search + SearchRec
    //Contains
    //SearchByCity + SearchByCityRec
    //NormaliserTexte    ← ici, c'est une méthode utilitaire pour SearchByCityRec
    // Recherche un contact par nom dans l'arbre
    // Complexité : O(log n) en moyenne, O(n) en pire cas
    public BSTNode<T> Search(T value)
    {
        return SearchRec(_root, value); ;
    }

    // Vérifie si une valeur existe dans l'arbre
    // Complexité : O(log n) en moyenne
    public bool Contains(T value) => SearchRec(_root, value) != null;

    private BSTNode<T> SearchRec(BSTNode<T> node, T value)
    {
        if (node == null) return null;

        int cmp = _comparateur(value, node.Value);  // ← changement

        if (cmp == 0) return node;
        if (cmp < 0)
            return SearchRec(node.Left, value);
        else
            return SearchRec(node.Right, value);
    }

    // Recherche tous les contacts d'une ville — traversée complète InOrder
    // Complexité : O(n) — on doit visiter tous les nœuds
    public List<T> SearchByCity(string ville, Func<T, string> getCity)
    {
        List<T> resultats = new List<T>();
        SearchByCityRec(_root, ville, resultats, getCity);
        return resultats;
    }
    private void SearchByCityRec(BSTNode<T> node, string ville, List<T> resultats, Func<T, string> getCity)
    {
        if (node == null) return;
        SearchByCityRec(node.Left, ville, resultats, getCity);
        if (NormaliserTexte(getCity(node.Value)).Contains(NormaliserTexte(ville)))
            resultats.Add(node.Value);
        SearchByCityRec(node.Right, ville, resultats, getCity);
    }

    // Recherche tous les contacts avec le même nom — traversée complète
    // Complexité : O(n) — on doit visiter tous les noeuds
    public List<T> SearchByName(string nom, Func<T, string> getName)
    {
        List<T> resultats = new List<T>();
        SearchByNameRec(_root, nom, resultats, getName);
        return resultats;
    }
    private void SearchByNameRec(BSTNode<T> node, string nom, List<T> resultats, Func<T, string> getName)
    {
        if (node == null) return;
        SearchByNameRec(node.Left, nom, resultats, getName);
        if (string.Compare(getName(node.Value), nom, StringComparison.OrdinalIgnoreCase) == 0)
            resultats.Add(node.Value);
        SearchByNameRec(node.Right, nom, resultats, getName);
    }

    // Recherche un contact par numéro de téléphone — unique
    // Complexité : O(n) — on doit visiter tous les noeuds
    public List<T> SearchByPhone(string tel, Func<T, string> getPhone)
    {
        List<T> resultats = new List<T>();
        SearchByPhoneRec(_root, tel, resultats, getPhone);
        return resultats;
    }
    private void SearchByPhoneRec(BSTNode<T> node, string tel, List<T> resultats, Func<T, string> getPhone)
    {
        if (node == null) return;
        SearchByPhoneRec(node.Left, tel, resultats, getPhone);
        if (string.Compare(getPhone(node.Value), tel, StringComparison.OrdinalIgnoreCase) == 0)
            resultats.Add(node.Value);
        SearchByPhoneRec(node.Right, tel, resultats, getPhone);
    }

    // Normalise le texte : enlève les accents et met en minuscules
    // Permet la recherche insensible aux accents et à la casse
    // Complexité : O(k) où k = longueur du texte
    private string NormaliserTexte(string texte)
    {
        string normalized = texte.Normalize(NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (char c in normalized)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c)
                != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().ToLower();
    }



    // Supprime un contact par nom
    // Complexité : O(log n) en moyenne, O(n) en pire cas
    public void Delete(T value)
    {
        _root = DeleteRec(_root, value);
    }
    // ===== SUPPRESSION =====
    private BSTNode<T> DeleteRec(BSTNode<T> node, T value)
    {
        // Nœud introuvable
        if (node == null)
        {
            return null;
        }

        int cmp = _comparateur(value, node.Value);  // ← changement

        if (cmp < 0)
            node.Left = DeleteRec(node.Left, value);   // Aller à gauche
        else if (cmp > 0)
            node.Right = DeleteRec(node.Right, value); // Aller à droite
        else
        {
            // Cas 1 — feuille ou Cas 2 — un seul enfant
            if (node.Left == null) return node.Right;
            if (node.Right == null) return node.Left;

            // Cas 3 — deux enfants
            BSTNode<T> successeur = TrouverMin(node.Right);
            // Remplacer la valeur par le successeur
            node.Value = successeur.Value;
            // Supprimer le successeur dans le sous-arbre droit
            node.Right = DeleteRec(node.Right, successeur.Value);
        }
        return node;
    }
    // Trouve le nœud minimum (le plus à gauche) dans un sous-arbre
    // Complexité : O(log n)
    private BSTNode<T> TrouverMin(BSTNode<T> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }
    // ===== SAUVEGARDE CSV =====
    // Sauvegarde les contacts en ordre alphabétique (gauche, nœud, droite)
    // Complexité : O(n) — on visite chaque nœud une seule fois
    public void SaveInOrder(List<string> lignes)
    {
        SaveInOrderRec(_root, lignes);
    }
    private void SaveInOrderRec(BSTNode<T> node, List<string> lignes)
    {
        if (node == null) return;           // cas de base — stop
        SaveInOrderRec(node.Left, lignes);  // 1. aller à gauche
        lignes.Add(node.Value.ToString());  // 2. ajouter ce nœud
        SaveInOrderRec(node.Right, lignes); // 3. aller à droite
    }
    // Sauvegarde les contacts en PreOrder (nœud, gauche, droite) — préserve la forme de l'arbre
    // Complexité : O(n) — on visite chaque nœud une seule fois
    public void SavePreOrder(List<string> lignes)
    {
        SavePreOrderRec(_root, lignes);
    }
    private void SavePreOrderRec(BSTNode<T> node, List<string> lignes)
    {
        if (node == null) return;
        lignes.Add(node.Value.ToString());   // 1. ce nœud d'abord
        SavePreOrderRec(node.Left, lignes);  // 2. ensuite gauche
        SavePreOrderRec(node.Right, lignes); // 3. ensuite droite
    }
    // Sauvegarde les contacts en PostOrder (gauche, droite, nœud)
    // Complexité : O(n) — on visite chaque nœud une seule fois
    public void SavePostOrder(List<string> lignes)
    {
        SavePostOrderRec(_root, lignes);
    }
    private void SavePostOrderRec(BSTNode<T> node, List<string> lignes)
    {
        if (node == null) return;
        SavePostOrderRec(node.Left, lignes);  // 1. gauche d'abord
        SavePostOrderRec(node.Right, lignes); // 2. ensuite droite
        lignes.Add(node.Value.ToString());    // 3. nœud en dernier
    }
    // Sauvegarde les contacts niveau par niveau (BFS) avec une file d'attente
    // Complexité : O(n) — on visite chaque nœud une seule fois
    public void SaveBFS(List<string> lignes)
    {
        if (_root == null) return;

        Queue<BSTNode<T>> file = new Queue<BSTNode<T>>();
        file.Enqueue(_root);

        while (file.Count > 0)
        {
            BSTNode<T> node = file.Dequeue();
            lignes.Add(node.Value.ToString());

            if (node.Left != null) file.Enqueue(node.Left);
            if (node.Right != null) file.Enqueue(node.Right);
        }
    }

}
