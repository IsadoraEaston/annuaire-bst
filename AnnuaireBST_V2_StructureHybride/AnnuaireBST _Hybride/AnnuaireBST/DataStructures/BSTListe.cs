using AnnuaireBST.Models;

namespace AnnuaireBST.DataStructures;

/// <summary>
/// BST dont chaque noeud contient une liste de valeurs
/// Permet de gerer les doublons de cle — meme cle, valeurs differentes
/// Complexite : Insert O(log n), Search O(log n), Delete O(log n)
/// </summary>
public class BSTListe<K, V> where K : IComparable<K>
{
    private BSTNodeListe<K, V> _root;

    // Insert — ajoute une valeur dans la liste du noeud correspondant
    // Si le noeud n'existe pas, il est cree avec une nouvelle liste
    // Complexite : O(log n)
    public void Insert(K cle, V valeur)
    {
        _root = InsertRec(_root, cle, valeur);
    }

    private BSTNodeListe<K, V> InsertRec(BSTNodeListe<K, V> node, K cle, V valeur)
    {
        // Cas de base : noeud vide -> creer un nouveau noeud avec la liste
        if (node == null) return new BSTNodeListe<K, V>(cle, valeur);

        int cmp = cle.CompareTo(node.Cle);

        if (cmp < 0)
            node.Left = InsertRec(node.Left, cle, valeur);
        else if (cmp > 0)
            node.Right = InsertRec(node.Right, cle, valeur);
        else
            // Meme cle -> ajouter dans la liste du noeud existant !
            node.Valeurs.Add(valeur);

        return node;
    }

    // Search — retourne la liste de valeurs pour une cle
    // Complexite : O(log n)
    public List<V> Search(K cle)
    {
        return SearchRec(_root, cle);
    }

    private List<V> SearchRec(BSTNodeListe<K, V> node, K cle)
    {
        if (node == null) return null;

        int cmp = cle.CompareTo(node.Cle);

        if (cmp == 0) return node.Valeurs;
        if (cmp < 0) return SearchRec(node.Left, cle);
        else return SearchRec(node.Right, cle);
    }

    // Delete — supprime une valeur specifique de la liste
    // Si la liste devient vide, le noeud est supprime
    // Complexite : O(log n)
    public void Delete(K cle, V valeur)
    {
        _root = DeleteRec(_root, cle, valeur);
    }

    private BSTNodeListe<K, V> DeleteRec(BSTNodeListe<K, V> node, K cle, V valeur)
    {
        if (node == null) return null;

        int cmp = cle.CompareTo(node.Cle);

        if (cmp < 0)
            node.Left = DeleteRec(node.Left, cle, valeur);
        else if (cmp > 0)
            node.Right = DeleteRec(node.Right, cle, valeur);
        else
        {
            // Supprimer la valeur de la liste
            node.Valeurs.Remove(valeur);

            // Si la liste est vide -> supprimer le noeud
            if (node.Valeurs.Count == 0)
            {
                if (node.IsLeaf) return null;
                if (node.Left == null) return node.Right;
                if (node.Right == null) return node.Left;

                // Cas 3 — deux enfants
                BSTNodeListe<K, V> successeur = TrouverMin(node.Right);
                node.Cle = successeur.Cle;
                node.Valeurs = successeur.Valeurs;
                node.Right = DeleteRec(node.Right, successeur.Cle, default);
            }
        }
        return node;
    }

    private BSTNodeListe<K, V> TrouverMin(BSTNodeListe<K, V> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    // InOrder — affiche tous les contacts de tous les noeuds
    // Complexite : O(n)
    public void InOrder()
    {
        InOrderRec(_root);
    }

    private void InOrderRec(BSTNodeListe<K, V> node)
    {
        if (node == null) return;
        InOrderRec(node.Left);
        foreach (V valeur in node.Valeurs)
            Console.WriteLine("  " + valeur);
        InOrderRec(node.Right);
    }

    // Count — retourne le nombre total de valeurs
    // Complexite : O(n)
    public int Count()
    {
        return CountRec(_root);
    }

    private int CountRec(BSTNodeListe<K, V> node)
    {
        if (node == null) return 0;
        return node.Valeurs.Count + CountRec(node.Left) + CountRec(node.Right);
    }
}